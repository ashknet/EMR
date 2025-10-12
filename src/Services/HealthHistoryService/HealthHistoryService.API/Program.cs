using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using HealthHistoryService.Infrastructure.Data;
using Shared.Security.Authentication;
using Microsoft.Extensions.Logging;

var builder = WebApplication.CreateBuilder(args);

builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

builder.Services.AddControllers();

var isDevelopment = builder.Environment.IsDevelopment();
var connectionString = isDevelopment 
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : builder.Configuration.GetConnectionString("ProductionConnection");

builder.Services.AddDbContext<HealthHistoryDbContext>(options =>
    options.UseSqlServer(connectionString, 
        sqlOptions => sqlOptions.EnableRetryOnFailure()));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
    });
});

builder.Services.AddJwtAuthentication(builder.Configuration, isDevelopment);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Health History Service API",
        Version = "v1",
        Description = "FHIR-compliant Health History Service API for managing medical conditions, allergies, medications, and immunizations"
    });
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<HealthHistoryDbContext>("database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Health History Service API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

var logger = app.Services.GetRequiredService<ILogger<Program>>();
logger.LogInformation("Health History Service API starting up...");
app.Run();
