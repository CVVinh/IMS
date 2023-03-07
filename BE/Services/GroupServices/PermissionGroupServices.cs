﻿using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Response;
using Microsoft.EntityFrameworkCore;

namespace BE.Services.GroupServices
{
    public interface IPermissionGroupServices
    {
        Task<BaseResponse<List<Permission_Group>>> GetAllPermissionGroupAsync();
        Task<BaseResponse<List<Permission_Group>>> GetPermissionGroupWithModuleId(int moduleId);
        Task<BaseResponse<List<Permission_Group>>> GetPermissionGroupWithGroupId(int groupId);
        Task<BaseResponse<List<Permission_Group>>> CreatePermissionGroup(List<PermissionGroupDto> permissionGroupDtos);
        Task<BaseResponse<Permission_Group>> UpdatePermissionGroup(PermissionGroupRequestDto permissionGroupRequestDto, PermissionGroupDto permissionGroupDtos);
        Task<BaseResponse<Permission_Group>> DeletePermissionGroup(PermissionGroupRequestDto permissionGroupRequestDto);
        Task<BaseResponse<List<Permission_Group>>> DeleteMultiPermissionGroup(List<PermissionGroupRequestDto> permissionGroupRequestDto);
    }

    public class PermissionGroupServices : IPermissionGroupServices
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public PermissionGroupServices(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<Permission_Group>>> GetAllPermissionGroupAsync()
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                var permissionGroup = await _db.Permission_Groups.ToListAsync();

                success = true;
                message = "Get all data successfully";
                data.AddRange(permissionGroup);
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> GetPermissionGroupWithGroupId(int groupId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IdGroup.Equals(groupId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionGroup);
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> GetPermissionGroupWithModuleId(int moduleId)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IdModule.Equals(moduleId)).ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(permissionGroup);
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<Permission_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> CreatePermissionGroup(List<PermissionGroupDto> permissionGroupDtos)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                foreach (var item in permissionGroupDtos)
                {
                    var permissionGroup = _mapper.Map<Permission_Group>(item);
                    await _db.Permission_Groups.AddAsync(permissionGroup);
                    data.Add(permissionGroup);
                }

                await _db.SaveChangesAsync();
                success = true;
                message = "Add new Permission_Group successfully";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new Permission_Group failed! {ex.Message}";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<Permission_Group>> UpdatePermissionGroup(PermissionGroupRequestDto permissionGroupRequestDto, PermissionGroupDto permissionGroupDtos)
        {
            var success = false;
            var message = "";
            var data = new Permission_Group();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IdGroup.Equals(permissionGroupRequestDto.IdGroup) && s.IdModule.Equals(permissionGroupRequestDto.IdModule)).FirstOrDefaultAsync();
                if (permissionGroup is null)
                {
                    message = "Permission_Group doesn't exist !";
                    data = null;
                    return new BaseResponse<Permission_Group>(success, message, data);
                }
                var permissionGroupMapData = _mapper.Map<PermissionGroupDto, Permission_Group>(permissionGroupDtos, permissionGroup);

                _db.Permission_Groups.Update(permissionGroupMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing Permission_Group successfully";
                data = permissionGroupMapData;
                return new BaseResponse<Permission_Group>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing Permission_Group failed! {ex.Message}";
                return new BaseResponse<Permission_Group>(success, message, data = null);
            }
        }

        public async Task<BaseResponse<Permission_Group>> DeletePermissionGroup(PermissionGroupRequestDto permissionGroupRequestDto)
        {
            var success = false;
            var message = "";
            var data = new Permission_Group();
            try
            {
                var permissionGroup = await _db.Permission_Groups.Where(s => s.IdModule.Equals(permissionGroupRequestDto.IdModule) && s.IdGroup.Equals(permissionGroupRequestDto.IdGroup)).FirstOrDefaultAsync();
                if (permissionGroup is null)
                {
                    success = false;
                    message = "UserGroup doesn't exist !";
                    return new BaseResponse<Permission_Group>(success, message, data = null);
                }

                _db.Permission_Groups.Remove(permissionGroup);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting Permission_Group successfully";
                return new BaseResponse<Permission_Group>(success, message, permissionGroup);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Permission_Group failed! {ex.InnerException}";
                return new BaseResponse<Permission_Group>(success, message, data = null);
            }
        }

        public async Task<BaseResponse<List<Permission_Group>>> DeleteMultiPermissionGroup(List<PermissionGroupRequestDto> permissionGroupRequestDto)
        {
            var success = false;
            var message = "";
            var data = new List<Permission_Group>();
            try
            {
                foreach (var item in permissionGroupRequestDto)
                {
                    var result = await DeletePermissionGroup(item);
                    if (result._success)
                    {
                        data.Add(result._Data);
                    }
                    else
                    {
                        return new BaseResponse<List<Permission_Group>>(success, result._Message, data = null);
                    }
                }
                success = true;
                message = "Deleting Multi Permission_Group successfully";
                return new BaseResponse<List<Permission_Group>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting Multi Permission_Group failed! {ex.InnerException}";
                return new BaseResponse<List<Permission_Group>>(success, message, data = null);
            }
        }


    }
}
