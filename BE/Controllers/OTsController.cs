using BE.Data.Contexts;
using BE.Data.Dtos.OTDtos;
using BE.Data.Enum;
using BE.Data.Enum.OTEnums;
using BE.Data.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BE.Services.TokenServices;
using ClosedXML.Excel;
using ClosedXML.Extensions;
using DocumentFormat.OpenXml.Bibliography;

namespace BE.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "permission_group: True module: ots")]
    public class OTsController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;

        public OTsController(AppDbContext context, TokenService tokenService)
        {
            _context = context;
            _tokenService = tokenService;
        }

        private async Task<bool> checkToken(HttpContext httpContext)
        {
            string token = httpContext.Request.Headers["Authorization"];
            if (token == null)
                return false;
            token = token.Substring("Bearer ".Length).Trim();
            var response = _tokenService.Decode(token);
            var user = await _context.Users.Where(user => user.id == response.id && user.userCode == response.userCode && user.IdGroup == response.group).SingleOrDefaultAsync();
            if (user == null)
                return false;
            return true;
        }


        [HttpGet]
        [Route("getByType/{type}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetByType(Types type)
        {
            var ots = await _context.OTs.Where(ot => ot.type == type).ToListAsync();
            if (ots == null)
                return NotFound();
            return Ok(ots);
        }


        [HttpGet]
        [Route("getPageTotal")]
        public ActionResult getPage(Types type, int recordNum)
        {
            var count = _context.OTs.Count(ot => ot.type == type);
            var pageCount = Math.Ceiling(count / Convert.ToDecimal(recordNum));
            return Ok((int)pageCount);
        }


        // Pagination
        [HttpPost]
        [Route("getPaginate")]
        public async Task<ActionResult> GetAll(PaginateOT paginate)
        {
            try
            {
                if (paginate.recordNum == 0)
                    paginate.recordNum = 10;
                if (paginate.page == 0)
                    paginate.page = 1;

                var ots = await _context.OTs.Where(ot => ot.type.Equals(paginate.type)).OrderByDescending(ot => ot.dateCreate).Skip((paginate.page - 1) * paginate.recordNum)
                    .Take(paginate.recordNum).ToListAsync();
                if (ots == null)
                    return NotFound();
                if (paginate.recordNum == 0)
                    paginate.recordNum = 10;
                var pageCount = Math.Ceiling(ots.Count() / Convert.ToDecimal(paginate.recordNum));
                var OTs = ots
                    .Skip((paginate.page - 1) * paginate.recordNum)
                    .Take(paginate.recordNum).ToList();
                var response = new OTresponse
                {
                    OTs = OTs,
                    CurrentPage = paginate.page,
                    Pages = (int)pageCount
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }

        [HttpGet]
        [Route("getOTByUser/{userId}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetByUser(int userId)
        {
            var ot = await _context.OTs.Where(o => o.user == userId).ToListAsync();
            if (ot == null)
                return NotFound();
            return Ok(ot);
        }


        [HttpGet]
        [Route("getOTByLead/{leadId}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetById(int leadId)
        {
            var ot = await _context.OTs.Where(o => o.leadCreate == leadId).ToListAsync();
            if (ot == null)
                return NotFound();
            return Ok(ot);
        }


        [HttpGet]
        [Route("getOTByID/{id}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetByLead(int id)
        {
            var ot = await _context.OTs.Where(o => o.id == id).SingleOrDefaultAsync();
            if (ot == null)
                return NotFound();
            return Ok(ot);
        }


        [HttpGet]
        [Route("getByStatus/{status}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetByStatus(StatusOT status)
        {
            var OTs = await _context.OTs.Where(o => o.status == status).ToListAsync();
            if (OTs == null)
                return NoContent();
            return Ok(OTs);
        }


        [HttpGet]
        [Route("getOTByProject/{proId}")]
        public async Task<ActionResult<IEnumerable<OTs>>> GetByProject(int proId)
        {
            var list = from x in _context.OTs
                       join c in _context.Projects on x.idProject equals c.Id
                       join f in _context.Users on x.leadCreate equals f.id
                       join q in _context.Users on x.updateUser equals q.id
                       join d in _context.Users on x.user equals d.id
                       where (x.idProject == proId)
                       select new
                       {
                           x,
                           c.Name,
                           nameLead = f.id == x.leadCreate  ? f.FullName : null,
                           nameUser = x.user == d.id ? d.FullName : null,
                           nameUserUpdate = q.id == x.updateUser  ? q.FullName : null,
                       };
            
            if (list == null)
                return NoContent();
            return Ok(list);
        }


        [HttpPost]
        [Route("createOT")]
        //[Authorize(Roles = "admin,lead")]
        [Authorize(Roles = "modules: ots add: 1")]
        public async Task<IActionResult> Create(OTs OTs)
        {
            var ots = await _context.OTs.Where(ot => ot.Date == OTs.Date && ot.user == OTs.user).SingleOrDefaultAsync();
            if (ots != null && ots.status != StatusOT.deleted)
            {
                TimeOnly startCurrent = TimeOnly.Parse(ots.start);
                TimeOnly endCurrent = TimeOnly.Parse(ots.end);
                TimeOnly startCreate = TimeOnly.Parse(OTs.start);
                TimeOnly endCreate = TimeOnly.Parse(OTs.end);
                if ((startCurrent >= endCreate) || (endCurrent <= startCreate))
                {
                    await _context.OTs.AddAsync(OTs);
                    _context.SaveChanges();
                    return Ok(OTs);
                }
                else return BadRequest();
            }
            await _context.OTs.AddAsync(OTs);
            _context.SaveChanges();
            return Ok(OTs);
        }


        [HttpPut]
        [Route("acceptOT")]
        //[Authorize(Roles = "admin,pm")]
        [Authorize(Roles = "modules: ots confirm: 1")]
        public async Task<IActionResult> Update(AcceptOTDto dto)
        {
            var ot = await _context.OTs.Where(o => o.id == dto.id).SingleOrDefaultAsync();
            if (ot == null)
                return NoContent();
            if (dto.status == StatusOT.deny)
                ot.note = dto.note;
            ot.dateUpdate = DateTime.UtcNow;
            ot.updateUser = dto.PM;
            ot.status = dto.status;
            _context.SaveChanges();
            return Ok(ot);
        }


        [HttpPut]
        [Route("updateOT/{id}")]
        //[Authorize(Roles = "admin,lead")]
        [Authorize(Roles = "modules: ots update: 1")]
        public async Task<IActionResult> Update(int id, EditOTDto dto)
        {
            try
            {
                var OTs = await _context.OTs.Where(o => o.id == id).SingleOrDefaultAsync();
                if (OTs == null)
                    return NotFound();
                if (OTs.status == StatusOT.accepted)
                    return BadRequest();
                OTs.Date = dto.Date;
                OTs.start = dto.start;
                OTs.end = dto.end;
                OTs.realTime = dto.realTime;
                OTs.user = dto.user;
                OTs.idProject = dto.idProject;
                OTs.description = dto.description;
                OTs.dateUpdate = DateTime.UtcNow;
                OTs.updateUser = dto.updateUser;
                OTs.status = StatusOT.process;
                OTs.note = "";
                _context.SaveChanges();
                return Ok(OTs);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPut]
        [Route("deleteOT")]
        //[Authorize(Roles = "admin,pm")]
        [Authorize(Roles = "modules: ots delete: 1")]
        public async Task<IActionResult> Accept(int idOT, int PM)
        {
            var ot = await _context.OTs.Where(o => o.id == idOT).SingleOrDefaultAsync();
            if (ot == null)
                return NoContent();
            ot.dateUpdate = DateTime.UtcNow;
            ot.updateUser = PM;
            ot.status = StatusOT.deleted;
            _context.SaveChanges();
            return Ok(ot);
        }


        [HttpGet]
        [Route("getAccept/{type}")]
        public async Task<ActionResult<IEnumerable<OTs>>> getAccept(Types type)
        {
            return await _context.OTs.Where(ot => ot.type == type && ot.status == 0).ToListAsync();
        }


        [HttpGet("exportExcelFollowRole/{month}/{year}/{idproject}/{idrole}/{iduser}")]
        //[Authorize(Roles = "admin,pm,lead,sample,staff")]
        [Authorize(Roles = "module: ots export: 1")]
        public async Task<IActionResult> exportExcelFollowRole(int? month = 0,int? year = 0,int? idproject = 0,int? idrole = 0,int? iduser = 0)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sheet1");

            if(idrole == 0 || iduser == 0)
            {
                return BadRequest("please input IdUser and IdGroup");
            }
            else
            {

                // Admin PM
                if (idrole == 1 || idrole == 5)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };


                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");

                    }

                    if (idproject == 0 && month == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " +f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject =c.Name,
                                   };


                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }

                    if(idproject == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.Date.Month == month && x.Date.Year == year)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };
                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }



                    if (month == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };

                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }
                }
                //Sample
                if (idrole == 2)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.status == StatusOT.accepted)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };


                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");

                    }

                    if (idproject == 0 && month == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where x.status == StatusOT.accepted
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };


                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }

                    if (idproject == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.Date.Month == month && x.Date.Year == year && x.status == StatusOT.accepted)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };
                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }



                    if (month == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.status == StatusOT.accepted)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };

                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }
                }
                // Lead
                if (idrole == 3)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && c.Leader == iduser)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };


                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");

                    }

                    if (idproject == 0 && month == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where  c.Leader == iduser
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };


                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }

                    if (idproject == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.Date.Month == month && x.Date.Year == year && c.Leader == iduser)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };
                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }

                    if (month == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && c.Leader == iduser)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };

                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }
                }
                //STAFF
                if (idrole == 4)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.user == iduser)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };


                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");

                    }

                    if (idproject == 0 && month == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where x.user == iduser
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };


                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }

                    if (idproject == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.Date.Month == month && x.Date.Year == year && x.user == iduser)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };
                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }



                    if (month == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.user == iduser)
                                   select new
                                   {
                                       id = x.id,
                                       type = x.type,
                                       Date = x.Date,
                                       start = x.start,
                                       end = x.end,
                                       realTime = x.realTime,
                                       status = x.status,
                                       description = x.description,
                                       leadCreate = f.lastName + " " + f.firstName,
                                       dateCreate = x.dateCreate,
                                       updateUser = q.lastName + " " + q.firstName,
                                       dateUpdate = x.dateUpdate,
                                       note = x.note,
                                       user = d.lastName + " " + d.firstName,
                                       idProject = c.Name,
                                   };

                        if (list == null)
                            return NoContent();
                        // get column name for header
                        var columns_name = typeof(ExportOT).GetProperties()
                                    .Select(property => property.Name)
                                    .ToArray();
                        // table header
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var cell = ws.Cell(1, idx + 1);
                            cell.Value = columns_name[idx];
                        }
                        // table data
                        ws.Cells("A2").Value = list;
                        // Apply style to excel
                        for (int idx = 0; idx < columns_name.Length; idx++)
                        {
                            var col = ws.Column(idx + 1);
                            col.AdjustToContents();
                        }
                        // border for table
                        IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(list.Count() + 1, columns_name.Length).Address);
                        data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                        data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
                        // save file to excel folder
                        wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
                        return Ok("Excel\\OTs_Table.xlsx");
                    }
                }
            }

            return BadRequest("Something went wrong !");
        }

        
        [HttpGet]
        [Route("exportExcel/month={month}&year={year}&idProject={idProject}")]
        //[Authorize(Roles = "admin,pm,lead,sample,staff")]
        [Authorize(Roles = "modules: ots export: 1")]
        public async Task<string> DownloadFile(int? month = 0, int? year = 0, int? idProject = 0)
        {
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sheet1");
            var _data = await (from o in _context.OTs
                               from p in _context.Projects.Where(p => p.Id == o.idProject).DefaultIfEmpty()
                               from u1 in _context.Users.Where(u1 => u1.id == o.leadCreate).DefaultIfEmpty()
                               from u2 in _context.Users.Where(u2 => u2.id == o.updateUser).DefaultIfEmpty()
                               from u3 in _context.Users.Where(u3 => u3.id == o.user).DefaultIfEmpty()
                               select new
                               {
                                   id = o.id,
                                   type = o.type,
                                   Date = o.Date,
                                   start = o.start,
                                   end = o.end,
                                   realTime = o.realTime,
                                   status = o.status,
                                   description = o.description,
                                   leadCreate = u1.lastName + " " + u1.firstName,
                                   dateCreate = o.dateCreate,
                                   updateUser = u2.lastName + " " + u2.firstName,
                                   dateUpdate = o.dateUpdate,
                                   note = o.note,
                                   user = u3.lastName + " " + u3.firstName,
                                   idProject = p.Name,
                               }).ToListAsync();
            // get data from DB
            if (idProject == 0 && month != 0)
            {
                _data = await (from o in _context.OTs
                               from p in _context.Projects.Where(p => p.Id == o.idProject).DefaultIfEmpty()
                               from u1 in _context.Users.Where(u1 => u1.id == o.leadCreate).DefaultIfEmpty()
                               from u2 in _context.Users.Where(u2 => u2.id == o.updateUser).DefaultIfEmpty()
                               from u3 in _context.Users.Where(u3 => u3.id == o.user).DefaultIfEmpty()
                               select new
                               {
                                   id = o.id,
                                   type = o.type,
                                   Date = o.Date,
                                   start = o.start,
                                   end = o.end,
                                   realTime = o.realTime,
                                   status = o.status,
                                   description = o.description,
                                   leadCreate = u1.lastName + " " + u1.firstName,
                                   dateCreate = o.dateCreate,
                                   updateUser = u2.lastName + " " + u2.firstName,
                                   dateUpdate = o.dateUpdate,
                                   note = o.note,
                                   user = u3.lastName + " " + u3.firstName,
                                   idProject = p.Name,
                               }).Where(o => o.Date.Month == month && o.Date.Year == year).ToListAsync();
            }
            else if (month == 0 && idProject != 0)
            {
                _data = await (from o in _context.OTs
                               from p in _context.Projects.Where(p => p.Id == o.idProject).DefaultIfEmpty()
                               from u1 in _context.Users.Where(u1 => u1.id == o.leadCreate).DefaultIfEmpty()
                               from u2 in _context.Users.Where(u2 => u2.id == o.updateUser).DefaultIfEmpty()
                               from u3 in _context.Users.Where(u3 => u3.id == o.user).DefaultIfEmpty()
                               select new
                               {
                                   id = o.id,
                                   type = o.type,
                                   Date = o.Date,
                                   start = o.start,
                                   end = o.end,
                                   realTime = o.realTime,
                                   status = o.status,
                                   description = o.description,
                                   leadCreate = u1.lastName + " " + u1.firstName,
                                   dateCreate = o.dateCreate,
                                   updateUser = u2.lastName + " " + u2.firstName,
                                   dateUpdate = o.dateUpdate,
                                   note = o.note,
                                   user = u3.lastName + " " + u3.firstName,
                                   idProject = p.Name,
                               }).ToListAsync();
            }
            else if (month != 0 && idProject != 0)
            {
                _data = await (from o in _context.OTs
                               from p in _context.Projects.Where(p => p.Id == o.idProject && o.idProject == idProject).DefaultIfEmpty()
                               from u in _context.Users.Where(u => u.id == o.leadCreate || u.id == o.updateUser || u.id == o.user).DefaultIfEmpty()
                               select new
                               {
                                   id = o.id,
                                   type = o.type,
                                   Date = o.Date,
                                   start = o.start,
                                   end = o.end,
                                   realTime = o.realTime,
                                   status = o.status,
                                   description = o.description,
                                   leadCreate = u.lastName + " " + u.firstName,
                                   dateCreate = o.dateCreate,
                                   updateUser = u.lastName + " " + u.firstName,
                                   dateUpdate = o.dateUpdate,
                                   note = o.note,
                                   user = u.lastName + " " + u.firstName,
                                   idProject = p.Name,
                               }).Where(o => o.Date.Month == month && o.Date.Year == year).ToListAsync();
            }
            else if (idProject == 0 && month == 0)
            {

            }
            // get column name for header
            var columns_name = typeof(OTs).GetProperties()
                        .Select(property => property.Name)
                        .ToArray();
            // table header
            for (int idx = 0; idx < columns_name.Length; idx++)
            {
                var cell = ws.Cell(1, idx + 1);
                cell.Value = columns_name[idx];
            }
            // table data
            ws.Cells("A2").Value = _data;
            // Apply style to excel
            for (int idx = 0; idx < columns_name.Length; idx++)
            {
                var col = ws.Column(idx + 1);
                col.AdjustToContents();
            }
            // border for table
            IXLRange data_range = ws.Range(ws.Cell(1, 1).Address, ws.Cell(_data.Count() + 1, columns_name.Length).Address);
            data_range.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
            data_range.Style.Border.InsideBorder = XLBorderStyleValues.Thin;
            // save file to excel folder
            wb.SaveAs("..\\FE\\Excel\\OTs_Table.xlsx");
            return "Excel\\OTs_Table.xlsx";
        }


        // For PM
        [HttpGet("getOTsByidPM/{IdPM}")]
        public ActionResult GetOTsByIdPM(int IdPM)
        {
            try
            {
                
                var list = from x in _context.OTs
                           join c in _context.Projects on x.idProject equals c.Id
                           join f in _context.Users on x.leadCreate equals f.id
                           join q in _context.Users on x.updateUser equals q.id
                           join d in _context.Users on x.user equals d.id
                           select new
                           {
                               x,
                               c.Name,
                               nameLead = f.id == x.leadCreate ? f.FullName : null,
                               nameUser = x.user == d.id ? d.FullName : null,
                               nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                               dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                               note = x.note
                           };
                return Ok(list);
            }catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }


        // For sample
        [HttpGet("GetAllOTs")]
        public ActionResult GetAllOTs()
        {
            try
            {
                var list = (from x in _context.OTs
                            join c in _context.Projects on x.idProject equals c.Id
                            join f in _context.Users on x.leadCreate equals f.id
                            join q in _context.Users on x.updateUser equals q.id
                            join d in _context.Users on x.user equals d.id
                            select new
                            {
                                x,
                                c.Name,
                                nameLead = f.id == x.leadCreate ? f.FullName : null,
                                nameUser = x.user == d.id ? d.FullName : null,
                                nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                note = x.note
                            }).ToList();
                return Ok(list);
            } catch(Exception ex)
            {
                return BadRequest(ex);
            }
        }


        // For staff
        [HttpGet("GetAllOTsByStaff/{idstaff}")]
        public ActionResult GetAllOTsByStaff(int idstaff)
        {
            try
            {
                var list = from x in _context.OTs
                           join c in _context.Projects on x.idProject equals c.Id
                           join f in _context.Users on x.leadCreate equals f.id
                           join q in _context.Users on x.updateUser equals q.id
                           join d in _context.Users on x.user equals d.id
                           where (x.user == idstaff)
                           select new
                           {
                               x,
                               c.Name,
                               nameLead = f.id == x.leadCreate ? f.FullName : null,
                               nameUser = x.user == d.id ? d.FullName : null,
                               nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                               dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                               note = x.note

                           };
                return Ok(list);
            }
            catch(Exception ex)
            {
                return BadRequest(ex);
            }
            
        }


        // For LEAD
        [HttpGet("GetAllOTsByLead/{IdLead}")]
        public ActionResult GetAllOTsByLead(int IdLead)
        {
            try
            {
                var list = (from x in _context.OTs
                           join c in _context.Projects on x.idProject equals c.Id
                           join f in _context.Users on x.leadCreate equals f.id
                           join q in _context.Users on x.updateUser equals q.id
                           join d in _context.Users on x.user equals d.id
                           where (x.leadCreate == IdLead)
                           select new
                           {
                               x,
                               c.Name,
                               nameLead = f.id == x.leadCreate ? f.FullName : null,
                               nameUser = x.user == d.id ? d.FullName : null,
                               nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                               dateUpdate =x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                               note = x.note
                           }).ToList();            
                return Ok(list);
            }catch(Exception ex)
            {
                return BadRequest(ex);
            }   
        }


        [HttpGet]
        [Route("getOTByMonth/month={month}&year={year}")]
        public IActionResult GetByMonth(int month, int year)
        {

            var list = from x in _context.OTs
                       join c in _context.Projects on x.idProject equals c.Id
                       join f in _context.Users on x.leadCreate equals f.id
                       join q in _context.Users on x.updateUser equals q.id
                       join d in _context.Users on x.user equals d.id
                       where (x.Date.Month == month && x.Date.Year == year)
                       select new
                       {
                           x,
                           c.Name,
                           nameLead = f.id == x.leadCreate ? f.FullName : null,
                           nameUser = x.user == d.id ? d.FullName : null,
                           nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                       };



            if (list == null)
                return NoContent();
            return Ok(list);
        }

        [HttpGet("filterByRole/{month}/{year}/{idproject}/{idrole}")]
        public async Task<ActionResult> filterByRole(int? month = 0,int? year = 0,int? idproject = 0,int? idrole = 0,int? iduser =0)
        {
            if (idrole == 0 || iduser == 0)
            {
                return Ok("Please input Idrole or IdUser");
            }
            else
            {
                // Admin
                if (idrole == 1 || idrole == 5)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };


                        if (list == null)
                            return NoContent();
                        return Ok(list);

                    }

                    if (idproject == 0 && month != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.Date.Month == month && x.Date.Year == year)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };
                        if (list == null)
                            return NoContent();
                        return Ok(list);
                    }

                    if (month == 0 && idproject !=0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };

                        if (list == null)
                            return NoContent();
                        return Ok(list);
                    }
                }
                //Sample
                if (idrole == 2)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.status == StatusOT.accepted)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };


                        if (list == null)
                            return NoContent();
                        return Ok(list);

                    }

                    if (idproject == 0 && month != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.Date.Month == month && x.Date.Year == year && x.status== StatusOT.accepted)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };
                        if (list == null)
                            return NoContent();
                        return Ok(list);
                    }

                    if (month == 0 && idproject != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.status == StatusOT.accepted)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };

                        if (list == null)
                            return NoContent();
                        return Ok(list);
                    }
                }
                //Lead
                if(idrole == 3)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.leadCreate == iduser)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };


                        if (list == null)
                            return NoContent();
                        return Ok(list);

                    }

                    if (idproject == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.Date.Month == month && x.Date.Year == year && x.leadCreate == iduser)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };
                        if (list == null)
                            return NoContent();
                        return Ok(list);
                    }

                    if (month == 0 )
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.leadCreate == iduser)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };

                        if (list == null)
                            return NoContent();
                        return Ok(list);
                    }
                }
                // Staff
                if(idrole == 4)
                {
                    if (month != 0 && year != 0 && idproject != 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.Date.Month == month && x.Date.Year == year && x.user == iduser)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };


                        if (list == null)
                            return NoContent();
                        return Ok(list);

                    }

                    if (idproject == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.Date.Month == month && x.Date.Year == year && x.user == iduser)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };
                        if (list == null)
                            return NoContent();
                        return Ok(list);
                    }

                    if (month == 0)
                    {
                        var list = from x in _context.OTs
                                   join c in _context.Projects on x.idProject equals c.Id
                                   join f in _context.Users on x.leadCreate equals f.id
                                   join q in _context.Users on x.updateUser equals q.id
                                   join d in _context.Users on x.user equals d.id
                                   where (x.idProject == idproject && x.leadCreate == iduser)
                                   select new
                                   {
                                       x,
                                       c.Name,
                                       nameLead = f.id == x.leadCreate ? f.FullName : null,
                                       nameUser = x.user == d.id ? d.FullName : null,
                                       nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                       dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                       note = x.note
                                   };

                        if (list == null)
                            return NoContent();
                        return Ok(list);
                    }
                }   
                return BadRequest("some thing went wrong");
            }  
        }


        [HttpGet]
        [Route("GetByMonthAndProject/month={month}&year={year}&idProject={idProject}")]
        public async Task<ActionResult> GetByMonthAndProject(int? month = 0, int? year = 0, int? idProject = 0)
        {
            try
            {
                if (idProject == 0)
                {
                    var list = from x in _context.OTs
                               join c in _context.Projects on x.idProject equals c.Id
                               join f in _context.Users on x.leadCreate equals f.id
                               join q in _context.Users on x.updateUser equals q.id
                               join d in _context.Users on x.user equals d.id
                               where (x.Date.Month == month && x.Date.Year == year)
                               select new
                               {
                                   x,
                                   c.Name,
                                   nameLead = f.id == x.leadCreate ? f.FullName : null,
                                   nameUser = x.user == d.id ? d.FullName : null,
                                   nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                   dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                   note = x.note
                               };
                    if (list == null)
                        return NoContent();
                    return Ok(list);

                }
                else if (month == 0)
                {

                    var list = from x in _context.OTs
                               join c in _context.Projects on x.idProject equals c.Id
                               join f in _context.Users on x.leadCreate equals f.id
                               join q in _context.Users on x.updateUser equals q.id
                               join d in _context.Users on x.user equals d.id
                               where (x.idProject == idProject)
                               select new
                               {
                                   x,
                                   c.Name,
                                   nameLead = f.id == x.leadCreate ? f.FullName : null,
                                   nameUser = x.user == d.id ? d.FullName : null,
                                   nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                   dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                   note = x.note
                               };

                        if (list == null)
                        return NoContent();
                    return Ok(list);
                }
                else if (month != 0 && idProject != 0)
                {
                    var list = from x in _context.OTs
                               join c in _context.Projects on x.idProject equals c.Id
                               join f in _context.Users on x.leadCreate equals f.id
                               join q in _context.Users on x.updateUser equals q.id
                               join d in _context.Users on x.user equals d.id
                               where (x.idProject == idProject && x.Date.Month == month && x.Date.Year == year)
                               select new
                               {
                                   x,
                                   c.Name,
                                   nameLead = f.id == x.leadCreate ? f.FullName : null,
                                   nameUser = x.user == d.id ? d.FullName : null,
                                   nameUserUpdate = q.id == x.updateUser ? q.FullName : null,
                                   dateUpdate = x.dateUpdate == x.dateCreate ? null : x.dateUpdate,
                                   note = x.note
                               };


                    if (list == null)
                        return NoContent();
                    return Ok(list);
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex);
            }
        }


        [HttpPost("AddOTs")]
        //[Authorize(Roles = "admin,lead")]
        [Authorize(Roles = "modules: ots add: 1")]
        public async Task<IActionResult> AddOTs(AddRangeOTs OTs)
        {
            try
            {            
               var listadd = new List<OTs>();
               OTs.user.ForEach(ele =>
               {
                  listadd.Add(new OTs()
                  {
                    Date = OTs.Date,
                    start = OTs.start,
                    end = OTs.end,
                    realTime = OTs.realTime,
                    user = ele,
                    idProject = OTs.idProject,
                    description = OTs.description,
                    updateUser = OTs.leadCreate,
                    dateCreate = OTs.dateCreate,
                    dateUpdate = OTs.dateUpdate,
                    status = OTs.status,
                    leadCreate = OTs.leadCreate,
                    note = OTs.note,
                    type = 0,
                  });
               });

               await _context.OTs.AddRangeAsync(listadd);
               _context.SaveChanges();
               return Ok("Success");
                    
            }
            catch(Exception ex)
            {
                return BadRequest("failed");
            }
        }
      

    }
}

