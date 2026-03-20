using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Analyzers;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests;

public class VALC402Tests : AnalyzerTestBase<RunValidationOverloadAnalyzer>
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

    [StringSyntax("CSharp")] private const string InputCallsRunValidation = """
                                                                            using System.Collections.Generic;
                                                                            using Test.Requests;
                                                                            using ValiCraft;

                                                                            namespace Test.Usage;

                                                                            public class Consumer
                                                                            {
                                                                                public void DoWork(IValidator<Order> validator)
                                                                                {
                                                                                    validator.RunValidation(new Order { OrderNumber = "123" }, null);
                                                                                }
                                                                            }
                                                                            """;

    [StringSyntax("CSharp")] private const string InputCallsValidateNormally = """
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

    [StringSyntax("CSharp")] private const string InputCallsRunValidationAsync = """
                                                                                 using System.Collections.Generic;
                                                                                 using System.Threading;
                                                                                 using System.Threading.Tasks;
                                                                                 using Test.Requests;
                                                                                 using ValiCraft;

                                                                                 namespace Test.Usage;

                                                                                 public class AsyncConsumer
                                                                                 {
                                                                                     public async Task DoWork(IAsyncValidator<Order> validator)
                                                                                     {
                                                                                         await validator.RunValidationAsync(new Order { OrderNumber = "123" }, null, CancellationToken.None);
                                                                                     }
                                                                                 }
                                                                                 """;

    [Fact]
    public void ShouldReportVALC402_WhenCallingRunValidation()
    {
        AssertAnalyzer(
            inputs: [InputRequests, InputValidatorClass, InputCallsRunValidation],
            diagnostics: [
                "RunValidation is for internal use by generated code only and should not be called directly"
            ]);
    }

    [Fact]
    public void ShouldNotReportVALC402_WhenCallingValidateNormally()
    {
        AssertAnalyzer(
            inputs: [InputRequests, InputValidatorClass, InputCallsValidateNormally],
            diagnostics: []);
    }

    [Fact]
    public void ShouldReportVALC402_WhenCallingRunValidationAsync()
    {
        AssertAnalyzer(
            inputs: [InputRequests, InputValidatorClass, InputCallsRunValidationAsync],
            diagnostics: [
                "RunValidation is for internal use by generated code only and should not be called directly"
            ]);
    }
}
