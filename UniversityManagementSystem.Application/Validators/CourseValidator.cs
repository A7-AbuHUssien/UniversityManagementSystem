using FluentValidation;
using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Validators;

public class CourseValidator : AbstractValidator<CreateCourseDto>
{
    public CourseValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Course title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Course code is required.")
            .Matches(@"^[A-Z]{2,4}\d{3}$")
            .WithMessage("Course code must be like 'CS101' (2-4 letters followed by 3 digits).");

        RuleFor(x => x.Credits)
            .InclusiveBetween(1, 3).WithMessage("Credits must be between 1 and 3.");

        RuleFor(x => x.MaxCapacity)
            .GreaterThan(9).WithMessage("Capacity must be at least 10 student.");
            
        RuleFor(x => x.Hour)
            .InclusiveBetween(8, 20).WithMessage("Lecture hour must be between 8 AM and 8 PM.");
    }
}