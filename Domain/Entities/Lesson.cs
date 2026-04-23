namespace Domain.Entities;

public class Lesson
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? VideoUrl { get; set; }
    public int Order { get; set; }
    public string? MaterialPath { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
}