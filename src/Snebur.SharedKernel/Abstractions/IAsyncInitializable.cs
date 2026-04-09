namespace Snebur.SharedKernel.Abstractions;

public interface IAsyncInitializable
{
    Task InitializeAsync();
}
