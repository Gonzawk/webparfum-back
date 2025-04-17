// Controllers/DecantsController.cs
using System;
using System.Data;
using System.Threading.Tasks;
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

    // GET all
    [HttpGet]
    public async Task<IActionResult> Listar()
    {
        var lista = await _context.Decants
            .FromSqlRaw("EXEC dbo.usp_ListarDecants")
            .ToListAsync();
        return Ok(lista);
    }

    // GET by Id
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Obtener(Guid id)
    {
        var p = new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id };
        var item = await _context.Decants
            .FromSqlRaw("EXEC dbo.usp_BuscarDecantPorId @Id", p)
            .FirstOrDefaultAsync();
        if (item == null) return NotFound();
        return Ok(item);
    }

    // POST
    [HttpPost]
    public async Task<IActionResult> Agregar([FromBody] DecantCreateDto dto)
    {
        var p1 = new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre };
        var p2 = new SqlParameter("@CodigoQR", SqlDbType.VarChar, 100) { Value = dto.CodigoQR };
        var p3 = new SqlParameter("@CantidadDisponible", SqlDbType.Int) { Value = dto.CantidadDisponible };
        var p4 = new SqlParameter("@UrlImagen", SqlDbType.NVarChar, 200) { Value = (object)dto.UrlImagen ?? DBNull.Value };
        var p5 = new SqlParameter("@Estado", SqlDbType.TinyInt) { Value = dto.Estado };

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.usp_AgregarDecant @Nombre, @CodigoQR, @CantidadDisponible, @UrlImagen, @Estado",
            p1, p2, p3, p4, p5
        );
        return NoContent();
    }

    // PUT
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Editar(Guid id, [FromBody] DecantUpdateDto dto)
    {
        if (id != dto.Id) return BadRequest();

        var p0 = new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = dto.Id };
        var p1 = new SqlParameter("@Nombre", SqlDbType.NVarChar, 100) { Value = dto.Nombre };
        var p2 = new SqlParameter("@CodigoQR", SqlDbType.VarChar, 100) { Value = dto.CodigoQR };
        var p3 = new SqlParameter("@CantidadDisponible", SqlDbType.Int) { Value = dto.CantidadDisponible };
        var p4 = new SqlParameter("@UrlImagen", SqlDbType.NVarChar, 200) { Value = (object)dto.UrlImagen ?? DBNull.Value };
        var p5 = new SqlParameter("@Estado", SqlDbType.TinyInt) { Value = dto.Estado };

        await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.usp_EditarDecant @Id, @Nombre, @CodigoQR, @CantidadDisponible, @UrlImagen, @Estado",
            p0, p1, p2, p3, p4, p5
        );
        return NoContent();
    }

    // DELETE
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Eliminar(Guid id)
    {
        var p = new SqlParameter("@Id", SqlDbType.UniqueIdentifier) { Value = id };
        await _context.Database.ExecuteSqlRawAsync(
            "EXEC dbo.usp_EliminarDecant @Id",
            p
        );
        return NoContent();
    }
}
