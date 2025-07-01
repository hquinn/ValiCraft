using LitePrimitives;

namespace ValiCraft.Providers.LitePrimitives;

public interface IValidator<TRequest> where TRequest : class
{
    Validation<TRequest> Validate(TRequest request);
}