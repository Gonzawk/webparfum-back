namespace WebParfum.API.Models.Usuarios
{
    public class GetUserDataResponse
    {
        public int UsuarioId { get; set; }
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
      
        public string PasswordHash { get; set; }
    }
}
