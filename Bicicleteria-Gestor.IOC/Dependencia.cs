using Bicicleteria_Gestor.BLL.Implementacion;
using Bicicleteria_Gestor.BLL.Interfaces;
using Bicicleteria_Gestor.DAL;
using Bicicleteria_Gestor.DAL.Implementacion;
using Bicicleteria_Gestor.DAL.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Bicicleteria_Gestor.IOC
{
    public static class Dependencia
    {
        public static void InyectarDependencia(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DbTiendaContext>(options =>
                                    options.UseSqlServer(configuration.GetConnectionString("cadenaSQL")));

            services.AddTransient(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.AddScoped<IVentaRepository, VentaRepository>();
            services.AddScoped<ICorreoService, CorreoService>();
            services.AddScoped<IFireBaseService, FireBaseService>();
            services.AddScoped<IUtilidadesService, UtilidadesService>();
            services.AddScoped<IRolService, RolService>();
            services.AddScoped<IUsuarioService, UsuarioService>();
            services.AddScoped<ICategoriaService, CategoriaService>();
            services.AddScoped<IProductoService, ProductoService>();
            services.AddScoped<IVentaService, VentaService>();
            services.AddScoped<IClienteService, ClienteService>();
            services.AddScoped<IDashboardService, DashBoardService>();
        }
    }
}
