using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Data;  // Ajusta según la ubicación de tu AppDbContext

namespace WebParfum.API.Controllers
{
    [ApiController]
    [Route("verificar")]
    public class VerificarController : ControllerBase
    {
        private readonly AppDbContext _context;

        public VerificarController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return BadRequest("Token inválido.");
            }

            // Buscar el usuario con el token de verificación
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.CodigoVerificacion == token);
            if (usuario == null)
            {
                return NotFound("Usuario no encontrado o token inválido.");
            }

            // Opcional: puedes agregar validación de expiración si implementaste esa columna

            // Actualiza el usuario para marcar el correo como verificado y limpia el token
            usuario.EmailVerificado = true;
            usuario.CodigoVerificacion = null;

            await _context.SaveChangesAsync();

            return Ok("Correo verificado exitosamente.");
        }
    }
}
