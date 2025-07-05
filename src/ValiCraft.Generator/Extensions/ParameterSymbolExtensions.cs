using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Extensions;

public static class ParameterSymbolExtensions
{
    /// <summary>
    ///     Determines if a parameter is nullable, handling both nullable reference types (e.g., string?)
    ///     and nullable value types (e.g., int?).
    /// </summary>
    public static bool IsNullable(this IParameterSymbol parameter)
    {
        // Check #1: Nullable Reference Types (e.g., string?, MyClass?)
        // The '?' annotation on a reference type sets this property.
        if (parameter.NullableAnnotation == NullableAnnotation.Annotated)
        {
            return true;
        }

        // Check #2: Nullable Value Types (e.g., int?, bool?)
        // The type must be an INamedTypeSymbol to be System.Nullable<T>.
        if (parameter.Type is not INamedTypeSymbol namedType)
        {
            return false;
        }

        // A nullable value type like 'int?' is actually 'System.Nullable<int>'.
        // We check if its original definition is the special 'System.Nullable<T>' type.
        return namedType is { IsGenericType: true, OriginalDefinition.SpecialType: SpecialType.System_Nullable_T };
    }
}