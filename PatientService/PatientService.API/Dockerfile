# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj files and restore dependencies
COPY ["PatientService/PatientService.API/PatientService.API.csproj", "PatientService/PatientService.API/"]
COPY ["PatientService/PatientService.Domain/PatientService.Domain.csproj", "PatientService/PatientService.Domain/"]
COPY ["PatientService/PatientService.Infrastructure/PatientService.Infrastructure.csproj", "PatientService/PatientService.Infrastructure/"]

RUN dotnet restore "PatientService/PatientService.API/PatientService.API.csproj"

# Copy everything else and build
COPY . .
WORKDIR "/src/PatientService/PatientService.API"
RUN dotnet build "PatientService.API.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "PatientService.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Create non-root user for security
RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
  CMD curl -f http://localhost:80/health || exit 1

ENTRYPOINT ["dotnet", "PatientService.API.dll"]

