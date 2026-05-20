using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using ComercialMorro.API.Data;
using ComercialMorro.API.DTOs;
using ComercialMorro.API.Services.Interfaces;

namespace ComercialMorro.API.Services
{
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITokenService _tokenService;
        private readonly IDistributedCache _cache;
        private const string TOKEN_PREFIX = "ComercialMorro_token_";
        private const string BLACKLIST_PREFIX = "blacklist_";

        public AuthService(ApplicationDbContext context, ITokenService tokenService, IDistributedCache cache)
        {
            _context = context;
            _tokenService = tokenService;
            _cache = cache;
        }

        public async Task<TokenDto?> LoginAsync(LoginDto loginDto)
        {
            var usuario = await _context.Usuarios
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username);

            if (usuario == null || usuario.Status != "Ativo")
                return null;

            if (usuario.Senha != loginDto.Senha)
                return null;

            // Buscar funcionário separadamente
            var funcionario = await _context.Funcionarios
                .FirstOrDefaultAsync(f => f.IdFuncionario == usuario.IdFuncionario);

            var cargo = usuario.Funcionario?.Cargo ?? "Vendedor";
            var tipo = cargo == "Admin" ? "Admin" : "PDV";
            
            var accessToken = _tokenService.GenerateAccessToken(usuario.Username, cargo, tipo);
            var refreshToken = _tokenService.GenerateRefreshToken(tipo);
            var expirationMinutes = _tokenService.GetTokenExpirationMinutes(tipo);
            var expiresAt = DateTime.UtcNow.AddMinutes(expirationMinutes);

            var cacheExpiration = expirationMinutes > 60 * 24 * 30 
                ? TimeSpan.FromDays(365 * 10)
                : TimeSpan.FromHours(1);
            
            await _cache.SetStringAsync($"{TOKEN_PREFIX}{usuario.Username}", accessToken, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = cacheExpiration
            });

            return new TokenDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresAt = expiresAt,
                Username = usuario.Username,
                Role = cargo,
                Tipo = tipo
            };
        }

        public async Task<bool> LogoutAsync(string username)
        {
            await _cache.RemoveAsync($"{TOKEN_PREFIX}{username}");
            return true;
        }

        public async Task<TokenDto?> RefreshTokenAsync(string refreshToken)
        {
            
            return null;
        }

        public async Task<bool> BlacklistTokenAsync(string token, TimeSpan expiration)
        {
            await _cache.SetStringAsync($"{BLACKLIST_PREFIX}{token}", "revoked", new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = expiration
            });
            return true;
        }
    }
}