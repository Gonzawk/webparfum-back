using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using WebParfum.API.Models.Estadisiticas;

namespace WebParfum.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EstadisticasController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public EstadisticasController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // GET: api/estadisticas/resumen?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD
        [HttpGet("resumen")]
        public async Task<IActionResult> GetResumen([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("usp_GetVentasResumen", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            var resumen = new ResumenVentasDTO
                            {
                                TotalVentas = reader["TotalVentas"] != DBNull.Value ? Convert.ToInt32(reader["TotalVentas"]) : 0,
                                IngresosTotales = reader["IngresosTotales"] != DBNull.Value ? Convert.ToDecimal(reader["IngresosTotales"]) : 0,
                                TicketPromedio = reader["TicketPromedio"] != DBNull.Value ? Convert.ToDecimal(reader["TicketPromedio"]) : 0
                            };

                            return Ok(resumen);
                        }
                    }
                }

                return NotFound("No se encontraron datos para el rango especificado.");
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener el resumen: " + ex.Message);
            }
        }

        // GET: api/estadisticas/topperfumes?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD&top=10
        [HttpGet("topperfumes")]
        public async Task<IActionResult> GetTopPerfumes([FromQuery] DateTime startDate, [FromQuery] DateTime endDate, [FromQuery] int top = 10)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            var topPerfumes = new List<TopPerfumeDTO>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("usp_GetTopPerfumes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);
                    command.Parameters.AddWithValue("@Top", top);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var perfume = new TopPerfumeDTO
                            {
                                PerfumeId = reader["PerfumeId"] != DBNull.Value ? Convert.ToInt32(reader["PerfumeId"]) : 0,
                                Modelo = reader["Modelo"].ToString(),
                                TotalVendidos = reader["TotalVendidos"] != DBNull.Value ? Convert.ToInt32(reader["TotalVendidos"]) : 0,
                                Ingresos = reader["Ingresos"] != DBNull.Value ? Convert.ToDecimal(reader["Ingresos"]) : 0
                            };

                            topPerfumes.Add(perfume);
                        }
                    }
                }

                return Ok(topPerfumes);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener el top de perfumes: " + ex.Message);
            }
        }

        // GET: api/estadisticas/ventasdiarias?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD
        [HttpGet("ventasdiarias")]
        public async Task<IActionResult> GetVentasDiarias([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            var ventasDiarias = new List<VentasDiariasDTO>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("usp_GetVentasPorDia", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var venta = new VentasDiariasDTO
                            {
                                Dia = Convert.ToDateTime(reader["Dia"]),
                                TotalVentas = reader["TotalVentas"] != DBNull.Value ? Convert.ToInt32(reader["TotalVentas"]) : 0,
                                IngresosTotales = reader["IngresosTotales"] != DBNull.Value ? Convert.ToDecimal(reader["IngresosTotales"]) : 0
                            };

                            ventasDiarias.Add(venta);
                        }
                    }
                }

                return Ok(ventasDiarias);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener las ventas diarias: " + ex.Message);
            }
        }

        // GET: api/estadisticas/ventasmensuales?startDate=YYYY-MM-DD&endDate=YYYY-MM-DD
        [HttpGet("ventasmensuales")]
        public async Task<IActionResult> GetVentasMensuales([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            string connectionString = _configuration.GetConnectionString("DefaultConnection");
            var ventasMensuales = new List<VentasMensualesDTO>();

            try
            {
                using (var connection = new SqlConnection(connectionString))
                using (var command = new SqlCommand("usp_GetVentasPorMes", connection))
                {
                    command.CommandType = CommandType.StoredProcedure;
                    command.Parameters.AddWithValue("@StartDate", startDate);
                    command.Parameters.AddWithValue("@EndDate", endDate);

                    await connection.OpenAsync();

                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            var ventaMes = new VentasMensualesDTO
                            {
                                Año = reader["Año"] != DBNull.Value ? Convert.ToInt32(reader["Año"]) : 0,
                                Mes = reader["Mes"] != DBNull.Value ? Convert.ToInt32(reader["Mes"]) : 0,
                                TotalVentas = reader["TotalVentas"] != DBNull.Value ? Convert.ToInt32(reader["TotalVentas"]) : 0,
                                IngresosTotales = reader["IngresosTotales"] != DBNull.Value ? Convert.ToDecimal(reader["IngresosTotales"]) : 0
                            };

                            ventasMensuales.Add(ventaMes);
                        }
                    }
                }

                return Ok(ventasMensuales);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Error al obtener las ventas mensuales: " + ex.Message);
            }
        }
    }
}
