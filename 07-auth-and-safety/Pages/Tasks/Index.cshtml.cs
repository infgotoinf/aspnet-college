using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoManager.Data;
using TodoManager.Models;

namespace TodoManager.Pages.Tasks;

[Authorize]
public class IndexModel : PageModel
{
    private readonly ApplicationDbContext _db;
    private readonly UserManager<AppUser> _userManager;

    public IndexModel(
        ApplicationDbContext db,
        UserManager<AppUser> userManager)
    {
        _db = db;
        _userManager = userManager;
    }

    public IList<TaskItem> Tasks { get; private set; } = new List<TaskItem>();

    public async Task OnGetAsync()
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return;
        }

        Tasks = await _db.TaskItems
            .Where(task => task.UserId == userId)
            .OrderByDescending(task => task.CreatedAt)
            .ToListAsync();
    }

    public async Task<IActionResult> OnPostCompleteAsync(int id)
    {
        var userId = _userManager.GetUserId(User);

        if (userId is null)
        {
            return Unauthorized();
        }

        var task = await _db.TaskItems
            .SingleOrDefaultAsync(item => item.Id == id && item.UserId == userId);

        if (task is null)
        {
            return NotFound();
        }

        task.IsCompleted = true;
        await _db.SaveChangesAsync();

        return new JsonResult(new { success = true });
    }
}
