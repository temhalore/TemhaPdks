using AutoMapper;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Role;
using LorePdks.COMMON.Models;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;


namespace LorePdks.BAL.Managers.Security
{
    public class RoleManager : IRoleManager
    {
        private readonly IMapper _mapper;


        public RoleManager(
            IMapper mapper

            )
        {
            _mapper = mapper;

        }
        public List<RoleDTO> GetList()
        {
            GenericRepository<T_Pos_AUTH_ROLE> _repoRole = new GenericRepository<T_Pos_AUTH_ROLE>();
            List<T_Pos_AUTH_ROLE> roleList = _repoRole.GetList();
            List<RoleDTO> roleListDto = _mapper.Map<List<RoleDTO>>(roleList);
            return roleListDto;
        }

        public RoleDTO Get(long roleId)
        {
            GenericRepository<T_Pos_AUTH_ROLE> _repoRole = new GenericRepository<T_Pos_AUTH_ROLE>();
            T_Pos_AUTH_ROLE role = _repoRole.Get(roleId);
            RoleDTO roleDto = _mapper.Map<RoleDTO>(role);
            return roleDto;

        }
        public List<RoleDTO> GetListByWidgetId(long widgetId)
        {
            string query = @"select r.* from T_Pos_AUTH_ROLE (nolock) r 
            inner join T_Pos_AUTH_ROLE_WIDGET (nolock) rw on r.Id=rw.RoleId and rw.IsDeleted=0
            where rw.WidgetId=@p1 and r.IsDeleted=0";
            GenericRepository<T_Pos_AUTH_ROLE> _repoRole = new GenericRepository<T_Pos_AUTH_ROLE>();
            List<T_Pos_AUTH_ROLE> roleList = _repoRole.Query<T_Pos_AUTH_ROLE>(query, new { p1 = widgetId }).ToList();
            List<RoleDTO> roleListDto = _mapper.Map<List<RoleDTO>>(roleList);
            return roleListDto;
        }

        public long Add(RoleDTO roleDto)
        {
            GenericRepository<T_Pos_AUTH_ROLE> _repoRole = new GenericRepository<T_Pos_AUTH_ROLE>();
            T_Pos_AUTH_ROLE hasRole = _repoRole.Get(@"LOWER(Name)=LOWER(:p1)", new { p1 = roleDto.name });
            if (hasRole != null)
            {
                throw new AppException(500, $"{hasRole.NAME} adlı yetki grubu sistemde var olduğundan eklenmedi");
            }
            T_Pos_AUTH_ROLE role = new T_Pos_AUTH_ROLE()
            {
                NAME = roleDto.name,

            };

            long roleId = _repoRole.Insert(role);
            return roleId;

        }
        public void Set(RoleDTO roleDto)
        {
            GenericRepository<T_Pos_AUTH_ROLE> _repoRole = new GenericRepository<T_Pos_AUTH_ROLE>();
            T_Pos_AUTH_ROLE hasRole = _repoRole.Get(roleDto.id);
            string oldRoleName = hasRole.NAME;

            if (hasRole == null)
            {
                throw new AppException(500, $"Mantıksız işlem yürütüldü. {oldRoleName} adlı yetki grubu güncellenemedi");
            }

            hasRole.NAME = roleDto.name;
            _repoRole.Update(hasRole);
        }
        public void Del(RoleDTO roleDto)
        {
            GenericRepository<T_Pos_AUTH_ROLE> _repoRole = new GenericRepository<T_Pos_AUTH_ROLE>();
            T_Pos_AUTH_ROLE hasRole = _repoRole.Get(roleDto.id);
            if (hasRole == null)
            {
                throw new AppException(500, "Yetki grubu bulunamadığından silinemedi");
            }
            GenericRepository<T_Pos_AUTH_ROLE_WIDGET> _repoRoleWidget = new GenericRepository<T_Pos_AUTH_ROLE_WIDGET>();
            bool hasRoleWidget = _repoRoleWidget.GetList(@"RoleId=@p1", new { p1 = roleDto.id }).Count > 0;
            if (!hasRoleWidget)
            {
                throw new AppException(500, "Yetki grubuna bağlı komponentler buluduğundan silinemedi");
            }
            _repoRole.Delete(hasRole);
        }
    }
}
