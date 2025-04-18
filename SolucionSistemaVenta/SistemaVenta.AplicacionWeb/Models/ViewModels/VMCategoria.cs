using SistemaVenta.Entity;

namespace SistemaVenta.AplicacionWeb.Models.ViewModels
{
    public class VMCategoria
    {
        public int IdCategoria { get; set; }

        public string? Descripcion { get; set; }

        public int? EsActivo { get; set; }

    }
}
