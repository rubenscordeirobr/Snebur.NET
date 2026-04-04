namespace Snebur.UseCases.Abstractions;

public interface IApiService : ICommunicationService
{
    string GetVersion();
}

