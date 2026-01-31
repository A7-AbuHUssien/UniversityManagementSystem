using UniversityManagementSystem.Application.LogicConstraints.States;

namespace UniversityManagementSystem.Application.LogicConstraints.Interfaces;

public interface IEnrollmentBusinessValidation
{
    bool IsDropped(EnrollmentValidationState state);
    bool IsAlreadyEnrolled(EnrollmentValidationState state);
    bool HasScheduleConflict(EnrollmentValidationState state);
    bool HasReachedMaxHours(EnrollmentValidationState state);
    bool IsPrerequisiteMet(EnrollmentValidationState state);
    bool HasAlreadyCompletedCourse(EnrollmentValidationState state);

}
