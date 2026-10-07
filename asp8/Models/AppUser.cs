using Microsoft.AspNetCore.Identity;

namespace TodoManager.Models;

public class AppUser : IdentityUser
{
    public string? FullName { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
