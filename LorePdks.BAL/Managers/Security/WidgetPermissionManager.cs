using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Permission;
using LorePdks.COMMON.DTO.Security.Widget;
using LorePdks.COMMON.DTO.Security.WidgetPermission;
using LorePdks.COMMON.Models;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;

namespace LorePdks.BAL.Managers.Security
{
    public class WidgetPermissionManager : IWidgetPermissionManager
    {
        private readonly IPermissionManager _permissionManager;
        public WidgetPermissionManager(IPermissionManager permissionManager)
        {
            _permissionManager = permissionManager;
        }
        public WidgetPermissionDTO GetList(WidgetDTO widgetDto)
        {
            string query = @"select 
            id=p.Id,
            type=p.Type,
            area=p.Area,
            controller=p.Controller,
            action=p.Action,
            returnType=p.ReturnType,
            name=p.Name,
            url=p.Url,
            isWidgetPermission=case  when wp.WidgetId is NULL then 0 else 1 end from T_Pos_AUTH_PERMISSION p
            left join T_Pos_AUTH_WIDGET_PERMISSION wp on wp.PermissionId=p.Id and wp.IsDeleted=0 and wp.WidgetId=@p1
            where p.IsDeleted=0 ";

            GenericRepository<T_Pos_AUTH_WIDGET_PERMISSION> _repoWidgetPermission = new GenericRepository<T_Pos_AUTH_WIDGET_PERMISSION>();
            List<PermissionDTO> permissionListDto = _repoWidgetPermission.Query<PermissionDTO>(query, new { p1 = widgetDto.id }).ToList();

            WidgetPermissionDTO widgetPermissionDto = new WidgetPermissionDTO();
            widgetPermissionDto.widgetDto = widgetDto;
            widgetPermissionDto.permissionListDto = permissionListDto;
            return widgetPermissionDto;
        }
        public void Set(WidgetPermissionDTO widgetPermissionDto)
        {
            GenericRepository<T_Pos_AUTH_WIDGET_PERMISSION> _repoWidgetPermission = new GenericRepository<T_Pos_AUTH_WIDGET_PERMISSION>();
            T_Pos_AUTH_WIDGET_PERMISSION widgetPermission = _repoWidgetPermission.Get("WidgetId=@p1 and PermissionId=@p2", new { p1 = widgetPermissionDto.widgetDto.id, p2 = widgetPermissionDto.permissionDto.id });
            if (!widgetPermissionDto.permissionDto.isWidgetPermission && widgetPermission == null)
            {
                T_Pos_AUTH_WIDGET_PERMISSION newWidgetPermission = new T_Pos_AUTH_WIDGET_PERMISSION()
                {
                    WIDGET_ID = widgetPermissionDto.widgetDto.id,
                    PERMISSION_ID = widgetPermissionDto.permissionDto.id
                };
                _repoWidgetPermission.Insert(newWidgetPermission);

            }
            else if (widgetPermissionDto.permissionDto.isWidgetPermission && widgetPermission != null)
            {
                _repoWidgetPermission.Delete(widgetPermission);
            }
            else
            {
                throw new AppException(500, "Anlamsız işlem yürütüldü");
            }

        }
    }
}