using Microsoft.Extensions.DependencyInjection;

namespace ValiCraft.DependencyInjection;

/// <summary>
/// Extension methods for manually registering ValiCraft validators with <see cref="IServiceCollection"/>.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Registers a synchronous validator as <see cref="IValidator{TRequest}"/>.
    /// </summary>
    /// <typeparam name="TValidator">The concrete validator type.</typeparam>
    /// <typeparam name="TRequest">The request type being validated.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime. Defaults to <see cref="ServiceLifetime.Transient"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddValidator<TValidator, TRequest>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TValidator : class, IValidator<TRequest>
        where TRequest : class
    {
        services.Add(new ServiceDescriptor(
            typeof(IValidator<TRequest>),
            typeof(TValidator),
            lifetime));

        return services;
    }

    /// <summary>
    /// Registers an asynchronous validator as <see cref="IAsyncValidator{TRequest}"/>.
    /// </summary>
    /// <typeparam name="TValidator">The concrete validator type.</typeparam>
    /// <typeparam name="TRequest">The request type being validated.</typeparam>
    /// <param name="services">The service collection.</param>
    /// <param name="lifetime">The service lifetime. Defaults to <see cref="ServiceLifetime.Transient"/>.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddAsyncValidator<TValidator, TRequest>(
        this IServiceCollection services,
        ServiceLifetime lifetime = ServiceLifetime.Transient)
        where TValidator : class, IAsyncValidator<TRequest>
        where TRequest : class
    {
        services.Add(new ServiceDescriptor(
            typeof(IAsyncValidator<TRequest>),
            typeof(TValidator),
            lifetime));

        return services;
    }
}
