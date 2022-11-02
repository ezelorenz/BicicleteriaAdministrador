using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.DAL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using Microsoft.EntityFrameworkCore;

namespace Bicicleteria_Gestor.BLL.Implementacion
{
    public class ProductoService : IProductoService
    {
        private readonly IGenericRepository<Producto> _repo;
        private readonly IFireBaseService _fireBaseService;
        public ProductoService(IGenericRepository<Producto> repositorio, 
                                IFireBaseService fireBaseService)
        {
            _repo = repositorio;
            _fireBaseService = fireBaseService;
        }

        public async Task<List<Producto>> Lista()
        {
            IQueryable<Producto> query = await _repo.Consultar();
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }

        public async Task<Producto> Crear(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            //Se valida que el producto exista o no en base al Codigo de Barra
            Producto producto_existe = await _repo.Obtener(p=> p.CodigoBarra == entidad.CodigoBarra);

            if (producto_existe != null)
                throw new TaskCanceledException("El codigo de barra ya existe");

            try
            {
                entidad.NombreImagen = nombreImagen;
                if(imagen != null)
                {
                    string urlImagen = await _fireBaseService.SubirStorage(imagen, "carpeta_producto", nombreImagen);
                    entidad.UrlImagen = urlImagen;
                }

                Producto producto_creado = await _repo.Crear(entidad);

                if(producto_creado.IdProducto == 0)
                    throw new TaskCanceledException("No se pudo crear el Producto");

                IQueryable<Producto> query = await _repo.Consultar(p => p.IdProducto == entidad.IdProducto);
                producto_creado = query.Include(c => c.IdCategoriaNavigation).First();

                return producto_creado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Producto> Editar(Producto entidad, Stream imagen = null, string nombreImagen = "")
        {
            Producto producto_existe = await _repo.Obtener(p=> p.CodigoBarra == entidad.CodigoBarra && p.IdProducto != entidad.IdProducto);

            if(producto_existe != null)
                throw new TaskCanceledException("El codigo de barra ya está asignado a otro Producto");

            try
            {
                IQueryable<Producto> queryProducto = await _repo.Consultar(p => p.IdProducto == entidad.IdProducto);

                Producto producto_para_editar = queryProducto.First();

                producto_para_editar.CodigoBarra = entidad.CodigoBarra;
                producto_para_editar.Marca = entidad.Marca;
                producto_para_editar.Descripcion = entidad.Descripcion;
                producto_para_editar.IdCategoria = entidad.IdCategoria;
                producto_para_editar.Stock = entidad.Stock;
                producto_para_editar.Precio = entidad.Precio;
                producto_para_editar.EsActivo = entidad.EsActivo;

                if(producto_para_editar.NombreImagen == "")
                {
                    producto_para_editar.NombreImagen = nombreImagen;
                }

                if (imagen != null)
                {
                    string urlImagen = await _fireBaseService.SubirStorage(imagen, "carpeta_producto", producto_para_editar.NombreImagen);
                    producto_para_editar.UrlImagen = urlImagen;
                }

                bool respuesta = await _repo.Editar(producto_para_editar);
                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar el Producto");

                Producto producto_editado = queryProducto.Include(c => c.IdCategoriaNavigation).First();
                return producto_editado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idProducto)
        {
            try
            {
                Producto producto_encontrado = await _repo.Obtener(p => p.IdProducto == idProducto);

                if(producto_encontrado == null)
                    throw new TaskCanceledException("No existe el Producto");

                string nombreImagen = producto_encontrado.NombreImagen;

                bool respuesta = await _repo.Eliminar(producto_encontrado);
                if (respuesta)
                {
                    await _fireBaseService.EliminarStorage("carpeta_producto", nombreImagen);
                }
                return true;
            }
            catch
            {
                throw;
            }
        }
    }
}
