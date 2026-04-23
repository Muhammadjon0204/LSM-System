namespace Application.DTOs.DashboardDTOs;

public class InstructorStatsDto
{
    public string InstructorName { get; set; } = null!;
    public int CourseCount { get; set; }
    public int PublishedCourseCount { get; set; }
    public int TotalStudents { get; set; }
    public int TotalReviews { get; set; }
    public double AverageRating { get; set; }
    public decimal TotalRevenue { get; set; }
    public List<TopCourseDto> TopCourses { get; set; } = new();
    public List<MonthlyEnrollmentDto> EnrollmentTrend { get; set; } = new();
}