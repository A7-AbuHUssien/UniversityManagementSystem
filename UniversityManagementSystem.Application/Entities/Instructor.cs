using Microsoft.AspNetCore.Identity;

namespace UniversityManagementSystem.Application.Entities;

public class Instructor : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string PersonalEmail { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
    public ICollection<Course> Courses { get; set; }
    
    public Guid ApplicationUserId { get; set; } 
    public IdentityUser<Guid> ApplicationUser { get; set; }

}