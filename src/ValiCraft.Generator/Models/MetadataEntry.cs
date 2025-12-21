using System;

namespace ValiCraft.Generator.Models;

/// <summary>
/// Represents a single metadata key-value entry for a validation error.
/// </summary>
public record MetadataEntry(
    string Key,
    string Value,
    string ValueType,
    bool IsLiteral) : IEquatable<MetadataEntry>;
