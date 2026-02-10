using System.Text;
using ValiCraft.Generator.Concepts;
using ValiCraft.Generator.Models;
using ValiCraft.Generator.RuleChains.Context;
using ValiCraft.Generator.Types;

namespace ValiCraft.Generator.RuleChains;

public record PolymorphicRuleChain(
    bool IsAsync,
    ValidationTarget Object,
    ValidationTarget Target,
    int Depth,
    IndentModel Indent,
    OnFailureMode? FailureMode,
    PolymorphicNullBehavior NullBehavior,
    EquatableArray<PolymorphicBranch> Branches,
    PolymorphicOtherwiseBranch? OtherwiseBranch) : RuleChain(IsAsync, Object, Target, Depth, Indent, 1, FailureMode)
{
    public override bool NeedsGotoLabels()
    {
        return false;
    }

    protected override string HandleCodeGeneration(RuleChainContext context)
    {
        var requestName = GetRequestParameterName();
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        var code = new StringBuilder();
        var childIndent = IndentModel.CreateChild(Indent);
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
            
            code.AppendLine($"{Indent}{keyword} ({requestAccessor} is {branch.DerivedType.FormattedTypeName} {typedVarName})");
            code.AppendLine($"{Indent}{{");
            code.Append(GenerateBranchBody(branch, typedVarName, childIndent, doubleChildIndent, context));
            code.AppendLine($"{Indent}}}");
            
            isFirst = false;
        }

        // Generate otherwise branch
        if (OtherwiseBranch is not null)
        {
            code.AppendLine($"{Indent}else");
            code.AppendLine($"{Indent}{{");
            code.Append(GenerateOtherwiseBranchBody(OtherwiseBranch, childIndent, doubleChildIndent, context));
            code.AppendLine($"{Indent}}}");
        }
        else if (NullBehavior == PolymorphicNullBehavior.Skip)
        {
            // If no otherwise and null behavior is skip, we need an else for when type doesn't match
            // but target is not null - default to allow (do nothing)
        }

        context.DecrementCountdown();
        return code.ToString().TrimEnd('\r', '\n');
    }

    private string GenerateNullFailCheck(string requestAccessor, RuleChainContext context)
    {
        var nullMessage = $"{{TargetName}} cannot be null.";
        return $$"""
                 {{Indent}}if ({{requestAccessor}} is null)
                 {{Indent}}{
                 {{Indent}}    (errors ??= []).Add(new global::{{KnownNames.Types.ValidationError}}<{{Target!.Type.FormattedTypeName}}>
                 {{Indent}}    {
                 {{Indent}}        Code = "{{Target!.TargetPath.Value}}IsNull",
                 {{Indent}}        Message = $"{{Target!.DefaultTargetName.Value}} cannot be null.",
                 {{Indent}}        Severity = global::{{KnownNames.Enums.ErrorSeverity}}.Error,
                 {{Indent}}        TargetName = "{{Target!.DefaultTargetName.Value}}",
                 {{Indent}}        TargetPath = $"{inheritedTargetPath}{{Target!.TargetPath.Value}}",
                 {{Indent}}        AttemptedValue = {{requestAccessor}}
                 {{Indent}}    });
                 {{GetGotoLabelIfNeeded(Indent, context)}}{{Indent}}}
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
        var requestAccessor = string.Format(Target!.AccessorExpressionFormat, requestName);
        var message = failMessage?.Value ?? $"{{TargetName}} is not a supported type.";
        var messageExpression = failMessage?.IsLiteral != false 
            ? $"$\"{message.Replace("{TargetName}", Target!.DefaultTargetName.Value)}\"" 
            : message;

        return $$"""
                 {{childIndent}}(errors ??= []).Add(new global::{{KnownNames.Types.ValidationError}}<{{Target!.Type.FormattedTypeName}}>
                 {{childIndent}}{
                 {{childIndent}}    Code = "{{Target!.TargetPath.Value}}UnsupportedType",
                 {{childIndent}}    Message = {{messageExpression}},
                 {{childIndent}}    Severity = global::{{KnownNames.Enums.ErrorSeverity}}.Error,
                 {{childIndent}}    TargetName = "{{Target!.DefaultTargetName.Value}}",
                 {{childIndent}}    TargetPath = $"{inheritedTargetPath}{{Target!.TargetPath.Value}}",
                 {{childIndent}}    AttemptedValue = {{requestAccessor}}
                 {{childIndent}}});
                 {{GetGotoLabelIfNeeded(childIndent, context)}}
                 """;
    }

    private string GenerateValidateWithBranch(
        PolymorphicBranch branch,
        string typedVarName,
        IndentModel childIndent,
        RuleChainContext context)
    {
        string methodCall;
        
        if (branch.IsStaticValidator)
        {
            // Static validator - call ValidateToList or ValidateToListAsync statically
            if (IsAsync && branch.IsAsyncValidatorCall)
            {
                methodCall = $"await {branch.StaticValidatorTypeName}.ValidateToListAsync({typedVarName}, $\"{{inheritedTargetPath}}{Target!.TargetPath.Value}.\", cancellationToken)";
            }
            else
            {
                methodCall = $"{branch.StaticValidatorTypeName}.ValidateToList({typedVarName}, $\"{{inheritedTargetPath}}{Target!.TargetPath.Value}.\")";
            }
        }
        else if (IsAsync && branch.IsAsyncValidatorCall)
        {
            methodCall = $"await {branch.ValidatorExpression}.ValidateToListAsync({typedVarName}, $\"{{inheritedTargetPath}}{Target!.TargetPath.Value}.\", cancellationToken)";
        }
        else
        {
            methodCall = $"{branch.ValidatorExpression}.ValidateToList({typedVarName}, $\"{{inheritedTargetPath}}{Target!.TargetPath.Value}.\")";
        }

        return $$"""
                 {{childIndent}}var errors{{context.Counter}} = {{methodCall}};
                 {{childIndent}}if (errors{{context.Counter}}.Count != 0)
                 {{childIndent}}{
                 {{childIndent}}    if (errors is null)
                 {{childIndent}}    {
                 {{childIndent}}        errors = new(errors{{context.Counter}});
                 {{GetGotoLabelIfNeeded(childIndent, context)}}{{childIndent}}    }
                 {{childIndent}}    else
                 {{childIndent}}    {
                 {{childIndent}}        errors.AddRange(errors{{context.Counter}});
                 {{GetGotoLabelIfNeeded(childIndent, context)}}{{childIndent}}    }
                 {{childIndent}}}

                 """;
    }

    private string GetGotoLabelIfNeeded(IndentModel indent, RuleChainContext context)
    {
        if (context is { ParentFailureMode: OnFailureMode.Halt, HaltLabel: not null })
        {
            return $"""
                    {indent}        goto {context.HaltLabel};

                    """;
        }

        return string.Empty;
    }

    protected override string GetTargetPath(RuleChainContext context)
    {
        return $"{context.TargetPath}{Target!.TargetPath.Value}";
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
