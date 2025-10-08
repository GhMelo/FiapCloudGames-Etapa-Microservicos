using Application.Interfaces.IService;
using Application.Services;
using Domain.Entity;
using Domain.Interfaces.IRepository;
using FIAP_Cloud_Games.Middlewares;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Carregar configuração externa
var configuration = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .Build();

#region Configuração de Autenticação e Autorização

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = false,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("UsuarioPadrao", policy => policy.RequireRole("UsuarioPadrao", "Administrador"));
});

#endregion
#region Configuração dos Controllers e Swagger

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Fiap Cloud Games", Version = "v1" });
});

#endregion
#region Configuração do MongoDB

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoSettings"));

builder.Services.AddSingleton<IMongoDatabase>(sp =>
{
    var settings = sp.GetRequiredService<IOptions<MongoSettings>>().Value;
    var client = new MongoClient(settings.ConnectionString);
    return client.GetDatabase(settings.Database);
});

#endregion
#region Configuração do Entity Framework e Banco de Dados

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
    options.UseLazyLoadingProxies();
}, ServiceLifetime.Scoped);

#endregion

#region Registro de Repositórios e Serviços
builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IUsuarioBibliotecaRepository, UsuarioBibliotecaRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

builder.Services.AddScoped<IUsuarioService, UsuarioService>();
builder.Services.AddScoped<IUsuarioBibliotecaService, UsuarioBibliotecaService>();
builder.Services.AddScoped<IAuthService, AuthService>();


builder.Services.AddHttpClient<IUsuarioBibliotecaService, UsuarioBibliotecaService>();
builder.Services.AddHttpClient<IAuthService, AuthService>();
builder.Services.AddHttpClient<IUsuarioService, UsuarioService>();

builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));

#endregion

#region Expondo porta
builder.WebHost.UseUrls("http://*:80");
#endregion

#region Configuração do Pipeline HTTP

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<DatabaseLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion