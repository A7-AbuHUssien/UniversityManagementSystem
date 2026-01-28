using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface IDepartmentService
{
    Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync();
    Task<DepartmentDto?> GetDepartmentByIdAsync(int id);
    Task<DepartmentDto> CreateDepartmentAsync(DepartmentDto dto);
    Task<bool> UpdateDepartmentAsync(int id, DepartmentDto dto);
}