namespace WebParfum.API.Models.Decant
{
    public class Decant
    {
        public int Id { get; set; }   // antes Guid
        public string Nombre { get; set; }
        public string CodigoQR { get; set; }
        public int CantidadDisponible { get; set; }
        public string UrlImagen { get; set; }
        public byte Estado { get; set; }
        public DateTimeOffset FechaCreacion { get; set; }
    }
}
