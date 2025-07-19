using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record RulePlaceholder(string PlaceholderName, string ParameterName)
{
    public static EquatableArray<RulePlaceholder> CreateFromRulePlaceholderAttributes(INamedTypeSymbol symbol)
    {
        const string attributeName = KnownNames.Attributes.RulePlaceholderAttribute;

        return symbol
            .GetAttributes()
            .Where(ad => ad.AttributeClass?.ToDisplayString() == attributeName &&
                         ad.ConstructorArguments is { IsDefaultOrEmpty: false, Length: 2 } &&
                         ad.ConstructorArguments[0].Kind == TypedConstantKind.Primitive &&
                         ad.ConstructorArguments[0].Value is string placeHolderName &&
                         !string.IsNullOrWhiteSpace(placeHolderName) &&
                         ad.ConstructorArguments[1].Kind == TypedConstantKind.Primitive &&
                         ad.ConstructorArguments[1].Value is string parameterName &&
                         !string.IsNullOrWhiteSpace(parameterName))
            .Select(ad =>
            {
                var placeHolderName = ad.ConstructorArguments[0].Value as string;
                var parameterName = ad.ConstructorArguments[1].Value as string;
                return new RulePlaceholder(placeHolderName!, parameterName!);
            })
            .ToEquatableImmutableArray();
    }
    
    public static bool TryCreateFromRulePlaceholderAttributes(
        INamedTypeSymbol symbol,
        MethodSignature isValidMethodSignature,
        List<DiagnosticInfo> diagnostics,
        out EquatableArray<RulePlaceholder> rulePlaceholders)
    {
        const string attributeName = KnownNames.Attributes.RulePlaceholderAttribute;
        
        var rulePlaceholderList = new List<RulePlaceholder>();

        var rulePlaceholderAttributes = symbol
            .GetAttributes()
            .Where(ad => ad.AttributeClass?.ToDisplayString() == attributeName);
        
        foreach (var attribute in rulePlaceholderAttributes)
        {
            // No need for diagnostics here as it's not valid C# anyway
            if (attribute.ConstructorArguments is not { IsDefaultOrEmpty: false, Length: 2 })
            {
                rulePlaceholders = EquatableArray<RulePlaceholder>.Empty;
                return false;
            }

            if (!ValidConstructorArgument(attribute.ConstructorArguments[0]))
            {
                diagnostics.Add(DefinedDiagnostics.InvalidRulePlaceholderConstructorArgument(
                    attribute.ApplicationSyntaxReference!.GetSyntax().GetLocation()));
                rulePlaceholders = EquatableArray<RulePlaceholder>.Empty;
                return false;
            }

            if (!ValidConstructorArgument(attribute.ConstructorArguments[1]))
            {
                diagnostics.Add(DefinedDiagnostics.InvalidRulePlaceholderConstructorArgument(
                    attribute.ApplicationSyntaxReference!.GetSyntax().GetLocation()));
                rulePlaceholders = EquatableArray<RulePlaceholder>.Empty;
                return false;
            }
            
            var placeHolderName = attribute.ConstructorArguments[0].Value as string;
            var parameterName = attribute.ConstructorArguments[1].Value as string;

            if (!isValidMethodSignature.Parameters.Select(x => x.Name).Contains(parameterName))
            {
                diagnostics.Add(DefinedDiagnostics.InvalidRulePlaceholderParameterName(
                    parameterName!,
                    attribute.ApplicationSyntaxReference!.GetSyntax().GetLocation()));
                rulePlaceholders = EquatableArray<RulePlaceholder>.Empty;
                return false;
            }
            
            rulePlaceholderList.Add(new RulePlaceholder(placeHolderName!, parameterName!));
        }

        rulePlaceholders = rulePlaceholderList.ToEquatableImmutableArray();

        return true;
    }

    private static bool ValidConstructorArgument(TypedConstant argument)
    {
        return argument is { Kind: TypedConstantKind.Primitive, Value: string argumentValue } &&
               !string.IsNullOrWhiteSpace(argumentValue);
    }
}