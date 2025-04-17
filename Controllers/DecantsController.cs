// Controllers/DecantsController.cs
using System;
using System.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Data;
using WebParfum.API.Models.Decant;

namespace WebParfum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DecantsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public DecantsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/decants
        [HttpGet]
        public async Task<IActionResult> Listar()
        {
            var lista = await _context.Decants
                .FromSqlRaw("EXEC dbo.usp_ListarDecants")
                .ToListAsync();
            return Ok(lista);
        }

        // GET: api/decants/{id}
        [HttpGet("{id:int}")]
        public async Task<IActionResult> Obtener(int id)
        {
            var p = new SqlParameter("@Id", SqlDbType.Int) { Value = id };
            var item = await _context.Decants
                .FromSqlRaw("EXEC dbo.usp_BuscarDecantPorId @Id", p)
                .FirstOrDefaultAsync();
            if (item == null) return NotFound();
            return Ok(item);
        }

        // POST: api/decants
        // Inserta y devuelve el nuevo Id
        [HttpPost]
        public async Task<IActionResult> Agregar([FromBody] DecantCreateDto dto)
        {
            // Abrimos la conexión ADO.NET
            var conn = _context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            // Ejecutamos el SP y luego leemos el SCOPE_IDENTITY()
            cmd.CommandText = @"
                EXEC dbo.usp_AgregarDecant 
                    @Nombre, @CodigoQR, @CantidadDisponible, @UrlImagen, @Estado;
                SELECT CAST(SCOPE_IDENTITY() AS INT);
            ";
            cmd.CommandType = CommandType.Text;

            cmd.Parameters.Add(new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre });
            // placeholder en servidor; el frontend calculará el URL del QR
            cmd.Parameters.Add(new SqlParameter("@CodigoQR", SqlDbType.VarChar, 100) { Value = "" });
            cmd.Parameters.Add(new SqlParameter("@CantidadDisponible", SqlDbType.Int) { Value = dto.CantidadDisponible });
            cmd.Parameters.Add(new SqlParameter("@UrlImagen", SqlDbType.NVarChar, 200) { Value = (object)dto.UrlImagen ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@Estado", SqlDbType.TinyInt) { Value = dto.Estado });

            var result = await cmd.ExecuteScalarAsync();
            await conn.CloseAsync();

            if (result == null || !int.TryParse(result.ToString(), out var newId))
                return StatusCode(500, "No se pudo obtener el ID del nuevo decant.");

            // Devolver 201 Created con la ruta GET /api/decants/{newId}
            return CreatedAtAction(nameof(Obtener), new { id = newId }, new { id = newId });
        }

        // PUT: api/decants/{id}
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Editar(int id, [FromBody] DecantUpdateDto dto)
        {
            if (id != dto.Id) return BadRequest("El ID de la ruta y del cuerpo no coinciden.");

            await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.usp_EditarDecant @Id, @Nombre, @CodigoQR, @CantidadDisponible, @UrlImagen, @Estado",
                new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id },
                new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre },
                new SqlParameter("@CodigoQR", SqlDbType.VarChar, 100) { Value = dto.CodigoQR },
                new SqlParameter("@CantidadDisponible", SqlDbType.Int) { Value = dto.CantidadDisponible },
                new SqlParameter("@UrlImagen", SqlDbType.NVarChar, 200) { Value = (object)dto.UrlImagen ?? DBNull.Value },
                new SqlParameter("@Estado", SqlDbType.TinyInt) { Value = dto.Estado }
            );
            return NoContent();
        }

        // DELETE: api/decants/{id}
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            await _context.Database.ExecuteSqlRawAsync(
                "EXEC dbo.usp_EliminarDecant @Id",
                new SqlParameter("@Id", SqlDbType.Int) { Value = id }
            );
            return NoContent();
        }
    }
}
