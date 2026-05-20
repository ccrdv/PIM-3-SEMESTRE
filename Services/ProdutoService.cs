using ComercialMorro.API.DTOs;
using ComercialMorro.API.Models;
using ComercialMorro.API.Repositories.Interfaces;
using ComercialMorro.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ComercialMorro.API.Data;

namespace ComercialMorro.API.Services
{
    public class ProdutoService : IProdutoService
    {
        private readonly IProdutoRepository _repository;
        private readonly ApplicationDbContext _context;

        public ProdutoService(IProdutoRepository repository, ApplicationDbContext context)
        {
            _repository = repository;
            _context = context;
        }
        public async Task<IEnumerable<ProdutoDto>> BuscarAsync(string termo)
        {
            var produtos = await _context.Produtos
                .Include(p => p.Categoria)
                .Where(p => p.Descricao.Contains(termo))
                .Take(20)
                .ToListAsync();

            return produtos.Select(p => new ProdutoDto
            {
                IdProduto = p.IdProduto,
                Descricao = p.Descricao,
                PrecoVenda = p.PrecoVenda,
                Qtde = p.Qtde,
                CategoriaDescricao = p.Categoria?.Descricao
            });
        }

        public async Task<IEnumerable<ProdutoDto>> GetAllAsync()
        {
            var produtos = await _repository.GetAllAsync();
            return produtos.Select(p => new ProdutoDto
            {
                IdProduto = p.IdProduto,
                Descricao = p.Descricao,
                PrecoCompra = p.PrecoCompra,
                PrecoVenda = p.PrecoVenda,
                Qtde = p.Qtde,
                IdCategoria = p.IdCategoria,
                CategoriaDescricao = p.Categoria?.Descricao
            });
        }

        public async Task<ProdutoDto?> GetByIdAsync(int id)
        {
            var produto = await _repository.GetByIdAsync(id);
            if (produto == null) return null;

            return new ProdutoDto
            {
                IdProduto = produto.IdProduto,
                Descricao = produto.Descricao,
                PrecoCompra = produto.PrecoCompra,
                PrecoVenda = produto.PrecoVenda,
                Qtde = produto.Qtde,
                IdCategoria = produto.IdCategoria,
                CategoriaDescricao = produto.Categoria?.Descricao
            };
        }

        public async Task<ProdutoDto> CreateAsync(ProdutoCreateDto dto)
        {
            // Regras de negócio
            if (dto.PrecoVenda <= 0)
                throw new Exception("Preço de venda deve ser maior que zero.");

            if (dto.Qtde < 0)
                throw new Exception("Quantidade não pode ser negativa.");

            var produto = new Produto
            {
                Descricao = dto.Descricao,
                PrecoCompra = dto.PrecoCompra,
                PrecoVenda = dto.PrecoVenda,
                Qtde = dto.Qtde,
                IdCategoria = dto.IdCategoria
            };

            await _repository.AddAsync(produto);

            return await GetByIdAsync(produto.IdProduto) ?? throw new Exception("Erro ao criar produto");
        }

        public async Task UpdateAsync(int id, ProdutoCreateDto dto)
        {
            var produto = await _repository.GetByIdAsync(id);
            if (produto == null)
                throw new Exception("Produto não encontrado.");

            produto.Descricao = dto.Descricao;
            produto.PrecoCompra = dto.PrecoCompra;
            produto.PrecoVenda = dto.PrecoVenda;
            produto.Qtde = dto.Qtde;
            produto.IdCategoria = dto.IdCategoria;

            await _repository.UpdateAsync(produto);
        }

        public async Task DeleteAsync(int id)
        {
            if (!await _repository.ExistsAsync(id))
                throw new Exception("Produto não encontrado.");

            await _repository.DeleteAsync(id);
        }
    }
}