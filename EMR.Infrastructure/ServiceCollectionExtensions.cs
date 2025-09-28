using EMR.Infrastructure.DataAccess;
using EMR.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EMR.Infrastructure;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=localhost;Database=EMR;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";

        services.AddScoped<IStoredProcedureGateway>(_ => new StoredProcedureGateway(connectionString));
        services.AddScoped<IPatientRepository, PatientRepository>();
        return services;
    }
}

