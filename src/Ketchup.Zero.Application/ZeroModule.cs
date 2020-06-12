using System;
using Autofac;
using Ketchup.Core;
using Ketchup.Core.Modules;
using Ketchup.Profession.ORM.EntityFramworkCore.Context;
using Microsoft.AspNetCore.Routing;

namespace Ketchup.Zero.Application
{
    public class ZeroModule : KernelModule
    {
        public override void Initialize(KetchupPlatformContainer builder)
        {
            base.Initialize(builder);
        }

        public override void MapGrpcService(IEndpointRouteBuilder endpointRoute)
        {
            base.MapGrpcService(endpointRoute);
        }

        protected override void RegisterModule(ContainerBuilderWrapper builder)
        {
            builder.ContainerBuilder.RegisterType<ZeroDbContext>().As<IEfCoreContext>();
        }
    }
}
