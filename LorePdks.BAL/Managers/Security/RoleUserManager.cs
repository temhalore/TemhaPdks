using AutoMapper;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Role;
using LorePdks.COMMON.DTO.Security.RoleUser;
using LorePdks.COMMON.DTO.Security.User;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;


namespace LorePdks.BAL.Managers.Security
{
    public class RoleUserManager : IRoleUserManager
    {

        private readonly IMapper _mapper;


        private readonly IUserManager _userManager;
        public RoleUserManager(
                IMapper mapper,
               IUserManager userManager
            )
        {
            _mapper = mapper;
            _userManager = userManager;
        }
        //public List<RoleUserDTO> GetListByUserId(long userId)
        //{
        //    List<T_Pos_AUTH_ROLE_USER> roleUserList = _repoRoleUser.GetList("UserId=@p1", new { p1 = userId });
        //    List<RoleUserDTO> roleUserListDto = _mapper.Map<List<RoleUserDTO>>(roleUserList);
        //    return roleUserListDto;
        //}
        //public List<RoleUserDTO> GetListByRoleId(long roleId)
        //{
        //    List<T_Pos_AUTH_ROLE_USER> roleUserList = _repoRoleUser.GetList("RoleId=@p1", new { p1 = roleId });
        //    List<RoleUserDTO> roleUserListDto = _mapper.Map<List<RoleUserDTO>>(roleUserList);
        //    return roleUserListDto;
        //}
        public RoleUserDTO GetList(RoleDTO roleDto)
        {
            string query = @"select 
            id=u.Id,
			identificationNumber=u.IdentificationNumber,
            name = u.Name,
            lastName = u.LastName,
            isRoleUser = case when ru.RoleId is NULL then 0 else 1 end 
            from T_Pos_AUTH_USER u
            left join T_Pos_AUTH_ROLE_USER ru on ru.UserId=u.Id and ru.IsDeleted=0 and ru.RoleId=@p1
			left join T_Pos_AUTH_ROLE r on r.Id=ru.RoleId and  r.IsDeleted=0  
            where u.IsDeleted=0
			order by u.Id";
            GenericRepository<T_Pos_AUTH_ROLE_USER> _repoRoleUser = new GenericRepository<T_Pos_AUTH_ROLE_USER>();
            List<UserDTO> userListDto = _repoRoleUser.Query<UserDTO>(query, new { p1 = roleDto.id }).ToList();
            RoleUserDTO roleUserDto = new RoleUserDTO();
            roleUserDto.roleDto = roleDto;
            roleUserDto.userListDto = userListDto;//.OrderBy(x=>x.pageDto.orderNo).ThenBy(x=>x.orderNo).ToList();
            return roleUserDto;
        }
        public void Set(RoleUserDTO roleUserDto)
        {
            GenericRepository<T_Pos_AUTH_ROLE_USER> _repoRoleUser = new GenericRepository<T_Pos_AUTH_ROLE_USER>();
            T_Pos_AUTH_ROLE_USER roleUser = _repoRoleUser.Get("RoleId =@p1 and UserId=@p2", new { p1 = roleUserDto.roleDto.id, p2 = roleUserDto.userDto.id });
            //if (!roleUserDto.userDto.isRoleUser && roleUser == null)
            //{
            //    T_Pos_AUTH_ROLE_USER newRoleUser = new T_Pos_AUTH_ROLE_USER()
            //    {
            //        RoleId = roleUserDto.roleDto.id,
            //        UserId = roleUserDto.userDto.id
            //    };
            //    _repoRoleUser.Insert(newRoleUser);
            //    //TODO:Bunlar nedir ?
            //    //T_Pos_AUTH_MENU parentMenu = repoMenu.Get(request.menuDTO.parentId);
            //    //if (parentMenu != null) { 
            //    //MenuDTO parentMenuDTO = Mapper.Map<T_Pos_AUTH_MENU, MenuDTO>(parentMenu);
            //    //parentMenuDTO.isRoleMenu = true;
            //    //request.menuDTO = parentMenuDTO;
            //    //Set(request);
            //    //}


            //}
            //else if (roleUserDto.userDto.isRoleUser && roleUser != null)
            //{
            //    _repoRoleUser.Delete(roleUser);

            //}
            //else
            //{
            //    throw new AppException(500, "Anlamsız islem yürütüldü");
            //}

        }

        //Internals
        public void AddList(List<int> roleIdList, int userId)
        {
            GenericRepository<T_Pos_AUTH_ROLE_USER> _repoRoleUser = new GenericRepository<T_Pos_AUTH_ROLE_USER>();

            foreach (var roleId in roleIdList)
            {
                T_Pos_AUTH_ROLE_USER roleUser = _repoRoleUser.Get("RoleId=@p1 and UserId=@p2", new { p1 = roleId, p2 = userId });
                if (roleUser == null)
                {
                    T_Pos_AUTH_ROLE_USER newRoleUser = new T_Pos_AUTH_ROLE_USER()
                    {
                        ROLE_ID = roleId,
                        USER_ID = userId
                    };
                    _repoRoleUser.Insert(newRoleUser);
                }
            }
        }

        //private List<UserDTO> IsRoleUser(List<UserDTO> userListDto, List<T_Pos_AUTH_ROLE_USER> roleUserList)
        //{
        //    foreach (var userDto in userListDto)
        //    {
        //        foreach (var roleuser in roleUserList)
        //        {
        //            if (roleuser.UserId == userDto.id)
        //            {
        //                userDto.isRoleUser = true;
        //                break;
        //            }
        //        }
        //        //IsRoleMenu(userDTO.subMenuList, roleMenuList);

        //    }
        //    return userListDto;
        //}

    }
}