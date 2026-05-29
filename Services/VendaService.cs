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
                ClienteIdCliente = null, // Será preenchido se for fiado
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
                await _produtoRepository.UpdateAsync(produto);
            }

            venda.TotalVenda = totalVenda;
            venda.TotalDesconto = venda.Itens.Sum(i => i.ValorDesconto);

            // Busca nome do cliente da tabela PESSOA
            string nomeCliente = "Venda à Vista";
            int? clienteIdCliente = null;

            if (dto.ClienteId.HasValue)
            {
                var pessoa = await _context.Pessoas
                    .FirstOrDefaultAsync(p => p.IdPessoa == dto.ClienteId.Value);

                if (pessoa != null)
                {
                    nomeCliente = pessoa.Nome;
                    clienteIdCliente = pessoa.ClienteIdCliente;
                    venda.ClienteIdCliente = clienteIdCliente;
                }
                else
                {
                    nomeCliente = "Cliente não encontrado";
                }
            }

            // Salva a venda primeiro
            await _vendaRepository.AddAsync(venda);
            await _context.SaveChangesAsync();

            // Se for venda fiada, atualiza o TotalFiado do cliente
            if (dto.IsFiado && clienteIdCliente.HasValue)
            {
                var cliente = await _context.Clientes
                    .FirstOrDefaultAsync(c => c.IdCliente == clienteIdCliente.Value);

                if (cliente != null)
                {
                    cliente.TotalFiado += totalVenda;
                    _context.Clientes.Update(cliente);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    throw new Exception("Cliente não encontrado na tabela de clientes.");
                }
            }

            // Retorna a resposta com os dados completos
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
            var vendasDto = new List<VendaResponseDto>();

            foreach (var venda in vendas)
            {
                vendasDto.Add(await MapToResponseAsync(venda));
            }

            return vendasDto;
        }

        public async Task<VendaResponseDto?> GetByIdAsync(int id)
        {
            var venda = await _vendaRepository.GetByIdAsync(id);
            if (venda == null) return null;

            return await MapToResponseAsync(venda);
        }

        private async Task<VendaResponseDto> MapToResponseAsync(Venda v)
        {
            // Busca os produtos para obter os nomes
            var produtoIds = v.Itens.Select(i => i.IdProduto).Distinct().ToList();
            var produtos = await _produtoRepository.GetAllAsync();
            var produtosDict = produtos
                .Where(p => produtoIds.Contains(p.IdProduto))
                .ToDictionary(p => p.IdProduto);

            // Busca o nome do cliente através da tabela PESSOA
            string nomeCliente = "Venda à Vista";
            if (v.ClienteIdCliente.HasValue)
            {
                var pessoa = await _context.Pessoas
                    .FirstOrDefaultAsync(p => p.ClienteIdCliente == v.ClienteIdCliente.Value);

                nomeCliente = pessoa?.Nome ?? "Cliente não encontrado";
            }

            return new VendaResponseDto
            {
                IdVenda = v.IdVenda,
                DataHora = v.DataHora,
                TotalVenda = v.TotalVenda,
                TotalDesconto = v.TotalDesconto,
                NomeCliente = nomeCliente,
                Itens = v.Itens.Select(i => new ItemVendaResponseDto
                {
                    ProdutoId = i.IdProduto,
                    NomeProduto = produtosDict.ContainsKey(i.IdProduto) ? produtosDict[i.IdProduto].Descricao : $"Produto {i.IdProduto}",
                    Quantidade = i.Qdte,
                    PrecoUnitario = i.PrecoUnitario,
                    ValorDesconto = i.ValorDesconto,
                    ValorTotal = i.ValorTotal
                }).ToList()
            };
        }
    }
}