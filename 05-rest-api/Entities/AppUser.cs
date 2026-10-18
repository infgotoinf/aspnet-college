using System.ComponentModel.DataAnnotations;

namespace TaskFlowApi.Entities;

public class AppUser
{
    public int Id { get; set; }

    [Required]
    public string Username { get; set; } = string.Empty;

    [Required]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;
}
