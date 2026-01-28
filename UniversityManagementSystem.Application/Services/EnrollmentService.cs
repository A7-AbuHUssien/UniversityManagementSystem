using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Parameters;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.Services;

public class EnrollmentService : IEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnrollmentBusinessValidation _validator;
    private readonly IStudentBusinessValidation _studentValidator;
    private readonly ICourseBusinessValidation _courseValidator;

    public EnrollmentService(IUnitOfWork unitOfWork, IEnrollmentBusinessValidation validator,
        IStudentBusinessValidation studentValidator, ICourseBusinessValidation courseValidator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
        _studentValidator = studentValidator;
        _courseValidator = courseValidator;
    }
    public async Task<EnrollmentResponseDto> EnrollStudentAsync(EnrollmentRequestDto dto)
    {
        int semsterId = (await _unitOfWork.Repository<Semester>().GetOneAsync(s => s.IsActive == true)).Id;
        
        // This is Fuck The Performance I Know . In the Future I will Write Validation hits Database Less than this. 
        if(await _validator.IsSuccessed(dto.StudentId, dto.CourseId))
            throw new InvalidOperationException("You Already Succeeded in this Course");
        if (!await _validator.IsSemesterValidForEnrollmentAsync(semsterId))
            throw new InvalidOperationException("Semester is Not valid");
        if ((await _studentValidator.CheckUserExistAsync(dto.StudentId)) == null)
            throw new InvalidOperationException("There is no student with this id");
        if ((await _courseValidator.CheckCourseExistAsync(dto.CourseId)) == null)
            throw new InvalidOperationException("There is no course with this id");
        if (await _validator.IsAlreadyEnrolledAsync(dto.StudentId, dto.CourseId, semsterId))
            throw new InvalidOperationException("Student already enrolled");
        if (await _validator.IsDropped(dto.StudentId, dto.CourseId, semsterId))
            return await ReturnDroppedEnrollmentAsync(dto.StudentId, dto.CourseId, semsterId);
        if (!await _courseValidator.IsCapacityAvailableAsync(dto.CourseId))
            throw new InvalidOperationException("Course capacity Is Full");
        if (!await _validator.IsPrerequisiteMetAsync(dto.StudentId, dto.CourseId))
            throw new InvalidOperationException("You don't have the required Prerequisites");
        if (await _validator.HasScheduleConflictAsync(dto.StudentId, dto.CourseId, semsterId))
            throw new InvalidOperationException(
                "Schedule conflict, Contact the college administration to check the possibility of registration");
        if (await _validator.IsHaveMaxHours(dto.StudentId))
            throw new InvalidOperationException("Student has Max Hours");

        Enrollment enrollment = new Enrollment
        {
            StudentId = dto.StudentId,
            CourseId = dto.CourseId,
            SemesterId = semsterId,
            CreatedAt = DateTime.UtcNow,
            Status = EnrollmentStatus.Active,
        };
        await _unitOfWork.Repository<Enrollment>().CreateAsync(enrollment);
        await _unitOfWork.CompleteAsync();

        var student = await _unitOfWork.Repository<Student>().GetOneAsync(e => e.Id == dto.StudentId);
        var course = await _unitOfWork.Repository<Course>().GetOneAsync(e => e.Id == dto.CourseId);
        return new EnrollmentResponseDto
        {
            EnrollmentId = enrollment.Id,
            StudentName = $"{student?.FirstName} {student?.LastName}",
            CourseTitle = course?.Title ?? "N/A",
            EnrollmentDate = enrollment.EnrollmentDate,
            Status = "Active"
        };
    }

    public async Task<bool> DropCourseAsync(int studentId, int courseId)
    {
        var e = await _unitOfWork.Repository<Enrollment>().GetOneAsync(e =>
            e.StudentId == studentId && e.CourseId == courseId && e.Status == EnrollmentStatus.Active);
        if (e == null)
            throw new InvalidOperationException("No active enrollment found for this course");
        e.Status = EnrollmentStatus.Dropped;
        _unitOfWork.Repository<Enrollment>().Update(e);
        var result = await _unitOfWork.CompleteAsync();
        return result > 0;
    }

    private async Task<EnrollmentResponseDto> ReturnDroppedEnrollmentAsync(int studentId, int courseId, int semesterId)
    {
        Enrollment? droppedSubject = await _unitOfWork.Repository<Enrollment>()
            .GetOneAsync(e => e.StudentId == studentId
                              && e.CourseId == courseId
                              && e.SemesterId == semesterId
                              && e.Status == EnrollmentStatus.Dropped);
        droppedSubject.Status = EnrollmentStatus.Active;
        _unitOfWork.Repository<Enrollment>().Update(droppedSubject);
        await _unitOfWork.CompleteAsync();


        var student = await _unitOfWork.Repository<Student>().GetOneAsync(e => e.Id == studentId);
        var course = await _unitOfWork.Repository<Course>().GetOneAsync(e => e.Id == courseId);
        return new EnrollmentResponseDto
        {
            EnrollmentId = droppedSubject.Id,
            StudentName = $"{student?.FirstName} {student?.LastName}",
            CourseTitle = course?.Title ?? "N/A",
            EnrollmentDate = droppedSubject.EnrollmentDate,
            Status = "Active"
        };
    }
}