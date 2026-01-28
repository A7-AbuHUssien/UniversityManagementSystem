namespace UniversityManagementSystem.Application.Entities;

public class Grade : BaseEntity
{
    public string Symbol { get; set; } // A+, B, C
    public decimal MinScore { get; set; } // 95
    public decimal MaxScore { get; set; } // 100
    public string Description { get; set; } // Excellent, Very Good
    public decimal GPAPoint { get; set; } // 4.0, 3.7, 3.3 (ضروري للحسابات)
    public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}