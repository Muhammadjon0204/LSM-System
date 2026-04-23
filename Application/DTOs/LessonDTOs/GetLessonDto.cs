namespace Application.DTOs.LessonDTOs;

public class GetLessonDto
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public DateTime CreatedAt { get; set; }
}