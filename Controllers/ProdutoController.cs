using Microsoft.AspNetCore.Mvc;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Services.Interfaces;

namespace ComercialMorro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProdutoController : ControllerBase
    {
        private readonly IProdutoService _service;

        public ProdutoController(IProdutoService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProdutoDto>>> GetAll()
        {
            var produtos = await _service.GetAllAsync();
            return Ok(produtos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ProdutoDto>> GetById(int id)
        {
            var produto = await _service.GetByIdAsync(id);
            return produto != null ? Ok(produto) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<ProdutoDto>> Create([FromBody] ProdutoCreateDto dto)
        {
            var produto = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = produto.IdProduto }, produto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProdutoCreateDto dto)
        {
            await _service.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }
    }
}