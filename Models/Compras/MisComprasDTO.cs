namespace WebParfum.API.Models.Compras
{
    public class MisComprasDetalleDTO
    {
        public int VentaDetalleId { get; set; }
        public int PerfumeId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
        public string Modelo { get; set; }  // Nuevo
        public string Marca { get; set; }   // Nuevo
    }

    public class MisComprasDTO
    {
        public int VentaId { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public string AdminName { get; set; }  // Nuevo: Nombre del administrador asignado
        public List<MisComprasDetalleDTO> Detalles { get; set; }
    }
}
