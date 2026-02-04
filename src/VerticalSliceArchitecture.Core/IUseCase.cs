namespace VerticalSliceArchitecture.Core;

/// <summary>
/// Marker interface for use cases.
/// Use cases must implement this interface directly (not through inheritance).
/// </summary>
/// <typeparam name="TRequest">The request type for this use case.</typeparam>
/// <typeparam name="TResponse">The response type for this use case.</typeparam>
public interface IUseCase<in TRequest, TResponse>
{
    ValueTask<TResponse> ExecuteAsync(TRequest request, CancellationToken cancellationToken = default);
}
