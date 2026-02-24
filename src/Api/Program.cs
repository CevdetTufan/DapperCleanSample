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

// CORS (Cross-Origin Resource Sharing) politikası tanımlanıyor.
// Tarayıcılar, güvenlik nedeniyle farklı origin'lerden (farklı port/domain) gelen API isteklerini varsayılan olarak engeller.
// "AllowUI" adlı bu politika; Vite geliştirme sunucusunun çalıştığı http://localhost:5173 adresinden gelen
// tüm HTTP metodlarına (GET, POST, PUT, DELETE vb.) ve tüm header'lara izin verir.
// Politika, aşağıda app.UseCors("AllowUI") ile middleware pipeline'a eklenerek aktif hale getirilir.
builder.Services.AddCors(options =>
{
	options.AddPolicy("AllowUI", policy =>
	{
		policy.WithOrigins("http://localhost:5173")
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

// Minimal API kullanıldığında derleyici Program sınıfını internal üretir; dış assembly'ler (test projeleri) erişemez.
// "public partial class Program" yazarak sınıfı public yapıyor, partial keyword'ü ise derleyicinin ürettiği
// sınıfla bu tanımı birleştirmesini sağlıyor. Bu sayede integration test projesinde
// WebApplicationFactory<Program> kullanılarak uygulama gerçek bir web sunucusu gibi ayağa kaldırılabilir
// ve testler tüm middleware/veritabanı pipeline'ı dahil gerçek endpoint'lere HTTP isteği atabilir.
public partial class Program { }
