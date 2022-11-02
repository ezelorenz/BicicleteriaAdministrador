using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bicicleteria_Gestor.APP.Utilidades.ViewComponents
{
    public class MenuUsuarioViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            ClaimsPrincipal claimUser = HttpContext.User;
            string nombreUsuario = "";
            string urlFotoUsuario = "";

            //Si el Usuario esta autenticado
            if (claimUser.Identity.IsAuthenticated)
            {
                //darle el nombre que recibe en AccesoController, metodo Login.
                nombreUsuario = claimUser.Claims
                                        .Where(c => c.Type == ClaimTypes.Name)
                                        .Select(c => c.Value).SingleOrDefault();

                urlFotoUsuario = ((ClaimsIdentity)claimUser.Identity).FindFirst("UrlFoto").Value;
            }

            ViewData["nombreUsuario"] = nombreUsuario;
            ViewData["urlFotoUsuario"] = urlFotoUsuario;

            return View();
        }
    }
}
