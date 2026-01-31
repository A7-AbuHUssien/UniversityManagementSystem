using FluentValidation;
using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Validators;
public class EnrollmentValidator : AbstractValidator<EnrollmentRequestDto>
{
    public EnrollmentValidator()
    {
        RuleFor(x => x.StudentId)
            .GreaterThan(0).WithMessage("Valid Student ID is required.");

        RuleFor(x => x.CourseId)
            .GreaterThan(0).WithMessage("Valid Course ID is required.");
    }
}