using ValiCraft.Generator.Shared.Types;

namespace ValiCraft.Generator.Shared.Concepts;

public record MethodSignatureInfo(string MethodName, EquatableArray<string> ParameterTypeNames);