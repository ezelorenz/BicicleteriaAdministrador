using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.DAL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;

namespace Bicicleteria_Gestor.BLL.Implementacion
{
    public class ClienteService : IClienteService
    {
        private readonly IGenericRepository<Cliente> repo;
        public ClienteService(IGenericRepository<Cliente> repositorio)
        {
            repo = repositorio;
        }
        public async Task<List<Cliente>> Lista()
        {
            IQueryable<Cliente> query = await repo.Consultar();
            return query.ToList();
        }
        public async Task<Cliente> Crear(Cliente entidad)
        {
            try
            {
                Cliente cliente_creado = await repo.Crear(entidad);
                if (cliente_creado.IdCliente == 0)
                    throw new TaskCanceledException("No se pudo crear el Cliente");

                return cliente_creado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Cliente> Editar(Cliente entidad)
        {
            try
            {
                Cliente cliente_encontrado = await repo.Obtener(c => c.IdCliente == entidad.IdCliente);

                cliente_encontrado.Documento = entidad.Documento;
                cliente_encontrado.NombreCompleto = entidad.NombreCompleto;
                cliente_encontrado.Ciudad = entidad.Ciudad;
                cliente_encontrado.Direccion = entidad.Direccion;
                cliente_encontrado.Deudor = entidad.Deudor;

                bool respuesta = await repo.Editar(cliente_encontrado);

                if (!respuesta)
                    throw new TaskCanceledException("No se pudo editar el Cliente");

                return cliente_encontrado;
            }
            catch
            {
                throw;
            }
        }

        public async Task<bool> Eliminar(int idCliente)
        {
            try
            {
                Cliente cliente_encontrado = await repo.Obtener(c => c.IdCliente == idCliente);

                if (cliente_encontrado == null)
                    throw new TaskCanceledException("El Cliente no existe");

                bool respuesta = await repo.Eliminar(cliente_encontrado);
                return respuesta;
            }
            catch
            {
                throw;
            }
        }
    }
}
