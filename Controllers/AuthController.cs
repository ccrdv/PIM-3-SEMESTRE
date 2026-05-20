using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Services.Interfaces;

namespace ComercialMorro.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<ActionResult<TokenDto>> Login([FromBody] LoginDto loginDto)
        {
            var result = await _authService.LoginAsync(loginDto);

            if (result == null)
                return Unauthorized(new { mensagem = "Usuário ou senha inválidos" });

            return Ok(result);
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            var username = User.Identity?.Name;
            if (username != null)
            {
                await _authService.LogoutAsync(username);
            }
            return Ok(new { mensagem = "Logout realizado com sucesso" });
        }

        [HttpPost("refresh")]
        public async Task<ActionResult<TokenDto>> Refresh([FromBody] RefreshTokenDto refreshTokenDto)
        {
            var result = await _authService.RefreshTokenAsync(refreshTokenDto.RefreshToken);

            if (result == null)
                return Unauthorized(new { mensagem = "Refresh token inválido" });

            return Ok(result);
        }
        [HttpPost("force-logout/{username}")]
        [Authorize]
        public async Task<IActionResult> ForceLogout(string username)
        {
            // Remover token do Redis
            await _authService.LogoutAsync(username);

            return Ok(new { mensagem = $"Usuário {username} foi forçado a sair do sistema" });
        }

        [HttpGet("check-token")]
        [Authorize]
        public IActionResult CheckToken()
        {
            var username = User.Identity?.Name;
            return Ok(new
            {
                mensagem = "Token válido",
                username = username,
                expiraEm = DateTime.UtcNow.AddMinutes(60)
            });
        }
    }
}