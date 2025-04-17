namespace WebParfum.API.Models.Decant
{
    public class DecantCreateDto
    {
        public string Nombre { get; set; }
        public string UrlImagen { get; set; }
        public int CantidadDisponible { get; set; }
        public byte Estado { get; set; }
        // no incluyas Id ni CodigoQR
    }

}
