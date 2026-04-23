using Application.Common;
using Application.DTOs.CourseDTOs;

namespace Application.Interfaces.Services;

public interface ICourseService
{
    Task<Result<PagedResult<GetCourseDto>>> GetAllAsync(CourseFilterDto filter);
    Task<Result<GetCourseDto>> GetByIdAsync(int id);
    Task<Result<GetCourseDto>> AddAsync(AddCourseDto dto);
    Task<Result<GetCourseDto>> UpdateAsync(int id, UpdateCourseDto dto);
    Task<Result<bool>> DeleteAsync(int id);
    Task<Result<bool>> PublishAsync(int id);
}