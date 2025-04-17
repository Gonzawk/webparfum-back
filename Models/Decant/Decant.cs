namespace WebParfum.API.Models.Decant
{
    public class Decant
    {
        public int Id { get; set; }      // antes Guid
        public string Nombre { get; set; }      // NVARCHAR(100)
        public string CodigoQR { get; set; }      // VARCHAR(200) UNIQUE
        public int CantidadDisponible { get; set; }      // INT
        public string UrlImagen { get; set; }      // NVARCHAR(200), nullable
        public byte Estado { get; set; }      // TINYINT
        public DateTimeOffset FechaCreacion { get; set; }  // DATETIMEOFFSET
    }
}
