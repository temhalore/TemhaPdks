//using AutoMapper;
//using LorePdks.COMMON.DTO.Security.Permission;
//using LorePdks.DAL.Model;

//namespace LorePdks.BAL.AutoMapper.Mappers.Security
//{
//    public class PermissionMapper : Profile
//    {
//        public PermissionMapper()
//        {
//            CreateMap<T_Pos_AUTH_PERMISSION, PermissionDTO>()
//                   .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
//                   .ForMember(x => x.type, y => y.MapFrom(z => z.TYPE))
//                   .ForMember(x => x.area, y => y.MapFrom(z => z.AREA))
//                   .ForMember(x => x.controller, y => y.MapFrom(z => z.CONTROLLER))
//                   .ForMember(x => x.action, y => y.MapFrom(z => z.ACTION))
//                   .ForMember(x => x.returnType, y => y.MapFrom(z => z.RETURN_TYPE))
//                   .ForMember(x => x.name, y => y.MapFrom(z => z.NAME))
//                   .ForMember(x => x.url, y => y.MapFrom(z => z.URL))
//                   .ReverseMap()
//                   .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
//                   .ForMember(x => x.TYPE, y => y.MapFrom(z => z.type))
//                   .ForMember(x => x.AREA, y => y.MapFrom(z => z.area))
//                   .ForMember(x => x.CONTROLLER, y => y.MapFrom(z => z.controller))
//                   .ForMember(x => x.ACTION, y => y.MapFrom(z => z.action))
//                   .ForMember(x => x.RETURN_TYPE, y => y.MapFrom(z => z.returnType))
//                   .ForMember(x => x.NAME, y => y.MapFrom(z => z.name))
//                   .ForMember(x => x.URL, y => y.MapFrom(z => z.url))
//                   ;
//        }
//    }
//}
