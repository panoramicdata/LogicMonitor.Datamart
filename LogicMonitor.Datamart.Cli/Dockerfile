# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install Git for Nerdbank.GitVersioning in the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files and restore as separate layers for build caching
COPY ["LogicMonitor.Datamart/LogicMonitor.Datamart.csproj", "LogicMonitor.Datamart/"]
COPY ["LogicMonitor.Datamart.Cli/LogicMonitor.Datamart.Cli.csproj", "LogicMonitor.Datamart.Cli/"]

# Restore dependencies for the projects
RUN dotnet restore "LogicMonitor.Datamart.Cli/LogicMonitor.Datamart.Cli.csproj"

# Copy the remaining source files
COPY . ./

# Set the build configuration (default to Release)
ARG BUILD_CONFIGURATION=Release

# Build the project(s)
RUN dotnet build -c $BUILD_CONFIGURATION "LogicMonitor.Datamart.Cli/LogicMonitor.Datamart.Cli.csproj" --no-restore

# Publish the project (use /app/publish to ensure clean directory)
WORKDIR "/src/LogicMonitor.Datamart.Cli"
RUN dotnet publish -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false --no-restore

# Final stage: Create the runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the entrypoint for the application
ENTRYPOINT ["dotnet", "LogicMonitor.Datamart.Cli.dll"]
