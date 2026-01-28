using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Api.Areas.Admin;

[Area("Admin")]
[Route("api/[area]/[controller]")]
[ApiController]
public class SemestersController : ControllerBase
{
    private readonly ISemesterService _semesterService;

    public SemestersController(ISemesterService semesterService)
    {
        _semesterService = semesterService;
    }
    // 1. Get all semesters
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var semesters = await _semesterService.GetAllSemestersAsync();
        return Ok(semesters);
    }

    // 2. Get the currently active semester
    [HttpGet("active")]
    public async Task<IActionResult> GetActive()
    {
        var activeSemester = await _semesterService.GetActiveSemesterAsync();
        if (activeSemester == null)
            return NotFound("No active semester found.");

        return Ok(activeSemester);
    }

    // 3. Create a new semester
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] SemesterDto semesterDto)
    {
        var createdSemester = await _semesterService.CreateSemesterAsync(semesterDto);
        return CreatedAtAction(nameof(GetActive), new { id = createdSemester.Id }, createdSemester);
    }

    // 4. Activate a specific semester
    [HttpPatch("{id}/activate")]
    public async Task<IActionResult> Activate(int id)
    {
        var result = await _semesterService.ActivateSemesterAsync(id);
        if (!result)
            return NotFound($"Semester with ID {id} not found.");

        return Ok(new { Message = $"Semester {id} is now active. All others are deactivated." });
    }
}