using Microsoft.AspNetCore.Mvc;

namespace Bicicleteria_Gestor.APP.Controllers
{
    public class ProductoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
