using UniversityManagementSystem.Application.DTOs.Dashboard_DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface IDashboardService
{
    Task<DashboardDto> GetAdminDashboardStatsAsync();
}