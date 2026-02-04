using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using tesis.Data;
using tesis.Services;

var builder = WebApplication.CreateBuilder(args);

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

// Validar que la cadena no venga vacía antes de arrancar
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

// 6. CORS (¡PUERTAS ABIERTAS PARA TU TESIS!)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()  // Angular, Flutter, Postman, etc.
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// =========================================================
// 📁 GESTIÓN DE CARPETAS (Evita errores si no existen)
// =========================================================
var webRoot = app.Environment.WebRootPath ?? Path.Combine(app.Environment.ContentRootPath, "wwwroot");
var uploadsPath = Path.Combine(webRoot, "uploads");

if (!Directory.Exists(webRoot)) Directory.CreateDirectory(webRoot);
if (!Directory.Exists(uploadsPath)) Directory.CreateDirectory(uploadsPath);
// =========================================================

// 🚀 SWAGGER: Lo dejamos SIEMPRE visible, incluso en Render (Producción)
// Así podrás probar tus endpoints desde el celular sin instalar nada extra.
app.UseSwagger();
app.UseSwaggerUI();

// Servir archivos estáticos (Para que Angular pueda mostrar las fotos con <img src="...">)
app.UseStaticFiles();

app.UseRouting();

// ¡Activar CORS antes de los controladores!
app.UseCors("AllowAll");

app.UseAuthorization();

app.MapControllers();

app.Run();