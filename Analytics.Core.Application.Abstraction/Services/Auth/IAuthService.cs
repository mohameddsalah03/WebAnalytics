using Analytics.Shared.DTOs.Auth;

namespace Analytics.Core.Application.Abstraction.Services.Auth
{
    public interface IAuthService
    {
        Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto);
        Task<AuthResponseDto> LoginAsync(LoginDto loginDto);
    }
}
