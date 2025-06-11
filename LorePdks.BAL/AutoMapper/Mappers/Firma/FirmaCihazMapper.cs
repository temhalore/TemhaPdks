

using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.FirmaCihaz;
using LorePdks.DAL.Model;

public class FirmaCihazMapper : MappingProfile
{
    public FirmaCihazMapper()
    {        CreateMap<t_firma_cihaz, FirmaCihazDTO>()
           .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
           .ForMember(x => x.ad, y => y.MapFrom(z => z.AD))
           .ForMember(x => x.aciklama, y => y.MapFrom(z => z.ACIKLAMA))
           .ForMember(x => x.cihazMakineGercekId, y => y.MapFrom(z => z.CIHAZ_MAKINE_GERCEK_ID))
           .ForMember(x => x.firmaCihazTipKodDto, y => y.MapFrom(z => _kodManager.Value.GetKodDtoByKodId(Convert.ToInt32(z.FIRMA_CIHAZ_TIP_KID)) ))
           .ForMember(x => x.firmaDto, y => y.MapFrom(z => new FirmaDTO() { id = z.FIRMA_ID }))
           .ForMember(x => x.logParserConfig, y => y.MapFrom(z => z.LOG_PARSER_CONFIG))
           .ForMember(x => x.logDelimiter, y => y.MapFrom(z => z.LOG_DELIMITER))
           .ForMember(x => x.logDateFormat, y => y.MapFrom(z => z.LOG_DATE_FORMAT))
           .ForMember(x => x.logTimeFormat, y => y.MapFrom(z => z.LOG_TIME_FORMAT))
           .ForMember(x => x.logFieldMapping, y => y.MapFrom(z => z.LOG_FIELD_MAPPING))
           .ForMember(x => x.logSample, y => y.MapFrom(z => z.LOG_SAMPLE))
           .ReverseMap()
           .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
           .ForMember(x => x.AD, y => y.MapFrom(z => z.ad))
           .ForMember(x => x.ACIKLAMA, y => y.MapFrom(z => z.aciklama))
           .ForMember(x => x.CIHAZ_MAKINE_GERCEK_ID, y => y.MapFrom(z => z.cihazMakineGercekId))
           .ForMember(x => x.FIRMA_CIHAZ_TIP_KID, y => y.MapFrom(z => z.firmaCihazTipKodDto.id))
           .ForMember(x => x.FIRMA_ID, y => y.MapFrom(z => z.firmaDto.id))
           .ForMember(x => x.LOG_PARSER_CONFIG, y => y.MapFrom(z => z.logParserConfig))
           .ForMember(x => x.LOG_DELIMITER, y => y.MapFrom(z => z.logDelimiter))
           .ForMember(x => x.LOG_DATE_FORMAT, y => y.MapFrom(z => z.logDateFormat))
           .ForMember(x => x.LOG_TIME_FORMAT, y => y.MapFrom(z => z.logTimeFormat))
           .ForMember(x => x.LOG_FIELD_MAPPING, y => y.MapFrom(z => z.logFieldMapping))
           .ForMember(x => x.LOG_SAMPLE, y => y.MapFrom(z => z.logSample));
    }
}
