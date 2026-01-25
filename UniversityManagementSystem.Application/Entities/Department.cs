namespace UniversityManagementSystem.Infrastructure.Entities;

public class Department : BaseEntity
{
    public string Name { get; set; } 
    public string Code { get; set; }
    
    public ICollection<Student> Students { get; set; }
    public ICollection<Instructor> Instructors { get; set; }
    public ICollection<Course> Courses { get; set; }
}