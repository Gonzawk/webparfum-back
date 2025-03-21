namespace WebParfum.API.Models.Usuarios
{
    public class UserRegistrationRequest
    {
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
