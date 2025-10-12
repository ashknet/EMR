using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using PatientService.Infrastructure.Data;
using Serilog;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Configure Serilog
        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(context.Configuration)
            .Enrich.WithProperty("ServiceName", "PatientService.Functions")
            .WriteTo.Console()
            .CreateLogger();

        services.AddApplicationInsightsTelemetryWorkerService();
        services.ConfigureFunctionsApplicationInsights();

        // Configure DbContext
        var connectionString = context.Configuration["SqlConnectionString"];
        services.AddDbContext<PatientDbContext>(options =>
            options.UseSqlServer(connectionString));

        // Add Azure Storage
        services.AddAzureClients(builder =>
        {
            builder.AddBlobServiceClient(context.Configuration["AzureWebJobsStorage"]);
        });
    })
    .Build();

host.Run();
