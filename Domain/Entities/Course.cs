using Domain.Constants;

namespace Domain.Entities;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Price { get; set; }
    public CourseLevel Level { get; set; }
    public bool IsPublished { get; set; }
    public DateTime CreatedAt { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }
    public int InstructorId { get; set; }
    public ApplicationUser Instructor { get; set; } = null!;

    public ICollection<Lesson> Lessons { get; set; } = new List<Lesson>();
    public ICollection<Enrollment> Enrollments { get; set; } =  new List<Enrollment>();
    public ICollection<Review> Reviews { get; set; } =  new List<Review>();
}