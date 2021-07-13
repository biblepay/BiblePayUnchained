using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Web;
using static Unchained.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    /// <summary>
    /// Summary description for Web
    /// </summary>
    public class Web : IHttpHandler
    {

        public static int nPerc = 0;
        public void ProcessRequest(HttpContext context)
        {
            byte[] bytes = context.Request.BinaryRead(context.Request.ContentLength);
            string s = Encoding.UTF8.GetString(bytes, 0, bytes.Length);

            string path = HttpContext.Current.Request.Url.AbsolutePath;
            string[] vID = path.Split("/");
            
            string SessionID = GetElement(vID[vID.Length-1], "=", 1);
            nPerc++;
            context.Response.Write(nPerc.ToString());
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