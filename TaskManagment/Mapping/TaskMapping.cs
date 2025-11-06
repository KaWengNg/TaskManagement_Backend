using AutoMapper;
using TaskManagment.Dtos;
using TaskManagment.Models;

namespace TaskManagment.Mapping;

public class TaskMapping:Profile
{
    public TaskMapping()
    {
        CreateMap<CreateTaskDto, TaskItem>()
            .ForMember(dest => dest.Completed, opt => opt.MapFrom(_ => false))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow))
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<UpdateTaskDto, TaskItem>()
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(_ => DateTime.UtcNow));

        CreateMap<TaskItem, ReadTaskDto>();
    }
}
