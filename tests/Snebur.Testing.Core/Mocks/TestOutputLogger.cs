using Microsoft.Extensions.Logging;

namespace Snebur.Testing.Core.Mocks;

public class TestOutputLogger<T> : ILogger<T>
{
    private readonly ITestOutputHelper _testOutput;

    public TestOutputLogger(ITestOutputHelper testOutput)
    {
        _testOutput = testOutput;
    }

    public bool IsEnabled(LogLevel logLevel)
    {
        return false;
    }

    public void Log<TState>(LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
    {
        if (_testOutput is not null)
        {
            Guard.NotNull(state);

            var message = BuildMessage(state, exception, formatter);
            _testOutput.WriteLine(message);
        }
    }

    private string BuildMessage<TState>(TState state, 
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
       
        if (formatter is not null)
        {
            return formatter(state, exception);
        }
        if (exception is not null)
        {
            return $"{state} - Exception: {exception.Message}";
        }
        return state!.ToString() ?? string.Empty;
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
    {
        return null;
    }
}
