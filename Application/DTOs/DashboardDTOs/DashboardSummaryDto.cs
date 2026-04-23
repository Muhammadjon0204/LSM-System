namespace Application.DTOs.DashboardDTOs;

public class DashboardSummaryDto
{
    public int TotalCourses { get; set; }
    public int PublishedCourses { get; set; }
    public int TotalStudents { get; set; }
    public int TotalInstructors { get; set; }
    public int TotalEnrollments { get; set; }
    public int ActiveEnrollments { get; set; }
    public int CompletedEnrollments { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AveragePlatformRating { get; set; }
    public int TotalReviews { get; set; }
}