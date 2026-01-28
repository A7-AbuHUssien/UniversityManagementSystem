using Mapster;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;

namespace UniversityManagementSystem.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICourseBusinessValidation _validator;

    public CourseService(IUnitOfWork unitOfWork, ICourseBusinessValidation validator)
    {
        _unitOfWork = unitOfWork;
        _validator = validator;
    }

    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        var courses = await _unitOfWork.Repository<Course>()
            .GetAsync(includes: [e => e.Department, e => e.Instructor]);
        var result = courses.Select(course => new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            CourseCode = course.CourseCode,
            Credits = course.Credits,
            DepartmentName = course.Department?.Name ?? "No Department",
            InstructorFullName = $"{course.Instructor.FirstName} {course.Instructor.LastName}",
            Hour = course.Hour,
            MaxCapacity =  course.MaxCapacity
        });
        return result;
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var course = await _unitOfWork.Repository<Course>()
            .GetOneAsync(e=> e.Id == id
                ,includes: [e => e.Department, e => e.Instructor]);
        var result =  new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            CourseCode = course.CourseCode,
            Credits = course.Credits,
            DepartmentName = course.Department?.Name ?? "No Department",
            InstructorFullName = $"{course.Instructor.FirstName} {course.Instructor.LastName}",
            Hour = course.Hour,
            MaxCapacity = course.MaxCapacity
        };
        return result;
    }

    public async Task<CreateCourseDto> CreateAsync(CreateCourseDto dto)
    {
        await _validator.CheckCodeUniqueAsync(dto.CourseCode);
        await _validator.CheckDepartmentExistAsync(dto.DepartmentId);

        var entity = dto.Adapt<Course>();
        await _unitOfWork.Repository<Course>().CreateAsync(entity);
        await _unitOfWork.CompleteAsync();

        return entity.Adapt<CreateCourseDto>();
    }

    public async Task<bool> UpdateAsync(int id, CreateCourseDto dto)
    {
        var inDb = await _validator.CheckCourseExistAsync(id);

        if (dto.CourseCode != inDb.CourseCode)
            await _validator.CheckCodeUniqueAsync(dto.CourseCode);

        if (dto.DepartmentId != inDb.DepartmentId)
            await _validator.CheckDepartmentExistAsync(dto.DepartmentId);

        dto.Adapt(inDb);

        _unitOfWork.Repository<Course>().Update(inDb);
        return await _unitOfWork.CompleteAsync() > 0;
    }
}