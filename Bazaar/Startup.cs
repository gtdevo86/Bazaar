using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Bazaar.Startup))]
namespace Bazaar
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
