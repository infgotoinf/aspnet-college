using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TodoManager.Models;

namespace TodoManager.Pages.Admin;

[Authorize(Roles = "Admin")]
public class IndexModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;

    public IndexModel(UserManager<AppUser> userManager)
    {
        _userManager = userManager;
    }

    public List<UserRow> Users { get; private set; } = new();

    public async Task OnGetAsync()
    {
        var users = await _userManager.Users
            .OrderBy(user => user.Email)
            .ToListAsync();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);

            Users.Add(new UserRow
            {
                FullName = user.FullName ?? string.Empty,
                Email = user.Email ?? string.Empty,
                Roles = string.Join(", ", roles)
            });
        }
    }

    public class UserRow
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Roles { get; set; } = string.Empty;
    }
}
