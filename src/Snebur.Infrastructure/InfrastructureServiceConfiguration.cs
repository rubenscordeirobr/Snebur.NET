using Snebur.Application.Abstractions.Security;
using Snebur.Infrastructure.Cache;
using Snebur.Infrastructure.Configurations;
using Snebur.Infrastructure.Factories;
using Snebur.Infrastructure.Services;
using Snebur.SharedKernel.Abstractions;
using Snebur.SharedKernel.Localization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace Snebur.Infrastructure;

public static class InfrastructureServiceConfiguration
{
    public static IServiceCollection AddInfrastructureServices(
        this IServiceCollection services,
        IConfiguration configuration,
        IHostEnvironment hostEnvironment)
    {
        var localizationConfiguration = JsonLocalizationConfigurationFactory.Create(configuration, hostEnvironment);

        services.AddSingleton<ISecureConfiguration, SecureConfiguration>()
            .AddSingleton<ICacheRepository, CacheRepository>()
            .AddSingleton<IFileService, FileService>()
            .AddSingleton<JsonLocalizationCacheConfiguration>(localizationConfiguration)
            .AddSingleton(localizationConfiguration);
            

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var redisConnection = configuration.GetConnectionString("CacheRedis");
            return ConnectionMultiplexer.Connect(redisConnection!);
        });
        services.AddTransient<IEmailSender, EmailSender>();

        return services;
    }
}
