using Microsoft.AspNetCore.Mvc;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Services.Interfaces;

namespace ComercialMorro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FuncionarioController : ControllerBase
    {
        private readonly IPessoaService _pessoaService;

        public FuncionarioController(IPessoaService pessoaService)
        {
            _pessoaService = pessoaService;
        }

        [HttpPost]
        public async Task<ActionResult<PessoaResponseDto>> Create([FromBody] PessoaCreateDto dto)
        {
            dto.IsCliente = false;
            dto.IsFuncionario = true;
            return await _pessoaService.CreateAsync(dto);
        }
    }
}