using Analytics.Core.Domain.Entities;

namespace Analytics.Core.Application.Abstraction.Services.Auth
{
    public interface IJwtTokenService
    {
        string GenerateToken(User user);
    }
}
