using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Models;

public enum AccessorType
{
    Property,
    Object
}

public record ValidationTarget(
    AccessorType AccessorType,
    string AccessorExpressionFormat,
    TypeInfo Type,
    MessageInfo DefaultTargetName,
    MessageInfo TargetPath);