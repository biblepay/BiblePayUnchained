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

                context.Response.ContentType = "text/plain";
                context.Response.Expires = -1;
                try
                {
                    HttpPostedFile postedFile = context.Request.Files[0];
                    string savepath = "";
                    savepath = context.Server.MapPath("Uploads");
                    string filename = Guid.NewGuid().ToString() + ".webm";
                    string sPath = savepath + "\\" + filename;
                    postedFile.SaveAs(sPath);
                    BiblePayCommon.Entity.object1 o = new BiblePayCommon.Entity.object1();
                    o.FileName = sPath;
                    //o.ParentID = sParentID;
                    o.Title = DateTime.Now.ToString() + " Live";
                    o.Subject = "Live Video";
                    o.Body = "Live Video";
                    o.Attachment = 0;
                    // string sCat = Request.Form["ddCategory"].ToNonNullString();
                    o.Category = context.Request.Form["ddCategory"].ToNonNullString2();

                    //o.Category = "Live";
                    User u = Common.gUserSession(context.Session);

                    if (!u.LoggedIn)
                    {
                        Log2("Cannot upload a webm file from live video if not logged in .");
                        context.Response.StatusCode = 500;
                        return;
                    }
                    
                    BiblePayDLL.Sidechain.UploadIntoDSQL_Background(Common.IsTestNetFromSession(context.Session), ref o, Common.gUserSession(context.Session));
                    
                    context.Response.Write(filename);

                    context.Response.StatusCode = 200;
                }
                catch (Exception ex)
                {
                    context.Response.Write("Error: " + ex.Message);
                }

          


            /*
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
            */

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }


    public class Upload : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/plain";
            context.Response.Expires = -1;
            try
            {
                HttpPostedFile postedFile = context.Request.Files["Filedata"];
                string savepath = "";
                string tempPath = "";
                savepath = context.Server.MapPath(tempPath);
                string filename = postedFile.FileName;
                postedFile.SaveAs(savepath + @"\" + filename);
                context.Response.Write(tempPath + "/" + filename);
                context.Response.StatusCode = 200;
            }
            catch (Exception ex)
            {
                context.Response.Write("Error: " + ex.Message);
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