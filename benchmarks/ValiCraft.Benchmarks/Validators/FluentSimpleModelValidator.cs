using FluentValidation;
using ValiCraft.Benchmarks.Models;

namespace ValiCraft.Benchmarks.Validators;

public class FluentSimpleModelValidator : AbstractValidator<SimpleModel>
{
    public FluentSimpleModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(x => x.Age)
            .GreaterThan(0)
            .LessThan(150);

        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
    }
}
