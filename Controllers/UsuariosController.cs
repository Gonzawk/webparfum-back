using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using System.Security.Cryptography;
using System.Text;
using WebParfum.API.Services;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Data;
using WebParfum.API.Models.Usuarios;
using WebParfum.API.Models.Recuperar_clave;

namespace WebParfum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IEmailService _emailService;
        private readonly AppDbContext _context;

        public UsuariosController(IConfiguration configuration, IEmailService emailService, AppDbContext context)
        {
            _configuration = configuration;
            _emailService = emailService;
            _context = context;
        }

        // GET: api/usuarios
        // Retorna solo los datos necesarios de cada usuario
        [HttpGet]
        public async Task<IActionResult> GetUsuarios()
        {
            // Incluir roles para obtener el nombre del rol asignado
            var usuarios = await _context.Usuarios
                .Include(u => u.Roles)
                .ToListAsync();

            // Mapeamos cada usuario a un objeto con los datos necesarios
            var usuariosDto = usuarios.Select(u => new
            {
                UsuarioId = u.UsuarioId,
                NombreCompleto = u.NombreCompleto,
                Email = u.Email,
                EmailVerificado = u.EmailVerificado,
                FechaRegistro = u.FechaRegistro,
                // Se asume que cada usuario tiene al menos un rol; en caso contrario, se muestra "Sin rol"
                Rol = u.Roles.FirstOrDefault()?.RoleName ?? "Sin rol"
            });

            return Ok(usuariosDto);
        }

        // POST: api/usuarios/register (registro de usuario con verificación de email)
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.NombreCompleto) ||
                string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Password))
            {
                return BadRequest("Todos los campos son requeridos.");
            }

            // Validar formato de email
            if (!IsValidEmail(request.Email))
            {
                return BadRequest("El correo electrónico no es válido.");
            }

            // Genera el hash de la contraseña
            var passwordHash = HashPassword(request.Password);

            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string verificationCode = null;
            int newUserId = 0;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("sp_InsertUserVerify", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@NombreCompleto", request.NombreCompleto);
                    command.Parameters.AddWithValue("@Email", request.Email);
                    command.Parameters.AddWithValue("@Password", passwordHash);

                    // Establece un timeout para evitar bloqueos indefinidos
                    command.CommandTimeout = 60;

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            newUserId = reader.GetInt32(0);
                            // El SP retorna dos columnas: UsuarioId (índice 0) y CodigoVerificacion (índice 1)
                            if (!reader.IsDBNull(1))
                            {
                                verificationCode = reader.GetString(1);
                            }
                        }
                        else
                        {
                            // Si no se retorna ningún registro, se puede registrar un log o retornar error.
                            return StatusCode(500, "El procedimiento almacenado no retornó datos.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al registrar el usuario: " + ex.Message);
            }

            // Si se generó el código de verificación, envía el correo
            if (!string.IsNullOrEmpty(verificationCode))
            {
                // Construir el enlace de verificación (ajusta el dominio según corresponda)
                string verificationLink = $"{Request.Scheme}://{Request.Host}/verificar?token={verificationCode}";
                string emailBody = $"<p>Gracias por registrarte. Por favor, verifica tu correo haciendo clic en el siguiente enlace:</p>" +
                                   $"<p><a href='{verificationLink}'>Verificar Correo</a></p>";

                try
                {
                    await _emailService.SendEmailAsync(request.Email, "Verificación de Correo", emailBody);
                }
                catch (Exception ex)
                {
                    return StatusCode(500, "Usuario registrado, pero falló el envío del correo: " + ex.Message);
                }
            }

            return Ok(new { Message = "Usuario registrado. Por favor, revisa tu correo para verificar tu cuenta.", UsuarioId = newUserId });
        }


        // Nuevo endpoint para que el administrador cree un usuario sin verificación (usando SP)
        [HttpPost("createByAdmin")]
        public async Task<IActionResult> CreateUserByAdmin([FromBody] NewUserAdminRequest request)
        {
            if (request == null)
                return BadRequest("Datos inválidos.");

            // Generar el hash de la contraseña
            var passwordHash = HashPassword(request.Password);

            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("sp_InsertUserByAdmin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    command.Parameters.AddWithValue("@NombreCompleto", request.NombreCompleto);
                    command.Parameters.AddWithValue("@Email", request.Email);
                    command.Parameters.AddWithValue("@PasswordHash", passwordHash);
                    command.Parameters.AddWithValue("@RoleName", request.RoleName);

                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            int usuarioId = reader.GetInt32(0);
                            return Ok(new { Message = "Usuario creado exitosamente.", UsuarioId = usuarioId });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al crear el usuario: " + ex.Message);
            }
            return BadRequest("Error al crear el usuario.");
        }

        [HttpPost("solicitarRecuperacion")]
        public async Task<IActionResult> SolicitarRecuperacion([FromBody] PasswordRecoveryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
            {
                return BadRequest("El correo electrónico es inválido.");
            }

            // Buscar el usuario en la base de datos
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == request.Email);
            if (usuario == null)
            {
                // Respuesta genérica para no revelar si el email existe o no
                return Ok("Si existe una cuenta asociada a este correo, se enviará un código de recuperación.");
            }

            // Generar un código de 4 dígitos
            var random = new Random();
            var code = random.Next(1000, 10000).ToString(); // Garantiza 4 dígitos

            // Establecer la expiración del código (por ejemplo, 1 hora)
            var expiry = DateTime.UtcNow.AddHours(1);

            // Insertar el código en la tabla PasswordResetCodes
            var resetCode = new PasswordResetCode
            {
                UsuarioId = usuario.UsuarioId,
                Code = code,
                ExpiryDate = expiry
            };
            _context.PasswordResetCodes.Add(resetCode);
            await _context.SaveChangesAsync();

            // Construir el mensaje de correo
            string emailBody = $"<p>Para restablecer tu contraseña, utiliza el siguiente código de recuperación:</p>" +
                               $"<h2>{code}</h2>" +
                               $"<p>Este código expirará en 1 hora.</p>";

            try
            {
                await _emailService.SendEmailAsync(request.Email, "Recuperación de Contraseña", emailBody);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "No se pudo enviar el correo de recuperación: " + ex.Message);
            }

            return Ok("Si existe una cuenta asociada a este correo, se enviará un código de recuperación.");
        }

        [HttpPost("restablecer")]
        public async Task<IActionResult> RestablecerPassword([FromBody] PasswordResetRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Code) || string.IsNullOrWhiteSpace(request.NewPassword))
            {
                return BadRequest("El código y la nueva contraseña son requeridos.");
            }

            // Buscar el registro de código de recuperación
            var resetRecord = await _context.PasswordResetCodes
                .Include(r => r.Usuario)
                .FirstOrDefaultAsync(r => r.Code == request.Code);

            if (resetRecord == null || resetRecord.ExpiryDate < DateTime.UtcNow)
            {
                return BadRequest("El código es inválido o ha expirado.");
            }

            // Actualizar la contraseña del usuario (asegúrate de aplicar el hash)
            resetRecord.Usuario.PasswordHash = HashPassword(request.NewPassword);

            // Eliminar el registro del código (o puedes marcarlo como usado)
            _context.PasswordResetCodes.Remove(resetRecord);
            await _context.SaveChangesAsync();

            return Ok("Contraseña actualizada correctamente.");
        }


        // DELETE: api/usuarios/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            _context.Usuarios.Remove(usuario);
            await _context.SaveChangesAsync();

            return Ok("Usuario eliminado correctamente.");
        }


        // Nuevo endpoint para listar MIS DATOS del usuario logueado
        // GET: api/usuarios/misdatos/{id}
        [HttpGet("misdatos/{id}")]
        public async Task<IActionResult> GetMisDatos(int id)
        {
            var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.UsuarioId == id);
            if (usuario == null)
                return NotFound("Usuario no encontrado.");

            var response = new
            {
                usuario.UsuarioId,
                usuario.NombreCompleto,
                usuario.Email,
                usuario.EmailVerificado
            };
            return Ok(response);
        }

        // Métodos auxiliares
        private bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
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
