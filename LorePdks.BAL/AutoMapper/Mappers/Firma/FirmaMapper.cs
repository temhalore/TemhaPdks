

using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

public class FirmaMapper : MappingProfile
{
    // private ICommonManager _commonManager;
    // private IBirimManager _birimManager;
    // private IKisiManager _kisiManager;
    // public KodMapper(ICommonManager commonManager, IBirimManager birimManager, IKisiManager kisiManager)
    // {
    //     _commonManager = commonManager;
    //     _birimManager = birimManager;
    //     _kisiManager = kisiManager;
    //
    // }

    public FirmaMapper()
    {
        CreateMap<t_firma, FirmaDTO>()
           .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
                .ForMember(x => x.ad, y => y.MapFrom(z => z.AD))
                .ForMember(x => x.kod, y => y.MapFrom(z => z.KOD))
                .ForMember(x => x.adres, y => y.MapFrom(z => z.ADRES))
                .ForMember(x => x.aciklama, y => y.MapFrom(z => z.ACIKLAMA))
                .ForMember(x => x.mesaiSaat, y => y.MapFrom(z => z.MESAI_SAAT))
                .ForMember(x => x.molaSaat, y => y.MapFrom(z => z.MOLA_SAAT))
                .ForMember(x => x.cumartesiMesaiSaat, y => y.MapFrom(z => z.CUMARTESI_MESAI_SAAT))
                .ForMember(x => x.cumartesiMolaSaat, y => y.MapFrom(z => z.CUMARTESI_MOLA_SAAT))
        //   ;
        .ReverseMap()
        .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
        .ForMember(x => x.AD, y => y.MapFrom(z => z.ad))
        .ForMember(x => x.KOD , y => y.MapFrom(z => z.kod))
                .ForMember(x => x.ADRES, y => y.MapFrom(z => z.adres))
                .ForMember(x => x.ACIKLAMA , y => y.MapFrom(z => z.aciklama))
                .ForMember(x => x.MESAI_SAAT , y => y.MapFrom(z => z.mesaiSaat))
                .ForMember(x => x.MOLA_SAAT , y => y.MapFrom(z => z.molaSaat))
                .ForMember(x => x.CUMARTESI_MESAI_SAAT , y => y.MapFrom(z => z.cumartesiMesaiSaat))
                .ForMember(x => x.CUMARTESI_MOLA_SAAT , y => y.MapFrom(z => z.cumartesiMolaSaat))
        ;
    }
}
