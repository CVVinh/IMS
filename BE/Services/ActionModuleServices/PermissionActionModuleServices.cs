using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.PermissionActionModuleDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Response;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BE.Services.ActionModuleServices
{
    public interface IPermissionActionModuleServices
    {
        Task<BaseResponse<List<Permission_Action_Module>>> GetAllPermissionActionModuleAsync();
        Task<BaseResponse<List<Permission_Action_Module>>> GetPermissionActionModuleWithModuleId(int moduleId);
        Task<BaseResponse<List<Permission_Action_Module>>> GetPermissionActionModuleWithActionId(int actionId);
        Task<BaseResponse<List<Permission_Action_Module>>> CreatePermissionActionModule(List<AddPermissionActionModuleDto> addPermissionActionModuleDtos);
        Task<BaseResponse<Permission_Action_Module>> UpdateUPermissionActionModule(RequestPermissionActionModuleDto requestPermissionActionModuleDto, UpdatePermissionActionModuleDto updatePermissionActionModuleDto);
        Task<BaseResponse<Permission_Action_Module>> DeletePermissionActionModule(RequestPermissionActionModuleDto requestPermissionActionModuleDto, DeletePermissionActionModuleDto deletePermissionActionModuleDto);
        Task<BaseResponse<List<Permission_Action_Module>>> DeleteMultiPermissionActionModule(List<RequestPermissionActionModuleDto> requestPermissionActionModuleDto, DeletePermissionActionModuleDto deletePermissionActionModuleDto);
    }

    public class PermissionActionModuleServices : IPermissionActionModuleServices
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public PermissionActionModuleServices(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Permission_Action_Module>>> GetAllPermissionActionModuleAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                var permissionActionModule = await _db.PermissionActionModules.Where(s => s.isDeleted == false).OrderByDescending(s => s.dateCreated).ToListAsync();
                success = true;
                message = "Get all data successfully";
                data.AddRange(permissionActionModule);
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Action_Module>>> GetPermissionActionModuleWithModuleId(int moduleId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                var permissionActionModule = await _db.PermissionActionModules.OrderByDescending(s => s.dateCreated).Where(s => s.isDeleted == false && s.idModule.Equals(moduleId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionActionModule);
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Action_Module>>> GetPermissionActionModuleWithActionId(int actionId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                var permissionActionModule = await _db.PermissionActionModules.OrderByDescending(s => s.dateCreated).Where(s => s.isDeleted == false && s.idAction.Equals(actionId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionActionModule);
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Action_Module>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Action_Module>>> CreatePermissionActionModule(List<AddPermissionActionModuleDto> addPermissionActionModuleDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                foreach (var item in addPermissionActionModuleDtos)
                {
                    var permissionActionModule = _mapper.Map<Permission_Action_Module>(item);
                    permissionActionModule.dateCreated = DateTime.Now;
                    permissionActionModule.isDeleted = false;
                    await _db.PermissionActionModules.AddAsync(permissionActionModule);
                    data.Add(permissionActionModule);
                }

                await _db.SaveChangesAsync();
                success = true;
                message = "Add new Permission_Action_Module successfully";
                return new BaseResponse<List<Permission_Action_Module>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Permission_Action_Module failed! {ex.Message}";
                return new BaseResponse<List<Permission_Action_Module>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Permission_Action_Module>> UpdateUPermissionActionModule(RequestPermissionActionModuleDto requestPermissionActionModuleDto, UpdatePermissionActionModuleDto updatePermissionActionModuleDto)
        {
            var success = false;
            var message = "";
            var data = new Permission_Action_Module();
            try
            {
                var permissionActionModule = await _db.PermissionActionModules.Where(s => s.isDeleted == false && s.idModule.Equals(requestPermissionActionModuleDto.idModule) && s.idAction.Equals(requestPermissionActionModuleDto.idAction)).FirstOrDefaultAsync();
                if (permissionActionModule is null)
                {
                    message = "Permission_Action_Module doesn't exist !";
                    data = null;
                    return new BaseResponse<Permission_Action_Module>(success, message, data);
                }
                var permissionActionModuleMapData = _mapper.Map<UpdatePermissionActionModuleDto, Permission_Action_Module>(updatePermissionActionModuleDto, permissionActionModule);

                permissionActionModuleMapData.dateUpdated = DateTime.Now;
                _db.PermissionActionModules.Update(permissionActionModuleMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing Permission_Action_Module successfully";
                data = permissionActionModuleMapData;
                return new BaseResponse<Permission_Action_Module>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Permission_Action_Module failed! {ex.Message}";
                return new BaseResponse<Permission_Action_Module>(success, message, data = null);
            }
        }

        public async Task<BaseResponse<Permission_Action_Module>> DeletePermissionActionModule(RequestPermissionActionModuleDto requestPermissionActionModuleDto, DeletePermissionActionModuleDto deletePermissionActionModuleDto)
        {
            var success = false;
            var message = "";
            var data = new Permission_Action_Module();
            try
            {
                var permissionActionModule = await _db.PermissionActionModules.Where(s => s.isDeleted == false && s.idModule.Equals(requestPermissionActionModuleDto.idModule) && s.idAction.Equals(requestPermissionActionModuleDto.idAction)).FirstOrDefaultAsync();
                if (permissionActionModule is null)
                {
                    success = false;
                    message = "Permission_Action_Module doesn't exist !";
                    return new BaseResponse<Permission_Action_Module>(success, message, data = null);
                }

                var permissionActionModuleMapData = _mapper.Map<DeletePermissionActionModuleDto, Permission_Action_Module>(deletePermissionActionModuleDto, permissionActionModule);
                permissionActionModuleMapData.dateDeleted = DateTime.Now;
                permissionActionModuleMapData.isDeleted = true;
                _db.PermissionActionModules.Update(permissionActionModuleMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Permission_Action_Module successfully";
                return new BaseResponse<Permission_Action_Module>(success, message, permissionActionModuleMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Permission_Action_Module failed! {ex.InnerException}";
                return new BaseResponse<Permission_Action_Module>(success, message, data = null);
            }
        }

        public async Task<BaseResponse<List<Permission_Action_Module>>> DeleteMultiPermissionActionModule(List<RequestPermissionActionModuleDto> requestPermissionActionModuleDto, DeletePermissionActionModuleDto deletePermissionActionModuleDto)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                foreach (var item in requestPermissionActionModuleDto)
                {
                    var result = await DeletePermissionActionModule(item, deletePermissionActionModuleDto);
                    if (result._success)
                    {
                        data.Add(result._Data);
                    }
                    else
                    {
                        return new BaseResponse<List<Permission_Action_Module>>(success, result._Message, data = null);
                    }
                }
                success = true;
                message = "Deleting Multi Permission_Action_Module successfully";
                return new BaseResponse<List<Permission_Action_Module>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Multi Permission_Action_Module failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Action_Module>>(success, message, data = null);
            }
        }


    }
}
