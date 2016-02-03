using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Transcoder.WebApp.Web.Startup))]
namespace Transcoder.WebApp.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
