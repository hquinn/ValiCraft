using ValiCraft.Attributes;
using ValiCraft.DependencyInjection.Tests.Models;

namespace ValiCraft.DependencyInjection.Tests.Validators;

[GenerateValidator]
public partial class TestOrderValidator : Validator<TestOrder>
{
    protected override void DefineRules(IValidationRuleBuilder<TestOrder> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.OrderTotal)
            .IsGreaterThan(0M);
    }
}

[GenerateValidator]
public partial class TestCustomerValidator : Validator<TestCustomer>
{
    protected override void DefineRules(IValidationRuleBuilder<TestCustomer> builder)
    {
        builder.Ensure(x => x.Name)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.Email)
            .IsNotNullOrWhiteSpace();
    }
}

[GenerateValidator]
public partial class TestCustomerAsyncValidator : AsyncValidator<TestCustomer>
{
    protected override void DefineRules(IAsyncValidationRuleBuilder<TestCustomer> builder)
    {
        builder.Ensure(x => x.Name)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.Email)
            .IsNotNullOrWhiteSpace();
    }
}

[GenerateValidator]
public partial class TestItemStaticValidator : StaticValidator<TestItem>
{
    protected override void DefineRules(IValidationRuleBuilder<TestItem> builder)
    {
        builder.Ensure(x => x.ItemName)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.Quantity)
            .IsGreaterThan(0);
    }
}

[GenerateValidator]
public partial class TestOrderWithDependencyValidator(IValidator<TestCustomer> customerValidator) : Validator<TestOrder>
{
    protected override void DefineRules(IValidationRuleBuilder<TestOrder> builder)
    {
        builder.Ensure(x => x.OrderNumber)
            .IsNotNullOrWhiteSpace();

        builder.Ensure(x => x.Customer)
            .ValidateWith(customerValidator);
    }
}
