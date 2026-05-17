using Microsoft.EntityFrameworkCore;
using ComercialMorro.API.Data;
using ComercialMorro.API.Repositories.Interfaces;
using ComercialMorro.API.Repositories;
using ComercialMorro.API.Services.Interfaces;
using ComercialMorro.API.Services;

var builder = WebApplication.CreateBuilder(args);

// ====================== CONFIGURAÇÕES ======================

// Oracle Database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));

// Redis
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "ComercialMorro_";
});

// ====================== INJEÇÃO DE DEPENDÊNCIAS ======================

// Repositories
builder.Services.AddScoped<IProdutoRepository, ProdutoRepository>();
builder.Services.AddScoped<IClienteRepository, ClienteRepository>();
builder.Services.AddScoped<ICategoriaRepository, CategoriaRepository>();
builder.Services.AddScoped<IVendaRepository, VendaRepository>();

// Services
builder.Services.AddScoped<IProdutoService, ProdutoService>();
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ICategoriaService, CategoriaService>();
builder.Services.AddScoped<IVendaService, VendaService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ====================== MIDDLEWARE ======================
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.Run();


