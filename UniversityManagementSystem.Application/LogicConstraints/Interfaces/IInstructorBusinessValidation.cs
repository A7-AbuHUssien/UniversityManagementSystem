using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Application.LogicConstraints.Interfaces;

public interface IInstructorBusinessValidation
{
    Task CheckEmailUniqueAsync(string email);
    Task<Instructor> CheckInstructorExistAsync(int id);
    Task CheckDepartmentExistAsync(int departmentId);
}