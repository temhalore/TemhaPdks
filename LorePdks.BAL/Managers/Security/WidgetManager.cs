using AutoMapper;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Page;
using LorePdks.COMMON.DTO.Security.Widget;
using LorePdks.COMMON.Models;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;

namespace LorePdks.BAL.Managers.Security
{
    public class WidgetManager : IWidgetManager
    {
        private readonly IMapper _mapper;
        private readonly IRoleManager _roleManager;
        private readonly IPageManager _pageManager;

        public WidgetManager(
            IMapper mapper,
            IRoleManager roleManager,
            IPageManager pageManager
            )
        {
            _mapper = mapper;
            _roleManager = roleManager;
            _pageManager = pageManager;
        }
        public WidgetDTO Get(long widgetId)
        {
            GenericRepository<T_Pos_AUTH_WIDGET> _repoWidget = new GenericRepository<T_Pos_AUTH_WIDGET>();
            T_Pos_AUTH_WIDGET widget = _repoWidget.Get(widgetId);
            WidgetDTO widgetDto = _mapper.Map<WidgetDTO>(widget);
            widgetDto.pageDto = _pageManager.Get(widgetDto.pageDto.id);
            return widgetDto;
        }
        public List<WidgetDTO> GetListByPageDto(PageDTO pageDto)
        {
            pageDto = _pageManager.Get(pageDto.id);
            GenericRepository<T_Pos_AUTH_WIDGET> _repoWidget = new GenericRepository<T_Pos_AUTH_WIDGET>();

            List<T_Pos_AUTH_WIDGET> widgetList = _repoWidget.GetList("PAGE_ID=@p1", new { p1 = pageDto.id });
            List<WidgetDTO> widgetListDto = _mapper.Map<List<WidgetDTO>>(widgetList);
            foreach (var widgetDto in widgetListDto)
            {
                widgetDto.pageDto = pageDto;
            }

            return widgetListDto;
        }
        public long Add(WidgetDTO widgetDto)
        {
            PageDTO hasPage = _pageManager.Get(widgetDto.pageDto.id);
            if (hasPage == null)
            {
                throw new AppException(500, "Sayfa Bulunamadı");
            }
            GenericRepository<T_Pos_AUTH_WIDGET> _repoWidget = new GenericRepository<T_Pos_AUTH_WIDGET>();

            T_Pos_AUTH_WIDGET hasWidget = _repoWidget.Get(@"LOWER(SELECTOR)=LOWER(:p1) and PAGE_ID=@p2", new { p1 = widgetDto.selector.ToLower(), p2 = widgetDto.pageDto.id });
            if (hasWidget != null)
            {
                throw new AppException(500, "Widget Bu sayfada var");
            }
            List<T_Pos_AUTH_WIDGET> widgetListByPage = _repoWidget.GetList(@"PAGE_ID=@p1", new { p1 = widgetDto.pageDto.id });
            T_Pos_AUTH_WIDGET lastOrder = widgetListByPage.OrderByDescending(x => x.ORDER_NO).FirstOrDefault();
            T_Pos_AUTH_WIDGET widget = new T_Pos_AUTH_WIDGET()
            {
                PAGE_ID = widgetDto.pageDto.id,
                NAME = widgetDto.name,
                SELECTOR = widgetDto.selector.ToLower(),
                ORDER_NO = widgetDto != null && lastOrder != null ? lastOrder.ORDER_NO + 1 : 1
            };

            long newWidgetId = _repoWidget.Insert(widget);
            return newWidgetId;

        }
        public void Set(WidgetDTO widgetDto)
        {
            GenericRepository<T_Pos_AUTH_WIDGET> _repoWidget = new GenericRepository<T_Pos_AUTH_WIDGET>();

            T_Pos_AUTH_WIDGET hasWidget = _repoWidget.Get(widgetDto.id);

            if (hasWidget == null)
            {
                throw new AppException(500, "widget bulnamadı");
            }
            hasWidget.NAME = widgetDto.name;
            hasWidget.SELECTOR = widgetDto.selector;
            _repoWidget.Update(hasWidget);

        }
        public void Del(WidgetDTO widgetDto)
        {
            GenericRepository<T_Pos_AUTH_WIDGET> _repoWidget = new GenericRepository<T_Pos_AUTH_WIDGET>();

            T_Pos_AUTH_WIDGET widget = _repoWidget.Get(widgetDto.id);
            if (widget == null)
            {
                throw new AppException(500, "Sayfaya bağlı Komponent Buluamadı");
            }
            GenericRepository<T_Pos_AUTH_WIDGET_PERMISSION> _repoWidgetPermission = new GenericRepository<T_Pos_AUTH_WIDGET_PERMISSION>();
            bool hasWidgetPermission = _repoWidgetPermission.GetList("WIDGET_ID=@p1", new { p1 = widgetDto.id }).Count > 0;
            if (hasWidgetPermission)
            {
                throw new AppException(500, "Komponente Bağlı Yetkiler Bulunduğundan bu işlemi Yapamazsınız");
            }
            _repoWidget.Delete(widget);
        }

        //public WidgetDTO Get(long pageId, string selector)
        //{
        //    T_Pos_AUTH_WIDGET hasWidget = _repoWidget.Get(@"PageId=@p1 and LOWER(Selector)=@p2", new { p1 = pageId, p2 = selector.ToLower() });
        //    if (hasWidget != null)
        //    {
        //        WidgetDTO widgetDto = _mapper.Map<WidgetDTO>(hasWidget);
        //        return widgetDto;
        //    }
        //    else
        //    {
        //        return null;
        //    }

        //}


        //public void MoveUp(WidgetDTO widgetDto)
        //{
        //    var widgetListWithSameParent = _repoWidget.GetList("PageId=@p1 and OrderNo<@p2", new { p1 = widgetDto.pageDto.id, p2 = widgetDto.orderNo });
        //    var last = widgetListWithSameParent.OrderByDescending(x => x.OrderNo).FirstOrDefault();
        //    var first = widgetListWithSameParent.OrderBy(x => x.OrderNo).FirstOrDefault();

        //    if (first != null && widgetDto.orderNo > first.OrderNo)
        //    {
        //        T_Pos_AUTH_WIDGET menuUp = _repoWidget.Get(widgetDto.id);
        //        T_Pos_AUTH_WIDGET menuDown = _repoWidget.Get(last.Id);

        //        menuUp.OrderNo = menuUp.OrderNo - 1;
        //        menuDown.OrderNo = menuDown.OrderNo + 1;
        //        _repoWidget.Update(menuUp);
        //        _repoWidget.Update(menuDown); ;
        //    }
        //}

        //public void MoveDown(WidgetDTO widgetDto)
        //{
        //    var widgetListWithSameParent = _repoWidget.GetList("PageId=@p1 and OrderNo>@p2", new { p1 = widgetDto.pageDto, p2 = widgetDto.orderNo });
        //    var last = widgetListWithSameParent.OrderByDescending(x => x.OrderNo).FirstOrDefault();
        //    var first = widgetListWithSameParent.OrderBy(x => x.OrderNo).FirstOrDefault();

        //    if (last != null && widgetDto.orderNo < first.OrderNo)
        //    {
        //        T_Pos_AUTH_WIDGET menuDown = _repoWidget.Get(widgetDto.id);
        //        T_Pos_AUTH_WIDGET menuUp = _repoWidget.Get(first.Id);

        //        menuDown.OrderNo = menuDown.OrderNo + 1;
        //        menuUp.OrderNo = menuUp.OrderNo - 1;
        //        _repoWidget.Update(menuDown);
        //        _repoWidget.Update(menuUp);
        //    }
        //}

        public List<WidgetDTO> GetWidgetListByUserId(long userId)
        {
            string query = @"select distinct w.* from 
            Pos.AUTH_ROLE_USER ru
            inner join Pos.AUTH_ROLE_WIDGET rw on rw.ROLE_ID=ru.ROLE_ID and rw.ISDELETED=0 
            inner join Pos.AUTH_WIDGET w on w.ID=rw.WIDGET_ID  and w.ISDELETED=0
            where ru.USER_ID=@p1
            and ru.ISDELETED=0 ";
            GenericRepository<T_Pos_AUTH_WIDGET> _repoWidget = new GenericRepository<T_Pos_AUTH_WIDGET>();

            List<T_Pos_AUTH_WIDGET> userWidgetList = _repoWidget.Query<T_Pos_AUTH_WIDGET>(query, new { p1 = userId }).ToList();

            List<WidgetDTO> userWidgetListDto = _mapper.Map<List<WidgetDTO>>(userWidgetList);
            return userWidgetListDto;
        }//login olan kullanıcısının yetkili olduğu widgetlerı çeker.
    }
}

