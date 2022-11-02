using Bicicleteria_Gestor.APP.Models.ViewModels;
using Bicicleteria_Gestor.APP.Utilidades.Response;
using Bicicleteria_Gestor.BLL.Implementacion;
using Bicicleteria_Gestor.BLL.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bicicleteria_Gestor.APP.Controllers
{
    [Authorize]
    public class DashBoardController : Controller
    {
        private readonly IDashboardService _dashBoardService;
        public DashBoardController(IDashboardService dashBoardService)
        {
            _dashBoardService = dashBoardService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ObtenerResumen()
        {
            GenericResponse<VMDashBoard> gResponse = new GenericResponse<VMDashBoard>();
            try
            {
                VMDashBoard vmDashBoard = new VMDashBoard();

                vmDashBoard.TotalVentas = await _dashBoardService.TotalVentasUltimaSemana();
                vmDashBoard.TotalIngresos = await _dashBoardService.TotalIngresosUltimaSemana();
                vmDashBoard.TotalProductos = await _dashBoardService.TotalProductos();

                List<VMVentasSemana> listaVentaSemana = new List<VMVentasSemana>();

                foreach(KeyValuePair<string, int> item in await _dashBoardService.VentasUltimaSemana())
                {
                    listaVentaSemana.Add(new VMVentasSemana()
                    {
                        Fecha = item.Key,
                        Total = item.Value
                    });
                }

                vmDashBoard.VentasUltimaSemana = listaVentaSemana;

                gResponse.Estado = true;
                gResponse.Objeto = vmDashBoard;

            }
            catch(Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }

            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
