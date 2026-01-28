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
        
        // Logic: If this is the first semester, or forced active, handle it.
        await _unitOfWork.Repository<Semester>().CreateAsync(semester);
        await _unitOfWork.CompleteAsync();
        
        return semester.Adapt<SemesterDto>();
    }

    public async Task<bool> ActivateSemesterAsync(int id)
    {
        var semesterRepo = _unitOfWork.Repository<Semester>();
        
        // 1. Find the semester to activate
        var targetSemester = await semesterRepo.GetOneAsync(s => s.Id == id);
        if (targetSemester == null) return false;

        // 2. Deactivate all other semesters
        var allSemesters = await semesterRepo.GetAsync();
        foreach (var s in allSemesters)
        {
            s.IsActive = false;
        }

        // 3. Activate the target one
        targetSemester.IsActive = true;
        
        await _unitOfWork.CompleteAsync();
        return true;
    }
}