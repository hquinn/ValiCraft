using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.Models;

public record RuleOverrideData(
    MessageInfo? OverrideMessage,
    MessageInfo? OverrideTargetName,
    MessageInfo? OverrideErrorCode,
    MessageInfo? OverrideSeverity,
    EquatableArray<MetadataEntry>? OverrideMetadata);
