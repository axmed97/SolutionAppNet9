using Asp.Versioning;
using Business.Abstract;
using Entities.DTOs.AuthDTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace WebAPI.Controllers.V1;

[ApiVersion("1.0")]
[Route("api/v{v:apiVersion}/[controller]")]
[ApiController]
public class AuthController(IAuthService authService) : ControllerBase
{
    [MapToApiVersion("1.0")]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto model)
    {
        var result = await authService.RegisterAsync(model);
        return StatusCode((int)result.StatusCode, result);
    }

    [MapToApiVersion("1.0")]
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto model)
    {
        var result = await authService.LoginAsync(model);
        return StatusCode((int)result.StatusCode, result);
    }

    [MapToApiVersion("1.0")]
    [HttpPost("refresh-token-login")]
    public async Task<IActionResult> RefreshTokenLogin(RefreshTokenDto model)
    {
        var result = await authService.RefreshTokenLoginAsync(model);
        return StatusCode((int)result.StatusCode, result);
    }

    [MapToApiVersion("1.0")]
    [HttpPut("logout")]
    [Authorize]
    public async Task<IActionResult> LogOut()
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var result = await authService.LogOutAsync(userId);
        return StatusCode((int)result.StatusCode, result);
    }

    [MapToApiVersion("1.0")]
    [HttpGet("get-all")]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var result = await authService.GetAllAsync();
        return StatusCode((int)result.StatusCode, result);
    }

    [MapToApiVersion("1.0")]
    [HttpDelete("{userId}")]
    [Authorize]
    public async Task<IActionResult> Remove(string userId)
    {
        var result = await authService.RemoveUserAsync(userId);
        return StatusCode((int)result.StatusCode, result);
    }

    [MapToApiVersion("1.0")]
    [HttpPost("google-auth")]
    public async Task<IActionResult> GoogleAuth(GoogleAuthDto model)
    {
        var result = await authService.GoogleAuthAsync(model);
        return StatusCode((int)result.StatusCode, result);
    }
}