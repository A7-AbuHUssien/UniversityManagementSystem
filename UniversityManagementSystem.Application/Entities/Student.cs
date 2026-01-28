namespace UniversityManagementSystem.Application.Entities;

public class Student : BaseEntity
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public DateOnly DateOfBirth { get; set; }

    public int DepartmentId { get; set; }
    public Department Department { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedAt { get; set; }
}