namespace WebParfum.API.Models.Admin_Ventas
{
    public class VentaCompletaDTO
    {
        public int VentaId { get; set; }
        public int UsuarioId { get; set; }
        public string ClienteName { get; set; }
        public int AdminId { get; set; }
        public DateTime FechaCompra { get; set; }
        public decimal Total { get; set; }
        public string Estado { get; set; }
        public List<VentaDetalleCompletoDTO> Detalles { get; set; }
    }
}
