using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.DAL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;

namespace Bicicleteria_Gestor.BLL.Implementacion
{
    public class DashBoardService : IDashboardService
    {
        private readonly IVentaRepository _repoVenta;
        private readonly IGenericRepository<Producto> _repoProducto;
        private DateTime FechaInicio = DateTime.Now;

        public DashBoardService(IVentaRepository repoVenta,  
            IGenericRepository<Producto> repoProducto)
        {
            _repoVenta = repoVenta;
            _repoProducto = repoProducto;
            FechaInicio = FechaInicio.AddDays(-7);
        }

        public async Task<int> TotalVentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repoVenta.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<string> TotalIngresosUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repoVenta.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);
                decimal resultado = query.Select(v => v.Total).Sum(v => v.Value);
                return Convert.ToString(resultado);
            }
            catch
            {
                throw;
            }
        }

        public async Task<int> TotalProductos()
        {
            try
            {
                IQueryable<Producto> query = await _repoProducto.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);
                int total = query.Count();
                return total;
            }
            catch
            {
                throw;
            }
        }

        public async Task<Dictionary<string, int>> VentasUltimaSemana()
        {
            try
            {
                IQueryable<Venta> query = await _repoVenta.Consultar(v => v.FechaRegistro.Value.Date >= FechaInicio.Date);

                Dictionary<string, int> resultado = query.GroupBy(v => v.FechaRegistro.Value.Date).OrderByDescending(g => g.Key)
                                                    .Select(dv => new
                                                    {
                                                        fecha = dv.Key.ToString(),
                                                        total = dv.Count()
                                                    })
                                                    .ToDictionary(keySelector: r => r.fecha,
                                                                  elementSelector: r => r.total);

                return resultado;
            }
            catch
            {
                throw;
            }
        }
    }
}
