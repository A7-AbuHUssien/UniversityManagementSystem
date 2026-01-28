using System.Data.Common;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.Services;

public class StudentScheduleService : IStudentScheduleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentBusinessValidation _studentBusinessValidation;
    public StudentScheduleService(IUnitOfWork unitOfWork, IStudentBusinessValidation studentBusinessValidation)
    {
        _unitOfWork = unitOfWork;
        _studentBusinessValidation = studentBusinessValidation;
    }

    public async Task<IEnumerable<StudentScheduleDto>> GetStudentScheduleAsync(int studentId)
    {
        await _studentBusinessValidation.CheckUserExistAsync(studentId);
        Semester? semester = await _unitOfWork.Repository<Semester>().GetOneAsync(e => e.IsActive == true);
        if (semester == null)
            throw new ArgumentException("semester Does not Started Yet");
        var enrollments = await _unitOfWork.Repository<Enrollment>().GetAsync(
            expression: e => e.StudentId == studentId &&
                             e.SemesterId == semester.Id &&
                             e.Status == EnrollmentStatus.Active,
            includes: [e => e.Course, e => e.Course.Instructor]
        );

        return enrollments.Select(e => new StudentScheduleDto
        {
            CourseTitle = e.Course.Title,
            InstructorName = e.Course.Instructor.FirstName + " " + e.Course.Instructor.LastName,
            Day = e.Course.Day,
            Hour = e.Course.Hour
        });
    }
}