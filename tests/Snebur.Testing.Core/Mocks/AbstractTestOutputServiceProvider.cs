namespace Snebur.Testing.Core.Mocks;

public abstract class AbstractTestOutputServiceProvider  : IServiceProvider
{
    private ITestOutputHelper _currentTestOutput;

    protected abstract IServiceProvider ServiceProvider { get; }

    public object? GetService(Type serviceType)
    {
        if (_currentTestOutput is null)
        {
            throw new InvalidOperationException(
                $"Test output helper is not defined. Add {nameof(ITestOutputHelper)} calling AddTestOutput() before using the service provider.");
        }

        serviceType = NormalizeServiceType(serviceType);
        return ServiceProvider.GetService(serviceType);
    }

    public virtual void AddTestOutput(ITestOutputHelper output)
    {
        if (ReferenceEquals(_currentTestOutput, output))
            return;

        var service = (TestOutputProxy)ServiceProvider.GetRequiredService<ITestOutputHelper>()!;
        service.AddTestOutput(output);
        _currentTestOutput = output;
    }

    protected virtual Type NormalizeServiceType(Type serviceType)
    {
        return serviceType;
    }
}

