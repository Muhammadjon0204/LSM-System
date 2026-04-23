using Application.Common;
using Application.DTOs.AuthDTOs;
using Application.Interfaces.Services.AuthService;
using Application.Interfaces.Services.TokenService;
using Domain.Constants;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Services.AuthService;

public class AuthService(
    UserManager<ApplicationUser> userManager,
    IJwtService jwtService,
    IEmailService emailService,
    IMemoryCache cache,
    ILogger<AuthService> logger) : IAuthService

{
    public async Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto)
    {
        try
        {
            var existing = await userManager.FindByEmailAsync(dto.Email);
            if (existing != null)
                return Result<AuthResponseDto>.Failure("Пользователь с таким email уже существует");

            var user = new ApplicationUser
            {
                FullName = dto.FullName,
                Email = dto.Email,
                UserName = dto.Email,
                CreatedAt = DateTime.UtcNow,

                Student = new Student
                {
                    FirstName = dto.FullName.Split(' ').First(),
                    LastName = dto.FullName.Split(' ').Last(),
                    Address = ""
                }
            };

            var createResult = await userManager.CreateAsync(user, dto.Password);
            if (!createResult.Succeeded)
            {
                var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                return Result<AuthResponseDto>.Failure(errors);
            }

            await userManager.AddToRoleAsync(user, UserRole.Student);
            logger.LogInformation("Зарегистрирован новый пользователь: {Email}", dto.Email);

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtService.GenerateToken(user, roles);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email,
                Role = roles.First()
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при регистрации пользователя");
            return Result<AuthResponseDto>.Failure("Не удалось зарегистрироваться");
        }
    }

    public async Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Result<AuthResponseDto>.NotFound("Пользователь не найден");

            var isPasswordValid = await userManager.CheckPasswordAsync(user, dto.Password);
            if (!isPasswordValid)
                return Result<AuthResponseDto>.Failure("Неверный пароль");

            var roles = await userManager.GetRolesAsync(user);
            var token = jwtService.GenerateToken(user, roles);

            logger.LogInformation("Пользователь {Email} вошёл в систему", dto.Email);

            return Result<AuthResponseDto>.Success(new AuthResponseDto
            {
                Token = token,
                FullName = user.FullName,
                Email = user.Email!,
                Role = roles.FirstOrDefault() ?? ""
            });
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при входе пользователя");
            return Result<AuthResponseDto>.Failure("Не удалось войти в систему");
        }
    }

    public async Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto dto)
    {
        try
        {
            var user = await userManager.FindByEmailAsync(dto.Email);

            if (user == null)
                return Result<bool>.Success(true, "Если email существует — код отправлен");

            var code = Random.Shared.Next(100000, 999999).ToString();
            var cacheKey = $"reset_code:{dto.Email}";

            cache.Set(cacheKey, code, TimeSpan.FromMinutes(2));

            await emailService.SendAsync(
                to: dto.Email,
                subject: "Сброс пароля — код подтверждения",
                body: $"""
                           <h2>Сброс пароля</h2>
                           <p>Ваш код для сброса пароля:</p>
                           <h1 style="letter-spacing:8px; color:#4F46E5;">{code}</h1>
                           <p>Код действителен <strong>2 минуты</strong>.</p>
                           <p>Если вы не запрашивали сброс — проигнорируйте это письмо.</p>
                       """
            );

            logger.LogInformation("Код сброса отправлен на {Email}", dto.Email);
            return Result<bool>.Success(true, "Код отправлен на email");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при отправке кода сброса пароля");
            return Result<bool>.Failure("Не удалось отправить код");
        }
    }

    public async Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto dto)
    {
        try
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return Result<bool>.Failure("Пароли не совпадают");

            var cacheKey = $"reset_code:{dto.Email}";
            if (!cache.TryGetValue(cacheKey, out string? savedCode))
                return Result<bool>.Failure("Код истёк или не существует. Запросите новый");

            if (savedCode != dto.ResetCode)
                return Result<bool>.Failure("Неверный код подтверждения");

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Result<bool>.NotFound("Пользователь не найден");

            var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);
            var result = await userManager.ResetPasswordAsync(user, resetToken, dto.NewPassword);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<bool>.Failure(errors);
            }

            cache.Remove(cacheKey);

            logger.LogInformation("Пароль успешно сброшен для {Email}", dto.Email);
            return Result<bool>.Success(true, "Пароль успешно изменён");
        }
        catch (Exception e)
        {
            logger.LogError(e, "Ошибка при сбросе пароля");
            return Result<bool>.Failure("Не удалось сбросить пароль");
        }
    }

    public async Task<Result<bool>> ChangePasswordAsync(ChangePasswordDto dto)
    {
        try
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return Result<bool>.Failure("Пароли не совпадают");

            var user = await userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return Result<bool>.NotFound("Пользователь не найден");

            var result = await userManager.ChangePasswordAsync(user, dto.OldPassword, dto.NewPassword);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                return Result<bool>.Failure(errors);
            }

            logger.LogInformation($"Пользователь {dto.Email} сменил пароль");
            return Result<bool>.Success(true, "Пароль успешно изменён");
        }
        catch (Exception e)
        {
            logger.LogError(e, $"Ошибка при смене пароля пользователя Email-ом: {dto.Email}");
            return Result<bool>.Failure("Не удалось изменить пароль");
        }
    }
}