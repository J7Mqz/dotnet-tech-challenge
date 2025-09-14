using AuthServer.Models;
using AuthServer.Services;
using Microsoft.AspNetCore.Mvc;

namespace AuthServer.Controller
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
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var tokenString = _authService.GenerateJwtToken(request);

            if (tokenString == null)
            {
                return Unauthorized();
            }

            return Ok(new { access_token = tokenString, expires_in = 3600 });
        }
    }
}