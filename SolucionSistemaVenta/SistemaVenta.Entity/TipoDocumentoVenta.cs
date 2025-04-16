using System;
using System.Collections.Generic;

namespace SistemaVenta.Entity;

public partial class TipoDocumentoVenta
{
    public int IdTipoDocumentoVenta { get; set; }

    public string? Descripcion { get; set; }

    public bool? EsActivo { get; set; }

    public DateTime? FechaRegistro { get; set; }

    public virtual ICollection<Venta> Venta { get; set; } = new List<Venta>();
}
