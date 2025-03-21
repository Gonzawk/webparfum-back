using System;
using System.Collections.Generic;

namespace WebParfum.API.Models;

public partial class Marca
{
    public int MarcaId { get; set; }

    public string Nombre { get; set; } = null!;

    public virtual ICollection<Perfume> Perfumes { get; set; } = new List<Perfume>();
}
