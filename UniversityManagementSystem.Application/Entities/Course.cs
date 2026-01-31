namespace UniversityManagementSystem.Application.Entities;

public class Course : BaseEntity
{
    public string Title { get; set; }
    public string CourseCode { get; set; }
    public int Credits { get; set; }
    public int MaxCapacity { get; set; } = 30;
    public int CurrentCapacity { get; set; } = 0;
    public int DepartmentId { get; set; }
    public int InstructorId { get; set; }
    public Instructor Instructor { get; set; }
    public Department Department { get; set; }
    public DayOfWeek Day { get; set; }
    public int Hour { get; set; }    
    public ICollection<Enrollment> Enrollments { get; set; }
    public int? PrerequisiteId { get; set; }
    public Course? Prerequisite { get; set; }
    public bool IsActive { get; set; }
}