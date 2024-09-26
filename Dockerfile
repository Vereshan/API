# See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

# Depending on the operating system of the host machines(s) that will build or run the containers, the image specified in the FROM statement may need to be changed.
# For more information, please see https://aka.ms/containercompat

FROM mcr.microsoft.com/dotnet/aspnet:8.0-nanoserver-1809 AS base
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0-nanoserver-1809 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy the csproj file and restore dependencies
COPY ["API/API.csproj", "API/"]
RUN dotnet restore "./API/API.csproj"

# Copy the entire API folder to the image
COPY . .

# Build the API
WORKDIR "/src/API"
RUN dotnet build "./API.csproj" -c %BUILD_CONFIGURATION% -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release

# Publish the API
RUN dotnet publish "./API.csproj" -c %BUILD_CONFIGURATION% -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app

# Copy the published output
COPY --from=publish /app/publish .

# Set environment variable for Firestore credentials
ENV FIRESTORE_CREDENTIALS_PATH=/app/path/to/your/credentials.json

# Start the application
ENTRYPOINT ["dotnet", "API.dll"]
