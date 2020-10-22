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
using Ketchup.Zero.Application.Domain;
using Ketchup.Zero.Application.Domain.Repos;
using Ketchup.Zero.Application.Services.Role.DTO;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Ketchup.Zero.Application.Services.Role
{
    [Service(Name = nameof(RpcRole), Package = "Ketchup.Permission", TypeClientName = nameof(RpcRole.RpcRoleClient))]
    public class RoleService : RpcRole.RpcRoleBase
    {
        private readonly IEfCoreRepository<SysMenu, int> _menu;
        private readonly IEfCoreRepository<SysRole, int> _role;
        private readonly IRoleMenuRepos _roleMenu;
        private readonly IMapper _mapper;

        public RoleService(IEfCoreRepository<SysRole, int> role, IRoleMenuRepos roleMenu,
            IEfCoreRepository<SysMenu, int> menu, IMapper mapper)
        {
            _role = role;
            _roleMenu = roleMenu;
            _menu = menu;
            _mapper = mapper;
        }

        /// <summary>
        ///     分页查询
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [KongRoute(Name = "roles.PageSerachRole", Paths = new[] {"/zero/roles/PageSerachRole"},
            Methods = new[] {"POST", "OPTIONS"}, Tags = new[] {"role"})]
        [ServiceRoute(Name = "roles", MethodName = nameof(PageSerachRole))]
        public override Task<RoleList> PageSerachRole(SearchRole request, ServerCallContext context)
        {
            var query = _role.GetAll().AsNoTracking();

            if (SearchFilter(request) != null)
                query = query.Where(SearchFilter(request));

            query = OrderFilter() != null
                ? query.OrderByDescending(OrderFilter())
                : query.OrderByDescending(item => item.Id);

            var total = query.Count();

            var result = query.Skip(request.PageMax * (request.PageIndex - 1))
                .Take(request.PageMax)
                .ToList();

            var date = new RoleList {Total = total};

            ConvertToEntities(result).ForEach(item => { date.Datas.Add(item); });

            return Task.FromResult(date);
        }

        /// <summary>
        ///     创建或修改
        /// </summary>
        /// <param name="request"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        [KongRoute(Name = "roles.CreateOrEditRole", Paths = new[] {"/zero/roles/CreateOrEditRole"},
            Methods = new[] {"POST", "OPTIONS"}, Tags = new[] {"role"})]
        [ServiceRoute(Name = "roles", MethodName = nameof(CreateOrEditRole))]
        public override Task<RoleDto> CreateOrEditRole(RoleDto request, ServerCallContext context)
        {
            var role = _mapper.Map<SysRole>(request);

            if (role.Id > 0)
            {
                var oldMenu = _role.SingleOrDefault(item => item.Id == role.Id);

                oldMenu.Name = role.Name;
                oldMenu.Remark = role.Remark;
                role = _role.Update(oldMenu);
            }
            else
            {
                role = _role.Insert(role);
            }

            return Task.FromResult(_mapper.Map<RoleDto>(role));
        }

        [KongRoute(Name = "roles.RemoveRole", Tags = new[] {"role"}, Methods = new[] {"POST", "OPTIONS"},
            Paths = new[] {"/zero/roles/RemoveRole"})]
        [ServiceRoute(Name = "roles", MethodName = nameof(RemoveRole))]
        public override Task<RemoveResponse> RemoveRole(RemoveRequest request, ServerCallContext context)
        {
            var response = new RemoveResponse();

            if (request.Id == 1)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "管理员角色不能被删除"));
            try
            {
                _role.Delete(request.Id);
                response.IsComplete = true;
                return Task.FromResult(response);
            }
            catch
            {
                response.IsComplete = false;
                return Task.FromResult(response);
            }
        }

        [KongRoute(Name = "roles.SetRolePermission", Tags = new[] {"role"},
            Methods = new[] {"POST", "OPTIONS"}, Paths = new[] {"/zero/roles/SetRolePermission"})]
        [ServiceRoute(Name = "roles", MethodName = nameof(SetRolePermission))]
        public override Task<SetRolePermissionResponse> SetRolePermission(SetRolepermissionRequest request,
            ServerCallContext context)
        {
            var result = new SetRolePermissionResponse {IsComplete = true};
            var datas = _roleMenu.GetAllList(item => item.RoleId == request.RoleId);
            if (datas.Count > 0)
                _roleMenu.Delete(item => item.RoleId == request.RoleId);

            if (request.MenuIds.Count == 0)
                return Task.FromResult(result);

            var models = new List<RolePermissionDto>();
            var list = new List<SysRoleMenu>();

            request.MenuIds.ToList().ForEach(item =>
            {
                var model = new RolePermissionDto();
                var operateArray = item.Split('_');
                if (Convert.ToInt32(operateArray.LastOrDefault()) == 0)
                {
                    if (models.FirstOrDefault(m => m.MenuId == Convert.ToInt32(operateArray.FirstOrDefault())) != null)
                        return;
                    model.MenuId = Convert.ToInt32(operateArray.FirstOrDefault());
                    models.Add(model);
                }
                else
                {
                    var data = models.FirstOrDefault(m => m.MenuId == Convert.ToInt32(operateArray.FirstOrDefault()));
                    if (data == null)
                    {
                        model.MenuId = Convert.ToInt32(operateArray.FirstOrDefault());
                        model.Operates.Add(Convert.ToInt32(operateArray.LastOrDefault()));
                        models.Add(model);
                    }
                    else
                    {
                        data.Operates.Add(Convert.ToInt32(operateArray.LastOrDefault()));
                    }
                }
            });

            models.ForEach(rp =>
            {
                var menu = _menu.SingleOrDefault(item => item.Id == rp.MenuId);
                if (menu == null)
                    return;

                var roleMenu = new SysRoleMenu
                {
                    MenuId = rp.MenuId,
                    RoleId = request.RoleId,
                    Operates = JsonConvert.SerializeObject(menu.ParentId == 0
                        ? new List<int>()
                        : rp.Operates)
                };

                list.Add(roleMenu);
            });

            if (!_roleMenu.BatchInsert(list))
                result.IsComplete = false;

            return Task.FromResult(result);
        }

        #region

        protected Expression<Func<SysRole, bool>> SearchFilter(SearchRole search)
        {
            Expression<Func<SysRole, bool>> getFilter = item => true;
            if (!string.IsNullOrEmpty(search.RoleName))
                getFilter = getFilter.And(item => item.Name.Contains(search.RoleName));
            return getFilter;
        }

        protected Expression<Func<SysRole, int>> OrderFilter()
        {
            return null;
        }

        protected List<RoleDto> ConvertToEntities(List<SysRole> entities)
        {
            return _mapper.Map<List<RoleDto>>(entities);
        }

        #endregion
    }
}