using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.LogicConstraints;

public class EnrollmentBusinessValidation : IEnrollmentBusinessValidation
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentProgressService _studentProgressService;

    public EnrollmentBusinessValidation(IUnitOfWork unitOfWork, IStudentProgressService studentProgressService)
    {
        _unitOfWork = unitOfWork;
        _studentProgressService = studentProgressService;
    }

    public Task<bool> IsDropped(int studentId, int courseId, int semesterId)
    {
        var res = _unitOfWork.Repository<Enrollment>()
            .AnyAsync(e => e.StudentId == studentId
                           && e.CourseId == courseId
                           && e.SemesterId == semesterId
                           && e.Status == EnrollmentStatus.Dropped);
        return res;
    }

    public Task<bool> IsSuccessed(int studentId, int courseId)
    {
        var res = _unitOfWork.Repository<Enrollment>()
            .AnyAsync(e => e.StudentId == studentId
                           && e.CourseId == courseId
                           && e.Status == EnrollmentStatus.Completed);
        return res;
    }
    public async Task<bool> IsAlreadyEnrolledAsync(int studentId, int courseId, int semesterId)
    {
        return await _unitOfWork.Repository<Enrollment>()
            .AnyAsync(e => e.StudentId == studentId
                           && e.CourseId == courseId
                           && e.SemesterId == semesterId
                           && e.Status == EnrollmentStatus.Active);
    }

    public async Task<bool> IsPrerequisiteMetAsync(int studentId, int courseId)
    {
        Course? course = await _unitOfWork.Repository<Course>().GetOneAsync(e => e.Id == courseId);
        if (course == null)
            return false;
        if (course.PrerequisiteId == null)
            return true;

        return await _unitOfWork.Repository<Enrollment>().AnyAsync(e =>
            e.StudentId == studentId &&
            e.CourseId == course.PrerequisiteId &&
            e.NumericScore >= 60);
    }

    public async Task<bool> HasScheduleConflictAsync(int studentId, int courseId, int semesterId)
    {
        var newCourse = await _unitOfWork.Repository<Course>().GetOneAsync(c => c.Id == courseId);

        if (newCourse == null) return false;

        var studentEnrollments = await _unitOfWork.Repository<Enrollment>().GetAsync(
            expression: e => e.StudentId == studentId
                             && e.SemesterId == semesterId
                             && e.Status == EnrollmentStatus.Active,
            includes: [e => e.Course]
        );

        return studentEnrollments.Any(e =>
            e.Course.Day == newCourse.Day &&
            e.Course.Hour == newCourse.Hour);
    }

    public async Task<bool> IsSemesterValidForEnrollmentAsync(int semesterId)
    {
        var semester = await _unitOfWork.Repository<Semester>().GetOneAsync(s => s.Id == semesterId);
        if (semester == null || !semester.IsActive) return false;
        DateOnly today = DateOnly.FromDateTime(DateTime.Now);
        if (today > semester.EndDate)
            return false;
        return true;
    }

    public async Task<bool> IsHaveMaxHours(int studentId)
    {
        decimal gpa = await _studentProgressService.CalculateGPAAsync(studentId);
        var subs = await _unitOfWork.Repository<Enrollment>().GetAsync(e => e.StudentId == studentId &&
                                                                            e.Semester.IsActive == true,
            includes: [e => e.Semester, e => e.Course]);
        int studentHour = subs.Sum(e => e.Course.Credits);
        return (gpa <= (decimal)2.0) ?  studentHour >= 15 : studentHour >= 18;
    }
}