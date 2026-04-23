using Microsoft.AspNetCore.Identity;

namespace Domain.Entities;

public class ApplicationUser : IdentityUser<int>
{
    public string FullName { get; set; } = null!;
    public DateTime CreatedAt { get; set; }

    public ICollection<Course> Courses { get; set; } = new List<Course>();
    public Student? Student { get; set; }
}