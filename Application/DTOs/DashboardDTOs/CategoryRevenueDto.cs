namespace Application.DTOs.DashboardDTOs;

public class CategoryRevenueDto
{
    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = null!;
    public int CourseCount { get; set; }
    public int TotalStudents { get; set; }
    public decimal TotalRevenue { get; set; }
    public double AverageRating { get; set; }
}