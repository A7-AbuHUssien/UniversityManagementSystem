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
        if (!await _unitOfWork.Repository<Student>().AnyAsync(s => s.Id == studentId))
            throw new ArgumentException("No Student Found");
        
        Semester? semester = await _unitOfWork.Repository<Semester>().GetOneAsync(e => e.IsActive == true);
        if (semester == null)
            throw new ArgumentException("semester Does not Started Yet");
        IEnumerable<Enrollment> enrollments = await _unitOfWork.Repository<Enrollment>().GetAsync(
            expression: e => e.StudentId == studentId &&
                             e.SemesterId == semester.Id &&
                             e.Status == EnrollmentStatus.Active,
            includes: [e => e.Course, e => e.Course.Instructor]
        );
        if (!enrollments.Any())
        {
            if (semester.IsRegistrationOpen)
                throw new ArgumentException("No Active Courses, Go Enroll");
            throw new ArgumentException("No Active Courses, Talk The Operation to Enroll");
        }

        return enrollments.Select(e => new StudentScheduleDto
        {
            CourseTitle = e.Course.Title,
            InstructorName = e.Course.Instructor.FirstName + " " + e.Course.Instructor.LastName,
            Day = e.Course.Day,
            Hour = e.Course.Hour
        });
    }
}