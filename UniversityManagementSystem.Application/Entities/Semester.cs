namespace UniversityManagementSystem.Application.Entities;

public class Semester : BaseEntity
{
    public string Name { get; set; } 
    public int Year { get; set; }
    public bool IsActive { get; set; }
    public bool IsRegistrationOpen{get;set;}
    public DateOnly StartDate { get; set; }
    public DateOnly EndDate { get; set; }

    public ICollection<Enrollment> Enrollments { get; set; }
}