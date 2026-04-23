using Application.Common;
using Application.DTOs.EnrollmentDTOs;

namespace Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<Result<GetEnrollmentDto>> EnrollAsync(AddEnrollmentDto dto);
    Task<Result<bool>> CancelAsync(int id);
    Task<Result<GetEnrollmentDto>> UpdateProgressAsync(int id, UpdateProgressDto dto);
    Task<Result<List<GetEnrollmentDto>>> GetByStudentIdAsync(int studentId);
}