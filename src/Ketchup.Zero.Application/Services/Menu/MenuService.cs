using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Grpc.Core;
using Ketchup.Core.Attributes;
using Ketchup.Core.Kong.Attribute;
using Ketchup.Permission;
using Ketchup.Profession.AutoMapper;
using Ketchup.Profession.ORM.EntityFramworkCore.Repository;
using Ketchup.Profession.Specification;
using Ketchup.Zero.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ketchup.Zero.Application.Services.Menu
{
    [Service(Name = "Ketchup.Permission.RpcMenu")]
    public class MenuService : RpcMenu.RpcMenuBase
    {
        private readonly IEfCoreRepository<SysMenu, int> _menu;

        public MenuService(IEfCoreRepository<SysMenu, int> menu)
        {
            _menu = menu;
        }

        [KongRoute(Name = "menus.PageSerach", Paths = new[] { "/zero/menus/PageSerach" })]
        public override Task<MenutList> PageSerach(SearchMenu request, ServerCallContext context)
        {
            var query = _menu.GetAll().AsNoTracking();

            if (SearchFilter(request) != null)
                query = query.Where(SearchFilter(request));

            query = OrderFilter() != null
                ? query.OrderByDescending(OrderFilter())
                : query.OrderByDescending(item => item.Id);

            var total = query.Count();

            var result = query.Skip(request.PageMax * (request.PageIndex - 1))
                .Take(request.PageMax)
                .ToList();

            var date = new MenutList { Total = total };

            ConvertToEntities(result).ForEach(item =>
            {
                date.Results.Add(item);
            });

            return Task.FromResult(date);
        }

        [KongRoute(Name = "menus.CreateOrEdit", Paths = new[] { "/zero/menus/CreateOrEdit" })]
        public override Task<MenuDto> CreateOrEdit(MenuDto request, ServerCallContext context)
        {
            var menu = request.MapTo<SysMenu>();

            if (menu.Id > 0)
            {
                var oldMenu = _menu.SingleOrDefault(item => item.Id == menu.Id);
                oldMenu.Operates = menu.Operates;
                oldMenu.ParentId = menu.ParentId;
                oldMenu.Name = menu.Name;
                oldMenu.Level = menu.Level;
                oldMenu.Url = menu.Url;
                oldMenu.Sort = menu.Sort;
                oldMenu.Icon = menu.Icon;
                menu = _menu.Update(oldMenu);
            }
            else
            {
                var lastMenu = _menu.GetAll().OrderByDescending(item => item.Id).LastOrDefault();
                if (lastMenu != null && request.Id == 0)
                    menu.Sort = lastMenu.AddOperateSort();
                menu = _menu.Insert(menu);
            }
            return Task.FromResult(menu.MapTo<MenuDto>());
        }

        protected Expression<Func<SysMenu, bool>> SearchFilter(SearchMenu search)
        {
            Expression<Func<SysMenu, bool>> getFilter = item => true;
            if (search.ParentId > 0)
                getFilter = getFilter.And(item => item.ParentId == search.ParentId);
            return getFilter;
        }

        protected Expression<Func<SysMenu, int>> OrderFilter()
        {
            return null;
        }

        protected List<MenuDto> ConvertToEntities(List<SysMenu> entities)
        {
            return entities.MapTo<List<MenuDto>>();
        }
    }
}
