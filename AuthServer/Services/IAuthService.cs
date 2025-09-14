using AuthServer.Models;

namespace AuthServer.Services
{
    public interface IAuthService
    {
        string? GenerateJwtToken(LoginRequest loginRequest);
    }
}