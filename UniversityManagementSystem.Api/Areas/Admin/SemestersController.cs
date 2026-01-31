using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.Common;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Api.Areas.Admin;

[Area("Admin")]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.ADMIN)]

public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var semesters = await _semesterService.GetAllSemestersAsync();
        return Ok(semesters);
    }

    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var activeSemester = await _semesterService.GetActiveSemesterAsync();
        if (activeSemester == null)
            return NotFound("No active semester found.");

        return Ok(activeSemester);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SemesterDto semesterDto)
    {
        var createdSemester = await _semesterService.CreateSemesterAsync(semesterDto);
        return CreatedAtAction(nameof(GetActive), new { id = createdSemester.Id }, createdSemester);
    }

    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        var result = await _semesterService.ActivateSemesterAsync(id);
        if (!result)
            return NotFound($"Semester with ID {id} not found.");

        return Ok(new { Message = $"Semester {id} is now active. All others are deactivated." });
    }
}