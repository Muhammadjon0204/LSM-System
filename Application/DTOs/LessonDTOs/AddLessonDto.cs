namespace Application.DTOs.LessonDTOs;

public class AddLessonDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
}