using Microsoft.AspNetCore.Mvc;
using TaskFlowApi.Entities;

namespace TaskFlowApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProjectsController : ControllerBase
{
    private static readonly List<Project> Projects = new();

    /// <summary>
    /// Returns all projects.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Project>), StatusCodes.Status200OK)]
    public ActionResult<IEnumerable<Project>> GetAll()
    {
        return Ok(Projects);
    }

    /// <summary>
    /// Returns a project by ID.
    /// </summary>
    /// <param name="id">Project ID.</param>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(Project), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Project> GetById(int id)
    {
        var project = Projects.FirstOrDefault(project => project.Id == id);

        if (project is null)
        {
            return NotFound();
        }

        return Ok(project);
    }

    /// <summary>
    /// Creates a new project.
    /// </summary>
    /// <param name="project">Project data.</param>
    [HttpPost]
    [ProducesResponseType(typeof(Project), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public ActionResult<Project> Create(Project project)
    {
        project.Id = Projects.Count == 0
            ? 1
            : Projects.Max(existingProject => existingProject.Id) + 1;

        project.CreatedAt = DateTime.UtcNow;

        Projects.Add(project);

        return CreatedAtAction(
            nameof(GetById),
            new { id = project.Id },
            project);
    }

    /// <summary>
    /// Replaces an existing project.
    /// </summary>
    /// <param name="id">Project ID.</param>
    /// <param name="updatedProject">Replacement project data.</param>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Project> Update(int id, Project updatedProject)
    {
        var project = Projects.FirstOrDefault(project => project.Id == id);

        if (project is null)
        {
            return NotFound();
        }

        project.Name = updatedProject.Name;
        project.Description = updatedProject.Description;

        return NoContent();
    }

    /// <summary>
    /// Deletes a project.
    /// </summary>
    /// <param name="id">Project ID.</param>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<Project> Delete(int id)
    {
        var project = Projects.FirstOrDefault(project => project.Id == id);

        if (project is null)
        {
            return NotFound();
        }

        Projects.Remove(project);

        return NoContent();
    }
}
