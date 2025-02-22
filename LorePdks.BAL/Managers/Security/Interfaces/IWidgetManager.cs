using LorePdks.COMMON.DTO.Security.Page;
using LorePdks.COMMON.DTO.Security.Widget;

namespace LorePdks.BAL.Managers.Security.Interfaces
{
    public interface IWidgetManager
    {
        WidgetDTO Get(long widgetId);
        List<WidgetDTO> GetListByPageDto(PageDTO pageDto);
        long Add(WidgetDTO widgeteDto);
        void Set(WidgetDTO widgeteDto);
        void Del(WidgetDTO widgeteDto);
        List<WidgetDTO> GetWidgetListByUserId(long userId);
    }
}
