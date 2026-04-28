using System.ComponentModel.DataAnnotations;
using Application.DTOs.CourseDTOs;
using Application.DTOs.EnrollmentDTOs;
using Domain.Constants;

namespace WebApp.Models;

// ── AUTH ─────────────────────────────────────────────────────────────────────

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [MinLength(6, ErrorMessage = "Минимум 6 символов")]
    public string Password { get; set; } = string.Empty;

    public string? ReturnUrl { get; set; }
    public string? ErrorMessage { get; set; }
}

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите полное имя")]
    [MinLength(2, ErrorMessage = "Слишком короткое")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите пароль")]
    [MinLength(6, ErrorMessage = "Минимум 6 символов")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; } = string.Empty;

    public string? ErrorMessage { get; set; }
}

public class ForgotPasswordViewModel
{
    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Некорректный email")]
    public string Email { get; set; } = string.Empty;

    public bool EmailSent { get; set; }
    public string? SentToEmail { get; set; }
}

public class ResetPasswordViewModel
{
    [Required] [EmailAddress]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите код")]
    [StringLength(6, MinimumLength = 6, ErrorMessage = "Код — 6 цифр")]
    public string ResetCode { get; set; } = string.Empty;

    [Required(ErrorMessage = "Введите новый пароль")]
    [MinLength(6, ErrorMessage = "Минимум 6 символов")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Подтвердите пароль")]
    [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmNewPassword { get; set; } = string.Empty;

    public bool ResetSuccess { get; set; }
    public string? ErrorMessage { get; set; }
}

// ── COURSES ───────────────────────────────────────────────────────────────────

public class CourseListViewModel
{
    public List<GetCourseDto> Courses { get; set; } = new();
    public CourseFilterDto    Filter  { get; set; } = new();
    public int TotalCount  { get; set; }
    public int TotalPages  { get; set; }
    public HashSet<int> EnrolledCourseIds { get; set; } = new();
}

public class CourseDetailViewModel
{
    public GetCourseDto Course { get; set; } = null!;
    public bool IsEnrolled  { get; set; }
    public bool IsStudent   { get; set; }
    public GetEnrollmentDto? Enrollment { get; set; }
}

// ── STUDENT ───────────────────────────────────────────────────────────────────

public class MyCoursesViewModel
{
    public List<GetEnrollmentDto> Enrollments { get; set; } = new();
    public string FullName { get; set; } = string.Empty;
}

// ── ADMIN ─────────────────────────────────────────────────────────────────────

public class AdminUsersViewModel
{
    public List<AdminUserRow> Users { get; set; } = new();
}

public class AdminUserRow
{
    public int    Id       { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Email    { get; set; } = string.Empty;
    public string Role     { get; set; } = string.Empty;
}

public class AdminCoursesViewModel
{
    public List<GetCourseDto> Courses { get; set; } = new();
}

// ── INSTRUCTOR ────────────────────────────────────────────────────────────────

public class InstructorCoursesViewModel
{
    public List<GetCourseDto> Courses     { get; set; } = new();
    public string InstructorName          { get; set; } = string.Empty;
    public AddCourseViewModel? AddForm    { get; set; }
}

public class AddCourseViewModel
{
    [Required(ErrorMessage = "Введите название")]
    public string Title { get; set; } = string.Empty;

    public string? Description { get; set; }

    [Range(0, 999999)]
    public decimal Price { get; set; }

    public CourseLevel Level { get; set; }
    public int CategoryId   { get; set; }
    public string? ErrorMessage { get; set; }
}