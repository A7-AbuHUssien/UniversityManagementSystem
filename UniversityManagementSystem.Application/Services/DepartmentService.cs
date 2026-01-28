using Mapster;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDepartmentBusinessValidation _validator;

    public DepartmentService(IUnitOfWork unitOfWork, IDepartmentBusinessValidation validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
    {
        var deps = await _unitOfWork.Repository<Department>().GetAsync();
       return deps.Adapt<IEnumerable<DepartmentDto>>();
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(int id)
    {
        Department? dep = await _unitOfWork.Repository<Department>().GetOneAsync(d => d.Id == id);
        return dep?.Adapt<DepartmentDto>();
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(DepartmentDto dto)
    {
        if(String.IsNullOrWhiteSpace(dto.Code) || String.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Code and Name are required");
        await _validator.CheckCodeUniqueAsync(dto.Code);
        await _validator.CheckNameUniqueAsync(dto.Name);
        Department entity = dto.Adapt<Department>();
        await _unitOfWork.Repository<Department>().CreateAsync(entity);
        await _unitOfWork.CompleteAsync();
       
        return entity.Adapt<DepartmentDto>();
    }

    public async Task<bool> UpdateDepartmentAsync(int id, DepartmentDto dto)
    {
        var inDb = await _validator.CheckDepartmentExistAsync(id);
        if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != inDb.Name)
        {
            await _validator.CheckNameUniqueAsync(dto.Name);
            inDb.Name = dto.Name;
        }
        if (!string.IsNullOrWhiteSpace(dto.Code)  && dto.Code != inDb.Code)
        {      
            await _validator.CheckCodeUniqueAsync(dto.Code);
            inDb.Code = dto.Code;
        }
        inDb.StudentsCount = dto.StudentsCount;
        _unitOfWork.Repository<Department>().Update(inDb);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}