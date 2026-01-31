namespace UniversityManagementSystem.Application.DTOs.Identity_DTOs;

public class UpdateProfileDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string PersonalEmail { get; init; } = string.Empty;
    public string Phone { get; init; } = string.Empty;
}