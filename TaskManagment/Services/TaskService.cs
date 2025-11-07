using AutoMapper;
using Microsoft.EntityFrameworkCore;
using TaskManagment.Data;
using TaskManagment.Dtos;
using TaskManagment.Models;

namespace TaskManagment.Services
{
    public class TaskService : ITaskService
    {
        private readonly TasksDbContext _db;
        private readonly IMapper _mapper;
        private readonly ILogger<TaskService> _logger;

        public TaskService(TasksDbContext db, IMapper mapper, ILogger<TaskService> logger)
        {
            _db = db;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ServiceResult<ReadTaskDto>> CreateAsync(CreateTaskDto dto)
        {
            var entity = _mapper.Map<TaskItem>(dto);
            _db.Tasks.Add(entity);
            await _db.SaveChangesAsync();
            var result = _mapper.Map<ReadTaskDto>(entity);

            _logger.LogInformation($"Task {result.Title} created successfully.");
            return ServiceResult<ReadTaskDto>.Success(result, "Task created successfully.");
        }

        public async Task<ServiceResult<PagedResultDto<ReadTaskDto>>> GetAllAsync(bool? completed, int page, int pageSize)
        {
            IQueryable<TaskItem> query = _db.Tasks;

            if (completed.HasValue)
                query = query.Where(t => t.Completed == completed.Value);

            var totalCount = await query.CountAsync();

            var entities = await query
                .OrderByDescending(t => t.UpdatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
            .ToListAsync();

            var mapped = _mapper.Map<IEnumerable<ReadTaskDto>>(entities);

            var pagedResult = new PagedResultDto<ReadTaskDto>
            {
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                Items = mapped
            };

            return ServiceResult<PagedResultDto<ReadTaskDto>>.Success(pagedResult);
        }

        public async Task<ServiceResult<ReadTaskDto>> GetByIdAsync(Guid id)
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
            {
                _logger.LogWarning("Get failed: Task {TaskId} not found.", id);
                return ServiceResult<ReadTaskDto>.NotFound($"Task with ID {id} not found.");
            }

            var dto = _mapper.Map<ReadTaskDto>(entity);
            return ServiceResult<ReadTaskDto>.Success(dto);
        }

        public async Task<ServiceResult<ReadTaskDto>> UpdateAsync(Guid id, UpdateTaskDto dto)
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
            {
                _logger.LogWarning("Update failed: Task {TaskId} not found.", id);
                return ServiceResult<ReadTaskDto>.NotFound($"Task with ID {id} not found.");
            }

            _mapper.Map(dto, entity);
            await _db.SaveChangesAsync();
            var result = _mapper.Map<ReadTaskDto>(entity);

            _logger.LogInformation($"Task Id {result.Id} updated successfully.");
            return ServiceResult<ReadTaskDto>.Success(result, "Task updated successfully.");
        }

        public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);

            if (entity == null)
            {
                _logger.LogWarning("Delete failed: Task {TaskId} not found.", id);
                return ServiceResult<bool>.NotFound($"Task with ID {id} not found.");
            }

            _db.Tasks.Remove(entity);
            await _db.SaveChangesAsync();

            _logger.LogInformation($"Task {entity.Title} deleted successfully.");
            return ServiceResult<bool>.Success(true, "Task deleted successfully.");
        }
    }
}
