using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Analyzers;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC401Tests : AnalyzerTestBase<ValidateOverloadAnalyzer>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                                  namespace Test.Requests;

                                                                  public class Order
                                                                  {
                                                                      public required string OrderNumber { get; set; }
                                                                  }
                                                                  """;

    [StringSyntax("CSharp")] private const string InputValidatorClass = """
                                                                        using Test.Requests;
                                                                        using ValiCraft;
                                                                        using ValiCraft.Attributes;
                                                                        using ValiCraft.BuilderTypes;

                                                                        namespace Test.Validators;

                                                                        [GenerateValidator]
                                                                        public partial class OrderValidator : Validator<Order>
                                                                        {
                                                                            protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                            {
                                                                            }
                                                                        }
                                                                        """;

    [StringSyntax("CSharp")] private const string InputCallsValidateWithInheritedTargetPath = """
                                                                                              using Test.Requests;
                                                                                              using ValiCraft;

                                                                                              namespace Test.Usage;

                                                                                              public class Consumer
                                                                                              {
                                                                                                  public void DoWork(IValidator<Order> validator)
                                                                                                  {
                                                                                                      validator.Validate(new Order { OrderNumber = "123" }, "path");
                                                                                                  }
                                                                                              }
                                                                                              """;

    [StringSyntax("CSharp")] private const string InputCallsValidateWithoutInheritedTargetPath = """
                                                                                                 using Test.Requests;
                                                                                                 using ValiCraft;

                                                                                                 namespace Test.Usage;

                                                                                                 public class Consumer
                                                                                                 {
                                                                                                     public void DoWork(IValidator<Order> validator)
                                                                                                     {
                                                                                                         validator.Validate(new Order { OrderNumber = "123" });
                                                                                                     }
                                                                                                 }
                                                                                                 """;

    [StringSyntax("CSharp")] private const string InputCallsAsyncValidateWithInheritedTargetPath = """
                                                                                                   using System.Threading;
                                                                                                   using System.Threading.Tasks;
                                                                                                   using Test.Requests;
                                                                                                   using ValiCraft;

                                                                                                   namespace Test.Usage;

                                                                                                   public class Consumer
                                                                                                   {
                                                                                                       public async Task DoWork(IAsyncValidator<Order> validator)
                                                                                                       {
                                                                                                           await validator.ValidateAsync(new Order { OrderNumber = "123" }, "path");
                                                                                                       }
                                                                                                   }
                                                                                                   """;

    [StringSyntax("CSharp")] private const string InputCallsAsyncValidateWithoutInheritedTargetPath = """
                                                                                                      using System.Threading;
                                                                                                      using System.Threading.Tasks;
                                                                                                      using Test.Requests;
                                                                                                      using ValiCraft;

                                                                                                      namespace Test.Usage;

                                                                                                      public class Consumer
                                                                                                      {
                                                                                                          public async Task DoWork(IAsyncValidator<Order> validator)
                                                                                                          {
                                                                                                              await validator.ValidateAsync(new Order { OrderNumber = "123" });
                                                                                                          }
                                                                                                      }
                                                                                                      """;

    [Fact]
    public void ShouldReportVALC401_WhenCallingValidateWithInheritedTargetPath()
    {
        AssertAnalyzer(
            inputs: [InputRequests, InputValidatorClass, InputCallsValidateWithInheritedTargetPath],
            diagnostics: [
                "The Validate overload with 'inheritedTargetPath' is for internal use only and should not be called directly"
            ]);
    }

    [Fact]
    public void ShouldNotReportVALC401_WhenCallingValidateWithoutInheritedTargetPath()
    {
        AssertAnalyzer(
            inputs: [InputRequests, InputValidatorClass, InputCallsValidateWithoutInheritedTargetPath],
            diagnostics: []);
    }

    [Fact]
    public void ShouldReportVALC401_WhenCallingAsyncValidateWithInheritedTargetPath()
    {
        AssertAnalyzer(
            inputs: [InputRequests, InputValidatorClass, InputCallsAsyncValidateWithInheritedTargetPath],
            diagnostics: [
                "The Validate overload with 'inheritedTargetPath' is for internal use only and should not be called directly"
            ]);
    }

    [Fact]
    public void ShouldNotReportVALC401_WhenCallingAsyncValidateWithoutInheritedTargetPath()
    {
        AssertAnalyzer(
            inputs: [InputRequests, InputValidatorClass, InputCallsAsyncValidateWithoutInheritedTargetPath],
            diagnostics: []);
    }
}
