using System.ComponentModel.DataAnnotations;
using System.Security.Claims;
using Application.DTOs.AuthDTOs;
using Application.Interfaces.Services.AuthService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPages.Pages.Auth;

public class LoginModel(IAuthService authService) : PageModel
{
    [BindProperty]
    public LoginInputModel Input { get; set; } = new();

    public string? ErrorMessage { get; set; }

    public IActionResult OnGet(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToPage("/Courses/Index");

        ViewData["ReturnUrl"] = returnUrl;
        return Page();
    }

    public async Task<IActionResult> OnPostAsync(string? returnUrl = null)
    {
        if (!ModelState.IsValid)
            return Page();

        var result = await authService.LoginAsync(new LoginDto
        {
            Email    = Input.Email,
            Password = Input.Password
        });

        if (!result.IsSuccess)
        {
            ErrorMessage = result.Message;
            return Page();
        }

        var data = result.Data!;

        // Создаём Cookie-сессию (ClaimsIdentity)
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name,              data.Email),
            new(ClaimTypes.Email,             data.Email),
            new("FullName",                   data.FullName),
            new(ClaimTypes.Role,              data.Role),
        };

        var identity  = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc   = DateTimeOffset.UtcNow.AddHours(8)
            });

        // Также сохраняем JWT в отдельную HttpOnly cookie (для возможного API-доступа)
        Response.Cookies.Append("edu.jwt", data.Token, new CookieOptions
        {
            HttpOnly  = true,
            Secure    = true,
            SameSite  = SameSiteMode.Strict,
            Expires   = DateTimeOffset.UtcNow.AddHours(8)
        });

        TempData["Success"] = $"Добро пожаловать, {data.FullName}!";

        if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            return Redirect(returnUrl);

        // Роль-ориентированный редирект
        return data.Role switch
        {
            "Admin"      => RedirectToPage("/Admin/Index"),
            "Instructor" => RedirectToPage("/Instructor/Index"),
            _            => RedirectToPage("/Courses/Index")
        };
    }
}

public class LoginInputModel
{
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [MinLength(6, ErrorMessage = "Минимум 6 символов")]
    public string Password { get; set; } = string.Empty;
}