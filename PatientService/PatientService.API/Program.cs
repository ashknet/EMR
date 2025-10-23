using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using PatientService.Infrastructure.Data;
using PatientService.Infrastructure.Services;
using Serilog;
//using Microsoft.Extensions.Diagnostics.HealthChecks; // Add this using
//using HealthChecks.EntityFrameworkCore; // Add this using

var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT authentication
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Patient Service API",
        Version = "v1",
        Description = "HIPAA-compliant patient management microservice with FHIR R4 support",
        Contact = new OpenApiContact
        {
            Name = "Healthcare Platform Team",
            Email = "support@healthcare-platform.com"
        }
    });

    // Include XML comments if available
    var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
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

    // Ignore obsolete properties to prevent Swagger generation errors
    c.IgnoreObsoleteProperties();
    c.IgnoreObsoleteActions();
});

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    Log.Error("Database connection string is not configured");
    throw new InvalidOperationException("Database connection string is required");
}

builder.Services.AddDbContext<PatientDbContext>(options =>
{
    options.UseSqlServer(connectionString,
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
    
    // Enable sensitive data logging in development
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging();
        options.EnableDetailedErrors();
    }
});

// Configure Authentication - Conditional for Development
var isDevelopment = builder.Environment.IsDevelopment();

//if (!isDevelopment)
//{
//    // Production: Use Azure AD authentication
//    builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//        .AddMicrosoftIdentityWebApi(builder.Configuration.GetSection("AzureAd"));
//}
//else
//{
    // Development: No authentication required
    Log.Warning("Running in Development mode - Authentication is DISABLED");
//}

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdmin", policy => policy.RequireRole("Admin"));
});

// Register application services
var keyVaultUri = builder.Configuration["AzureKeyVault:VaultUri"];

// Only register encryption service if Key Vault is configured
if (!string.IsNullOrEmpty(keyVaultUri))
{
    builder.Services.AddSingleton<IEncryptionService>(sp => new EncryptionService(keyVaultUri));
}
else
{
    // Development fallback - use a mock encryption service
    Log.Warning("Azure Key Vault not configured - using mock encryption service for development");
    builder.Services.AddSingleton<IEncryptionService>(sp => new MockEncryptionService());
}
builder.Services.AddScoped<IFhirService, FhirService>();
builder.Services.AddScoped<IPatientDataService, PatientDataService>(); // ADO.NET for high-performance operations
builder.Services.AddAutoMapper(typeof(Program));

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowPatientPortal", policy =>
    {
        policy.WithOrigins(
            builder.Configuration["AllowedOrigins:PatientPortal"] ?? "http://localhost:5173",
            "https://localhost:5173",
            "http://localhost:5174",
            "https://localhost:5174",
            "https://emr-sage.vercel.app"
        )
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

// Health checks
//builder.Services.AddHealthChecks()
//    .AddDbContext<PatientDbContext>(options =>
//    {
//        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//    });

var app = builder.Build();

// Configure the HTTP request pipeline
//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Patient Service API v1");
        c.RoutePrefix = "swagger";
    });
//}

app.UseHttpsRedirection();

app.UseCors("AllowPatientPortal");

//if (!isDevelopment)
//{
//    app.UseAuthentication();
//}

app.UseAuthorization();

app.MapControllers();

//app.MapHealthChecks("/health");

app.Run();

// Make Program accessible to test project
public partial class Program { }
