using LitePrimitives;

namespace ValiCraft;

public interface IValidator<TRequest> where TRequest : class
{
    Validation<TRequest> Validate(TRequest request);
}