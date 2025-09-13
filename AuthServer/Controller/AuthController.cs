using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthServer.Models; 

namespace AuthServer.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _config;

        public AuthController(IConfiguration config)
        {
            _config = config;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {

            var issuer = _config["Jwt:Issuer"];
            var audience = _config["Jwt:Audience"];
            var key = _config["Jwt:Key"]; // <-- Añadir esta línea

            Console.WriteLine($"--- AuthServer JWT Config ---");
            Console.WriteLine($"Issuer: {issuer}");
            Console.WriteLine($"Audience: {audience}");
            Console.WriteLine($"Key: {key}"); // <-- Añadir esta línea
            Console.WriteLine($"---------------------------");
            try // <-- INICIO DEL BLOQUE DE DEPURACIÓN
            {
                if (request.Username != "admin" || request.Password != "password")
                {
                    return Unauthorized();
                }

                var jwtKey = _config["Jwt:Key"];

                // Verificación para asegurarnos que la clave no es nula
                if (string.IsNullOrEmpty(jwtKey))
                {
                    throw new InvalidOperationException("Jwt:Key no está configurada en appsettings.json");
                }

                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
                var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new[]
                {
            new Claim(JwtRegisteredClaimNames.Sub, "user_id_123"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

                var token = new JwtSecurityToken(
                  issuer: _config["Jwt:Issuer"],
                  audience: _config["Jwt:Audience"],
                  claims: claims,
                  expires: DateTime.UtcNow.AddMinutes(60),
                  signingCredentials: credentials);

                var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

                return Ok(new { access_token = tokenString, expires_in = 3600 });
            }
            catch (Exception ex) // <-- LA TRAMPA PARA LA EXCEPCIÓN
            {
                // Imprimimos el error directamente a la consola
                Console.WriteLine("----------- ERROR CAPTURADO EN AUTHCONTROLLER -----------");
                Console.WriteLine(ex.ToString());
                Console.WriteLine("-------------------------------------------------------");

                // Volvemos a lanzar el error para que el comportamiento no cambie
                throw;
            }
        }
    }
}