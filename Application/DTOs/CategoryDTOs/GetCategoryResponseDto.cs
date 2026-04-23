namespace Application.DTOs.CategoryDTOs;

public class GetCategoryResponseDto
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; } = null!;
    public bool IsActive { get; set; }
}