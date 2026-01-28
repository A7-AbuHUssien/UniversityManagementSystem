namespace UniversityManagementSystem.Application.LogicConstraints.Interfaces;

public interface IEnrollmentBusinessValidation
{
    Task<bool> IsAlreadyEnrolledAsync(int studentId, int courseId, int semesterId);
    Task<bool> IsPrerequisiteMetAsync(int studentId, int courseId);
    Task<bool> HasScheduleConflictAsync(int studentId, int courseId, int semesterId);
    Task<bool> IsSemesterValidForEnrollmentAsync(int semesterId);
    Task<bool> IsHaveMaxHours(int studentId);
    Task<bool> IsDropped(int studentId, int courseId, int semesterId);
    Task<bool> IsSuccessed(int studentId, int courseId);
}