using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Barangay_Management_Information_System.Startup))]
namespace Barangay_Management_Information_System
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
