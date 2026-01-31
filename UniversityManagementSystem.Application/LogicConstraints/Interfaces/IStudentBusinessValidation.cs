using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Application.LogicConstraints.Interfaces;

public interface IStudentBusinessValidation
{
    Task<bool> IsValidAgeAsync(DateOnly dateOfBirth);
}