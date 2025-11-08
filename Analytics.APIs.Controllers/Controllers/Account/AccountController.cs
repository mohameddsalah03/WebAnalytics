using Analytics.APIs.Controllers.Controllers.Base;
using Analytics.Core.Application.Abstraction.Services.Auth;
using Analytics.Shared.DTOs.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Analytics.APIs.Controllers.Controllers.Account;

public class AccountController : BaseApiController
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
    }

    
    [HttpPost("login")]
    public async Task<ActionResult<AuthResponseDto>> Login(LoginDto model)
    {
        var response = await _authService.LoginAsync(model);
        return Ok(response);
    }

   
    [HttpPost("register")]
    public async Task<ActionResult<AuthResponseDto>> Register(RegisterDto model)
    {
        var response = await _authService.RegisterAsync(model);
        return Ok(response);
    }
}