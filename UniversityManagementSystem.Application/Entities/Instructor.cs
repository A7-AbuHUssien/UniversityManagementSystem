namespace UniversityManagementSystem.Infrastructure.Entities;

public class Instructor : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }

    public int DepartmentId { get; set; }
    public Department Department { get; set; }

    public ICollection<Course> Courses { get; set; }
}