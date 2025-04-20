using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Kisi;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.Kisi
{
    public class KisiMapper : MappingProfile
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

        public KisiMapper()
        {
            CreateMap<t_kisi, KisiDTO>()
               .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
                    .ForMember(x => x.ad, y => y.MapFrom(z => z.AD))
                    .ForMember(x => x.soyad, y => y.MapFrom(z => z.SOYAD))
                    .ForMember(x => x.tc, y => y.MapFrom(z => z.TC))
                    .ForMember(x => x.cepTel, y => y.MapFrom(z => z.CEP_TEL))
                    .ForMember(x => x.email, y => y.MapFrom(z => z.EMAIL))
                    .ForMember(x => x.loginName, y => y.MapFrom(z => z.LOGIN_NAME))
                    .ForMember(x => x.sifre, y => y.MapFrom(z => z.SIFRE))
            .ReverseMap()
            .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
            .ForMember(x => x.AD, y => y.MapFrom(z => z.ad))
            .ForMember(x => x.SOYAD, y => y.MapFrom(z => z.soyad))
            .ForMember(x => x.TC, y => y.MapFrom(z => z.tc))
            .ForMember(x => x.CEP_TEL, y => y.MapFrom(z => z.cepTel))
            .ForMember(x => x.EMAIL, y => y.MapFrom(z => z.email))
            .ForMember(x => x.LOGIN_NAME, y => y.MapFrom(z => z.loginName))
            .ForMember(x => x.SIFRE, y => y.MapFrom(z => z.sifre));
        }
    }
}