using AutoMapper;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Security.Widget;

namespace LorePdks.BAL.AutoMapper.Mappers.Security
{
    public class WidgetTreeTableMapper : Profile
    {
        public WidgetTreeTableMapper()
        {
            CreateMap<WidgetDTO, TreeTableDTO<WidgetDTO>>()
                       .ForPath(x => x.data.id, y => y.MapFrom(z => z.id))
                       .ForPath(x => x.data.pageId, y => y.MapFrom(z => z.pageId))
                       .ForPath(x => x.data.pageDto, y => y.MapFrom(z => z.pageDto))
                       .ForPath(x => x.data.isRoleWidget, y => y.MapFrom(z => z.isRoleWidget))
                       .ForPath(x => x.data.name, y => y.MapFrom(z => z.name))
                       .ForPath(x => x.data.selector, y => y.MapFrom(z => z.selector))
                       .ForPath(x => x.data.orderNo, y => y.MapFrom(z => z.orderNo))
                       //.ForPath(x => x.children, y => y.MapFrom(z => _mapper.Map<List<TreeTableDTO<MenuDTO>>>(z.subMenuListDto)))
                       .ReverseMap()
                       //.ForMember(x => x.Id, y => y.MapFrom(z => z.id))
                       ;
        }

    }
}
