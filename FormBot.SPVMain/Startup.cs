using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FormBot.SPVMain.Startup))]
namespace FormBot.SPVMain
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }
    }
}