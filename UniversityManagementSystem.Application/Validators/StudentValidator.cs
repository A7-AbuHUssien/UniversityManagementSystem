using FluentValidation;
using UniversityManagementSystem.Application.DTOs;

namespace UniversityManagementSystem.Application.Validators;

public class StudentValidator : AbstractValidator<StudentDto>
{
    public StudentValidator()
    {
        RuleFor(s => s.FirstName)
            .NotEmpty().WithMessage("First Name is required.")
            .MinimumLength(3).WithMessage("First Name must be at least 3 characters.")
            .MaximumLength(50).WithMessage("First Name cannot exceed 50 characters.");

        RuleFor(s => s.LastName)
            .NotEmpty().WithMessage("Last Name is required.");

        RuleFor(s => s.PersonalEmail)
            .NotEmpty().WithMessage("PersonalEmail address is required.")
            .EmailAddress().WithMessage("A valid email format is required.");

        RuleFor(s => s.Phone)
            .NotEmpty().WithMessage("Phone number is required.")
            .Matches(@"^\d{10,15}$").WithMessage("Phone number must be numeric and between 10 to 15 digits.");

        RuleFor(s => s.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required.")
            .Must(date => date < DateOnly.FromDateTime(DateTime.Now.AddYears(-16)))
            .WithMessage("The student must be at least 16 years old.");
    }
}