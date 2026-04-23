namespace Domain.Entities;

public class Review
{
    public int Id { get; set; }
    public int Rating { get; set; } // 1–5
    public string? Comment { get; set; }
    public DateTime CreatedAt { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
}