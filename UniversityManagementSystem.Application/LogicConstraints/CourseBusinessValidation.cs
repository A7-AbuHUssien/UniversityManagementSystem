using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.LogicConstraints;

public class CourseBusinessValidation : ICourseBusinessValidation
{
    private readonly IUnitOfWork _unitOfWork;
    public CourseBusinessValidation(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task CheckCodeUniqueAsync(string courseCode)
    {
        var exists = await _unitOfWork.Repository<Course>().GetOneAsync(c => c.CourseCode == courseCode);
        if (exists != null) throw new InvalidOperationException($"Course code '{courseCode}' is already in use.");
    }

    public async Task CheckDepartmentExistAsync(int departmentId)
    {
        var dept = await _unitOfWork.Repository<Department>().GetOneAsync(e => e.Id == departmentId);
        if (dept == null) throw new KeyNotFoundException("Department not found.");
    }

    public async Task<Course> CheckCourseExistAsync(int id)
    {
        var course = await _unitOfWork.Repository<Course>().GetOneAsync(e => e.Id == id);
        if (course == null) throw new KeyNotFoundException("Course not found.");
        return course;
    }
    public async Task<bool> IsCapacityAvailableAsync(int courseId)
    {
        Course? course = await _unitOfWork.Repository<Course>().GetOneAsync(e => e.Id == courseId);
        if (course == null)
            return false;
        
        int courseCount = await _unitOfWork.Repository<Enrollment>()
            .CountAsync(e => e.CourseId == courseId && e.Status == EnrollmentStatus.Active);
        if (courseCount >= course.MaxCapacity)
            return false;
        return true;
    }
}