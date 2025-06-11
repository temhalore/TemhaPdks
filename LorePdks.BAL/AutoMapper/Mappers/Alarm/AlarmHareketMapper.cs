using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.COMMON.DTO.AlarmHareket;
using LorePdks.COMMON.Helpers;
using LorePdks.DAL.Model;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace LorePdks.BAL.AutoMapper.Mappers.Alarm
{
    public class AlarmHareketMapper : MappingProfile
    {
        public AlarmHareketMapper()
        {            CreateMap<t_alarm_hareket, AlarmHareketDTO>()
                .ForMember(dest => dest.id, opt => opt.MapFrom(src => src.ID))
                .ForMember(dest => dest.firmaDto, opt => opt.MapFrom(src => new FirmaDTO { id = src.FIRMA_ID }))
                .ForMember(dest => dest.firmaCihazDto, opt => opt.MapFrom(src => new FirmaCihazDTO { id = src.FIRMA_CIHAZ_ID }))
                .ForMember(dest => dest.alarmTarihi, opt => opt.MapFrom(src => src.ALARM_TARIHI))
                .ForMember(dest => dest.alarmTipKodDto, opt => opt.MapFrom(src => _kodManager.Value.GetKodDtoByKodId(Convert.ToInt32(src.ALARM_TIP_KID))))
                .ForMember(dest => dest.aciklama, opt => opt.MapFrom(src => src.ACIKLAMA))
                .ForMember(dest => dest.alarmSeviyeKodDto, opt => opt.MapFrom(src => _kodManager.Value.GetKodDtoByKodId(Convert.ToInt32(src.ALARM_SEVIYE_KID))))
                .ForMember(dest => dest.sensorBilgisi, opt => opt.MapFrom(src => src.SENSOR_BILGISI))
                .ForMember(dest => dest.hamLogVerisi, opt => opt.MapFrom(src => src.HAM_LOG_VERISI));

            CreateMap<AlarmHareketDTO, t_alarm_hareket>()
                .ForMember(dest => dest.ID, opt => opt.MapFrom(src => src.id))
                .ForMember(dest => dest.FIRMA_ID, opt => opt.MapFrom(src => src.firmaDto != null ? src.firmaDto.id : 0))
                .ForMember(dest => dest.FIRMA_CIHAZ_ID, opt => opt.MapFrom(src => src.firmaCihazDto != null ? src.firmaCihazDto.id : 0))
                .ForMember(dest => dest.ALARM_TARIHI, opt => opt.MapFrom(src => src.alarmTarihi))
                .ForMember(dest => dest.ALARM_TIP_KID, opt => opt.MapFrom(src => src.alarmTipKodDto != null ? src.alarmTipKodDto.id : 0))
                .ForMember(dest => dest.ACIKLAMA, opt => opt.MapFrom(src => src.aciklama))
                .ForMember(dest => dest.ALARM_SEVIYE_KID, opt => opt.MapFrom(src => src.alarmSeviyeKodDto != null ? src.alarmSeviyeKodDto.id : 0))
                .ForMember(dest => dest.SENSOR_BILGISI, opt => opt.MapFrom(src => src.sensorBilgisi))
                .ForMember(dest => dest.HAM_LOG_VERISI, opt => opt.MapFrom(src => src.hamLogVerisi));
        }
    }
}
