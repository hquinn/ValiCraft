namespace ValiCraft;

public interface IValidationRule<in TTargetValue>
{
    static abstract bool IsValid(TTargetValue targetValue);
}

public interface IValidationRule<in TTargetValue, in TParam1Value>
{
    static abstract bool IsValid(TTargetValue targetValue, TParam1Value param1);
}

public interface IValidationRule<in TTargetValue, in TParam1Value, in TParam2Value>
{
    static abstract bool IsValid(TTargetValue targetValue, TParam1Value param1, TParam2Value param2);
}

public interface IValidationRule<in TTargetValue, in TParam1Value, in TParam2Value, in TParam3Value>
{
    static abstract bool IsValid(
        TTargetValue targetValue,
        TParam1Value param1,
        TParam2Value param2,
        TParam3Value param3);
}

public interface IValidationRule<in TTargetValue, in TParam1Value, in TParam2Value, in TParam3Value, in TParam4Value>
{
    static abstract bool IsValid(
        TTargetValue targetValue,
        TParam1Value param1,
        TParam2Value param2,
        TParam3Value param3,
        TParam4Value param4);
}