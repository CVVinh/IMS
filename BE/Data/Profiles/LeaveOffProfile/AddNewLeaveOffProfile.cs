using AutoMapper;
using BE.Data.Dtos.LeaveOffDtos;
using BE.Data.Models;

namespace BE.Data.Profiles.LeaveOffProfile
{
    public class AddNewLeaveOffProfile: Profile
    {
        public AddNewLeaveOffProfile()
        {
            CreateMap<AddNewLeaveOffDto, LeaveOff>();
        }
    }
}
