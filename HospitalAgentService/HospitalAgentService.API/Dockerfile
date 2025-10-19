FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY ["HospitalAgentService/HospitalAgentService.API/HospitalAgentService.API.csproj", "HospitalAgentService/HospitalAgentService.API/"]
RUN dotnet restore "HospitalAgentService/HospitalAgentService.API/HospitalAgentService.API.csproj"

COPY . .
WORKDIR "/src/HospitalAgentService/HospitalAgentService.API"
RUN dotnet build "HospitalAgentService.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "HospitalAgentService.API.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
EXPOSE 80

RUN adduser --disabled-password --gecos '' appuser && chown -R appuser /app
USER appuser

COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "HospitalAgentService.API.dll"]

