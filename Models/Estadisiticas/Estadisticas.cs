namespace WebParfum.API.Models.Estadisiticas
{
    // DTO para el resumen general de ventas
    public class ResumenVentasDTO
    {
        public int TotalVentas { get; set; }
        public decimal IngresosTotales { get; set; }
        public decimal TicketPromedio { get; set; }
    }

    // DTO para el top de perfumes más vendidos
    public class TopPerfumeDTO
    {
        public int PerfumeId { get; set; }
        public string Modelo { get; set; }
        public int TotalVendidos { get; set; }
        public decimal Ingresos { get; set; }
    }

    // DTO para ventas diarias
    public class VentasDiariasDTO
    {
        public DateTime Dia { get; set; }
        public int TotalVentas { get; set; }
        public decimal IngresosTotales { get; set; }
    }

    // DTO para ventas mensuales
    public class VentasMensualesDTO
    {
        public int Año { get; set; }
        public int Mes { get; set; }
        public int TotalVentas { get; set; }
        public decimal IngresosTotales { get; set; }
    }

}
