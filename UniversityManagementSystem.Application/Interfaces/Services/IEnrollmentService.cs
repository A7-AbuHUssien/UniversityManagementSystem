using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Parameters;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface IEnrollmentService
{
    Task<EnrollmentResponseDto> EnrollStudentAsync(EnrollmentRequestDto request);
    Task<bool> DropCourseAsync(int studentId, int courseId);
}