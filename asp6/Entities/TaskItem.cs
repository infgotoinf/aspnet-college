using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Entities;

public class TaskItem
{
    public int Id { get; set; }

    /// <summary>
    /// Task title.
    /// </summary>
    [Required]
    [MinLength(5)]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Task description.
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Task status.
    /// </summary>
    public TaskStatus? Status { get; set; }

    public int? ProjectId { get; set; }

    public int? AssignedToId { get; set; }

    /// <summary>
    /// Optional due date. It must be in the future.
    /// </summary>
    [CustomValidation(typeof(TaskItem), nameof(ValidateDueDate))]
    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public static ValidationResult? ValidateDueDate(
        DateTime? dueDate,
        ValidationContext context)
    {
        return dueDate.HasValue && dueDate.Value <= DateTime.UtcNow
            ? new ValidationResult("DueDate must be in the future.")
            : ValidationResult.Success;
    }
}
