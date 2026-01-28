using FluentValidation;
using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Validators;

public class CourseValidator : AbstractValidator<CreateCourseDto>
{
    public CourseValidator()
    {
        // Title: Not empty and reasonable length
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Course title is required.")
            .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

        // CourseCode: Example pattern (CS101, ENG202)
        RuleFor(x => x.CourseCode)
            .NotEmpty().WithMessage("Course code is required.")
            .Matches(@"^[A-Z]{2,4}\d{3}$")
            .WithMessage("Course code must be like 'CS101' (2-4 letters followed by 3 digits).");

        // Credits: Usually between 1 and 5
        RuleFor(x => x.Credits)
            .InclusiveBetween(1, 3).WithMessage("Credits must be between 1 and 3.");

        // Capacity: Must be a positive number
        RuleFor(x => x.MaxCapacity)
            .GreaterThan(9).WithMessage("Capacity must be at least 10 student.");
            
        // Schedule: Hour must be in 24-hour format (e.g., 8 to 20)
        RuleFor(x => x.Hour)
            .InclusiveBetween(8, 20).WithMessage("Lecture hour must be between 8 AM and 8 PM.");
    }
}