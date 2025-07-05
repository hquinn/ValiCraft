using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ValiCraft.Generator.Tests.Helpers;

/// <summary>
///     Provides configuration options for the <see cref="IncrementalGeneratorAdapter" />.
/// </summary>
public class IncrementalGeneratorTestOptions
{
    /// <summary>
    ///     Gets or sets the name of the compilation created during the test.
    ///     Defaults to "TestCompilation".
    /// </summary>
    public string CompilationName { get; set; } = "TestCompilation";

    /// <summary>
    ///     Gets or sets the C# compilation options.
    ///     Defaults to a standard DLL output kind.
    /// </summary>
    public required CSharpCompilationOptions CompilationOptions { get; set; }

    /// <summary>
    ///     Gets or sets the list of metadata references to be included in the test compilation.
    ///     This should include your generator's dependencies and any other required assemblies.
    /// </summary>
    public required List<MetadataReference> MetadataReferences { get; set; }

    /// <summary>
    ///     Gets or sets a list of types that are "banned" from appearing in the output of
    ///     an incremental generator pipeline stage. The object graph of each output will be
    ///     traversed to ensure it doesn't contain instances of these types.
    ///     Defaults to Roslyn's Compilation, ISymbol, and SyntaxNode.
    /// </summary>
    public required List<Type> BannedTypesForAnalysis { get; set; }

    /// <summary>
    ///     Gets or sets a value indicating whether to perform a second generator run
    ///     to assert that all pipeline stages are producing cacheable outputs.
    ///     Defaults to true.
    /// </summary>
    public bool AssertCacheability { get; set; } = true;

    /// <summary>
    ///     Creates a new instance of <see cref="IncrementalGeneratorTestOptions" /> with sensible defaults.
    ///     This includes references to core .NET assemblies and default banned types for analysis.
    /// </summary>
    /// <returns>A new <see cref="IncrementalGeneratorTestOptions" /> instance with default values.</returns>
    public static IncrementalGeneratorTestOptions CreateDefault(Type[] additionalMetadataReferenceTypes)
    {
        var loadedAssemblies = AppDomain.CurrentDomain.GetAssemblies();

        var references = new List<MetadataReference>();

        foreach (var assembly in loadedAssemblies)
            // We can't create a reference to dynamic or in-memory assemblies, so we skip them.
        {
            if (!assembly.IsDynamic && !string.IsNullOrEmpty(assembly.Location))
            {
                references.Add(MetadataReference.CreateFromFile(assembly.Location));
            }
        }

        // Your existing logic for adding specific types is still useful as a fallback
        // in case an assembly wasn't loaded for some reason.
        foreach (var type in additionalMetadataReferenceTypes)
        {
            if (!references.Any(r => Path.GetFileName(r.Display) == Path.GetFileName(type.Assembly.Location)))
            {
                references.Add(MetadataReference.CreateFromFile(type.Assembly.Location));
            }
        }

        return new IncrementalGeneratorTestOptions
        {
            CompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            MetadataReferences = references,
            BannedTypesForAnalysis = new List<Type>
            {
                // These types are expensive and should not be stored in incremental models
                typeof(Compilation),
                typeof(ISymbol),
                typeof(SyntaxNode)
            }
        };
    }
}