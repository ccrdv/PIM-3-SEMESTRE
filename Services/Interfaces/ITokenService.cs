using System.Security.Claims;

namespace ComercialMorro.API.Services.Interfaces
{
    public interface ITokenService
    {
        string GenerateAccessToken(string username, string role, string tipo);
        string GenerateRefreshToken(string tipo);
        ClaimsPrincipal GetPrincipalFromExpiredToken(string token);
        int GetTokenExpirationMinutes(string tipo);
    }
}