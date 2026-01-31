using Mapster;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Parameters;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.Services;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentBusinessValidation _validator;

    public StudentService(IUnitOfWork unitOfWork, IStudentBusinessValidation validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<PaginatedResultDto<StudentDto>> GetAllStudentsAsync(StudentFilterDto filter)
    {
        var students = _unitOfWork.Repository<Student>()
            .Query(expression: e => !e.IsDeleted, includes: [e => e.Department]);
        if (filter.SearchTerm != null)
            students = students.Where(e => e.FirstName.ToLower().Contains(filter.SearchTerm) ||
                                           e.LastName.ToLower().Contains(filter.SearchTerm) ||
                                           e.PersonalEmail.ToLower().Contains(filter.SearchTerm) ||
                                           e.Phone.ToLower().Contains(filter.SearchTerm));
        if (filter.DepartmentId.HasValue)
            students = students.Where(e => e.DepartmentId == filter.DepartmentId);
        int totalCount = students.Count();

        var items = students.Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        var dtos = items.Adapt<IEnumerable<StudentDto>>();
        return new PaginatedResultDto<StudentDto>(dtos, totalCount, filter.PageNumber, filter.PageSize);
    }

    public async Task<StudentDto?> GetStudentByIdAsync(int id)
    {
        var student = await _unitOfWork.Repository<Student>()
            .GetOneAsync(expression: s => s.Id == id, includes: [e => e.Department]);
        if (student == null) throw new Exception("Student not found");
        return student.Adapt<StudentDto>();
    }

    public async Task<IEnumerable<StudentDto>> GetDeletedStudentsAsync()
    {
        var students = await _unitOfWork.Repository<Student>()
            .GetAsync(expression: e => e.IsDeleted, includes: [e => e.Department]);
        return students.Adapt<IEnumerable<StudentDto>>();
    }

    public async Task<StudentDto> CreateStudentAsync(StudentDto dto)
    {
        Department? dep = await _unitOfWork.Repository<Department>().GetOneAsync(e => e.Id == dto.DepartmentId);
        if (dep is null) throw new Exception("No department with this id");
        if (await _unitOfWork.Repository<Student>().AnyAsync(s => s.PersonalEmail == dto.PersonalEmail))
            throw new InvalidOperationException("Unavailable PersonalEmail");
        if (await _unitOfWork.Repository<Student>().AnyAsync(s => s.Phone == dto.Phone))
            throw new InvalidOperationException("Unavailable Phone Number");
        if (!await _validator.IsValidAgeAsync(dto.DateOfBirth))
            throw new Exception("Student must be at least 18 years old.");
        var studentEntity = new Student
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            PersonalEmail = dto.PersonalEmail,
            Department = dep,
            DepartmentId = dep.Id,
            DateOfBirth = dto.DateOfBirth,
            Phone = dto.Phone
        };
        dep.StudentsCount++;
        await _unitOfWork.Repository<Student>().CreateAsync(studentEntity);
        _unitOfWork.Repository<Department>().Update(dep);
        await _unitOfWork.CompleteAsync();
       
        
        dto.Id = studentEntity.Id;
        return dto;
    }

    public async Task<bool> UpdateStudentAsync(UpdateStudentDto dto)
    {
        Student? student = await _unitOfWork.Repository<Student>().GetOneAsync(e => e.Id == dto.Id);
        if (student is null) throw new Exception("Student not found");
        if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != student.PersonalEmail)
        {
            if (await _unitOfWork.Repository<Student>().AnyAsync(s => s.PersonalEmail == dto.Email))
                throw new InvalidOperationException("This PersonalEmail is Not Available");
            student.PersonalEmail = dto.Email;
        }

        if (!string.IsNullOrWhiteSpace(dto.Phone) && dto.Phone != student.Phone)
        {
            if (await _unitOfWork.Repository<Student>().AnyAsync(s => s.Phone == dto.Phone))
                throw new InvalidOperationException("This Phone Number Related to another Student");
            student.Phone = dto.Phone;
        }

        if (dto.DateOfBirth != default)
        {
            if (!await _validator.IsValidAgeAsync(dto.DateOfBirth))
                throw new Exception("Date of birth is invalid");
            student.DateOfBirth = dto.DateOfBirth;
        }

        if (dto.DepartmentId != -1 && dto.DepartmentId != student.Department.Id)
        {
            var dep = await _unitOfWork.Repository<Department>().GetOneAsync(e => e.Id == dto.DepartmentId);
            if (dep is null) throw new Exception("No Department With this Id");
            student.DepartmentId = dep.Id;
        }

        student.FirstName = string.IsNullOrWhiteSpace(dto.FirstName) ? student.FirstName : dto.FirstName;
        student.LastName = string.IsNullOrWhiteSpace(dto.LastName) ? student.LastName : dto.LastName;

        _unitOfWork.Repository<Student>().Update(student);
        await _unitOfWork.CompleteAsync();

        return true;
    }

    public async Task<bool> DeleteStudentAsync(int id)
    {
        Student? student = await _unitOfWork.Repository<Student>().GetOneAsync(e => e.Id == id);
        if (student is null) throw new Exception("Student not found");
        student.IsDeleted = true;
        _unitOfWork.Repository<Student>().Update(student);
        await _unitOfWork.CompleteAsync();
        return true;
    }
}