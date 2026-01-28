using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.LogicConstraints;

public class DepartmentBusinessValidation : IDepartmentBusinessValidation
{
    private readonly IUnitOfWork _unitOfWork;
    public DepartmentBusinessValidation(IUnitOfWork unitOfWork) => _unitOfWork = unitOfWork;

    public async Task CheckNameUniqueAsync(string name)
    {
        var exists = await _unitOfWork.Repository<Department>().GetOneAsync(d => d.Name == name);
        if (exists != null) throw new InvalidOperationException("Department name already exists.");
    }
    public async Task CheckCodeUniqueAsync(string code)
    {
        var exists = await _unitOfWork.Repository<Department>().GetOneAsync(d => d.Code == code);
        if (exists != null) throw new InvalidOperationException("Department Code already exists.");
    }
    public async Task<Department> CheckDepartmentExistAsync(int id)
    {
        var dep = await _unitOfWork.Repository<Department>().GetOneAsync(d => d.Id == id);
        if (dep == null) throw new KeyNotFoundException("Department not found.");
        return dep;
    }
}