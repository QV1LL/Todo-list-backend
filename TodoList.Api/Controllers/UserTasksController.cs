using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TodoList.Api.Entities;
using TodoList.Api.Persistence;

namespace TodoList.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserTaskController : ControllerBase
{
    private readonly TodoListContext _context;

    public UserTaskController(TodoListContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ActionName("GetUserTasks")]
    public async Task<ActionResult<IEnumerable<UserTask>>> GetUserTasksAsync()
    {
        return Ok(await _context.UserTasks
                             .AsNoTracking()
                             .ToListAsync());
    }

    [HttpGet("{id}")]
    [ActionName("GetUserTaskById")]
    public async Task<ActionResult<UserTask>> GetUserTaskByIdAsync(Guid id)
    {
        var task = await _context.UserTasks
                                 .FirstOrDefaultAsync(x => x.Id == id);

        return task == null ? NotFound() : Ok(task);
    }

    [HttpPost]
    [ActionName("CreateUserTask")]
    public async Task<IActionResult> CreateUserTaskAsync([FromBody] UserTask userTask)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        userTask.Id = Guid.NewGuid();
        await _context.UserTasks.AddAsync(userTask);
        await _context.SaveChangesAsync();

        return CreatedAtAction("GetUserTaskById", new { id = userTask.Id }, userTask);
    }

    [HttpPut("{id}")]
    [ActionName("UpdateUserTask")]
    public async Task<IActionResult> UpdateUserTaskAsync(Guid id, [FromBody] UserTask updatedTask)
    {
        if (id != updatedTask.Id)
            return BadRequest("ID mismatch");

        var existingTask = await _context.UserTasks.FindAsync(id);
        if (existingTask == null)
            return NotFound();

        try
        {
            existingTask.Name = updatedTask.Name;

            await _context.SaveChangesAsync();
        }
        catch (ArgumentNullException ex)
        {
            return BadRequest(ex.Message);
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    [ActionName("DeleteUserTask")]
    public async Task<IActionResult> DeleteUserTaskAsync(Guid id)
    {
        await _context.UserTasks
                      .Where(t => t.Id == id)
                      .ExecuteDeleteAsync();

        return NoContent();
    }
}
