namespace Application.DTOs.DashboardDTOs;

public class MonthlyEnrollmentDto
{
    public int Year { get; set; }
    public int Month { get; set; }
    public string MonthName { get; set; } = null!;
    public int NewEnrollments { get; set; }
    public int Completions { get; set; }
    public decimal Revenue { get; set; }
}