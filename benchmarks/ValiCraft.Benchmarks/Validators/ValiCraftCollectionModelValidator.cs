using ValiCraft.Attributes;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Rules;

namespace ValiCraft.Benchmarks.Validators;

[GenerateValidator]
public partial class ValiCraftCollectionModelValidator : Validator<CollectionModel>
{
    protected override void DefineRules(IValidationRuleBuilder<CollectionModel> builder)
    {
        builder.Ensure(x => x.Name)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(2)
            .HasMaxLength(100);

        builder.Ensure(x => x.Tags)
            .HasMinCount(1)
            .HasMaxCount(10);

        builder.Ensure(x => x.Scores)
            .Must(scores => scores != null && scores.Count >= 1)
            .WithMessage("Scores must have a minimum count of 1")
            .Must(scores => scores != null && scores.Count <= 1)
            .WithMessage("Scores must have a maximum count of 100");
    }
}
