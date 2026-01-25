namespace UniversityManagementSystem.Infrastructure.Entities;

public class Enrollment : BaseEntity
{
    public int StudentId { get; set; }
    public Student Student { get; set; }

    public int CourseId { get; set; }
    public Course Course { get; set; }

    public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
    public EnrollmentStatus Status { get; set; } = EnrollmentStatus.Active;

    public decimal? NumericScore { get; set; } 
    
    public int? GradeId { get; set; }
    public Grade Grade { get; set; }
}