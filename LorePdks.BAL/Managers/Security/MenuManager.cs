using AutoMapper;
using LorePdks.COMMON.DTO.Security.Menu;
using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.Extensions;
using LorePdks.COMMON.Models;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;
using LorePdks.BAL.Managers.Security.Interfaces;


namespace LorePdks.BAL.Managers.Security
{
    public class MenuManager : IMenuManager
    {
        private readonly IMapper _mapper;

        private readonly IPageManager _pageManager;

        public MenuManager(
            IMapper mapper,
            IPageManager pageManager
            )
        {
            _mapper = mapper;
            _pageManager = pageManager;
        }

        #region  Menu List For Admin Menu Tree Page
        public List<TreeTableDTO<MenuDTO>> GetMenuTreeListForAdmin(long? PARENT_ID = 0)
        {
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            List<T_Pos_AUTH_MENU> menuListAll = _repoMenu.GetList().OrderBy(x => x.ORDER_NO).ToList();

            List<TreeTableDTO<MenuDTO>> menuTreeListDto = GetSubMenuTreeList(menuListAll, PARENT_ID);
            return menuTreeListDto;
        }
        private List<TreeTableDTO<MenuDTO>> GetSubMenuTreeList(List<T_Pos_AUTH_MENU> menuListAll, long? PARENT_ID = 0)
        {
            List<T_Pos_AUTH_MENU> menuList = menuListAll.Where(x => x.PARENT_ID == PARENT_ID).ToList();
            List<MenuDTO> menuListDto = _mapper.Map<List<MenuDTO>>(menuList);
            foreach (var menuDto in menuListDto)
            {
                menuDto.pageDto = _pageManager.Get(menuDto.pageDto.id);
            }
            List<TreeTableDTO<MenuDTO>> menuTreeListDto = _mapper.Map<List<TreeTableDTO<MenuDTO>>>(menuListDto);

            foreach (var menuTreeDTO in menuTreeListDto)
            {
                menuTreeDTO.expanded = true;
                menuTreeDTO.children = GetSubMenuTreeList(menuListAll, menuTreeDTO.data.id);
            }
            return menuTreeListDto;
        }
        #endregion
        public List<MenuDTO> GetList()
        {
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            List<T_Pos_AUTH_MENU> menuList = _repoMenu.GetList().OrderBy(x => x.PARENT_ID).ThenBy(x => x.ORDER_NO).ToList();
            List<MenuDTO> menuListDTO = _mapper.Map<List<MenuDTO>>(menuList);
            return menuListDTO;
        }
        public long Add(MenuDTO menuDto)
        {
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            var hasMenuPage = _repoMenu.Get("PAGE_ID=@p1 AND PAGE_ID<>0", new { p1 = menuDto.pageDto.id });
            if (hasMenuPage != null && hasMenuPage.PAGE_ID != 0)
            {
                throw new AppException(500, $"Bu sayfa {hasMenuPage.TREE} menüsünde kullanılıyor.");
            }
            T_Pos_AUTH_MENU hasMenu = _repoMenu.Get(@"LOWER(Title)=LOWER(@p1) and PARENT_ID=@p2", new { p1 = menuDto.title, p2 = menuDto.parentMenuDto.id });
            if (hasMenu != null)
            {
                throw new AppException(500, $"{menuDto.title} adlı menü sistemde var olduğundan eklenmedi");
            }

            T_Pos_AUTH_MENU lastOrder = _repoMenu.GetList(@"PARENT_ID=@p1", new { p1 = menuDto.parentMenuDto.id }).OrderByDescending(x => x.ORDER_NO).FirstOrDefault();
            string menuTree = "";
            if (menuDto.parentMenuDto.id == 0)
            {
                menuTree = menuDto.title;
            }
            else
            {
                menuTree = GetTree(menuDto.parentMenuDto.id, "") + " > " + menuDto.title;
            }

            T_Pos_AUTH_MENU menu = new T_Pos_AUTH_MENU()
            {
                TITLE = menuDto.title,
                PAGE_ID = menuDto.pageDto.id,
                HREF = menuDto.href,
                ICON = menuDto.icon,
                TARGET = menuDto.target,
                HAS_SUB_MENU = menuDto.hasSubMenu,
                PARENT_ID = menuDto.parentMenuDto.id,
                TREE = menuTree,
                ORDER_NO = lastOrder != null ? lastOrder.ORDER_NO + 1 : 1
            };

            long newMenuId = _repoMenu.Insert(menu);
            return newMenuId;


        }
        public void Set(MenuDTO menuDto)
        {
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            T_Pos_AUTH_MENU hasMenu = _repoMenu.Get(menuDto.id);
            if (hasMenu == null)
            {
                throw new AppException(500, "Güncellenecek menü bulunamadı");
            }
            if (menuDto.parentMenuDto.id == menuDto.id)
            {
                throw new AppException(500, "Menünün üst menüsü kendisi olamaz");
            }
            var hasMenuPage = _repoMenu.Get("PAGE_ID=@p1 and Id<>@p2", new { p1 = menuDto.pageDto.id, p2 = menuDto.id });
            if (hasMenuPage != null && hasMenuPage.PAGE_ID != 0)
            {
                throw new AppException(500, $"Bu sayfa {hasMenuPage.TREE} menüsünde kullanılıyor.");
            }

            string oldMenuTitle = hasMenu.TITLE;
            T_Pos_AUTH_MENU lastOrder = _repoMenu.GetList(@"PARENT_ID=@p1", new { p1 = menuDto.parentMenuDto.id }).OrderByDescending(x => x.ORDER_NO).FirstOrDefault();


            string menuTree = "";
            if (menuDto.parentMenuDto.id == 0)
            {
                menuTree = menuDto.title;
            }
            else
            {
                menuTree = GetTree(menuDto.parentMenuDto.id, "") + " > " + menuDto.title;
            }


            hasMenu.TITLE = menuDto.title;
            hasMenu.PAGE_ID = menuDto.pageDto.id;
            hasMenu.HREF = menuDto.href;
            hasMenu.ICON = menuDto.icon;
            hasMenu.TARGET = menuDto.target;
            hasMenu.HAS_SUB_MENU = menuDto.hasSubMenu;
            if (hasMenu.PARENT_ID != menuDto.parentMenuDto.id)
            {
                hasMenu.ORDER_NO = lastOrder != null ? lastOrder.ORDER_NO + 1 : 1;
                hasMenu.PARENT_ID = menuDto.parentMenuDto.id;
            }
            hasMenu.TREE = menuTree;

            _repoMenu.Update(hasMenu);
            SubTreeUpdater(hasMenu.ID);

        }
        public void Del(MenuDTO menuDto)
        {
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            var hasMenu = _repoMenu.Get(menuDto.id);

            if (hasMenu == null)
            {
                throw new AppException(500, $"{menuDto.title} menüsü bulunamadığından silme işlemi gerçekleştirilemedi");
            }
            _repoMenu.Delete(hasMenu);


        }
        public void MoveUp(MenuDTO menuDto)
        {
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            var menuListWithSameParent = _repoMenu.GetList("PARENT_ID=@p1 and ORDER_NO<@p2", new { p1 = menuDto.parentMenuDto.id, p2 = menuDto.orderNo });
            var last = menuListWithSameParent.OrderByDescending(x => x.ORDER_NO).FirstOrDefault();
            var first = menuListWithSameParent.OrderBy(x => x.ORDER_NO).FirstOrDefault();

            if (first != null && menuDto.orderNo > first.ORDER_NO)
            {
                T_Pos_AUTH_MENU menuUp = _repoMenu.Get(menuDto.id);
                T_Pos_AUTH_MENU menuDown = _repoMenu.Get(last.ID);

                menuUp.ORDER_NO = menuUp.ORDER_NO - 1;
                menuDown.ORDER_NO = menuDown.ORDER_NO + 1;
                _repoMenu.Update(menuUp);
                _repoMenu.Update(menuDown); ;
            }

        }
        public void MoveDown(MenuDTO menuDto)
        {
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            var menuListWithSameParent = _repoMenu.GetList("PARENT_ID=@p1 and ORDER_NO>@p2", new { p1 = menuDto.parentMenuDto.id, p2 = menuDto.orderNo });
            var last = menuListWithSameParent.OrderByDescending(x => x.ORDER_NO).FirstOrDefault();
            var first = menuListWithSameParent.OrderBy(x => x.ORDER_NO).FirstOrDefault();

            if (last != null && menuDto.orderNo < first.ORDER_NO)
            {
                T_Pos_AUTH_MENU menuDown = _repoMenu.Get(menuDto.id);
                T_Pos_AUTH_MENU menuUp = _repoMenu.Get(first.ID);

                menuDown.ORDER_NO = menuDown.ORDER_NO + 1;
                menuUp.ORDER_NO = menuUp.ORDER_NO - 1;
                _repoMenu.Update(menuDown);
                _repoMenu.Update(menuUp);
            }

        }
        private string GetTree(int menuId, string menuTree)
        {
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            T_Pos_AUTH_MENU menu = _repoMenu.Get(menuId);
            menuTree = menu.TITLE + menuTree;
            if (menu.PARENT_ID != 0)
            {
                menuTree = " > " + menuTree;
                menuTree = GetTree(menu.PARENT_ID, menuTree);
                //menuTree = GetTree(menu.PARENT_ID, menuTree) + menuTree;
            }
            return menuTree;
        }
        private void SubTreeUpdater(long parentMenuId)
        {
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            List<T_Pos_AUTH_MENU> menuList = _repoMenu.GetList("PARENT_ID=@p1", new { p1 = parentMenuId });
            if (menuList.Count > 0)
            {
                foreach (T_Pos_AUTH_MENU menu in menuList)
                {
                    string menuTree = "";
                    if (menu.PARENT_ID == 0)
                    {
                        menuTree = menu.TITLE;
                    }
                    else
                    {
                        menuTree = GetTree(menu.PARENT_ID, "") + " > " + menu.TITLE;
                    }
                    menu.TREE = menuTree;
                    _repoMenu.Update(menu);
                    SubTreeUpdater(menu.ID);
                }
            }
        }
        public List<MenuDTO> GetMenuListByUserId(long userId)
        {
            List<MenuDTO> menuListAllDto = GetList();
            string query = @"select distinct m.* from 
            Pos.AUTH_ROLE_USER ru 
            inner join Pos.AUTH_ROLE_WIDGET rw on rw.ROLE_ID=ru.ROLE_ID and rw.ISDELETED=0
            inner join Pos.AUTH_WIDGET w on w.ID=rw.WIDGET_ID and w.ISDELETED=0
            inner join Pos.AUTH_MENU m on m.PAGE_ID=w.PAGE_ID and m.ISDELETED=0
            where ru.USER_ID=@p1
            and ru.ISDELETED=0
            order by m.ORDER_NO";
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            List<T_Pos_AUTH_MENU> userSubMenuList = _repoMenu.Query<T_Pos_AUTH_MENU>(query, new { p1 = userId }).ToList();
            List<MenuDTO> userSubMenuListDto = _mapper.Map<List<MenuDTO>>(userSubMenuList);
            List<MenuDTO> userMenuListDto = new List<MenuDTO>();
            userMenuListDto = GetWithParentMenu(menuListAllDto, userSubMenuListDto, userMenuListDto);
            userMenuListDto = OrderedMenu(userMenuListDto);
            return userMenuListDto;
        }
        private List<MenuDTO> GetWithParentMenu(List<MenuDTO> menuListAllDto, List<MenuDTO> subMenuListDto, List<MenuDTO> userMenuListDto)
        {
            List<MenuDTO> userMenuListCalculatedDto = userMenuListDto;
            foreach (var subMenuDto in subMenuListDto)
            {
                if (subMenuDto.pageDto != null)
                {
                    subMenuDto.pageDto = _pageManager.Get(subMenuDto.pageDto.id);
                }
            }
            List<int> perentMenuIdList = subMenuListDto.Where(x => x.parentMenuDto.id != 0).Select(x => x.parentMenuDto.id).Distinct().ToList();
            List<MenuDTO> topMenuListDto = subMenuListDto.Where(x => x.parentMenuDto.id == 0).Distinct().ToList();
            foreach (var topMenuDto in topMenuListDto)
            {
                if (userMenuListCalculatedDto.Where(x => x.id == topMenuDto.id).ToList().Count > 0)
                {
                    userMenuListCalculatedDto.Where(x => x.id == topMenuDto.id).FirstOrDefault().subMenuListDto.AddRange(GeneralExtensions.Clone(topMenuDto.subMenuListDto));
                }
                else
                {
                    userMenuListCalculatedDto.Add(GeneralExtensions.Clone(topMenuDto));
                }
            }
            List<MenuDTO> parentMenuListDto = menuListAllDto.Where(x => perentMenuIdList.Contains(x.id)).ToList();
            //userMenuListDto.AddRange(subMenuListDto.Where(x => x.parentMenuDto.id == parentMenuDto.id).ToList()

            foreach (var parentMenuDto in parentMenuListDto)
            {
                parentMenuDto.subMenuListDto = subMenuListDto.Where(x => x.parentMenuDto.id == parentMenuDto.id).ToList();
            }
            if (parentMenuListDto.Count > 0)
            {
                return GetWithParentMenu(menuListAllDto, parentMenuListDto, userMenuListCalculatedDto);
            }
            else
            {
                return userMenuListCalculatedDto;
            }

        }
        private List<MenuDTO> OrderedMenu(List<MenuDTO> userMenuListDto)
        {
            userMenuListDto = userMenuListDto.OrderBy(x => x.orderNo).ToList();
            foreach (var userMenu in userMenuListDto)
            {
                userMenu.subMenuListDto = OrderedMenu(userMenu.subMenuListDto);
            }
            return userMenuListDto;
        }
    }
}

//[CacheRemoveAspect("Get")] 
//[CacheAspect(duration: 10)]
//[SecuredOperation(Priority = 1)]