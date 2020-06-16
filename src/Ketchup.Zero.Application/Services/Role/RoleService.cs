using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Ketchup.Core.Attributes;
using Ketchup.Core.Kong.Attribute;
using Ketchup.Permission;
using Ketchup.Profession.AutoMapper;
using Ketchup.Profession.ORM.EntityFramworkCore.Repository;
using Ketchup.Profession.Specification;
using Ketchup.Zero.Application.Domain;
using Microsoft.EntityFrameworkCore;
namespace Ketchup.Zero.Application.Services.Role
{
    [Service(Name = "Ketchup.Permission.RpcRole")]
    public  class RoleService: RpcRole.RpcRoleBase
    {
         private readonly IEfCoreRepository<SysRole, int> _role;
         public RoleService(IEfCoreRepository<SysRole, int> role)
         {
             _role = role;
         }
         /// <summary>
         /// 分页查询
         /// </summary>
         /// <param name="request"></param>
         /// <param name="context"></param>
         /// <returns></returns>
         [KongRoute(Name = "roles.PageSerach", Paths = new[] { "/zero/roles/PageSerach" })]
         public override Task<RoleList> PageSerach(SearchRole request, ServerCallContext context)
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

             var date = new RoleList { Total = total };

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
         [KongRoute(Name = "role.CreateOrEdit", Paths = new[] { "/zero/roles/CreateOrEdit" })]
         public override Task<RoleDto> CreateOrEdit(RoleDto request, ServerCallContext context)
         {
             var role = request.MapTo<SysRole>();

             if (role.Id > 0)
             {
                 var oldMenu = _role.SingleOrDefault(item => item.Id == role.Id);
                
                 oldMenu.Name = role.Name;
                 oldMenu.Remark = role.Remark;
                 role = _role.Update(oldMenu);
             }
             else
             {
                 var lastMenu = _role.GetAll().OrderByDescending(item => item.Id).LastOrDefault();
                 role = _role.Insert(role);
             }
             return Task.FromResult(role.MapTo<RoleDto>());
         }
         [KongRoute(Name = "sysRoles.RemoveRole", Tags = new[] { "sysUser" }, Paths = new[] { "/zero/roles/RemoveRole" })]
         public override Task<RemoveRoleResponse> RemoveRole(RemoveRoleRequest request, ServerCallContext context)
         {
             var response = new RemoveRoleResponse();
             try
             {
                 if (request.Id == 1)
                     throw new RpcException(new Status(StatusCode.InvalidArgument, "管理员角色不能被删除"));

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
        #region
        //todo 需要优化

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
            return entities.MapTo<List<RoleDto>>();
        }
        #endregion

    }
}
