using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using System.Web;
using Owin;

[assembly: OwinStartup(typeof(FormBot.VendorAPI.Startup))]

namespace FormBot.VendorAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}