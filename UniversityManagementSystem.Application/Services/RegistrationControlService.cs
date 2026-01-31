using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.Interfaces;
using UniversityManagementSystem.Application.Interfaces.Services;

namespace UniversityManagementSystem.Application.Services;

public class RegistrationControlService : IRegistrationControlService
{
    private readonly IUnitOfWork _unitOfWork;
    public RegistrationControlService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task OpenRegistrationAsync()
    {
        Semester? semester = await _unitOfWork.Repository<Semester>().GetOneAsync(e => e.IsActive);
        if (semester == null) throw new Exception("No Active Semester");
        if(semester.IsRegistrationOpen) return;
        semester.IsRegistrationOpen = true;
        _unitOfWork.Repository<Semester>().Update(semester);
        await _unitOfWork.CompleteAsync();
    }

    public async Task CloseRegistrationAsync()
    {
        Semester? semester = await _unitOfWork.Repository<Semester>().GetOneAsync(e => e.IsActive);
        if (semester == null) throw new Exception("No Active Semester");
        if(!semester.IsRegistrationOpen) return;
        semester.IsRegistrationOpen = false;
        _unitOfWork.Repository<Semester>().Update(semester);
        await _unitOfWork.CompleteAsync(); 
    }
}