namespace UniversityManagementSystem.Infrastructure.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; }
    public string CourseCode { get; set; } // CS101
    public int Credits { get; set; } // عدد الساعات

    public int DepartmentId { get; set; }
    public Department Department { get; set; }

    public int? InstructorId { get; set; }
    public Instructor? Instructor { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }
}