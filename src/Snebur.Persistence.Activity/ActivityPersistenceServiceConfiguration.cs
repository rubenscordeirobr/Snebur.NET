using Snebur.Application.Abstractions.Persistence.Activities;
using Snebur.Persistence.Activity.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Driver;

namespace Snebur.Persistence.Activity;

public static class ActivityPersistenceServiceConfiguration
{
    public static IServiceCollection AddActivityPersistenceServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<IMongoClient>(provider =>
        {
            var connectionString = configuration.GetConnectionString("ActivityMongoDb");
            // Apply global convention to ignore nulls
            var conventionPack = new ConventionPack
            {
                new IgnoreIfNullConvention(true)
            };
            ConventionRegistry.Register("IgnoreNulls", conventionPack, _ => true);

            return new MongoClient(connectionString);
        });

        services.AddScoped(provider =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            return client.GetDatabase("activityDB");
        });

        services.AddScoped<IActivityRepository, ActivityRepository>();
        return services;
    }
}
