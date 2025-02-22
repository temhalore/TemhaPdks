using AutoMapper;
using LorePdks.COMMON.DTO.Security.User;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.Security
{
    public class KisiMapper : Profile
    {

        public KisiMapper()
        {

            CreateMap<T_Pos_AUTH_USER, UserDTO>()
                       .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
                       .ForMember(x => x.loginName, y => y.MapFrom(z => z.LOGIN_NAME))
                       .ForMember(x => x.tc, y => y.MapFrom(z => z.KIMLIK_NO))
                       .ForMember(x => x.ad, y => y.MapFrom(z => z.AD))
                       .ForMember(x => x.soyad, y => y.MapFrom(z => z.SOYAD))
                       .ForMember(x => x.tcx, y => y.MapFrom(z => z.KIMLIK_NO))
                       .ReverseMap()
                       .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
                       .ForMember(x => x.LOGIN_NAME, y => y.MapFrom(z => z.loginName))
                       .ForMember(x => x.KIMLIK_NO, y => y.MapFrom(z => z.tc))
                       .ForMember(x => x.AD, y => y.MapFrom(z => z.ad))
                       .ForMember(x => x.SOYAD, y => y.MapFrom(z => z.soyad))
                       ;
        }
    }
}
