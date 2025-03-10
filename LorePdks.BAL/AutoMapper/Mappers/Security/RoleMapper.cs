//using AutoMapper;
//using LorePdks.COMMON.DTO.Security.Role;
//using LorePdks.DAL.Model;

//namespace LorePdks.BAL.AutoMapper.Mappers.Security
//{
//    public class RoleMapper : Profile
//    {
//        public RoleMapper()
//        {
//            CreateMap<T_Pos_AUTH_ROLE, RoleDTO>()
//                   .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
//                   .ForMember(x => x.name, y => y.MapFrom(z => z.NAME))
//                   .ReverseMap()
//                   .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
//                   .ForMember(x => x.NAME, y => y.MapFrom(z => z.name))
//                   ;
//        }
//    }
//}
