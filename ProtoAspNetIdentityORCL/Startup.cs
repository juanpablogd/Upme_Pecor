using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NSPecor.Startup))]
namespace NSPecor
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
