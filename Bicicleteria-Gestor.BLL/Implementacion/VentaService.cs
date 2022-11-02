using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.DAL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace Bicicleteria_Gestor.BLL.Implementacion
{
    public class VentaService : IVentaService
    {
        private readonly IGenericRepository<Producto> _repositorioProducto;
        private readonly IVentaRepository _repositorioVenta;

        public VentaService(IGenericRepository<Producto> repositorioProducto, IVentaRepository repositorioVenta)
        {
            _repositorioProducto = repositorioProducto;
            _repositorioVenta = repositorioVenta;
        }

        public async Task<List<Producto>> ObtenerProductos(string busqueda)
        {
            IQueryable<Producto> query = await _repositorioProducto.Consultar(p=>
                                            p.EsActivo == true &&
                                            p.Stock > 0 &&
                                            string.Concat(p.CodigoBarra,p.Marca,p.Descripcion).Contains(busqueda));
            return query.Include(c => c.IdCategoriaNavigation).ToList();
        }
        public async Task<Venta> Registrar(Venta entidad)
        {
            try
            {
                return await _repositorioVenta.Registrar(entidad);
            }
            catch
            {
                throw;
            }
        }

        public async Task<Venta> Detalle(string numeroVenta)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar(v =>v.NumeroVenta == numeroVenta);

               return  query
                            .Include(u => u.IdUsuarioNavigation)
                            .Include(dv => dv.DetalleVenta)
                            .First();
        }

        public async Task<List<Venta>> Historial(string numeroVenta, string fechaInicio, string fechaFin)
        {
            IQueryable<Venta> query = await _repositorioVenta.Consultar();
            fechaInicio = fechaInicio is null ? "" : fechaInicio;
            fechaFin = fechaFin is null ? "" : fechaFin;

            //return query.ToList();

            if (fechaInicio != "" && fechaFin != "")
            {
                //, "dd/mm/yyyy", new CultureInfo("es-PE")
                //, "dd/mm/yyyy", new CultureInfo("es-PE")
                DateTime fech_inicio = DateTime.Parse(fechaInicio);
                DateTime fech_fin = DateTime.Parse(fechaFin);

                return query.Where(v =>
                    v.FechaRegistro.Value.Date >= fech_inicio &&
                    v.FechaRegistro.Value.Date <= fech_fin)
                    .Include(u => u.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }
            else
            {
                return query.Where(v => v.NumeroVenta == numeroVenta)
                    .Include(u => u.IdUsuarioNavigation)
                    .Include(dv => dv.DetalleVenta)
                    .ToList();
            }

        }

        public async Task<List<DetalleVenta>> Reporte(string fechaInicio, string fechaFin)
        {
            DateTime fech_inicio = DateTime.ParseExact(fechaInicio, "dd/mm/yyyy", new CultureInfo("es-AR"));
            DateTime fech_fin = DateTime.ParseExact(fechaFin, "dd/mm/yyyy", new CultureInfo("es-AR"));

            List<DetalleVenta> lista = await _repositorioVenta.Reporte(fech_inicio, fech_fin);

            return lista;
        }
    }
}
