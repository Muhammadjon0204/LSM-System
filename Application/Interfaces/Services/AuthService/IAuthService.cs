using Application.Common;
using Application.DTOs.AuthDTOs;

namespace Application.Interfaces.Services.AuthService;

public interface IAuthService
{
    Task<Result<AuthResponseDto>> RegisterAsync(RegisterDto dto);
    Task<Result<AuthResponseDto>> LoginAsync(LoginDto dto);
    Task<Result<bool>> ForgotPasswordAsync(ForgotPasswordDto dto);
    Task<Result<bool>> ResetPasswordAsync(ResetPasswordDto dto);
    Task<Result<bool>> ChangePasswordAsync(ChangePasswordDto dto);
}