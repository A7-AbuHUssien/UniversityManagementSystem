namespace UniversityManagementSystem.Application.Interfaces.Services;

public interface IRegistrationControlService
{
    Task OpenRegistrationAsync();
    Task CloseRegistrationAsync();
}