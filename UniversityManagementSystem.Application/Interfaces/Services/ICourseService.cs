using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface ICourseService
{
    Task<IEnumerable<CourseDto>> GetAllAsync();
    Task<CourseDto?> GetByIdAsync(int id);
    Task<CreateCourseDto> CreateAsync(CreateCourseDto dto);
    Task<bool> UpdateAsync(int id, CreateCourseDto dto);
}