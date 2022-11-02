using Bicicleteria_Gestor.DAL.Interfaces;
using Bicicleteria_Gestor.ENTIDADES;
using Microsoft.EntityFrameworkCore;

namespace Bicicleteria_Gestor.DAL.Implementacion
{
    public class VentaRepository : GenericRepository<Venta>, IVentaRepository
    {
        private readonly DbTiendaContext db;
        public VentaRepository(DbTiendaContext context) : base(context)
        {
            db = context;
        }

        public async Task<Venta> Registrar(Venta entidad)
        {
            Venta ventaGenerada = new Venta();
            using (var transaction = db.Database.BeginTransaction())
            {
                try
                {
                    foreach (DetalleVenta dv in entidad.DetalleVenta)
                    {
                        Producto producto_encontrado = db.Productos.Where(p => p.IdProducto == dv.IdProducto).First();
                        producto_encontrado.Stock = producto_encontrado.Stock - dv.Cantidad;
                        db.Productos.Update(producto_encontrado);
                    }
                    await db.SaveChangesAsync();

                    NumeroCorrelativo correlativo = db.NumeroCorrelativo.Where(n => n.Gestion == "venta").First();
                    correlativo.UltimoNumero = correlativo.UltimoNumero + 1;
                    correlativo.FechaActualizacion = DateTime.Now;

                    db.NumeroCorrelativo.Update(correlativo);
                    await db.SaveChangesAsync();

                    string ceros = string.Concat(Enumerable.Repeat("0", correlativo.CantidadDigitos.Value));
                    string numeroVenta = ceros + correlativo.UltimoNumero.ToString();
                    numeroVenta = numeroVenta.Substring(numeroVenta.Length - correlativo.CantidadDigitos.Value,
                                                         correlativo.CantidadDigitos.Value);

                    entidad.NumeroVenta = numeroVenta;
                    await db.Venta.AddAsync(entidad);
                    await db.SaveChangesAsync();
                    ventaGenerada = entidad;

                    transaction.Commit();
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw ex;
                }
                return ventaGenerada;
            }
        }

        public async Task<List<DetalleVenta>> Reporte(DateTime fechaInicio, DateTime fechaFin)
        {

            /*
                    listaResumen va a recibir desde base de datos: 
                        tabla Venta con:
                            tabla Usuario
                            
                            tabla Cliente
            
             */


            List<DetalleVenta> listaResumen = await db.DetalleVenta
                                                .Include(v => v.IdVentaNavigation)
                                                .ThenInclude(u => u.IdUsuarioNavigation)
                                                .Include(v => v.IdVentaNavigation)
                                                .ThenInclude(c => c.IdClienteNavigation)
                                                .Where(dv => dv.IdVentaNavigation.FechaRegistro.Value.Date >= fechaInicio.Date
                                                && dv.IdVentaNavigation.FechaRegistro.Value.Date <= fechaFin.Date)
                                                .ToListAsync();
            return listaResumen;
        }

        //public async Task<List<>>
    }
}
