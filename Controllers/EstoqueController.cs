using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ComercialMorro.API.Data;

namespace ComercialMorro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstoqueController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public EstoqueController(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Resumo para os cards do estoque
        /// </summary>
        [HttpGet("resumo")]
        public async Task<IActionResult> GetResumo()
        {
            var totalProdutos = await _context.Produtos.CountAsync();
            var produtosBaixoEstoque = await _context.Produtos.CountAsync(p => p.Qtde < 10);
            var valorTotalEstoque = await _context.Produtos.SumAsync(p => p.PrecoVenda * p.Qtde);

            return Ok(new
            {
                TotalProdutos = totalProdutos,
                ProdutosBaixoEstoque = produtosBaixoEstoque,
                ValorTotalEstoque = valorTotalEstoque
            });
        }

        /// <summary>
        /// Lista produtos com baixo estoque (para o card vermelho)
        /// </summary>
        [HttpGet("baixo-estoque")]
        public async Task<IActionResult> GetBaixoEstoque([FromQuery] int limite = 10)
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Qtde < limite)
                .OrderBy(p => p.Qtde)
                .Select(p => new
                {
                    p.IdProduto,
                    p.Descricao,
                    p.Qtde,
                    Categoria = p.Categoria != null ? p.Categoria.Descricao : "Sem categoria",
                    p.PrecoVenda,
                    ValorTotal = p.PrecoVenda * p.Qtde
                })
                .ToListAsync();

            return Ok(produtos);
        }

        /// <summary>
        /// Lista todos os produtos com filtro por categoria
        /// </summary>
        [HttpGet("produtos")]
        public async Task<IActionResult> GetAllProdutos([FromQuery] int? categoriaId = null)
        {
            var query = _context.Produtos
                .Include(p => p.Categoria)
                .AsQueryable();

            if (categoriaId.HasValue)
                query = query.Where(p => p.IdCategoria == categoriaId.Value);

            var produtos = await query
                .Select(p => new
                {
                    p.IdProduto,
                    p.Descricao,
                    p.Qtde,
                    Categoria = p.Categoria != null ? p.Categoria.Descricao : "Sem categoria",
                    p.PrecoVenda,
                    p.PrecoCompra,
                    Status = p.Qtde < 10 ? "Baixo Estoque" : "Normal",
                    ValorTotalEstoque = p.PrecoVenda * p.Qtde
                })
                .ToListAsync();

            return Ok(produtos);
        }
    }
}