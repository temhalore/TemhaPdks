//using AutoMapper;
//using OYS.COMMON.DTO.Security.Auth;
//using OYS.COMMON.Helpers;
//using OYS.DAL.Model;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Reflection;
//using System.Text;
//using System.Threading.Tasks;

//namespace OYS.BAL.AutoMapper.Mappers.Ogrenci
//{
//    public class KisiTokenMapper : MappingProfile
//    {
//        public KisiTokenMapper()
//        {
//            CreateMap<T_oys_KisiToken, UserTokenDTO>()
//                .ForMember(x => x.loginName, y => y.MapFrom(z => z.LOGIN_NAME))
//                .ForMember(x => x.isLogin, y => y.MapFrom(z => z.IS_YERINE_LOGIN))
//                .ForMember(x => x.idmToken, y => y.MapFrom(z => z.IDM_TOKEN))
//                .ForMember(x => x.oysToken, y => y.MapFrom(z => z.OYS_TOKEN))
//                .ForMember(x => x.ip, y => y.MapFrom(z => z.IP_ADRES))
//                .ForMember(x => x.expireDate, y => y.MapFrom(z => z.EPOSTA))
//                .ForMember(x => x.isYerineLogin, y => y.MapFrom(z => z.IS_YERINE_LOGIN))
//                .ForMember(x => x.yerineLoginAdminLoginName, y => y.MapFrom(z => z.YERINE_LOGIN_ADMIN_LOGIN_NAME))
//                .ForMember(x => x.createdDate, y => y.MapFrom(z => z.CREATEDDATE))
//                .ForMember(x => x.updatedate, y => y.MapFrom(z => z.MODIFIEDDATE))
//                .ForMember(x => x.userAgent, y => y.MapFrom(z => z.USER_AGENT))
//                .ForMember(x => x.deleteReason, y => y.MapFrom(z => z.DELETE_REASON));

//        }
//    }
//}