

using LorePdks.BAL.AutoMapper;
using LorePdks.BAL.Managers.Deneme.Interfaces;
using LorePdks.COMMON.DTO.Common;
using LorePdks.COMMON.Helpers;
using LorePdks.DAL.Model;
using Microsoft.Extensions.DependencyInjection;

public class FirmaKisiMapper : MappingProfile
{

    public FirmaKisiMapper()
    {
        var kisiManager = ServiceProviderHelper.ServiceProvider.GetService<Lazy<IKisiManager>>();
        var firmaManager = ServiceProviderHelper.ServiceProvider.GetService<Lazy<IFirmaManager>>();
        var aa = firmaManager.Value.getFirmaDtoById(1);
        CreateMap<t_firma_kisi, FirmaKisiDTO>()
           .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
          // .ForMember(x => x.firmaDto, y => y.MapFrom(z => firmaManager.Value.getFirmaDtoById(1)))
           
           .ForMember(x => x.firmaDto, y => y.MapFrom(z => new FirmaDTO() { id = z.FIRMA_ID }))
           .ReverseMap()
           //.ForMember(x => x.ID, y => y.MapFrom(z => z.id))
           //.ForMember(x => x.AD, y => y.MapFrom(z => z.ad))
           //.ForMember(x => x.ACIKLAMA, y => y.MapFrom(z => z.aciklama))
           //.ForMember(x => x.CIHAZ_MAKINE_GERCEK_ID, y => y.MapFrom(z => z.cihazMakineGercekId))
           //.ForMember(x => x.FIRMA_CIHAZ_TIP_KID, y => y.MapFrom(z => z.firmaCihazTipKodDto.id))
           //.ForMember(x => x.FIRMA_ID, y => y.MapFrom(z => z.firmaDto.id));
           ;
    }
}
