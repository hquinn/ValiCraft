using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Models;

public record ValidationTarget(
    string AccessorExpressionFormat,
    TypeInfo Type,
    MessageInfo DefaultTargetName);