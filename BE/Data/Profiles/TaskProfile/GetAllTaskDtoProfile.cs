using AutoMapper;
using BE.Data.Dtos.TaskDto;
using BE.Data.Models;

namespace BE.Data.Profiles.TaskProfile
{
    public class GetAllTaskDtoProfile: Profile
    {
        public GetAllTaskDtoProfile()
        {
            CreateMap<Tasks, GetAllTaskDto>();
        }
    }
}
