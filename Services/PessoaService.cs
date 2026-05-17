using ComercialMorro.API.DTOs;
using ComercialMorro.API.Models;
using ComercialMorro.API.Data;
using ComercialMorro.API.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace ComercialMorro.API.Services
{
    public class PessoaService : IPessoaService
    {
        private readonly ApplicationDbContext _context;

        public PessoaService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<PessoaResponseDto> CreateAsync(PessoaCreateDto dto)
        {
            var pessoa = new Pessoa
            {
                Nome = dto.Nome,
                Cpf = dto.Cpf,
                Telefone = dto.Telefone,
                Endereco = dto.Endereco
            };

            _context.Pessoas.Add(pessoa);
            await _context.SaveChangesAsync();

            if (dto.IsCliente)
            {
                _context.Clientes.Add(new Cliente
                {
                    //IdCliente = pessoa.IdPessoa,
                    TotalFiado = 0,
                    Status = "Ativo"
                });
            }

            if (dto.IsFuncionario)
            {
                _context.Funcionarios.Add(new Funcionario
                {
                    //IdFuncionario = pessoa.IdPessoa,
                    Cargo = dto.Cargo ?? "Vendedor",
                    Salario = dto.Salario ?? 0
                });
            }

            await _context.SaveChangesAsync();

            return new PessoaResponseDto
            {
                IdPessoa = pessoa.IdPessoa,
                Nome = pessoa.Nome,
                Cpf = pessoa.Cpf,
                Telefone = pessoa.Telefone,
                IsCliente = dto.IsCliente,
                IsFuncionario = dto.IsFuncionario
            };
        }

        public async Task<PessoaResponseDto?> GetByIdAsync(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null) return null;

            return new PessoaResponseDto
            {
                IdPessoa = pessoa.IdPessoa,
                Nome = pessoa.Nome,
                Cpf = pessoa.Cpf,
                Telefone = pessoa.Telefone,
                IsCliente = await _context.Clientes.AnyAsync(c => c.IdCliente == id),
                IsFuncionario = await _context.Funcionarios.AnyAsync(f => f.IdFuncionario == id)
            };
        }

        public async Task<IEnumerable<PessoaResponseDto>> GetAllAsync()
        {
            var pessoas = await _context.Pessoas.ToListAsync();
            var result = new List<PessoaResponseDto>();

            foreach (var p in pessoas)
            {
                result.Add(new PessoaResponseDto
                {
                    IdPessoa = p.IdPessoa,
                    Nome = p.Nome,
                    Cpf = p.Cpf,
                    Telefone = p.Telefone,
                    IsCliente = await _context.Clientes.AnyAsync(c => c.IdCliente == p.IdPessoa),
                    IsFuncionario = await _context.Funcionarios.AnyAsync(f => f.IdFuncionario == p.IdPessoa)
                });
            }

            return result;
        }
    }
}