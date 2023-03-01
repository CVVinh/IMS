using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos;
using BE.Data.Dtos.ProjectDtos;
using BE.Data.Models;
using BE.Services.PaginationServices;
using BE.Services.TokenServices;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;


namespace BE.Controllers
{
	[Route("api/[controller]")]
	[ApiController]
    [Authorize(Roles = "permission_group: True module: project")]
    public class ProjectController : ControllerBase
	{

		private readonly AppDbContext _context;
		private readonly TokenService _tokenService;
		private readonly IPaginationServices<Projects> _paginationServices;
		private readonly IMapper _mapper;
		public ProjectController(AppDbContext context, IPaginationServices<Projects> paginationServices, TokenService tokenService, IMapper mapper)
		{
			_context = context;
			_paginationServices = paginationServices;
			_tokenService = tokenService;
			_mapper = mapper;
		}
		// GET: ProjectController

		// PM, sample
		[HttpGet("getAllProject")]
		public ActionResult getAllProject()
		{
			try
			{
				var dsProject = from x in _context.Projects
								join k in _context.Users on x.Leader equals k.id
								where x.IsDeleted == false
								select new
								{
									id = x.Id,
									dateCreated = x.DateCreated,
									dateUpdate = x.DateUpdate,
									description = x.Description,
									endDate = x.EndDate,
									isDeleted = x.IsDeleted,
									isFinished = x.IsFinished,
									isOnGitlab = x.IsOnGitlab,
									leader = k.FullName,
									name = x.Name,
									projectCode = x.ProjectCode,
									startDate = x.StartDate,
									userCreated = x.UserCreated,
									userId = x.UserId,
									userUpdate = x.UserUpdate
								};

                return Ok(dsProject);
			}
			catch (Exception ew)
			{
				return BadRequest(ew);
			}
		}

        [HttpGet("getAllProjectRunning")]
        public ActionResult getAllProjectRunning()
        {
            try
            {
                var dsProject = from x in _context.Projects
                                join k in _context.Users on x.Leader equals k.id
                                where x.IsDeleted == false && x.IsFinished == false
                                select new
                                {
                                    id = x.Id,
                                    dateCreated = x.DateCreated,
                                    dateUpdate = x.DateUpdate,
                                    description = x.Description,
                                    endDate = x.EndDate,
                                    isDeleted = x.IsDeleted,
                                    isFinished = x.IsFinished,
                                    isOnGitlab = x.IsOnGitlab,
                                    leader = k.FullName,
                                    name = x.Name,
                                    projectCode = x.ProjectCode,
                                    startDate = x.StartDate,
                                    userCreated = x.UserCreated,
                                    userId = x.UserId,
                                    userUpdate = x.UserUpdate
                                };

                return Ok(dsProject);
            }
            catch (Exception ew)
            {
                return BadRequest(ew);
            }
        }

        // Lead
        [HttpGet("getAllProjectByLead/{IdLead}")]
		public IActionResult getAllProjectByLead(int IdLead)
		{

			try
			{
                var list = from x in _context.Projects
                           join d in _context.Users on x.Leader equals d.id
                           where d.id == IdLead && x.IsDeleted == false && x.IsFinished == false
                           select new
                           {
                               id = x.Id,
                               dateCreated = x.DateCreated,
                               dateUpdate = x.DateUpdate,
                               description = x.Description,
                               endDate = x.EndDate,
                               isDeleted = x.IsDeleted,
                               isFinished = x.IsFinished,
                               isOnGitlab = x.IsOnGitlab,
                               leader = d.FullName,
                               name = x.Name,
                               projectCode = x.ProjectCode,
                               startDate = x.StartDate,
                               userCreated = x.UserCreated,
                               userId = x.UserId,
                               userUpdate = x.UserUpdate
                           };

                return Ok(list);
            }catch(Exception ex)
			{
				return BadRequest(ex);
			}
		}

        // Staff
        [HttpGet("getAllProjectByStaff/{idstaff}")]
	    public IActionResult getAllProjectByStaff(int idstaff)
		{

			try
			{
				var list = from x in _context.Projects
						   join c in _context.Member_Projects on x.Id equals c.idProject
						   join k in _context.Users on x.Leader equals k.id
						   where c.member == idstaff && x.IsDeleted == false && x.IsFinished == false
                           select new
						   {
							   id = x.Id,
							   dateCreated = x.DateCreated,
							   dateUpdate = x.DateUpdate,
							   description = x.Description,
							   endDate = x.EndDate,
							   isDeleted = x.IsDeleted,
							   isFinished = x.IsFinished,
							   isOnGitlab = x.IsOnGitlab,
							   leader = k.FullName,
							   name = x.Name,
							   projectCode = x.ProjectCode,
							   startDate = x.StartDate,
							   userCreated = x.UserCreated,
							   userId = x.UserId,
							   userUpdate = x.UserUpdate
						   };

				return Ok(list);

            }
            catch (Exception ex)
			{
				return BadRequest(ex);
			}	
		}

        // get lead list in addProject
        [HttpGet("getListLead")]
		public IActionResult getListLead()
		{
			try
			{
                var list = _context.Users.Where(x => x.IdGroup == 3);
                return Ok(list);
            }
            catch(Exception ex)
			{
				return BadRequest(ex);
			}
			
		}

        [HttpPost("addProject")]
        [Authorize(Roles = "group: pm")]
        public async Task<IActionResult> Create(AddNewProjectDto project_Model)
		{
			try
			{
				var existingProject = _context.Projects.Where(p => p.ProjectCode == project_Model.ProjectCode);
				if (existingProject.Any())
				{
					return BadRequest("Project code is exist!");
				}
				/*            var p = new Projects
                            {

                                Name = project_Model.Name,
                                ProjectCode = project_Model.ProjectCode,
                                Description = project_Model.Description,
                                StartDate = project_Model.StartDate,
                                EndDate = project_Model.EndDate,
                                IsDeleted = false,
                                IsFinished = false,
                                UserId = project_Model.UserId,
                                Leader = project_Model.Leader,
                                UserCreated = project_Model.UserCreated,
                                DateCreated = DateTime.Now,
                                UserUpdate = project_Model.UserCreated,
                                DateUpdate = DateTime.Now

                            };*/
				var p = _mapper.Map<Projects>(project_Model);
				var pro = await _context.Projects.SingleOrDefaultAsync(pr => pr.Name == p.Name);
				if (pro == null && p.StartDate < p.EndDate)
				{
					if (p.IsOnGitlab)
					{
						p.EndDate = null;
					}
					_context.Add(p);
					_context.SaveChanges();
					return Ok();
				}
				return BadRequest("Wrong data enter");
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}


		}
		[HttpPut("DeleteProject/{id}")]
        [Authorize(Roles = "group: pm")]
        public IActionResult DeleteProject(int id, IdUserChangeProjectDto request)
		{
			try
			{
				var pro = _context.Projects.SingleOrDefault(p => p.Id == id);
				if (pro == null)
				{
					return NotFound("Khong tim thay doi tuong");
				}
				else
				{
					pro.IsDeleted = true;
					pro.UserUpdate = request.UserId;
					pro.DateUpdate = DateTime.UtcNow;
					_context.SaveChanges();
					return Ok();
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpPut("FinishProject/{id}")]
        [Authorize(Roles = "group: pm")]
        public IActionResult FinishProject(int id, IdUserChangeProjectDto request)
		{

			try
			{
				var pro = _context.Projects.SingleOrDefault(p => p.Id == id);
				if (pro == null)
				{
					return NotFound("Khong tim thay doi tuong");
				}
				else
				{
					pro.IsFinished = true;
					pro.UserUpdate = request.UserId;
					pro.DateUpdate = DateTime.UtcNow;
					_context.SaveChanges();
					return Ok();
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpGet("getById/{id}")]
		public IActionResult getById(int id)
		{
			try
			{
				var pro = _context.Projects.Where(p => p.IsDeleted == false).SingleOrDefault(p => p.Id == id);
				if (pro == null)
				{
					return NotFound();
				}
				return Ok(pro);
			}
			catch (Exception ex) { return BadRequest(ex.Message); }
		}

        [HttpGet("getProByIdDel/{id}")]
        public IActionResult getProByIdDel(int id)
        {
            try
            {
                var pro = _context.Projects.SingleOrDefault(p => p.Id == id);
                if (pro == null)
                {
                    return NotFound();
                }
                return Ok(pro);
            }
            catch (Exception ex) { return BadRequest(ex.Message); }
        }

        [HttpGet("getProjectIsDelete")]
		public IActionResult getListProIsDelete()
		{
			try
			{
				var pro = _context.Projects.Where(p => p.IsDeleted == true);
				if (pro == null)
				{
					return NotFound("Khong tim thay");
				}
				return Ok(pro);
			}
			catch (Exception ex) { return BadRequest(ex.Message); }
		}

		[HttpGet("getProjectByDayBefore/{day}")]
		public IActionResult getProjectByDayBefore(DateTime day)
		{
			try
			{
				var pro = _context.Projects.Where(p => p.IsDeleted == false).Where(p => p.EndDate < day);
				if (pro == null)
				{
					return NotFound("Khong tim thay du an");
				}
				else
				{
					return Ok(pro);
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}

		[HttpGet("getProjectByName/{name}")]
		public IActionResult getProjectByName(string name)
		{
			try
			{
				var pro = _context.Projects.Where(p => p.IsDeleted == false).SingleOrDefault(p => p.Name == name);
				if (pro == null)
				{
					return NotFound();
				}
				return Ok(pro);

			}
			catch (Exception ex) { return BadRequest(ex); }
		}
		[HttpGet("getProjectIsNotFinished")]
		public IActionResult getProjectIsNotFinished()
		{
			try
			{
				var pro = _context.Projects.Where(p => p.IsFinished == false);
				if (pro == null)
				{
					return NotFound();
				}
				return Ok(pro);

			}
			catch (Exception ex) { return BadRequest(ex); }
		}



		[HttpGet("getLengthOfProject/{name}")]
		public IActionResult getLengthOfProject(string name)
		{
			try
			{
				var pro = _context.Projects.Where(p => p.IsDeleted == false).SingleOrDefault(p => p.Name == name);
				if (pro == null)
				{
					return NotFound();
				}
				else
				{
					TimeSpan count = (TimeSpan)(pro.EndDate - pro.StartDate);
					double day = count.TotalDays;
					return Ok(day);
				}
			}
			catch (Exception ex)
			{
				return BadRequest(ex);
			}
		}
		[HttpGet]
		[Route("getProjectById/{Id}")]
		public async Task<ActionResult<Projects>> getProjectById(int Id)
		{
			var project = await _context.Projects.Where(p => p.IsDeleted == false).SingleOrDefaultAsync(x => x.Id == Id);

			if (project == null)
			{
				return NotFound();
			}

			return project;
		}

		[HttpPut]
        [Authorize(Roles = "group: pm")]
        [Route("updateProject/{id}")]
		public async Task<ActionResult> updateProject(EditProjectDto requests, int id)
		{

			var acc = await _context.Projects.FindAsync(id);
			if (acc == null)
			{
				return NotFound();
			}
			/*acc.Name = requests.Name;
            acc.ProjectCode = requests.ProjectCode;
            acc.Description = requests.Description;
            acc.StartDate = requests.StartDate;
            acc.EndDate = requests.EndDate;
            acc.UserId = requests.UserId;
            acc.Leader = requests.Leader;
            acc.UserUpdate = requests.UserUpdate;
            acc.DateUpdate = DateTime.Now;*/
			var project = _mapper.Map<EditProjectDto, Projects>(requests, acc);
			_context.Update(project);
			await _context.SaveChangesAsync();
			return Ok();
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
		[HttpPost("getAll")]
		public async Task<IActionResult> getAll(Paginate p)
		{
			if (p.pageIndex == 0)
			{
				p.pageIndex = 1;
			}

			if (p.pageSizeEnum == 0)
			{
				p.pageSizeEnum = 2;
			}
			var keyWord = await _context.Projects.Where(i => i.IsFinished == false 
															&& (string.IsNullOrEmpty(p.word)
															|| i.Id.ToString().Contains(p.word)
															|| i.StartDate.ToString().Contains(p.word)
															|| i.EndDate.ToString().Contains(p.word)
															|| i.UserId.ToString().Contains(p.word)
															|| i.Leader.ToString().Contains(p.word)
															|| i.UserCreated.ToString().Contains(p.word)
															|| i.DateCreated.ToString().Contains(p.word)
															|| i.UserUpdate.ToString().Contains(p.word)
															|| i.DateUpdate.ToString().Contains(p.word)
															|| i.ProjectCode.Contains(p.word)
															|| i.Name.Contains(p.word)
															|| i.Description!.Contains(p.word))).Skip((p.pageIndex - 1) * p.pageSizeEnum).Take(p.pageSizeEnum).ToListAsync();

			var listSort = keyWord.OrderBy(i => i.DateCreated).ToList();
			var pageIndex = p.pageIndex;
			var pageSize = p.pageSizeEnum;
			var resultPage = await _paginationServices.paginationListTableAsync(listSort, pageIndex, pageSize);

			if (resultPage._success)
			{
				return Ok(resultPage);
			}
			return BadRequest(resultPage);
		}

		[HttpGet]
		[Route("getProjectByDayAfter")]
		public async Task<ActionResult<Projects>> getProjectByDayAfter(DateTime d)
		{
			var project = await _context.Projects.Where(x => x.EndDate > d && x.IsDeleted == false).ToListAsync();
			if (project == null)
			{
				return NotFound();
			}
			return Ok(project);
        }


		[HttpGet]
		[Route("getProjectisFinished")]
		public async Task<ActionResult<IEnumerable<Projects>>> getProjectisFinished()
		{
			var project = await _context.Projects.Where(x => x.IsFinished == true).ToListAsync();
			if (project == null)
			{
				return NotFound();
			}
			return Ok(project);
		}

		[HttpGet]
		[Route("getUserByproject/{Id}")]
		public async Task<ActionResult> getUserByproject(int Id)
		{
			var project = await _context.Projects.Where(x => x.IsFinished == false).SingleOrDefaultAsync(x => x.Id == Id);
			if (project == null)
			{
				return NotFound();
			}
			var acc = project.UserId;
			return Ok(acc);
		}
		[HttpGet]
		[Route("exportExcel")]
		public async Task<string> DownloadFile()
		{
			var wb = new XLWorkbook();
			var ws = wb.Worksheets.Add("Sheet1");
			// get data from DB
			var _data = await _context.Projects.ToListAsync();
			var columns_name = typeof(Projects).GetProperties()
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
			wb.SaveAs("..\\FE\\Excel\\Projects_Table.xlsx");
			return "Excel\\Projects_Table.xlsx";
		}
        [HttpGet("getProjectOnGitlab")]
		[Authorize(Roles = "permission_group: True module: project")]
		public IActionResult getProjectOnGitlab()
        {
            try
            {
				var pro = _context.Projects.Where(p => p.IsOnGitlab == true && p.IsDeleted == false).ToList();
                if (pro == null)
                {
                    return NotFound();
                }
                return Ok(pro);

            }
            catch (Exception ex) { return BadRequest(ex); }
        }

		[HttpGet("UserInProject/{idproject}")]
		public ActionResult UserInProject(int idproject)
		{
			try
			{
                var listuser = from x in _context.Users
                               join c in _context.Member_Projects on x.id equals c.member
                               join d in _context.Projects on c.idProject equals d.Id
                               where d.Id == idproject && d.IsDeleted == false
                               select new
                               {
                                   x,
								   name = x.lastName +" "+ x.firstName
                               };

                return Ok(listuser);
            }
            catch(Exception ex)
			{
				return BadRequest(ex);
			}
			
		}

		[HttpGet("GetProjectByIdLead/{idlead}")]
		public IActionResult GetProjectByIdLead(int idlead)
		{
			try
			{
                var list = _context.Projects.Where(x => x.IsFinished == false && x.IsDeleted == false && x.Leader == idlead).ToList();

                return Ok(list);
            }
            catch(Exception ex)
			{
				return BadRequest(ex);
			}		
		}
    }

}
