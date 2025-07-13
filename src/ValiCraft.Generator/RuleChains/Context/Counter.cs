namespace ValiCraft.Generator.RuleChains.Context;

public class Counter
{
    public Counter(int value)
    {
        Value = value;
    }
    
    public int Value { get; set; }
    
    public override string ToString() => Value.ToString();
    
    public static implicit operator int(Counter counter) => counter.Value;
}