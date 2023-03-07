using AutoMapper;
using BE.Data.Dtos.ActionModuleDtos;
using BE.Data.Dtos.GruopDtos;
using BE.Data.Dtos.InfoDeviceDtos;
using BE.Data.Dtos.ModuleDtos;
using BE.Data.Dtos.PaidDtos;
using BE.Data.Dtos.Permission_Use_Menus;
using BE.Data.Dtos.PermissionActionModuleDtos;
using BE.Data.Dtos.ProjectDtos;
using BE.Data.Dtos.RulesDTOs;
using BE.Data.Dtos.UserDtos;
using BE.Data.Models;

namespace BE.Mapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<Projects, AddNewProjectDto>().ReverseMap();
            CreateMap<Projects, EditProjectDto>().ReverseMap();
            CreateMap<Paid, CreatePaidDtos>().ReverseMap();
            CreateMap<Paid, AcceptPaymentPaidDtos>().ReverseMap();
            CreateMap<IdPaidImgDtos, PaidImage>().ReverseMap();

            CreateMap<Rules, AddOrUpdateRulesDTO>().ReverseMap();
            CreateMap<InfoDevices, CreateInfoDeviceDto>().ReverseMap();
            CreateMap<DeviceInstalledApps, CreateInfoDeviceDto>().ReverseMap();
            CreateMap<DeviceInstalledApps, ApplicationDto>().ReverseMap();


            CreateMap<UserGroupDeletedDto, User_Group>().ReverseMap();
            CreateMap<UserGroupUpdatedDto, User_Group>().ReverseMap();
            CreateMap<UserGroupCreatedDto, User_Group>().ReverseMap();
            CreateMap<PermissionUserMenuAddDto, Permission_Use_Menu>().ReverseMap();
            CreateMap<PermissionUserMenuEditDto, Permission_Use_Menu>().ReverseMap();

            CreateMap<UpdateGroupDtos, Group>().ReverseMap();
            CreateMap<AddGroupDtos, Group>().ReverseMap();

            CreateMap<AddActionModuleDto, Action_Module>().ReverseMap();
            CreateMap<EditActionModuleDto, Action_Module>().ReverseMap();
            CreateMap<DeleteActionModuleDto, Action_Module>().ReverseMap();

            CreateMap<ModuleDtos, Module>().ReverseMap();

            CreateMap<PermissionGroupDto, Permission_Group>().ReverseMap();
            CreateMap<PermissionGroupRequestDto, Permission_Group>().ReverseMap();

            CreateMap<AddPermissionActionModuleDto, Permission_Action_Module>().ReverseMap();
            CreateMap<UpdatePermissionActionModuleDto, Permission_Action_Module>().ReverseMap();
            CreateMap<DeletePermissionActionModuleDto, Permission_Action_Module>().ReverseMap();
            CreateMap<RequestPermissionActionModuleDto, Permission_Action_Module>().ReverseMap();
        }
    }
}
