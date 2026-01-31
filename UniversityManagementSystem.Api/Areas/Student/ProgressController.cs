using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.Common;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Api.Areas.Student;

[Area("Student")]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = $"{AppRoles.STUDENT},{AppRoles.OPERATION}")]
public class ProgressController : ControllerBase
{
    private readonly IStudentProgressService _progressService;

    public ProgressController(IStudentProgressService progressService)
    {
        _progressService = progressService;
    }

    [HttpGet("gpa/{studentId}")]
    public async Task<IActionResult> GetGpa(int studentId)
    {
        var gpa = await _progressService.CalculateGPAAsync(studentId);
        return Ok(new { StudentId = studentId, GPA = gpa });
    }

    [HttpGet("transcript/{studentId}")]
    public async Task<IActionResult> GetTranscript(int studentId)
    {
        var transcript = await _progressService.GetTranscriptAsync(studentId);
        
        if (!transcript.Any())
        {
            return NotFound($"No academic record found for Student ID: {studentId}");
        }
        return Ok(transcript);
    }
}