using TaskManagment.Dtos;

namespace TaskManagment.Services
{
	public interface ITaskService
	{
        Task<ReadTaskDto> CreateAsync(CreateTaskDto dto);
        Task<(IEnumerable<ReadTaskDto> tasks, int totalCount)> GetAllAsync(bool? completed, int page, int pageSize);
        Task<ReadTaskDto?> GetByIdAsync(Guid id);
        Task<ReadTaskDto?> UpdateAsync(Guid id, UpdateTaskDto dto);
        Task<bool> DeleteAsync(Guid id);
    }
}

