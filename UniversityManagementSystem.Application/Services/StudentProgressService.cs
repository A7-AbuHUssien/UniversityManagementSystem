using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Application.Services;

public class StudentProgressService : IStudentProgressService
{
    private readonly IUnitOfWork _unitOfWork;
    public StudentProgressService(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task<decimal> CalculateGPAAsync(int studentId)
    {
        var enrollmentRecords = await _unitOfWork.Repository<Enrollment>().GetAsync(
            e => e.StudentId == studentId && e.Status == EnrollmentStatus.Completed,
            includes: [e => e.Grade, e => e.Course]);

        var records = enrollmentRecords.ToList();

        if (records.Count == 0) return 0m;

        decimal totalPoints = records.Sum(e => (e.Grade?.GPAPoint ?? 0m) * (e.Course?.Credits ?? 0));
        int totalCredits = records.Sum(e => e.Course?.Credits ?? 0);

        return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0m;
    }
    
    public async Task<IEnumerable<TranscriptDto>> GetTranscriptAsync(int studentId)
    {
        var enrollmentRecords = await _unitOfWork.Repository<Enrollment>().GetAsync(
            expression: e => e.StudentId == studentId && 
                             (e.Status == EnrollmentStatus.Completed || e.Status == EnrollmentStatus.Failed),
            includes: [e => e.Course, e => e.Grade, e => e.Semester]
        );

        var records = enrollmentRecords.ToList();

        if (records.Count == 0)
        {
            return Enumerable.Empty<TranscriptDto>();
        }

        return records
            .OrderBy(e => e.Semester?.Year ?? 0)
            .ThenBy(e => e.Semester?.Name ?? string.Empty)
            .Select(e => new TranscriptDto
            {
                CourseTitle = e.Course?.Title ?? "Unknown Course",
                SemesterName = e.Semester != null ? $"{e.Semester.Name} {e.Semester.Year}" : "N/A",
                Credits = e.Course?.Credits ?? 0,
                NumericScore = e.NumericScore ?? 0,
                GradeSymbol = e.Grade?.Symbol ?? "F",
                GradePoints = e.Grade?.GPAPoint ?? 0
            });
    }
}