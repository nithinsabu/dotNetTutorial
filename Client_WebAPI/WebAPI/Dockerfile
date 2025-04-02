# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy the solution and project files
COPY WebAPI.sln ./
COPY WebAPI/WebAPI.csproj WebAPI/
RUN dotnet restore WebAPI/WebAPI.csproj

# Copy the rest of the project and build it
COPY WebAPI/ WebAPI/
RUN dotnet publish WebAPI/WebAPI.csproj -c Release -o out

# Stage 2: Run
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out ./

# Expose backend port
EXPOSE 5000
EXPOSE 8080

# Run the application
CMD ["dotnet", "WebAPI.dll"]
