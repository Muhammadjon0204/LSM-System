using Domain.Constants;
namespace Application.DTOs.CourseDTOs;

public class GetCourseDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CategoryName { get; set; } = null!;
    public string InstructorName { get; set; } = null!;
}