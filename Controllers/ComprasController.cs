using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Data;
using WebParfum.API.DTOs;
using WebParfum.API.Models;
using WebParfum.API.Models.Compras;

namespace WebParfum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ComprasController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ComprasController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/compras/miscompras/{userId}
        [HttpGet("miscompras/{userId}")]
        public async Task<IActionResult> GetMisCompras(int userId)
        {
            // Obtener las ventas del usuario junto con sus detalles
            var ventas = await _context.Ventas
                .Include(v => v.VentaDetalles)
                .Where(v => v.UsuarioId == userId)
                .ToListAsync();

            // Obtener los IDs de administradores asignados en las ventas
            var adminIds = ventas.Select(v => v.AdminId).Distinct().ToList();
            // Construir un diccionario con la información de los administradores
            var adminsDict = await _context.Usuarios
                .Where(u => adminIds.Contains(u.UsuarioId))
                .ToDictionaryAsync(u => u.UsuarioId, u => u.NombreCompleto);

            // Obtener los IDs de perfumes de los detalles
            var perfumeIds = ventas.SelectMany(v => v.VentaDetalles.Select(d => d.PerfumeId)).Distinct().ToList();
            // Construir un diccionario con la información de los perfumes (modelo y marca)
            var perfumesDict = await _context.Perfumes
                .Where(p => perfumeIds.Contains(p.PerfumeId))
                .ToDictionaryAsync(p => p.PerfumeId, p => new { p.Modelo, p.Marca });

            // Proyectar a DTO incluyendo AdminName, Modelo y Marca en cada detalle
            var ventasDto = ventas.Select(v => new MisComprasDTO
            {
                VentaId = v.VentaId,
                FechaCompra = (DateTime)v.FechaCompra,
                Total = v.Total,
                Estado = v.Estado,
                AdminName = adminsDict.ContainsKey(v.AdminId) ? adminsDict[v.AdminId] : v.AdminId.ToString(),
                Detalles = v.VentaDetalles.Select(d => new MisComprasDetalleDTO
                {
                    VentaDetalleId = d.VentaDetalleId,
                    PerfumeId = d.PerfumeId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = (decimal)d.Subtotal,
                    Modelo = perfumesDict.ContainsKey(d.PerfumeId) ? perfumesDict[d.PerfumeId].Modelo : "",
                    Marca = perfumesDict.ContainsKey(d.PerfumeId) ? Convert.ToString(perfumesDict[d.PerfumeId].Marca) : ""
                }).ToList()
            }).ToList();

            return Ok(ventasDto);
        }
    }
}
