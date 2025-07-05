using Microsoft.CodeAnalysis;

namespace ValiCraft.Generator.Concepts;

public sealed record DiagnosticInfo
{
    public DiagnosticInfo(DiagnosticDescriptor descriptor, Location? location)
    {
        Descriptor = descriptor;
        Location = location is not null ? LocationInfo.CreateFrom(location) : null;
    }

    public DiagnosticDescriptor Descriptor { get; }
    public LocationInfo? Location { get; }

    public Diagnostic CreateDiagnostic()
    {
        return Diagnostic.Create(Descriptor, Location?.ToLocation());
    }
}