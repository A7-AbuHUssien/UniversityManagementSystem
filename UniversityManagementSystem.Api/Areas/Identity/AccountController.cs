using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.Interfaces.Services.Identity;

namespace UniversityManagementSystem.Api.Areas.Identity;

[Area("Identity")]
[Route("api/[area]/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly IPasswordService _passwordService;
    private readonly IProfileService _profileService;
    private readonly IRegistrationService _registrationService;
    private readonly ITokenService _tokenService;
    public AccountController(IAuthService authService, IPasswordService passwordService, IProfileService profileService,
        IRegistrationService registrationService, ITokenService tokenService)
    {
        _authService = authService;
        _passwordService = passwordService;
        _profileService = profileService;
        _registrationService = registrationService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto model)
    {
        var result = await _authService.LoginAsync(model);
        if (!result.Succeeded) return Unauthorized(result);
        return Ok(result);
    }

    [HttpPost("register/student")]
    public async Task<IActionResult> RegisterStudent([FromBody] RegisterStudentDto model)
    {
        var result = await _registrationService.RegisterStudentAsync(model);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("register/instructor")]
    public async Task<IActionResult> RegisterInstructor([FromBody] RegisterInstructorDto model)
    {
        var result = await _registrationService.RegisterInstructorAsync(model);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        var result = await _passwordService.ForgotPasswordAsync(email);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var result = await _passwordService.ResetPasswordAsync(model);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var result = await _passwordService.ChangePasswordAsync(Guid.Parse(userIdClaim), model);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var result = await _profileService.UpdateProfileAsync(Guid.Parse(userIdClaim), model);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequestDto model)
    {
        var result = await _tokenService.RefreshTokenAsync(model);
        if (!result.Succeeded) return Unauthorized(result);
        return Ok(result);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] string refreshToken)
    {
        var result = await _authService.LogOutAsync(refreshToken);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }
}