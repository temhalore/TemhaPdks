using AutoMapper;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Yetki.Rol;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.Yetki.Rol
{
    public class RolEkranMapper : MappingProfile
    {
        public RolEkranMapper()
        {
            CreateMap<t_rol_ekran, RolEkranDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.rolEidDto, opt => opt.MapFrom(src => new EIdDTO { id = src.ROL_ID })) //src.ROL_ID)) //opt.MapFrom(src => src.ROL_ID))
                .ForMember(dest => dest.ekranEidDto, opt => opt.MapFrom(src => new EIdDTO() { id = src.EKRAN_ID })); //src.EKRAN_ID)) //opt.MapFrom(src => src.EKRAN_ID));

            CreateMap<RolEkranDTO, t_rol_ekran>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.ROL_ID, opt => opt.MapFrom(src => src.rolEidDto.id))
                .ForMember(dest => dest.EKRAN_ID, opt => opt.MapFrom(src => src.ekranEidDto.id));
        }
    }
}