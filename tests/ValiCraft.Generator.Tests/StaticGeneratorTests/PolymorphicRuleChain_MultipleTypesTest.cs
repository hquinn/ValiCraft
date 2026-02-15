using System.Diagnostics.CodeAnalysis;
using ValiCraft.Generator.Tests.Helpers;

namespace ValiCraft.Generator.Tests.StaticGeneratorTests;

public class PolymorphicRuleChain_MultipleTypesTest : IncrementalGeneratorTestBase<ValiCraftGenerator>
{
    [StringSyntax("CSharp")] private const string InputRequests = """
                                                             using System;
                                                             using System.Collections.Generic;

                                                             namespace Test.Requests;

                                                             public abstract class Payment
                                                             {
                                                                 public decimal Amount { get; set; }
                                                             }
                                                             
                                                             public class CreditCardPayment : Payment
                                                             {
                                                                 public required string CardNumber { get; set; }
                                                             }
                                                             
                                                             public class CryptoPayment : Payment
                                                             {
                                                                 public required string WalletAddress { get; set; }
                                                             }
                                                             
                                                             public class PayPalPayment : Payment
                                                             {
                                                                 public required string Email { get; set; }
                                                             }
                                                             
                                                             public class Order
                                                             {
                                                                 public required string OrderNumber { get; set; }
                                                                 public Payment? Payment { get; set; }
                                                             }
                                                             """;

    [StringSyntax("CSharp")] private const string InputCreditCardValidator = """
                                                                         using Test.Requests;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.BuilderTypes;
                                                                         using ErrorCraft;

                                                                         namespace Test.Validators;

                                                                         [GenerateValidator]
                                                                         public partial class CreditCardPaymentValidator : StaticValidator<CreditCardPayment>
                                                                         {
                                                                             protected override void DefineRules(IValidationRuleBuilder<CreditCardPayment> builder)
                                                                             {
                                                                                 builder.Ensure(x => x.CardNumber)
                                                                                    .IsNotNullOrWhiteSpace();
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string InputCryptoValidator = """
                                                                         using Test.Requests;
                                                                         using ValiCraft;
                                                                         using ValiCraft.Attributes;
                                                                         using ValiCraft.BuilderTypes;
                                                                         using ErrorCraft;

                                                                         namespace Test.Validators;

                                                                         [GenerateValidator]
                                                                         public partial class CryptoPaymentValidator : StaticValidator<CryptoPayment>
                                                                         {
                                                                             protected override void DefineRules(IValidationRuleBuilder<CryptoPayment> builder)
                                                                             {
                                                                                 builder.Ensure(x => x.WalletAddress)
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
                                                                             protected override void DefineRules(IValidationRuleBuilder<Order> builder)
                                                                             {
                                                                                 builder.Polymorphic(x => x.Payment)
                                                                                    .WhenType<CreditCardPayment>().Validate<CreditCardPaymentValidator>()
                                                                                    .WhenType<CryptoPayment>().Validate<CryptoPaymentValidator>()
                                                                                    .WhenType<PayPalPayment>().Allow()
                                                                                    .Otherwise().Allow();
                                                                             }
                                                                         }
                                                                         """;

    [StringSyntax("CSharp")] private const string ExpectedCreditCardValidator = """
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
                                                                           /// Generated static validator for <see cref="global::Test.Requests.CreditCardPayment"/>.
                                                                           /// </summary>
                                                                           public partial class CreditCardPaymentValidator : global::ValiCraft.IStaticValidator<global::Test.Requests.CreditCardPayment>
                                                                           {
                                                                               /// <inheritdoc />
                                                                               public static global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.CreditCardPayment> Validate(global::Test.Requests.CreditCardPayment request)
                                                                               {
                                                                                   var errors = RunValidationLogic(request, null);

                                                                                   return errors is not null
                                                                                       ? global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.CreditCardPayment>.Failure(new global::ErrorCraft.ValidationErrors
                                                                                       {
                                                                                           Code = "CreditCardPaymentErrors",
                                                                                           Message = "One or more validation errors occurred.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           Errors = errors
                                                                                       })
                                                                                       : global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.CreditCardPayment>.Success(request);
                                                                               }

                                                                               /// <inheritdoc />
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.CreditCardPayment request)
                                                                               {
                                                                                   return RunValidationLogic(request, null) ?? [];
                                                                               }

                                                                               /// <inheritdoc />
                                                                               [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.CreditCardPayment request, string? inheritedTargetPath)
                                                                               {
                                                                                   return RunValidationLogic(request, inheritedTargetPath) ?? [];
                                                                               }

                                                                               private static global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? RunValidationLogic(global::Test.Requests.CreditCardPayment request, string? inheritedTargetPath)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;

                                                                                   if (!global::ValiCraft.Rules.NotNullOrWhiteSpace(request.CardNumber))
                                                                                   {
                                                                                       errors ??= new(1);
                                                                                       errors.Add(new global::ErrorCraft.ValidationError<string>
                                                                                       {
                                                                                           Code = nameof(global::ValiCraft.Rules.NotNullOrWhiteSpace),
                                                                                           Message = $"Card Number must not be null or contain only whitespace.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           TargetName = "Card Number",
                                                                                           TargetPath = $"{inheritedTargetPath}CardNumber",
                                                                                           AttemptedValue = request.CardNumber,
                                                                                       });
                                                                                   }

                                                                                   return errors;
                                                                               }
                                                                           }
                                                                       }
                                                                       """;

    [StringSyntax("CSharp")] private const string ExpectedCryptoValidator = """
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
                                                                           /// Generated static validator for <see cref="global::Test.Requests.CryptoPayment"/>.
                                                                           /// </summary>
                                                                           public partial class CryptoPaymentValidator : global::ValiCraft.IStaticValidator<global::Test.Requests.CryptoPayment>
                                                                           {
                                                                               /// <inheritdoc />
                                                                               public static global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.CryptoPayment> Validate(global::Test.Requests.CryptoPayment request)
                                                                               {
                                                                                   var errors = RunValidationLogic(request, null);

                                                                                   return errors is not null
                                                                                       ? global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.CryptoPayment>.Failure(new global::ErrorCraft.ValidationErrors
                                                                                       {
                                                                                           Code = "CryptoPaymentErrors",
                                                                                           Message = "One or more validation errors occurred.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           Errors = errors
                                                                                       })
                                                                                       : global::MonadCraft.Result<global::ErrorCraft.IValidationErrors, global::Test.Requests.CryptoPayment>.Success(request);
                                                                               }

                                                                               /// <inheritdoc />
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.CryptoPayment request)
                                                                               {
                                                                                   return RunValidationLogic(request, null) ?? [];
                                                                               }

                                                                               /// <inheritdoc />
                                                                               [global::System.ComponentModel.EditorBrowsable(global::System.ComponentModel.EditorBrowsableState.Never)]
                                                                               public static global::System.Collections.Generic.IReadOnlyList<global::ErrorCraft.IValidationError> ValidateToList(global::Test.Requests.CryptoPayment request, string? inheritedTargetPath)
                                                                               {
                                                                                   return RunValidationLogic(request, inheritedTargetPath) ?? [];
                                                                               }

                                                                               private static global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? RunValidationLogic(global::Test.Requests.CryptoPayment request, string? inheritedTargetPath)
                                                                               {
                                                                                   global::System.Collections.Generic.List<global::ErrorCraft.IValidationError>? errors = null;

                                                                                   if (!global::ValiCraft.Rules.NotNullOrWhiteSpace(request.WalletAddress))
                                                                                   {
                                                                                       errors ??= new(1);
                                                                                       errors.Add(new global::ErrorCraft.ValidationError<string>
                                                                                       {
                                                                                           Code = nameof(global::ValiCraft.Rules.NotNullOrWhiteSpace),
                                                                                           Message = $"Wallet Address must not be null or contain only whitespace.",
                                                                                           Severity = global::ErrorCraft.ErrorSeverity.Error,
                                                                                           TargetName = "Wallet Address",
                                                                                           TargetPath = $"{inheritedTargetPath}WalletAddress",
                                                                                           AttemptedValue = request.WalletAddress,
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
                                                                       
                                                                                   if (request.Payment is global::Test.Requests.CreditCardPayment typedCreditCardPayment)
                                                                                   {
                                                                                       var errors1 = global::Test.Validators.CreditCardPaymentValidator.ValidateToList(typedCreditCardPayment, $"{inheritedTargetPath}Payment.");
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
                                                                                   }
                                                                                   else if (request.Payment is global::Test.Requests.CryptoPayment typedCryptoPayment)
                                                                                   {
                                                                                       var errors1 = global::Test.Validators.CryptoPaymentValidator.ValidateToList(typedCryptoPayment, $"{inheritedTargetPath}Payment.");
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
                                                                                   }
                                                                                   else if (request.Payment is global::Test.Requests.PayPalPayment typedPayPalPayment)
                                                                                   {
                                                                                       // Allow - no validation needed
                                                                                   }
                                                                                   else
                                                                                   {
                                                                                       // Allow - no validation needed
                                                                                   }

                                                                                   return errors;
                                                                               }
                                                                           }
                                                                       }
                                                                       """;

    [Fact]
    public void ShouldGenerateStaticValidator()
    {
        AssertGenerator(inputs: [InputRequests, InputCreditCardValidator, InputCryptoValidator, InputOrderValidator], 
            outputs: [ExpectedCreditCardValidator, ExpectedCryptoValidator, ExpectedOrderValidator],
            diagnostics: [],
            assertInitialCompilation: true);
    }
}
