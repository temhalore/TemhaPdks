

using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

public class FirmaCihazMapper : MappingProfile
{
    public FirmaCihazMapper()
    {
        CreateMap<t_firma_cihaz, FirmaCihazDTO>()
           .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
           .ForMember(x => x.ad, y => y.MapFrom(z => z.AD))
           .ForMember(x => x.aciklama, y => y.MapFrom(z => z.ACIKLAMA))
           .ForMember(x => x.cihazMakineGercekId, y => y.MapFrom(z => z.CIHAZ_MAKINE_GERCEK_ID))
           .ForMember(x => x.firmaCihazTipKodDto, y => y.MapFrom(z => _kodManager.Value.GetKodDtoByKodId(Convert.ToInt32(z.FIRMA_CIHAZ_TIP_KID)) ))
           .ForMember(x => x.firmaDto, y => y.MapFrom(z => new FirmaDTO() { id = z.FIRMA_ID }))
           .ReverseMap()
           .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
           .ForMember(x => x.AD, y => y.MapFrom(z => z.ad))
           .ForMember(x => x.ACIKLAMA, y => y.MapFrom(z => z.aciklama))
           .ForMember(x => x.CIHAZ_MAKINE_GERCEK_ID, y => y.MapFrom(z => z.cihazMakineGercekId))
           .ForMember(x => x.FIRMA_CIHAZ_TIP_KID, y => y.MapFrom(z => z.firmaCihazTipKodDto.id))
           .ForMember(x => x.FIRMA_ID, y => y.MapFrom(z => z.firmaDto.id));
    }
}
