using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.DTOs.Identity_DTOs;
using UniversityManagementSystem.Application.Interfaces.Services.Identity;

namespace UniversityManagementSystem.Api.Areas.Identity;

[Area("Identity")]
[Route("api/[area]/[controller]")]
[ApiController]
public class AccountController : ControllerBase
{
    private readonly IAuthService _authService;

    public AccountController(IAuthService authService)
    {
        _authService = authService;
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
        var result = await _authService.RegisterStudentAsync(model);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("register/instructor")]
    public async Task<IActionResult> RegisterInstructor([FromBody] RegisterInstructorDto model)
    {
        var result = await _authService.RegisterInstructorAsync(model);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword([FromBody] string email)
    {
        var result = await _authService.ForgotPasswordAsync(email);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
    {
        var result = await _authService.ResetPasswordAsync(model);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [Authorize]
    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var result = await _authService.ChangePasswordAsync(Guid.Parse(userIdClaim), model);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

    [Authorize]
    [HttpPut("update-profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto model)
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (string.IsNullOrEmpty(userIdClaim)) return Unauthorized();

        var result = await _authService.UpdateProfileAsync(Guid.Parse(userIdClaim), model);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }

}