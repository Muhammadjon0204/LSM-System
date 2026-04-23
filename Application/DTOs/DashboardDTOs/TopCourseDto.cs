namespace Application.DTOs.DashboardDTOs;

public class TopCourseDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = null!;
    public string InstructorName { get; set; } = null!;
    public int EnrollmentCount { get; set; }
    public int CompletedCount { get; set; }
    public double CompletionRate { get; set; }
    public double AverageRating { get; set; }
    public decimal Revenue { get; set; }
}