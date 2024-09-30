# Base image for ASP.NET Core
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080 # Expose the HTTP port
EXPOSE 8081 # Expose another port if needed (optional)

# Add a non-root user for security
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY ["API/API/API.csproj", "API/API/"]
RUN dotnet restore "API/API/API.csproj"

# Copy the entire API folder to the image
COPY . .

# Build the API
WORKDIR "/src/API"
RUN dotnet build "API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish stage
FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish the API to the /app/publish folder
RUN dotnet publish "API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage
FROM base AS final
WORKDIR /app

# Copy the published output from the publish stage
COPY --from=publish /app/publish .

# Set environment variable for Firestore credentials (this assumes the credentials file is passed securely)
ENV GOOGLE_APPLICATION_CREDENTIALS=/app/credentials.json

# Add a health check to ensure the app is running
HEALTHCHECK CMD curl --fail http://localhost:8080 || exit 1

# Start the application
ENTRYPOINT ["dotnet", "API.dll"]
