FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
RUN apt-get update && apt-get install -y git
EXPOSE 8080
EXPOSE 8081

# This stage is used to build the service project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
# Copy the directory build props file
COPY Directory.Build.props .
# Copy all the csproj files
COPY */*.csproj ./
# Restore all the project nuget packages
RUN for project in $(ls *.csproj); do dotnet restore $project; done
COPY . .

# Set the build configuration argument
ARG BUILD_CONFIGURATION=Release

# Build projects to cache them for other docker images
RUN dotnet build -c $BUILD_CONFIGURATION ./LogicMonitor.Datamart.Cli.csproj --verbosity detailed

WORKDIR "/src/LogicMonitor.Datamart.Cli"
RUN dotnet publish "./LogicMonitor.Datamart.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .
ENTRYPOINT ["dotnet", "LogicMonitor.Datamart.Cli.dll"]