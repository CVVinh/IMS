using AutoMapper;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Models;

namespace BE.Data.Profiles.LeaveOffProfile
{
    public class EditResgisterLeaveOffProfile : Profile
    {
        public EditResgisterLeaveOffProfile()
        {
            CreateMap<EditRegisterLeaveOffDtos, LeaveOff>();
        }

    }
}
