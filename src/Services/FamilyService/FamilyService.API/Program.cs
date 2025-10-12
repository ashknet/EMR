using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using FamilyService.Infrastructure.Data;
using FamilyService.Infrastructure.Repositories;
using FamilyService.Domain.Entities;
using Shared.Common.Interfaces;
using Shared.Security.Authentication;
using Serilog;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.WithProperty("ServiceName", "FamilyService")
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddControllers();

// Configure DbContext
var isDevelopment = builder.Environment.IsDevelopment();
var connectionString = isDevelopment 
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : builder.Configuration.GetConnectionString("ProductionConnection");

builder.Services.AddDbContext<FamilyDbContext>(options =>
    options.UseSqlServer(connectionString, 
        sqlOptions => sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null)));

// Register repositories
builder.Services.AddScoped<IRepository<FamilyGroup>, FamilyGroupRepository>();
builder.Services.AddScoped<FamilyGroupRepository>();

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Configure Authentication
builder.Services.AddJwtAuthentication(builder.Configuration, isDevelopment);

// Configure Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Family Service API",
        Version = "v1",
        Description = "HIPAA-compliant Family Service API for managing family relationships and authorizations",
        Contact = new OpenApiContact
        {
            Name = "Healthcare Platform Team",
            Email = "support@healthcare.local"
        }
    });

    if (!isDevelopment)
    {
        c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = "JWT Authorization header using the Bearer scheme.",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                Array.Empty<string>()
            }
        });
    }
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<FamilyDbContext>("database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Family Service API V1");
        c.RoutePrefix = string.Empty;
    });
}
else
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Family Service API V1");
        c.RoutePrefix = "swagger";
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

Log.Information("Family Service API starting up...");
Log.Information($"Environment: {app.Environment.EnvironmentName}");
Log.Information($"Authentication: {(isDevelopment ? "Disabled (Development)" : "Enabled (Production)")}");

app.Run();
