using ValiCraft.Attributes;
using ValiCraft.IntegrationTests.Models;

namespace ValiCraft.IntegrationTests.Validators;

/// <summary>
/// Validator for non-nullable string model.
/// This should compile without warnings thanks to the nullability fixes.
/// </summary>
[GenerateValidator]
public partial class NonNullableModelValidator : Validator<NonNullableModel>
{
    protected override void DefineRules(IValidationRuleBuilder<NonNullableModel> builder)
    {
        // These extension methods now accept IBuilderType<TRequest, string> instead of string?
        builder.Ensure(x => x.Name)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(2)
            .HasMaxLength(100);

        builder.Ensure(x => x.Email)
            .IsNotNullOrWhiteSpace()
            .IsEmailAddress();

        builder.Ensure(x => x.Age)
            .IsGreaterThan(0)
            .IsLessThan(150);
    }
}

/// <summary>
/// Validator for child model.
/// </summary>
[GenerateValidator]
public partial class ChildModelValidator : Validator<ChildModel>
{
    protected override void DefineRules(IValidationRuleBuilder<ChildModel> builder)
    {
        builder.Ensure(x => x.Description)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.Value)
            .IsGreaterThan(0);
    }
}

/// <summary>
/// Validator for parent model with nested validation.
/// </summary>
[GenerateValidator]
public partial class ParentModelValidator : Validator<ParentModel>
{
    protected override void DefineRules(IValidationRuleBuilder<ParentModel> builder)
    {
        builder.Ensure(x => x.Name)
            .IsNotNullOrWhiteSpace();

        // Validate nested non-nullable child using instance
        builder.Ensure(x => x.Child)
            .ValidateWith(new ChildModelValidator());
    }
}

/// <summary>
/// Validator for collection model.
/// </summary>
[GenerateValidator]
public partial class CollectionModelValidator : Validator<CollectionModel>
{
    protected override void DefineRules(IValidationRuleBuilder<CollectionModel> builder)
    {
        // Validate the collection has items
        builder.Ensure(x => x.Tags)
            .HasMinCount(1);

        // Validate each child model in the collection using ValidateWith
        builder.EnsureEach(x => x.Children)
            .ValidateWith(new ChildModelValidator());
    }
}

/// <summary>
/// Validator for nullable string model.
/// This tests that nullable string properties work with string extension methods.
/// </summary>
[GenerateValidator]
public partial class NullableModelValidator : Validator<NullableModel>
{
    protected override void DefineRules(IValidationRuleBuilder<NullableModel> builder)
    {
        // These extension methods should work with string? properties
        builder.Ensure(x => x.Name)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(2)
            .HasMaxLength(100);

        builder.Ensure(x => x.Email)
            .IsNotNullOrWhiteSpace()
            .IsEmailAddress();

        builder.Ensure(x => x.Age)
            .IsGreaterThan(0)
            .IsLessThan(150);
    }
}
