using AutoMapper;
using Bicicleteria_Gestor.APP.Models.ViewModels;
using Bicicleteria_Gestor.APP.Utilidades.Response;
using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Bicicleteria_Gestor.APP.Controllers
{
    [Authorize]
    public class VentaController : Controller
    {
        private readonly IVentaService _ventaService;
        private readonly IMapper _mapper;

        public VentaController(IVentaService ventaService,  
                               IMapper mapper)
        {
            _ventaService = ventaService;
            _mapper = mapper;
        }

        public IActionResult NuevaVenta()
        {
            return View();
        }
        public IActionResult HistorialVenta()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerProductos(string busqueda)
        {
            List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(await _ventaService.ObtenerProductos(busqueda));
            return StatusCode(StatusCodes.Status200OK, vmListaProductos);
        }

        [HttpPost]
        public async Task<IActionResult> RegistrarVenta([FromBody] VMVenta modelo)
        {
            GenericResponse<VMVenta> gResponse = new GenericResponse<VMVenta>();

            try
            {
                ClaimsPrincipal claimUser = HttpContext.User;

                string idUsuario = claimUser.Claims
                                    .Where(c => c.Type == ClaimTypes.NameIdentifier)
                                    .Select(c => c.Value).SingleOrDefault();

                modelo.IdUsuario = int.Parse(idUsuario);

                Venta venta_creada = await _ventaService.Registrar(_mapper.Map<Venta>(modelo));
                modelo = _mapper.Map<VMVenta>(venta_creada);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpGet]
        public async Task<IActionResult> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            List<VMVenta> vmHistorialVenta = _mapper.Map<List<VMVenta>>(await _ventaService.Historial(numeroVenta, fechaInicio, fechaFin));
            return StatusCode(StatusCodes.Status200OK, vmHistorialVenta);
        }
    }
}
