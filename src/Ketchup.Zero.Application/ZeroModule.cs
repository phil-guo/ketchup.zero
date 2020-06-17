using System;
using Autofac;
using AutoMapper;
using Ketchup.Core;
using Ketchup.Core.Kong;
using Ketchup.Core.Modules;
using Ketchup.Permission;
using Ketchup.Profession.ORM.EntityFramworkCore.Context;
using Ketchup.Profession.ORM.EntityFramworkCore.UntiOfWork;
using Ketchup.Zero.Application.Domain.Repos;
using Ketchup.Zero.Application.Services.Menu;
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
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<ZeroDbContext>().As<IEfCoreContext>();
            builder.ContainerBuilder.RegisterType<ZeroUnitOfWork>().As<IEfUnitOfWork>();
            builder.ContainerBuilder.RegisterType<RoleMenuReponse>().As<IRoleMenuRepos>();
        }
    }
}
