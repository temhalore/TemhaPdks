//using AutoMapper;
//using LorePdks.COMMON.DTO.Security.Menu;
//using LorePdks.COMMON.DTO.Security.Page;
//using LorePdks.DAL.Model;

//namespace LorePdks.BAL.AutoMapper.Mappers.Security
//{
//    public class MenuMapper : Profile
//    {
//        public MenuMapper()
//        {
//            CreateMap<T_Pos_AUTH_MENU, MenuDTO>()
//                   .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
//                   .ForMember(x => x.title, y => y.MapFrom(z => z.TITLE))
//                   .ForMember(x => x.pageDto, y => y.MapFrom(z => new PageDTO { id = Convert.ToInt32(z.PAGE_ID) }))
//                   .ForMember(x => x.target, y => y.MapFrom(z => z.TARGET))
//                   .ForMember(x => x.icon, y => y.MapFrom(z => z.ICON))
//                   .ForMember(x => x.href, y => y.MapFrom(z => z.HREF))
//                   .ForMember(x => x.hasSubMenu, y => y.MapFrom(z => z.HAS_SUB_MENU))
//                   .ForMember(x => x.parentMenuDto, y => y.MapFrom(z => new MenuDTO { id = z.PARENT_ID }))
//                   .ForMember(x => x.tree, y => y.MapFrom(z => z.TREE))
//                   .ForMember(x => x.orderNo, y => y.MapFrom(z => z.ORDER_NO))
//                   .ForMember(x => x.subMenuListDto, y => y.MapFrom(z => new List<MenuDTO>()))
//                   .ForMember(x => x.isExpended, y => y.MapFrom(z => true))
//                   .ForMember(x => x.isRoleMenu, y => y.MapFrom(z => false))

//                   .ReverseMap()
//                  .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
//                   .ForMember(x => x.TITLE, y => y.MapFrom(z => z.title))
//                   .ForMember(x => x.PAGE_ID, y => y.MapFrom(z => z.pageDto.id))
//                   .ForMember(x => x.TARGET, y => y.MapFrom(z => z.target))
//                   .ForMember(x => x.ICON, y => y.MapFrom(z => z.icon))
//                   .ForMember(x => x.HREF, y => y.MapFrom(z => z.href))
//                   .ForMember(x => x.HAS_SUB_MENU, y => y.MapFrom(z => z.hasSubMenu))
//                   .ForMember(x => x.PARENT_ID, y => y.MapFrom(z => new MenuDTO { id = z.parentMenuDto.id }))
//                   .ForMember(x => x.TREE, y => y.MapFrom(z => z.tree))
//                   .ForMember(x => x.ORDER_NO, y => y.MapFrom(z => z.orderNo))
//                   //.ForMember(x => x.subMenuListDto, y => y.MapFrom(z => new List<MenuDTO>()))
//                   //.ForMember(x => x.isExpended, y => y.MapFrom(z => true))
//                   //.ForMember(x => x.isRoleMenu, y => y.MapFrom(z => false))
//                   ;
//        }
//    }
//}
