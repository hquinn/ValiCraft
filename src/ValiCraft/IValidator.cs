using MonadCraft;

namespace ValiCraft;

public interface IValidator<TRequest> where TRequest : class
{
    Result<IReadOnlyList<IValidationError>, TRequest> Validate(TRequest request);
    IReadOnlyList<IValidationError> ValidateToList(TRequest request);
}