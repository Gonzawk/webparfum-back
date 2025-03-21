namespace WebParfum.API.DTOs
{
    public class PerfumeDTO
    {
        public int PerfumeId { get; set; }
        public int MarcaId { get; set; }
        public string Modelo { get; set; }
        public decimal PrecioMinorista { get; set; }
        public decimal PrecioMayorista { get; set; }
        public string Genero { get; set; }
        public string? Descripcion { get; set; }
        public int Volumen { get; set; }
        public int Stock { get; set; }
        public string? ImagenUrl { get; set; }
    }
}
