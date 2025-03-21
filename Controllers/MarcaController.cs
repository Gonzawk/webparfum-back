using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Data;
using WebParfum.API.Models; // Asegúrate de que la entidad Marca esté en este namespace

namespace WebParfum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MarcaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public MarcaController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Marca
        [HttpGet]
        public async Task<IActionResult> GetMarcas()
        {
            var marcas = await _context.Marcas.ToListAsync();
            return Ok(marcas);
        }

        // POST: api/Marca
        [HttpPost]
        public async Task<IActionResult> CreateMarca([FromBody] Marca marca)
        {
            if (marca == null || string.IsNullOrWhiteSpace(marca.Nombre))
            {
                return BadRequest("El nombre de la marca es requerido.");
            }

            _context.Marcas.Add(marca);
            await _context.SaveChangesAsync();
            return Ok(marca);
        }

        // DELETE: api/Marca/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMarca(int id)
        {
            var marca = await _context.Marcas.FindAsync(id);
            if (marca == null)
            {
                return NotFound("Marca no encontrada.");
            }

            _context.Marcas.Remove(marca);
            await _context.SaveChangesAsync();
            return Ok("Marca eliminada correctamente.");
        }
    }
}
