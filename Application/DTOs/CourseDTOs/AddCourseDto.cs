using Domain.Constants;

namespace Application.DTOs.CourseDTOs;

public class AddCourseDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public int CategoryId { get; set; }
}