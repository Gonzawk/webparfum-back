namespace WebParfum.API.Models.Admin_Ventas
{
    public class VentaDetalleCompletoDTO
    {
        public int VentaDetalleId { get; set; }
        public int PerfumeId { get; set; }
        public string PerfumeName { get; set; }
        public string Marca { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Subtotal { get; set; }
    }
}
