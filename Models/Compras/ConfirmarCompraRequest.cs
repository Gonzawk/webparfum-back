namespace WebParfum.API.Models.Compras
{
    public class CartItemDTO
    {
        public int PerfumeId { get; set; }
        public int Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
    }

    public class ConfirmarCompraRequest
    {
        public int UsuarioId { get; set; }
        public int AdminId { get; set; }
        public List<CartItemDTO> Items { get; set; }
    }
}
