using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TodoManager.Models;

namespace TodoManager.Pages.Account;

public class RegisterModel : PageModel
{
    private readonly UserManager<AppUser> _userManager;
    private readonly SignInManager<AppUser> _signInManager;

    public RegisterModel(
        UserManager<AppUser> userManager,
        SignInManager<AppUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    [BindProperty]
    public RegisterInput Input { get; set; } = new();

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
        {
            return Page();
        }

        var user = new AppUser
        {
            FullName = Input.FullName,
            UserName = Input.Email,
            Email = Input.Email
        };

        var createResult = await _userManager.CreateAsync(user, Input.Password);

        if (!createResult.Succeeded)
        {
            foreach (var error in createResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "User");

        if (!roleResult.Succeeded)
        {
            foreach (var error in roleResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return Page();
        }

        await _signInManager.SignInAsync(user, isPersistent: false);
        return RedirectToPage("/Index");
    }

    public class RegisterInput
    {
        [Required(ErrorMessage = "Укажите имя.")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите email.")]
        [EmailAddress(ErrorMessage = "Введите корректный email.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Укажите пароль.")]
        [MinLength(6, ErrorMessage = "Пароль должен содержать не менее 6 символов.")]
        [RegularExpression(@".*\d.*", ErrorMessage = "Пароль должен содержать хотя бы одну цифру.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Подтвердите пароль.")]
        [Compare(nameof(Password), ErrorMessage = "Пароли не совпадают.")]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; } = string.Empty;
    }
}
