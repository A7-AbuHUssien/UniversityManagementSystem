
using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Interfaces.Services;
public interface IStudentProgressService
{
    Task<decimal> CalculateGPAAsync(int studentId);
    Task<IEnumerable<TranscriptDto>> GetTranscriptAsync(int studentId);
}