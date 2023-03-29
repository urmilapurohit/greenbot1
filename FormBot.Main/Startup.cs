using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FormBot.Main.Startup))]
namespace FormBot.Main
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            this.ConfigureAuth(app);
        }
    }
}
