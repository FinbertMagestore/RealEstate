using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RealEstateWebUI.Startup))]
namespace RealEstateWebUI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
