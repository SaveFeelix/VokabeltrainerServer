using System.Net;
using System.Net.Sockets;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MySqlConnector;
using REST_API.Data.Config;
using REST_API.Data.Db;
using REST_API.Utils.Services;

var builder = WebApplication.CreateBuilder(args);

#if DEBUG
JwtSettings jwtSettings = new JwtSettings("12345678910111213", 8);
#else
var token = Environment.GetEnvironmentVariable("JWT_TOKEN") ?? "yourSecureTokenWithSpecificLength";
if (string.IsNullOrEmpty(token))
    return;
if (!long.TryParse(Environment.GetEnvironmentVariable("JWT_EXPIRE"), out var result))
    return;
JwtSettings jwtSettings = new JwtSettings(token, result);
#endif

// Add services to the container.
builder.Services.AddSingleton(jwtSettings);
builder.Services.AddSingleton<JwtService>();

builder.Services.AddDbContext<DataContext>(options =>
{
#if DEBUG
    options.UseInMemoryDatabase("Berufsschule Vokabeltrainer").UseLazyLoadingProxies();
#else
    MySqlConnectionStringBuilder dbBuilder = new()
    {
        Database = "Vokabeltrainer-Berufsschule",
        UserID = "root",
        Port = 3306,
        Server = Environment.GetEnvironmentVariable("DB_HOST"),
        Password = Environment.GetEnvironmentVariable("DB_PASSWORD"),
        ApplicationName = "Berufsschule-Vokabeltrainer",
        Pipelining = true,
        Pooling = true,
        UseCompression = true,
        AllowUserVariables = true,
    };
    options.UseMySql(dbBuilder.ToString(), ServerVersion.AutoDetect(dbBuilder.ToString())).UseLazyLoadingProxies();
#endif
}, ServiceLifetime.Singleton);

builder.Services.AddAuthentication(x =>
{
    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(x =>
{
    x.RequireHttpsMetadata = false;
    x.SaveToken = true;
    x.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Token)),
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    var scheme = new OpenApiSecurityScheme
    {
        BearerFormat = "JWT",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = JwtBearerDefaults.AuthenticationScheme,
        Description = "Put **_ONLY_** your JWT Bearer token on TextBox below!",
        Reference = new OpenApiReference
        {
            Id = JwtBearerDefaults.AuthenticationScheme,
            Type = ReferenceType.SecurityScheme
        }
    };
    c.AddSecurityDefinition(scheme.Reference.Id, scheme);
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        { scheme, Array.Empty<string>() }
    });
});

var app = builder.Build();


var scope = app.Services.CreateScope();
var context = scope.ServiceProvider.GetRequiredService<DataContext>();
await context.Database.EnsureCreatedAsync();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var host = Dns.GetHostEntry(Dns.GetHostName());
var address = host.AddressList.FirstOrDefault(it => it.AddressFamily == AddressFamily.InterNetwork);

app.Run($"http://{address}:5432");