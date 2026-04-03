using System.Text;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record PolymorphicRuleChain(
    RuleChainConfig Config,
    PolymorphicNullBehavior NullBehavior,
    EquatableArray<PolymorphicBranch> Branches,
    PolymorphicOtherwiseBranch? OtherwiseBranch) : RuleChain(Config)
{
    protected override string GetTargetPath(RuleChainContext context)
    {
        return $"{context.TargetPath}{Config.Target!.TargetPath.Value}";
    }

    public override bool NeedsGotoLabels()
    {
        // The internal if/else type-switch breaks any halt-level if/else chain, so goto labels are required
        return true;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Config.Target!.AccessorExpressionFormat, requestName);
        var code = new StringBuilder();
        var childIndent = IndentModel.CreateChild(Config.Indent);
        var doubleChildIndent = IndentModel.CreateChild(childIndent);

        // Generate null check if needed
        if (NullBehavior == PolymorphicNullBehavior.Fail)
        {
            code.AppendLine(GenerateNullFailCheck(requestAccessor, context));
            code.AppendLine();
        }

        // Generate type switch using if-else chain with pattern matching
        var isFirst = true;
        foreach (var branch in Branches)
        {
            var keyword = isFirst ? "if" : "else if";
            var simpleTypeName = GetSimpleTypeName(branch.DerivedType.PureTypeName);
            var typedVarName = $"typed{simpleTypeName}";
            
            code.AppendLine($"{Config.Indent}{keyword} ({requestAccessor} is {branch.DerivedType.FormattedTypeName} {typedVarName})");
            code.AppendLine($"{Config.Indent}{{");
            code.Append(GenerateBranchBody(branch, typedVarName, childIndent, doubleChildIndent, context));
            code.AppendLine($"{Config.Indent}}}");

            isFirst = false;
        }

        // Generate otherwise branch
        if (OtherwiseBranch is not null)
        {
            code.AppendLine($"{Config.Indent}else");
            code.AppendLine($"{Config.Indent}{{");
            code.Append(GenerateOtherwiseBranchBody(OtherwiseBranch, childIndent, doubleChildIndent, context));
            code.AppendLine($"{Config.Indent}}}");
        }
        else if (NullBehavior == PolymorphicNullBehavior.Skip)
        {
            // If no otherwise and null behavior is skip, we need an else for when type doesn't match
            // but target is not null - default to allow (do nothing)
        }

        context.DecrementCountdown();
        // Reset because the polymorphic if/else type-switch breaks any halt-level if/else chain
        context.ResetIfElseMode();
        return code.ToString().TrimEnd('\r', '\n');
    }

    private string GenerateNullFailCheck(string requestAccessor, RuleChainContext context)
    {
        var nullMessage = $"{{TargetName}} cannot be null.";
        return $$"""
                 {{Config.Indent}}if ({{requestAccessor}} is null)
                 {{Config.Indent}}{
                 {{Config.Indent}}    (errors ??= []).Add(new global::{{KnownNames.Types.ValidationError}}
                 {{Config.Indent}}    {
                 {{Config.Indent}}        Code = "{{Config.Target!.TargetPath.Value}}IsNull",
                 {{Config.Indent}}        Message = $"{{Config.Target!.DefaultTargetName.Value}} cannot be null.",
                 {{Config.Indent}}        Severity = global::{{KnownNames.Enums.ErrorSeverity}}.Error,
                 {{Config.Indent}}        TargetName = "{{Config.Target!.DefaultTargetName.Value}}",
                 {{Config.Indent}}        TargetPath = $"{inheritedTargetPath}{{Config.Target!.TargetPath.Value}}",
                 {{Config.Indent}}        AttemptedValue = {{requestAccessor}}
                 {{Config.Indent}}    });
                 {{GetValidatorGotoLabelIfNeeded(Config.Indent, context)}}{{Config.Indent}}}
                 """;
    }

    private string GenerateBranchBody(
        PolymorphicBranch branch,
        string typedVarName,
        IndentModel childIndent,
        IndentModel doubleChildIndent,
        RuleChainContext context)
    {
        return branch.Behavior switch
        {
            PolymorphicBranchBehavior.Allow => $"{childIndent}// Allow - no validation needed\r\n",
            PolymorphicBranchBehavior.Fail => GenerateFailBranch(branch.FailMessage, childIndent, doubleChildIndent, context),
            PolymorphicBranchBehavior.ValidateWith => GenerateValidateWithBranch(branch, typedVarName, childIndent, context),
            _ => string.Empty
        };
    }

    private string GenerateOtherwiseBranchBody(
        PolymorphicOtherwiseBranch branch,
        IndentModel childIndent,
        IndentModel doubleChildIndent,
        RuleChainContext context)
    {
        return branch.Behavior switch
        {
            PolymorphicBranchBehavior.Allow => $"{childIndent}// Allow - no validation needed\r\n",
            PolymorphicBranchBehavior.Fail => GenerateFailBranch(branch.FailMessage, childIndent, doubleChildIndent, context),
            _ => string.Empty
        };
    }

    private string GenerateFailBranch(
        MessageInfo? failMessage,
        IndentModel childIndent,
        IndentModel doubleChildIndent,
        RuleChainContext context)
    {
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Config.Target!.AccessorExpressionFormat, requestName);
        var message = failMessage?.Value ?? $"{{TargetName}} is not a supported type.";
        var messageExpression = failMessage?.IsLiteral != false 
            ? $"$\"{message.Replace("{TargetName}", Config.Target!.DefaultTargetName.Value)}\"" 
            : message;

        return $$"""
                 {{childIndent}}(errors ??= []).Add(new global::{{KnownNames.Types.ValidationError}}
                 {{childIndent}}{
                 {{childIndent}}    Code = "{{Config.Target!.TargetPath.Value}}UnsupportedType",
                 {{childIndent}}    Message = {{messageExpression}},
                 {{childIndent}}    Severity = global::{{KnownNames.Enums.ErrorSeverity}}.Error,
                 {{childIndent}}    TargetName = "{{Config.Target!.DefaultTargetName.Value}}",
                 {{childIndent}}    TargetPath = $"{inheritedTargetPath}{{Config.Target!.TargetPath.Value}}",
                 {{childIndent}}    AttemptedValue = {{requestAccessor}}
                 {{childIndent}}});
                 {{GetValidatorGotoLabelIfNeeded(childIndent, context)}}
                 """;
    }

    private string GenerateValidateWithBranch(
        PolymorphicBranch branch,
        string typedVarName,
        IndentModel childIndent,
        RuleChainContext context)
    {
        var callTarget = (branch.IsStaticValidator ? branch.StaticValidatorTypeName : branch.ValidatorExpression)!;
        var methodCall = BuildValidatorMethodCall(Config.IsAsync, branch.IsAsyncValidatorCall, callTarget, typedVarName, Config.Target!.TargetPath.Value);

        return GenerateValidatorCallCode(childIndent, methodCall, context) + "\r\n";
    }

    private static string GetSimpleTypeName(string fullyQualifiedName)
    {
        // Remove global:: prefix if present
        var name = fullyQualifiedName;
        if (name.StartsWith("global::"))
        {
            name = name.Substring(8);
        }

        // Get the last part after the last dot
        var lastDotIndex = name.LastIndexOf('.');
        return lastDotIndex >= 0 ? name.Substring(lastDotIndex + 1) : name;
    }
}
