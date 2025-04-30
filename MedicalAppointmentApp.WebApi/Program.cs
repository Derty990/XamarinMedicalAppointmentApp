using Microsoft.EntityFrameworkCore; // Dodano using dla EF Core
using MedicalAppointmentApp.WebApi.Data;
using System.Text.Json.Serialization; // Dodano using dla DbContext

var builder = WebApplication.CreateBuilder(args);

// --- Dodano konfiguracjê DbContext ---
// Pobierz Connection String z appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// Zarejestruj MedicalDbContext
builder.Services.AddDbContext<MedicalDbContext>(options =>
    options.UseSqlServer(connectionString));
// --- Koniec dodanego fragmentu ---

// Add services to the container. (Reszta Twoich istniej¹cych serwisów)
builder.Services.AddControllers() 
    .AddJsonOptions(options => 
    {
        // Ignoruj cykle podczas serializacji
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();