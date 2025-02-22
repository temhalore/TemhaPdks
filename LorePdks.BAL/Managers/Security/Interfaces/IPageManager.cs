using LorePdks.COMMON.DTO.Security.Page;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IPageManager
    {
        PageDTO Get(long pageId);
        List<PageDTO> GetList();
        long Add(PageDTO pageDto);
        void Set(PageDTO pageDto);
        void Del(PageDTO pageDto);
        void MoveUp(PageDTO pageDto);
        void MoveDown(PageDTO pageDto);
        List<PageDTO> GetPageListByUserId(long userId);
    }
}
