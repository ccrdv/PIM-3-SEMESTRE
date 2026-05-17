using Microsoft.AspNetCore.Mvc;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Services.Interfaces;

namespace ComercialMorro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriasController : ControllerBase
    {
        private readonly ICategoriaService _service;

        public CategoriasController(ICategoriaService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoriaDto>>> GetAll()
            => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoriaDto>> GetById(int id)
        {
            var cat = await _service.GetByIdAsync(id);
            return cat != null ? Ok(cat) : NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<CategoriaDto>> Create([FromBody] CategoriaCreateDto dto)
        {
            var cat = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = cat.IdCategoria }, cat);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoriaCreateDto dto)
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