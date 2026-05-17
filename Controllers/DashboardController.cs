using Microsoft.AspNetCore.Mvc;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Data;
using Microsoft.EntityFrameworkCore;

namespace ComercialMorro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public DashboardController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<DashboardDto>> GetDashboard()
        {
            var hoje = DateTime.Today;

            var vendasHoje = await _context.Vendas
                .AsNoTracking()
                .Where(v => v.DataHora.Date == hoje)
                .ToListAsync();

            var totalFiado = await _context.Clientes
                .AsNoTracking()
                .SumAsync(c => c.TotalFiado);

            var baixoEstoque = await _context.Produtos
                .AsNoTracking()
                .Where(p => p.Qtde < 10)
                .CountAsync();

            var totalClientes = await _context.Clientes
                .AsNoTracking()
                .CountAsync();

            var ultimasVendas = await _context.Vendas
                .AsNoTracking()
                .OrderByDescending(v => v.DataHora)
                .Take(5)
                .Select(v => new VendaSimplesDto
                {
                    IdVenda = v.IdVenda,
                    DataHora = v.DataHora,
                    NomeCliente = "Venda",           // Temporário
                    ValorTotal = v.TotalVenda
                })
                .ToListAsync();

            var dashboard = new DashboardDto
            {
                VendasHoje = new VendasHojeDto
                {
                    QuantidadeVendas = vendasHoje.Count,
                    TotalValor = vendasHoje.Sum(v => v.TotalVenda)
                },
                TotalFiado = totalFiado,
                ProdutosBaixoEstoque = baixoEstoque,
                TotalClientes = totalClientes,
                UltimasVendas = ultimasVendas
            };

            return Ok(dashboard);
        }
    }
}