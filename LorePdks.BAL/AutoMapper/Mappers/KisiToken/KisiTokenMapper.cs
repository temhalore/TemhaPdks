using AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.KisiToken
{
    public class KisiTokenMapper : MappingProfile
    {
        public KisiTokenMapper()
        {
            // Entity'den DTO'ya dönüşüm
            CreateMap<t_kisi_token, KisiTokenDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.kisiId, opt => opt.MapFrom(src => src.KISI_ID))
                .ForMember(dest => dest.loginName, opt => opt.MapFrom(src => src.LOGIN_NAME))
                .ForMember(dest => dest.token, opt => opt.MapFrom(src => src.TOKEN))
                .ForMember(dest => dest.ipAdresi, opt => opt.MapFrom(src => src.IP_ADRESI))
                .ForMember(dest => dest.userAgent, opt => opt.MapFrom(src => src.USER_AGENT))
                .ForMember(dest => dest.expDate, opt => opt.MapFrom(src => src.EXP_DATE));

            // DTO'dan Entity'ye dönüşüm
            CreateMap<KisiTokenDTO, t_kisi_token>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.KISI_ID, opt => opt.MapFrom(src => src.kisiId))
                .ForMember(dest => dest.LOGIN_NAME, opt => opt.MapFrom(src => src.loginName))
                .ForMember(dest => dest.TOKEN, opt => opt.MapFrom(src => src.token))
                .ForMember(dest => dest.IP_ADRESI, opt => opt.MapFrom(src => src.ipAdresi))
                .ForMember(dest => dest.USER_AGENT, opt => opt.MapFrom(src => src.userAgent))
                .ForMember(dest => dest.EXP_DATE, opt => opt.MapFrom(src => src.expDate))
                .ForMember(dest => dest.ISDELETED, opt => opt.MapFrom(src => 0)); // Yeni oluşturulan veya güncellenen tokenlar aktif olmalı
        }
    }
}