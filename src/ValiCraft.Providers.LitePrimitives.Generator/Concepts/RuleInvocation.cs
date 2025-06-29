using ValiCraft.Generator.Shared.Concepts;
using ValiCraft.Generator.Shared.Types;

namespace ValiCraft.Providers.LitePrimitives.Generator.Concepts;

public record RuleInvocation(
    NameAndTypeInfo Property,
    string MethodName,
    EquatableArray<NameAndTypeInfo> Arguments,
    string FullyQualifiedValidationRule,
    string ValidationRuleGenericFormat);