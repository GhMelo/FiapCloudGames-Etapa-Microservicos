using Application.Interfaces.IService;
using Application.Services;
using Domain.Entity;
using Domain.Interfaces.IRepository;
using FIAP_Cloud_Games.Middlewares;
using Infrastructure.Repository;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;
using System.Text;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Carregar configura��o externa
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
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = context =>
        {
            return Task.CompletedTask;
        },
        OnMessageReceived = context =>
        {
            var endpoint = context.HttpContext.GetEndpoint();
            var allowAnonymous = endpoint?.Metadata.GetMetadata<IAllowAnonymous>() != null;

            if (allowAnonymous)
            {
                context.NoResult();
            }

            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            context.Response.ContentType = "application/json";

            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                var result = JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Seu token expirou."
                });
                return context.Response.WriteAsync(result);
            }
            else
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                var result = JsonSerializer.Serialize(new
                {
                    success = false,
                    message = "Token inválido."
                });
                return context.Response.WriteAsync(result);
            }
        },
        OnForbidden = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";
            var result = JsonSerializer.Serialize(new
            {
                success = false,
                message = "Você não tem permissão para acessar este recurso."
            });

            return context.Response.WriteAsync(result);
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Administrador", policy => policy.RequireRole("Administrador"));
    options.AddPolicy("UsuarioPadrao", policy => policy.RequireRole("UsuarioPadrao", "Administrador"));
});
builder.Services.AddHttpContextAccessor();

#endregion

#region Configura��o dos Controllers e Swagger

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Fiap Cloud Games", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Digite 'Bearer' [espa�o] e depois o token JWT.\n\nExemplo: \"Bearer 12345abcdef\""
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        Array.Empty<string>()
    }
});
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

#region Configura��o do Entity Framework e Banco de Dados

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(configuration.GetConnectionString("ConnectionString"));
    options.UseLazyLoadingProxies();
}, ServiceLifetime.Scoped);

#endregion

#region Registro de Reposit�rios e Servi�os

builder.Services.AddScoped<ICompraRepository, CompraRepository>();
builder.Services.AddScoped<IPagamentoRepository, PagamentoRepository>();

builder.Services.AddScoped<ICompraService, CompraService>();
builder.Services.AddScoped<IPagamentoService, PagamentoService>();
builder.Services.AddHttpClient<ICompraService, CompraService>();

builder.Services.AddScoped<IEventStoreRepository, EventStoreRepository>();
builder.Services.AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>));
#endregion

#region Expondo porta
builder.WebHost.UseUrls("http://*:80");
#endregion

#region Configura��o do Pipeline HTTP

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<DatabaseLoggingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();

#endregion