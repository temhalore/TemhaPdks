using AutoMapper;
using LorePdks.COMMON.DTO.Yetki.Ekran;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.Yetki.Ekran
{
    public class EkranMapper : MappingProfile
    {
        public EkranMapper()
        {
            CreateMap<t_ekran, EkranDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.ekranAdi, opt => opt.MapFrom(src => src.EKRAN_ADI))
                .ForMember(dest => dest.ekranYolu, opt => opt.MapFrom(src => src.EKRAN_YOLU))
                .ForMember(dest => dest.ekranKodu, opt => opt.MapFrom(src => src.EKRAN_KODU))
                .ForMember(dest => dest.aciklama, opt => opt.MapFrom(src => src.ACIKLAMA))
                .ForMember(dest => dest.ustEkranId, opt => opt.MapFrom(src => src.UST_EKRAN_ID))
                .ForMember(dest => dest.siraNo, opt => opt.MapFrom(src => src.SIRA_NO))
                .ForMember(dest => dest.ikon, opt => opt.MapFrom(src => src.IKON))
                .ForMember(dest => dest.aktif, opt => opt.MapFrom(src => src.AKTIF))
                .ForMember(dest => dest.altEkranlar, opt => opt.Ignore());

            CreateMap<EkranDTO, t_ekran>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.EKRAN_ADI, opt => opt.MapFrom(src => src.ekranAdi))
                .ForMember(dest => dest.EKRAN_YOLU, opt => opt.MapFrom(src => src.ekranYolu))
                .ForMember(dest => dest.EKRAN_KODU, opt => opt.MapFrom(src => src.ekranKodu))
                .ForMember(dest => dest.ACIKLAMA, opt => opt.MapFrom(src => src.aciklama))
                .ForMember(dest => dest.UST_EKRAN_ID, opt => opt.MapFrom(src => src.ustEkranId))
                .ForMember(dest => dest.SIRA_NO, opt => opt.MapFrom(src => src.siraNo))
                .ForMember(dest => dest.IKON, opt => opt.MapFrom(src => src.ikon))
                .ForMember(dest => dest.AKTIF, opt => opt.MapFrom(src => src.aktif));
        }
    }
}