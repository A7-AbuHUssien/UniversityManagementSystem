using FluentValidation.Validators;
using Mapster;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.DTOs.Parameters;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Application.Services;

public class DisplayEnrollmentService : IDisplayEnrollmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DisplayEnrollmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PaginatedResultDto<EnrollmentResponseDto>> GetEnrollmentsAsync(EnrollmentFilterDto filter)
    {
        var enrollments = _unitOfWork.Repository<Enrollment>().Query(includes:[e=>e.Student,e=> e.Course]);
        if (filter.CourseId.HasValue)
            enrollments = enrollments.Where(e => e.CourseId == filter.CourseId);
        if (filter.StudentId.HasValue)
            enrollments = enrollments.Where(e => e.StudentId == filter.StudentId);
        if (filter.SemesterId.HasValue)
            enrollments = enrollments.Where(e => e.SemesterId == filter.SemesterId);
        int totalCount = enrollments.Count();

        var items = enrollments.Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToList();
        List<EnrollmentResponseDto> dtos = new(totalCount);
        foreach (var item in items)
        {
            dtos.Add(new EnrollmentResponseDto()
            {
                EnrollmentId = item.Id,
                StudentName =  item.Student.FirstName + " " + item.Student.LastName,
                CourseTitle =  item.Course.Title,
                Status =   item.Status.ToString(),
                EnrollmentDate = item.EnrollmentDate
            });
        }
        return new PaginatedResultDto<EnrollmentResponseDto>(dtos,totalCount,filter.PageNumber,filter.PageSize);
    }
}