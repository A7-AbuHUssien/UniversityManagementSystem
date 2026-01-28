using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Parameters;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface IStudentService
{
    Task<PaginatedResultDto<StudentDto>> GetAllStudentsAsync(StudentFilterDto filter);
    Task<StudentDto?> GetStudentByIdAsync(int id);
    Task<StudentDto> CreateStudentAsync(StudentDto dto);
    Task<bool> UpdateStudentAsync(UpdateStudentDto dto);
    Task<bool> DeleteStudentAsync(int id);
    Task<IEnumerable<StudentDto>> GetDeletedStudentsAsync();
}
