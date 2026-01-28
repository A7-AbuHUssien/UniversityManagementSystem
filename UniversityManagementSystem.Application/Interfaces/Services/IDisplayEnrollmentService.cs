using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Parameters;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface IDisplayEnrollmentService
{
    Task<PaginatedResultDto<EnrollmentResponseDto>> GetEnrollmentsAsync(EnrollmentFilterDto filter);

}