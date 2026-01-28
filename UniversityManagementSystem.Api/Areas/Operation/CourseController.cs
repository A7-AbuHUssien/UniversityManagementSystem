using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Api.Areas.Operation;
[Area("Operation")]
[Route("api/[area]/[controller]")]
[ApiController]
public class CourseController : ControllerBase
{
    private readonly ICourseService _courseService;
    public CourseController(ICourseService courseService) => _courseService = courseService;

    [HttpGet]
    public async Task<IActionResult> GetAll() => Ok(await _courseService.GetAllAsync());
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id) => Ok(await _courseService.GetByIdAsync(id));
    
    [HttpPost]
    [ProducesResponseType(typeof(DepartmentDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCourseDto dto) 
    {
        var result = await _courseService.CreateAsync(dto);
        return CreatedAtAction("Create", new { id = result }, result);
    }
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] CreateCourseDto dto)
    {
        await _courseService.UpdateAsync(id, dto);
        return Ok(new { Message = "Course updated successfully." });
    }
}