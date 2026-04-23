using Application.Common;
using Application.DTOs.StudentDTOs;

namespace Application.Interfaces.Services;

public interface IStudentService
{
    Task<Result<List<GetStudentDto>>> GetAllAsync();
    Task<Result<GetStudentDto>> GetByIdAsync(int id);
    Task<Result<GetStudentDto>> AddAsync(AddStudentDto dto);
    Task<Result<GetStudentDto>> UpdateAsync(int id, UpdateStudentDto dto);
    Task<Result<bool>> DeleteAsync(int id);
}