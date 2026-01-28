using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.LogicConstraints;

public class StudentBusinessValidation : IStudentBusinessValidation
{
    private readonly IUnitOfWork _unitOfWork;

    public StudentBusinessValidation(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task CheckEmailUniqueAsync(string email)
    {
        var exists = await _unitOfWork.Repository<Student>().GetOneAsync(s => s.Email == email);
        if (exists != null)
        {
            throw new InvalidOperationException("Email already exists");
        }
    }

    public async Task<Department> CheckDepartmentExistAsync(string depName)
    {
        Department? dep = await _unitOfWork.Repository<Department>().GetOneAsync(d => d.Name == depName);
        if (dep == null) throw new Exception("Invalid Department Name");
        return dep;
    }

    public async Task CheckPhoneExistAsync(string phone)
    {
        if (await _unitOfWork.Repository<Student>().GetOneAsync(s => s.Phone == phone) != null)
            throw new Exception("Invalid Phone Number");
    }

    public Task CheckAgeAsync(DateOnly dateOfBirth)
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        var minAllowedBirthDate = today.AddYears(-18);
        if (dateOfBirth > minAllowedBirthDate)
            throw new Exception("Student must be at least 18 years old.");

        return Task.CompletedTask;
    }

    public async Task<Student> CheckUserExistAsync(int id)
    {
       var student = await _unitOfWork.Repository<Student>().GetOneAsync(s => s.Id == id);
       if (student == null)
            throw new Exception("Invalid Student Id");
       return student;
    }
}