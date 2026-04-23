using Domain.Constants;
namespace Application.DTOs.CourseDTOs;

public class CourseFilterDto
{
    public string? Search { get; set; }
    public int? CategoryId { get; set; }
    public CourseLevel? Level { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public bool? IsPublished { get; set; }
    public string? SortBy { get; set; }       // "price", "createdAt"
    public bool SortDescending { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}