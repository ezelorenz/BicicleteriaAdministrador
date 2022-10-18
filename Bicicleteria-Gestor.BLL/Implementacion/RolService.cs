using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.DAL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bicicleteria_Gestor.BLL.Implementacion
{
    public class RolService : IRolService
    {
        private readonly IGenericRepository<Rol> repo;

        public RolService(IGenericRepository<Rol> repositorio)
        {
            repo = repositorio;
        }

        //Se va a utilizar en el formulario de usuario, para asignarle un rol.
        public async Task<List<Rol>> Lista()
        {
            IQueryable<Rol> query = await repo.Consultar();
            return query.ToList();
        }
    }
}
