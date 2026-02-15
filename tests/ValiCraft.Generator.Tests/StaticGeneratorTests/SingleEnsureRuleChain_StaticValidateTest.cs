using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests.StaticGeneratorTests;

public class SingleEnsureRuleChain_StaticValidateTest : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                             using System;
                                                             using System.Collections.Generic;

                                                             namespace Test.Requests;

                                                             public class Order
                                                             {
                                                                 public required string OrderNumber { get; set; }
                                                                 public decimal OrderTotal { get; set; }
                                                                 public required Customer Customer { get; set; }
                                                             }
                                                             
                                                             public class Customer
                                                             {
                                                                 public required string Name { get; set; }
                                                             }
                                                             """;

    [StringSyntax("CSharp")] private const string InputCustomerValidator = """
                                                                         using Test.Requests;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.BuilderTypes;
                                                                         using ErrorCraft;

                                                                         namespace Test.Validators;

                                                                         [GenerateValidator]
                                                                         public partial class CustomerValidator : StaticValidator<Customer>
                                                                         {
                                                                             protected override void DefineRules(IValidationRuleBuilder<Customer> builder)
                                                                             {
                                                                                 builder.Ensure(x => x.Name)
                                                                                    .IsNotNullOrWhiteSpace();
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string InputOrderValidator = """
                                                                         using Test.Requests;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.BuilderTypes;
                                                                         using ErrorCraft;

                                                                         namespace Test.Validators;

                                                                         [GenerateValidator]
                                                                         public partial class OrderValidator : StaticValidator<Order>
                                                                         {
                                                                             protected override void DefineRules(IValidationRuleBuilder<Order> orderBuilder)
                                                                             {
                                                                                 orderBuilder.Ensure(x => x.Customer)
                                                                                    .Validate<CustomerValidator>();
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string ExpectedCustomerValidator = """
                                                                       // <auto-generated />
                                                                       #nullable enable
                                                                       
                                                                       using Test.Requests;
                                                                       using ValiCraft;
                                                                       using ValiCraft.Attributes;
                                                                       using ValiCraft.BuilderTypes;
                                                                       using ErrorCraft;
                                                                       
                                                                       namespace Test.Validators
                                                                       {
                                                                           /// <summary>
                                                                           /// Generated static validator for <see cref="global::Test.Requests.Customer"/>.
                                                                           /// </summary>
                                                                           public partial class CustomerValidator : global::ValiCraft.IStaticValidator<global::Test.Requests.Customer>
                                                                           {
                                                                               /// <inheritdoc />
                                                                               public static global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Customer> Validate(global::Test.Requests.Customer request)
                                                                               {
                                                                                   var errors = RunValidationLogic(request, null);

                                                                                   return errors is not null
                                                                                       ? global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Customer>.Failure(new global::ErrorCraft.ValidationErrors
                                                                                       {
                                                                                           Code = "CustomerErrors",
                                                                                           Message = "One or more validation errors occurred.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           Errors = errors
                                                                                       })
                                                                                       : global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Customer>.Success(request);
                                                                               }

                                                                               /// <inheritdoc />
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.Customer request)
                                                                               {
                                                                                   return RunValidationLogic(request, null) ?? [];
                                                                               }

                                                                               /// <inheritdoc />
                                                                               [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.Customer request, string? inheritedTargetPath)
                                                                               {
                                                                                   return RunValidationLogic(request, inheritedTargetPath) ?? [];
                                                                               }

                                                                               private static global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? RunValidationLogic(global::Test.Requests.Customer request, string? inheritedTargetPath)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;

                                                                                   if (!global::ValiCraft.Rules.NotNullOrWhiteSpace(request.Name))
                                                                                   {
                                                                                       errors ??= new(1);
                                                                                       errors.Add(new global::ErrorCraft.ValidationError<string>
                                                                                       {
                                                                                           Code = nameof(global::ValiCraft.Rules.NotNullOrWhiteSpace),
                                                                                           Message = $"Name must not be null or contain only whitespace.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           TargetName = "Name",
                                                                                           TargetPath = $"{inheritedTargetPath}Name",
                                                                                           AttemptedValue = request.Name,
                                                                                       });
                                                                                   }

                                                                                   return errors;
                                                                               }
                                                                           }
                                                                       }
                                                                       """;

    [StringSyntax("CSharp")] private const string ExpectedOrderValidator = """
                                                                       // <auto-generated />
                                                                       #nullable enable
                                                                       
                                                                       using Test.Requests;
                                                                       using ValiCraft;
                                                                       using ValiCraft.Attributes;
                                                                       using ValiCraft.BuilderTypes;
                                                                       using ErrorCraft;
                                                                       
                                                                       namespace Test.Validators
                                                                       {
                                                                           /// <summary>
                                                                           /// Generated static validator for <see cref="global::Test.Requests.Order"/>.
                                                                           /// </summary>
                                                                           public partial class OrderValidator : global::ValiCraft.IStaticValidator<global::Test.Requests.Order>
                                                                           {
                                                                               /// <inheritdoc />
                                                                               public static global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.Order> Validate(global::Test.Requests.Order request)
                                                                               {
                                                                                   var errors = RunValidationLogic(request, null);

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
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.Order request)
                                                                               {
                                                                                   return RunValidationLogic(request, null) ?? [];
                                                                               }

                                                                               /// <inheritdoc />
                                                                               [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.Order request, string? inheritedTargetPath)
                                                                               {
                                                                                   return RunValidationLogic(request, inheritedTargetPath) ?? [];
                                                                               }

                                                                               private static global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? RunValidationLogic(global::Test.Requests.Order request, string? inheritedTargetPath)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;
                                                                       
                                                                                   var errors1 = global::Test.Validators.CustomerValidator.ValidateToList(request.Customer, $"{inheritedTargetPath}Customer.");
                                                                                   if (errors1.Count != 0)
                                                                                   {
                                                                                       if (errors is null)
                                                                                       {
                                                                                           errors = new(errors1);
                                                                                       }
                                                                                       else
                                                                                       {
                                                                                           errors.AddRange(errors1);
                                                                                       }
                                                                                   }

                                                                                   return errors;
                                                                               }
                                                                           }
                                                                       }
                                                                       """;

    [Fact]
    public void ShouldGenerateStaticValidatorWithNestedStaticValidation()
    {
        AssertGenerator(inputs: [InputRequests, InputCustomerValidator, InputOrderValidator], 
            outputs: [ExpectedCustomerValidator, ExpectedOrderValidator],
            diagnostics: [],
            assertInitialCompilation: true);
    }
}
