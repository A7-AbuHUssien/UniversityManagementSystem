using UniversityManagementSystem.Application.Entities;
using UniversityManagementSystem.Application.LogicConstraints.Interfaces;
using UniversityManagementSystem.Application.LogicConstraints.States;

namespace UniversityManagementSystem.Application.LogicConstraints;

public class EnrollmentBusinessValidation : IEnrollmentBusinessValidation
{

    public bool IsDropped(EnrollmentValidationState state)
    {
        return state.AllEnrollments.Any(e =>
            e.CourseId == state.TargetCourse.Id
            && e.Status == EnrollmentStatus.Dropped);
    }

    public bool IsAlreadyEnrolled(EnrollmentValidationState state)
    {
        return state.AllEnrollments.Any(e =>
            e.CourseId == state.TargetCourse.Id
            && e.Status == EnrollmentStatus.Active);
    }

    public bool HasScheduleConflict(EnrollmentValidationState state)
    {
        return state.AllEnrollments.Any(e =>
            e.Course.Day == state.TargetCourse.Day &&
            e.Course.Hour == state.TargetCourse.Hour&&
            e.Status == EnrollmentStatus.Active
        );
    }

    public bool HasReachedMaxHours(EnrollmentValidationState state)
    {
        int currentCredits = state.AllEnrollments.Where(e => e.Status == EnrollmentStatus.Active)
            .Sum(e => e.Course.Credits);
        int maxAllowedCredits = state.Gpa <= 2.0m ? 15 : 18;
        return currentCredits + state.TargetCourse.Credits > maxAllowedCredits;
    }

    public bool IsPrerequisiteMet(EnrollmentValidationState state)
    {
        int? preRequisiteId = state.TargetCourse.PrerequisiteId;
        if (preRequisiteId == null) return true;
        return state.AllEnrollments.Any(e => e.Course.PrerequisiteId == preRequisiteId && e.Status == EnrollmentStatus.Completed);
    }

    public bool HasAlreadyCompletedCourse(EnrollmentValidationState state)
    {
        return state.AllEnrollments.Any(e =>
            e.CourseId == state.TargetCourse.Id &&
            e.NumericScore >= 60&&
            e.Status == EnrollmentStatus.Completed
        );
    }
}