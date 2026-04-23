using Application.Common;
using Application.DTOs.DashboardDTOs;
using Application.Interfaces.Services;
using Domain.Constants;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services;

public class DashboardService(
    AppDbContext context,
    ILogger<DashboardService> logger) : IDashboardService
{
    public async Task<Result<DashboardSummaryDto>> GetSummaryAsync()
    {
        try
        {
            var usersInRole = await context.UserRoles.ToListAsync();
            var roles = await context.Roles.ToListAsync();

            var studentRoleId = roles.First(r => r.Name == UserRole.Student).Id;
            var instructorRoleId = roles.First(r => r.Name == UserRole.Instructor).Id;

            var totalStudents = usersInRole.Count(ur => ur.RoleId == studentRoleId);
            var totalInstructors = usersInRole.Count(ur => ur.RoleId == instructorRoleId);

            var enrollments = await context.Enrollments.ToListAsync();
            var reviews = await context.Reviews.ToListAsync();
            var courses = await context.Courses.ToListAsync();

            var dto = new DashboardSummaryDto
            {
                TotalCourses = courses.Count,
                PublishedCourses = courses.Count(c => c.IsPublished),
                TotalStudents = totalStudents,
                TotalInstructors = totalInstructors,
                TotalEnrollments = enrollments.Count,
                ActiveEnrollments = enrollments.Count(e => e.Status == EnrollmentStatus.Active),
                CompletedEnrollments = enrollments.Count(e => e.Status == EnrollmentStatus.Completed),
                TotalRevenue = enrollments.Count * 0, 
                TotalReviews = reviews.Count,
                AveragePlatformRating = reviews.Any()
                    ? Math.Round(reviews.Average(r => r.Rating), 2)
                    : 0
            };

            dto.TotalRevenue = await context.Enrollments
                .Include(e => e.Course)
                .SumAsync(e => e.Course.Price);

            return Result<DashboardSummaryDto>.Success(dto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении сводки дашборда");
            return Result<DashboardSummaryDto>.Failure("Не удалось получить сводку");
        }
    }

    public async Task<Result<List<TopCourseDto>>> GetTopCoursesAsync()
    {
        try
        {
            var topCourses = await context.Courses
                .Include(c => c.Instructor)
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .Select(c => new TopCourseDto
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    InstructorName = c.Instructor.FullName,
                    EnrollmentCount = c.Enrollments.Count,
                    CompletedCount = c.Enrollments
                        .Count(e => e.Status == EnrollmentStatus.Completed),
                    CompletionRate = c.Enrollments.Any()
                        ? Math.Round((double)c.Enrollments
                                .Count(e => e.Status == EnrollmentStatus.Completed)
                            / c.Enrollments.Count * 100, 2)
                        : 0,
                    AverageRating = c.Reviews.Any()
                        ? Math.Round(c.Reviews.Average(r => r.Rating), 2)
                        : 0,
                    Revenue = c.Price * c.Enrollments.Count
                })
                .OrderByDescending(c => c.EnrollmentCount)
                .Take(10)
                .ToListAsync();

            return Result<List<TopCourseDto>>.Success(topCourses);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении топ курсов");
            return Result<List<TopCourseDto>>.Failure("Не удалось получить топ курсов");
        }
    }

    public async Task<Result<List<MonthlyEnrollmentDto>>> GetEnrollmentsByMonthAsync()
    {
        try
        {
            var monthNames = new[]
            {
                "", "Январь", "Февраль", "Март", "Апрель",
                "Май", "Июнь", "Июль", "Август",
                "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
            };

            var data = await context.Enrollments
                .Include(e => e.Course)
                .Where(e => e.EnrolledAt >= DateTime.UtcNow.AddMonths(-12))
                .GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month })
                .Select(g => new MonthlyEnrollmentDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    NewEnrollments = g.Count(),
                    Completions = g.Count(e => e.Status == EnrollmentStatus.Completed),
                    Revenue = g.Sum(e => e.Course.Price)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToListAsync();

            data.ForEach(d => d.MonthName = monthNames[d.Month]);

            return Result<List<MonthlyEnrollmentDto>>.Success(data);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении статистики по месяцам");
            return Result<List<MonthlyEnrollmentDto>>.Failure("Не удалось получить статистику");
        }
    }

    public async Task<Result<List<CategoryRevenueDto>>> GetRevenueByCategoryAsync()
    {
        try
        {
            var data = await context.Categories
                .Include(cat => cat.Courses)
                .ThenInclude(c => c.Enrollments)
                .Include(cat => cat.Courses)
                .ThenInclude(c => c.Reviews)
                .Select(cat => new CategoryRevenueDto
                {
                    CategoryId = cat.Id,
                    CategoryName = cat.Name,
                    CourseCount = cat.Courses.Count,
                    TotalStudents = cat.Courses.Sum(c => c.Enrollments.Count),
                    TotalRevenue = cat.Courses.Sum(c => c.Price * c.Enrollments.Count),
                    AverageRating = cat.Courses.Any() && cat.Courses.SelectMany(c => c.Reviews).Any()
                        ? Math.Round(cat.Courses
                            .SelectMany(c => c.Reviews)
                            .Average(r => r.Rating), 2)
                        : 0
                })
                .OrderByDescending(c => c.TotalRevenue)
                .ToListAsync();

            return Result<List<CategoryRevenueDto>>.Success(data);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении выручки по категориям");
            return Result<List<CategoryRevenueDto>>.Failure("Не удалось получить данные");
        }
    }

    public async Task<Result<List<CompletionRateDto>>> GetCompletionRateAsync()
    {
        try
        {
            var data = await context.Courses
                .Include(c => c.Enrollments)
                .Where(c => c.Enrollments.Count >= 5) // только с >= 5 студентами
                .Select(c => new CompletionRateDto
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    TotalEnrolled = c.Enrollments.Count,
                    TotalCompleted = c.Enrollments
                        .Count(e => e.Status == EnrollmentStatus.Completed),
                    CompletionRatePercent = c.Enrollments.Any()
                        ? Math.Round((double)c.Enrollments
                                .Count(e => e.Status == EnrollmentStatus.Completed)
                            / c.Enrollments.Count * 100, 2)
                        : 0,
                    AverageProgressPercent = c.Enrollments.Any()
                        ? Math.Round(c.Enrollments.Average(e => e.ProgressPercent), 2)
                        : 0
                })
                .OrderByDescending(c => c.CompletionRatePercent)
                .ToListAsync();

            return Result<List<CompletionRateDto>>.Success(data);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении завершаемости курсов");
            return Result<List<CompletionRateDto>>.Failure("Не удалось получить данные");
        }
    }

    public async Task<Result<InstructorStatsDto>> GetInstructorStatsAsync(int instructorId)
    {
        try
        {
            var instructor = await context.Users.FindAsync(instructorId);
            if (instructor == null)
                return Result<InstructorStatsDto>.NotFound("Инструктор не найден");

            var courses = await context.Courses
                .Include(c => c.Enrollments)
                .Include(c => c.Reviews)
                .Where(c => c.InstructorId == instructorId)
                .ToListAsync();

            var allEnrollments = courses.SelectMany(c => c.Enrollments).ToList();
            var allReviews = courses.SelectMany(c => c.Reviews).ToList();

            var topCourses = courses
                .OrderByDescending(c => c.Enrollments.Count)
                .Take(3)
                .Select(c => new TopCourseDto
                {
                    CourseId = c.Id,
                    Title = c.Title,
                    InstructorName = instructor.FullName,
                    EnrollmentCount = c.Enrollments.Count,
                    CompletedCount = c.Enrollments.Count(e => e.Status == EnrollmentStatus.Completed),
                    CompletionRate = c.Enrollments.Any()
                        ? Math.Round((double)c.Enrollments
                                .Count(e => e.Status == EnrollmentStatus.Completed)
                            / c.Enrollments.Count * 100, 2)
                        : 0,
                    AverageRating = c.Reviews.Any()
                        ? Math.Round(c.Reviews.Average(r => r.Rating), 2)
                        : 0,
                    Revenue = c.Price * c.Enrollments.Count
                }).ToList();

            var monthNames = new[]
            {
                "", "Январь", "Февраль", "Март", "Апрель",
                "Май", "Июнь", "Июль", "Август",
                "Сентябрь", "Октябрь", "Ноябрь", "Декабрь"
            };

            var trend = allEnrollments
                .Where(e => e.EnrolledAt >= DateTime.UtcNow.AddMonths(-6))
                .GroupBy(e => new { e.EnrolledAt.Year, e.EnrolledAt.Month })
                .Select(g => new MonthlyEnrollmentDto
                {
                    Year = g.Key.Year,
                    Month = g.Key.Month,
                    MonthName = monthNames[g.Key.Month],
                    NewEnrollments = g.Count(),
                    Completions = g.Count(e => e.Status == EnrollmentStatus.Completed),
                    Revenue = g.Sum(e => courses
                        .First(c => c.Id == e.CourseId).Price)
                })
                .OrderBy(x => x.Year)
                .ThenBy(x => x.Month)
                .ToList();

            var dto = new InstructorStatsDto
            {
                InstructorName = instructor.FullName,
                CourseCount = courses.Count,
                PublishedCourseCount = courses.Count(c => c.IsPublished),
                TotalStudents = allEnrollments.Select(e => e.StudentId).Distinct().Count(),
                TotalReviews = allReviews.Count,
                AverageRating = allReviews.Any()
                    ? Math.Round(allReviews.Average(r => r.Rating), 2)
                    : 0,
                TotalRevenue = courses.Sum(c => c.Price * c.Enrollments.Count),
                TopCourses = topCourses,
                EnrollmentTrend = trend
            };

            return Result<InstructorStatsDto>.Success(dto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении статистики инструктора Id: {Id}", instructorId);
            return Result<InstructorStatsDto>.Failure("Не удалось получить статистику инструктора");
        }
    }

    public async Task<Result<StudentsProgressSummaryDto>> GetStudentsProgressAsync()
    {
        try
        {
            var students = await context.Students
                .Include(s => s.User)
                .Include(s => s.Enrollments)
                .ToListAsync();

            var topActive = students
                .OrderByDescending(s => s.Enrollments
                    .Count(e => e.Status == EnrollmentStatus.Completed))
                .Take(5)
                .Select(s => new StudentProgressDto
                {
                    StudentId = s.Id,
                    FullName = $"{s.FirstName} {s.LastName}",
                    CompletedCourses = s.Enrollments
                        .Count(e => e.Status == EnrollmentStatus.Completed),
                    ActiveEnrollments = s.Enrollments
                        .Count(e => e.Status == EnrollmentStatus.Active),
                    AverageProgress = s.Enrollments.Any()
                        ? Math.Round(s.Enrollments.Average(e => e.ProgressPercent), 2)
                        : 0
                }).ToList();

            var dto = new StudentsProgressSummaryDto
            {
                TotalStudents = students.Count,
                StudentsWithActiveEnrollment = students
                    .Count(s => s.Enrollments.Any(e => e.Status == EnrollmentStatus.Active)),
                StudentsCompletedAtLeastOne = students
                    .Count(s => s.Enrollments.Any(e => e.Status == EnrollmentStatus.Completed)),
                StudentsNeverStarted = students
                    .Count(s => !s.Enrollments.Any()),
                AverageCoursesPerStudent = students.Any()
                    ? Math.Round((double)students.Sum(s => s.Enrollments.Count) / students.Count, 2)
                    : 0,
                TopActiveStudents = topActive
            };

            return Result<StudentsProgressSummaryDto>.Success(dto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении прогресса студентов");
            return Result<StudentsProgressSummaryDto>.Failure("Не удалось получить данные");
        }
    }

    public async Task<Result<RatingsDistributionDto>> GetRatingsDistributionAsync()
    {
        try
        {
            var reviews = await context.Reviews.ToListAsync();

            var dto = new RatingsDistributionDto
            {
                OneStar = reviews.Count(r => r.Rating == 1),
                TwoStars = reviews.Count(r => r.Rating == 2),
                ThreeStars = reviews.Count(r => r.Rating == 3),
                FourStars = reviews.Count(r => r.Rating == 4),
                FiveStars = reviews.Count(r => r.Rating == 5),
                TotalReviews = reviews.Count,
                AverageRating = reviews.Any()
                    ? Math.Round(reviews.Average(r => r.Rating), 2)
                    : 0
            };

            return Result<RatingsDistributionDto>.Success(dto);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении распределения оценок");
            return Result<RatingsDistributionDto>.Failure("Не удалось получить данные");
        }
    }
}