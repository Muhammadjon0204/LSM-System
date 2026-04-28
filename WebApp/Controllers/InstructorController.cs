using System.Security.Claims;
using Application.DTOs.CourseDTOs;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

[Authorize(Roles = "Instructor,Admin")]
public class InstructorController(ICourseService courseService) : Controller
{
    public async Task<IActionResult> Index()
    {
        var fullName = User.FindFirstValue("FullName") ?? User.FindFirstValue(ClaimTypes.Name) ?? "";
        var userId   = GetUserId();

        var all = await courseService.GetAllAsync(new CourseFilterDto { PageSize = 1000 });
        var my  = all.IsSuccess
            ? all.Data!.Items.Where(c => c.InstructorName == fullName).ToList()
            : new();

        return View(new InstructorCoursesViewModel
        {
            Courses         = my,
            InstructorName  = fullName,
            AddForm         = new AddCourseViewModel()
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(AddCourseViewModel form)
    {
        if (!ModelState.IsValid)
        {
            var fullName = User.FindFirstValue("FullName") ?? "";
            var all = await courseService.GetAllAsync(new CourseFilterDto { PageSize = 1000 });
            return View("Index", new InstructorCoursesViewModel
            {
                Courses        = all.IsSuccess ? all.Data!.Items.Where(c => c.InstructorName == fullName).ToList() : new(),
                InstructorName = fullName,
                AddForm        = form
            });
        }

        var result = await courseService.AddAsync(new AddCourseDto
        {
            Title       = form.Title,
            Description = form.Description,
            Price       = form.Price,
            Level       = form.Level,
            CategoryId  = form.CategoryId
        });

        if (result.IsSuccess)
            TempData["Success"] = $"Курс «{result.Data!.Title}» создан!";
        else
            TempData["Error"] = result.Message;

        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await courseService.DeleteAsync(id);
        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "Курс удалён" : result.Message;
        return RedirectToAction("Index");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> TogglePublish(int id)
    {
        var result = await courseService.PublishAsync(id);
        TempData[result.IsSuccess ? "Success" : "Error"] =
            result.IsSuccess ? "Статус изменён" : result.Message;
        return RedirectToAction("Index");
    }

    private int GetUserId()
    {
        var idStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        return int.TryParse(idStr, out var id) ? id : 0;
    }
}