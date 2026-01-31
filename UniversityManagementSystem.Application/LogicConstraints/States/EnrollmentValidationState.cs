using UniversityManagementSystem.Application.Entities;

namespace UniversityManagementSystem.Application.LogicConstraints.States;

public sealed class EnrollmentValidationState
{
    public int StudentId { get; }
    public int SemesterId { get; }

    public IReadOnlyList<Enrollment> AllEnrollments { get; }
    public Course TargetCourse { get; }

    public decimal Gpa { get; }

    public EnrollmentValidationState(
        int studentId,
        int semesterId,
        IReadOnlyList<Enrollment> allEnrollments,
        Course targetCourse,
        decimal gpa)
    {
        StudentId = studentId;
        SemesterId = semesterId;
        TargetCourse = targetCourse;
        Gpa = gpa;
        AllEnrollments = allEnrollments;
    }
}
