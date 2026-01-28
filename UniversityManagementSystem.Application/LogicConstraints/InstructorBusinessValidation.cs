using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.LogicConstraints;

public class InstructorBusinessValidation : IInstructorBusinessValidation
{
    private readonly IUnitOfWork _unitOfWork;
    public InstructorBusinessValidation(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task CheckEmailUniqueAsync(string email)
    {
        var exists = await _unitOfWork.Repository<Instructor>().GetOneAsync(i => i.Email == email);
        if (exists != null) throw new InvalidOperationException("Email is already assigned to another instructor.");
    }

    public async Task<Instructor> CheckInstructorExistAsync(int id)
    {
        var instructor = await _unitOfWork.Repository<Instructor>().GetOneAsync(e => e.Id == id);
        if (instructor == null) throw new KeyNotFoundException("Instructor not found.");
        return instructor;
    }

    public async Task CheckDepartmentExistAsync(int departmentId)
    {
        var dept = await _unitOfWork.Repository<Department>().GetOneAsync(e => e.Id == departmentId);
        if (dept == null) throw new KeyNotFoundException("The specified Department does not exist.");
    }
}