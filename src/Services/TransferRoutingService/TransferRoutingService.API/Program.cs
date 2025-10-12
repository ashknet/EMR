using Microsoft.OpenApi.Models;
using Shared.Security.Authentication;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
var isDevelopment = builder.Environment.IsDevelopment();
builder.Services.AddJwtAuthentication(builder.Configuration, isDevelopment);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Transfer & Routing Service API",
        Version = "v1",
        Description = "Secure inter-hospital medical record transfer and routing service"
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transfer & Routing Service API V1");
        c.RoutePrefix = string.Empty;
    });
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
