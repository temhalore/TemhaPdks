using AutoMapper;
using LorePdks.COMMON.DTO.Security.Page;
using LorePdks.COMMON.DTO.Security.Role;
using LorePdks.COMMON.DTO.Security.RoleWidget;
using LorePdks.COMMON.DTO.Security.Widget;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Models;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using LorePdks.BAL.Managers.Security.Interfaces;

namespace LorePdks.BAL.Managers.Security
{
    public class RoleWidgetManager : IRoleWidgetManager
    {
        private readonly IMapper _mapper;

        private readonly IWidgetManager _widgetManager;
        private readonly IPageManager _pageManager;
        private readonly IRoleManager _roleManager;

        public RoleWidgetManager(
            IMapper mapper,
            IWidgetManager moduleManager,
            IPageManager pageManager,
            IRoleManager roleManager

            )
        {
            _mapper = mapper;
            _widgetManager = moduleManager;
            _pageManager = pageManager;
            _roleManager = roleManager;
        }


        public RoleWidgetDTO GetRoleWidgetTreeListForAdmin(RoleDTO roleDto)
        {
            RoleWidgetDTO roleWidgetList = GetList(roleDto);
            var pageListDto = roleWidgetList.widgetListDto.Select(x => x.pageDto).Distinct().ToList();
            RoleWidgetDTO newRoleWidgetListDto = new RoleWidgetDTO();
            newRoleWidgetListDto.roleDto = roleDto;
            newRoleWidgetListDto.widgetListDto = new List<WidgetDTO>();
            foreach (var pageDto in pageListDto)
            {
                newRoleWidgetListDto.widgetListDto.Add(new WidgetDTO() { pageDto = pageDto });
            }

            List<TreeTableDTO<WidgetDTO>> widgetTreeListDto = _mapper.Map<List<TreeTableDTO<WidgetDTO>>>(newRoleWidgetListDto.widgetListDto);
            foreach (var widgetTreeDto in widgetTreeListDto)
            {
                widgetTreeDto.expanded = true;
                widgetTreeDto.children = _mapper.Map<List<TreeTableDTO<WidgetDTO>>>(roleWidgetList.widgetListDto.Where(x => x.pageDto.id == widgetTreeDto.data.pageDto.id));
            }
            newRoleWidgetListDto.widgetTreeListDto = widgetTreeListDto;
            return newRoleWidgetListDto;
        }




        public RoleWidgetDTO GetList(RoleDTO roleDto)
        {

            string query = @"select 
            w.ID as id,
            w.NAME as name,
            w.SELECTOR asselector ,
            w.ORDER_NO as  orderNo,
            w.PAGE_ID as pageId,
            case when rw.ROLE_ID is NULL then 0 else 1 end  as isRoleWidget
            from Pos.AUTH_WIDGET w
            left join Pos.AUTH_ROLE_WIDGET rw on rw.WIDGET_ID=w.ID and rw.ISDELETED=0 and rw.ROLE_ID=@p1
            left join Pos.AUTH_PAGE p on p.ID=w.PAGE_ID and p.ISDELETED=0
            where w.ISDELETED=0 
            order by p.ORDER_NO,w.ORDER_NO";
            GenericRepository<T_Pos_AUTH_ROLE_WIDGET> _repoRoleWidget = new GenericRepository<T_Pos_AUTH_ROLE_WIDGET>();
            List<WidgetDTO> widgetListDto = _repoRoleWidget.Query<WidgetDTO>(query, new { p1 = roleDto.id }).ToList();

            List<PageDTO> pageListDto = _pageManager.GetList();
            foreach (var widgetDto in widgetListDto)
            {
                widgetDto.pageDto = pageListDto.Where(x => x.id == widgetDto.pageId).FirstOrDefault();
            }
            RoleWidgetDTO roleWidgetDto = new RoleWidgetDTO();
            roleWidgetDto.roleDto = roleDto;
            roleWidgetDto.widgetListDto = widgetListDto.OrderBy(x => x.pageDto.name).ToList();//.OrderBy(x=>x.pageDto.orderNo).ThenBy(x=>x.orderNo).ToList();
            return roleWidgetDto;
        }
        public void Set(RoleWidgetDTO roleWidgetDto)
        {
            GenericRepository<T_Pos_AUTH_ROLE_WIDGET> _repoRoleWidget = new GenericRepository<T_Pos_AUTH_ROLE_WIDGET>();
            T_Pos_AUTH_ROLE_WIDGET roleWidget = _repoRoleWidget.Get("ROLE_ID=@p1 and WIDGET_ID=@p2", new { p1 = roleWidgetDto.roleDto.id, p2 = roleWidgetDto.widgetDto.id });
            if (roleWidgetDto.widgetDto.isRoleWidget && roleWidget == null)
            {
                T_Pos_AUTH_ROLE_WIDGET newRoleModule = new T_Pos_AUTH_ROLE_WIDGET()
                {
                    ROLE_ID = roleWidgetDto.roleDto.id,
                    WIDGET_ID = roleWidgetDto.widgetDto.id
                };
                _repoRoleWidget.Insert(newRoleModule);


            }
            else if (!roleWidgetDto.widgetDto.isRoleWidget && roleWidget != null)
            {
                _repoRoleWidget.Delete(roleWidget);
            }
            else
            {
                throw new AppException(500, "Anlamsız İşlem Yütürüldü");
            }
        }

        //public long SetRegisterAdminRoleAndRoleWidgets(OrganizationDTO organizationDto)
        //{
        //    RoleDTO roleAdminDto = _roleManager.Get(11);
        //    roleAdminDto.organizationDto = organizationDto;
        //    long newRoleId = _roleManager.Add(roleAdminDto);

        //    List<T_Pos_AUTH_ROLE_WIDGET> roleWidgetList = _repoRoleWidget.GetList("RoleId=@p1", new { p1 = 11 });
        //    foreach (var roleWidget in roleWidgetList)
        //    {
        //        T_Pos_AUTH_ROLE_WIDGET newRoleWidget = new T_Pos_AUTH_ROLE_WIDGET()
        //        {
        //            RoleId = newRoleId,
        //            WidgetId = roleWidget.WidgetId
        //        };
        //        _repoRoleWidget.Insert(newRoleWidget);

        //    }
        //    return newRoleId;
        //}
    }
}
