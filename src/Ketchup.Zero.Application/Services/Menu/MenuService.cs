using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Grpc.Core;
using Ketchup.Core.Attributes;
using Ketchup.Core.Kong.Attribute;
using Ketchup.Permission;
using Ketchup.Profession.AutoMapper;
using Ketchup.Profession.ORM.EntityFramworkCore.Repository;
using Ketchup.Profession.Specification;
using Ketchup.Zero.Application.Domain;
using Ketchup.Zero.Application.Domain.Repos;
using Ketchup.Zero.Application.Services.Menu.DTO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ketchup.Zero.Application.Services.Menu
{
    [Service(Name = "Ketchup.Permission.RpcMenu")]
    public class MenuService : RpcMenu.RpcMenuBase
    {
        private readonly IEfCoreRepository<SysMenu, int> _menu;
        private readonly IEfCoreRepository<SysOperate, int> _operate;
        private readonly IRoleMenuRepos _roleMenu;

        public MenuService(IEfCoreRepository<SysMenu, int> menu,
            IRoleMenuRepos roleMenu,
            IEfCoreRepository<SysOperate, int> operate)
        {
            _menu = menu;
            _roleMenu = roleMenu;
            _operate = operate;
        }

        /// <summary>
        ///     分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [KongRoute(Name = "menus.PageSerachMenu", Paths = new[] {"/zero/menus/PageSerachMenu"}, Tags = new[] {"menu"})]
        public override Task<MenutList> PageSerachMenu(SearchMenu request, ServerCallContext context)
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

            var date = new MenutList {Total = total};

            ConvertToEntities(result).ForEach(item => { date.Datas.Add(item); });

            return Task.FromResult(date);
        }

        /// <summary>
        ///     创建或修改
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [KongRoute(Name = "menus.CreateOrEditMenu", Paths = new[] {"/zero/menus/CreateOrEditMenu"},
            Tags = new[] {"menu"})]
        public override Task<MenuDto> CreateOrEditMenu(MenuDto request, ServerCallContext context)
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
        ///     根据角色获取菜单
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [KongRoute(Name = "menus.GetMenusByRole", Paths = new[] {"/zero/menus/GetMenusByRole"}, Tags = new[] {"menu"})]
        public override Task<MenusRoleReponse> GetMenusByRole(MenusRoleRequest request, ServerCallContext context)
        {
            var result = new MenusRoleReponse();
            var tree = GetRoleOfMenus(request.RoleId);

            result.Datas.Add(new MenusByRole {Id = 0, Icon = "home", Title = "首页", Path = "/index"});


            tree.ForEach(item =>
            {
                var model = new MenusByRole {Id = item.Id, Title = item.Title, Icon = item.Icon ?? "", Path = ""};

                if (item.Children.Count > 0)
                    item.Children.ForEach(child =>
                    {
                        model.Children.Add(new MenusByRole
                        {
                            Id = child.Id, Icon = child.Icon ?? "", Path = child.Path + "?id=" + child.Id,
                            Title = child.Title
                        });
                    });

                result.Datas.Add(model);
            });

            return Task.FromResult(result);
        }

        /// <summary>
        ///     设置角色权限时获取菜单及其功能
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [KongRoute(Name = "menus.GetMenusSetRole", Paths = new[] {"/zero/menus/GetMenusSetRole"},
            Tags = new[] {"menu"})]
        public override Task<RoleMenuReponse> GetMenusSetRole(MenusRoleRequest request, ServerCallContext context)
        {
            var result = new RoleMenuReponse();

            var datas = _menu.GetAll().OrderBy(item => item.Id).AsNoTracking().ToList();
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
                    Operates = item.Operates,
                    Children = new List<RoleMenuDto>()
                });
            });

            var tree = listMenus.Where(item => item.ParentId == 0).ToList();

            var operates = _operate
                .GetAll()
                .AsNoTracking()
                .Select(item => new
                {
                    item.Id,
                    item.Name,
                    item.Unique
                }).ToList();

            tree.ForEach(item => { BuildMeunsRecursiveTree(listMenus, item); });
            tree.ForEach(item =>
            {
                var model = new MenuModel {Id = $"{item.Id}_0", Lable = item.Title};

                if (item.Children.Count > 0)
                    item.Children.ForEach(child =>
                    {
                        var operateModel = new MenuModel {Id = $"{child.Id}_0", Lable = child.Title};
                        model.Children.Add(operateModel);
                        operates.ForEach(op =>
                        {
                            operateModel.Children.Add(new MenuModel {Id = $"{child.Id}_{op.Id}", Lable = op.Name});
                        });
                    });
                result.List.Add(model);
            });
            var roleMenus = GetRoleOfMenus(request.RoleId);
            roleMenus.ForEach(item =>
            {
                if (item.Children.Count > 0)
                    item.Children.ForEach(child =>
                    {
                        JsonConvert.DeserializeObject<List<int>>(child.Operates).ForEach(operateId =>
                        {
                            operates.ForEach(op =>
                            {
                                if (op.Id != operateId)
                                    return;
                                result.MenuIds.Add($"{child.Id}_{op.Id}");
                            });
                        });
                    });
            });

            return Task.FromResult(result);
        }

        [KongRoute(Name = "menus.RemoveMenu", Paths = new[] {"/zero/menus/RemoveMenu"}, Tags = new[] {"menu"})]
        public override Task<RemoveResponse> RemoveMenu(RemoveRequest request, ServerCallContext context)
        {
            var response = new RemoveResponse();
            try
            {
                _menu.Delete(request.Id);
                response.IsComplete = true;
                return Task.FromResult(response);
            }
            catch
            {
                response.IsComplete = false;
                return Task.FromResult(response);
            }
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