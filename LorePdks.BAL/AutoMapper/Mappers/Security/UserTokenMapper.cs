using AutoMapper;
using LorePdks.COMMON.DTO.Security.Auth;
using LorePdks.COMMON.DTO.Security.User;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.Security
{
    public class UserTokenMapper : Profile
    {
        public UserTokenMapper()
        {
            CreateMap<T_Pos_AUTH_USER_TOKEN, UserTokenDTO>()
                   .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
                   .ForMember(x => x.loginName, y => y.MapFrom(z => z.LOGIN_NAME))
                   .ForMember(x => x.userDto, y => y.MapFrom(z => new UserDTO() { id = Convert.ToInt32(z.USER_ID) }))
                   .ForMember(x => x.isLogin, y => y.MapFrom(z => true))
                   .ForMember(x => x.appToken, y => y.MapFrom(z => z.APP_TOKEN))
                   .ForMember(x => x.ip, y => y.MapFrom(z => z.IP))
                   //.ForMember(x => x.userTypes, y => y.MapFrom(z => z.userTypes))
                   .ForMember(x => x.expireDate, y => y.MapFrom(z => z.EXP_DATE))
                   .ForMember(x => x.isYerineLogin, y => y.MapFrom(z => z.IS_YERINE_LOGIN))
                   .ForMember(x => x.yerineLoginAdminLoginName, y => y.MapFrom(z => z.YERINE_LOGIN_ADMIN_LOGIN_NAME))


                   .ReverseMap()
                    .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
                   .ForMember(x => x.LOGIN_NAME, y => y.MapFrom(z => z.loginName))
                   .ForMember(x => x.USER_ID, y => y.MapFrom(z => z.userDto.id))
                   //.ForMember(x => x.isLogin, y => y.MapFrom(z => z.isLogin))
                   .ForMember(x => x.APP_TOKEN, y => y.MapFrom(z => z.appToken))
                   .ForMember(x => x.IP, y => y.MapFrom(z => z.ip))
                   //.ForMember(x => x.userTypes, y => y.MapFrom(z => z.userTypes))
                   .ForMember(x => x.EXP_DATE, y => y.MapFrom(z => z.expireDate))
                   .ForMember(x => x.IS_YERINE_LOGIN, y => y.MapFrom(z => z.isYerineLogin))
                   .ForMember(x => x.YERINE_LOGIN_ADMIN_LOGIN_NAME, y => y.MapFrom(z => z.yerineLoginAdminLoginName))

                   //.ForMember(x => x.Photo, y => y.MapFrom(z => Encoding.ASCII.GetBytes(z.photo)))
                   ;
        }

    }
}
