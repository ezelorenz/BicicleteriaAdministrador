using AutoMapper;
using Bicicleteria_Gestor.APP.Models.ViewModels;
using Bicicleteria_Gestor.ENTIDADES;

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
        }
    }
}
