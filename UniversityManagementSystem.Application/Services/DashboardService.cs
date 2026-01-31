using Mapster;
using UniversityManagementSystem.Application.DTOs.Dashboard_DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Application.Services;

public class DashboardService : IDashboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentProgressService _studentProgressService;

    public DashboardService(IUnitOfWork unitOfWork, IStudentProgressService studentProgressService)
    {
        _unitOfWork = unitOfWork;
        _studentProgressService = studentProgressService;
    }

    public async Task<DashboardDto> GetAdminDashboardStatsAsync()
    {
        var semester = await _unitOfWork.Repository<Semester>().GetOneAsync(s => s.IsActive);
        if (semester == null)
            throw new InvalidOperationException("No active semester found. Please activate a semester first.");
        var criticalCourses = await GetCriticalCoursesAsync(semester.Id);
        var (atRiskCount, atRiskPercentage) = await GetAtRiskStatsAsync();
        int studentsWithNoEnrollments = await GetStudentsWithNoEnrollmentsAsync(semester.Id);
        var problematicCourses = await GetProblematicCoursesAsync(semester.Id);
        return new DashboardDto
        {
            AtRiskStudentsCount = atRiskCount,
            AtRiskPercentage = atRiskPercentage,
            BottleneckCourses = criticalCourses,
            StudentsWithNoEnrollments = studentsWithNoEnrollments,
            ProblematicCourses = problematicCourses
        };
    }

    private async Task<List<CriticalCourseDto>> GetCriticalCoursesAsync(int semesterId)
    {
        return _unitOfWork.Repository<Course>().Query(tracked: false)
            .Where(c => c.IsActive &&
                        (c.MaxCapacity > 0 && (double)c.CurrentCapacity / c.MaxCapacity >= 0.9))
            .Select(c => new CriticalCourseDto
            {
                CourseTitle = c.Title,
                CurrentStudents = c.CurrentCapacity,
                MaxCapacity = c.MaxCapacity,
                FullnessPercentage = (double)c.CurrentCapacity / c.MaxCapacity * 100
            })
            .ToList();
    }

    private async Task<int> GetStudentsWithNoEnrollmentsAsync(int semesterId)
    {
        return _unitOfWork.Repository<Student>().Query()
            .Count(s =>
                !s.Enrollments.Any(e => e.SemesterId == semesterId && e.Status == EnrollmentStatus.Active));
    }

    private async Task<(int , double)> GetAtRiskStatsAsync()
    {
        var totalStudents = await _unitOfWork.Repository<Student>().CountAsync();
        if (totalStudents == 0) return (0, 0);

        var atRiskCount = _unitOfWork.Repository<Enrollment>().Query()
            .Where(e => e.Grade != null)
            .GroupBy(e => e.StudentId)
            .Select(g => new
            {
                StudentId = g.Key,
                GPA = g.Sum(e => e.Grade.GPAPoint * e.Course.Credits) / (decimal)g.Sum(e => e.Course.Credits)
            })
            .Count(x => x.GPA < 2.0m);
        double percentage = (double)atRiskCount / totalStudents * 100;
        return (atRiskCount, percentage);
    }
    private Task<List<HighDropRateCourseDto>> GetProblematicCoursesAsync(int semesterId)
    {
        return Task.FromResult(_unitOfWork.Repository<Enrollment>().Query(tracked: false)
            .Where(e => e.SemesterId == semesterId)
            .GroupBy(e => e.Course.Title) 
            .Select(g => new HighDropRateCourseDto
            {
                CourseTitle = g.Key,
                DropCount = g.Count(e => e.Status == EnrollmentStatus.Dropped),
                DropRate = g.Any() 
                    ? (double)g.Count(e => e.Status == EnrollmentStatus.Dropped) / g.Count() * 100 
                    : 0
            })
            .Where(x => x.DropRate > 15) 
            .OrderByDescending(x => x.DropRate) 
            .Take(5) 
            .ToList());
    }
}