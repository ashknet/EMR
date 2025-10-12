using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using ConsentAuditService.Infrastructure.Data;
using Shared.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var isDevelopment = builder.Environment.IsDevelopment();
var connectionString = isDevelopment 
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : builder.Configuration.GetConnectionString("ProductionConnection");

builder.Services.AddDbContext<ConsentAuditDbContext>(options =>
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
        Title = "Consent & Audit Service API",
        Version = "v1",
        Description = "HIPAA-compliant Consent and Audit Service API for managing patient consents, QR code sharing, and comprehensive audit logging"
    });
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<ConsentAuditDbContext>("database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Consent & Audit Service API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
