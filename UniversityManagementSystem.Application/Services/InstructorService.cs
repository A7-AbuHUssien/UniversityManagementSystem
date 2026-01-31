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

    public InstructorService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
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
        if (!await _unitOfWork.Repository<Department>().AnyAsync(e => e.Id == departmentId))
            throw new ArgumentException($"Department with id {departmentId} not found");
        var instructorsInDb = await _unitOfWork.Repository<Instructor>()
            .GetAsync(e => e.DepartmentId == departmentId, includes: [e => e.Department]);
        return instructorsInDb.Adapt<IEnumerable<InstructorDto>>();
    }

    public async Task<InstructorDto> CreateInstructorAsync(InstructorDto dto)
    {
        if (!(await _unitOfWork.Repository<Department>().AnyAsync(e => e.Id == dto.DepartmentId)))
            throw new InvalidOperationException("No department with this id");
        if (await _unitOfWork.Repository<Instructor>().AnyAsync(s => s.PersonalEmail == dto.Email))
            throw new InvalidOperationException("Unavailable PersonalEmail");
        var entity = dto.Adapt<Instructor>();
        await _unitOfWork.Repository<Instructor>().CreateAsync(entity);
        await _unitOfWork.CompleteAsync();
        return entity.Adapt<InstructorDto>();
    }

    public async Task<bool> UpdateInstructorAsync(int id, InstructorDto dto)
    {
        var inDb = await _unitOfWork.Repository<Instructor>().GetOneAsync(e => e.Id == id);
        if (inDb is null) throw new Exception("Instructor with this id not found");
        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != inDb.PersonalEmail)
        {
            if (await _unitOfWork.Repository<Instructor>().AnyAsync(s => s.PersonalEmail == dto.Email))
                throw new InvalidOperationException("Unavailable PersonalEmail");
            inDb.PersonalEmail = dto.Email;
        }

        if (dto.DepartmentId != 0 && dto.DepartmentId != inDb.DepartmentId)
        {
            if (dto.DepartmentName is null) throw new Exception("No department with this id");
            inDb.DepartmentId = dto.DepartmentId;
        }

        if (!String.IsNullOrWhiteSpace(dto.FirstName) && !String.IsNullOrWhiteSpace(dto.LastName))
        {
            inDb.FirstName = dto.FirstName;
            inDb.LastName = dto.LastName;
        }

        if (!String.IsNullOrWhiteSpace(dto.Specialization))
            inDb.Specialization = dto.Specialization;

        _unitOfWork.Repository<Instructor>().Update(inDb);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}