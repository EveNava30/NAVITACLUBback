using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ReservasNC.Api.Hubs;
using ReservasNC.Application.Interfaces.Services;
using ReservasNC.Application.Services;
using ReservasNC.Domain.Interfaces.Repositories;
using ReservasNC.Domain.Interfaces.Services;
using ReservasNC.Infrastructure.DataContexts;
using ReservasNC.Infrastructure.Persistence;
using ReservasNC.Infrastructure.Services; // <- EmailService
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ------------------------
// Configuración de cadena de conexión
// ------------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// ------------------------
// Inyección de dependencias
// ------------------------
// Repositorios y servicios principales
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();

// Servicio de email (implementación en Infrastructure)
builder.Services.AddScoped<IEmailService, EmailService>();

// Otros repos y servicios (SignalR, reservas, etc.)
builder.Services.AddSignalR();
builder.Services.AddScoped<IReservaRepository, ReservaRepository>();
builder.Services.AddScoped<IReservaService, ReservaService>();

// ------------------------
// Configurar JWT
// ------------------------
var jwtKey = "EstaEsUnaClaveSuperSeguraDe32Caracteres34"; // clave de 32+ caracteres
var keyBytes = Encoding.UTF8.GetBytes(jwtKey);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes)
        };
    });

// ------------------------
// Servicios MVC / Controllers
// ------------------------
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// ------------------------
// Swagger con soporte JWT
// ------------------------
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "ReservasNC API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresa 'Bearer' seguido de un espacio y tu token JWT."
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// ------------------------
// CORS
// ------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader());
});

var app = builder.Build();

// ------------------------
// Middleware
// ------------------------
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection(); // una sola vez

app.UseCors("AllowAll");
app.UseAuthentication(); // Importante: primero autenticación
app.UseAuthorization();

app.MapControllers();

app.MapHub<ReservaHub>("/reservasHub");

app.Run();
