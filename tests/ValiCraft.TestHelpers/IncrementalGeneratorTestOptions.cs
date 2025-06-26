using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace ValiCraft.TestHelpers;

/// <summary>
/// Provides configuration options for the <see cref="IncrementalGeneratorAdapter{TGenerator}"/>.
/// </summary>
public class IncrementalGeneratorTestOptions
{
    /// <summary>
    /// Gets or sets the name of the compilation created during the test.
    /// Defaults to "TestCompilation".
    /// </summary>
    public string CompilationName { get; set; } = "TestCompilation";

    /// <summary>
    /// Gets or sets the C# compilation options.
    /// Defaults to a standard DLL output kind.
    /// </summary>
    public CSharpCompilationOptions CompilationOptions { get; set; }

    /// <summary>
    /// Gets or sets the list of metadata references to be included in the test compilation.
    /// This should include your generator's dependencies and any other required assemblies.
    /// </summary>
    public List<MetadataReference> MetadataReferences { get; set; }

    /// <summary>
    /// Gets or sets a list of types that are "banned" from appearing in the output of
    /// an incremental generator pipeline stage. The object graph of each output will be
    /// traversed to ensure it doesn't contain instances of these types.
    /// Defaults to Roslyn's Compilation, ISymbol, and SyntaxNode.
    /// </summary>
    public List<Type> BannedTypesForAnalysis { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to perform a second generator run
    /// to assert that all pipeline stages are producing cacheable outputs.
    /// Defaults to true.
    /// </summary>
    public bool AssertCacheability { get; set; } = true;

    /// <summary>
    /// Creates a new instance of <see cref="IncrementalGeneratorTestOptions"/> with sensible defaults.
    /// This includes references to core .NET assemblies and default banned types for analysis.
    /// </summary>
    /// <returns>A new <see cref="IncrementalGeneratorTestOptions"/> instance with default values.</returns>
    public static IncrementalGeneratorTestOptions CreateDefault(params Type[] metadataReferenceTypes)
    {
        // The path to the .NET assemblies
        var dotNetAssemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location)!;

        var metadataReferences = new List<MetadataReference>()
        {
            // A minimal set of default references
            MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "mscorlib.dll")),
            MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.dll")),
            MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Core.dll")),
            MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Private.CoreLib.dll")),
            MetadataReference.CreateFromFile(Path.Combine(dotNetAssemblyPath, "System.Runtime.dll")),
        };
        
        foreach (var metadataReferenceType in metadataReferenceTypes)
        {
            metadataReferences.Add(MetadataReference.CreateFromFile(metadataReferenceType.Assembly.Location));
        }
        
        return new IncrementalGeneratorTestOptions
        {
            CompilationOptions = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary),
            MetadataReferences = metadataReferences,
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