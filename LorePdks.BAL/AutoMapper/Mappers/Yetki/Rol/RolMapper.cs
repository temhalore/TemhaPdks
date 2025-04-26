using AutoMapper;
using LorePdks.COMMON.DTO.Yetki.Rol;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.Yetki.Rol
{
    public class RolMapper : MappingProfile
    {
        public RolMapper()
        {
            CreateMap<t_rol, RolDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.rolAdi, opt => opt.MapFrom(src => src.ROL_ADI))
                .ForMember(dest => dest.aciklama, opt => opt.MapFrom(src => src.ACIKLAMA))
                .ForMember(dest => dest.controllerName, opt => opt.MapFrom(src => src.CONTROLLER_NAME))
                .ForMember(dest => dest.controllerMethodName, opt => opt.MapFrom(src => src.CONTROLLER_METHOD_NAME)) 
                .ForMember(dest => dest.rolAdi, opt => opt.MapFrom(src => src.ROL_ADI))
                .ForMember(dest => dest.ekranlar, opt => opt.Ignore());

            CreateMap<RolDTO, t_rol>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.ROL_ADI, opt => opt.MapFrom(src => src.rolAdi))
                .ForMember(dest => dest.ACIKLAMA, opt => opt.MapFrom(src => src.aciklama))
                .ForMember(dest => dest.CONTROLLER_NAME , opt => opt.MapFrom(src => src.controllerName))
                .ForMember(dest => dest.CONTROLLER_METHOD_NAME , opt => opt.MapFrom(src => src.controllerMethodName))
                ;
        }
    }
}