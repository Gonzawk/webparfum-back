using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Data;
using WebParfum.API.Helpers;
using WebParfum.API.Models;

namespace WebParfum.API.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly AppDbContext _context;
        private readonly IConfiguration _configuration;

        public AuthenticationService(AppDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
        }

        public async Task<string> AuthenticateAsync(string email, string password)
        {
            // Buscar al usuario (incluyendo sus roles, si los hay)
            var usuario = await _context.Usuarios
                .Include(u => u.Roles)
                .FirstOrDefaultAsync(u => u.Email == email);

            // Validar existencia y que el correo esté verificado
            if (usuario == null || !(usuario.EmailVerificado ?? false))
            {
                return null;
            }

            // Hashear la contraseña ingresada (debe coincidir con el hash almacenado en base64)
            string inputHash = HashPassword(password);
            if (!usuario.PasswordHash.Equals(inputHash))
            {
                return null;
            }

            // Determinar el rol según la jerarquía (por ejemplo, Superadmin > Admin > Usuario)
            string role = "Usuario"; // valor por defecto
            if (usuario.Roles.Any(r => r.RoleName.Equals("Superadmin", StringComparison.OrdinalIgnoreCase)))
            {
                role = "Superadmin";
            }
            else if (usuario.Roles.Any(r => r.RoleName.Equals("Admin", StringComparison.OrdinalIgnoreCase)))
            {
                role = "Admin";
            }

            string secret = _configuration["Jwt:Secret"];
            var token = JwtHelper.GenerateToken(
                usuario.UsuarioId.ToString(),
                usuario.Email,
                role,
                secret
            );

            return token;
        }

        private string HashPassword(string plainPassword)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
