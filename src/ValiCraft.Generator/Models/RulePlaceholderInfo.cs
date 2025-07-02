using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record RulePlaceholderInfo(string PlaceholderName, string ParameterName)
{
    public static EquatableArray<RulePlaceholderInfo> CreateFromRulePlaceholderAttributes(
        INamedTypeSymbol symbol)
    {
        const string attributeName = FullyQualifiedNames.Attributes.RulePlaceholderAttribute;

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
                return new RulePlaceholderInfo(placeHolderName!, parameterName!);
            })
            .ToEquatableImmutableArray();
    }
}