using Microsoft.AspNetCore.Mvc;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Services.Interfaces;

namespace ComercialMorro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClienteController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;

        public ClienteController(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;
        }

        /// <summary>
        /// Criar novo cliente
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<PessoaResponseDto>> Create([FromBody] PessoaCreateDto dto)
        {
            try
            {
                dto.IsCliente = true;
                dto.IsFuncionario = false;
                var result = await _pessoaService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = result.IdPessoa }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Listar todos os clientes
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaResponseDto>>> GetAll()
        {
            var clientes = await _pessoaService.GetAllAsync();
            return Ok(clientes);
        }

        /// <summary>
        /// Buscar cliente por ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<PessoaResponseDto>> GetById(int id)
        {
            var cliente = await _pessoaService.GetByIdAsync(id);
            if (cliente == null)
                return NotFound(new { mensagem = "Cliente não encontrado" });

            return Ok(cliente);
        }

        /// <summary>
        /// Atualizar dados do cliente
        /// </summary>
        [HttpPut("{id}")]
        public async Task<ActionResult<PessoaResponseDto>> Update(int id, [FromBody] PessoaCreateDto dto)
        {
            try
            {
                // Garantir que não está tentando converter cliente em funcionário
                dto.IsCliente = true;
                dto.IsFuncionario = false;

                var updated = await _pessoaService.UpdateAsync(id, dto);
                if (updated == null)
                    return NotFound(new { mensagem = "Cliente não encontrado" });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        /// <summary>
        /// Excluir cliente
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _pessoaService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { mensagem = "Cliente não encontrado" });

            return NoContent();
        }

        /// <summary>
        /// Buscar clientes por nome ou CPF
        /// </summary>
        [HttpGet("buscar")]
        public async Task<ActionResult<IEnumerable<PessoaResponseDto>>> Buscar([FromQuery] string termo)
        {
            if (string.IsNullOrWhiteSpace(termo))
                return BadRequest(new { mensagem = "Termo de busca não informado" });

            var result = await _pessoaService.BuscarAsync(termo);
            return Ok(result);
        }
    }
}