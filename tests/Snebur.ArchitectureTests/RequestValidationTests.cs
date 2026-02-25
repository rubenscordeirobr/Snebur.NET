using Snebur.Application.Abstractions.Handlers;

namespace Snebur.ArchitectureTests;

public class RequestValidationTests
    : IClassFixture<ApplicationAssemblyContext>,
    IClassFixture<ServiceProviderMock<AnonymousRole>>
{
    private readonly IServiceProvider _serviceProvider;
    private readonly IReadOnlyDictionary<Type, Type> _requestTypeToHandlerTypeMap;
    private readonly ITestOutputHelper _testOutput;

    public RequestValidationTests(
        ApplicationAssemblyContext assemblyContext,
        ServiceProviderMock<AnonymousRole> serviceProviderMock,
        ITestOutputHelper output)
    {
        serviceProviderMock.AddTestOutput(output);

        _serviceProvider = serviceProviderMock;

        _requestTypeToHandlerTypeMap = assemblyContext.RequestTypeToHandlerTypeMap;
        _testOutput = output;
    }

    public static IEnumerable<object[]> RequestTypes
    {
        get
        {
            var assemblyContext = new ApplicationAssemblyContext();
            var requestTypes = assemblyContext.RequestTypes;
            return requestTypes.Select(type => new object[] { type });
        }
    }
     
    [Theory]
    [MemberData(nameof(RequestTypes))]
    public void Request_Should_Have_Handler_Registered(Type type)
    {
        // Act
        var handlerType = _requestTypeToHandlerTypeMap.GetValueOrDefault(type);
        var responseType = type.GetGenericArgumentFromInterfaceDefinition(typeof(IRequest<>));
        var handlerServiceType = typeof(IRequestHandler<,>)
             .MakeGenericType(type, responseType);

        var handlerService = _serviceProvider.GetService(handlerServiceType);

        // Assert
        handlerType
            .Should()
            .NotBeNull($"Request {type.Name} should have a RequestHandler.");

        handlerService
            .Should()
            .NotBeNull($"RequestHandler {handlerType!.Name} should be registered.");

        _testOutput.WriteLine($"Request {type.Name} has a registered RequestHandler {handlerType.Name}.");
    }
}

