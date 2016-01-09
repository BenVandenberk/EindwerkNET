using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(UurFac.Startup))]
namespace UurFac
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
