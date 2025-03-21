using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using WebParfum.API.Services;
using WebParfum.API.Models.Usuarios;

namespace WebParfum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [EnableCors("AllowSpecificOrigins")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthenticationService _authService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthenticationService authService, ILogger<AuthController> logger)
        {
            _authService = authService;
            _logger = logger;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Email y contraseña son requeridos.");
            }

            try
            {
                var token = await _authService.AuthenticateAsync(request.Email, request.Password);
                if (token == null)
                {
                    return Unauthorized("Credenciales inválidas o correo no verificado.");
                }

                // Configurar las opciones de la cookie
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true, // No accesible desde JavaScript
                    Secure = true,   // Requiere HTTPS
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddHours(1)
                };

                // Establecer el token en una cookie
                Response.Cookies.Append("token", token, cookieOptions);

                return Ok(new { Token = token, Message = "Inicio de sesión exitoso." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error en el proceso de inicio de sesión");
                return StatusCode(500, "Error interno del servidor.");
            }
        }
    }
}
