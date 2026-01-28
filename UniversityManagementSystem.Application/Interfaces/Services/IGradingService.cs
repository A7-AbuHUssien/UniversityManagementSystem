using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface IGradingService
{
    Task<bool> AssignGradeAsync(AssignGradeDto dto);
}