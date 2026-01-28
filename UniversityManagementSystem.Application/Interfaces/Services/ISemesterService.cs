using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface ISemesterService
{
    Task<IEnumerable<SemesterDto>> GetAllSemestersAsync();
    Task<SemesterDto?> GetActiveSemesterAsync();
    Task<SemesterDto> CreateSemesterAsync(SemesterDto semesterDto);
    Task<bool> ActivateSemesterAsync(int id);
}