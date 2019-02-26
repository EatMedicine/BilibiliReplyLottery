using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace BilibiliReplyLottery
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            Timer t = new Timer(10 * 1000);
            t.Elapsed += new ElapsedEventHandler(Tools.UpdateResult);
            t.AutoReset = true;
            t.Enabled = true;
            Tools.UpdateMaxPageNum();
            Tools.count = 0;
        }
    }
}
