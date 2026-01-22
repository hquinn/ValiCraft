using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Models;

public record MapToValidationRuleData(
    string FullyQualifiedValidationRule,
    string ValidationRuleGenericFormat)
{
    public static MapToValidationRuleData? CreateFromMethodAndAttribute(
        IMethodSymbol methodSymbol,
        string attributeName)
    {
        var attributeDisplayFormat = SymbolDisplayFormats.FormatAttributeWithoutParameters;
        var attribute = methodSymbol
            .GetAttributes()
            .FirstOrDefault(ad => ad.AttributeClass?.ToDisplayString(attributeDisplayFormat) == attributeName);

        if (attribute is not null)
        {
            return CreateFromAttribute(attribute);
        }

        return null;
    }

    public static MapToValidationRuleData CreateFromAttribute(AttributeData attributeData)
    {
        return new(
            (attributeData.ConstructorArguments[0].Value as INamedTypeSymbol)
            ?.ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric) ?? string.Empty, // Not needed here
            attributeData.ConstructorArguments[1].Value as string ?? string.Empty);
    }
}