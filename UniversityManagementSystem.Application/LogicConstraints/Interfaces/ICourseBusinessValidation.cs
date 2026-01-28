using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Application.LogicConstraints.Interfaces;

public interface ICourseBusinessValidation
{
    Task CheckCodeUniqueAsync(string courseCode);
    Task CheckDepartmentExistAsync(int departmentId);
    Task<Course> CheckCourseExistAsync(int id);
    Task<bool> IsCapacityAvailableAsync(int courseId);
}