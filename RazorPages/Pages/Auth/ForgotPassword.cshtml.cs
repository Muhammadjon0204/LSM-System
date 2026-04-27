using System.ComponentModel.DataAnnotations;
using Application.DTOs.AuthDTOs;
using Application.Interfaces.Services.AuthService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPages.Pages.Auth;

public class ForgotPasswordModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public ForgotInputModel Input { get; set; } = new();

    public bool   EmailSent    { get; set; }
    public string? SentToEmail { get; set; }

    public void OnGet() { }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        // Всегда показываем успех — не раскрываем существование email (security best practice)
        await authService.ForgotPasswordAsync(new ForgotPasswordDto { Email = Input.Email });

        EmailSent    = true;
        SentToEmail  = Input.Email;
        return Page();
    }
}

public class ForgotInputModel
{
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;
}