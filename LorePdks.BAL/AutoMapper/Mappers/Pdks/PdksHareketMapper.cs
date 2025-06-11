

using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.COMMON.DTO.Kisi;
using LorePdks.COMMON.DTO.PdksHareket;
using LorePdks.COMMON.Helpers;
using LorePdks.DAL.Model;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LorePdks.BAL.AutoMapper.Mappers.Pdks
{
    public class PdksHareketMapper : MappingProfile
    {
        public PdksHareketMapper()
        {
            CreateMap<t_pdks_hareket, PdksHareketDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.firmaDto, opt => opt.MapFrom(src => new FirmaDTO { id = src.FIRMA_ID }))
                .ForMember(dest => dest.kisiDto, opt => opt.MapFrom(src => src.KISI_ID.HasValue ? new KisiDTO { id = src.KISI_ID.Value } : null))
                .ForMember(dest => dest.firmaCihazDto, opt => opt.MapFrom(src => new FirmaCihazDTO { id = src.FIRMA_CIHAZ_ID }))
                .ForMember(dest => dest.kullaniciCihazKodu, opt => opt.MapFrom(src => src.KULLANICI_CIHAZ_KODU))
                .ForMember(dest => dest.hareketTarihi, opt => opt.MapFrom(src => src.HAREKET_TARIHI))
                .ForMember(dest => dest.pdksYonKodDto, opt => opt.MapFrom(src => _kodManager.Value.GetKodDtoByKodId(Convert.ToInt32(src.PDKS_YON_KID))))
                .ForMember(dest => dest.hamLogVerisi, opt => opt.MapFrom(src => src.HAM_LOG_VERISI));

            CreateMap<PdksHareketDTO, t_pdks_hareket>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.FIRMA_ID, opt => opt.MapFrom(src => src.firmaDto != null ? src.firmaDto.id : 0))
                .ForMember(dest => dest.KISI_ID, opt => opt.MapFrom(src => src.kisiDto != null ? src.kisiDto.id : (int?)null))
                .ForMember(dest => dest.FIRMA_CIHAZ_ID, opt => opt.MapFrom(src => src.firmaCihazDto != null ? src.firmaCihazDto.id : 0))
                .ForMember(dest => dest.KULLANICI_CIHAZ_KODU, opt => opt.MapFrom(src => src.kullaniciCihazKodu))
                .ForMember(dest => dest.HAREKET_TARIHI, opt => opt.MapFrom(src => src.hareketTarihi))
                .ForMember(dest => dest.PDKS_YON_KID, opt => opt.MapFrom(src => src.pdksYonKodDto != null ? src.pdksYonKodDto.id : 0))
                .ForMember(dest => dest.HAM_LOG_VERISI, opt => opt.MapFrom(src => src.hamLogVerisi));
        }
    }
}
