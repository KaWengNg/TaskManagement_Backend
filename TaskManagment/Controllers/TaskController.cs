using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TaskManagment.Data;
using TaskManagment.Dtos;
using TaskManagment.Models;

namespace TaskApi.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly TasksDbContext _db;
    private readonly IMapper _mapper;

    public TasksController(TasksDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    /// <summary>
    /// Create task
    /// </summary>
    /// <param name="dto"></param>
    /// <returns></returns>
    // POST /api/tasks
    [HttpPost]
    public async Task<ActionResult<ReadTaskDto>> Create(CreateTaskDto dto)
    {
        var entity = _mapper.Map<TaskItem>(dto);
        _db.Tasks.Add(entity);
        await _db.SaveChangesAsync();

        var result = _mapper.Map<ReadTaskDto>(entity);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, result);
    }

    /// <summary>
    /// Get all task with optional filters
    /// </summary>
    /// <param name="completed">filter by status</param>
    /// <param name="page">page number</param>
    /// <param name="pageSize">Number of item per page</param>
    /// <returns>Paginated list object</returns>
    // GET /api/tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadTaskDto>>> GetAll(
        [FromQuery] bool? completed,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        IQueryable<TaskItem> query = _db.Tasks;

        if (completed.HasValue)
            query = query.Where(t => t.Completed == completed.Value);

        var totalCount = await query.CountAsync();

        var tasks = await query
            .OrderByDescending(t => t.UpdatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var taskDtos = _mapper.Map<IEnumerable<ReadTaskDto>>(tasks);

        return Ok(new
        {
            total = totalCount,
            page,
            pageSize,
            tasks = taskDtos
        });
    }

    /// <summary>
    /// Get task by id
    /// </summary>
    /// <param name="id">Guid</param>
    /// <returns></returns>
    // GET /api/tasks/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReadTaskDto>> GetById(Guid id)
    {
        var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null) return NotFound();

        return Ok(_mapper.Map<ReadTaskDto>(entity));
    }

    /// <summary>
    /// Update task by id
    /// </summary>
    /// <param name="id">id</param>
    /// <param name="dto">update object</param>
    /// <returns></returns>
    // PUT /api/tasks/{id}
    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ReadTaskDto>> Update(Guid id, UpdateTaskDto dto)
    {
        var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null) return NotFound();

        _mapper.Map(dto, entity);
        await _db.SaveChangesAsync();

        return Ok(_mapper.Map<ReadTaskDto>(entity));
    }

    /// <summary>
    /// Delete task by id
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    // DELETE /api/tasks/{id}
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var entity = await _db.Tasks.FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null) return NotFound();

        _db.Tasks.Remove(entity);
        await _db.SaveChangesAsync();

        return NoContent();
    }
}


