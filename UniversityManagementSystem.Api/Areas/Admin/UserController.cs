using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.Common;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Api.Areas.Admin;

[Route("api/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.SUPER_ADMIN)]
public class UserController : ControllerBase
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }
    [HttpGet("manage-users")]
    public async Task<IActionResult> GetAllUsers()
    {
        var result = await _userService.GetAllUsersExceptSuperAdminAsync();
        return Ok(result);
    }

    [HttpPost("toggle-status/{userId}")]
    public async Task<IActionResult> ToggleStatus(Guid userId)
    {
        var result = await _userService.ToggleUserStatusAsync(userId);
        if (!result.Succeeded) return BadRequest(result);
        return Ok(result);
    }
}