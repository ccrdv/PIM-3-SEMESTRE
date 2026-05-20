using ComercialMorro.API.DTOs;

namespace ComercialMorro.API.Services.Interfaces
{
    public interface IAuthService
    {
        Task<TokenDto?> LoginAsync(LoginDto loginDto);
        Task<bool> LogoutAsync(string username);
        Task<TokenDto?> RefreshTokenAsync(string refreshToken);
        Task<bool> BlacklistTokenAsync(string token, TimeSpan expiration);
    }
}