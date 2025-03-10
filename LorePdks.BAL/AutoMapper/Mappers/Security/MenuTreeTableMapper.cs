//using AutoMapper;
//using LorePdks.COMMON.DTO.Base;
//using LorePdks.COMMON.DTO.Security.Menu;

//namespace LorePdks.BAL.AutoMapper.Mappers.Security
//{
//    public class MenuTreeTableMapper : Profile
//    {
//        public MenuTreeTableMapper()
//        {
//            CreateMap<MenuDTO, TreeTableDTO<MenuDTO>>()
//                       .ForPath(x => x.data.id, y => y.MapFrom(z => z.id))
//                       .ForPath(x => x.data.title, y => y.MapFrom(z => z.title))
//                       .ForPath(x => x.data.pageDto, y => y.MapFrom(z => z.pageDto))
//                       .ForPath(x => x.data.target, y => y.MapFrom(z => z.target))
//                       .ForPath(x => x.data.icon, y => y.MapFrom(z => z.icon))
//                       .ForPath(x => x.data.href, y => y.MapFrom(z => z.href))
//                       .ForPath(x => x.data.hasSubMenu, y => y.MapFrom(z => z.hasSubMenu))
//                       .ForPath(x => x.data.orderNo, y => y.MapFrom(z => z.orderNo))
//                       .ForPath(x => x.data.parentMenuDto, y => y.MapFrom(z => z.parentMenuDto.id != 0 ? z.parentMenuDto : null))
//                       .ForPath(x => x.data.tree, y => y.MapFrom(z => z.tree))
//                       .ForPath(x => x.data.isExpended, y => y.MapFrom(z => z.isExpended))
//                       .ForPath(x => x.data.isRoleMenu, y => y.MapFrom(z => z.isRoleMenu))
//                       //.ForPath(x => x.children, y => y.MapFrom(z => _mapper.Map<List<TreeTableDTO<MenuDTO>>>(z.subMenuListDto)))
//                       .ReverseMap()
//                       //.ForMember(x => x.Id, y => y.MapFrom(z => z.id))
//                       ;
//        }

//    }
//}
