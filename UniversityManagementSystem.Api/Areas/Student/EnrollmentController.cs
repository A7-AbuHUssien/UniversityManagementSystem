using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Api.Areas.Student;
[Area("Student")]
[ApiController]
[Route("api/[controller]")]
public class EnrollmentController : ControllerBase
{
    private readonly IEnrollmentService _enrollmentService;

    public EnrollmentController(IEnrollmentService enrollmentService)
    {
        _enrollmentService = enrollmentService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Enroll([FromBody] EnrollmentRequestDto dto)
    {
        var result = await _enrollmentService.EnrollStudentAsync(dto);
        return Ok(new {message = "Course Enrolled successfully", result });
    }
    [HttpDelete("drop/{studentId}/{courseId}")]
    public async Task<IActionResult> Drop(int studentId, int courseId)
    {
        var success = await _enrollmentService.DropCourseAsync(studentId, courseId);
        return Ok(new { message = "Course dropped successfully", success });
    }

}