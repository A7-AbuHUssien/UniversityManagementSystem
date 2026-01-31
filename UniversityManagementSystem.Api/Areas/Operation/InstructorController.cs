using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.Common;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Interfaces.Services;
namespace UniversityManagementSystem.Api.Areas.Operation;

[Area("Operation")]
[Route("api/[area]/[controller]")]
[ApiController]
[Authorize(Roles = AppRoles.OPERATION)]

public class InstructorController : ControllerBase
{
    private readonly IInstructorService _instructorService;

    public InstructorController(IInstructorService instructorService)
    {
        _instructorService = instructorService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<InstructorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll()
    {
        var instructors = await _instructorService.GetAllInstructorsAsync();
        return Ok(instructors);
    }

    [HttpGet("ByDepartment/{departmentId}")]
    [ProducesResponseType(typeof(IEnumerable<InstructorDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllByDepartment(int departmentId)
    {
        var instructors = await _instructorService.GetInstructorsByDepartmentAsync(departmentId);
        return Ok(instructors);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(int id)
    {
        var instructor = await _instructorService.GetInstructorByIdAsync(id);
        if (instructor == null)
            return NotFound(new { Message = $"Instructor with ID {id} not found." });

        return Ok(instructor);
    }

    [HttpPost]
    [ProducesResponseType(typeof(InstructorDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] InstructorDto dto)
    {
        var createdInstructor = await _instructorService.CreateInstructorAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = createdInstructor.Id }, createdInstructor);
    }

    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] InstructorDto dto)
    {
        await _instructorService.UpdateInstructorAsync(id, dto);
        return Ok(new { Message = "Instructor details updated successfully." });
    }
}