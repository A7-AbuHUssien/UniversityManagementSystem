using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.Common;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Api.Areas.Operation;

[Area("Operations")]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.OPERATION)]

public class GradingController : ControllerBase
{
    private readonly IGradingService _gradingService;

    public GradingController(IGradingService gradingService)
    {
        _gradingService = gradingService;
    }

    [HttpPost("assign-grade")]
    public async Task<IActionResult> AssignGrade([FromBody] AssignGradeDto dto)
    {
        await _gradingService.AssignGradeAsync(dto);
        return Ok(new { Message = "Grade assigned and enrollment status updated successfully." });
    }
}