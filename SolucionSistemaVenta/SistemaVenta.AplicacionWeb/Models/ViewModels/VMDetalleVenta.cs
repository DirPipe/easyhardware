using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMDetalleVenta
    {

        public int? IdProducto { get; set; }

        public string? MarcaProducto { get; set; }

        public string? DescripcionProducto { get; set; }

        public string? CategoriaProducto { get; set; }

        public int? Cantidad { get; set; }

        public decimal? Precio { get; set; }

        public decimal? Total { get; set; }

    }
}
