using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.SourceProviders;

public static class DependencyInjectionSourceProvider
{
    private const string ValiCraftModuleAttributeMetadataName =
        "ValiCraft.ValiCraftModuleAttribute`1";

    private const string ValiCraftModuleInterfaceMetadataName =
        "ValiCraft.IValiCraftModule";

    private const string ServiceCollectionMetadataName =
        "Microsoft.Extensions.DependencyInjection.IServiceCollection";

    /// <summary>
    /// Extracts DI-relevant context from the compilation into a cacheable value type.
    /// Returns null if the ValiCraft core library is not referenced (i.e. IValiCraftModule is not available).
    /// </summary>
    public static DiContext? ExtractDiContext(Compilation compilation)
    {
        if (compilation.GetTypeByMetadataName(ValiCraftModuleInterfaceMetadataName) is null)
        {
            return null;
        }

        var assemblyName = compilation.AssemblyName ?? "Unknown";
        var hasDependencyInjection = compilation.GetTypeByMetadataName(ServiceCollectionMetadataName) is not null;
        var referencedModules = DiscoverReferencedModules(compilation);

        return new DiContext(assemblyName, hasDependencyInjection, referencedModules.ToEquatableImmutableArray());
    }

    public static void EmitSourceCode(
        ImmutableArray<ProviderResult<Validator>> validatorResults,
        DiContext diContext,
        SourceProductionContext context)
    {
        var registrableValidators = validatorResults
            .Where(vr => vr.Value is not null
                         && !vr.Value.IsStatic
                         && vr.Diagnostics is not { Count: > 0 })
            .Select(vr => vr.Value!)
            .ToList();

        var assemblyName = diContext.AssemblyName;
        var safeAssemblyName = GetSafeIdentifier(assemblyName);
        var referencedModules = diContext.ReferencedModuleTypes;

        // Emit the module registrar and assembly attribute if this project has validators.
        // These are DI-agnostic and only require ValiCraft core.
        if (registrableValidators.Count > 0)
        {
            var registrarSource = GenerateModuleRegistrar(registrableValidators, assemblyName);
            context.AddSource(
                "ValiCraftModuleRegistrar.g.cs",
                SourceText.From(registrarSource, Encoding.UTF8));

            var assemblyAttributeSource = GenerateAssemblyAttribute(assemblyName);
            context.AddSource(
                "ValiCraftModuleAttribute.g.cs",
                SourceText.From(assemblyAttributeSource, Encoding.UTF8));
        }

        // Only emit DI extension methods if Microsoft.Extensions.DependencyInjection is referenced
        if (diContext.HasDependencyInjection)
        {
            // Per-module extension method (e.g. AddMyModuleValiCraft)
            if (registrableValidators.Count > 0)
            {
                var moduleExtensionSource = GenerateModuleExtensionMethod(assemblyName, safeAssemblyName);
                context.AddSource(
                    $"{safeAssemblyName}ValiCraftExtensions.g.cs",
                    SourceText.From(moduleExtensionSource, Encoding.UTF8));
            }

            // Host-level AddValiCraft() method
            var addValiCraftSource = GenerateAddValiCraft(
                registrableValidators.Count > 0 ? assemblyName : null,
                referencedModules);

            context.AddSource(
                "ValiCraftServiceCollectionExtensions.g.cs",
                SourceText.From(addValiCraftSource, Encoding.UTF8));
        }
    }

    private static List<string> DiscoverReferencedModules(Compilation compilation)
    {
        var moduleRegistrarTypes = new List<string>();

        var moduleAttributeType = compilation.GetTypeByMetadataName(ValiCraftModuleAttributeMetadataName);

        if (moduleAttributeType is null)
        {
            return moduleRegistrarTypes;
        }

        foreach (var reference in compilation.References)
        {
            var assemblySymbol = compilation.GetAssemblyOrModuleSymbol(reference) as IAssemblySymbol;

            if (assemblySymbol is null)
            {
                continue;
            }

            foreach (var attribute in assemblySymbol.GetAttributes())
            {
                if (attribute.AttributeClass is not INamedTypeSymbol attributeType)
                {
                    continue;
                }

                if (!SymbolEqualityComparer.Default.Equals(
                        attributeType.OriginalDefinition, moduleAttributeType))
                {
                    continue;
                }

                if (attributeType.TypeArguments.Length == 1)
                {
                    var moduleType = attributeType.TypeArguments[0];
                    moduleRegistrarTypes.Add($"global::{moduleType.ToDisplayString()}");
                }
            }
        }

        return moduleRegistrarTypes;
    }

    private static string GenerateModuleRegistrar(
        List<Validator> validators,
        string assemblyName)
    {
        var registrationsBuilder = new StringBuilder();

        foreach (var validator in validators)
        {
            var interfaceName = validator.IsAsync
                ? KnownNames.Interfaces.IAsyncValidator
                : KnownNames.Interfaces.IValidator;

            var fullyQualifiedClassName = GetFullyQualifiedClassName(validator);

            registrationsBuilder.AppendLine(
                $"            yield return (typeof(global::{interfaceName}<{validator.RequestTypeName.FullyQualifiedName}>), typeof({fullyQualifiedClassName}));");
        }

        return $$"""
                 // <auto-generated />
                 #nullable enable

                 namespace {{assemblyName}}.ValiCraft.Generated
                 {
                     /// <summary>
                     /// Auto-generated module registrar for ValiCraft validators in the {{assemblyName}} assembly.
                     /// </summary>
                     [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                     public sealed class ValiCraftModuleRegistrar : global::ValiCraft.IValiCraftModule
                     {
                         public static global::System.Collections.Generic.IEnumerable<(global::System.Type ServiceType, global::System.Type ImplementationType)> GetValidatorRegistrations()
                         {
                 {{registrationsBuilder}}        }
                     }
                 }
                 """;
    }

    private static string GenerateAssemblyAttribute(string assemblyName)
    {
        return $$"""
                 // <auto-generated />

                 [assembly: global::ValiCraft.ValiCraftModuleAttribute<global::{{assemblyName}}.ValiCraft.Generated.ValiCraftModuleRegistrar>]
                 """;
    }

    private static string GenerateModuleExtensionMethod(
        string assemblyName,
        string safeAssemblyName)
    {
        return $$"""
                 // <auto-generated />
                 #nullable enable

                 namespace ValiCraft.DependencyInjection
                 {
                     /// <summary>
                     /// Extension methods for registering ValiCraft validators from the {{assemblyName}} assembly.
                     /// </summary>
                     public static class {{safeAssemblyName}}ValiCraftExtensions
                     {
                         /// <summary>
                         /// Registers all ValiCraft validators from the {{assemblyName}} assembly.
                         /// </summary>
                         /// <param name="services">The service collection.</param>
                         /// <param name="lifetime">The service lifetime. Defaults to <see cref="global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient"/>.</param>
                         /// <returns>The service collection for chaining.</returns>
                         public static global::Microsoft.Extensions.DependencyInjection.IServiceCollection Add{{safeAssemblyName}}ValiCraft(
                             this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services,
                             global::Microsoft.Extensions.DependencyInjection.ServiceLifetime lifetime = global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient)
                         {
                             foreach (var (serviceType, implementationType) in global::{{assemblyName}}.ValiCraft.Generated.ValiCraftModuleRegistrar.GetValidatorRegistrations())
                             {
                                 services.Add(new global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor(serviceType, implementationType, lifetime));
                             }

                             return services;
                         }
                     }
                 }
                 """;
    }

    private static string GenerateAddValiCraft(
        string? currentAssemblyName,
        EquatableArray<string> referencedModuleTypes)
    {
        var registrationCallsBuilder = new StringBuilder();

        if (currentAssemblyName is not null)
        {
            registrationCallsBuilder.AppendLine(
                $"            foreach (var (serviceType, implementationType) in global::{currentAssemblyName}.ValiCraft.Generated.ValiCraftModuleRegistrar.GetValidatorRegistrations())");
            registrationCallsBuilder.AppendLine(
                "            {");
            registrationCallsBuilder.AppendLine(
                "                services.Add(new global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor(serviceType, implementationType, lifetime));");
            registrationCallsBuilder.AppendLine(
                "            }");
            registrationCallsBuilder.AppendLine();
        }

        foreach (var moduleType in referencedModuleTypes)
        {
            registrationCallsBuilder.AppendLine(
                $"            foreach (var (serviceType, implementationType) in {moduleType}.GetValidatorRegistrations())");
            registrationCallsBuilder.AppendLine(
                "            {");
            registrationCallsBuilder.AppendLine(
                "                services.Add(new global::Microsoft.Extensions.DependencyInjection.ServiceDescriptor(serviceType, implementationType, lifetime));");
            registrationCallsBuilder.AppendLine(
                "            }");
            registrationCallsBuilder.AppendLine();
        }

        return $$"""
                 // <auto-generated />
                 #nullable enable

                 namespace ValiCraft.DependencyInjection
                 {
                     /// <summary>
                     /// Extension methods for registering all discovered ValiCraft validators.
                     /// This class is internal to avoid conflicts when multiple projects each generate their own version.
                     /// </summary>
                     internal static class ValiCraftServiceCollectionExtensions
                     {
                         /// <summary>
                         /// Registers all ValiCraft validators discovered at compile time, including validators
                         /// from referenced assemblies. Static validators are excluded as they do not require
                         /// dependency injection.
                         /// </summary>
                         /// <param name="services">The service collection.</param>
                         /// <param name="lifetime">The service lifetime for all validators. Defaults to <see cref="global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient"/>.</param>
                         /// <returns>The service collection for chaining.</returns>
                         internal static global::Microsoft.Extensions.DependencyInjection.IServiceCollection AddValiCraft(
                             this global::Microsoft.Extensions.DependencyInjection.IServiceCollection services,
                             global::Microsoft.Extensions.DependencyInjection.ServiceLifetime lifetime = global::Microsoft.Extensions.DependencyInjection.ServiceLifetime.Transient)
                         {
                 {{registrationCallsBuilder}}            return services;
                         }
                     }
                 }
                 """;
    }

    private static string GetFullyQualifiedClassName(Validator validator)
    {
        var containingPrefix = string.Join(".",
            validator.Class.ContainingTypes.Select(ct => ct.Name));

        if (!string.IsNullOrEmpty(containingPrefix))
        {
            return $"global::{validator.Class.Namespace}.{containingPrefix}.{validator.Class.Name}";
        }

        return $"global::{validator.Class.Namespace}.{validator.Class.Name}";
    }

    private static string GetSafeIdentifier(string name)
    {
        var sb = new StringBuilder(name.Length);

        foreach (var c in name)
        {
            if (char.IsLetterOrDigit(c) || c == '_')
            {
                sb.Append(c);
            }
        }

        return sb.ToString();
    }
}

/// <summary>
/// Cacheable context extracted from the compilation for DI code generation.
/// </summary>
public record DiContext(
    string AssemblyName,
    bool HasDependencyInjection,
    EquatableArray<string> ReferencedModuleTypes);
