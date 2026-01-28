using FluentValidation;
using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Validators;

public class InstructorValidator : AbstractValidator<InstructorDto>
{
    public InstructorValidator()
    {
        RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
        RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress().WithMessage("Invalid instructor email format.");

        RuleFor(x => x.Specialization)
            .NotEmpty().WithMessage("Specialization is required for instructors.");

        RuleFor(x => x.DepartmentId)
            .GreaterThan(0).WithMessage("Instructor must be assigned to a valid department.");
    }
}