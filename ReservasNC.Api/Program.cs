using Microsoft.EntityFrameworkCore;
using ReservasNC.Infrastructure.DataContexts;

var builder = WebApplication.CreateBuilder(args);

// Detectar ambiente actual
var environment = builder.Environment.EnvironmentName;
string? connectionString;

if (environment == "Development")
{
    connectionString = builder.Configuration.GetConnectionString("SqlServerDev");
}
else if (environment == "Staging")
{
    connectionString = builder.Configuration.GetConnectionString("SqlServerTest");
}
else
{
    connectionString = builder.Configuration.GetConnectionString("SqlServerProd");
}

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException($"No se encontró cadena de conexión para el ambiente {environment}");
}

// Configurar EF Core con SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// CORS (permitir acceso desde cualquier origen)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

// Add services to the container
builder.Services.AddControllers();

// Agregar soporte Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configurar Swagger solo en Development (opcional)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Usar CORS
app.UseCors("AllowAll");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
