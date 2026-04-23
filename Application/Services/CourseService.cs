// Application/Services/CourseService.cs
using Application.Common;
using Application.DTOs.CourseDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CourseService(ICourseRepository repository, ILogger<CourseService> logger) : ICourseService
{
    public async Task<Result<PagedResult<GetCourseDto>>> GetAllAsync(CourseFilterDto filter)
    {
        try
        {
            var paged = await repository.GetFilteredAsync(filter);
            var dtos = paged.Items.Select(c => new GetCourseDto
            {
                Id = c.Id,
                Title = c.Title,
                Description = c.Description,
                ImageUrl = c.ImageUrl,
                Price = c.Price,
                Level = c.Level,
                IsPublished = c.IsPublished,
                CreatedAt = c.CreatedAt,
                CategoryName = c.Category?.Name ?? "",
                InstructorName = c.Instructor.FullName
            }).ToList();

            var result = PagedResult<GetCourseDto>.Create(dtos, paged.TotalCount, paged.Page, paged.PageSize);
            return Result<PagedResult<GetCourseDto>>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении списка курсов");
            return Result<PagedResult<GetCourseDto>>.Failure("Произошла ошибка при получении курсов");
        }
    }

    public async Task<Result<GetCourseDto>> GetByIdAsync(int id)
    {
        try
        {
            var course = await repository.GetWithDetailsAsync(id);
            if (course == null)
                return Result<GetCourseDto>.NotFound($"Курс с Id {id} не найден");

            return Result<GetCourseDto>.Success(new GetCourseDto
            {
                Id = course.Id,
                Title = course.Title,
                Description = course.Description,
                ImageUrl = course.ImageUrl,
                Price = course.Price,
                Level = course.Level,
                IsPublished = course.IsPublished,
                CreatedAt = course.CreatedAt,
                CategoryName = course.Category?.Name ?? "",
                InstructorName = course.Instructor.FullName
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении курса по Id: {Id}", id);
            return Result<GetCourseDto>.Failure("Произошла ошибка при получении курса");
        }
    }

    public async Task<Result<GetCourseDto>> AddAsync(AddCourseDto dto)
    {
        try
        {
            var entity = new Course
            {
                Title = dto.Title,
                Description = dto.Description,
                Price = dto.Price,
                Level = dto.Level,
                CategoryId = dto.CategoryId,
                IsPublished = false,
                CreatedAt = DateTime.UtcNow
            };

            var created = await repository.AddAsync(entity);
            logger.LogInformation("Курс успешно добавлен: {Title}", created.Title);

            return Result<GetCourseDto>.Success(new GetCourseDto
            {
                Id = created.Id,
                Title = created.Title,
                Description = created.Description,
                Price = created.Price,
                Level = created.Level,
                IsPublished = created.IsPublished,
                CreatedAt = created.CreatedAt
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при добавлении курса");
            return Result<GetCourseDto>.Failure("Не удалось добавить курс");
        }
    }

    public async Task<Result<GetCourseDto>> UpdateAsync(int id, UpdateCourseDto dto)
    {
        try
        {
            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
                return Result<GetCourseDto>.NotFound($"Курс с Id {id} не найден");

            exist.Title = dto.Title;
            exist.Description = dto.Description;
            exist.Price = dto.Price;
            exist.Level = dto.Level;
            exist.CategoryId = dto.CategoryId;

            var updated = await repository.UpdateAsync(exist);
            return Result<GetCourseDto>.Success(new GetCourseDto
            {
                Id = updated.Id,
                Title = updated.Title,
                Description = updated.Description,
                Price = updated.Price,
                Level = updated.Level,
                IsPublished = updated.IsPublished,
                CreatedAt = updated.CreatedAt
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при обновлении курса Id: {Id}", id);
            return Result<GetCourseDto>.Failure("Не удалось обновить курс");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        try
        {
            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
                return Result<bool>.NotFound($"Курс с Id {id} не найден");

            await repository.DeleteAsync(exist);
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при удалении курса Id: {Id}", id);
            return Result<bool>.Failure("Не удалось удалить курс");
        }
    }

    public async Task<Result<bool>> PublishAsync(int id)
    {
        try
        {
            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
                return Result<bool>.NotFound($"Курс с Id {id} не найден");

            exist.IsPublished = !exist.IsPublished;   // toggle
            await repository.UpdateAsync(exist);

            logger.LogInformation("Курс Id {Id} — IsPublished = {Status}", id, exist.IsPublished);
            return Result<bool>.Success(exist.IsPublished);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при публикации курса Id: {Id}", id);
            return Result<bool>.Failure("Не удалось изменить статус публикации");
        }
    }
}