using FluentValidation;
using ValiCraft.Benchmarks.Models;

namespace ValiCraft.Benchmarks.Validators;

public class FluentComplexModelValidator : AbstractValidator<ComplexModel>
{
    public FluentComplexModelValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);

        RuleFor(x => x.LastName)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(50);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();

        RuleFor(x => x.Age)
            .GreaterThanOrEqualTo(18)
            .LessThanOrEqualTo(120);

        RuleFor(x => x.Salary)
            .GreaterThanOrEqualTo(0)
            .LessThanOrEqualTo(1000000m);

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .MinimumLength(10)
            .MaximumLength(15);

        RuleFor(x => x.Address)
            .NotEmpty()
            .MinimumLength(5)
            .MaximumLength(200);

        RuleFor(x => x.City)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(x => x.PostalCode)
            .NotEmpty()
            .MinimumLength(3)
            .MaximumLength(10);

        RuleFor(x => x.Country)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);
    }
}
