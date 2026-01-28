using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface IStudentScheduleService
{
    Task<IEnumerable<StudentScheduleDto>> GetStudentScheduleAsync(int studentId);
}