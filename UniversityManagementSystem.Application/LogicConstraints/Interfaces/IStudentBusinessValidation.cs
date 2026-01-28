using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Application.LogicConstraints.Interfaces;

public interface IStudentBusinessValidation
{
    Task CheckEmailUniqueAsync(string email);
    Task<Department> CheckDepartmentExistAsync(string depName);
    Task CheckPhoneExistAsync(string phone);
    Task CheckAgeAsync(DateOnly dateOfBirth);
    Task<Student> CheckUserExistAsync(int id);
}