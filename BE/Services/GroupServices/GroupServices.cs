using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Response;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.InkML;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;
using Org.BouncyCastle.Ocsp;

namespace BE.Services.GroupServices
{
    public interface IGroupServices
    {
        Task<BaseResponse<List<Group>>> GetAllGroupAsync();
        Task<BaseResponse<Group>> GetGroupById(int groupId);
        Task<BaseResponse<Group>> CreateGroup(AddGroupDtos addGroupDtos);
        Task<BaseResponse<Group>> UpdateGroup(int idGroup, UpdateGroupDtos updateGroupDtos);
        Task<BaseResponse<Group>> DeleteGroup(int idGroup);
        Task<BaseResponse<List<Group>>> DeleteMultiGroup(List<int> idGroup);
        Task<BaseResponse<Group>> DownloadFile();
    }

    public class GroupServices : IGroupServices
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public GroupServices(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Group>>> GetAllGroupAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Group>();
            try
            {
                var group = await _db.Groups.Where(s => s.IsDeleted == 0).OrderByDescending(s => s.dateCreated).ToListAsync();

                success = true;
                message = "Get all data successfully";
                data.AddRange(group);
                return (new BaseResponse<List<Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<Group>> GetGroupById(int groupId)
        {
            var success = false;
            var message = "";
            try
            {
                var group = await _db.Groups
                    .OrderByDescending(s => s.dateCreated)
                    .Where(s => s.IsDeleted == 0
                            && s.Id.Equals(groupId))
                    .FirstOrDefaultAsync();
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<Group>(success, message, group));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<Group>(success, message, new Group()));
            }
        }

        public async Task<BaseResponse<Group>> CreateGroup(AddGroupDtos addGroupDtos)
        {
            var success = false;
            var message = "";
            try
            {
                var addGroup = await _db.Groups.FirstOrDefaultAsync(g => g.NameGroup!.ToLower() == addGroupDtos.NameGroup!.ToLower());
                if (addGroup == null)
                {
                    var group = _mapper.Map<Group>(addGroupDtos);
                    group.dateCreated = DateTime.Now;
                    group.IsDeleted = 0;
                    await _db.Groups.AddAsync(group);
                    await _db.SaveChangesAsync();

                    success = true;
                    message = "Add new Group successfully";
                    return new BaseResponse<Group>(success, message, group);
                }
                else
                {
                    message = "The group already exists!";
                    return new BaseResponse<Group>(success, message, new Group());
                }
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Group failed! {ex.Message}";
                return new BaseResponse<Group>(success, message, new Group());
            }
        }

        public async Task<BaseResponse<Group>> UpdateGroup(int idGroup, UpdateGroupDtos updateGroupDtos)
        {
            var success = false;
            var message = "";
            try
            {
                var group = await _db.Groups.Where(s => s.IsDeleted == 0 && s.Id.Equals(idGroup)).FirstOrDefaultAsync();
                if (group is null)
                {
                    message = "Group doesn't exist !";
                    return new BaseResponse<Group>(success, message, new Group());
                }
                var groupMapData = _mapper.Map<UpdateGroupDtos, Group>(updateGroupDtos, group);

                groupMapData.dateModified = DateTime.Now;
                _db.Groups.Update(groupMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing Group successfully";
                return new BaseResponse<Group>(success, message, groupMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Group failed! {ex.Message}";
                return new BaseResponse<Group>(success, message, new Group());
            }
        }


        public async Task<BaseResponse<Group>> DeleteGroup(int idGroup)
        {
            var success = false;
            var message = "";
            try
            {
                var group = await _db.Groups.Where(s => s.IsDeleted == 0 && s.Id.Equals(idGroup)).FirstOrDefaultAsync();
                if (group is null)
                {
                    message = "Group doesn't exist !";
                    return new BaseResponse<Group>(success, message, new Group());
                }

                group.IsDeleted = 1;
                _db.Groups.Update(group);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Group successfully";
                return new BaseResponse<Group>(success, message, group);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting UserGroup failed! {ex.InnerException}";
                return new BaseResponse<Group>(success, message, new Group());
            }
        }

        public async Task<BaseResponse<List<Group>>> DeleteMultiGroup(List<int> listIdGroup)
        {
            var success = false;
            var message = "";
            var data = new List<Group>();
            try
            {
                foreach(var item in listIdGroup)
                {
                    var group = await _db.Groups.Where(s => s.IsDeleted == 0 && s.Id.Equals(item)).FirstOrDefaultAsync();
                    if (group != null)
                    {
                        group.IsDeleted = 1;
                        _db.Groups.Update(group);
                        data.Add(group);
                    }
                    else
                    {
                        message = item + " group doesn't exist !";
                        return new BaseResponse<List<Group>>(success, message, data = null);
                    }
                }
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting GroupMulti successfully";
                return new BaseResponse<List<Group>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting GroupMulti failed! {ex.InnerException}";
                return new BaseResponse<List<Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Group>> DownloadFile()
        {
            var success = false;
            var message = "";
            try
            {
                var wb = new XLWorkbook();
                var ws = wb.Worksheets.Add("Sheet1");
                // get data from DB
                var _data = await _db.Groups.ToListAsync();
                var columns_name = typeof(Group).GetProperties()
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
                wb.SaveAs("..\\FE\\Excel\\Group_Table.xlsx");

                success = true;
                message =  "Excel\\Group_Table.xlsx";
                return new BaseResponse<Group>(success, message, new Group());
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting UserGroup failed! {ex.InnerException}";
                return new BaseResponse<Group>(success, message, new Group());
            }
        }


    }
}
