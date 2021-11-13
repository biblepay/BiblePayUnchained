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
using System.Diagnostics;
using System.Net.Mail;

namespace Unchained
{
    public class Global : HttpApplication
    {

        void Session_Start(object sender, EventArgs e)
        {

            Session.Timeout = 86400;
        }

        void Application_Error(object sender, EventArgs e)
        {
            System.Web.UI.Page page = System.Web.HttpContext.Current.Handler as System.Web.UI.Page;

            bool fSessionExists = false;
            if (Context.Handler is IRequiresSessionState ||  Context.Handler is IReadOnlySessionState)
            {
                fSessionExists = true;
            }


            BiblePayCommon.Common.User u = new BiblePayCommon.Common.User();
            if (fSessionExists)
            {
                u = Common.gUserSession(Session);
            }

            Exception ex = Server.GetLastError().GetBaseException();
            bool fHandled = false;
            if (ex.Message.Contains("does not exist") && ex.Message.Contains("The file"))
            {
                // 404
                Response.Redirect("~/Images/404.jpg");
                fHandled = true;
                return;
            }
            else if (ex.Message.Contains("Invalid length for a Base-64 char array or string."))
            {
                fHandled = true;
            }
            else if (ex.Message.Contains("This is an invalid webresource"))
            {
                fHandled = true;
            }
            else if (ex.Message.Contains("A potentially dangerous Request.Path"))
            {
                fHandled = true;
            }
            else if (ex.Message.Contains("The file") && ex.Message.Contains("does not exist"))
            {
                fHandled = true;
            }
            Server.ClearError();
            string sNarr = "User: " + u.FullUserName() + "\r\nGlobalExceptionHandler::Error Caught in Application_Error event" +
                            "Error in: " + Request.Url.ToString() + " \r\n" +
                            "Error Message: " + ex.Message.ToString() +
                            "Stack Trace:" + ex.StackTrace.ToString();

            if (!fHandled)
            {
                MailAddress mTo = new MailAddress("rob@biblepay.org", "Rob Andrews");
                MailMessage m = new MailMessage(mTo, mTo);

                string sSubject = "SERVER ERROR " + Request.Url.ToString() + " " + BiblePayCommon.Common.Mid(sNarr, 0, 200);
                sSubject = sSubject.Replace('\r', ' ').Replace('\n', ' ');
                m.Subject = sSubject;
                m.Body = sNarr;
                m.IsBodyHtml = false;
                BiblePayDLL.Sidechain.SendMail(false, m, "");
            }

            BiblePayCommon.Common.Log2(Request.Url.ToString() + "\r\n" + sNarr);
            if (fSessionExists)
            {
                
                Session["MSGBOX_TITLE"] = "An exception occurred, but we see the problem!";
                Session["MSGBOX_BODY"] = "Sorry, an exception occurred while processing your request.  However, we were able to e-mail the team with the entire set of details leading up to the error.  Please accept our apologies and continue to enjoy our system.  <br><br>";
                Response.Redirect("MessagePage");
            }
            else
            {
                Response.Redirect("VideoList");
            }
            //            UICommon.MsgBox("Error", "Sorry, an error occurred while performing this request.  I have e-mailed the support team with all of the details.   "                    +"We will open a ticket to work this problem.  ",  page);

        }

        void Application_Start(object sender, EventArgs e)
        {

            BiblePayCommon.Common.DOMAIN_NAME = System.Web.Hosting.HostingEnvironment.SiteName;
            BiblePayCommon.Common.Log2("BiblePay Unchained v1.3 - Starting Up - Site Name: " + BiblePayCommon.Common.DOMAIN_NAME);

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

            //BiblePayDLL.Sidechain.InsertIntoDSQL(true, userTest, uT);
            //BiblePayDLL.Sidechain.InsertIntoDSQL(false, userProd, uP);

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
                Path = "~/Scripts/core3.js",
                LoadSuccessExpression = "window.core"
            });
            ScriptManager.ScriptResourceMapping.AddDefinition("muse", new ScriptResourceDefinition
            {
                Path = "~/Content/embed-player.min.js",
                LoadSuccessExpression = "window.muse"
            });
            ScriptManager.ScriptResourceMapping.AddDefinition("lightbox", new ScriptResourceDefinition
            {
                Path = "~/Content/html5gallery/html5gallery.js",
                LoadSuccessExpression = "window.lightbox"
            });
            ScriptManager.ScriptResourceMapping.AddDefinition("rte", new ScriptResourceDefinition
            {
                Path = "~/richtexteditor/rte.js",
                LoadSuccessExpression = "window.rte"
            });
            ScriptManager.ScriptResourceMapping.AddDefinition("rte2", new ScriptResourceDefinition
            {
                Path = "~/richtexteditor/plugins/all_plugins.js",
                LoadSuccessExpression = "window.rte2"
            });

            // Code that runs on application startup
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            GlobalConfiguration.Configure(WebApiConfig.Register);
            BiblePayCommon.Common.Log2("Starting Service");

            System.Threading.Thread t = new System.Threading.Thread(Service.Executor);
            t.Start();
        }
    }
}