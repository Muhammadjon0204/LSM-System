using System.Security.Claims;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize(Roles = "Student")]
public class StudentController(
    IEnrollmentService enrollmentService,
    IStudentRepository studentRepository) : Controller
{
    public async Task<IActionResult> MyCourses()
    {
        var email   = User.FindFirstValue(ClaimTypes.Email) ?? User.FindFirstValue(ClaimTypes.Name) ?? "";
        var userId  = GetUserId();
        var student = await studentRepository.GetByUserIdAsync(userId);

        if (student == null)
        {
            TempData["Error"] = "Профиль студента не найден";
            return RedirectToAction("Index", "Courses");
        }

        var result = await enrollmentService.GetByStudentIdAsync(student.Id);

        return View(new MyCoursesViewModel
        {
            Enrollments = result.IsSuccess ? result.Data! : new(),
            FullName    = $"{student.FirstName} {student.LastName}"
        });
    }

    private int GetUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : 0;
    }
}