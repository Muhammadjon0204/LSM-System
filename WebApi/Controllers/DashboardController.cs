using Application.Interfaces.Services;
using Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebApi.Controllers;

[ApiController]
[Route("api/dashboard")]
[Authorize(Roles = UserRole.Admin)]
public class DashboardController(IDashboardService dashboardService) : ControllerBase
{
    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        var result = await dashboardService.GetSummaryAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("top-courses")]
    public async Task<IActionResult> GetTopCourses()
    {
        var result = await dashboardService.GetTopCoursesAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("enrollments-by-month")]
    public async Task<IActionResult> GetEnrollmentsByMonth()
    {
        var result = await dashboardService.GetEnrollmentsByMonthAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("revenue-by-category")]
    public async Task<IActionResult> GetRevenueByCategory()
    {
        var result = await dashboardService.GetRevenueByCategoryAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("completion-rate")]
    public async Task<IActionResult> GetCompletionRate()
    {
        var result = await dashboardService.GetCompletionRateAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("ratings-distribution")]
    public async Task<IActionResult> GetRatingsDistribution()
    {
        var result = await dashboardService.GetRatingsDistributionAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [HttpGet("students-progress")]
    public async Task<IActionResult> GetStudentsProgress()
    {
        var result = await dashboardService.GetStudentsProgressAsync();
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }

    [Authorize(Roles = $"{UserRole.Admin},{UserRole.Instructor}")]
    [HttpGet("instructor/{instructorId}")]
    public async Task<IActionResult> GetInstructorStats(int instructorId)
    {
        if (User.IsInRole(UserRole.Instructor) && !User.IsInRole(UserRole.Admin))
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            if (currentUserId != instructorId)
                return StatusCode(403, new
                {
                    isSuccess = false,
                    message = "Нет доступа к чужой статистике"
                });
        }

        var result = await dashboardService.GetInstructorStatsAsync(instructorId);
        return result.IsSuccess ? Ok(result) : BadRequest(result);
    }
}