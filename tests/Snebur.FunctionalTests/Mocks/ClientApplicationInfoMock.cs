using Snebur.SharedKernel.Abstractions;

namespace Snebur.FunctionalTests.Mocks;

public class ClientApplicationInfoMock : IApplicationInfo
{
    public string ApplicationName 
        => "Snebur.Test";

    public Version ApplicationVersion 
        => new Version(1, 0, 0);

    public string Environment
        => "Test";

    public string Title 
        => "Snebur Test Application";
}
