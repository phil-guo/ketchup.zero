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
using Ketchup.Zero.Application.Services.Menu.DTO;
using Microsoft.EntityFrameworkCore;

namespace Ketchup.Zero.Application.Services.Menu
{
    [Service(Name = "Ketchup.Permission.RpcMenu")]
    public class MenuService : RpcMenu.RpcMenuBase
    {
        private readonly IEfCoreRepository<SysMenu, int> _menu;
        private readonly IEfCoreRepository<SysRoleMenu, int> _roleMenu;

        public MenuService(IEfCoreRepository<SysMenu, int> menu, IEfCoreRepository<SysRoleMenu, int> roleMenu)
        {
            _menu = menu;
            _roleMenu = roleMenu;
        }

        /// <summary>
        /// 分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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
                date.Datas.Add(item);
            });

            return Task.FromResult(date);
        }

        /// <summary>
        /// 创建或修改
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 根据角色获取菜单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [KongRoute(Name = "menus.GetMenusByRole", Paths = new[] { "/zero/menus/GetMenusByRole" })]
        public override Task<MenusRoleReponse> GetMenusByRole(MenusRoleRequest request, ServerCallContext context)
        {
            var result = new MenusRoleReponse();
            var tree = GetRoleOfMenus(request.RoleId);

            result.Datas.Add(new MenusByRole() { Id = 0, Icon = "home", Title = "首页", Path = "/index" });


            tree.ForEach(item =>
            {
                var model = new MenusByRole { Id = item.Id, Title = item.Title, Icon = item.Icon ?? "", Path = "" };

                if (item.Children.Count > 0)
                    item.Children.ForEach(child =>
                    {
                        model.Children.Add(new MenusByRole
                        { Id = child.Id, Icon = child.Icon ?? "", Path = child.Path + "?id=" + child.Id, Title = child.Title });
                    });

                result.Datas.Add(model);
            });

            return Task.FromResult(result);
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

        private List<RoleMenuDto> GetRoleOfMenus(int roleId)
        {
            var datas = GetRoleMenu(roleId);

            var listMenus = new List<RoleMenuDto>();
            datas.ForEach(item =>
            {
                listMenus.Add(new RoleMenuDto
                {
                    ParentId = item.ParentId,
                    Id = item.Id,
                    Title = item.Name,
                    Icon = item.Icon,
                    Path = item.Url,
                    Operates = item.Operates
                });
            });

            var tree = listMenus.Where(item => item.ParentId == 0).ToList();

            tree.ForEach(item => { BuildMeunsRecursiveTree(listMenus, item); });

            return tree;
        }

        private List<SysMenu> GetRoleMenu(int roleId)
        {
            var menus = new List<SysMenu>();
            var roleMenusByRole = _roleMenu.GetAll()
                .Where(item => item.RoleId == roleId)
                .OrderByDescending(item => item.MenuId)
                .ToList();

            if (roleMenusByRole.Count == 0)
                return menus;
            roleMenusByRole.ForEach(rm =>
            {
                var menu = _menu.SingleOrDefault(item => item.Id == rm.MenuId);
                menus.Add(menu);
            });
            return menus;
        }

        private void BuildMeunsRecursiveTree(List<RoleMenuDto> list, RoleMenuDto currentTree)
        {
            list.ForEach(item =>
            {
                if (item.ParentId == currentTree.Id) currentTree.Children.Add(item);
            });
        }
    }
}
