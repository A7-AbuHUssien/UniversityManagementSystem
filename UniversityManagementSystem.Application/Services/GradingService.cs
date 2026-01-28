using MapsterMapper;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.Services;
public class GradingService : IGradingService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentBusinessValidation _studentValidation;
    private readonly ICourseBusinessValidation _courseValidation;
    private readonly IEnrollmentBusinessValidation _enrollmentValidation;
    public GradingService(IUnitOfWork unitOfWork,
        IStudentBusinessValidation studentValidation,
        ICourseBusinessValidation courseValidation,
        IEnrollmentBusinessValidation enrollmentValidation)
    {
        _unitOfWork = unitOfWork;
        _studentValidation = studentValidation;
        _courseValidation = courseValidation;
        _enrollmentValidation = enrollmentValidation;
    }
    
    public async Task<bool> AssignGradeAsync(AssignGradeDto dto)
    {
        await _studentValidation.CheckUserExistAsync(dto.studentId);
        await _courseValidation.CheckCourseExistAsync(dto.courseId);
        var enrollment = await _unitOfWork.Repository<Enrollment>().GetOneAsync(
            e => e.StudentId == dto.studentId && 
                 e.CourseId == dto.courseId && 
                 e.Status == EnrollmentStatus.Active);
        if (enrollment == null)
            throw new InvalidOperationException("No active enrollment found for this student in this course.");
        Semester? semester = await _unitOfWork.Repository<Semester>().GetOneAsync(s => s.Id == enrollment.SemesterId);
        if (semester == null || semester.IsActive)
            throw new InvalidOperationException("Cannot assign grade in Active Semester.");
        var grade = await _unitOfWork.Repository<Grade>().GetOneAsync(
            g => dto.Score >= g.MinScore && dto.Score <= g.MaxScore);

        if (grade == null)
            throw new InvalidOperationException("The provided score does not map to any defined grade range.");

        enrollment.NumericScore = dto.Score;
        enrollment.GradeId = grade.Id;
        enrollment.Status = dto.Score >= 60 ? EnrollmentStatus.Completed : EnrollmentStatus.Failed;
        
        _unitOfWork.Repository<Enrollment>().Update(enrollment);
        return await _unitOfWork.CompleteAsync() > 0;
    }
}