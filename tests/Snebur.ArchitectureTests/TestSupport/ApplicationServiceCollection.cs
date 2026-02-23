using Snebur.Application;
using Snebur.Application.Abstractions.Handlers;
using Snebur.Application.Abstractions.Persistence;
using Snebur.Application.Abstractions.Services;
using Snebur.Infrastructure;
using Snebur.Infrastructure.Helpers;
using Snebur.Persistence.Activity;
using Snebur.Persistence.Identity;
using Snebur.Presentation;
using Snebur.RuntimeServices;
using Snebur.SharedKernel;
using Snebur.UseCases;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Moq;
using StackExchange.Redis;

namespace Snebur.ArchitectureTests.TestSupport ;

public class ApplicationServiceCollection : ServiceCollection
{
    public IReadOnlyList<ServiceDescriptor> ApplicationServices { get; }
    public IReadOnlyList<Type> RegisteredImplementedServiceTypes { get; }

    public ApplicationServiceCollection()
    {
        var configurations = new Dictionary<string, string> {
            { "ConnectionStrings:IdentityPostgresql", "Host=postgress;Port=5432;Database=snebur;Username=snebur_user;Password=Temp@Teste123%$;Include Error Detail=true" },
            { "ConnectionStrings:ActivityMongoDb", "mongodb://snebur_user:Temp%40Teste123%25%24@localhost:27017/?authSource=admin&readPreference=primary&ssl=false&directConnection=true" },
            { "ConnectionStrings:CacheRedis", "redis:6379,password=Temp@Teste123%$" },
            { SecureConfigKeysProvider.PasswordSalt.AppSettingsKey, "fake-salt" },
            { SecureConfigKeysProvider.JwtAuthentication.AppSettingsKey, "fake-jwt-auth-key" },
            { SecureConfigKeysProvider.JwtAudience.AppSettingsKey, "fake-jwt-audience" },
            { SecureConfigKeysProvider.JwtIssuer.AppSettingsKey, "fake-jwt-issuer" }
        };

        var fakeHostEnvironmentMock = new Mock<IHostEnvironment>();
        fakeHostEnvironmentMock.Setup(x => x.EnvironmentName).Returns("Development");
        fakeHostEnvironmentMock.Setup(x => x.ApplicationName).Returns("Snebur");
        fakeHostEnvironmentMock.Setup(x => x.ContentRootPath).Returns(Directory.GetCurrentDirectory());
         
        var fakeConfiguration = new ConfigurationBuilder()
            .AddInMemoryCollection(configurations!)
            .Build();

        var fakeHostEnvironment = fakeHostEnvironmentMock.Object;

        this.AddApplicationServices()
            .AddRuntimeServices()
            .AddSharedKernelServices()
            .AddUserCasesServices()
            .AddUserCasesSharedServices()
            .AddPresentationServices()
            .AddInfrastructureServices(fakeConfiguration, fakeHostEnvironment)
            .AddIdentityPersistenceServices(fakeConfiguration, fakeHostEnvironment)
            .AddActivityPersistenceServices(fakeConfiguration);

        AddMockDependency(fakeConfiguration);
         
        ApplicationServices = RetrieveApplicationServices();
        RegisteredImplementedServiceTypes = GetImplementedServiceTypes();
    }

    private void AddMockDependency(IConfigurationRoot fakeConfiguration)
    {
        this.AddScoped<IHttpContextAccessor, HttpContextAccessorMock>();
        this.AddTransient(typeof(ILogger<>), typeof(TestOutputLogger<>));
        this.AddSingleton<IConfiguration>(fakeConfiguration);

        this.RemoveAll<IConnectionMultiplexer>();
        this.AddSingleton(new Mock<IConnectionMultiplexer>().Object);
        this.AddSingleton(new Mock<IHostEnvironment>().Object);

        this.AddSingleton<ITestOutputHelper, TestOutputProxy>();
    }

    private IReadOnlyList<ServiceDescriptor> RetrieveApplicationServices()
    {
        Type[] targetTypes = [
            typeof(IRepositoryBase<>),
            typeof(IApplicationHandler),
            typeof(IValidator),
            typeof(IValidationService),
            typeof(IApplicationService)
       ];

        return this
            .Where(descriptor => descriptor.IsAssignableTo(targetTypes))
            .ToList();
    }

    private List<Type> GetImplementedServiceTypes()
    {
        return ApplicationServices
           .Select(descriptor => descriptor.ImplementationType ?? descriptor.ServiceType)
           .Select(type => type.IsGenericType ? type.GetGenericTypeDefinition() : type)
           .ToList();
    }
}
