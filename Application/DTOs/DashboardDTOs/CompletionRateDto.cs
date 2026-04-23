namespace Application.DTOs.DashboardDTOs;

public class CompletionRateDto
{
    public int CourseId { get; set; }
    public string Title { get; set; } = null!;
    public int TotalEnrolled { get; set; }
    public int TotalCompleted { get; set; }
    public double CompletionRatePercent { get; set; }
    public double AverageProgressPercent { get; set; }
}