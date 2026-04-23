using Application.Common;
using Application.DTOs.EnrollmentDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Constants;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class EnrollmentService(
    IEnrollmentRepository repository,
    ICourseRepository courseRepository,
    IStudentRepository studentRepository,
    ILogger<EnrollmentService> logger) : IEnrollmentService
{
    public async Task<Result<GetEnrollmentDto>> EnrollAsync(AddEnrollmentDto dto)
    {
        try
        {
            var course = await courseRepository.GetByIdAsync(dto.CourseId);
            if (course == null)
                return Result<GetEnrollmentDto>.NotFound("Курс не найден");

            var student = await studentRepository.GetByIdAsync(dto.StudentId);
            if (student == null)
                return Result<GetEnrollmentDto>.NotFound("Студент не найден");

            var existing = await repository.GetByStudentAndCourseAsync(dto.StudentId, dto.CourseId);
            if (existing != null)
                return Result<GetEnrollmentDto>.Failure("Студент уже записан на этот курс");

            var entity = new Enrollment
            {
                CourseId = dto.CourseId,
                StudentId = dto.StudentId,
                Status = EnrollmentStatus.Active,
                ProgressPercent = 0,
                EnrolledAt = DateTime.UtcNow
            };

            var created = await repository.AddAsync(entity);
            logger.LogInformation("Студент {StudentId} записался на курс {CourseId}", dto.StudentId, dto.CourseId);

            return Result<GetEnrollmentDto>.Success(new GetEnrollmentDto
            {
                Id = created.Id,
                CourseTitle = course.Title,
                StudentFullName = $"{student.FirstName} {student.LastName}",
                Status = created.Status,
                ProgressPercent = created.ProgressPercent,
                EnrolledAt = created.EnrolledAt
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при записи на курс");
            return Result<GetEnrollmentDto>.Failure("Не удалось записаться на курс");
        }
    }

    public async Task<Result<bool>> CancelAsync(int id)
    {
        try
        {
            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
                return Result<bool>.NotFound($"Запись с Id {id} не найдена");

            exist.Status = EnrollmentStatus.Cancelled;
            await repository.UpdateAsync(exist);
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при отмене записи Id: {Id}", id);
            return Result<bool>.Failure("Не удалось отменить запись");
        }
    }

    public async Task<Result<GetEnrollmentDto>> UpdateProgressAsync(int id, UpdateProgressDto dto)
    {
        try
        {
            if (dto.ProgressPercent < 0 || dto.ProgressPercent > 100)
                return Result<GetEnrollmentDto>.Failure("Прогресс должен быть от 0 до 100");

            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
                return Result<GetEnrollmentDto>.NotFound($"Запись с Id {id} не найдена");

            exist.ProgressPercent = dto.ProgressPercent;

            // Если прогресс 100% — автоматически завершить
            if (dto.ProgressPercent == 100)
            {
                exist.Status = EnrollmentStatus.Completed;
                exist.CompletedAt = DateTime.UtcNow;
            }

            var updated = await repository.UpdateAsync(exist);
            return Result<GetEnrollmentDto>.Success(new GetEnrollmentDto
            {
                Id = updated.Id,
                CourseTitle = updated.Course.Title,
                StudentFullName = $"{updated.Student.FirstName} {updated.Student.LastName}",
                Status = updated.Status,
                ProgressPercent = updated.ProgressPercent,
                EnrolledAt = updated.EnrolledAt,
                CompletedAt = updated.CompletedAt
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при обновлении прогресса Id: {Id}", id);
            return Result<GetEnrollmentDto>.Failure("Не удалось обновить прогресс");
        }
    }

    public async Task<Result<List<GetEnrollmentDto>>> GetByStudentIdAsync(int studentId)
    {
        try
        {
            var list = await repository.GetByStudentIdAsync(studentId);
            var result = list.Select(e => new GetEnrollmentDto
            {
                Id = e.Id,
                CourseTitle = e.Course.Title,
                StudentFullName = $"{e.Student.FirstName} {e.Student.LastName}",
                Status = e.Status,
                ProgressPercent = e.ProgressPercent,
                EnrolledAt = e.EnrolledAt,
                CompletedAt = e.CompletedAt
            }).ToList();

            return Result<List<GetEnrollmentDto>>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении записей студента Id: {Id}", studentId);
            return Result<List<GetEnrollmentDto>>.Failure("Не удалось получить записи студента");
        }
    }
}