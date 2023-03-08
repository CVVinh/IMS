using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.ActionModuleDtos;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Response;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace BE.Services.ActionModuleServices
{
    public interface IActionModuleServices
    {
        Task<BaseResponse<List<Action_Module>>> GetAllActionModuleAsync();
        Task<BaseResponse<Action_Module>> GetActionModuleById(int idAction);
        Task<BaseResponse<Action_Module>> CreateActionModule(AddActionModuleDto addActionModuleDto);
        Task<BaseResponse<Action_Module>> UpdateActionModule(int id, EditActionModuleDto editActionModuleDto);
        Task<BaseResponse<Action_Module>> DeleteActionModule(int id, DeleteActionModuleDto deleteActionModuleDto);
    }

    public class ActionModuleServices : IActionModuleServices
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public ActionModuleServices(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Action_Module>>> GetAllActionModuleAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Action_Module>();
            try
            {
                var actionModule = await _db.ActionModules.Where(s => s.isDeleted == false).OrderBy(s => s.id).ToListAsync();

                success = true;
                message = "Get all data successfully";
                data.AddRange(actionModule);
                return (new BaseResponse<List<Action_Module>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Action_Module>>(success, message, data));
            }
        }

        public async Task<BaseResponse<Action_Module>> GetActionModuleById(int idAction)
        {
            var success = false;
            var message = "";
            try
            {
                var actionModule = await _db.ActionModules.OrderBy(s => s.id).Where(s => s.isDeleted == false && s.id.Equals(idAction)).FirstOrDefaultAsync();
                success = true;
                message = "Get data successfully";
                return (new BaseResponse<Action_Module>(success, message, actionModule));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<Action_Module>(success, message, new Action_Module()));
            }
        }

        public async Task<BaseResponse<Action_Module>> CreateActionModule(AddActionModuleDto addActionModuleDto)
        {
            var success = false;
            var message = "";
            try
            {
                var actionModule = _mapper.Map<Action_Module>(addActionModuleDto);
                actionModule.dateCreated = DateTime.Now;
                actionModule.isDeleted = false;
                await _db.ActionModules.AddAsync(actionModule);
                await _db.SaveChangesAsync();

                success = true;
                message = "Add new Action_Module successfully";
                return new BaseResponse<Action_Module>(success, message, actionModule);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Action_Module failed! {ex.Message}";
                return new BaseResponse<Action_Module>(success, message, new Action_Module());
            }
        }


        public async Task<BaseResponse<Action_Module>> UpdateActionModule(int id, EditActionModuleDto editActionModuleDto)
        {
            var success = false;
            var message = "";
            try
            {
                var actionModule = await _db.ActionModules.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if (actionModule is null)
                {
                    message = "Action_Module doesn't exist !";
                    return new BaseResponse<Action_Module>(success, message, new Action_Module());
                }
                var actionModuleMapData = _mapper.Map<EditActionModuleDto, Action_Module>(editActionModuleDto, actionModule);

                actionModuleMapData.dateUpdated = DateTime.Now;
                _db.ActionModules.Update(actionModule);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing Action_Module successfully";
                return new BaseResponse<Action_Module>(success, message, actionModuleMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Action_Module failed! {ex.Message}";
                return new BaseResponse<Action_Module>(success, message, new Action_Module());
            }
        }

        public async Task<BaseResponse<Action_Module>> DeleteActionModule(int id, DeleteActionModuleDto deleteActionModuleDto)
        {
            var success = false;
            var message = "";
            try
            {
                var actionModule = await _db.ActionModules.Where(s => s.isDeleted == false && s.id.Equals(id)).FirstOrDefaultAsync();
                if (actionModule is null)
                {
                    message = "Action_Module doesn't exist !";
                    return new BaseResponse<Action_Module>(success, message, new Action_Module());
                }

                var actionModuleMapData = _mapper.Map<DeleteActionModuleDto, Action_Module>(deleteActionModuleDto, actionModule);

                actionModuleMapData.isDeleted = true;
                actionModuleMapData.dateDeleted = DateTime.Now;
                _db.ActionModules.Update(actionModuleMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Action_Module successfully";
                return new BaseResponse<Action_Module>(success, message, actionModuleMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Action_Module failed! {ex.InnerException}";
                return new BaseResponse<Action_Module>(success, message, new Action_Module());
            }
        }


    }
}
