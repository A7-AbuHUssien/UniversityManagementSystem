using System.Data;
using Mapster;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Application.Services;

public class CourseService : ICourseService
{
    private readonly IUnitOfWork _unitOfWork;

    public CourseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CourseDto>> GetAllAsync()
    {
        var courses = await _unitOfWork.Repository<Course>()
            .GetAsync(includes: [e => e.Department, e => e.Instructor]);
        var result = courses.Select(course => new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Credits = course.Credits,
            DepartmentName = course.Department.Name,
            InstructorFullName = $"{course.Instructor.FirstName} {course.Instructor.LastName}",
            AvailableSeats = course.MaxCapacity - course.CurrentCapacity,
            IsActive = course.IsActive,
        });
        return result;
    }

    public async Task<CourseDto?> GetByIdAsync(int id)
    {
        var course = await _unitOfWork.Repository<Course>()
            .GetOneAsync(e => e.Id == id
                , includes: [e => e.Department, e => e.Instructor]);
        if (course == null) throw new KeyNotFoundException("Course not found");
        var result = new CourseDto
        {
            Id = course.Id,
            Title = course.Title,
            Credits = course.Credits,
            DepartmentName = course.Department.Name,
            InstructorFullName = $"{course.Instructor.FirstName} {course.Instructor.LastName}",
            AvailableSeats = course.MaxCapacity - course.CurrentCapacity,
            IsActive = course.IsActive,
        };
        return result;
    }

    public async Task<CreateCourseDto> CreateAsync(CreateCourseDto dto)
    {
        if (await _unitOfWork.Repository<Course>().AnyAsync(e => e.CourseCode == dto.CourseCode))
            throw new InvalidExpressionException("Course code already exists");
        if (!await _unitOfWork.Repository<Department>().AnyAsync(e => e.Id == dto.DepartmentId))
            throw new InvalidExpressionException("Department Not exists");

        var entity = dto.Adapt<Course>();
        entity.IsActive = true;
        entity.CurrentCapacity = 0;
        await _unitOfWork.Repository<Course>().CreateAsync(entity);
        await _unitOfWork.CompleteAsync();

        return entity.Adapt<CreateCourseDto>();
    }

    public async Task<bool> UpdateAsync(int id, CreateCourseDto dto)
    {
        var inDb = await _unitOfWork.Repository<Course>().GetOneAsync(e => e.Id == id);
        if (inDb == null) throw new KeyNotFoundException("Course not found");
        if (dto.CourseCode != inDb.CourseCode)
        {
            if (await _unitOfWork.Repository<Course>().AnyAsync(e => e.CourseCode == dto.CourseCode))
                throw new InvalidOperationException($"Course code '{dto.CourseCode}' is already in use.");
        }
        if (dto.DepartmentId != inDb.DepartmentId)
        {
            if (!await _unitOfWork.Repository<Department>().AnyAsync(e => e.Id == dto.DepartmentId))
                throw new InvalidOperationException($"Department '{dto.DepartmentId}' Not Exist.");
        }
        dto.Adapt(inDb);
        
        _unitOfWork.Repository<Course>().Update(inDb);
        return await _unitOfWork.CompleteAsync() > 0;
    }

    public async Task<string> ActivationAsync(int courseId)
    {
        Course? course = await _unitOfWork.Repository<Course>().GetOneAsync(e => e.Id == courseId);
        if (course == null) throw new InvalidExpressionException("Course not found");
        if (course.IsActive)
            course.IsActive = false;
        else
            course.IsActive = true;
        _unitOfWork.Repository<Course>().Update(course);
        await _unitOfWork.CompleteAsync();
        return course.IsActive ? "Course Activated Successfully" : "Course UnActivated Successfully";
    }
}