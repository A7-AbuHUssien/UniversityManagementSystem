using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface IInstructorService
{
    Task<IEnumerable<InstructorDto>> GetAllInstructorsAsync();

    Task<InstructorDto?> GetInstructorByIdAsync(int id);

    Task<InstructorDto> CreateInstructorAsync(InstructorDto dto);

    Task<bool> UpdateInstructorAsync(int id, InstructorDto dto);

    Task<IEnumerable<InstructorDto>> GetInstructorsByDepartmentAsync(int departmentId);
}