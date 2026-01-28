namespace UniversityManagementSystem.Application.DTOs;

public class TranscriptDto
{
    public string CourseTitle { get; set; } = string.Empty;
    public string SemesterName { get; set; } = string.Empty;
    public int Credits { get; set; }
    public decimal NumericScore { get; set; }
    public string GradeSymbol { get; set; } = "N/A";
    public decimal GradePoints { get; set; } // Points for this specific course (e.g., 4.0)
}