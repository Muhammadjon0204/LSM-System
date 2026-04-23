using Application.Common;
using Application.DTOs.LessonDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class LessonService(
    ILessonRepository repository,
    ICourseRepository courseRepository,
    ILogger<LessonService> logger) : ILessonService
{
    public async Task<Result<List<GetLessonDto>>> GetByCourseIdAsync(int courseId)
    {
        try
        {
            var course = await courseRepository.GetByIdAsync(courseId);
            if (course == null)
                return Result<List<GetLessonDto>>.NotFound($"Курс с Id {courseId} не найден");

            var list = await repository.GetByCourseIdAsync(courseId);
            var result = list.Select(l => new GetLessonDto
            {
                Id = l.Id,
                Title = l.Title,
                Description = l.Description,
                VideoUrl = l.VideoUrl,
                Order = l.Order,
                CreatedAt = l.CreatedAt
            }).ToList();

            return Result<List<GetLessonDto>>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении уроков курса Id: {Id}", courseId);
            return Result<List<GetLessonDto>>.Failure("Не удалось получить уроки");
        }
    }

    public async Task<Result<GetLessonDto>> GetByIdAsync(int id)
    {
        try
        {
            var lesson = await repository.GetByIdAsync(id);
            if (lesson == null)
            {
                logger.LogWarning("Урок с Id {Id} не найден", id);
                return Result<GetLessonDto>.NotFound($"Урок с Id {id} не найден");
            }

            return Result<GetLessonDto>.Success(new GetLessonDto
            {
                Id = lesson.Id,
                Title = lesson.Title,
                Description = lesson.Description,
                VideoUrl = lesson.VideoUrl,
                Order = lesson.Order,
                CreatedAt = lesson.CreatedAt
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении урока Id: {Id}", id);
            return Result<GetLessonDto>.Failure("Не удалось получить урок");
        }
    }

    public async Task<Result<GetLessonDto>> AddAsync(int courseId, AddLessonDto dto)
    {
        try
        {
            var course = await courseRepository.GetByIdAsync(courseId);
            if (course == null)
                return Result<GetLessonDto>.NotFound($"Курс с Id {courseId} не найден");

            var entity = new Lesson
            {
                Title = dto.Title,
                Description = dto.Description,
                VideoUrl = dto.VideoUrl,
                Order = dto.Order,
                CourseId = courseId,
                CreatedAt = DateTime.UtcNow
            };

            var created = await repository.AddAsync(entity);
            logger.LogInformation("Урок '{Title}' добавлен в курс Id: {CourseId}", created.Title, courseId);

            return Result<GetLessonDto>.Success(new GetLessonDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                VideoUrl = created.VideoUrl,
                Order = created.Order,
                CreatedAt = created.CreatedAt
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при добавлении урока в курс Id: {Id}", courseId);
            return Result<GetLessonDto>.Failure("Не удалось добавить урок");
        }
    }

    public async Task<Result<GetLessonDto>> UpdateAsync(int id, UpdateLessonDto dto)
    {
        try
        {
            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
            {
                logger.LogWarning("Урок с Id {Id} не найден при обновлении", id);
                return Result<GetLessonDto>.NotFound($"Урок с Id {id} не найден");
            }

            exist.Title = dto.Title;
            exist.Description = dto.Description;
            exist.VideoUrl = dto.VideoUrl;
            exist.Order = dto.Order;

            var updated = await repository.UpdateAsync(exist);
            return Result<GetLessonDto>.Success(new GetLessonDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Description = updated.Description,
                VideoUrl = updated.VideoUrl,
                Order = updated.Order,
                CreatedAt = updated.CreatedAt
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при обновлении урока Id: {Id}", id);
            return Result<GetLessonDto>.Failure("Не удалось обновить урок");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        try
        {
            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
            {
                logger.LogWarning("Урок с Id {Id} не найден при удалении", id);
                return Result<bool>.NotFound($"Урок с Id {id} не найден");
            }

            await repository.DeleteAsync(exist);
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при удалении урока Id: {Id}", id);
            return Result<bool>.Failure("Не удалось удалить урок");
        }
    }
}