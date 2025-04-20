using AutoMapper;
using LorePdks.COMMON.DTO.Auth.Securty.Rol;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.Yetki.Rol
{
    public class RolEkranMapper : MappingProfile
    {
        public RolEkranMapper()
        {
            CreateMap<t_rol_ekran, RolEkranDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.rolId, opt => opt.MapFrom(src => src.ROL_ID))
                .ForMember(dest => dest.ekranId, opt => opt.MapFrom(src => src.EKRAN_ID));

            CreateMap<RolEkranDTO, t_rol_ekran>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.ROL_ID, opt => opt.MapFrom(src => src.rolId))
                .ForMember(dest => dest.EKRAN_ID, opt => opt.MapFrom(src => src.ekranId));
        }
    }
}