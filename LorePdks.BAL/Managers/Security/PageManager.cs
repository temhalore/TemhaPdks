using AutoMapper;
using LorePdks.BAL.Managers.Security.Interfaces;
using LorePdks.COMMON.DTO.Security.Page;
using LorePdks.COMMON.Models;
using LorePdks.DAL.Model;
using LorePdks.DAL.Repository;


namespace LorePdks.BAL.Managers.Security
{
    public class PageManager : IPageManager
    {
        private IMapper _mapper;

        public PageManager(
            IMapper mapper
            )
        {
            _mapper = mapper;
        }
        public PageDTO Get(long pageId)
        {
            GenericRepository<T_Pos_AUTH_PAGE> _repoPage = new GenericRepository<T_Pos_AUTH_PAGE>();
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            T_Pos_AUTH_PAGE page = _repoPage.Get(pageId);
            PageDTO pageDto = _mapper.Map<PageDTO>(page);
            if (pageDto != null)
            {
                pageDto.menuTree = _repoMenu.Get("PAGE_ID=@p1", new { p1 = pageDto.id })?.TREE;
            }
            return pageDto;
        }
        public List<PageDTO> GetList()
        {
            GenericRepository<T_Pos_AUTH_PAGE> _repoPage = new GenericRepository<T_Pos_AUTH_PAGE>();
            GenericRepository<T_Pos_AUTH_MENU> _repoMenu = new GenericRepository<T_Pos_AUTH_MENU>();
            List<T_Pos_AUTH_PAGE> pageList = _repoPage.GetList().OrderBy(x => x.ORDER_NO).ToList();
            List<PageDTO> pageListDto = _mapper.Map<List<PageDTO>>(pageList).OrderBy(x => x.orderNo).ToList();
            List<T_Pos_AUTH_MENU> menuList = _repoMenu.GetList();
            foreach (var pageDto in pageListDto)
            {
                pageDto.menuTree = menuList.Where(x => x.PAGE_ID == pageDto.id).FirstOrDefault()?.TREE;
            }
            return pageListDto.OrderBy(x => x.name).ToList();
        }
        public long Add(PageDTO pageDto)
        {
            GenericRepository<T_Pos_AUTH_PAGE> _repoPage = new GenericRepository<T_Pos_AUTH_PAGE>();
            List<T_Pos_AUTH_PAGE> pageList = _repoPage.GetList();
            pageDto.routerLink = pageDto.routerLink.Trim();
            T_Pos_AUTH_PAGE hasPage = pageList.Where(x => x.ROUTER_LINK.ToLower() == pageDto.routerLink.ToLower()).FirstOrDefault();
            T_Pos_AUTH_PAGE lastOrder = pageList.OrderByDescending(x => x.ORDER_NO).FirstOrDefault();


            if (hasPage == null)
            {
                T_Pos_AUTH_PAGE newPage = _mapper.Map<T_Pos_AUTH_PAGE>(pageDto);
                newPage.ORDER_NO = lastOrder == null ? 1 : lastOrder.ORDER_NO + 1;
                long newPageId = _repoPage.Insert(newPage);
                return newPageId;
            }
            else
            {
                throw new AppException(500, $"{pageDto.name} adlı sayfa sistemde var olduğundan eklenmedi");
            }
        }
        public void Set(PageDTO pageDto)
        {
            GenericRepository<T_Pos_AUTH_PAGE> _repoPage = new GenericRepository<T_Pos_AUTH_PAGE>();
            T_Pos_AUTH_PAGE hasPage = _repoPage.Get(pageDto.id);
            //string oldPageName = pageDto.name;

            if (hasPage != null)
            {
                hasPage.NAME = pageDto.name;
                hasPage.ROUTER_LINK = pageDto.routerLink;
                hasPage.ORDER_NO = pageDto.orderNo;
                _repoPage.Update(hasPage);
            }
            else
            {
                throw new AppException(500, $"Mantıksız işlem yürütüldü. {pageDto.name} adlı sayfa güncellenemedi");

            }
        }
        public void Del(PageDTO pageDto)
        {
            GenericRepository<T_Pos_AUTH_PAGE> _repoPage = new GenericRepository<T_Pos_AUTH_PAGE>();
            GenericRepository<T_Pos_AUTH_WIDGET> _repoWidget = new GenericRepository<T_Pos_AUTH_WIDGET>();
            T_Pos_AUTH_PAGE hasPage = _repoPage.Get(pageDto.id);

            if (hasPage == null)
            {
                throw new AppException(500, "Sayfa bulunamadığından siliemedi");
            }
            bool hasPageWidget = _repoWidget.GetList("PAGE_ID=@p1", new { p1 = pageDto.id }).Count > 0;

            if (hasPageWidget)
            {
                throw new AppException(500, "Sayfaya bağlı komponentler bulunduğğundan sayfa silinemedi");
            }

            _repoPage.Delete(hasPage);


        }
        public void MoveUp(PageDTO pageDto)
        {
            GenericRepository<T_Pos_AUTH_PAGE> _repoPage = new GenericRepository<T_Pos_AUTH_PAGE>();
            var pageList = _repoPage.GetList();
            var last = pageList.OrderByDescending(x => x.ORDER_NO).Where(x => x.ORDER_NO < pageDto.orderNo).FirstOrDefault();
            var first = pageList.OrderBy(x => x.ORDER_NO).FirstOrDefault();

            if (first != null && pageDto.orderNo > first.ORDER_NO)
            {
                T_Pos_AUTH_PAGE pageUp = pageList.Where(x => x.ID == pageDto.id).FirstOrDefault();
                T_Pos_AUTH_PAGE pageDown = pageList.Where(x => x.ID == last.ID).FirstOrDefault();

                pageUp.ORDER_NO = pageUp.ORDER_NO - 1;
                pageDown.ORDER_NO = pageDown.ORDER_NO + 1;
                _repoPage.Update(pageUp);
                _repoPage.Update(pageDown); ;
            }

        }
        public void MoveDown(PageDTO pageDto)
        {
            GenericRepository<T_Pos_AUTH_PAGE> _repoPage = new GenericRepository<T_Pos_AUTH_PAGE>();
            var pageList = _repoPage.GetList();
            var last = pageList.OrderByDescending(x => x.ORDER_NO).FirstOrDefault();
            var first = pageList.OrderBy(x => x.ORDER_NO).Where(x => x.ORDER_NO > pageDto.orderNo).FirstOrDefault();

            if (last != null && pageDto.orderNo < last.ORDER_NO)
            {
                T_Pos_AUTH_PAGE pageDown = pageList.Where(x => x.ID == pageDto.id).FirstOrDefault();
                T_Pos_AUTH_PAGE pageUp = pageList.Where(x => x.ID == first.ID).FirstOrDefault();

                pageDown.ORDER_NO = pageDown.ORDER_NO + 1;
                pageUp.ORDER_NO = pageUp.ORDER_NO - 1;
                _repoPage.Update(pageDown);
                _repoPage.Update(pageUp);
            }
        }
        public List<PageDTO> GetPageListByUserId(long userId)
        {
            GenericRepository<T_Pos_AUTH_PAGE> _repoPage = new GenericRepository<T_Pos_AUTH_PAGE>();
            string query = @"select distinct p.* from 
            Pos.AUTH_ROLE_USER ru 
            inner join Pos.AUTH_ROLE_WIDGET rw on rw.ROLE_ID=ru.ROLE_ID and ru.ISDELETED=0 
            inner join Pos.AUTH_WIDGET w on w.ID=rw.WIDGET_ID and w.ISDELETED=0
            inner join Pos.AUTH_PAGE p on p.ID=w.PAGE_ID and p.ISDELETED=0
            where ru.USER_ID=@p1
            and ru.ISDELETED=0 ";

            List<T_Pos_AUTH_PAGE> userPageList = _repoPage.Query<T_Pos_AUTH_PAGE>(query, new { p1 = userId }).ToList();

            List<PageDTO> userPageListDto = _mapper.Map<List<PageDTO>>(userPageList);
            return userPageListDto;
        }//login olan kullanıcısının yetkili olduğu pagelerı çeker.
    }
}