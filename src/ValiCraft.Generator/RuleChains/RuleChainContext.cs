using ValiCraft.Generator.Models;

namespace ValiCraft.Generator.RuleChains;

public enum IfElseMode
{
    AlwaysIf,
    BeginIfElseIf,
    ElseIf
}

public class RuleChainContext
{
    public RuleChainContext(int counter)
    {
        Counter = counter;
        ParentFailureMode = null;
        HaltLabel = null;
    }

    private RuleChainContext(RuleChainContext parent)
    {
        ParentFailureMode = parent.ParentFailureMode;
        HaltLabel = parent.HaltLabel;
        Counter = parent.Counter;
    }
    
    public int Counter { get; private set; }
    public OnFailureMode? ParentFailureMode { get; private set; }
    public string? HaltLabel { get; private set; }
    public IfElseMode IfElseMode { get; private set; } = IfElseMode.AlwaysIf;

    public RuleChainContext CreateHaltContext(bool needsLabel)
    {
        return new RuleChainContext(this)
        {
            ParentFailureMode = OnFailureMode.Halt,
            // Use the current counter to guarantee a unique label name
            HaltLabel = needsLabel ? $"HaltValidation_{Counter}" : HaltLabel,
            IfElseMode = needsLabel ? IfElseMode.AlwaysIf :  IfElseMode.BeginIfElseIf
        };
    }
    
    public RuleChainContext CreateContinueContext()
    {
        return new RuleChainContext(this)
        {
            ParentFailureMode = OnFailureMode.Continue,
            HaltLabel = null,
            IfElseMode = IfElseMode.AlwaysIf
        };
    }

    public void UpdateIfElseMode()
    {
        if (IfElseMode == IfElseMode.BeginIfElseIf)
        {
            IfElseMode = IfElseMode.ElseIf;
        }
    }

    public void ResetIfElseMode()
    {
        IfElseMode = IfElseMode.AlwaysIf;
    }
    
    public void DecrementCountdown() => Counter--;
}