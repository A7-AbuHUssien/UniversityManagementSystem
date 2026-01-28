using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Application.LogicConstraints.Interfaces;

public interface IDepartmentBusinessValidation
{
    Task CheckNameUniqueAsync(string name);
    Task<Department> CheckDepartmentExistAsync(int id);
    Task CheckCodeUniqueAsync(string code);
}