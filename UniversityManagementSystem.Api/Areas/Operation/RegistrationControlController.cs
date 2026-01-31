using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.Common;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Api.Areas.Operation;

[Area("Operation")]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = $"{AppRoles.ADMIN},{AppRoles.OPERATION}")]

public class RegistrationControlController : ControllerBase
{
    private readonly IRegistrationControlService _regService;

    public RegistrationControlController(IRegistrationControlService regService)
    {
        _regService = regService;
    }

    [HttpPost("open")]
    public async Task<IActionResult> Open()
    {
        await _regService.OpenRegistrationAsync();
        return Ok(new { Message = "Registration is now OPEN." });
    }

    [HttpPost("close")]
    public async Task<IActionResult> Close()
    {
        await _regService.CloseRegistrationAsync();
        return Ok(new { Message = "Registration is now CLOSED." });
    }
}