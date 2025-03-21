using System;
using System.Collections.Generic;
using WebParfum.API.Models.Ventas;

namespace WebParfum.API.Models;

public partial class Perfume
{
    public int PerfumeId { get; set; }

    public int MarcaId { get; set; }

    public string Modelo { get; set; } = null!;

    public decimal PrecioMinorista { get; set; }

    public decimal PrecioMayorista { get; set; }

    public string Genero { get; set; } = null!;

    public string? Descripcion { get; set; }

    public int Volumen { get; set; }

    public int Stock { get; set; }

    public string? ImagenUrl { get; set; }

    public virtual Marca Marca { get; set; } = null!;

    public virtual ICollection<VentaDetalle> VentaDetalles { get; set; } = new List<VentaDetalle>();
}
