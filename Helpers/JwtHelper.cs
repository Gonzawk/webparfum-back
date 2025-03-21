using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace WebParfum.API.Helpers
{
    public static class JwtHelper
    {
        /// <summary>
        /// Genera un token JWT con información del usuario.
        /// </summary>
        /// <param name="userId">ID del usuario</param>
        /// <param name="nombreCompleto">Nombre completo del usuario</param>
        /// <param name="role">Rol del usuario</param>
        /// <param name="secret">Clave secreta configurada en appsettings.json</param>
        /// <param name="expireMinutes">Tiempo de expiración en minutos</param>
        /// <returns>Token JWT en formato string</returns>
        public static string GenerateToken(string userId, string nombreCompleto, string role, string secret, int expireMinutes = 60)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(secret);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim("nombre", nombreCompleto),
                new Claim(ClaimTypes.Role, role)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
