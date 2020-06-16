﻿using Autofac;
using Autofac.Extensions.DependencyInjection;
using Ketchup.Core;
using Ketchup.Core.Configurations;
using Ketchup.Core.Utilities;
using Ketchup.Grpc.Internal.Intercept;
using Ketchup.Zero.Application;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ketchup.Zero.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            AppConfig.Configuration = (IConfigurationRoot)configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // Add things to the service collection.
            services.AddGrpc(grpc => grpc.Interceptors.Add<HystrixCommandIntercept>());
            var appConfig = new Application.Config.AppConfig();
            services.AddDbContext<ZeroDbContext>(op => op.UseMySql(appConfig.Zero.Connection));
        }

        public void ConfigureDevelopmentServices(IServiceCollection services)
        {
            // Add things to the service collection that are only for the
            // development environment.
            services.AddGrpc(grpc => grpc.Interceptors.Add<HystrixCommandIntercept>());
            var appConfig = new Application.Config.AppConfig();
            services.AddDbContext<ZeroDbContext>(op => op.UseMySql(appConfig.Zero.Connection));
        }

        public void ConfigureContainer(ContainerBuilder builder)
        {
            // Add things to the Autofac ContainerBuilder.
            builder.AddCoreService().RegisterModules();
        }

        public void ConfigureProductionContainer(ContainerBuilder builder)
        {
            // Add things to the ContainerBuilder that are only for the
            // production environment.
            builder.AddCoreService().RegisterModules();
        }

        public void Configure(IApplicationBuilder app)
        {
            // Set up the application for development.

            ServiceLocator.Current = app.ApplicationServices.GetAutofacRoot();
            app.UseRouting();
            app.UseKetchup();
        }

        public void ConfigureStaging(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            // Set up the application for staging.
            ServiceLocator.Current = app.ApplicationServices.GetAutofacRoot();
            app.UseRouting();
            app.UseKetchup();
        }
    }
}