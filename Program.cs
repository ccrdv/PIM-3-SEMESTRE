using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ComercialMorro.API.Helpers;
using ComercialMorro.API.Data;
using ComercialMorro.API.Repositories.Interfaces;
using ComercialMorro.API.Repositories;
using ComercialMorro.API.Services.Interfaces;
using ComercialMorro.API.Services;
using System.IdentityModel.Tokens.Jwt;  
using Microsoft.Extensions.Caching.Distributed;  

var builder = WebApplication.CreateBuilder(args);

// ====================== CONFIGURAÇÕES ======================

// Oracle Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
});

// ====================== CORS ======================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

// ========== CONFIGURAÇÃO DO JWT ==========
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Secret"] ?? "MinhaChaveSuperSecretaComPeloMenos32Caracteres123";
var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.Configure<JwtSettings>(jwtSettings);

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
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ====================== INJEÇÃO DE DEPENDÊNCIAS ======================
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IVendaRepository, VendaRepository>();

builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IVendaService, VendaService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<ComprovanteService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ====================== MIDDLEWARE ======================

app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.UseCors("AllowAll");

// ========== MIDDLEWARE DE REVOGAÇÃO DE TOKEN ==========
app.Use(async (context, next) =>
{
    var cache = context.RequestServices.GetRequiredService<IDistributedCache>();
    var authorizationHeader = context.Request.Headers["Authorization"].FirstOrDefault();
    const string tokenPrefix = "ComercialMorro_token_";

    if (!string.IsNullOrEmpty(authorizationHeader) && authorizationHeader.StartsWith("Bearer "))
    {
        var token = authorizationHeader.Substring("Bearer ".Length).Trim();

        try
        {
            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadJwtToken(token);
            var username = jsonToken.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value;

            if (!string.IsNullOrEmpty(username))
            {
                var cachedToken = await cache.GetStringAsync($"{tokenPrefix}{username}");

                if (string.IsNullOrEmpty(cachedToken))
                {
                    context.Response.StatusCode = 401;
                    await context.Response.WriteAsync("Token revogado. Faça login novamente.");
                    return;
                }
            }
        }
        catch
        {
            // Se falhar ao ler o token, continua
        }
    }

    await next();
});


app.UseHttpsRedirection();
app.MapControllers();
app.UseDefaultFiles();
app.UseStaticFiles();

app.Run();