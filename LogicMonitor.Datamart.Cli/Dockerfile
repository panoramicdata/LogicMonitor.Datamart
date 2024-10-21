# Base image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Install Git for Nerdbank.GitVersioning in the build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
RUN apt-get update && apt-get install -y git

# Copy necessary files for proper restore and build caching
COPY Directory.Build.props ./
COPY global.json ./  # This is the correct path assuming global.json is next to Directory.Build.props
COPY */*.csproj ./

# Restore dependencies for all projects
RUN for project in $(ls *.csproj); do dotnet restore $project; done

# Copy the rest of the source code
COPY . ./

# Set the build configuration (default to Release)
ARG BUILD_CONFIGURATION=Release

# Build the project(s)
RUN dotnet build -c $BUILD_CONFIGURATION ./LogicMonitor.Datamart.Cli.csproj --verbosity detailed

# Publish the project (use /app/publish to ensure clean directory)
WORKDIR "/src/LogicMonitor.Datamart.Cli"
RUN dotnet publish "./LogicMonitor.Datamart.Cli.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage: Create the runtime image
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

# Set the entrypoint for the application
ENTRYPOINT ["dotnet", "LogicMonitor.Datamart.Cli.dll"]
