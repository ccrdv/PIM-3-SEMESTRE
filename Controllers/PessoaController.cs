using Microsoft.AspNetCore.Mvc;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Models;
using ComercialMorro.API.Data;
using Microsoft.EntityFrameworkCore;

namespace ComercialMorro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PessoaController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PessoaController(ApplicationDbContext context)
        {
            _context = context;
        }


        /// Cadastra uma nova pessoa (pode ser Cliente, Funcionário ou ambos)
        [HttpPost]
        public async Task<ActionResult<PessoaResponseDto>> Create([FromBody] PessoaCreateDto dto)
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

            // Criar como Cliente
            if (dto.IsCliente)
            {
                var cliente = new Cliente
                {
                    //IdCliente = pessoa.IdPessoa,
                    TotalFiado = 0,
                    Status = "Ativo"
                };
                _context.Clientes.Add(cliente);
            }

            // Criar como Funcionário
            if (dto.IsFuncionario)
            {
                var funcionario = new Funcionario
                {
                    //IdFuncionario = pessoa.IdPessoa,
                    Cargo = dto.Cargo ?? "Vendedor",
                    Salario = dto.Salario ?? 0
                };
                _context.Funcionarios.Add(funcionario);
            }

            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetById), new { id = pessoa.IdPessoa }, new PessoaResponseDto
            {
                IdPessoa = pessoa.IdPessoa,
                Nome = pessoa.Nome,
                Cpf = pessoa.Cpf,
                Telefone = pessoa.Telefone,
                IsCliente = dto.IsCliente,
                IsFuncionario = dto.IsFuncionario
            });
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaResponseDto>> GetById(int id)
        {
            var pessoa = await _context.Pessoas.FindAsync(id);
            if (pessoa == null) return NotFound("Pessoa não encontrada.");

            return Ok(new PessoaResponseDto
            {
                IdPessoa = pessoa.IdPessoa,
                Nome = pessoa.Nome,
                Cpf = pessoa.Cpf,
                Telefone = pessoa.Telefone,
                IsCliente = await _context.Clientes.AnyAsync(c => c.IdCliente == id),
                IsFuncionario = await _context.Funcionarios.AnyAsync(f => f.IdFuncionario == id)
            });
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaResponseDto>>> GetAll()
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

            return Ok(result);
        }
    }
}