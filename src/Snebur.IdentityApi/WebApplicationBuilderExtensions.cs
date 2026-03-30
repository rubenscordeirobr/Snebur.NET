namespace Snebur.IdentityApi;

internal static class WebApplicationBuilderExtensions
{
    internal static IHostApplicationBuilder InitializeEnvironmentSettings(
        this IHostApplicationBuilder builder)
    {
        var baseEnv = builder.Environment.EnvironmentName;
        var subEnv = Environment.GetEnvironmentVariable("ASPNETCORE_SUB_ENVIRONMENT");
        var envConfig = string.IsNullOrEmpty(subEnv) ? baseEnv : $"{baseEnv}.{subEnv}";

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{baseEnv}.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{envConfig}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return builder;
    }

    public static IHostApplicationBuilder ConfigureCors(
         this IHostApplicationBuilder builder)
    {
        builder.Services.AddCors(OptionsBuilderConfigurationExtensions =>
        {
            OptionsBuilderConfigurationExtensions.AddPolicy("all", policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader();
            });
        });
        return builder;
    }
    public static IHostApplicationBuilder AddEssentialServices(
         this IHostApplicationBuilder builder)
    {
#pragma warning disable S125 
        /* builder.AddServiceDefaults(); */
#pragma warning restore S125 
        builder.Services
          .AddHttpContextAccessor()
          .AddOpenApi()
          .AddSwaggerGen();
         
        builder.Services.AddControllers(options =>
        {
            options.Conventions.Add(new RouteTokenTransformerConvention(new KebabCaseTransformer()));
        });
        return builder;
    }
}
