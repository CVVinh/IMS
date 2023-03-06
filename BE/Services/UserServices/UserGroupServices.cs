using AutoMapper;
using BE.Data.Contexts;
using BE.Data.Dtos.RulesDTOs;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;
using BE.Helpers;
using BE.Response;
using Microsoft.EntityFrameworkCore;
using NuGet.Packaging;

namespace BE.Services.UserServices
{
    public interface IUserGroupServices
    {
        Task<BaseResponse<List<User_Group>>> GetAllUserGroupAsync();
        Task<BaseResponse<List<User_Group>>> GetUserGroupWithUserId(int userId);
        Task<BaseResponse<List<User_Group>>> GetUserGroupWithGroupId(int groupId);
        Task<BaseResponse<List<User_Group>>> CreateUserGroup(List<UserGroupCreatedDto> userGroupCreatedDto);
        Task<BaseResponse<User_Group>> UpdateUserGroup(int idUser, int idGroup, UserGroupUpdatedDto userGroupUpdatedDto);
        Task<BaseResponse<User_Group>> DeleteUserGroup(UserGroupDeletedDto userGroupDeletedDto);
    }

    public class UserGroupServices : IUserGroupServices
    {
        private readonly AppDbContext _db;
        private readonly IMapper _mapper;

        public UserGroupServices(AppDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<BaseResponse<List<User_Group>>> GetAllUserGroupAsync()
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                var userGroup = await _db.UserGroups
                    .Where(s => s.isDeleted == false)
                    .OrderByDescending(s => s.dateCreated)
                    .ToListAsync();

                success = true;
                message = "Get all data successfully";
                data.AddRange(userGroup);
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<User_Group>>> GetUserGroupWithGroupId(int groupId)
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                var userGroup = await _db.UserGroups
                    .OrderByDescending(s => s.dateCreated)
                    .Where(s => s.isDeleted == false
                            && s.idGroup.Equals(groupId))
                    .ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(userGroup);
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<User_Group>>> GetUserGroupWithUserId(int userId)
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                var userGroup = await _db.UserGroups
                    .OrderByDescending(s => s.dateCreated)
                    .Where(s => s.isDeleted == false
                            && s.idUser.Equals(userId))
                    .ToListAsync();
                success = true;
                message = "Get data successfully";
                data.AddRange(userGroup);
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
            catch (Exception ex)
            {
                success = false;
                message = ex.Message;
                return (new BaseResponse<List<User_Group>>(success, message, data));
            }
        }

        public async Task<BaseResponse<List<User_Group>>> CreateUserGroup(List<UserGroupCreatedDto> userGroupCreatedDto)
        {
            var success = false;
            var message = "";
            var data = new List<User_Group>();
            try
            {
                foreach(var item in userGroupCreatedDto)
                {
                    var userGroup = _mapper.Map<User_Group>(item);
                    userGroup.dateCreated = DateTime.Now;
                    userGroup.isDeleted = false;
                    await _db.UserGroups.AddAsync(userGroup);
                    data.Add(userGroup);
                }
                
                await _db.SaveChangesAsync();
                success = true;
                message = "Add new User Group successfully";
                return new BaseResponse<List<User_Group>>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Adding new User Group failed! {ex.Message}";
                return new BaseResponse<List<User_Group>>(success, message, data);
            }
        }

        public async Task<BaseResponse<User_Group>> UpdateUserGroup(int idUser, int idGroup, UserGroupUpdatedDto userGroupUpdatedDto)
        {
            var success = false;
            var message = "";
            var data = new User_Group();
            try
            {
                var userGroup = await _db.UserGroups.Where(s => s.isDeleted == false && s.idUser.Equals(idUser) && s.idGroup.Equals(idGroup)).FirstOrDefaultAsync();
                if (userGroup is null)
                {
                    message = "UserGroup doesn't exist !";
                    data = null;
                    return new BaseResponse<User_Group>(success, message, data);
                }
                var userGroupMapData = _mapper.Map<UserGroupUpdatedDto, User_Group>(userGroupUpdatedDto, userGroup);

                userGroupMapData.dateUpdated = DateTime.Now;
                _db.UserGroups.Update(userGroupMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Editing UserGroup successfully";
                data = userGroupMapData;
                return new BaseResponse<User_Group>(success, message, data);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Editing UserGroup failed! {ex.Message}";
                return new BaseResponse<User_Group>(success, message, data = null);
            }
        }

        public async Task<BaseResponse<User_Group>> DeleteUserGroup(UserGroupDeletedDto userGroupDeletedDto)
        {
            var success = false;
            var message = "";
            var data = new User_Group();
            try
            {
                var userGroup = await _db.UserGroups.Where(s => s.isDeleted == false && s.idUser.Equals(userGroupDeletedDto.idUser) && s.idGroup.Equals(userGroupDeletedDto.idGroup)).FirstOrDefaultAsync();
                if (userGroup is null)
                {
                    success = false;
                    message = "UserGroup doesn't exist !";
                    return new BaseResponse<User_Group>(success, message, data = null);
                }

                var userGroupMapData = _mapper.Map<UserGroupDeletedDto, User_Group>(userGroupDeletedDto, userGroup);
                userGroupMapData.dateDeleted = DateTime.Now;
                userGroupMapData.isDeleted = true;
                _db.UserGroups.Update(userGroupMapData);
                await _db.SaveChangesAsync();

                success = true;
                message = "Deleting UserGroup successfully";
                return new BaseResponse<User_Group>(success, message, userGroupMapData);
            }
            catch (Exception ex)
            {
                success = false;
                message = $"Deleting UserGroup failed! {ex.InnerException}";
                return new BaseResponse<User_Group>(success, message, data = null);
            }
        }

        
    }
}
