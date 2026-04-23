using Domain.Entities;

namespace Application.Interfaces.Services.TokenService;

public interface IJwtService
{
    string GenerateToken(ApplicationUser user, IList<string> roles);
}