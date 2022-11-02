using AutoMapper;
using Bicicleteria_Gestor.APP.Models;
using Bicicleteria_Gestor.APP.Models.ViewModels;
using Bicicleteria_Gestor.APP.Utilidades.Response;
using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Security.Claims;

namespace Bicicleteria_Gestor.APP.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IUsuarioService _usuarioService;
        private readonly IMapper _mapper;

        public HomeController(IUsuarioService usuarioService, IMapper mapper)
        {
            _usuarioService = usuarioService;
            _mapper = mapper;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerUsuario()
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                                    .Select(c => c.Value).SingleOrDefault();

                VMUsuario usuario = _mapper.Map<VMUsuario>(await _usuarioService.ObtenerProId(int.Parse(idUsuario)));

                gResponse.Estado = true;
                gResponse.Objeto = usuario; 
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> GuardarPerfil([FromBody] VMUsuario modelo)
        {
            GenericResponse<VMUsuario> gResponse = new GenericResponse<VMUsuario>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                                    .Select(c => c.Value).SingleOrDefault();

                Usuario entidad = _mapper.Map<Usuario>(modelo);

                entidad.IdUsuario = int.Parse(idUsuario);

                bool resultado = await _usuarioService.GuardarPerfil(entidad);

                gResponse.Estado = true;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPost]
        public async Task<IActionResult> CambiarClave([FromBody] VMCambiarClave modelo)
        {
            GenericResponse<bool> gResponse = new GenericResponse<bool>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                                    .Select(c => c.Value).SingleOrDefault();

                

                bool resultado = await _usuarioService.CambiarClave(int.Parse(idUsuario),
                                                                    modelo.ClaveActual,
                                                                    modelo.ClaveNueva);

                gResponse.Estado = true;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Perfil()
        {
            return View();
        }

        public async Task<IActionResult> Salir()
        {
            //Terminar la sesion del usuario del sistema.
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Acceso");
        }
    }
}