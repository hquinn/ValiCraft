using Microsoft.CodeAnalysis;
using ValiCraft.Generator.Utils;

namespace ValiCraft.Generator.Models;

public record MapToValidationRuleData
{
    public MapToValidationRuleData(AttributeData attributeData)
    {
        FullyQualifiedValidationRule = (attributeData.ConstructorArguments[0].Value as INamedTypeSymbol)
            ?.ToDisplayString(SymbolDisplayFormats.FormatWithoutGeneric) ?? string.Empty;
        FullyQualifiedUnboundedName = ""; // Not needed here
        ValidationRuleGenericFormat = attributeData.ConstructorArguments[1].Value as string ?? string.Empty;
        DefaultMessage = attributeData.ConstructorArguments[2].Value as string;
    }

    public MapToValidationRuleData(
        string fullyQualifiedValidationRule,
        string fullyQualifiedUnboundedName,
        string validationRuleGenericFormat,
        string? defaultMessage)
    {
        FullyQualifiedValidationRule = fullyQualifiedValidationRule;
        FullyQualifiedUnboundedName = fullyQualifiedUnboundedName;
        ValidationRuleGenericFormat = validationRuleGenericFormat;
        DefaultMessage = defaultMessage;
    }
    
    public string FullyQualifiedValidationRule { get; init; }
    public string FullyQualifiedUnboundedName { get; init; }
    public string ValidationRuleGenericFormat { get; init; }
    public string? DefaultMessage { get; init; }
}