using ComercialMorro.API.DTOs;
using ComercialMorro.API.Models;
using ComercialMorro.API.Repositories.Interfaces;
using ComercialMorro.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using ComercialMorro.API.Data;

namespace ComercialMorro.API.Services
{
    public class VendaService : IVendaService
    {
        private readonly IVendaRepository _vendaRepository;
        private readonly IProdutoRepository _produtoRepository;
        private readonly ApplicationDbContext _context;

        public VendaService(
            IVendaRepository vendaRepository,
            IProdutoRepository produtoRepository,
            ApplicationDbContext context)
        {
            _vendaRepository = vendaRepository;
            _produtoRepository = produtoRepository;
            _context = context;
        }

        public async Task<VendaResponseDto> RegistrarVendaAsync(NovaVendaRequestDto dto)
        {
            if (dto.Itens == null || !dto.Itens.Any())
                throw new Exception("A venda deve conter pelo menos um item.");

            if (dto.IsFiado && dto.ClienteId == null)
                throw new Exception("É obrigatório informar o cliente para vendas fiadas.");

            var venda = new Venda
            {
                DataHora = DateTime.Now,
                ClienteIdCliente = dto.ClienteId,
                Itens = new List<ItemVenda>()
            };

            decimal totalVenda = 0;

            var produtoIds = dto.Itens.Select(i => i.ProdutoId).Distinct().ToList();
            var produtosDict = (await _produtoRepository.GetAllAsync())
                .Where(p => produtoIds.Contains(p.IdProduto))
                .ToDictionary(p => p.IdProduto);

            foreach (var itemDto in dto.Itens)
            {
                if (!produtosDict.TryGetValue(itemDto.ProdutoId, out var produto))
                    throw new Exception($"Produto ID {itemDto.ProdutoId} não encontrado.");

                if (produto.Qtde < itemDto.Quantidade)
                    throw new Exception($"Estoque insuficiente! {produto.Descricao} | Disponível: {produto.Qtde}");

                var valorItem = produto.PrecoVenda * itemDto.Quantidade;
                var desconto = itemDto.Desconto ?? 0;
                var valorTotalItem = valorItem - desconto;

                var item = new ItemVenda
                {
                    IdProduto = itemDto.ProdutoId,
                    Qdte = itemDto.Quantidade,
                    PrecoUnitario = produto.PrecoVenda,
                    ValorDesconto = desconto,
                    ValorTotal = valorTotalItem
                };

                venda.Itens.Add(item);
                totalVenda += valorTotalItem;

                produto.Qtde -= itemDto.Quantidade;
            }

            venda.TotalVenda = totalVenda;
            venda.TotalDesconto = venda.Itens.Sum(i => i.ValorDesconto);

            await _vendaRepository.AddAsync(venda);
            if (dto.IsFiado && dto.ClienteId.HasValue)
            {
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.IdCliente == dto.ClienteId.Value);

                if (cliente != null)
                {
                    cliente.TotalFiado += totalVenda;
                    _context.Clientes.Update(cliente);
                    await _context.SaveChangesAsync();
                }
            }

            // Busca nome do cliente da tabela PESSOA
            string nomeCliente = "Venda à Vista";
            if (dto.ClienteId.HasValue)
            {
                var pessoa = await _context.Pessoas.FindAsync(dto.ClienteId.Value);
                nomeCliente = pessoa?.Nome ?? "Cliente não encontrado";
            }

            return new VendaResponseDto
            {
                IdVenda = venda.IdVenda,
                DataHora = venda.DataHora,
                TotalVenda = venda.TotalVenda,
                TotalDesconto = venda.TotalDesconto,
                NomeCliente = nomeCliente,
                Itens = venda.Itens.Select(i => new ItemVendaResponseDto
                {
                    ProdutoId = i.IdProduto,
                    NomeProduto = produtosDict[i.IdProduto].Descricao,
                    Quantidade = i.Qdte,
                    PrecoUnitario = i.PrecoUnitario,
                    ValorDesconto = i.ValorDesconto,
                    ValorTotal = i.ValorTotal
                }).ToList()
            };
        }

        public async Task<IEnumerable<VendaResponseDto>> GetAllAsync()
        {
            var vendas = await _vendaRepository.GetAllAsync();
            return vendas.Select(MapToResponse).ToList();
        }

        public async Task<VendaResponseDto?> GetByIdAsync(int id)
        {
            var venda = await _vendaRepository.GetByIdAsync(id);
            return venda == null ? null : MapToResponse(venda);
        }

        private VendaResponseDto MapToResponse(Venda v)
        {
            return new VendaResponseDto
            {
                IdVenda = v.IdVenda,
                DataHora = v.DataHora,
                TotalVenda = v.TotalVenda,
                Itens = v.Itens.Select(i => new ItemVendaResponseDto
                {
                    NomeProduto = $"Produto {i.IdProduto}",
                    Quantidade = i.Qdte,
                    PrecoUnitario = i.PrecoUnitario,
                    ValorTotal = i.ValorTotal
                }).ToList()
            };
        }
    }
}