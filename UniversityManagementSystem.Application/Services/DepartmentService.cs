using Mapster;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Application.Services;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
        if (String.IsNullOrWhiteSpace(dto.Code) || String.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Code and Name are required");
        if (await _unitOfWork.Repository<Department>().AnyAsync(d => d.Code == dto.Code))
            throw new ArgumentException("Code is already exists");
        if (await _unitOfWork.Repository<Department>().AnyAsync(d => d.Name == dto.Name))
            throw new ArgumentException("Name is already exists");
        Department entity = dto.Adapt<Department>();
        await _unitOfWork.Repository<Department>().CreateAsync(entity);
        await _unitOfWork.CompleteAsync();

        return entity.Adapt<DepartmentDto>();
    }

    public async Task<bool> UpdateDepartmentAsync(int id, DepartmentDto dto)
    {
        Department? inDb = await _unitOfWork.Repository<Department>().GetOneAsync(d => d.Id == id);
        if (inDb == null) throw  new ArgumentException("Department not exist");
        if (!string.IsNullOrWhiteSpace(dto.Name) && dto.Name != inDb.Name)
        {
            if (await _unitOfWork.Repository<Department>().AnyAsync(d => d.Name == dto.Name))
                throw new ArgumentException("Name is already exists");

            inDb.Name = dto.Name;
        }

        if (!string.IsNullOrWhiteSpace(dto.Code) && dto.Code != inDb.Code)
        {
            if (await _unitOfWork.Repository<Department>().AnyAsync(d => d.Code == dto.Code))
                throw new ArgumentException("Code is already exists");
            inDb.Code = dto.Code;
        }

        inDb.StudentsCount = dto.StudentsCount;
        _unitOfWork.Repository<Department>().Update(inDb);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}