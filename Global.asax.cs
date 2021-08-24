using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Unchained
{
    public class Global : HttpApplication
    {

        void Application_Start(object sender, EventArgs e)
        {
            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            
            // Beginning of MVC Goes Here
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            /* This one is consumed by ASP.NET:             RouteConfig.RegisterRoutes(RouteTable.Routes);  */
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            // End of MVC here
            // Start of WebAPI:

            GlobalConfiguration.Configure(WebApiConfig.Register);

            // Reserved:  Any Service code we may want to run in the background goes here.
            System.Threading.Thread t = new System.Threading.Thread(Service.Executor);
            t.Start();
        }
    }
}