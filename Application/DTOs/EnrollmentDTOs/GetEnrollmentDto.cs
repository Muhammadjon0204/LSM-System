using Domain.Constants;
namespace Application.DTOs.EnrollmentDTOs;

public class GetEnrollmentDto
{
    public int Id { get; set; }
    public string CourseTitle { get; set; } = null!;
    public string StudentFullName { get; set; } = null!;
    public EnrollmentStatus Status { get; set; }
    public int ProgressPercent { get; set; }
    public DateTime EnrolledAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}