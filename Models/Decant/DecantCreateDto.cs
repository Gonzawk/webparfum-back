namespace WebParfum.API.Models.Decant
{
    public class DecantCreateDto
    {
        public string Nombre { get; set; }
        public int CantidadDisponible { get; set; }
        public string UrlImagen { get; set; }  // puede ser null
        public byte Estado { get; set; }
    }

}
