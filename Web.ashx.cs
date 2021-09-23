using System;
using System.Web;
using static BiblePayCommon.Common;

namespace Unchained
{
    /// <summary>
    /// Summary description for Web
    /// </summary>
    public class Web : IHttpHandler, System.Web.SessionState.IReadOnlySessionState
    {

        public static int nPerc = 0;

       
        public void ProcessRequest(HttpContext context)
        {

            if (context.Session != null && context.Session["ETA"] != null)
            {
                double nETA = GetDouble(context.Session["ETA"]);
                nETA += UnixTimestampUTC() / 1000000000;
                string sETA = Math.Round(nETA, 2).ToString();
                if (context.Session["user"] != null)
                {
                    User u = (User)context.Session["user"];
                    Log2("Uploading " + u.FullUserName() + " " + sETA);
                }
                context.Response.Write(sETA);

            }
            else
            {
                context.Response.Write("50");
                Log2("Session empty...");
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}