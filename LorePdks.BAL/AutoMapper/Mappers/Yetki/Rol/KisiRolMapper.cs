using AutoMapper;
using LorePdks.COMMON.DTO.Yetki.Rol;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.Yetki.Rol
{
    public class KisiRolMapper : MappingProfile
    {
        public KisiRolMapper()
        {
            CreateMap<t_kisi_rol, KisiRolDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.kisiId, opt => opt.MapFrom(src => src.KISI_ID))
                .ForMember(dest => dest.rolId, opt => opt.MapFrom(src => src.ROL_ID))
                .ForMember(dest => dest.rolAdi, opt => opt.Ignore());

            CreateMap<KisiRolDTO, t_kisi_rol>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.KISI_ID, opt => opt.MapFrom(src => src.kisiId))
                .ForMember(dest => dest.ROL_ID, opt => opt.MapFrom(src => src.rolId))
                ;
        }
    }
}