using ValiCraft.Attributes;
using ValiCraft.Benchmarks.Models;
using ValiCraft.Rules;

namespace ValiCraft.Benchmarks.Validators;

[GenerateValidator]
public partial class ValiCraftComplexModelValidator : Validator<ComplexModel>
{
    protected override void DefineRules(IValidationRuleBuilder<ComplexModel> builder)
    {
        builder.Ensure(x => x.FirstName)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(2)
            .HasMaxLength(50);

        builder.Ensure(x => x.LastName)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(2)
            .HasMaxLength(50);

        builder.Ensure(x => x.Email)
            .IsNotNullOrWhiteSpace()
            .IsEmailAddress();

        builder.Ensure(x => x.Age)
            .IsGreaterOrEqualThan(18)
            .IsLessOrEqualThan(120);

        builder.Ensure(x => x.Salary)
            .IsPositiveOrZero()
            .IsLessOrEqualThan(1000000m);

        builder.Ensure(x => x.PhoneNumber)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(10)
            .HasMaxLength(15);

        builder.Ensure(x => x.Address)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(5)
            .HasMaxLength(200);

        builder.Ensure(x => x.City)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(2)
            .HasMaxLength(100);

        builder.Ensure(x => x.PostalCode)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(3)
            .HasMaxLength(10);

        builder.Ensure(x => x.Country)
            .IsNotNullOrWhiteSpace()
            .HasMinLength(2)
            .HasMaxLength(100);
    }
}
