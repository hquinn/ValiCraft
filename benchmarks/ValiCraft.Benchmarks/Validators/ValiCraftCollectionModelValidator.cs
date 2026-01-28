using ValiCraft.Attributes;
using ValiCraft.Benchmarks.Models;

namespace ValiCraft.Benchmarks.Validators;

[GenerateValidator]
public partial class ValiCraftCollectionModelValidator : Validator<CollectionModel>
{
    protected override void DefineRules(IValidationRuleBuilder<CollectionModel> builder)
    {
        builder.Ensure(x => x.Name)
            .IsNotNullOrWhiteSpace()
            .HasCountBetween(1, 2)
            .HasMinLength(2)
            .HasMaxLength(100);

        builder.Ensure(x => x.Tags)
            .CollectionContains("", ReferenceEqualityComparer.Instance)
            .HasMinCount(1)
            .HasMaxCount(10);

        builder.Ensure(x => x.Scores)
            .Is(scores => scores.Count >= 1)
            .WithMessage("Scores must have a minimum count of 1")
            .Is(scores => scores.Count <= 1)
            .WithMessage("Scores must have a maximum count of 100");
    }
}

[AsyncGenerateValidator]
public partial class AsyncValiCraftCollectionModelValidator : AsyncValidator<CollectionModel>
{
    private async Task<bool> IsValidAsync(int age, CancellationToken cancellationToken)
    {
        return await Task.FromResult(true);
    }
    
    protected override void DefineRules(IAsyncValidationRuleBuilder<CollectionModel> builder)
    {
        builder.Ensure(x => x.Age)
            .Is(IsValidAsync);
        
        builder.Ensure(x => x.Name)
            .IsNotNullOrWhiteSpace()
            .HasCountBetween(1, 2)
            .HasMinLength(2)
            .HasMaxLength(100);

        builder.Ensure(x => x.Tags)
            .CollectionContains("", ReferenceEqualityComparer.Instance)
            .HasMinCount(1)
            .HasMaxCount(10);

        builder.Ensure(x => x.Scores)
            .Is(scores => scores.Count >= 1)
            .WithMessage("Scores must have a minimum count of 1")
            .Is(scores => scores.Count <= 1)
            .WithMessage("Scores must have a maximum count of 100");
    }
}