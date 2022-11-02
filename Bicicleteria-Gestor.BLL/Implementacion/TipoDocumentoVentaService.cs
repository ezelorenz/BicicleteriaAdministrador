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
    public class TipoDocumentoVentaService : ITipoDocumentoVentaService
    {
        private readonly IGenericRepository<TipoDocumentoVenta> repo;
        public TipoDocumentoVentaService(IGenericRepository<TipoDocumentoVenta> repositorio)
        {
            repo = repositorio;
        }
        public async Task<List<TipoDocumentoVenta>> Lista()
        {
            IQueryable<TipoDocumentoVenta> query = await repo.Consultar();
            return query.ToList();
        }
    }
}
