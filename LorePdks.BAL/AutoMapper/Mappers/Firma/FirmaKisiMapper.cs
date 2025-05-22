

using LorePdks.BAL.AutoMapper;
using LorePdks.BAL.Managers.Firma.Interfaces;
using LorePdks.BAL.Managers.Kisi.Interfaces;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.DTO.Firma;
using LorePdks.COMMON.DTO.FirmaKisi;
using LorePdks.COMMON.Helpers;
using LorePdks.DAL.Model;
using Microsoft.Extensions.DependencyInjection;

public class FirmaKisiMapper : MappingProfile
{

    public FirmaKisiMapper()
    {
        var kisiManager = ServiceProviderHelper.ServiceProvider.GetService<Lazy<IKisiManager>>();
        var firmaManager = ServiceProviderHelper.ServiceProvider.GetService<Lazy<IFirmaManager>>();
        //var aa = firmaManager.Value.getFirmaDtoById(1);
        CreateMap<t_firma_kisi, FirmaKisiDTO>()
           .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
           .ForMember(x => x.firmaDto, y => y.MapFrom(z => new FirmaDTO() { id = z.FIRMA_ID }))
           .ForMember(x => x.kisiDto, y => y.MapFrom(z => kisiManager.Value.getKisiDtoById(z.KISI_ID, false)))
           .ForMember(x => x.firmaKisiTipKodDto, y => y.MapFrom(z => _kodManager.Value.GetKodDtoByKodId(Convert.ToInt32(z.FIRMA_KISI_TIP_KID))))
           .ReverseMap()
           .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
           .ForMember(x => x.KISI_ID, y => y.MapFrom(z => z.kisiDto.id))
           .ForMember(x => x.FIRMA_ID, y => y.MapFrom(z => z.firmaDto.id))
           .ForMember(x => x.FIRMA_KISI_TIP_KID, y => y.MapFrom(z => z.firmaKisiTipKodDto.id))
           ;
    }
}
