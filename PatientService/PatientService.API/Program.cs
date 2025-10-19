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
});

// Configure Database
builder.Services.AddDbContext<PatientDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        });
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
var keyVaultUri = builder.Configuration["AzureKeyVault:VaultUri"] 
    ?? throw new InvalidOperationException("Azure Key Vault URI not configured");

builder.Services.AddSingleton<IEncryptionService>(sp => new EncryptionService(keyVaultUri));
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
            "https://localhost:5174"
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
