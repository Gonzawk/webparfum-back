using System;
using System.Collections.Generic;

namespace WebParfum.API.Models.Ventas;

public partial class Venta
{
    public int VentaId { get; set; }

    public int UsuarioId { get; set; }

    public int AdminId { get; set; }

    public DateTime? FechaCompra { get; set; }

    public decimal Total { get; set; }

    public string Estado { get; set; } = null!;

    public virtual Usuario Admin { get; set; } = null!;

    public virtual Usuario Usuario { get; set; } = null!;

    public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
}
