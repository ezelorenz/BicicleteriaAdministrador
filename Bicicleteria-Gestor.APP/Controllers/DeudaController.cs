using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bicicleteria_Gestor.APP.Controllers
{
    [Authorize]
    public class DeudaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
