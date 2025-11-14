using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ReservasNC.Application.Interfaces;
using ReservasNC.Application.Interfaces.Services;
using ReservasNC.Application.Services;
using ReservasNC.Infrastructure.Notifications;
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
// Repositorios y servicios de Restaurante 
// ------------------------
builder.Services.AddScoped<IRestauranteRepository, RestauranteRepository>();
builder.Services.AddScoped<IRestauranteService, RestauranteService>();


//irebase admin (ruta al json)
//FirebaseInitializer.Init(Configuration["Firebase:CredentialPath"]);

builder.Services.AddScoped<IFcmTokenRepository, FcmTokenRepository>();
builder.Services.AddScoped<INotificationService, FcmNotificationService>();

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
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ReservasNC API v1");
    c.RoutePrefix = string.Empty; // <-- hace que Swagger se cargue en la raíz "/"
});

app.UseHttpsRedirection();

app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ReservaHub>("/reservasHub");

app.Run();
