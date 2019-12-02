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
            //ConfigureAuth(app);

            RequestProcessor rp = RequestProcessor.GetInstance();
            RequestProcessor.RequestProcessorStrategy = new StrategyFIFO();
            JobSheduler.Start(() => rp.Process());
            
            //System.Threading.Tasks.Task.Run(() => rp.Process());


            /*DB Db = DB.Create();

            //var query = (from req in Db.ClientRequest
            //             where req.State == Models.ClientRequest.ClientRequestState.Process && req.StartTime != null &&
            //                (req.ProcessTime == 0 || req.StartTime <= System.Data.Entity.DbFunctions.AddSeconds(System.DateTime.Now, -req.ProcessTime))
            //             join empl in Db.Employee on req.Executor equals empl
            //             select req).ToList();
            var query = (from empl in Db.Employee
                         join req in Db.ClientRequest on empl equals req.Executor
                         where req.State == Models.ClientRequest.ClientRequestState.Process && req.StartTime != null &&
                            (req.ProcessTime == 0 || req.StartTime <= System.Data.Entity.DbFunctions.AddSeconds(System.DateTime.Now, -req.ProcessTime))
                         select req).ToList();
            foreach (var item in query)
            {
                item.State = Models.ClientRequest.ClientRequestState.Complete;
                item.Executor.IsBusy = false;
                Db.SaveChanges();
            }*/
        }
    }
}
