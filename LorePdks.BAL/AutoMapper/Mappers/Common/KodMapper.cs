

//using LorePdks.BAL.AutoMapper;

//public class KodMapper : MappingProfile
//{
//   // private ICommonManager _commonManager;
//   // private IBirimManager _birimManager;
//   // private IKisiManager _kisiManager;
//  // public KodMapper(ICommonManager commonManager, IBirimManager birimManager, IKisiManager kisiManager)
//  // {
//  //     _commonManager = commonManager;
//  //     _birimManager = birimManager;
//  //     _kisiManager = kisiManager;
//  //
//  // }

//    public KodMapper()
//    {
//        CreateMap<T_Pos_Kod, KodDTO>()
//           .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
//                .ForMember(x => x.tipId, y => y.MapFrom(z => z.TIP_ID))
//                .ForMember(x => x.kod, y => y.MapFrom(z => z.KOD))
//                .ForMember(x => x.sira, y => y.MapFrom(z => z.SIRA))
//                .ForMember(x => x.digerUygEnumAd, y => y.MapFrom(z => z.DIGER_UYG_ENUM_AD))
//                .ForMember(x => x.digerUygEnumDeger, y => y.MapFrom(z => z.DIGER_UYG_ID))
//             //   ;
//        .ReverseMap()
//        .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
//        .ForMember(x => x.TIP_ID, y => y.MapFrom(z => z.tipId))
//        .ForMember(x => x.KOD, y => y.MapFrom(z => z.kod))
//        .ForMember(x => x.SIRA, y => y.MapFrom(z => z.sira))
//        .ForMember(x => x.DIGER_UYG_ENUM_AD, y => y.MapFrom(z => z.digerUygEnumAd))
//        .ForMember(x => x.DIGER_UYG_ID, y => y.MapFrom(z => z.digerUygEnumDeger))
//        ;
//    }
//}
