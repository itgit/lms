using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(LMS.Startup))]
namespace LMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
