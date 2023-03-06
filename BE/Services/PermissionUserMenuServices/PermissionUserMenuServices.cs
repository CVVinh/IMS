using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.Permission_Use_Menus;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Response;
using BE.Services.UserServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace BE.Services.PermissionUserMenuServices
{
    public interface IPermissionUserMenuServices
    {
        Task<BaseResponse<List<Permission_Use_Menu>>> GetAllPermissionUserMenuAsync();
        Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithUserId(int userId);
        Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithModuleId(int moduleId);
        Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithMenuId(int menuId);
        Task<BaseResponse<List<Permission_Use_Menu>>> CreatePermissionUserMenu(List<PermissionUserMenuAddDto> permissionUserMenuAddDtos);
        Task<BaseResponse<Permission_Use_Menu>> UpdatePermissionUserMenu(PermissionUserMenuRequest permissionUserMenuRequest, PermissionUserMenuEditDto permissionUserMenuEditDto);
        Task<BaseResponse<Permission_Use_Menu>> DeletePermissionUserMenu(PermissionUserMenuRequest permissionUserMenuRequest);
    }

    public class PermissionUserMenuServices : IPermissionUserMenuServices
    {

        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public PermissionUserMenuServices(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> GetAllPermissionUserMenuAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.ToListAsync();

                success = true;
                message = "Get all data successfully";
                data.AddRange(permissionUserMenu);
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithMenuId(int menuId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.IdMenu.Equals(menuId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionUserMenu);
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithModuleId(int moduleId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.idModule.Equals(moduleId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionUserMenu);
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> GetPermissionUserMenuWithUserId(int userId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.IdUser.Equals(userId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionUserMenu);
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Use_Menu>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Use_Menu>>> CreatePermissionUserMenu(List<PermissionUserMenuAddDto> permissionUserMenuAddDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Use_Menu>();
            try
            {
                foreach (var item in permissionUserMenuAddDtos)
                {
                    var permissionUserMenu = _mapper.Map<Permission_Use_Menu>(item);
                    permissionUserMenu.dateCreated = DateTime.Now;
                    await _db.Permission_Use_Menus.AddAsync(permissionUserMenu);
                    data.Add(permissionUserMenu);
                }

                await _db.SaveChangesAsync();
                success = true;
                message = "Add new Permission_Use_Menu successfully";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Permission_Use_Menu failed! {ex.Message}";
                return new BaseResponse<List<Permission_Use_Menu>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Permission_Use_Menu>> UpdatePermissionUserMenu(PermissionUserMenuRequest permissionUserMenuRequest, PermissionUserMenuEditDto permissionUserMenuEditDto)
        {
            var success = false;
            var message = "";
            var data = new Permission_Use_Menu();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.IdUser.Equals(permissionUserMenuRequest.IdUser) 
                                && s.idModule.Equals(permissionUserMenuRequest.idModule) 
                                && s.IdMenu.Equals(permissionUserMenuRequest.IdMenu)).FirstOrDefaultAsync();
                if (permissionUserMenu is null)
                {
                    message = "Permission_Use_Menu doesn't exist !";
                    data = null;
                    return new BaseResponse<Permission_Use_Menu>(success, message, data);
                }
                var permissionUserMenuMapData = _mapper.Map<PermissionUserMenuEditDto, Permission_Use_Menu>(permissionUserMenuEditDto, permissionUserMenu);

                permissionUserMenuMapData.dateModified = DateTime.Now;
                _db.Permission_Use_Menus.Update(permissionUserMenuMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing Permission_Use_Menu successfully";
                data = permissionUserMenuMapData;
                return new BaseResponse<Permission_Use_Menu>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Permission_Use_Menu failed! {ex.Message}";
                return new BaseResponse<Permission_Use_Menu>(success, message, data = null);
            }
        }

        public async Task<BaseResponse<Permission_Use_Menu>> DeletePermissionUserMenu(PermissionUserMenuRequest permissionUserMenuRequest)
        {
            var success = false;
            var message = "";
            var data = new Permission_Use_Menu();
            try
            {
                var permissionUserMenu = await _db.Permission_Use_Menus.Where(s => s.IdUser.Equals(permissionUserMenuRequest.IdUser)
                                && s.idModule.Equals(permissionUserMenuRequest.idModule)
                                && s.IdMenu.Equals(permissionUserMenuRequest.IdMenu)).FirstOrDefaultAsync();

                if (permissionUserMenu is null)
                {
                    success = false;
                    message = "Permission_Use_Menus doesn't exist !";
                    return new BaseResponse<Permission_Use_Menu>(success, message, data = null);
                }

                _db.Permission_Use_Menus.Remove(permissionUserMenu);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Permission_Use_Menu successfully";
                return new BaseResponse<Permission_Use_Menu>(success, message, permissionUserMenu);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Permission_Use_Menu failed! {ex.InnerException}";
                return new BaseResponse<Permission_Use_Menu>(success, message, data = null);
            }
        }

        
    }
}
