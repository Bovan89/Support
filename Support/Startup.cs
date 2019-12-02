using Microsoft.Owin;
using Owin;
using Support.DAL;
using Support.Intellect;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(Support.Startup))]
namespace Support
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            RequestProcessor rp = RequestProcessor.GetInstance();
            RequestProcessor.RequestProcessorStrategy = new StrategyFIFO();
            JobSheduler.Start(() => rp.Process());
        }
    }
}
