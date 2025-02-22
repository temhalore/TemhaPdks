using LorePdks.COMMON.DTO.Base;
using LorePdks.COMMON.DTO.Security.Menu;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IMenuManager
    {
        List<TreeTableDTO<MenuDTO>> GetMenuTreeListForAdmin(long? parentId = 0);
        List<MenuDTO> GetList();
        long Add(MenuDTO menuDto);
        void Set(MenuDTO menuDto);
        void Del(MenuDTO menuDto);
        void MoveUp(MenuDTO menuDto);
        void MoveDown(MenuDTO menuDto);
        List<MenuDTO> GetMenuListByUserId(long userId);
    }
}
