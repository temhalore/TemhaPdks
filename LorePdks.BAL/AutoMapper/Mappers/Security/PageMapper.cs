using AutoMapper;
using LorePdks.COMMON.DTO.Security.Page;
using LorePdks.DAL.Model;

namespace LorePdks.BAL.AutoMapper.Mappers.Security
{
    public class PageMapper : Profile
    {
        public PageMapper()
        {
            CreateMap<T_Pos_AUTH_PAGE, PageDTO>()
                   .ForMember(x => x.id, y => y.MapFrom(z => z.ID))
                   .ForMember(x => x.name, y => y.MapFrom(z => z.NAME))
                   .ForMember(x => x.routerLink, y => y.MapFrom(z => z.ROUTER_LINK))
                   .ForMember(x => x.orderNo, y => y.MapFrom(z => z.ORDER_NO))
                   .ReverseMap()
                   .ForMember(x => x.ID, y => y.MapFrom(z => z.id))
                   .ForMember(x => x.NAME, y => y.MapFrom(z => z.name))
                   .ForMember(x => x.ROUTER_LINK, y => y.MapFrom(z => z.routerLink))
                   .ForMember(x => x.ORDER_NO, y => y.MapFrom(z => z.orderNo))
                   ;
        }
    }
}
