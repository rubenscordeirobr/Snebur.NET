using Snebur.Core.Extensions;
using Snebur.SharedKernel;

var builder = WebApplication.CreateBuilder(args);
var environment = builder.Environment;

builder.InitializeEnvironmentSettings()
    .ConfigureCors()
    .AddEssentialServices();

var configuration = builder.Configuration;
if (!environment.IsTest())
{
    builder.Services
        .AddInfrastructureServices(configuration, environment)
        .AddIdentityPersistenceServices(configuration, environment)
        .AddActivityPersistenceServices(configuration);

    //if (environment.IsAspire())
    //{
    //    builder.AddServiceDefaults();
    //}
}

builder.Services
    .AddSharedKernelServices()
    .AddApplicationServices()
    .AddRuntimeServices()
    .AddUserCasesSharedServices()
    .AddUserCasesServices()
    .AddPresentationServices();

var app = builder.Build();

app.UseHttpsRedirection()
   .UseAuthorization();

app.UsePresentationServices();

app.MapPresentationEndPoints();
app.MapFallback();
app.MapPing();

//if (environment.IsAspire())
//{
//    app.MapDefaultEndpoints();
//}

if (environment.IsDevelopment() && !environment.IsTest())
{
    app.MapOpenApi();

    app.UseSwagger()
       .UseSwaggerUI();

    await app.ApplyMigrationsAsync();
}
await app.RunAsync();
