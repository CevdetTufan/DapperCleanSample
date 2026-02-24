using Api.Endpoints;
using Application;
using Infrastructure;
using Infrastructure.Data;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
	?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

builder.Services.AddApplication();
builder.Services.AddInfrastructure(connectionString);

builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowUI", policy =>
	{
		policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
			  .AllowAnyHeader()
			  .AllowAnyMethod();
	});
});

var app = builder.Build();

// Initialize database
var dbInitializer = app.Services.GetRequiredService<DatabaseInitializer>();
await dbInitializer.InitializeAsync();

// Seed fake data (only if database is empty)
var dataSeeder = app.Services.GetRequiredService<DataSeeder>();
var wasSeeded = await dataSeeder.SeedAsync(customerCount: 15, productCount: 25, orderCount: 20);

if (wasSeeded)
{
	app.Logger.LogInformation("Database seeded with fake data.");
}
else
{
	app.Logger.LogInformation("Database already contains data, skipping seed.");
}

if (app.Environment.IsDevelopment())
{
	app.MapOpenApi();
	app.MapScalarApiReference();
}

app.UseCors("AllowUI");

app.UseHttpsRedirection();

app.MapEndpoints();

await app.RunAsync();

// Required for WebApplicationFactory in integration tests
public partial class Program { }
