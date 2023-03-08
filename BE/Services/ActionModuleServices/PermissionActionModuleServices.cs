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
        Task<BaseResponse<List<Permission_Action_Module>>> DeleteMultiPermissionActionModule(List<DeleteMultiPermissionActionModuleDto> deleteMultiPermissionActionModuleDtos);
    
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
                var permissionActionModule = await _db.PermissionActionModules.Where(s => s.isDeleted == false).OrderBy(s => s.actionModule.id).Include(s => s.module).Select(s => new Permission_Action_Module
                {
                    id = s.id,
                    moduleId= s.moduleId,
                    actionModuleId= s.actionModuleId,
                    module = s.module,
                    actionModule= s.actionModule,
                    
                }).ToListAsync();
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
                var permissionActionModule = await _db.PermissionActionModules.OrderBy(s => s.actionModule.id).Where(s => s.isDeleted == false && s.moduleId.Equals(moduleId)).Include(s => s.module).Select(s => new Permission_Action_Module
                {
                    id = s.id,
                    moduleId = s.moduleId,
                    actionModuleId = s.actionModuleId,
                    module = s.module,
                    actionModule = s.actionModule,

                }).ToListAsync();
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
                var permissionActionModule = await _db.PermissionActionModules.OrderBy(s => s.actionModule.id).Where(s => s.isDeleted == false && s.actionModuleId.Equals(actionId)).Include(s => s.module).Select(s => new Permission_Action_Module
                {
                    id = s.id,
                    moduleId = s.moduleId,
                    actionModuleId = s.actionModuleId,
                    module = s.module,
                    actionModule = s.actionModule,

                }).ToListAsync(); ;
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
                    var permissionActionModule = await _db.PermissionActionModules.Where(s => s.isDeleted == false && s.moduleId.Equals(item.moduleId) && s.actionModuleId.Equals(item.actionModuleId)).FirstOrDefaultAsync();
                    if (permissionActionModule is null)
                    {
                        var permissionActionModuleMapData = _mapper.Map<Permission_Action_Module>(item);
                        permissionActionModuleMapData.dateCreated = DateTime.Now;
                        permissionActionModuleMapData.isDeleted = false;
                        await _db.PermissionActionModules.AddAsync(permissionActionModuleMapData);
                        data.Add(permissionActionModuleMapData);
                    }
                    else
                    {
                        message = "Permission_Action_Module have been existed !";
                        return new BaseResponse<List<Permission_Action_Module>>(success, message, data = null);
                    }
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
                var permissionActionModule = await _db.PermissionActionModules.Where(s => s.isDeleted == false && s.moduleId.Equals(requestPermissionActionModuleDto.moduleId) && s.actionModuleId.Equals(requestPermissionActionModuleDto.actionModuleId)).FirstOrDefaultAsync();
                
                if (permissionActionModule is null)
                {
                    message = "Permission_Action_Module doesn't exist !";
                    return new BaseResponse<Permission_Action_Module>(success, message, data = null);
                }

                var permissionActionModuleTest = await _db.PermissionActionModules.Where(s => s.isDeleted == false && s.moduleId.Equals(requestPermissionActionModuleDto.moduleId) && s.actionModuleId.Equals(updatePermissionActionModuleDto.actionModuleId)).FirstOrDefaultAsync();

                if (permissionActionModuleTest != null)
                {
                    message = "Permission_Action_Module have been existed !";
                    return new BaseResponse<Permission_Action_Module>(success, message, data = null);
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
                var permissionActionModule = await _db.PermissionActionModules.Where(s => s.isDeleted == false && s.moduleId.Equals(requestPermissionActionModuleDto.moduleId) && s.actionModuleId.Equals(requestPermissionActionModuleDto.actionModuleId)).FirstOrDefaultAsync();
                if (permissionActionModule is null)
                {
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

        public async Task<BaseResponse<List<Permission_Action_Module>>> DeleteMultiPermissionActionModule(List<DeleteMultiPermissionActionModuleDto> deleteMultiPermissionActionModuleDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Action_Module>();
            try
            {
                foreach (var item in deleteMultiPermissionActionModuleDtos)
                {
                    var result = await _db.PermissionActionModules.Where(s => s.isDeleted == false && s.moduleId.Equals(item.moduleId) && s.actionModuleId.Equals(item.actionModuleId)).FirstOrDefaultAsync();
                    if (result is null)
                    {
                        message = "Permission_Action_Module doesn't exist !";
                        return new BaseResponse<List<Permission_Action_Module>>(success, message, data = null);
                    }
                    else
                    {
                        var permissionActionModuleMapData = _mapper.Map<DeleteMultiPermissionActionModuleDto, Permission_Action_Module>(item, result);
                        permissionActionModuleMapData.dateDeleted = DateTime.Now;
                        permissionActionModuleMapData.isDeleted = true;
                        _db.PermissionActionModules.Update(permissionActionModuleMapData);
                        data.Add(result);
                    }
                }
                await _db.SaveChangesAsync();
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
