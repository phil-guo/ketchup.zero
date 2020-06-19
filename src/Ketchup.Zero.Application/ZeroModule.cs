using Autofac;
using AutoMapper;
using Ketchup.Core;
using Ketchup.Core.Kong;
using Ketchup.Core.Modules;
using Ketchup.Profession.ORM.EntityFramworkCore.Context;
using Ketchup.Profession.ORM.EntityFramworkCore.UntiOfWork;
using Ketchup.Zero.Application.Domain.Repos;
using Ketchup.Zero.Application.Domain.Repos.Imp;
using Ketchup.Zero.Application.Services.Auth;
using Ketchup.Zero.Application.Services.Menu;
using Ketchup.Zero.Application.Services.Operate;
using Ketchup.Zero.Application.Services.Role;
using Ketchup.Zero.Application.Services.SysUser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace Ketchup.Zero.Application
{
    public class ZeroModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            Mapper.Initialize(cfg => cfg.AddProfile<PermissionProfile>());
            builder.GetInstances<IKongNetProvider>().AddKongSetting();
        }

        public override void MapGrpcService(IEndpointRouteBuilder endpointRoute)
        {
            endpointRoute.MapGrpcService<MenuService>();
            endpointRoute.MapGrpcService<AuthService>();
            endpointRoute.MapGrpcService<OperateService>();
            endpointRoute.MapGrpcService<RoleService>();
            endpointRoute.MapGrpcService<SysUserService>();
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<ZeroDbContext>().As<IEfCoreContext>();
            builder.ContainerBuilder.RegisterType<ZeroUnitOfWork>().As<IEfUnitOfWork>();
            builder.ContainerBuilder.RegisterType<RoleMenuRepos>().As<IRoleMenuRepos>().InstancePerLifetimeScope();
        }
    }
}
