using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Models;

public enum AccessorType
{
    Property,
    Object,
    Method
}

public record ValidationTarget(
    AccessorType AccessorType,
    string AccessorExpressionFormat,
    TypeInfo Type,
    MessageInfo DefaultTargetName,
    MessageInfo TargetPath);