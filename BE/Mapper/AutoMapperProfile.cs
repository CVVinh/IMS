using AutoMapper;
using BE.Data.Dtos.InfoDeviceDtos;
using BE.Data.Dtos.PaidDtos;
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
        }
    }
}
