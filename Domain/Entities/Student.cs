using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class Student 
{
    public int Id { get; set; }
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Address { get; set; } = null!;
    public string? AvatarUrl { get; set; }
    
    public int UserId { get; set; }                    
    public ApplicationUser User { get; set; } = null!;

    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    public ICollection<Review> Reviews { get; set; } = new List<Review>();
}