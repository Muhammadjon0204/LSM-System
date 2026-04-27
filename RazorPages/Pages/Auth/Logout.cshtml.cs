using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RazorPages.Pages.Auth;

public class LogoutModel : PageModel
{
    public async Task<IActionResult> OnPostAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Удаляем JWT cookie
        Response.Cookies.Delete("edu.jwt");
        Response.Cookies.Delete("edu.auth");

        TempData["Info"] = "Вы вышли из системы";
        return RedirectToPage("/Auth/Login");
    }

    // GET — защита от случайного выхода по ссылке
    public IActionResult OnGet() => RedirectToPage("/Index");
}