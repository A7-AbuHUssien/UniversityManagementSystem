using Mapster;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.Services;

public class InstructorService : IInstructorService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IInstructorBusinessValidation _validator;

    public InstructorService(IUnitOfWork unitOfWork, IInstructorBusinessValidation validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<InstructorDto?> GetInstructorByIdAsync(int id)
    {
        var instructor = await _unitOfWork.Repository<Instructor>()
            .GetOneAsync(e => e.Id == id, includes: [e => e.Department]);
        return instructor?.Adapt<InstructorDto>();
    }

    public async Task<IEnumerable<InstructorDto>> GetAllInstructorsAsync()
    {
        var instructors = await _unitOfWork.Repository<Instructor>().GetAsync(includes: [e => e.Department]);
        return instructors.Adapt<IEnumerable<InstructorDto>>();
    }

    public async Task<IEnumerable<InstructorDto>> GetInstructorsByDepartmentAsync(int departmentId)
    {
        await _validator.CheckDepartmentExistAsync(departmentId);
        var instractorsInDb =  await _unitOfWork.Repository<Instructor>()
            .GetAsync(e => e.DepartmentId == departmentId, includes: [e => e.Department]);
        return instractorsInDb.Adapt<IEnumerable<InstructorDto>>();
    }

    public async Task<InstructorDto> CreateInstructorAsync(InstructorDto dto)
    {
        await _validator.CheckEmailUniqueAsync(dto.Email);
        await _validator.CheckDepartmentExistAsync(dto.DepartmentId);

        var entity = dto.Adapt<Instructor>();

        await _unitOfWork.Repository<Instructor>().CreateAsync(entity);
        await _unitOfWork.CompleteAsync();

        return entity.Adapt<InstructorDto>();
    }

    public async Task<bool> UpdateInstructorAsync(int id, InstructorDto dto)
    {
        var inDb = await _validator.CheckInstructorExistAsync(id);

        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != inDb.Email)
        {
            await _validator.CheckEmailUniqueAsync(dto.Email);
            inDb.Email = dto.Email;
        }

        if (dto.DepartmentId != 0 && dto.DepartmentId != inDb.DepartmentId)
        {
            await _validator.CheckDepartmentExistAsync(dto.DepartmentId);
            inDb.DepartmentId = dto.DepartmentId;
        }

        if (!String.IsNullOrWhiteSpace(dto.FirstName) && !String.IsNullOrWhiteSpace(dto.LastName))
        {
            inDb.FirstName = dto.FirstName;
            inDb.LastName = dto.LastName;
        }
        if(String.IsNullOrWhiteSpace(dto.Specialization))
            inDb.Specialization = dto.Specialization;

        _unitOfWork.Repository<Instructor>().Update(inDb);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}