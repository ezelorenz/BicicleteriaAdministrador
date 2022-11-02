using AutoMapper;
using Bicicleteria_Gestor.APP.Models.ViewModels;
using Bicicleteria_Gestor.APP.Utilidades.Response;
using Bicicleteria_Gestor.BLL.Implementacion;
using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace Bicicleteria_Gestor.APP.Controllers
{
    [Authorize]
    public class ProductoController : Controller
    {
        private readonly IMapper _mapper;
        private readonly IProductoService _productoService;

        public ProductoController(IMapper mapper, 
            IProductoService productoService)
        {
            _mapper = mapper;
            _productoService = productoService;
        }
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Lista()
        {
            List<VMProducto> vmListaProductos = _mapper.Map<List<VMProducto>>(await _productoService.Lista());
            return StatusCode(StatusCodes.Status200OK,
                new
                {
                    data = vmListaProductos
                });
        }

        [HttpPost]
        public async Task<IActionResult> Crear([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();

            try
            {

                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

                string nombreImagen = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreImagen = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();
                }

                Producto producto_creado = await _productoService.Crear(_mapper.Map<Producto>(vmProducto), fotoStream, nombreImagen);
                vmProducto = _mapper.Map<VMProducto>(producto_creado);

                gResponse.Estado = true;
                gResponse.Objeto = vmProducto;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpPut]
        public async Task<IActionResult> Editar([FromForm] IFormFile foto, [FromForm] string modelo)
        {
            GenericResponse<VMProducto> gResponse = new GenericResponse<VMProducto>();

            try
            {

                VMProducto vmProducto = JsonConvert.DeserializeObject<VMProducto>(modelo);

                string nombreImagen = "";
                Stream fotoStream = null;

                if (foto != null)
                {
                    string nombre_en_codigo = Guid.NewGuid().ToString("N");
                    string extension = Path.GetExtension(foto.FileName);
                    nombreImagen = string.Concat(nombre_en_codigo, extension);
                    fotoStream = foto.OpenReadStream();//Se convierte la foto que recibimos a Stream
                }

                Producto producto_editado = await _productoService.Editar(_mapper.Map<Producto>(vmProducto), fotoStream, nombreImagen);
                vmProducto = _mapper.Map<VMProducto>(producto_editado);

                gResponse.Estado = true;
                gResponse.Objeto = vmProducto;

            }
            catch (Exception ex)
            {
                gResponse.Estado = false;
                gResponse.Mensaje = ex.Message;
            }
            return StatusCode(StatusCodes.Status200OK, gResponse);
        }

        [HttpDelete]
        public async Task<IActionResult> Eliminar(int idProducto)
        {
            GenericResponse<string> gResponse = new GenericResponse<string>();

            try
            {
                gResponse.Estado = await _productoService.Eliminar(idProducto);
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
