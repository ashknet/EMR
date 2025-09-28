using EMR.Infrastructure;
using EMR.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using EMR.Infrastructure.Messaging;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddControllers();
builder.Services.AddCors(options =>
{
	options.AddDefaultPolicy(p => p.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

// EF Core & Repositories (local dev friendly)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? "Server=localhost;Database=EMR;User Id=sa;Password=Your_password123;TrustServerCertificate=True;";
builder.Services.AddDbContext<EMRDbContext>(opt => opt.UseSqlServer(connectionString));
builder.Services.AddScoped<IPatientRepository, PatientRepository>();

// Infrastructure helpers and outbox
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddSingleton<IMessagePublisher, ConsoleMessagePublisher>();
builder.Services.AddHostedService<OutboxDispatcher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
}

app.UseCors();
app.MapControllers();

app.MapGet("/health", () => Results.Ok(new { status = "Healthy" }));

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast =  Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast");

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
