using AwesomeAssertions;
using Microsoft.Extensions.DependencyInjection;
using ValiCraft.DependencyInjection.Tests.Models;
using ValiCraft.DependencyInjection.Tests.Validators;

namespace ValiCraft.DependencyInjection.Tests;

public class ServiceCollectionExtensionsTests
{
    [Fact]
    public void AddValidator_RegistersWithDefaultTransientLifetime()
    {
        var services = new ServiceCollection();

        services.AddValidator<TestOrderValidator, TestOrder>();

        var descriptor = services.Should().ContainSingle().Subject;
        descriptor.ServiceType.Should().Be(typeof(IValidator<TestOrder>));
        descriptor.ImplementationType.Should().Be(typeof(TestOrderValidator));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddValidator_RegistersWithSpecifiedScopedLifetime()
    {
        var services = new ServiceCollection();

        services.AddValidator<TestOrderValidator, TestOrder>(ServiceLifetime.Scoped);

        var descriptor = services.Should().ContainSingle().Subject;
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddValidator_RegistersWithSpecifiedSingletonLifetime()
    {
        var services = new ServiceCollection();

        services.AddValidator<TestOrderValidator, TestOrder>(ServiceLifetime.Singleton);

        var descriptor = services.Should().ContainSingle().Subject;
        descriptor.Lifetime.Should().Be(ServiceLifetime.Singleton);
    }

    [Fact]
    public void AddAsyncValidator_RegistersWithDefaultTransientLifetime()
    {
        var services = new ServiceCollection();

        services.AddAsyncValidator<TestCustomerAsyncValidator, TestCustomer>();

        var descriptor = services.Should().ContainSingle().Subject;
        descriptor.ServiceType.Should().Be(typeof(IAsyncValidator<TestCustomer>));
        descriptor.ImplementationType.Should().Be(typeof(TestCustomerAsyncValidator));
        descriptor.Lifetime.Should().Be(ServiceLifetime.Transient);
    }

    [Fact]
    public void AddAsyncValidator_RegistersWithSpecifiedLifetime()
    {
        var services = new ServiceCollection();

        services.AddAsyncValidator<TestCustomerAsyncValidator, TestCustomer>(ServiceLifetime.Scoped);

        var descriptor = services.Should().ContainSingle().Subject;
        descriptor.Lifetime.Should().Be(ServiceLifetime.Scoped);
    }

    [Fact]
    public void AddValidator_ResolvedValidatorCanValidate()
    {
        var services = new ServiceCollection();
        services.AddValidator<TestOrderValidator, TestOrder>();
        using var provider = services.BuildServiceProvider();

        var validator = provider.GetRequiredService<IValidator<TestOrder>>();

        validator.Should().BeOfType<TestOrderValidator>();
        var result = validator.Validate(new TestOrder { OrderNumber = "ORD-001", OrderTotal = 100M });
        result.IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void AddAsyncValidator_ResolvedValidatorCanValidate()
    {
        var services = new ServiceCollection();
        services.AddAsyncValidator<TestCustomerAsyncValidator, TestCustomer>();
        using var provider = services.BuildServiceProvider();

        var validator = provider.GetRequiredService<IAsyncValidator<TestCustomer>>();

        validator.Should().BeOfType<TestCustomerAsyncValidator>();
    }

    [Fact]
    public void AddValidator_MultipleValidatorsCanBeRegistered()
    {
        var services = new ServiceCollection();

        services.AddValidator<TestOrderValidator, TestOrder>();
        services.AddValidator<TestCustomerValidator, TestCustomer>();

        services.Should().HaveCount(2);

        using var provider = services.BuildServiceProvider();
        provider.GetRequiredService<IValidator<TestOrder>>().Should().BeOfType<TestOrderValidator>();
        provider.GetRequiredService<IValidator<TestCustomer>>().Should().BeOfType<TestCustomerValidator>();
    }

    [Fact]
    public void AddValidator_ReturnsSameServiceCollectionForChaining()
    {
        var services = new ServiceCollection();

        var result = services.AddValidator<TestOrderValidator, TestOrder>();

        result.Should().BeSameAs(services);
    }

    [Fact]
    public void AddAsyncValidator_ReturnsSameServiceCollectionForChaining()
    {
        var services = new ServiceCollection();

        var result = services.AddAsyncValidator<TestCustomerAsyncValidator, TestCustomer>();

        result.Should().BeSameAs(services);
    }
}
