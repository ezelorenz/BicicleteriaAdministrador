using AutoMapper;
using Bicicleteria_Gestor.APP.Models.ViewModels;
using Bicicleteria_Gestor.APP.Utilidades.Response;
using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Bicicleteria_Gestor.APP.Controllers
{
    [Authorize]
    public class ClienteController : Controller
    {
        private readonly IClienteService _clienteService;
        private readonly IMapper _mapper;
        public ClienteController(IClienteService clienteService, IMapper mapper)
        {
            _clienteService = clienteService;
            _mapper = mapper;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMCliente> vmListaClientes = _mapper.Map<List<VMCliente>>(await _clienteService.Lista());
            return StatusCode(StatusCodes.Status200OK,
                new
                {
                    data = vmListaClientes
                });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromBody] VMCliente modelo)
        {
            GenericResponse<VMCliente> gResponse = new GenericResponse<VMCliente>();
            try
            {
                Cliente cliente_creado = await _clienteService.Crear(_mapper.Map<Cliente>(modelo));
                modelo = _mapper.Map<VMCliente>(cliente_creado);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromBody] VMCliente modelo)
        {
            GenericResponse<VMCliente> gResponse = new GenericResponse<VMCliente>();
            try
            {
                Cliente cliente_editado = await _clienteService.Editar(_mapper.Map<Cliente>(modelo));
                modelo = _mapper.Map<VMCliente>(cliente_editado);

                gResponse.Estado = true;
                gResponse.Objeto = modelo;
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idCliente)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _clienteService.Eliminar(idCliente);
            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }
    }
}
