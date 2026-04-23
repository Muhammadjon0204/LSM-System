namespace Application.DTOs.ReviewDTOs;

public class GetReviewDto
{
    public int Id { get; set; }
    public int Rating { get; set; }
    public string? Comment { get; set; }
    public string StudentFullName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
}