using FluentValidation;
using UniversityManagementSystem.Application.Entities; 

namespace UniversityManagementSystem.Application.Validators;

public class SemesterValidator : AbstractValidator<Semester>
{
    public SemesterValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Semester name is required (e.g., Fall, Spring).");

        RuleFor(x => x.Year)
            .InclusiveBetween(2025, 2100).WithMessage("Year must be between 2025 and 2100.");

        RuleFor(x => x.EndDate)
            .GreaterThan(x => x.StartDate)
            .WithMessage("Semester End Date must be after the Start Date.");
    }
}