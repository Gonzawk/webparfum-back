using WebParfum.API.Models;
using WebParfum.API.Models.Ventas;

public partial class Usuario
{
    public int UsuarioId { get; set; }
    public string NombreCompleto { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string PasswordHash { get; set; } = null!;  // string en lugar de byte[]
    public bool? EmailVerificado { get; set; }
    public string? CodigoVerificacion { get; set; }
    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Venta> VentaAdmins { get; set; } = new List<Venta>();
    public virtual ICollection<Venta> VentaUsuarios { get; set; } = new List<Venta>();
    public virtual ICollection<Role> Roles { get; set; } = new List<Role>();
}
