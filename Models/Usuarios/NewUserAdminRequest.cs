namespace WebParfum.API.Models.Usuarios
{
    public class NewUserAdminRequest
    {
        public string NombreCompleto { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string RoleName { get; set; } // Por ejemplo: "Superadmin", "Admin" o "Usuario"
    }
}
