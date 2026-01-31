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

public class StudentScheduleController : ControllerBase
{
    private readonly IStudentScheduleService _scheduleService;

    public StudentScheduleController(IStudentScheduleService scheduleService)
    {
        _scheduleService = scheduleService;
    }

    [HttpGet("get-student-scheduleService/{studentId}")]
    public async Task<IActionResult> GetSchedule(int studentId)
    {
        var dd = await _scheduleService.GetStudentScheduleAsync(studentId);
        return Ok(dd);
    }
}