using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Concepts;

public record MethodSignatureInfo(string MethodName, EquatableArray<string> ParameterTypeNames);