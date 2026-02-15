using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests.AsyncGeneratorTests;

public class SingleEnsureRuleChain_AsyncSingleIsMethodRuleUsingInvocationLambdaTest : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                             using System;
                                                             using System.Collections.Generic;

                                                             namespace Test.Requests;

                                                             public class Order
                                                             {
                                                                 public required string OrderNumber { get; set; }
                                                                 public decimal OrderTotal { get; set; }
                                                                 public string? ShippingReference { get; set; }
                                                             }
                                                             """;
    
    [StringSyntax("CSharp")] private const string InputAsyncRules = """
                                                                    using System.Threading;
                                                                    using System.Threading.Tasks;
                                                                    using ValiCraft.Attributes;
                                                                    using ValiCraft.AsyncBuilderTypes;
  
                                                                    namespace Test.Rules;
  
                                                                    public static class TestAsyncRules
                                                                    {
                                                                        [DefaultMessage("{TargetName} must be a valid order number for {Tenant}")]
                                                                        [RulePlaceholder("{Tenant}", "tenant")]
                                                                        public static Task<bool> OrderNumberValid(string orderNumber, string tenant, CancellationToken cancellationToken)
                                                                        {
                                                                            return Task.FromResult(true);
                                                                        }
                                                                        
                                                                        [DefaultMessage("{TargetName} must be a valid order number for {Tenant}")]
                                                                        [RulePlaceholder("{Tenant}", "tenant")]
                                                                        [MapToValidationRule(typeof(TestAsyncRules), nameof(TestAsyncRules.OrderNumberValid))]
                                                                        public static IAsyncValidationRuleBuilderType<TRequest, string> IsOrderNumberValid<TRequest>(
                                                                           this IAsyncBuilderType<TRequest, string> builder,
                                                                           string tenant)
                                                                           where TRequest : class
                                                                        {
                                                                           return builder.Is(OrderNumberValid, tenant);
                                                                        }
                                                                    }
                                                                    """;

    // Define the validator against our request types.
    // This will generate a Validate method in a partial class.
    [StringSyntax("CSharp")] private const string InputValidatorsToGenerate = """
                                                                         using Test.Requests;
                                                                         using Test.Rules;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.AsyncBuilderTypes;
                                                                         using ErrorCraft;

                                                                         namespace Test.Validators;

                                                                         [GenerateValidator]
                                                                         public partial class OrderValidator : AsyncValidator<Order>
                                                                         {
                                                                             protected override void DefineRules(IAsyncValidationRuleBuilder<Order> orderBuilder)
                                                                             {
                                                                                 orderBuilder.Ensure(x => x.OrderNumber)
                                                                                    .Is(async (x, ct) => await TestAsyncRules.OrderNumberValid(x, "tenant", ct));
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string ExpectedValidators = """
                                                                       // <auto-generated />
                                                                       #nullable enable
                                                                       
                                                                       using Test.Requests;
                                                                       using Test.Rules;
                                                                       using ValiCraft;
                                                                       using ValiCraft.Attributes;
                                                                       using ValiCraft.AsyncBuilderTypes;
                                                                       using ErrorCraft;
                                                                       
                                                                       namespace Test.Validators
                                                                       {
                                                                           /// <summary>
                                                                           /// Generated async validator for <see cref="global::Test.Requests.Order"/>.
                                                                           /// </summary>
                                                                           public partial class OrderValidator : global::ValiCraft.IAsyncValidator<global::Test.Requests.Order>
                                                                           {
                                                                               /// <inheritdoc />
                                                                               public async global::System.Threading.Tasks.Task<global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Order>> ValidateAsync(global::Test.Requests.Order request, global::System.Threading.CancellationToken cancellationToken = default)
                                                                               {
                                                                                   var errors = await RunValidationLogicAsync(request, null, cancellationToken);

                                                                                   return errors is not null
                                                                                       ? global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Order>.Failure(new global::ErrorCraft.ValidationErrors
                                                                                       {
                                                                                           Code = "OrderErrors",
                                                                                           Message = "One or more validation errors occurred.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           Errors = errors
                                                                                       })
                                                                                       : global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Order>.Success(request);
                                                                               }

                                                                               /// <inheritdoc />
                                                                               public async global::System.Threading.Tasks.Task<global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError>> ValidateToListAsync(global::Test.Requests.Order request, global::System.Threading.CancellationToken cancellationToken = default)
                                                                               {
                                                                                   return await RunValidationLogicAsync(request, null, cancellationToken) ?? [];
                                                                               }

                                                                               /// <inheritdoc />
                                                                               [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                               public async global::System.Threading.Tasks.Task<global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError>> ValidateToListAsync(global::Test.Requests.Order request, string? inheritedTargetPath, global::System.Threading.CancellationToken cancellationToken = default)
                                                                               {
                                                                                   return await RunValidationLogicAsync(request, inheritedTargetPath, cancellationToken) ?? [];
                                                                               }

                                                                               private async global::System.Threading.Tasks.Task<global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>?> RunValidationLogicAsync(global::Test.Requests.Order request, string? inheritedTargetPath, global::System.Threading.CancellationToken cancellationToken)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;
                                                                       
                                                                                   if (!await TestAsyncRules.OrderNumberValid(request.OrderNumber, "tenant", cancellationToken))
                                                                                   {
                                                                                       errors ??= new(1);
                                                                                       errors.Add(new global::ErrorCraft.ValidationError<string>
                                                                                       {
                                                                                           Code = "TestAsyncRules.OrderNumberValid",
                                                                                           Message = $"Order Number must be a valid order number for tenant",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           TargetName = "Order Number",
                                                                                           TargetPath = $"{inheritedTargetPath}OrderNumber",
                                                                                           AttemptedValue = request.OrderNumber,
                                                                                       });
                                                                                   }

                                                                                   return errors;
                                                                               }
                                                                           }
                                                                       }
                                                                       """;

    [Fact]
    public void ShouldGenerateValidator()
    {
        AssertGenerator(inputs: [InputRequests, InputAsyncRules, InputValidatorsToGenerate], 
            outputs: [ExpectedValidators],
            diagnostics: [],
            assertInitialCompilation: true);
    }
}