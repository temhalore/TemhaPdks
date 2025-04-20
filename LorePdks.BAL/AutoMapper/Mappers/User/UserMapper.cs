using AutoMapper;
using LorePdks.BAL.AutoMapper;
using LorePdks.COMMON.DTO.Common;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.User
{
    public class UserMapper : MappingProfile
    {
        public UserMapper()
        {
            CreateMap<t_user, UserDTO>()
                .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
                .ForMember(x => x.loginName, y => y.MapFrom(z => z.USER_NAME))
                .ForMember(x => x.sifre, y => y.MapFrom(z => z.SIFRE))
                .ForMember(x => x.isAktif, y => y.MapFrom(z => z.AKTIF == 1))
                .ForMember(x => x.kisiDto, y => y.MapFrom(z => new KisiDTO() { id = z.KISI_ID }))
                .ReverseMap()
                .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
                .ForMember(x => x.USER_NAME, y => y.MapFrom(z => z.loginName))
                .ForMember(x => x.SIFRE, y => y.MapFrom(z => z.sifre))
                .ForMember(x => x.AKTIF, y => y.MapFrom(z => z.isAktif ? 1 : 0))
                .ForMember(x => x.KISI_ID, y => y.MapFrom(z => z.kisiDto != null ? z.kisiDto.id : 0));
        }
    }
}