using Application.Common;
using Application.DTOs.CategoryDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class CategoryService(ICategoryRepository repository, ILogger<CategoryService> logger) : ICategoryService
{
    public async Task<Result<List<GetCategoryDto>>> GetAllAsync()
    {
        try
        {
            logger.LogInformation("Получение данных из Category");
            var list = await repository.GetAllAsync();
            logger.LogInformation($"Получено {list.Count} данных");

            var result = list.Select(c => new GetCategoryDto()
            {
                Name = c.Name,
                Description = c.Description,
            }).ToList();

            return Result<List<GetCategoryDto>>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении списка категорий");
            return Result<List<GetCategoryDto>>.Failure("Произошла ошибка во время получении даных");
        }
    }

    public async Task<Result<GetCategoryDto>> GetByIdAsync(int id)
    {
        try
        {
            var category = await repository.GetByIdAsync(id);
            if (category == null)
            {
                logger.LogError($"Ошибка при получении категории с Id: {id}");
                return Result<GetCategoryDto>.NotFound($"Категории с таким {id} не нашлось");
            }

            return Result<GetCategoryDto>.Success(new GetCategoryDto()
            {
                Name = category.Name,
                Description = category.Description,
            });
        }
        catch (Exception e)
        {
            logger.LogError("ошибка при получении данных категории по его Id");
            return Result<GetCategoryDto>.Unknown(e.Message);
        }
    }

    public async Task<Result<GetCategoryResponseDto>> AddAsync(AddCategoryDto dto)
    {
        try
        {
            var entity = new Category()
            {
                Name = dto.Name,
                Description = dto.Description
            };
            var created = await repository.AddAsync(entity);
            logger.LogInformation("Добавление категории прошла успешно");
            return Result<GetCategoryResponseDto>.Success(new GetCategoryResponseDto()
            {
                Id = created.Id,
                Name = created.Name,
                Description = created.Description
            });
        }
        catch (Exception e)
        {
            logger.LogError("Не получилось добавить категорию");
            return Result<GetCategoryResponseDto>.Unknown(e.Message);
        }
    }

    public async Task<Result<GetCategoryResponseDto>> UpdateAsync(int id, UpdateCategoryDto dto)
    {
        try
        {
            var existCategory = await repository.GetByIdAsync(id);
            if (existCategory == null)
            {
                logger.LogError("Не получилось найти такую категорию");
                return Result<GetCategoryResponseDto>.Failure($"Не удалось найти категорию с Id: {id}");
            }

            existCategory.Name = dto.Name;
            existCategory.Description = dto.Description;

            var updated = await repository.UpdateAsync(existCategory);
            return Result<GetCategoryResponseDto>.Success(new GetCategoryResponseDto()
            {
                Id = updated.Id,
                Name = updated.Name,
                Description = updated.Description
            });
        }
        catch (Exception e)
        {
            logger.LogError("Ошибка во время обновления данных категории");
            return Result<GetCategoryResponseDto>.Unknown(e.Message);
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        try
        {
            var existCategory = await repository.GetByIdAsync(id);
            if (existCategory == null)
            {
                logger.LogError($"Категория с таким Id: {id} не нашлось");
                return Result<bool>.NotFound("Не нашли такую категорию");
            }

            await repository.DeleteAsync(existCategory);
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            logger.LogError("Произошла ошибка во время удаления категории");
            return Result<bool>.Unknown(e.Message);
        }
    }
}