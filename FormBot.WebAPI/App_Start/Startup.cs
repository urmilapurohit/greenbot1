using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Builder;

[assembly: OwinStartup(typeof(FormBot.WebAPI.App_Start.Startup))]

namespace FormBot.WebAPI.App_Start
{
    public class Startup
    {
        public void Configuration(AppBuilder app)
        {
            // For more information on how to configure your application, visit http://go.microsoft.com/fwlink/?LinkID=316888
        }
    }
}
