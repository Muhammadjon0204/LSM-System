using System.ComponentModel.DataAnnotations;
using Application.DTOs.AuthDTOs;
using Application.Interfaces.Services.AuthService;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPages.Pages.Auth;

public class ResetPasswordModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public ResetInputModel Input { get; set; } = new();

    public string?  ErrorMessage  { get; set; }
    public bool     ResetSuccess  { get; set; }
    public string?  Email         { get; set; }

    public void OnGet(string? email = null)
    {
        Email        = email;
        Input.Email  = email ?? string.Empty;
    }

    public async Task<IActionResult> OnPostAsync()
    {
        Email = Input.Email;

        if (!ModelState.IsValid)
            return Page();

        if (Input.NewPassword != Input.ConfirmNewPassword)
        {
            ModelState.AddModelError("Input.ConfirmNewPassword", "Пароли не совпадают");
            return Page();
        }

        var result = await authService.ResetPasswordAsync(new ResetPasswordDto
        {
            Email              = Input.Email,
            ResetCode          = Input.ResetCode,
            NewPassword        = Input.NewPassword,
            ConfirmNewPassword = Input.ConfirmNewPassword
        });

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        ResetSuccess = true;
        return Page();
    }
}

public class ResetInputModel
{
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите код подтверждения")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Код состоит из 6 цифр")]
    public string ResetCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите новый пароль")]
    [MinLength(6, ErrorMessage = "Минимум 6 символов")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}