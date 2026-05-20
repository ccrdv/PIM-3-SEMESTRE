using Microsoft.EntityFrameworkCore;
using ComercialMorro.API.Data;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Models;
using ComercialMorro.API.Services.Interfaces;

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
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                if (dto.IsCliente && dto.IsFuncionario)
                    throw new Exception("Uma pessoa não pode ser cliente e funcionário ao mesmo tempo");

                var pessoa = new Pessoa
                {
                    Nome = dto.Nome ?? string.Empty,
                    Cpf = dto.Cpf ?? string.Empty,
                    Telefone = dto.Telefone ?? "Não informado",  // Valor padrão se null
                    Endereco = dto.Endereco ?? "Não informado"   // Valor padrão se null
                };

                // Se for cliente, criar registro na tabela CLIENTE
                if (dto.IsCliente)
                {
                    var cliente = new Cliente
                    {
                        TotalFiado = 0,
                        Status = dto.Status ?? "ATIVO"
                    };
                    _context.Clientes.Add(cliente);
                    await _context.SaveChangesAsync();

                    pessoa.ClienteIdCliente = cliente.IdCliente;
                }

                _context.Pessoas.Add(pessoa);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new PessoaResponseDto
                {
                    IdPessoa = pessoa.IdPessoa,
                    Nome = pessoa.Nome,
                    Cpf = pessoa.Cpf,
                    Telefone = pessoa.Telefone ?? "Não informado",
                    Endereco = pessoa.Endereco ?? "Não informado",
                    IsCliente = pessoa.ClienteIdCliente.HasValue,
                    IsFuncionario = pessoa.FuncionarioIdFuncionario.HasValue,
                    TotalFiado = 0,
                    Status = dto.Status ?? "ATIVO",
                    Cargo = dto.Cargo,
                    Salario = dto.Salario
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<PessoaResponseDto>> GetAllAsync()
        {
            var pessoas = await _context.Pessoas
                .Include(p => p.Cliente)
                .Include(p => p.Funcionario)
                .Where(p => p.ClienteIdCliente.HasValue)
                .ToListAsync();

            return pessoas.Select(p => new PessoaResponseDto
            {
                IdPessoa = p.IdPessoa,
                Nome = p.Nome ?? string.Empty,
                Cpf = p.Cpf ?? string.Empty,
                Telefone = string.IsNullOrEmpty(p.Telefone) ? "Não informado" : p.Telefone,
                Endereco = string.IsNullOrEmpty(p.Endereco) ? "Não informado" : p.Endereco,
                IsCliente = p.ClienteIdCliente.HasValue,
                IsFuncionario = p.FuncionarioIdFuncionario.HasValue,
                TotalFiado = p.Cliente != null ? p.Cliente.TotalFiado : 0,
                Status = p.Cliente != null ? (p.Cliente.Status ?? "ATIVO") : "ATIVO",
                Cargo = p.Funcionario != null ? p.Funcionario.Cargo : null,
                Salario = p.Funcionario != null ? p.Funcionario.Salario : null
            });
        }

        public async Task<PessoaResponseDto?> GetByIdAsync(int id)
        {
            var pessoa = await _context.Pessoas
                .Include(p => p.Cliente)
                .Include(p => p.Funcionario)
                .FirstOrDefaultAsync(p => p.IdPessoa == id && p.ClienteIdCliente.HasValue);

            if (pessoa == null)
                return null;

            return new PessoaResponseDto
            {
                IdPessoa = pessoa.IdPessoa,
                Nome = pessoa.Nome,
                Cpf = pessoa.Cpf,
                Telefone = pessoa.Telefone,
                Endereco = pessoa.Endereco,
                IsCliente = pessoa.ClienteIdCliente.HasValue,
                IsFuncionario = pessoa.FuncionarioIdFuncionario.HasValue,
                TotalFiado = pessoa.Cliente?.TotalFiado ?? 0,
                Status = pessoa.Cliente?.Status ?? "ATIVO",
                Cargo = pessoa.Funcionario?.Cargo,
                Salario = pessoa.Funcionario?.Salario
            };
        }

        public async Task<PessoaResponseDto?> UpdateAsync(int id, PessoaCreateDto dto)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pessoa = await _context.Pessoas
                    .Include(p => p.Cliente)
                    .Include(p => p.Funcionario)
                    .FirstOrDefaultAsync(p => p.IdPessoa == id);

                if (pessoa == null)
                    return null;

                // Atualizar dados da pessoa
                pessoa.Nome = dto.Nome;
                pessoa.Cpf = dto.Cpf;
                pessoa.Telefone = dto.Telefone;
                pessoa.Endereco = dto.Endereco;

                // Se for cliente, atualizar status
                if (pessoa.Cliente != null && dto.Status != null)
                {
                    pessoa.Cliente.Status = dto.Status;
                }

                // Se for funcionário, atualizar cargo e salário
                if (pessoa.Funcionario != null)
                {
                    if (dto.Cargo != null)
                        pessoa.Funcionario.Cargo = dto.Cargo;
                    if (dto.Salario.HasValue)
                        pessoa.Funcionario.Salario = dto.Salario.Value;
                }

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return new PessoaResponseDto
                {
                    IdPessoa = pessoa.IdPessoa,
                    Nome = pessoa.Nome,
                    Cpf = pessoa.Cpf,
                    Telefone = pessoa.Telefone,
                    Endereco = pessoa.Endereco,
                    IsCliente = pessoa.ClienteIdCliente.HasValue,
                    IsFuncionario = pessoa.FuncionarioIdFuncionario.HasValue,
                    TotalFiado = pessoa.Cliente?.TotalFiado ?? 0,
                    Status = pessoa.Cliente?.Status ?? "ATIVO",
                    Cargo = pessoa.Funcionario?.Cargo,
                    Salario = pessoa.Funcionario?.Salario
                };
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<bool> DeleteAsync(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var pessoa = await _context.Pessoas
                    .Include(p => p.Cliente)
                    .Include(p => p.Funcionario)
                    .FirstOrDefaultAsync(p => p.IdPessoa == id);

                if (pessoa == null)
                    return false;

                // Remover cliente associado
                if (pessoa.Cliente != null)
                {
                    _context.Clientes.Remove(pessoa.Cliente);
                }

                // Remover funcionário associado
                if (pessoa.Funcionario != null)
                {
                    _context.Funcionarios.Remove(pessoa.Funcionario);
                }

                _context.Pessoas.Remove(pessoa);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return true;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<IEnumerable<PessoaResponseDto>> BuscarAsync(string termo)
        {
            var pessoas = await _context.Pessoas
                .Include(p => p.Cliente)
                .Include(p => p.Funcionario)
                .Where(p => p.ClienteIdCliente.HasValue &&
                    (EF.Functions.Like(p.Nome, $"%{termo}%") ||
                     EF.Functions.Like(p.Cpf, $"%{termo}%")))
                .Take(20)
                .ToListAsync();

            return pessoas.Select(p => new PessoaResponseDto
            {
                IdPessoa = p.IdPessoa,
                Nome = p.Nome,
                Cpf = p.Cpf,
                Telefone = p.Telefone,
                Endereco = p.Endereco,
                IsCliente = p.ClienteIdCliente.HasValue,
                IsFuncionario = p.FuncionarioIdFuncionario.HasValue,
                TotalFiado = p.Cliente?.TotalFiado ?? 0,
                Status = p.Cliente?.Status ?? "ATIVO",
                Cargo = p.Funcionario?.Cargo,
                Salario = p.Funcionario?.Salario
            });
        }
    }
}