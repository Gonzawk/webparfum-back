namespace WebParfum.API.Models.Decant
{
    public class DecantUpdateDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; }
        public string CodigoQR { get; set; }
        public int CantidadDisponible { get; set; }
        public string UrlImagen { get; set; }
        public int Estado { get; set; }
    }
}
