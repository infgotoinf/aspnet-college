using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Entities;

public class Project
{
    public int Id { get; set; }

    /// <summary>
    /// Project name.
    /// </summary>
    [Required]
    [StringLength(100, MinimumLength = 3)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Project description.
    /// </summary>
    public string? Description { get; set; }

    public DateTime CreatedAt { get; set; }
}
