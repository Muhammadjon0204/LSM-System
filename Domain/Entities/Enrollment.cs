using Domain.Constants;

namespace Domain.Entities;

public class Enrollment
{
    public int Id { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public EnrollmentStatus Status { get; set; }
    public int ProgressPercent { get; set; }      // 0–100

    public int CourseId { get; set; }
    public Course Course { get; set; } = null!;
    
    public int StudentId { get; set; }
    public Student Student { get; set; } = null!;
}