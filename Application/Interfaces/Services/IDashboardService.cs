// Application/Interfaces/Services/IDashboardService.cs
using Application.Common;
using Application.DTOs.DashboardDTOs;

namespace Application.Interfaces.Services;

public interface IDashboardService
{
    Task<Result<DashboardSummaryDto>> GetSummaryAsync();
    Task<Result<List<TopCourseDto>>> GetTopCoursesAsync();
    Task<Result<List<MonthlyEnrollmentDto>>> GetEnrollmentsByMonthAsync();
    Task<Result<List<CategoryRevenueDto>>> GetRevenueByCategoryAsync();
    Task<Result<List<CompletionRateDto>>> GetCompletionRateAsync();
    Task<Result<InstructorStatsDto>> GetInstructorStatsAsync(int instructorId);
    Task<Result<StudentsProgressSummaryDto>> GetStudentsProgressAsync();
    Task<Result<RatingsDistributionDto>> GetRatingsDistributionAsync();
}