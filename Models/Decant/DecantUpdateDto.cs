namespace WebParfum.API.Models.Decant
{
    public class DecantUpdateDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string UrlImagen { get; set; }
        public int CantidadDisponible { get; set; }
        public byte Estado { get; set; }
        public string CodigoQR { get; set; }
    }
}
