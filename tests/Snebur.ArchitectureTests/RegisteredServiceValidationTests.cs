namespace Snebur.ArchitectureTests;

public class RegisteredServiceValidationTests
    : IClassFixture<ApplicationServiceProvider>,
      IClassFixture<ClientGatewayServiceProvider>,
      IClassFixture<UnitTestTypeNamesCollection>
{
    private readonly ApplicationServiceProvider _serviceProvider;
    private readonly ClientGatewayServiceProvider _clientServiceProvider;
    private readonly UnitTestTypeNamesCollection _unitTestsTypes;
    private readonly ITestOutputHelper _output;

    public RegisteredServiceValidationTests(
        ApplicationServiceProvider serviceProvider,
        ClientGatewayServiceProvider clientServiceProvider,
        UnitTestTypeNamesCollection unitTestsTypes,
        ITestOutputHelper output)
    {
        serviceProvider.AddTestOutput(output);
        clientServiceProvider.AddTestOutput(output);

        _serviceProvider = serviceProvider;
        _clientServiceProvider = clientServiceProvider;
        _unitTestsTypes = unitTestsTypes;
        _output = output;
    }

    public static IEnumerable<object[]> RegisteredImplementedServiceTypes
    {
        get
        {
            var serviceTypes = new ApplicationServiceCollection().RegisteredImplementedServiceTypes;
            return serviceTypes.Select(type => new object[] { type });
        }
    }

    [Theory]
    [MemberData(nameof(RegisteredImplementedServiceTypes))]
    public void RegisteredServiceType_ShouldHaveUnitTests(Type serviceType)
    {
        //Arrange
        var serviceTestName = $"{serviceType.GetSingleName()}Tests";

        //Act
        var hasTest = _unitTestsTypes.Contains(serviceTestName);

        //Assert
        hasTest.Should()
            .BeTrue($"The service {serviceType.Name} does not have a unit test class {serviceTestName}");

        _output.WriteLine($"Service {serviceType.Name} has a unit test class {serviceTestName}");
    }

    [Theory]
    [MemberData(nameof(RegisteredImplementedServiceTypes))]
    public void VerifyRegisteredService_ShouldHavePublicConstructorWithAllDependencies(Type serviceType)
    {
        ////Arrange
        var constructors = serviceType.GetConstructors()
            .Where(ctor => ctor.IsPublic);

        //Act
        var parameters = constructors.SelectMany(ctor => ctor.GetParameters());

        var parametersWithoutService = parameters.Where(parameter =>
        {
            var service = _serviceProvider.GetService(parameter.ParameterType);
            return service == null;
        });

        //Assert
        constructors.Should().HaveCount(1);

        parametersWithoutService.Should()
            .BeEmpty($"The Service {serviceType.Name} has parameters without service registered. \r\b" +
                     $"The parameter {string.Join(",", parametersWithoutService.Select(p => $"{p.ParameterType.Name} {p.Name}"))}");

        _output.WriteLine($"Service {serviceType.Name} has a public constructor with all dependencies");
    }
     
    public static IEnumerable<object[]> ImplementServiceTypes
    {
        get
        {
            var assemblyContext = new ApplicationAssemblyContext();
            var allTypes = assemblyContext.ImplementServiceTypes;
            return allTypes.Select(type => new object[] { type });
        }
    }

    [Theory]
    [MemberData(nameof(ImplementServiceTypes))]
    public void VerifyApplicationService_ShouldBeRegistered(Type serviceType)
    {
        //Arrange
        var services = _serviceProvider.Services;
        var clientServices = _clientServiceProvider.Services;

        var serviceDescriptor = services
                .FirstOrDefault(x => x.ServiceType == serviceType || x.ImplementationType == serviceType)
            ?? clientServices
                .FirstOrDefault(x => x.ServiceType == serviceType || x.ImplementationType == serviceType);
              
        //Assert
        serviceDescriptor.Should()
            .NotBeNull($"The service {serviceType.Name} should be registered");
    }
}
