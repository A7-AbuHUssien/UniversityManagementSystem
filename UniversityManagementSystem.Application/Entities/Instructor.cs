namespace UniversityManagementSystem.Application.Entities;

public class Instructor : BaseEntity
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Specialization { get; set; } = string.Empty;
    public int DepartmentId { get; set; }
    public Department Department { get; set; }
    public ICollection<Course> Courses { get; set; }
}