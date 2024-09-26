# Base image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Build stage to compile the project
FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Adjust the path to the .csproj file based on your structure
COPY ["API/API/API.csproj", "API/API/"]
RUN dotnet restore "./API/API/API.csproj"

# Copy the entire API folder to the image
COPY . .

# Build the API
WORKDIR "/src/API/API"
RUN dotnet build "./API.csproj" -c $BUILD_CONFIGURATION -o /app/build

# Publish the API
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./API.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# Final stage for running the application
FROM base AS final
WORKDIR /app

# Copy the published output
COPY --from=publish /app/publish .

# Set environment variable for Firestore credentials
# Adjust the path to the credentials if necessary, based on where you place the credentials.json file in the project
ENV FIRESTORE_CREDENTIALS_PATH=/app/path/to/your/credentials.json

# Start the application
ENTRYPOINT ["dotnet", "API.dll"]
