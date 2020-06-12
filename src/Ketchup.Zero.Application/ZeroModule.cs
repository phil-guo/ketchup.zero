using System;
using Ketchup.Core;
using Ketchup.Core.Modules;
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
            base.RegisterModule(builder);
        }
    }
}
