using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Data;
using WebParfum.API.DTOs; // Asegúrate de que PerfumeDTO esté en este namespace
using WebParfum.API.Models;
using System.Linq;

namespace WebParfum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly AppDbContext _context;
        public ProductsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Products
        [HttpGet]
        public async Task<IActionResult> GetProducts()
        {
            // Llamar al stored procedure sp_GetProducts
            var products = await _context.Perfumes
                .FromSqlRaw("EXEC sp_GetProducts")
                .ToListAsync();

            // Mapear la entidad Perfume a PerfumeDTO
            var productsDto = products.Select(p => new PerfumeDTO
            {
                PerfumeId = p.PerfumeId,
                MarcaId = p.MarcaId,
                Modelo = p.Modelo,
                PrecioMinorista = p.PrecioMinorista,
                PrecioMayorista = p.PrecioMayorista,
                Genero = p.Genero,
                Descripcion = p.Descripcion,
                Volumen = p.Volumen,
                Stock = p.Stock,
                ImagenUrl = p.ImagenUrl
            }).ToList();

            return Ok(productsDto);
        }

        // POST: api/Products
        [HttpPost]
        public async Task<IActionResult> CreateProduct([FromBody] PerfumeDTO productDto)
        {
            if (productDto == null)
                return BadRequest("El producto es requerido.");

            // Mapear el DTO a la entidad Perfume
            var product = new Perfume
            {
                // PerfumeId es identity, se omite
                MarcaId = productDto.MarcaId,
                Modelo = productDto.Modelo,
                PrecioMinorista = productDto.PrecioMinorista,
                PrecioMayorista = productDto.PrecioMayorista,
                Genero = productDto.Genero,
                Descripcion = productDto.Descripcion,
                Volumen = productDto.Volumen,
                Stock = productDto.Stock,
                ImagenUrl = productDto.ImagenUrl
            };

            _context.Perfumes.Add(product);
            await _context.SaveChangesAsync();

            // Mapear la entidad guardada a DTO para devolverla
            var resultDto = new PerfumeDTO
            {
                PerfumeId = product.PerfumeId,
                MarcaId = product.MarcaId,
                Modelo = product.Modelo,
                PrecioMinorista = product.PrecioMinorista,
                PrecioMayorista = product.PrecioMayorista,
                Genero = product.Genero,
                Descripcion = product.Descripcion,
                Volumen = product.Volumen,
                Stock = product.Stock,
                ImagenUrl = product.ImagenUrl
            };

            return Ok(resultDto);
        }

        // PUT: api/Products/{id}
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProduct(int id, [FromBody] PerfumeDTO productDto)
        {
            if (productDto == null)
                return BadRequest("El producto es requerido.");

            var product = await _context.Perfumes.FindAsync(id);
            if (product == null)
                return NotFound("Producto no encontrado.");

            // Actualizar las propiedades desde el DTO
            product.MarcaId = productDto.MarcaId;
            product.Modelo = productDto.Modelo;
            product.PrecioMinorista = productDto.PrecioMinorista;
            product.PrecioMayorista = productDto.PrecioMayorista;
            product.Genero = productDto.Genero;
            product.Descripcion = productDto.Descripcion;
            product.Volumen = productDto.Volumen;
            product.Stock = productDto.Stock;
            product.ImagenUrl = productDto.ImagenUrl;

            _context.Entry(product).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Perfumes.Any(p => p.PerfumeId == id))
                    return NotFound("Producto no encontrado.");
                throw;
            }

            var resultDto = new PerfumeDTO
            {
                PerfumeId = product.PerfumeId,
                MarcaId = product.MarcaId,
                Modelo = product.Modelo,
                PrecioMinorista = product.PrecioMinorista,
                PrecioMayorista = product.PrecioMayorista,
                Genero = product.Genero,
                Descripcion = product.Descripcion,
                Volumen = product.Volumen,
                Stock = product.Stock,
                ImagenUrl = product.ImagenUrl
            };

            return Ok(resultDto);
        }

        // DELETE: api/Products/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _context.Perfumes.FindAsync(id);
            if (product == null)
                return NotFound("Producto no encontrado.");

            _context.Perfumes.Remove(product);
            await _context.SaveChangesAsync();
            return Ok("Producto eliminado correctamente.");
        }
    }
}
