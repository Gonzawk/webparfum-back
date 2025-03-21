namespace WebParfum.API.Models.Ventas
{
    public class VentaDetalleDTO
    {
        public int VentaDetalleId { get; set; }
        public int PerfumeId { get; set; }
        public string PerfumeName { get; set; } // Nuevo: nombre del producto
        public string Marca { get; set; }         // Nuevo: nombre de la marca
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }

    public class VentaDTO
    {
        public int VentaId { get; set; }
        public int UsuarioId { get; set; }
        public int AdminId { get; set; }
        public string AdminName { get; set; } // Nuevo: nombre del administrador asignado
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public List<VentaDetalleDTO> Detalles { get; set; }
    }
}
