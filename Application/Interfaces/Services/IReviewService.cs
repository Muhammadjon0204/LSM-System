using Application.Common;
using Application.DTOs.ReviewDTOs;

namespace Application.Interfaces.Services;

public interface IReviewService
{
    Task<Result<List<GetReviewDto>>> GetByCourseIdAsync(int courseId);
    Task<Result<GetReviewDto>> AddAsync(AddReviewDto dto);
    Task<Result<GetReviewDto>> UpdateAsync(int id, UpdateReviewDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}