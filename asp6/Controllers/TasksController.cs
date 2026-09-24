using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Entities;

namespace TaskFlowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TasksController : ControllerBase
{
    private static readonly List<TaskItem> Tasks = new();

    /// <summary>
    /// Returns all tasks.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<TaskItem>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<TaskItem>> GetAll()
    {
        return Ok(Tasks);
    }

    /// <summary>
    /// Returns a task by ID.
    /// </summary>
    /// <param name="id">Task ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TaskItem), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TaskItem> GetById(int id)
    {
        var task = Tasks.FirstOrDefault(task => task.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        return Ok(task);
    }

    /// <summary>
    /// Creates a new task.
    /// </summary>
    /// <param name="task">Task data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(TaskItem), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<TaskItem> Create(TaskItem task)
    {
        task.Id = Tasks.Count == 0
            ? 1
            : Tasks.Max(existingTask => existingTask.Id) + 1;

        task.Status ??= TaskFlowApi.Entities.TaskStatus.ToDo;
        task.CreatedAt = DateTime.UtcNow;

        Tasks.Add(task);

        return CreatedAtAction(
            nameof(GetById),
            new { id = task.Id },
            task);
    }

    /// <summary>
    /// Replaces an existing task.
    /// </summary>
    /// <param name="id">Task ID.</param>
    /// <param name="updatedTask">Replacement task data.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TaskItem> Update(int id, TaskItem updatedTask)
    {
        if (updatedTask.Status is null)
        {
            ModelState.AddModelError(
                nameof(TaskItem.Status),
                "Status is required when replacing a task.");

            return ValidationProblem(ModelState);
        }

        var task = Tasks.FirstOrDefault(task => task.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        task.Title = updatedTask.Title;
        task.Description = updatedTask.Description;
        task.Status = updatedTask.Status;
        task.ProjectId = updatedTask.ProjectId;
        task.AssignedToId = updatedTask.AssignedToId;
        task.DueDate = updatedTask.DueDate;

        return NoContent();
    }

    /// <summary>
    /// Deletes a task.
    /// </summary>
    /// <param name="id">Task ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<TaskItem> Delete(int id)
    {
        var task = Tasks.FirstOrDefault(task => task.Id == id);

        if (task is null)
        {
            return NotFound();
        }

        Tasks.Remove(task);

        return NoContent();
    }
}
