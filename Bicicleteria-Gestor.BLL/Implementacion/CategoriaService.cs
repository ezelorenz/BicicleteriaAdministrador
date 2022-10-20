using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.DAL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using XAct;

namespace Bicicleteria_Gestor.BLL.Implementacion
{
    public class CategoriaService : ICategoriaService
    {
        private readonly IGenericRepository<Categoria> repo;
        public CategoriaService(IGenericRepository<Categoria> repositorio)
        {
            repo = repositorio;
        }
        public async Task<List<Categoria>> Lista()
        {
            IQueryable<Categoria> query = await repo.Consultar();
            return query.ToList();
        }

        public async Task<Categoria> Crear(Categoria entidad)
        {
            try
            {
                Categoria categoria_creada = await repo.Crear(entidad);
                if (categoria_creada.IdCategoria == 0)
                    throw new TaskCanceledException("No se pudo crear la Categoria");

                return categoria_creada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Categoria> Editar(Categoria entidad)
        {
            try
            {
                Categoria categoria_encontrada = await repo.Obtener(c => c.IdCategoria == entidad.IdCategoria);

                categoria_encontrada.Descripcion = entidad.Descripcion;
                categoria_encontrada.EsActivo = entidad.EsActivo;

                bool respuesta = await repo.Editar(categoria_encontrada);

                if(!respuesta)
                    throw new TaskCanceledException("No se pudo editar la Categoria");

                return categoria_encontrada;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idCategoria)
        {
            try
            {
                Categoria categoria_encontrada = await repo.Obtener(c => c.IdCategoria == idCategoria);

                if (categoria_encontrada == null)
                    throw new TaskCanceledException("La Categoria no existe");

                bool respuesta = await repo.Eliminar(categoria_encontrada);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }

    }
}
