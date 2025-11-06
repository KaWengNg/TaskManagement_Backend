using Microsoft.AspNetCore.Mvc;
using TaskManagment.Dtos;
using TaskManagment.Services;

namespace TaskApi.Controllers;

[ApiController]
[Route("api/tasks")]
public class TasksController : ControllerBase
{
    private readonly ITaskService _taskService;

    public TasksController(ITaskService taskService)
    {
        _taskService = taskService;
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
        var result = await _taskService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
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
        var (tasks, total) = await _taskService.GetAllAsync(completed, page, pageSize);
        return Ok(new { total, page, pageSize, tasks });
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
        var result = await _taskService.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
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
        var result = await _taskService.UpdateAsync(id, dto);
        if (result == null) return NotFound();
        return Ok(result);
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
        var deleted = await _taskService.DeleteAsync(id);
        if (!deleted) return NotFound();
        return NoContent();
    }
}


