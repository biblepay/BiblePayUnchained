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
using System.Web.Services;
using Newtonsoft.Json;

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