using System.Diagnostics.CodeAnalysis;
using MonadCraft;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

/// <summary>
/// Tests for VALC302: Class marked with [GenerateAsyncValidator] does not inherit from AsyncValidator&lt;T&gt;.
/// </summary>
public class VALC302Tests : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  using System;
                                                                  
                                                                  namespace Test.Requests;
                                                                  
                                                                  public class User
                                                                  {
                                                                      public string? Email { get; set; }
                                                                  }
                                                                  """;

    /// <summary>
    /// Tests that VALC302 is reported when [GenerateAsyncValidator] is used on a class 
    /// that does not inherit from AsyncValidator&lt;T&gt;.
    /// </summary>
    [Fact]
    public void ShouldReportVALC302_WhenNotInheritingFromAsyncValidator()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator
                                      {
                                          // No base class - should trigger VALC302
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: [],
            diagnostics: ["Missing AsyncValidator base class on validator marked with [GenerateAsyncValidator]"],
            assertTrackingSteps: false);
    }

    /// <summary>
    /// Tests that VALC302 is reported when [GenerateAsyncValidator] is used on a class 
    /// that inherits from the sync Validator&lt;T&gt; instead of AsyncValidator&lt;T&gt;.
    /// </summary>
    [Fact]
    public void ShouldReportVALC302_WhenInheritingFromSyncValidator()
    {
        const string inputValidator = """
                                      using System;
                                      using System.Threading;
                                      using System.Threading.Tasks;
                                      using Test.Requests;
                                      using ValiCraft;
                                      using ValiCraft.Attributes;
                                      using ValiCraft.BuilderTypes;
                                      
                                      namespace Test.Validators;
                                      
                                      [GenerateAsyncValidator]
                                      public partial class UserValidator : Validator<User>
                                      {
                                          protected override void DefineRules(IValidationRuleBuilder<User> builder)
                                          {
                                          }
                                      }
                                      """;

        AssertGenerator(
            errorCodePrefix: "VALC",
            additionalMetadataReferences: [typeof(AsyncValidator<>), typeof(Validator<>), typeof(Result<,>)],
            trackingSteps: [TrackingSteps.AsyncValidatorResultTrackingName],
            inputs: [InputRequests, inputValidator],
            outputs: [],
            diagnostics: ["Missing AsyncValidator base class on validator marked with [GenerateAsyncValidator]"],
            assertTrackingSteps: false);
    }
}
