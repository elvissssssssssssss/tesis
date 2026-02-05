using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using tesis.Data;
using tesis.Services;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions
{
    Args = args,
    ContentRootPath = Directory.GetCurrentDirectory()
});

// =========================================================
// 🔥 SOLUCIÓN AL ERROR 139 (INOTIFY LIMIT)
// =========================================================
builder.Configuration.Sources.Clear(); // Limpiamos los watchers automáticos
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false) // <--- DESACTIVADO
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables();
// =========================================================

// 1. Configurar JSON (camelCase vital para Angular/Flutter)
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

// 2. Aumentar límite de subida a 10MB (Para evidencias HD)
builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 10 * 1024 * 1024;
});

// 3. Conexión a MySQL (AlwaysData)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("❌ LA CADENA DE CONEXIÓN NO SE ENCONTRÓ.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
           .EnableSensitiveDataLogging()
           .EnableDetailedErrors());

// 4. Inyección de Dependencias
builder.Services.AddScoped<C2Service>();

// 5. Configurar Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 6. CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// GESTIÓN DE CARPETAS
var webRoot = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
var uploadsPath = Path.Combine(webRoot, "uploads");

if (!Directory.Exists(webRoot)) Directory.CreateDirectory(webRoot);
if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);

// SWAGGER siempre visible
app.UseSwagger();
app.UseSwaggerUI();

app.UseStaticFiles();
app.UseRouting();
app.UseCors("AllowAll");
app.UseAuthorization();
app.MapControllers();

app.Run();