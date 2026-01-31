using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;
using UniversityManagementSystem.Application.LogicConstraints.States;

namespace UniversityManagementSystem.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnrollmentBusinessValidation _rules;
    private readonly IStudentProgressService _studentProgressService;

    public EnrollmentService(
        IUnitOfWork unitOfWork,
        IEnrollmentBusinessValidation validator,
        IStudentProgressService studentProgressService)
    {
        _unitOfWork = unitOfWork;
        _rules = validator;
        _studentProgressService = studentProgressService;
    }

    public async Task<EnrollmentResponseDto> EnrollStudentAsync(EnrollmentRequestDto dto)
    {
        Semester? semester = await _unitOfWork.Repository<Semester>().GetOneAsync(s => s.IsActive);
        if (semester == null) throw new InvalidOperationException("No active semester available");
        if (semester.IsRegistrationOpen == false)
            throw new InvalidOperationException("Registration is not open Currently");
        Student? student = await _unitOfWork.Repository<Student>().GetOneAsync(s => s.Id == dto.StudentId);
        if (student == null) throw new InvalidOperationException("Student not found");

        var course = await _unitOfWork.Repository<Course>().GetOneAsync(c => c.Id == dto.CourseId);
        if (course == null) throw new InvalidOperationException("Course not found");
        if (course.IsActive == false) throw new InvalidOperationException("Course is not Active");

        var allEnrollments = (await _unitOfWork.Repository<Enrollment>().GetAsync(e =>
                e.StudentId == dto.StudentId
                && e.SemesterId == semester.Id,
            includes: [e => e.Course]
        )).ToList();

        var gpa = await _studentProgressService.CalculateGPAAsync(dto.StudentId);
        var state = new EnrollmentValidationState(
            studentId: dto.StudentId,
            semesterId: semester.Id,
            allEnrollments: allEnrollments,
            targetCourse: course,
            gpa: gpa
        );
        bool dropped = false;
        if (_rules.HasAlreadyCompletedCourse(state))
            throw new InvalidOperationException("Student already completed this course");
        if (_rules.IsAlreadyEnrolled(state))
            throw new InvalidOperationException("Student already enrolled in this course");
        if (_rules.IsDropped(state)) dropped = true;
        if (!_rules.IsPrerequisiteMet(state)) throw new InvalidOperationException("Prerequisites not satisfied");
        if (_rules.HasScheduleConflict(state)) throw new InvalidOperationException("Schedule conflict detected");
        if (_rules.HasReachedMaxHours(state))
            throw new InvalidOperationException("Student has reached maximum allowed credit hours");

        Enrollment? enrollmentbase = new Enrollment();
        if (dropped)
        {
            enrollmentbase = await _unitOfWork.Repository<Enrollment>().GetOneAsync(e =>
                e.StudentId == dto.StudentId &&
                e.CourseId == dto.CourseId &&
                e.Status == EnrollmentStatus.Dropped);
            enrollmentbase.Status = EnrollmentStatus.Active;
            _unitOfWork.Repository<Enrollment>().Update(enrollmentbase);
            await _unitOfWork.CompleteAsync();
        }
        else
        {
            enrollmentbase = new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                SemesterId = semester.Id,
                Status = EnrollmentStatus.Active,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<Enrollment>().CreateAsync(enrollmentbase);
            await _unitOfWork.CompleteAsync();
        }

        return new EnrollmentResponseDto
        {
            EnrollmentId = enrollmentbase.Id,
            StudentName = $"{student.FirstName} {student.LastName}",
            CourseTitle = course.Title,
            EnrollmentDate = enrollmentbase.CreatedAt,
            Status = "Active"
        };
    }

    public async Task<bool> DropCourseAsync(int studentId, int courseId)
    {
        Semester? semester = await _unitOfWork.Repository<Semester>().GetOneAsync(s => s.IsActive);
        if (semester == null) throw new InvalidOperationException("No active semester available");
        if (semester.IsRegistrationOpen == false)
            throw new InvalidOperationException("Registration is not open Currently");

        var e = await _unitOfWork.Repository<Enrollment>().GetOneAsync(e =>
            e.StudentId == studentId && e.CourseId == courseId && e.Status == EnrollmentStatus.Active);
        if (e == null)
            throw new InvalidOperationException("No active enrollment found for this course");
        e.Status = EnrollmentStatus.Dropped;
        _unitOfWork.Repository<Enrollment>().Update(e);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0;
    }
}