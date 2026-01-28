namespace UniversityManagementSystem.Application.DTOs.Parameters;

public class StudentFilterDto : PaginationParamsDto
{
    public string? SearchTerm { get; set; } 
    public int? DepartmentId { get; set; }
}