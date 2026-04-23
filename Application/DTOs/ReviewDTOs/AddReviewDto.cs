namespace Application.DTOs.ReviewDTOs;

public class AddReviewDto
{
    public int Rating { get; set; }       // 1–5
    public string? Comment { get; set; }
    public int CourseId { get; set; }
    public int StudentId { get; set; }
}