using Application.Common;
using Application.DTOs.StudentDTOs;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Domain.Entities;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class StudentService(IStudentRepository repository, ILogger<StudentService> logger) : IStudentService
{
    public async Task<Result<List<GetStudentDto>>> GetAllAsync()
    {
        try
        {
            logger.LogInformation("Получение списка студентов");
            var list = await repository.GetAllAsync();
            var result = list.Select(s => new GetStudentDto
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                Address = s.Address,
                AvatarUrl = s.AvatarUrl,
                Email = s.User.Email ?? ""
            }).ToList();

            return Result<List<GetStudentDto>>.Success(result);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении списка студентов");
            return Result<List<GetStudentDto>>.Failure("Не удалось получить список студентов");
        }
    }

    public async Task<Result<GetStudentDto>> GetByIdAsync(int id)
    {
        try
        {
            var student = await repository.GetByIdAsync(id);
            if (student == null)
            {
                logger.LogWarning("Студент с Id {Id} не найден", id);
                return Result<GetStudentDto>.NotFound($"Студент с Id {id} не найден");
            }

            return Result<GetStudentDto>.Success(new GetStudentDto
            {
                Id = student.Id,
                FirstName = student.FirstName,
                LastName = student.LastName,
                Address = student.Address,
                AvatarUrl = student.AvatarUrl,
                Email = student.User.Email ?? ""
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при получении студента по Id: {Id}", id);
            return Result<GetStudentDto>.Failure("Не удалось получить студента");
        }
    }

    public async Task<Result<GetStudentDto>> AddAsync(AddStudentDto dto)
    {
        try
        {
            var entity = new Student
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Address = dto.Address,
                UserId = dto.UserId
            };

            var created = await repository.AddAsync(entity);
            logger.LogInformation("Студент успешно добавлен: {FirstName} {LastName}", created.FirstName, created.LastName);

            return Result<GetStudentDto>.Success(new GetStudentDto
            {
                Id = created.Id,
                FirstName = created.FirstName,
                LastName = created.LastName,
                Address = created.Address,
                AvatarUrl = created.AvatarUrl
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при добавлении студента");
            return Result<GetStudentDto>.Failure("Не удалось добавить студента");
        }
    }

    public async Task<Result<GetStudentDto>> UpdateAsync(int id, UpdateStudentDto dto)
    {
        try
        {
            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
            {
                logger.LogWarning("Студент с Id {Id} не найден при обновлении", id);
                return Result<GetStudentDto>.NotFound($"Студент с Id {id} не найден");
            }

            exist.FirstName = dto.FirstName;
            exist.LastName = dto.LastName;
            exist.Address = dto.Address;

            var updated = await repository.UpdateAsync(exist);
            return Result<GetStudentDto>.Success(new GetStudentDto
            {
                Id = updated.Id,
                FirstName = updated.FirstName,
                LastName = updated.LastName,
                Address = updated.Address,
                AvatarUrl = updated.AvatarUrl
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при обновлении студента Id: {Id}", id);
            return Result<GetStudentDto>.Failure("Не удалось обновить студента");
        }
    }

    public async Task<Result<bool>> DeleteAsync(int id)
    {
        try
        {
            var exist = await repository.GetByIdAsync(id);
            if (exist == null)
            {
                logger.LogWarning("Студент с Id {Id} не найден при удалении", id);
                return Result<bool>.NotFound($"Студент с Id {id} не найден");
            }

            await repository.DeleteAsync(exist);
            return Result<bool>.Success(true);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при удалении студента Id: {Id}", id);
            return Result<bool>.Failure("Не удалось удалить студента");
        }
    }
}