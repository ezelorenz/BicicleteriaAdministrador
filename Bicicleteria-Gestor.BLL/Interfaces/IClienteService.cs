using Bicicleteria_Gestor.ENTIDADES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicicleteria_Gestor.BLL.Interfaces
{
    public interface IClienteService
    {
        Task<List<Cliente>> Lista();
        Task<Cliente> Crear(Cliente entidad);
        Task<Cliente> Editar(Cliente entidad);
        Task<bool> Eliminar(int idCliente);
    }
}
