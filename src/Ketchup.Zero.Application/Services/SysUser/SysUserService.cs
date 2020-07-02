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
using Ketchup.Profession.Utilis;
using Ketchup.Zero.Application.Domain;
using Microsoft.EntityFrameworkCore;

namespace Ketchup.Zero.Application.Services.SysUser
{
    [Service(Name = nameof(RpcSysUser), Package = "Ketchup.Permission", TypeClientName = nameof(RpcSysUser.RpcSysUserClient))]
    public class SysUserService : RpcSysUser.RpcSysUserBase
    {
        private readonly IEfCoreRepository<Domain.SysUser, int> _sysUser;
        private readonly IEfCoreRepository<SysRole, int> _role;

        public SysUserService(IEfCoreRepository<Domain.SysUser, int> sysUser, IEfCoreRepository<SysRole, int> role)
        {
            _sysUser = sysUser;
            _role = role;
        }

        [KongRoute(Name = "sysUsers.PageSerachSysUser", Tags = new[] { "sysUser" },
            Paths = new[] { "/zero/sysUsers/PageSerachSysUser" })]
        [ServiceRoute(Name = "sysUsers", MethodName = nameof(PageSerachSysUser))]
        public override Task<SearchSysUserResponse> PageSerachSysUser(SearchSysUser request, ServerCallContext context)
        {
            var query = _sysUser.GetAll().AsNoTracking();

            if (SearchFilter(request) != null)
                query = query.Where(SearchFilter(request));

            query = OrderFilter() != null
                ? query.OrderByDescending(OrderFilter())
                : query.OrderByDescending(item => item.Id);

            var total = query.Count();

            var result = query.Skip(request.PageMax * (request.PageIndex - 1))
                .Take(request.PageMax)
                .ToList();

            var date = new SearchSysUserResponse { Total = total };

            ConvertToEntities(result).ForEach(item =>
            {
                var role = _role.SingleOrDefault(ro => ro.Id == item.RoleId);
                item.RoleName = role?.Name;
                date.Datas.Add(item);
            });

            return Task.FromResult(date);
        }

        [KongRoute(Name = "sysUsers.CreateOrEditSysUser", Tags = new[] { "sysUser" },
            Paths = new[] { "/zero/sysUsers/CreateOrEditSysUser" })]
        [ServiceRoute(Name = "sysUsers", MethodName = nameof(CreateOrEditSysUser))]
        public override Task<SysUserDto> CreateOrEditSysUser(SysUserDto request, ServerCallContext context)
        {
            if (request.Id == 1)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "admin管理员不允许被修改"));

            Domain.SysUser data = null;
            if (request.Id == 0)
            {
                request.Password = request.Password.Get32MD5One();
                data = _sysUser.Insert(request.MapTo<Domain.SysUser>());
            }
            else
            {
                data = _sysUser.SingleOrDefault(item => item.Id == request.Id);
                if (data == null)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "系统用户不存在"));

                data.RoleId = request.RoleId;
                data.UserName = request.UserName;
                data = _sysUser.Update(data);
            }

            return Task.FromResult(data.MapTo<SysUserDto>());
        }

        [KongRoute(Name = "sysUsers.RemoveSysUser", Tags = new[] { "sysUser" },
            Paths = new[] { "/zero/sysUsers/RemoveSysUser" })]
        [ServiceRoute(Name = "sysUsers", MethodName = nameof(RemoveSysUser))]
        public override Task<RemoveResponse> RemoveSysUser(RemoveRequest request, ServerCallContext context)
        {
            var response = new RemoveResponse();

            if (request.Id == 1)
                throw new RpcException(new Status(StatusCode.InvalidArgument, "admin不能被删除"));
            try
            {
                _sysUser.Delete(request.Id);
                response.IsComplete = true;
                return Task.FromResult(response);
            }
            catch
            {
                response.IsComplete = false;
                return Task.FromResult(response);
            }
        }

        protected Expression<Func<Domain.SysUser, bool>> SearchFilter(SearchSysUser search)
        {
            return null;
        }

        protected Expression<Func<Domain.SysUser, int>> OrderFilter()
        {
            return null;
        }

        protected List<SysUserDto> ConvertToEntities(List<Domain.SysUser> entities)
        {
            return entities.MapTo<List<SysUserDto>>();
        }
    }
}