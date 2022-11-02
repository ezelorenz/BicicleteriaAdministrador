using AutoMapper;
using Bicicleteria_Gestor.APP.Models.ViewModels;
using Bicicleteria_Gestor.ENTIDADES;
using System.Globalization;

namespace Bicicleteria_Gestor.APP.Utilidades.Automapper
{
    public class AutomapperProfile : Profile
    {
        public AutomapperProfile()
        {
            //CreateMap<Origen,Destino>();

            #region Rol

            CreateMap<Rol, VMRol>().ReverseMap();

            #endregion

            #region Usuario

            CreateMap<Usuario, VMUsuario>()
                .ForMember(d => d.EsActivo,
                            opt => opt.MapFrom(o => o.EsActivo == true ? 1 : 0))
                .ForMember(d => d.NombreRol,
                            opt => opt.MapFrom(o => o.IdRolNavigation.Descripcion));

            CreateMap<VMUsuario, Usuario>()
                .ForMember(d => d.EsActivo,
                            opt => opt.MapFrom(o => o.EsActivo == 1 ? true : false))
                .ForMember(d => d.IdRolNavigation,
                            opt => opt.Ignore());

            #endregion

            #region Categoria

            CreateMap<Categoria, VMCategoria>()
                .ForMember(d => d.EsActivo,
                            opt => opt.MapFrom(o => o.EsActivo == true ? 1 : 0));

            CreateMap<VMCategoria, Categoria>()
                .ForMember(d => d.EsActivo,
                            opt => opt.MapFrom(o => o.EsActivo == 1 ? true : false));
            #endregion

            #region Producto
            CreateMap<Producto, VMProducto>()
                .ForMember(d => d.EsActivo,
                            opt => opt.MapFrom(o => o.EsActivo == true ? 1 : 0))
                .ForMember(d => d.Precio,
                            opt => opt.MapFrom(o => Convert.ToString(o.Precio.Value, new CultureInfo("es-AR"))))
                .ForMember(d => d.NombreCategoria,
                            opt => opt.MapFrom(o => o.IdCategoriaNavigation.Descripcion));

            CreateMap<VMProducto, Producto>()
                .ForMember(d => d.EsActivo,
                            opt => opt.MapFrom(o => o.EsActivo == 1 ? true : false))
                .ForMember(d => d.Precio,
                            opt => opt.MapFrom(o => Convert.ToDecimal(o.Precio, new CultureInfo("es-AR"))))
                .ForMember(d => d.IdCategoriaNavigation,
                            opt => opt.Ignore());

            #endregion

            #region TipoDocumentoVenta
            

            CreateMap<VMTipoDocumentoVenta, TipoDocumentoVenta>().ReverseMap();
            #endregion

            #region Venta
            CreateMap<Venta, VMVenta>()
                .ForMember(d => d.Usuario,
                    opt => opt.MapFrom(o => o.IdUsuarioNavigation.Nombre))
                .ForMember(d => d.Total,
                    opt => opt.MapFrom(o => Convert.ToString(o.Total.Value, new CultureInfo("es-AR"))))
                .ForMember(d => d.FechaRegistro,
                    opt => opt.MapFrom(o => o.FechaRegistro.Value.ToString("dd/MM/yyyy")))
                .ForMember(d => d.NombreCliente,
                            opt => opt.MapFrom(o => o.IdClienteNavigation.NombreCompleto))
                .ForMember(d => d.DocumentoCliente,
                            opt => opt.MapFrom(o => o.IdClienteNavigation.Documento));


            CreateMap<VMVenta, Venta>()
                .ForMember(d => d.Total,
                    opt => opt.MapFrom(o => Convert.ToDecimal(o.Total, new CultureInfo("es-AR"))));
            #endregion

            #region DetalleVenta

            CreateMap<DetalleVenta, VMDetalleVenta>()
                .ForMember(d => d.Precio,
                    opt => opt.MapFrom(o => Convert.ToString(o.Precio.Value, new CultureInfo("es-AR"))))
                .ForMember(d => d.Total,
                    opt => opt.MapFrom(o => Convert.ToString(o.Total.Value, new CultureInfo("es-AR"))));

            CreateMap<VMDetalleVenta, DetalleVenta>()
                .ForMember(d => d.Precio,
                    opt => opt.MapFrom(o => Convert.ToDecimal(o.Precio, new CultureInfo("es-AR"))))
                .ForMember(d => d.Total,
                    opt => opt.MapFrom(o => Convert.ToDecimal(o.Total, new CultureInfo("es-AR"))));

            CreateMap<DetalleVenta, VMReporteVenta>()
                .ForMember(d => d.NumeroVenta,
                    opt => opt.MapFrom(o => o.IdVentaNavigation.NumeroVenta))
                .ForMember(d => d.TotalVenta,
                    opt => opt.MapFrom(o => Convert.ToString(o.IdVentaNavigation.Total.Value, new CultureInfo("es-AR"))))
                .ForMember(d => d.Producto,
                    opt => opt.MapFrom(o => o.DescripcionProducto));
            #endregion

            #region Cliente
            CreateMap<Cliente, VMCliente>()
                .ForMember(d => d.Deudor,
                            opt => opt.MapFrom(o => o.Deudor == true ? 1 : 0));

            CreateMap<VMCliente, Cliente>()
                .ForMember(d => d.Deudor,
                            opt => opt.MapFrom(o => o.Deudor == 1 ? true : false));
            #endregion

        }
    }
}
