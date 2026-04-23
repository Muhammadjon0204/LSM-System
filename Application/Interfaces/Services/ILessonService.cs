using Application.Common;
using Application.DTOs.LessonDTOs;

namespace Application.Interfaces.Services;

public interface ILessonService
{
    Task<Result<List<GetLessonDto>>> GetByCourseIdAsync(int courseId);
    Task<Result<GetLessonDto>> GetByIdAsync(int id);
    Task<Result<GetLessonDto>> AddAsync(int courseId, AddLessonDto dto);
    Task<Result<GetLessonDto>> UpdateAsync(int id, UpdateLessonDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}