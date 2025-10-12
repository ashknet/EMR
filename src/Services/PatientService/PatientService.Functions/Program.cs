using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using PatientService.Infrastructure.Data;
using Microsoft.Extensions.Logging;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices((context, services) =>
    {
        // Configure Logging
        services.AddLogging(logging =>
        {
            logging.AddConsole();
            logging.AddDebug();
        });

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
