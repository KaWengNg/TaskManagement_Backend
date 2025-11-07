using TaskManagment.Dtos;

namespace TaskManagment.Services
{
	public interface ITaskService
	{
        Task<ServiceResult<ReadTaskDto>> CreateAsync(CreateTaskDto dto);
        Task<ServiceResult<PagedResultDto<ReadTaskDto>>> GetAllAsync(bool? completed, int page, int pageSize);
        Task<ServiceResult<ReadTaskDto>> GetByIdAsync(Guid id);
        Task<ServiceResult<ReadTaskDto>> UpdateAsync(Guid id, UpdateTaskDto dto);
        Task<ServiceResult<bool>> DeleteAsync(Guid id);
    }
}

