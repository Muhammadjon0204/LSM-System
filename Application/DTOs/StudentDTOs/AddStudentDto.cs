namespace Application.DTOs.StudentDTOs;

public class AddStudentDto
{
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public int UserId { get; set; }
}