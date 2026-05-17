using Microsoft.AspNetCore.Mvc;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Services.Interfaces;

namespace ComercialMorro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VendaController : ControllerBase
    {
        private readonly IVendaService _vendaService;

        public VendaController(IVendaService vendaService)
        {
            _vendaService = vendaService;
        }

        [HttpPost]
        public async Task<ActionResult<VendaResponseDto>> RegistrarVenda([FromBody] NovaVendaRequestDto dto)
        {
            try
            {
                var venda = await _vendaService.RegistrarVendaAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = venda.IdVenda }, venda);
            }
            catch (Exception ex)
            {
                return BadRequest(new { mensagem = ex.Message });
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<VendaResponseDto>>> GetAll()
        {
            var vendas = await _vendaService.GetAllAsync();
            return Ok(vendas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<VendaResponseDto>> GetById(int id)
        {
            var venda = await _vendaService.GetByIdAsync(id);
            return venda != null ? Ok(venda) : NotFound();
        }
    }
}