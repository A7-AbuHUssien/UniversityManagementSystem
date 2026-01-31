namespace UniversityManagementSystem.Application.DTOs;

public class UserManagementDto
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FullName { get; init; } = string.Empty;
    public List<string> Roles { get; init; } = new();
    public bool IsActive { get; init; } 
    public DateTime JoinedAt { get; init; }
    public string UserType { get; init; } = string.Empty;
}