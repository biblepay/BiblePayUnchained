using System;
using System.Collections.Generic;
using System.Data;
using System.Web.UI;
using static BiblePayCommon.Common;
using static Unchained.Common;
using static BiblePayCommonNET.StringExtension;
using System.Linq;
using static BiblePayCommon.EntityCommon;
using static BiblePayCommonNET.CommonNET;
using System.IO;
using System.Web;
using System.Dynamic;
using Newtonsoft.Json;

namespace Unchained
{
    public partial class webhook : BBPPage
    {
        public static string GetRequestBody()
        {
            var bodyStream = new StreamReader(HttpContext.Current.Request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            return bodyText;
        }
        protected new void Page_Load(object sender, EventArgs e)
        {
            /*string headers = Request.Headers["Jesus"].ToNonNullString();
            string[] keys = Request.Form.AllKeys;
            for (int i = 0; i < keys.Length; i++)
            {
                string data = keys[i] + ": " + Request.Form[keys[i]] + "<br>";
                Log("webhook " + data);
            }
            */
            string sData = GetRequestBody();
            if (sData.Length > 0)
            {
                try
                {
                    Log("Webhook body: " + sData);
                    dynamic o = JsonConvert.DeserializeObject<dynamic>(sData);
                    if (o["eventType"].ToString()=="RECORDING_UPLOADED")
                    {
                        string sURL = o["data"]["preAuthenticatedLink"].ToString();
                        string sUserID = o["data"]["initiatorId"].ToString();
                        BiblePayCommon.Common.User u = gUserById(this, sUserID);
                        Log(" User " + u.FullUserName() + " from domain " + u.domain.ToNonNullString()  + " uploaded " + sURL);
                        // Create an mp4 for this user in response to their Jitsi conference.

                        string sFile = Guid.NewGuid().ToString() + ".mp4";
                        string sPath = Path.Combine(Path.GetTempPath(), sFile);
                        bool fDownloaded = false;
                        fDownloaded = BiblePayDLL.Sidechain.DownloadStoredObject(sURL, sPath);
                        if (!fDownloaded)
                        {
                            Log("Utterly failed to download cloud recorded live file " + sURL);
                            return;
                        }

                        BiblePayCommon.Entity.object1 o1 = new BiblePayCommon.Entity.object1();
                        o1.FileName = sPath;
                        o1.Title = DateTime.Now.ToString() + " Live Elite";
                        o1.Subject = o.Title;
                        o1.Body = "Live Conference";
                        o1.Attachment = 0;
                        o1.Category = "Activism";

                        BiblePayDLL.Sidechain.UploadIntoDSQL_Background(false, ref o1, u);

                        Log("Uploaded into decentralized infra.");

                    }
                    else if (o["eventType"].ToString() == "PARTICIPANT_JOINED")
                    {
                        //
                    }
                    else if (o["eventType"].ToString() == "ROOM_CREATED")
                    {
                        //
                    }
                    else
                    {
                        Log("I have no idea what " + sData + " is");
                        Response.Write("404");
                        Response.End();
                        return;
                    }

                    Response.Write("OK");
                    Response.End();
                }catch(Exception ex)
                {
                    Log("Internal Webhook::Error " + ex.Message);
                }
            }

        }

    }
}