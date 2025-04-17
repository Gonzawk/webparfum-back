// Controllers/DecantsController.cs
using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Data;
using WebParfum.API.Models.Decant;

[ApiController]
[Route("api/[controller]")]
public class DecantsController : ControllerBase
{
    private readonly AppDbContext _context;
    public DecantsController(AppDbContext context) => _context = context;

    // GET: api/Decants
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _context.Decants
            .FromSqlRaw("EXEC dbo.usp_ListarDecants")
            .AsNoTracking()
            .ToListAsync();

        return Ok(lista);
    }

    // GET: api/Decants/5
    [HttpGet("{id:int}")]
    public async Task<IActionResult> Obtener(int id)
    {
        var p = new SqlParameter("@Id", SqlDbType.Int) { Value = id };

        // Ejecuta el SP, trae la lista (0 o 1 elemento) y luego toma el primero
        var resultados = await _context.Decants
            .FromSqlRaw("EXEC dbo.usp_BuscarDecantPorId @Id", p)
            .AsNoTracking()
            .ToListAsync();

        var item = resultados.FirstOrDefault();
        if (item == null) return NotFound();

        return Ok(item);
    }

    // POST: api/Decants
    // Se apoya en el DEFAULT de la columna CodigoQR (NEWID()) definido en la BD
    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] DecantCreateDto dto)
    {
        var decant = new Decant
        {
            Nombre = dto.Nombre,
            CantidadDisponible = dto.CantidadDisponible,
            UrlImagen = dto.UrlImagen,
            Estado = dto.Estado
            // Nota: CodigoQR no se asigna aquí, la BD usará su DF para generarlo
        };

        _context.Decants.Add(decant);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(Obtener),
            new { id = decant.Id },
            new { id = decant.Id }
        );
    }

    // PUT: api/Decants/5
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Editar(int id, [FromBody] DecantUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest();

        var pId = new SqlParameter("@Id", SqlDbType.Int) { Value = dto.Id };
        var pNombre = new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre };
        var pQr = new SqlParameter("@CodigoQR", SqlDbType.VarChar, 100) { Value = dto.CodigoQR };
        var pCant = new SqlParameter("@CantidadDisponible", SqlDbType.Int) { Value = dto.CantidadDisponible };
        var pUrl = new SqlParameter("@UrlImagen", SqlDbType.NVarChar, 200)
        {
            Value = dto.UrlImagen is null ? DBNull.Value : dto.UrlImagen
        };
        var pEst = new SqlParameter("@Estado", SqlDbType.TinyInt) { Value = dto.Estado };

        await _context.Database.ExecuteSqlRawAsync(
            @"EXEC dbo.usp_EditarDecant
                @Id                 = @Id,
                @Nombre             = @Nombre,
                @CodigoQR           = @CodigoQR,
                @CantidadDisponible = @CantidadDisponible,
                @UrlImagen          = @UrlImagen,
                @Estado             = @Estado",
            pId, pNombre, pQr, pCant, pUrl, pEst
        );

        return NoContent();
    }

    // DELETE: api/Decants/5
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Eliminar(int id)
    {
        var p = new SqlParameter("@Id", SqlDbType.Int) { Value = id };
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.usp_EliminarDecant @Id = @Id",
            p
        );
        return NoContent();
    }
}
