using Application.DTOs.CourseDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController(
    UserManager<ApplicationUser> userManager,
    ICourseService courseService) : Controller
{
    // ── DASHBOARD ──────────────────────────────────────────────────────────
    public async Task<IActionResult> Index()
    {
        var users   = await userManager.Users.ToListAsync();
        var courses = await courseService.GetAllAsync(new CourseFilterDto { PageSize = 1000 });

        ViewBag.TotalUsers   = users.Count;
        ViewBag.TotalCourses = courses.Data?.TotalCount ?? 0;
        ViewBag.TotalStudents = 0;

        foreach (var u in users)
            if (await userManager.IsInRoleAsync(u, UserRole.Student))
                ViewBag.TotalStudents = (int)ViewBag.TotalStudents + 1;

        return View();
    }

    // ── USERS ──────────────────────────────────────────────────────────────
    public async Task<IActionResult> Users()
    {
        var users = await userManager.Users.ToListAsync();
        var rows  = new List<AdminUserRow>();

        foreach (var u in users)
        {
            var roles = await userManager.GetRolesAsync(u);
            rows.Add(new AdminUserRow
            {
                Id       = u.Id,
                FullName = u.FullName,
                Email    = u.Email ?? "",
                Role     = roles.FirstOrDefault() ?? "—"
            });
        }

        return View(new AdminUsersViewModel { Users = rows });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeRole(int userId, string newRole)
    {
        var allowedRoles = new[] { UserRole.Student, UserRole.Instructor, UserRole.Admin };
        if (!allowedRoles.Contains(newRole))
        {
            TempData["Error"] = "Недопустимая роль";
            return RedirectToAction("Users");
        }

        var user = await userManager.FindByIdAsync(userId.ToString());
        if (user == null)
        {
            TempData["Error"] = "Пользователь не найден";
            return RedirectToAction("Users");
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        await userManager.RemoveFromRolesAsync(user, currentRoles);
        await userManager.AddToRoleAsync(user, newRole);

        TempData["Success"] = $"Роль пользователя {user.FullName} изменена на {newRole}";
        return RedirectToAction("Users");
    }

    // ── COURSES ────────────────────────────────────────────────────────────
    public async Task<IActionResult> Courses()
    {
        var result = await courseService.GetAllAsync(new CourseFilterDto { PageSize = 1000 });
        return View(new AdminCoursesViewModel
        {
            Courses = result.IsSuccess ? result.Data!.Items : new()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteCourse(int id)
    {
        var result = await courseService.DeleteAsync(id);
        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "Курс удалён" : result.Message;
        return RedirectToAction("Courses");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var result = await courseService.PublishAsync(id);
        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "Статус публикации изменён" : result.Message;
        return RedirectToAction("Courses");
    }
}