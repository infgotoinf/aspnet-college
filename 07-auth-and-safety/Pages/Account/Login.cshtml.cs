using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoManager.Models;

namespace TodoManager.Pages.Account;

public class LoginModel : PageModel
{
    private readonly SignInManager<AppUser> _signInManager;

    public LoginModel(SignInManager<AppUser> signInManager)
    {
        _signInManager = signInManager;
    }

    [BindProperty]
    public LoginInput Input { get; set; } = new();

    [BindProperty(SupportsGet = true)]
    public string? ReturnUrl { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var result = await _signInManager.PasswordSignInAsync(
            Input.Email,
            Input.Password,
            isPersistent: false,
            lockoutOnFailure: true);

        if (result.Succeeded)
        {
            return LocalRedirect(
                Url.IsLocalUrl(ReturnUrl) ? ReturnUrl! : Url.Content("~/"));
        }

        if (result.IsLockedOut)
        {
            ModelState.AddModelError(string.Empty, "Вход временно заблокирован.");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Неверный email или пароль.");
        }

        return Page();
    }

    public class LoginInput
    {
        [Required(ErrorMessage = "Укажите email.")]
        [EmailAddress(ErrorMessage = "Введите корректный email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите пароль.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}
