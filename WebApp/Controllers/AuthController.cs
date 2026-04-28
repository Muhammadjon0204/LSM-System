using System.Security.Claims;
using Application.DTOs.AuthDTOs;
using Application.Interfaces.Services.AuthService;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class AuthController(IAuthService authService) : Controller
{
    // ── LOGIN ──────────────────────────────────────────────────────────────
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Courses");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await authService.LoginAsync(new LoginDto
        {
            Email    = vm.Email,
            Password = vm.Password
        });

        if (!result.IsSuccess)
        {
            vm.ErrorMessage = result.Message;
            return View(vm);
        }

        await SignInAsync(result.Data!);

        TempData["Success"] = $"Добро пожаловать, {result.Data!.FullName}!";

        if (!string.IsNullOrEmpty(vm.ReturnUrl) && Url.IsLocalUrl(vm.ReturnUrl))
            return Redirect(vm.ReturnUrl);

        return result.Data!.Role switch
        {
            "Admin"      => RedirectToAction("Index", "Admin"),
            "Instructor" => RedirectToAction("Index", "Instructor"),
            _            => RedirectToAction("Index", "Courses")
        };
    }

    // ── REGISTER ───────────────────────────────────────────────────────────
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Courses");

        return View(new RegisterViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await authService.RegisterAsync(new RegisterDto
        {
            FullName = vm.FullName,
            Email    = vm.Email,
            Password = vm.Password
        });

        if (!result.IsSuccess)
        {
            vm.ErrorMessage = result.Message;
            return View(vm);
        }

        await SignInAsync(result.Data!);

        TempData["Success"] = $"Добро пожаловать, {result.Data!.FullName}! Вы успешно зарегистрированы.";
        return RedirectToAction("Index", "Courses");
    }

    // ── FORGOT PASSWORD ────────────────────────────────────────────────────
    public IActionResult ForgotPassword() => View(new ForgotPasswordViewModel());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        await authService.ForgotPasswordAsync(new ForgotPasswordDto { Email = vm.Email });

        vm.EmailSent   = true;
        vm.SentToEmail = vm.Email;
        return View(vm);
    }

    // ── RESET PASSWORD ─────────────────────────────────────────────────────
    public IActionResult ResetPassword(string? email = null)
        => View(new ResetPasswordViewModel { Email = email ?? string.Empty });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel vm)
    {
        if (!ModelState.IsValid) return View(vm);

        var result = await authService.ResetPasswordAsync(new ResetPasswordDto
        {
            Email              = vm.Email,
            ResetCode          = vm.ResetCode,
            NewPassword        = vm.NewPassword,
            ConfirmNewPassword = vm.ConfirmNewPassword
        });

        if (!result.IsSuccess)
        {
            vm.ErrorMessage = result.Message;
            return View(vm);
        }

        vm.ResetSuccess = true;
        return View(vm);
    }

    // ── LOGOUT ─────────────────────────────────────────────────────────────
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        Response.Cookies.Delete("edu.auth");
        TempData["Info"] = "Вы вышли из системы";
        return RedirectToAction("Login");
    }

    public IActionResult AccessDenied() => View();

    // ── HELPER ─────────────────────────────────────────────────────────────
    private async Task SignInAsync(Application.DTOs.AuthDTOs.AuthResponseDto data)
    {
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
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc   = DateTimeOffset.UtcNow.AddHours(8)
            });
    }
}