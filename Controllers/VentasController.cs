using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WebParfum.API.Data;
using WebParfum.API.Models;
using System.Collections.Generic;
using WebParfum.API.Models.Ventas;
using WebParfum.API.Models.Admin_Ventas;
using WebParfum.API.Models.Compras;

namespace WebParfum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VentasController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public VentasController(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        [HttpPost("confirmar-compra")]
        public async Task<IActionResult> ConfirmarCompra([FromBody] ConfirmarCompraRequest request)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string mensaje;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("ConfirmarCompra", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;

                    // Parámetros simples
                    command.Parameters.AddWithValue("@UsuarioId", request.UsuarioId);
                    command.Parameters.AddWithValue("@AdminId", request.AdminId);

                    // Crear DataTable para los ítems del carrito
                    DataTable cartItemsTable = new DataTable();
                    cartItemsTable.Columns.Add("PerfumeId", typeof(int));
                    cartItemsTable.Columns.Add("Cantidad", typeof(int));
                    cartItemsTable.Columns.Add("PrecioUnitario", typeof(decimal));

                    foreach (var item in request.Items)
                    {
                        cartItemsTable.Rows.Add(item.PerfumeId, item.Cantidad, item.PrecioUnitario);
                    }

                    var cartItemsParam = command.Parameters.AddWithValue("@CartItems", cartItemsTable);
                    cartItemsParam.SqlDbType = SqlDbType.Structured;
                    cartItemsParam.TypeName = "dbo.CartItemType";

                    // Parámetro de salida para el mensaje
                    var mensajeParam = new SqlParameter("@Mensaje", SqlDbType.NVarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(mensajeParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    mensaje = mensajeParam.Value?.ToString() ?? "No se obtuvo respuesta del SP.";
                }

                return Ok(new { Message = mensaje });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al procesar la compra: " + ex.Message);
            }
        }

        // GET: api/ventas/lista
        [HttpGet("lista")]
        public async Task<IActionResult> GetVentas()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            var ventasDict = new Dictionary<int, VentaDTO>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("GetVentasSuperadmin", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int ventaId = reader.GetInt32(reader.GetOrdinal("VentaId"));
                            if (!ventasDict.ContainsKey(ventaId))
                            {
                                var ventaDto = new VentaDTO
                                {
                                    VentaId = ventaId,
                                    UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                                    AdminId = reader.GetInt32(reader.GetOrdinal("AdminId")),
                                    AdminName = reader.IsDBNull(reader.GetOrdinal("AdminName"))
                                                ? reader.GetInt32(reader.GetOrdinal("AdminId")).ToString()
                                                : reader.GetString(reader.GetOrdinal("AdminName")),
                                    FechaCompra = reader.GetDateTime(reader.GetOrdinal("FechaCompra")),
                                    Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                                    Estado = reader.GetString(reader.GetOrdinal("Estado")),
                                    Detalles = new List<VentaDetalleDTO>()
                                };
                                ventasDict.Add(ventaId, ventaDto);
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("VentaDetalleId")))
                            {
                                var detalle = new VentaDetalleDTO
                                {
                                    VentaDetalleId = reader.GetInt32(reader.GetOrdinal("VentaDetalleId")),
                                    PerfumeId = reader.GetInt32(reader.GetOrdinal("PerfumeId")),
                                    PerfumeName = reader.IsDBNull(reader.GetOrdinal("PerfumeName"))
                                                    ? reader.GetInt32(reader.GetOrdinal("PerfumeId")).ToString()
                                                    : reader.GetString(reader.GetOrdinal("PerfumeName")),
                                    Marca = reader.IsDBNull(reader.GetOrdinal("Marca"))
                                                    ? string.Empty
                                                    : reader.GetString(reader.GetOrdinal("Marca")),
                                    Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                                    Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal"))
                                };
                                ventasDict[ventaId].Detalles.Add(detalle);
                            }
                        }
                    }
                }
                return Ok(ventasDict.Values.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener ventas: " + ex.Message);
            }
        }

        // PUT: api/ventas/{ventaId}/confirmar
        [HttpPut("{ventaId}/confirmar")]
        public async Task<IActionResult> ConfirmSale(int ventaId, [FromQuery] int adminId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string mensaje;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("sp_ConfirmSale", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VentaId", ventaId);
                    command.Parameters.AddWithValue("@AdminId", adminId);

                    var mensajeParam = new SqlParameter("@Mensaje", SqlDbType.NVarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(mensajeParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    mensaje = mensajeParam.Value?.ToString() ?? "No se obtuvo respuesta del SP.";
                }

                return Ok(new { Message = mensaje });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al confirmar la venta: " + ex.Message);
            }
        }

        // PUT: api/ventas/{ventaId}/finalizar
        [HttpPut("{ventaId}/finalizar")]
        public async Task<IActionResult> FinalizarVenta(int ventaId, [FromQuery] int adminId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string mensaje;
            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("FinalizarVenta", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VentaId", ventaId);
                    command.Parameters.AddWithValue("@AdminId", adminId);

                    var mensajeParam = new SqlParameter("@Mensaje", SqlDbType.NVarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(mensajeParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    mensaje = mensajeParam.Value?.ToString() ?? "No se obtuvo respuesta del SP.";
                }
                return Ok(new { Message = mensaje });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error finalizando la venta: " + ex.Message);
            }
        }

        // NUEVO MÉTODO: PUT: api/ventas/{ventaId}/cancelar
        [HttpPut("{ventaId}/cancelar")]
        public async Task<IActionResult> CancelarVenta(int ventaId, [FromQuery] int adminId)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            string mensaje;

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("sp_CancelarVenta", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@VentaId", ventaId);
                    command.Parameters.AddWithValue("@AdminId", adminId);

                    var mensajeParam = new SqlParameter("@Mensaje", SqlDbType.NVarChar, 200)
                    {
                        Direction = ParameterDirection.Output
                    };
                    command.Parameters.Add(mensajeParam);

                    await connection.OpenAsync();
                    await command.ExecuteNonQueryAsync();

                    mensaje = mensajeParam.Value?.ToString() ?? "No se obtuvo respuesta del SP.";
                }
                return Ok(new { Message = mensaje });
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error cancelando la venta: " + ex.Message);
            }
        }

        // GET: api/ventas/asignadas/{adminId}
        [HttpGet("asignadas/{adminId}")]
        public async Task<IActionResult> GetAssignedSales(int adminId)
        {
            var ventas = await _context.Ventas
                .Include(v => v.VentaDetalles)
                .Where(v => v.AdminId == adminId)
                .ToListAsync();

            var ventasDto = ventas.Select(v => new VentaDTO
            {
                VentaId = v.VentaId,
                UsuarioId = v.UsuarioId,
                AdminId = v.AdminId,
                FechaCompra = (DateTime)v.FechaCompra,
                Total = v.Total,
                Estado = v.Estado,
                Detalles = v.VentaDetalles.Select(d => new VentaDetalleDTO
                {
                    VentaDetalleId = d.VentaDetalleId,
                    PerfumeId = d.PerfumeId,
                    Cantidad = d.Cantidad,
                    PrecioUnitario = d.PrecioUnitario,
                    Subtotal = (decimal)d.Subtotal
                }).ToList()
            });

            return Ok(ventasDto);
        }

        // GET: api/ventas/admins
        [HttpGet("admins")]
        public async Task<IActionResult> GetAdmins()
        {
            var admins = await _context.Usuarios
                .Include(u => u.Roles)
                .Where(u => u.Roles.Any(r => r.RoleName.ToLower() == "admin"))
                .Select(u => new
                {
                    u.UsuarioId,
                    u.NombreCompleto,
                    u.Email
                })
                .ToListAsync();

            return Ok(admins);
        }


        // GET: api/ventas/lista-completa
        [HttpGet("lista-completa")]
        public async Task<IActionResult> GetVentasCompletas()
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            var ventasDict = new Dictionary<int, VentaCompletaDTO>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("GetVentasCompleta", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    await connection.OpenAsync();
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            int ventaId = reader.GetInt32(reader.GetOrdinal("VentaId"));
                            if (!ventasDict.ContainsKey(ventaId))
                            {
                                var ventaDto = new VentaCompletaDTO
                                {
                                    VentaId = ventaId,
                                    UsuarioId = reader.GetInt32(reader.GetOrdinal("UsuarioId")),
                                    ClienteName = reader.GetString(reader.GetOrdinal("ClienteName")),
                                    AdminId = reader.GetInt32(reader.GetOrdinal("AdminId")),
                                    FechaCompra = reader.GetDateTime(reader.GetOrdinal("FechaCompra")),
                                    Total = reader.GetDecimal(reader.GetOrdinal("Total")),
                                    Estado = reader.GetString(reader.GetOrdinal("Estado")),
                                    Detalles = new List<VentaDetalleCompletoDTO>()
                                };
                                ventasDict.Add(ventaId, ventaDto);
                            }

                            if (!reader.IsDBNull(reader.GetOrdinal("VentaDetalleId")))
                            {
                                var detalle = new VentaDetalleCompletoDTO
                                {
                                    VentaDetalleId = reader.GetInt32(reader.GetOrdinal("VentaDetalleId")),
                                    PerfumeId = reader.GetInt32(reader.GetOrdinal("PerfumeId")),
                                    PerfumeName = reader.GetString(reader.GetOrdinal("PerfumeName")),
                                    Marca = reader.GetString(reader.GetOrdinal("Marca")),
                                    Cantidad = reader.GetInt32(reader.GetOrdinal("Cantidad")),
                                    PrecioUnitario = reader.GetDecimal(reader.GetOrdinal("PrecioUnitario")),
                                    Subtotal = reader.GetDecimal(reader.GetOrdinal("Subtotal"))
                                };
                                ventasDict[ventaId].Detalles.Add(detalle);
                            }
                        }
                    }
                }
                return Ok(ventasDict.Values.ToList());
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener ventas completas: " + ex.Message);
            }
        }
    }
}
