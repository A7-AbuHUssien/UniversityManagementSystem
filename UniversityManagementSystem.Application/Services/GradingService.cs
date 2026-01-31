using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Application.Services;
public class GradingService(IUnitOfWork unitOfWork) : IGradingService
{
    public async Task<bool> AssignGradeAsync(AssignGradeDto dto)
    {
        var enrollment = await unitOfWork.Repository<Enrollment>().GetOneAsync(
            e => e.StudentId == dto.studentId && 
                 e.CourseId == dto.courseId && 
                 e.Status == EnrollmentStatus.Active);
        if (enrollment == null)
            throw new InvalidOperationException("No active enrollment found for this student in this course.");
        
        Semester? semester = await unitOfWork.Repository<Semester>().GetOneAsync(s => s.Id == enrollment.SemesterId);
        if (semester == null || semester.IsActive)
            throw new InvalidOperationException("Cannot assign grade in Active Semester.");
        var grade = await unitOfWork.Repository<Grade>().GetOneAsync(
            g => dto.Score >= g.MinScore && dto.Score <= g.MaxScore);
        if (grade == null)
            throw new InvalidOperationException("The provided score does not map to any defined grade range.");

        enrollment.NumericScore = dto.Score;
        enrollment.GradeId = grade.Id;
        enrollment.Status = dto.Score >= 60 ? EnrollmentStatus.Completed : EnrollmentStatus.Failed;
        
        unitOfWork.Repository<Enrollment>().Update(enrollment);
        return await unitOfWork.CompleteAsync() > 0;
    }
}