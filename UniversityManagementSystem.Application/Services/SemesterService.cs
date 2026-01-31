using Mapster;
using UniversityManagementSystem.Application.DTOs;
using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Application.Services;

public class SemesterService : ISemesterService
{
    private readonly IUnitOfWork _unitOfWork;

    public SemesterService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<SemesterDto>> GetAllSemestersAsync()
    {
        var semesters = await _unitOfWork.Repository<Semester>().GetAsync();
        return semesters.Adapt<IEnumerable<SemesterDto>>();
    }

    public async Task<SemesterDto?> GetActiveSemesterAsync()
    {
        var activeSemester = await _unitOfWork.Repository<Semester>()
            .GetOneAsync(s => s.IsActive);
        return activeSemester?.Adapt<SemesterDto>();
    }

    public async Task<SemesterDto> CreateSemesterAsync(SemesterDto semesterDto)
    {
        var semester = semesterDto.Adapt<Semester>();
        await _unitOfWork.Repository<Semester>().CreateAsync(semester);
        await _unitOfWork.CompleteAsync();
        if (semesterDto.IsActive)
            await ActivateSemesterAsync(semester.Id);
        return semester.Adapt<SemesterDto>();
    }

    public async Task<bool> ActivateSemesterAsync(int id)
    {
        var semesterRepo = _unitOfWork.Repository<Semester>();
        var targetSemester = await semesterRepo.GetOneAsync(s => s.Id == id);
        if (targetSemester == null) return false;
        var allSemesters = await semesterRepo.GetAsync();
        foreach (var s in allSemesters)
        {
            s.IsActive = false;
            s.IsRegistrationOpen = false;
        }
        targetSemester.IsActive = true;
        await _unitOfWork.CompleteAsync();
        return true;
    }
}