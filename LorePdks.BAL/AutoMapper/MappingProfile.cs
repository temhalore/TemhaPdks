using AutoMapper;
using LorePdks.BAL.Managers.Common.Kod;
using LorePdks.BAL.Managers.Common.Kod.Interfaces;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Helpers;
using LorePdks.DAL.Model;
using Microsoft.Extensions.DependencyInjection;

namespace LorePdks.BAL.AutoMapper
{
    public class MappingProfile : Profile
    {
        protected readonly Lazy<IKodManager>_kodManager;
        public MappingProfile()
        {
            _kodManager = ServiceProviderHelper.ServiceProvider.GetService<Lazy<IKodManager>>();

            // Kişi model mapping
            CreateMap<t_kisi, KisiDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.ad, opt => opt.MapFrom(src => src.AD))
                .ForMember(dest => dest.soyad, opt => opt.MapFrom(src => src.SOYAD))
                .ForMember(dest => dest.tc, opt => opt.MapFrom(src => src.TC))
                .ForMember(dest => dest.cepTel, opt => opt.MapFrom(src => src.CEP_TEL))
                .ForMember(dest => dest.email, opt => opt.MapFrom(src => src.EMAIL))
                .ForMember(dest => dest.loginName, opt => opt.MapFrom(src => src.LOGIN_NAME))
                .ForMember(dest => dest.sifre, opt => opt.MapFrom(src => src.SIFRE));

            CreateMap<KisiDTO, t_kisi>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.AD, opt => opt.MapFrom(src => src.ad))
                .ForMember(dest => dest.SOYAD, opt => opt.MapFrom(src => src.soyad))
                .ForMember(dest => dest.TC, opt => opt.MapFrom(src => src.tc))
                .ForMember(dest => dest.CEP_TEL, opt => opt.MapFrom(src => src.cepTel))
                .ForMember(dest => dest.EMAIL, opt => opt.MapFrom(src => src.email))
                .ForMember(dest => dest.LOGIN_NAME, opt => opt.MapFrom(src => src.loginName))
                .ForMember(dest => dest.SIFRE, opt => opt.MapFrom(src => src.sifre));

            // Kişi token mapping
            CreateMap<t_kisi_token, KisiTokenDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.kisiId, opt => opt.MapFrom(src => src.KISI_ID))
                .ForMember(dest => dest.loginName, opt => opt.MapFrom(src => src.LOGIN_NAME))
                .ForMember(dest => dest.token, opt => opt.MapFrom(src => src.TOKEN))
                .ForMember(dest => dest.ipAdresi, opt => opt.MapFrom(src => src.IP_ADRESI))
                .ForMember(dest => dest.userAgent, opt => opt.MapFrom(src => src.USER_AGENT))
                .ForMember(dest => dest.expDate, opt => opt.MapFrom(src => src.EXP_DATE));

            CreateMap<KisiTokenDTO, t_kisi_token>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.KISI_ID, opt => opt.MapFrom(src => src.kisiId))
                .ForMember(dest => dest.LOGIN_NAME, opt => opt.MapFrom(src => src.loginName))
                .ForMember(dest => dest.TOKEN, opt => opt.MapFrom(src => src.token))
                .ForMember(dest => dest.IP_ADRESI, opt => opt.MapFrom(src => src.ipAdresi))
                .ForMember(dest => dest.USER_AGENT, opt => opt.MapFrom(src => src.userAgent))
                .ForMember(dest => dest.EXP_DATE, opt => opt.MapFrom(src => src.expDate));
        }
    }
}