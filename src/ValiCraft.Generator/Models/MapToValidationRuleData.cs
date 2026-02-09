using System.Linq;
using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Extensions;
using ValiCraft.Generator.Types;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Models;

public record MapToValidationRuleData(
    string RuleType,
    string MethodName,
    EquatableArray<int>? GenericArgumentIndices)
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

    private static MapToValidationRuleData? CreateFromAttribute(AttributeData attributeData)
    {
        try
        {
            var ruleType = (attributeData.ConstructorArguments[0].Value as INamedTypeSymbol)
                ?.ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric) ?? string.Empty;
            var methodName = attributeData.ConstructorArguments[1].Value as string ?? string.Empty;

            if (attributeData.ConstructorArguments.Length > 2)
            {
                var arg2 = attributeData.ConstructorArguments[2];

                var genericArgumentIndices = !arg2.IsNull
                    ? arg2.Values.Select(v => (int)v.Value!).ToArray()
                    : null;

                return new(ruleType, methodName, genericArgumentIndices?.ToEquatableImmutableArray());
            }

            return new(ruleType, methodName, null);
        }
        catch
        {
            // Eat exception, we report on this. It's also invalid C#
            return null;
        }
    }
}