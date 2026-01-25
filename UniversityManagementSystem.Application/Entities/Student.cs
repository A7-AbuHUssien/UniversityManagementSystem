namespace UniversityManagementSystem.Infrastructure.Entities;

public class Student : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateTime DateOfBirth { get; set; }

    // الربط مع القسم
    public int DepartmentId { get; set; }
    public Department Department { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }
}