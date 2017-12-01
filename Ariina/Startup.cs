using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Ariina.Startup))]
namespace Ariina
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
