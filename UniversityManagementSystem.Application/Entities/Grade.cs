namespace UniversityManagementSystem.Infrastructure.Entities;

public class Grade : BaseEntity
{
    public string Symbol { get; set; } // A+, B, C
    public decimal MinScore { get; set; } // 95
    public decimal MaxScore { get; set; } // 100
    public string Description { get; set; } // Excellent, Very Good
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}