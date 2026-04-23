using Application.Common;
using Application.DTOs.ReviewDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class ReviewService(
    IReviewRepository repository,
    IEnrollmentRepository enrollmentRepository,
    ILogger<ReviewService> logger) : IReviewService
{
    public async Task<Result<List<GetReviewDto>>> GetByCourseIdAsync(int courseId)
    {
        try
        {
            var list = await repository.GetByCourseIdAsync(courseId);
            var result = list.Select(r => new GetReviewDto
            {
                Id = r.Id,
                Rating = r.Rating,
                Comment = r.Comment,
                StudentFullName = $"{r.Student.FirstName} {r.Student.LastName}",
                CreatedAt = r.CreatedAt
            }).ToList();

            return Result<List<GetReviewDto>>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении отзывов курса Id: {Id}", courseId);
            return Result<List<GetReviewDto>>.Failure("Не удалось получить отзывы");
        }
    }

    public async Task<Result<GetReviewDto>> AddAsync(AddReviewDto dto)
    {
        try
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return Result<GetReviewDto>.Failure("Оценка должна быть от 1 до 5");

            var enrollment = await enrollmentRepository.GetByStudentAndCourseAsync(dto.StudentId, dto.CourseId);
            if (enrollment == null)
                return Result<GetReviewDto>.Failure("Студент не записан на этот курс");

            var existing = await repository.GetByStudentAndCourseAsync(dto.StudentId, dto.CourseId);
            if (existing != null)
                return Result<GetReviewDto>.Failure("Студент уже оставил отзыв на этот курс");

            var entity = new Review
            {
                Rating = dto.Rating,
                Comment = dto.Comment,
                CourseId = dto.CourseId,
                StudentId = dto.StudentId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await repository.AddAsync(entity);
            logger.LogInformation("Добавлен отзыв от студента {StudentId} на курс {CourseId}", dto.StudentId, dto.CourseId);

            return Result<GetReviewDto>.Success(new GetReviewDto
            {
                Id = created.Id,
                Rating = created.Rating,
                Comment = created.Comment,
                CreatedAt = created.CreatedAt
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при добавлении отзыва");
            return Result<GetReviewDto>.Failure("Не удалось добавить отзыв");
        }
    }

    public async Task<Result<GetReviewDto>> UpdateAsync(int id, UpdateReviewDto dto)
    {
        try
        {
            if (dto.Rating < 1 || dto.Rating > 5)
                return Result<GetReviewDto>.Failure("Оценка должна быть от 1 до 5");

            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
                return Result<GetReviewDto>.NotFound($"Отзыв с Id {id} не найден");

            exist.Rating = dto.Rating;
            exist.Comment = dto.Comment;

            var updated = await repository.UpdateAsync(exist);
            return Result<GetReviewDto>.Success(new GetReviewDto
            {
                Id = updated.Id,
                Rating = updated.Rating,
                Comment = updated.Comment,
                CreatedAt = updated.CreatedAt
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при обновлении отзыва Id: {Id}", id);
            return Result<GetReviewDto>.Failure("Не удалось обновить отзыв");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        try
        {
            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
                return Result<bool>.NotFound($"Отзыв с Id {id} не найден");

            await repository.DeleteAsync(exist);
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при удалении отзыва Id: {Id}", id);
            return Result<bool>.Failure("Не удалось удалить отзыв");
        }
    }
}