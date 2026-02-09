using ValiCraft.Attributes;
using ValiCraft.Benchmarks.Models;

namespace ValiCraft.Benchmarks.Validators;

[GenerateValidator]
public partial class ValiCraftSimpleModelValidator : Validator<SimpleModel>
{
    protected override void DefineRules(IValidationRuleBuilder<SimpleModel> builder)
    {
        builder.Ensure(x => x.Name)
            .IsNotDefault()
            .IsNotNullOrWhiteSpace()
            .HasMinLength(2)
            .HasMaxLength(100);

        builder.Ensure(x => x.Age)
            .IsGreaterThan(0)
            .IsLessThan(150);

        builder.Ensure(x => x.Email)
            .IsNotNullOrWhiteSpace()
            .IsEmailAddress();
    }
}
