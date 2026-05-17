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

        [HttpPost]
        public async Task<ActionResult<PessoaResponseDto>> Create([FromBody] PessoaCreateDto dto)
        {
            dto.IsCliente = true;
            dto.IsFuncionario = false;
            return await _pessoaService.CreateAsync(dto);
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<PessoaResponseDto>>> GetAll()
            => Ok(await _pessoaService.GetAllAsync());
    }
}