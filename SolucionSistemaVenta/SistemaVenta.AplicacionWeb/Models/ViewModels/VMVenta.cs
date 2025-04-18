using SistemaVenta.Entity;

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

        public decimal? SubTotal { get; set; }

        public decimal? ImpuestoTotal { get; set; }

        public decimal? Total { get; set; }

        public DateTime? FechaRegistro { get; set; }

        public virtual ICollection<DetalleVenta> DetalleVenta { get; set; } = new List<DetalleVenta>();

    }
}
