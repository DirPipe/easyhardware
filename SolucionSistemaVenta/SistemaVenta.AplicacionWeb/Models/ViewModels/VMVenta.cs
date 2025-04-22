using SistemaVenta.Entity;
using System.Text.Json.Serialization;

namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMVenta
    {
        public int IdVenta { get; set; }

        public string? NumeroVenta { get; set; }

        public int? IdTipoDocumentoVenta { get; set; }
        public string? TipoDocumentoVenta { get; set; }

        public int? IdUsuario { get; set; }

        public string? Usuario { get; set; }

        public string? DocumentoCliente { get; set; }

        public string? NombreCliente { get; set; }

        public string? SubTotal { get; set; }

        public string? ImpuestoTotal { get; set; }

        public string? Total { get; set; }

        public string? FechaRegistro { get; set; }

        // se retiro la directiva "[JsonIgnore]" que arreglaba bug de error 500 perpetuo, pero creaba nuevo bug al solicitar el metodo historial del controlador venta {devolvia array vacio}
        public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    }
}
