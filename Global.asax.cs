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
using System.Web.UI;

namespace Unchained
{
    public class Global : HttpApplication
    {

        void Session_Start(object sender, EventArgs e)
        {

            Session.Timeout = 86400;
        }
        void Application_Start(object sender, EventArgs e)
        {

            BiblePayCommon.Common.DOMAIN_NAME = System.Web.Hosting.HostingEnvironment.SiteName;
            BiblePayCommon.Common.Log2("BiblePay Unchained - Starting Up - Site Name: " + BiblePayCommon.Common.DOMAIN_NAME);

            bool fOK = BiblePayDLL.Sidechain.CheckForUpgrade();
            if (!fOK)
            {
                Environment.Exit(1);
            }


            BiblePayCommon.Common.User uT = Common.CoerceUser(true);
            BiblePayCommon.Common.User uP = Common.CoerceUser(false);
            BiblePayCommon.Encryption.SetPrivateKey(uT, uP);
            BiblePayCommon.Entity.user1 userTest = new BiblePayCommon.Entity.user1();
            BiblePayCommon.Entity.user1 userProd = new BiblePayCommon.Entity.user1();
            userTest.UserName = "Administrator";
            userTest.FirstName = "Administrator";

            userTest.EmailAddress = "contact@biblepay.org";
            userTest.signingkey = uT.BiblePayAddress;
            userTest.signaturetime = uT.SignatureTimestamp;
            userTest.signature = uT.Signature;
            userTest.BiblePayAddress = uT.BiblePayAddress;
            userTest.id = "fbf35a7907a1958bf3d443bd178f729deefb0c8d2fea066680a49638d422bf4e";

            userProd.UserName = "Administrator";
            userProd.LastName = "Administrator";
            userProd.EmailAddress = "contact@biblepay.org";
            userProd.signingkey = uP.BiblePayAddress;
            userProd.signaturetime = uP.SignatureTimestamp;
            userProd.signature = uP.Signature;
            userProd.BiblePayAddress = uP.BiblePayAddress;
            userProd.id = "c1f4d7c2fb3235e4ae4823659835559f0012a48370826f95ad90a3f51a4baa21";

            BiblePayDLL.Sidechain.InsertIntoDSQL(true, userTest, uT);
            BiblePayDLL.Sidechain.InsertIntoDSQL(false, userProd, uP);

            ScriptManager.ScriptResourceMapping.AddDefinition("jquery-ui", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery-ui-112.js",
                LoadSuccessExpression = "window.jQuery-ui-112"
            });

            ScriptManager.ScriptResourceMapping.AddDefinition("jquery-context-menu", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery.contextMenu.js",
                LoadSuccessExpression = "window.jQuery-context-menu"
            });
            ScriptManager.ScriptResourceMapping.AddDefinition("jquery-toast", new ScriptResourceDefinition
            {
                Path = "~/Scripts/jquery.toast.js",
                LoadSuccessExpression = "window.jQuery-toast"
            });
            ScriptManager.ScriptResourceMapping.AddDefinition("core", new ScriptResourceDefinition
            {
                Path = "~/Scripts/core.js",
                LoadSuccessExpression = "window.core"
            });
            ScriptManager.ScriptResourceMapping.AddDefinition("muse", new ScriptResourceDefinition
            {
                Path = "~/Content/embed-player.min.js",
                LoadSuccessExpression = "window.muse"
            });

            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);

            System.Threading.Thread t = new System.Threading.Thread(Service.Executor);
            t.Start();
        }
    }
}