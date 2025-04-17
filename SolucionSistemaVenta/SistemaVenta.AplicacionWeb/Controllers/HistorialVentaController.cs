using Microsoft.AspNetCore.Mvc;

namespace SistemaVenta.AplicacionWeb.Controllers
{
    public class HistorialVentaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
