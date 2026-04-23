namespace Application.DTOs.StudentDTOs;

public class GetStudentDto
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    public string Email { get; set; } = null!;      // из ApplicationUser
}