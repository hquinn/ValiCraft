using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using ValiCraft.DependencyInjection.Tests.Models;
using ValiCraft.DependencyInjection.Tests.Validators;

namespace ValiCraft.DependencyInjection.Tests;

public class AddValiCraftTests
{
    [Fact]
    public void AddValiCraft_RegistersSyncValidators()
    {
        var services = new ServiceCollection();

        services.AddValiCraft();
        using var provider = services.BuildServiceProvider();

        var orderValidator = provider.GetService<IValidator<TestOrder>>();
        orderValidator.Should().NotBeNull();

        var customerValidator = provider.GetService<IValidator<TestCustomer>>();
        customerValidator.Should().NotBeNull();
    }

    [Fact]
    public void AddValiCraft_RegistersAsyncValidators()
    {
        var services = new ServiceCollection();

        services.AddValiCraft();
        using var provider = services.BuildServiceProvider();

        var asyncValidator = provider.GetService<IAsyncValidator<TestCustomer>>();
        asyncValidator.Should().NotBeNull();
    }

    [Fact]
    public void AddValiCraft_DoesNotRegisterStaticValidators()
    {
        var services = new ServiceCollection();

        services.AddValiCraft();

        services.Should().NotContain(sd =>
            sd.ImplementationType == typeof(TestItemStaticValidator));
    }

    [Fact]
    public void AddValiCraft_UsesTransientLifetimeByDefault()
    {
        var services = new ServiceCollection();

        services.AddValiCraft();

        var validatorDescriptors = services
            .Where(sd => sd.ServiceType.IsGenericType
                         && (sd.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>)
                             || sd.ServiceType.GetGenericTypeDefinition() == typeof(IAsyncValidator<>)));

        validatorDescriptors.Should().AllSatisfy(sd =>
            sd.Lifetime.Should().Be(ServiceLifetime.Transient));
    }

    [Fact]
    public void AddValiCraft_RespectsSpecifiedLifetime()
    {
        var services = new ServiceCollection();

        services.AddValiCraft(ServiceLifetime.Scoped);

        var validatorDescriptors = services
            .Where(sd => sd.ServiceType.IsGenericType
                         && (sd.ServiceType.GetGenericTypeDefinition() == typeof(IValidator<>)
                             || sd.ServiceType.GetGenericTypeDefinition() == typeof(IAsyncValidator<>)));

        validatorDescriptors.Should().AllSatisfy(sd =>
            sd.Lifetime.Should().Be(ServiceLifetime.Scoped));
    }

    [Fact]
    public void AddValiCraft_ResolvedSyncValidatorCanValidateSuccessfully()
    {
        var services = new ServiceCollection();
        services.AddValiCraft();
        using var provider = services.BuildServiceProvider();

        var validator = provider.GetRequiredService<IValidator<TestOrder>>();

        var result = validator.Validate(new TestOrder
        {
            OrderNumber = "ORD-001",
            OrderTotal = 100M,
            Customer = new TestCustomer
            {
                Name = "John",
                Email = "john@example.com"
            }
        });

        result.Should().BeNull();
    }

    [Fact]
    public void AddValiCraft_ResolvedSyncValidatorCanDetectErrors()
    {
        var services = new ServiceCollection();
        services.AddValiCraft();
        using var provider = services.BuildServiceProvider();

        var validator = provider.GetRequiredService<IValidator<TestOrder>>();

        var result = validator.Validate(new TestOrder
        {
            OrderNumber = "",
            OrderTotal = 0M
        });

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task AddValiCraft_ResolvedAsyncValidatorCanValidateSuccessfully()
    {
        var services = new ServiceCollection();
        services.AddValiCraft();
        using var provider = services.BuildServiceProvider();

        var validator = provider.GetRequiredService<IAsyncValidator<TestCustomer>>();

        var result = await validator.ValidateAsync(new TestCustomer
        {
            Name = "John",
            Email = "john@example.com"
        });

        result.Should().BeNull();
    }

    [Fact]
    public void AddValiCraft_ValidatorWithDependenciesResolvesCorrectly()
    {
        var services = new ServiceCollection();
        services.AddValiCraft();
        using var provider = services.BuildServiceProvider();

        var validator = provider.GetRequiredService<IValidator<TestOrder>>();
        validator.Should().NotBeNull();
    }

    [Fact]
    public void AddValiCraft_ReturnsSameServiceCollectionForChaining()
    {
        var services = new ServiceCollection();

        var result = services.AddValiCraft();

        result.Should().BeSameAs(services);
    }
}
