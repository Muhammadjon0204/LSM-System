namespace Application.DTOs.CategoryDTOs;

public class UpdateCategoryDto
{
    public string Name { get; set; } = null!;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}