namespace Application.DTOs.LessonDTOs;

public class UpdateLessonDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
}