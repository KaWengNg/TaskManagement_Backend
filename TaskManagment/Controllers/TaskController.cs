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

    // GET /api/tasks
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ReadTaskDto>>> GetAll([FromQuery] bool? completed)
    {
        IQueryable<TaskItem> query = _db.Tasks.AsNoTracking();

        if (completed.HasValue)
            query = query.Where(t => t.Completed == completed.Value);

        var tasks = await query
            .OrderByDescending(t => t.UpdatedAt)
            .ToListAsync();

        return Ok(_mapper.Map<IEnumerable<ReadTaskDto>>(tasks));
    }

    // GET /api/tasks/{id}
    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ReadTaskDto>> GetById(Guid id)
    {
        var entity = await _db.Tasks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
        if (entity == null) return NotFound();

        return Ok(_mapper.Map<ReadTaskDto>(entity));
    }

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


