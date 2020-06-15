using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace Ketchup.Zero.Application.Config
{
    public class AppConfig
    {
        public ZeroOption Zero { get; set; }

        public AppConfig()
        {
            GetZeroAppConfig();
        }

        protected ZeroOption GetZeroAppConfig()
        {
            var section = Core.Configurations.AppConfig.GetSection("Zero");

            if (section.Exists())
                Zero = section.Get<ZeroOption>();
            return Zero;
        }
    }
}
