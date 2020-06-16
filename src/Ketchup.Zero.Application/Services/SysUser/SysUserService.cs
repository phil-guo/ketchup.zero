using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Grpc.Core;
using Ketchup.Core.Attributes;
using Ketchup.Permission;

namespace Ketchup.Zero.Application.Services.SysUser
{
    [Service(Name = "Ketchup.Permission.RpcSysUser")]
    public class SysUserService : RpcSysUser.RpcSysUserBase
    {
        public override Task<SearchSysUserReponse> PageSerachSysUser(SearchSysUser request, ServerCallContext context)
        {
            return base.PageSerachSysUser(request, context);
        }

        public override Task<SysUserDto> CreateOrEditSysUser(SysUserDto request, ServerCallContext context)
        {
            return base.CreateOrEditSysUser(request, context);
        }
    }
}
