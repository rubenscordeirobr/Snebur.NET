# Base image for runtime environment
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build image with .NET SDK
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy project files and restore dependencies
WORKDIR /src
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]

# Copy project files and restore dependencies
COPY ["src/Snebur.IdentityApi/Snebur.IdentityApi.csproj", "Snebur.IdentityApi/"]

#Common
COPY ["src/Snebur.Core/Snebur.Core.csproj", "Snebur.Core/"]
#Core
COPY ["src/Snebur.Application/Snebur.Application.csproj", "Snebur.Application/"]
COPY ["src/Snebur.Domain/Snebur.Domain.csproj", "Snebur.Domain/"]
COPY ["src/Snebur.SharedKernel/Snebur.SharedKernel.csproj", "Snebur.SharedKernel/"]

#Use cases
COPY ["src/Snebur.UseCases/Snebur.UseCases.csproj", "Snebur.UseCases/"]
COPY ["src/Snebur.UseCases.Shared/Snebur.UseCases.Shared.csproj", "Snebur.UseCases.Shared/"]

#Persistence
COPY ["src/Snebur.Persistence.Common/Snebur.Persistence.Common.csproj", "Snebur.Persistence.Common/"]
COPY ["src/Snebur.Persistence.Identity/Snebur.Persistence.Identity.csproj", "Snebur.Persistence.Identity/"]
COPY ["src/Snebur.Persistence.Activity/Snebur.Persistence.Activity.csproj", "Snebur.Persistence.Activity/"]

#Presentation
COPY ["src/Snebur.Presentation/Snebur.Presentation.csproj", "Snebur.Presentation/"]

#Infrastructure
COPY ["src/Snebur.Infrastructure/Snebur.Infrastructure.csproj", "Snebur.Infrastructure/"]

#Aspire Defaults
COPY ["src/Snebur.ServiceDefaults/Snebur.ServiceDefaults.csproj", "Snebur.ServiceDefaults/"]


RUN dotnet restore "./Snebur.IdentityApi/Snebur.IdentityApi.csproj"

# Copy source files and build
COPY . .
WORKDIR "src/Snebur.IdentityApi"
RUN dotnet build -c Release -o /app/build

# Publish the app
FROM build AS publish
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "Snebur.IdentityApi.dll"]
