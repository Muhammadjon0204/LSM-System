using System.Security.Claims;
using Application.DTOs.CourseDTOs;
using Application.DTOs.EnrollmentDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebApp.Models;

namespace WebApp.Controllers;

public class CoursesController(
    ICourseService courseService,
    IEnrollmentService enrollmentService,
    IStudentRepository studentRepository) : Controller
{
    public async Task<IActionResult> Index(CourseFilterDto filter)
    {
        filter.IsPublished = true; // студенты видят только опубликованные
        filter.PageSize    = 9;

        var result = await courseService.GetAllAsync(filter);
        if (!result.IsSuccess)
        {
            TempData["Error"] = result.Message;
            return View(new CourseListViewModel { Filter = filter });
        }

        var enrolled = new HashSet<int>();

        if (User.Identity?.IsAuthenticated == true && User.IsInRole("Student"))
        {
            var userId  = GetUserId();
            var student = await studentRepository.GetByUserIdAsync(userId);
            if (student != null)
            {
                var myEnrollments = await enrollmentService.GetByStudentIdAsync(student.Id);
                if (myEnrollments.IsSuccess)
                    enrolled = myEnrollments.Data!.Select(e => e.Id).ToHashSet();
            }
        }

        var paged = result.Data!;
        return View(new CourseListViewModel
        {
            Courses           = paged.Items,
            Filter            = filter,
            TotalCount        = paged.TotalCount,
            TotalPages        = (int)Math.Ceiling((double)paged.TotalCount / filter.PageSize),
            EnrolledCourseIds = enrolled
        });
    }

    public async Task<IActionResult> Detail(int id)
    {
        var result = await courseService.GetByIdAsync(id);
        if (!result.IsSuccess)
        {
            TempData["Error"] = "Курс не найден";
            return RedirectToAction("Index");
        }

        var vm = new CourseDetailViewModel
        {
            Course    = result.Data!,
            IsStudent = User.IsInRole("Student")
        };

        if (User.Identity?.IsAuthenticated == true && User.IsInRole("Student"))
        {
            var userId  = GetUserId();
            var student = await studentRepository.GetByUserIdAsync(userId);
            if (student != null)
            {
                var enrollments = await enrollmentService.GetByStudentIdAsync(student.Id);
                if (enrollments.IsSuccess)
                {
                    var en = enrollments.Data!.FirstOrDefault(e => e.CourseTitle == result.Data!.Title);
                    vm.IsEnrolled  = en != null;
                    vm.Enrollment  = en;
                }
            }
        }

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Enroll(int courseId)
    {
        var userId  = GetUserId();
        var student = await studentRepository.GetByUserIdAsync(userId);

        if (student == null)
        {
            TempData["Error"] = "Профиль студента не найден";
            return RedirectToAction("Index");
        }

        var result = await enrollmentService.EnrollAsync(new AddEnrollmentDto
        {
            CourseId  = courseId,
            StudentId = student.Id
        });

        if (result.IsSuccess)
            TempData["Success"] = $"Вы записались на курс «{result.Data!.CourseTitle}»!";
        else
            TempData["Error"] = result.Message;

        return RedirectToAction("Index");
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(int enrollmentId)
    {
        var result = await enrollmentService.CancelAsync(enrollmentId);

        if (result.IsSuccess)
            TempData["Success"] = "Запись отменена";
        else
            TempData["Error"] = result.Message;

        return RedirectToAction("MyCourses", "Student");
    }

    private int GetUserId()
        => int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
           ?? User.FindFirstValue(ClaimTypes.Name) ?? "0");
}