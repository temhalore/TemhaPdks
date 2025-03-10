//using AutoMapper;
//using LorePdks.COMMON.DTO.Security.Page;
//using LorePdks.COMMON.DTO.Security.Widget;
//using LorePdks.DAL.Model;

//namespace LorePdks.BAL.AutoMapper.Mappers.Security
//{
//    public class WidgetMapper : Profile
//    {
//        public WidgetMapper()
//        {
//            CreateMap<T_Pos_AUTH_WIDGET, WidgetDTO>()
//                   .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
//                   .ForMember(x => x.name, y => y.MapFrom(z => z.NAME))
//                   .ForMember(x => x.selector, y => y.MapFrom(z => z.SELECTOR))
//                   .ForMember(x => x.pageDto, y => y.MapFrom(z => new PageDTO() { id = Convert.ToInt32(z.PAGE_ID) }))
//                   .ReverseMap()
//                   .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
//                   .ForMember(x => x.NAME, y => y.MapFrom(z => z.name))
//                   .ForMember(x => x.SELECTOR, y => y.MapFrom(z => z.selector))
//                   .ForMember(x => x.PAGE_ID, y => y.MapFrom(z => z.pageDto.id))
//                   ;
//        }
//    }
//}
