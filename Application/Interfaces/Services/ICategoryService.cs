using Application.Common;
using Application.DTOs.CategoryDTOs;

namespace Application.Interfaces.Services;

public interface ICategoryService
{
    Task<Result<List<GetCategoryDto>>> GetAllAsync();
    Task<Result<GetCategoryDto>> GetByIdAsync(int id);
    Task<Result<GetCategoryResponseDto>> AddAsync(AddCategoryDto dto);
    Task<Result<GetCategoryResponseDto>> UpdateAsync(int id , UpdateCategoryDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}