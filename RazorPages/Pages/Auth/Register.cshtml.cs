using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.DTOs.AuthDTOs;
using Application.Interfaces.Services.AuthService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPages.Pages.Auth;

public class RegisterModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public RegisterInputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Courses/Index");

        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        if (!ModelState.IsValid)
            return Page();

        if (Input.Password != Input.ConfirmPassword)
        {
            ModelState.AddModelError("Input.ConfirmPassword", "Пароли не совпадают");
            return Page();
        }

        var result = await authService.RegisterAsync(new RegisterDto
        {
            FullName = Input.FullName,
            Email    = Input.Email,
            Password = Input.Password
        });

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        var data = result.Data!;

        // Создаём Cookie-сессию
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,  data.Email),
            new(ClaimTypes.Email, data.Email),
            new("FullName",       data.FullName),
            new(ClaimTypes.Role,  data.Role),
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties { IsPersistent = true, ExpiresUtc = DateTimeOffset.UtcNow.AddHours(8) });

        // JWT в HttpOnly cookie
        Response.Cookies.Append("edu.jwt", data.Token, new CookieOptions
        {
            HttpOnly = true,
            Secure   = true,
            SameSite = SameSiteMode.Strict,
            Expires  = DateTimeOffset.UtcNow.AddHours(8)
        });

        TempData["Success"] = $"Добро пожаловать, {data.FullName}! Вы успешно зарегистрированы.";
        return RedirectToPage("/Courses/Index");
    }
}

public class RegisterInputModel
{
    [Required(ErrorMessage = "Введите полное имя")]
    [MinLength(2, ErrorMessage = "Имя слишком короткое")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [MinLength(6, ErrorMessage = "Минимум 6 символов")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    public string ConfirmPassword { get; set; } = string.Empty;
}