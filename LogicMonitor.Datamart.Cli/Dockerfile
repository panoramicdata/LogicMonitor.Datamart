FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY Directory.Build.props .
COPY */*.csproj ./
RUN for project in $(ls *.csproj); do dotnet restore $project; done
COPY . .

RUN dotnet build -c $BUILD_CONFIGURATION ./LogicMonitor.Datamart.Cli.csproj

WORKDIR "/src/LogicMonitor.Datamart.Cli"

# This stage is used to publish the service project to be copied to the final stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./LogicMonitor.Datamart.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# This stage is used in production or when running from VS in regular mode (Default when not using the Debug configuration)
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "LogicMonitor.Datamart.Cli.dll"]