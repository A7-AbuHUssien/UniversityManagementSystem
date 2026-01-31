using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.LogicConstraints;

public class StudentBusinessValidation : IStudentBusinessValidation
{
    public Task<bool> IsValidAgeAsync(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var minAllowedBirthDate = today.AddYears(-18);
        if (dateOfBirth > minAllowedBirthDate)
            return Task.FromResult(false);
        return Task.FromResult(true);
    }
}