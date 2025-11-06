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

        public TaskService(TasksDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task<ReadTaskDto> CreateAsync(CreateTaskDto dto)
        {
            var entity = _mapper.Map<TaskItem>(dto);
            _db.Tasks.Add(entity);
            await _db.SaveChangesAsync();
            return _mapper.Map<ReadTaskDto>(entity);
        }

        public async Task<(IEnumerable<ReadTaskDto> tasks, int totalCount)> GetAllAsync(bool? completed, int page, int pageSize)
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

            var dtoList = _mapper.Map<IEnumerable<ReadTaskDto>>(entities);

            return (dtoList, totalCount);
        }

        public async Task<ReadTaskDto?> GetByIdAsync(Guid id)
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            return entity == null ? null : _mapper.Map<ReadTaskDto>(entity);
        }

        public async Task<ReadTaskDto?> UpdateAsync(Guid id, UpdateTaskDto dto)
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null) // to do response serviceresult instead
                return null;

            _mapper.Map(dto, entity);
            await _db.SaveChangesAsync();

            return _mapper.Map<ReadTaskDto>(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
                return false;

            _db.Tasks.Remove(entity);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
