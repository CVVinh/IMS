using AutoMapper;
using BE.Data.Dtos.DeviceDto;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Models;

namespace BE.Data.Profiles.DevicesProfile
{
    public class DevicesProfile : Profile
    {
        public DevicesProfile()
        {
            CreateMap<CreateDevicesDto, Devices>();
        }
    }
}
