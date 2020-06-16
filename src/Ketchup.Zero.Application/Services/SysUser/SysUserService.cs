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
using Microsoft.EntityFrameworkCore;

namespace Ketchup.Zero.Application.Services.SysUser
{
    [Service(Name = "Ketchup.Permission.RpcSysUser")]
    public class SysUserService : RpcSysUser.RpcSysUserBase
    {
        private readonly IEfCoreRepository<Domain.SysUser, int> _sysUser;

        public SysUserService(IEfCoreRepository<Domain.SysUser, int> sysUser)
        {
            _sysUser = sysUser;
        }

        [KongRoute(Name = "sysUsers.PageSerachOperate", Tags = new[] { "sysUser" }, Paths = new[] { "/zero/sysUsers/PageSerachSysUser" })]
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
                date.Datas.Add(item);
            });

            return Task.FromResult(date);
        }

        [KongRoute(Name = "sysUsers.CreateOrEditSysUser", Tags = new[] { "sysUser" }, Paths = new[] { "/zero/sysUsers/CreateOrEditSysUser" })]
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

        [KongRoute(Name = "sysUsers.RemoveSysUser", Tags = new[] { "sysUser" }, Paths = new[] { "/zero/sysUsers/RemoveSysUser" })]
        public override Task<RemoveResponse> RemoveSysUser(RemoveRequest request, ServerCallContext context)
        {
            var response = new RemoveResponse();
            try
            {
                if (request.Id == 1)
                    throw new RpcException(new Status(StatusCode.InvalidArgument, "admin不能被删除"));

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
        [KongRoute(Name = "sysUsers.SetRole", Tags = new[] { "sysUser" }, Paths = new[] { "/zero/sysUsers/SetRole" })]
        public override Task<RemoveResponse> SetRole(SetRoleDto request, ServerCallContext context)
        {
            var response = new RemoveResponse();
            try
            {
              var  data = _sysUser.SingleOrDefault(item => item.Id == request.Id);
              if(data == null) {
                  throw new RpcException(new Status(StatusCode.InvalidArgument, "系统用户不存在"));
              }
              data.RoleId = request.RoleId;
                _sysUser.Update(data);
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
