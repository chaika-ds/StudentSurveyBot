using FluentValidation;
using StudentSurveyBot.Core.Entities;

namespace StudentSurveyBot.Core.Validators;

public sealed class FullNameValidator : AbstractValidator<Student>
{
    public FullNameValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty()
            .Length(2, 120)
            .Matches("^[A-Za-zА-Яа-яЁё\\s-]+$")
            .WithMessage("ФИО должно содержать только буквы, пробелы и дефис.");
    }
}
