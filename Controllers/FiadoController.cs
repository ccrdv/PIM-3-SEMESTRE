using Microsoft.AspNetCore.Mvc;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Data;
using Microsoft.EntityFrameworkCore;

namespace ComercialMorro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FiadoController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public FiadoController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna resumo completo dos fiados
        /// </summary>
        [HttpGet("resumo")]
        public async Task<ActionResult<TotalFiadosDto>> GetResumoFiados()
        {
            var vendasFiadas = await _context.Vendas
                .Include(v => v.Cliente)
                    .ThenInclude(c => c!.Pessoa)        // Importante: pegar nome da Pessoa
                .Where(v => v.ClienteIdCliente != null)
                .OrderByDescending(v => v.DataHora)
                .ToListAsync();

            var fiados = vendasFiadas.Select(v => new FiadoResponseDto
            {
                IdVenda = v.IdVenda,
                DataHora = v.DataHora,
                NomeCliente = v.Cliente?.Pessoa?.Nome ?? "Cliente não encontrado",
                ValorTotal = v.TotalVenda,
                ValorPendente = v.TotalVenda,
                Status = "Pendente"
            }).ToList();

            var totalAReceber = fiados.Sum(f => f.ValorPendente);

            return Ok(new TotalFiadosDto
            {
                TotalAReceber = totalAReceber,
                QuantidadeClientesDevedores = fiados.Select(f => f.NomeCliente).Distinct().Count(),
                QuantidadeVendasPendentes = fiados.Count,
                VendasFiadas = fiados
            });
        }

        /// <summary>
        /// Lista todas as vendas fiadas pendentes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<FiadoResponseDto>>> GetAllFiados()
        {
            var fiados = await _context.Vendas
                .Include(v => v.Cliente)
                    .ThenInclude(c => c!.Pessoa)
                .Where(v => v.ClienteIdCliente != null)
                .OrderByDescending(v => v.DataHora)
                .Select(v => new FiadoResponseDto
                {
                    IdVenda = v.IdVenda,
                    DataHora = v.DataHora,
                    NomeCliente = v.Cliente!.Pessoa!.Nome,
                    ValorTotal = v.TotalVenda,
                    ValorPendente = v.TotalVenda,
                    Status = "Pendente"
                })
                .ToListAsync();

            return Ok(fiados);
        }

        /// <summary>
        /// Registra pagamento parcial ou total de um fiado
        /// </summary>
        [HttpPost("pagar")]
        public async Task<ActionResult<PagamentoResponseDto>> RegistrarPagamento([FromBody] PagamentoFiadoDto dto)
        {
            if (dto.ValorPago <= 0)
                return BadRequest("O valor pago deve ser maior que zero.");

            var venda = await _context.Vendas
                .Include(v => v.Cliente)
                    .ThenInclude(c => c!.Pessoa)
                .FirstOrDefaultAsync(v => v.IdVenda == dto.IdVenda);

            if (venda == null)
                return NotFound("Venda não encontrada.");

            if (venda.ClienteIdCliente == null)
                return BadRequest("Esta venda não é fiada.");

            var cliente = venda.Cliente!;

            // Registra o pagamento
            cliente.TotalFiado = Math.Max(0, cliente.TotalFiado - dto.ValorPago);

            await _context.SaveChangesAsync();

            var valorRestante = cliente.TotalFiado;

            return Ok(new PagamentoResponseDto
            {
                IdVenda = venda.IdVenda,
                NomeCliente = cliente.Pessoa?.Nome ?? "Cliente não encontrado",
                ValorTotalVenda = venda.TotalVenda,
                ValorPago = dto.ValorPago,
                ValorRestante = valorRestante,
                Mensagem = valorRestante <= 0
                    ? "Fiado quitado com sucesso!"
                    : $"Pagamento registrado. Restam R$ {valorRestante:F2}"
            });
        }
    }
}