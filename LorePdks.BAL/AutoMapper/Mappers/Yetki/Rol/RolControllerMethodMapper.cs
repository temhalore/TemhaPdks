using AutoMapper;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Yetki.Rol;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.Yetki.Rol
{
    public class RolControllerMethodMapper : MappingProfile
    {
        public RolControllerMethodMapper()
        {
            CreateMap<t_rol_controller_method, RolControllerMethodDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.rolEidDto, opt => opt.MapFrom(src => new EIdDTO { id = Convert.ToInt32(src.ROL_ID) }))// src.ROL_ID))
                .ForMember(dest => dest.controllerName, opt => opt.MapFrom(src => src.CONTROLLER_NAME))
                .ForMember(dest => dest.methodName, opt => opt.MapFrom(src => src.METHOD_NAME));

            CreateMap<RolControllerMethodDTO, t_rol_controller_method>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.ROL_ID, opt => opt.MapFrom(src => src.rolEidDto.id))
                .ForMember(dest => dest.CONTROLLER_NAME, opt => opt.MapFrom(src => src.controllerName))
                .ForMember(dest => dest.METHOD_NAME, opt => opt.MapFrom(src => src.methodName));
        }
    }
}