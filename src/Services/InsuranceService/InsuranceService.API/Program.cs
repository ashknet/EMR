using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using InsuranceService.Infrastructure.Data;
using Shared.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

var isDevelopment = builder.Environment.IsDevelopment();
var connectionString = isDevelopment 
    ? builder.Configuration.GetConnectionString("DefaultConnection")
    : builder.Configuration.GetConnectionString("ProductionConnection");

builder.Services.AddDbContext<InsuranceDbContext>(options =>
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
        Title = "Insurance Service API",
        Version = "v1",
        Description = "HIPAA-compliant Insurance Service API for managing insurance policies and claims"
    });
});

builder.Services.AddHealthChecks()
    .AddDbContextCheck<InsuranceDbContext>("database");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Insurance Service API V1");
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
