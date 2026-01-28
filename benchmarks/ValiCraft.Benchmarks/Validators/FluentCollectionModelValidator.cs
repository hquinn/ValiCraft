using FluentValidation;
using ValiCraft.Benchmarks.Models;

namespace ValiCraft.Benchmarks.Validators;

public class FluentCollectionModelValidator : AbstractValidator<CollectionModel>
{
    public FluentCollectionModelValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MinimumLength(2)
            .MaximumLength(100);

        RuleFor(x => x.Tags)
            .Must(tags => tags.Count >= 1)
            .WithMessage("Tags must have a minimum count of 1")
            .Must(tags => tags.Count <= 10)
            .WithMessage("Tags must have a maximum count of 10");

        RuleFor(x => x.Scores)
            .Must(scores => scores.Count >= 1)
            .WithMessage("Scores must have a minimum count of 1")
            .Must(scores => scores.Count <= 100)
            .WithMessage("Scores must have a maximum count of 100");
    }
}
