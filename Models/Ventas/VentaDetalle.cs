using System;
using System.Collections.Generic;

namespace WebParfum.API.Models.Ventas;

public partial class VentaDetalle
{
    public int VentaDetalleId { get; set; }

    public int VentaId { get; set; }

    public int PerfumeId { get; set; }

    public int Cantidad { get; set; }

    public decimal PrecioUnitario { get; set; }

    public decimal? Subtotal { get; set; }

    public virtual Perfume Perfume { get; set; } = null!;

    public virtual Venta Venta { get; set; } = null!;
}
