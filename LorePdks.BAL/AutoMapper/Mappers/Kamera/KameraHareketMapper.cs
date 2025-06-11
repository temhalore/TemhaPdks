using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.COMMON.DTO.KameraHareket;
using LorePdks.COMMON.Helpers;
using LorePdks.DAL.Model;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LorePdks.BAL.AutoMapper.Mappers.Kamera
{
    public class KameraHareketMapper : MappingProfile
    {
        public KameraHareketMapper()
        {
            CreateMap<t_kamera_hareket, KameraHareketDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.firmaDto, opt => opt.MapFrom(src => new FirmaDTO { id = src.FIRMA_ID }))
                .ForMember(dest => dest.firmaCihazDto, opt => opt.MapFrom(src => new FirmaCihazDTO { id = src.FIRMA_CIHAZ_ID }))
                .ForMember(dest => dest.olayTarihi, opt => opt.MapFrom(src => src.OLAY_TARIHI))
                .ForMember(dest => dest.kameraOlayTipKodDto, opt => opt.MapFrom(src => _kodManager.Value.GetKodDtoByKodId(Convert.ToInt32(src.KAMERA_OLAY_TIP_KID))))
                .ForMember(dest => dest.dosyaYol, opt => opt.MapFrom(src => src.DOSYA_YOL))
                .ForMember(dest => dest.aciklama, opt => opt.MapFrom(src => src.ACIKLAMA))
                .ForMember(dest => dest.hamLogVerisi, opt => opt.MapFrom(src => src.HAM_LOG_VERISI));

            CreateMap<KameraHareketDTO, t_kamera_hareket>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.FIRMA_ID, opt => opt.MapFrom(src => src.firmaDto != null ? src.firmaDto.id : 0))
                .ForMember(dest => dest.FIRMA_CIHAZ_ID, opt => opt.MapFrom(src => src.firmaCihazDto != null ? src.firmaCihazDto.id : 0))
                .ForMember(dest => dest.OLAY_TARIHI, opt => opt.MapFrom(src => src.olayTarihi))
                .ForMember(dest => dest.KAMERA_OLAY_TIP_KID, opt => opt.MapFrom(src => src.kameraOlayTipKodDto != null ? src.kameraOlayTipKodDto.id : 0))
                .ForMember(dest => dest.DOSYA_YOL, opt => opt.MapFrom(src => src.dosyaYol))
                .ForMember(dest => dest.ACIKLAMA, opt => opt.MapFrom(src => src.aciklama))
                .ForMember(dest => dest.HAM_LOG_VERISI, opt => opt.MapFrom(src => src.hamLogVerisi));
        }
    }
}
