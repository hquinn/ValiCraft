using ValiCraft.Generator.Concepts;

namespace ValiCraft.Generator.Models;

public record RuleOverrideData(
    MessageInfo? OverrideMessage,
    MessageInfo? OverridePropertyName,
    MessageInfo? OverrideErrorCode);