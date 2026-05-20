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
            var amanha = hoje.AddDays(1);

            // 1. Vendas do dia
            var vendasDoDia = await _context.Vendas
                .Where(v => v.DataHora >= hoje && v.DataHora < amanha)
                .ToListAsync();

            decimal totalVendasDia = 0;
            decimal totalVendasFiadasHoje = 0;

            if (vendasDoDia.Any())
            {
                totalVendasDia = vendasDoDia.Sum(v => v.TotalVenda);
                totalVendasFiadasHoje = vendasDoDia.Where(v => v.ClienteIdCliente != null).Sum(v => v.TotalVenda);
            }

            // 2. Total Fiado
            decimal totalFiado = await _context.Clientes.SumAsync(c => c.TotalFiado);

            // 3. Baixo Estoque
            int baixoEstoque = await _context.Produtos.CountAsync(p => p.Qtde < 10);

            // 4. Total Clientes
            int totalClientes = await _context.Clientes.CountAsync();

            // 5. Últimas vendas
            var ultimasVendas = new List<VendaSimplesDto>();

            var ultimasVendasQuery = await _context.Vendas
                .Include(v => v.Cliente)
                    .ThenInclude(c => c!.Pessoa)
                .OrderByDescending(v => v.DataHora)
                .Take(5)
                .ToListAsync();

            foreach (var venda in ultimasVendasQuery)
            {
                string nomeCliente = "Venda à Vista";
                if (venda.Cliente != null && venda.Cliente.Pessoa != null)
                {
                    nomeCliente = venda.Cliente.Pessoa.Nome;
                }

                ultimasVendas.Add(new VendaSimplesDto
                {
                    IdVenda = venda.IdVenda,
                    DataHora = venda.DataHora,
                    NomeCliente = nomeCliente,
                    ValorTotal = venda.TotalVenda
                });
            }

            var dashboard = new DashboardDto
            {
                VendasHoje = new VendasHojeDto
                {
                    QuantidadeVendas = vendasDoDia.Count,
                    TotalValor = totalVendasDia
                },
                VendasFiadasHoje = totalVendasFiadasHoje,
                TotalFiado = totalFiado,
                ProdutosBaixoEstoque = baixoEstoque,
                TotalClientes = totalClientes,
                UltimasVendas = ultimasVendas
            };

            return Ok(dashboard);
        }
    }
}