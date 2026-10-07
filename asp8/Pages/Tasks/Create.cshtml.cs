using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoManager.Data;
using TodoManager.Models;

namespace TodoManager.Pages.Tasks;

[Authorize]
public class CreateModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public CreateModel(
        ApplicationDbContext db,
        UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    [BindProperty]
    public CreateInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Challenge();
        }

        var task = new TaskItem
        {
            Title = Input.Title,
            Description = Input.Description,
            UserId = userId
        };

        _db.TaskItems.Add(task);
        await _db.SaveChangesAsync();

        return RedirectToPage("/Tasks/Index");
    }

    public class CreateInput
    {
        [Required(ErrorMessage = "Укажите название задачи.")]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
