using Analytics.Core.Application.Abstraction.Services.Auth;
using Analytics.Core.Domain.Contracts.Persistence;
using Analytics.Core.Domain.Entities;
using Analytics.Shared.DTOs.Auth;
using Analytics.Shared.Exceptions;

namespace Analytics.Core.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IUnitOfWork unitOfWork,
        IJwtTokenService jwtTokenService)
    {
        _unitOfWork = unitOfWork;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto registerDto)
    {
        // Check if email already exists
        if (await _unitOfWork.UsersRepo.IsEmailExistsAsync(registerDto.Email))
        {
            throw new BadRequestException("Email already exists");
        }

        // Hash password using BCrypt
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password);

        // Create new user
        var user = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            PasswordHash = passwordHash
        };

        await _unitOfWork .UsersRepo.AddAsync(user);
        await _unitOfWork.CompleteAsync();

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        // Find user by email using specific repository method
        var user = await _unitOfWork.UsersRepo.GetByEmailAsync(loginDto.Email);

        if (user is null)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        // Verify password
        var isPasswordValid = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new UnauthorizedException("Invalid email or password");
        }

        // Generate JWT token
        var token = _jwtTokenService.GenerateToken(user);

        return new AuthResponseDto
        {
            Token = token,
            Email = user.Email
        };
    }
}