using static Unchained.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;

namespace Unchained
{
    public partial class Media : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        private string GetInnerVideo(string sID, string sURL)
        {
            bool fPlayable = true;
            int DimsX = 1000;
            int DimsY = 700;
            string sDims = "width='" + DimsX.ToString() + "' height='" + DimsY.ToString() + "'";
            string sAutoPlay = fPlayable ? "autostart autoplay controls playsinline" : "preload='metadata'";
            string sHTML = "<video id='vid" + sID + "' class='connect-bg' " + sDims + " " + sAutoPlay + " style='background-color:black'>";
            string sLoc = !fPlayable ? "#t=7" : "#t=7";
            sHTML += "<source src='" + sURL + sLoc + "' type='video/mp4'></video>";
            return sHTML;
        }
        private string CurateVideo(string sID, string sNickName, string sURL, string sAdded, string sSubject, string sNotes)
        {
            string sDiv = "<div style='height: 1000px; width:900px; overflow: hidden; '>";
            sDiv += GetInnerVideo(sID, sURL);
            string sSpeed1 = "<a id='aSlow' href='#' onclick='slowPlaySpeed();'>.5x</a>";
            string sSpeed2 = "<a id='aNormal' href='#' onclick='normalPlaySpeed();'>1x</a>";
            string sSpeed3 = "<a id='aFast' href='#' onclick='fastPlaySpeed();'>1.75x</a>";
            string sFooter = sSpeed1 + " • " + sSpeed2 + " • " + sSpeed3 + " • ? view(s) • " + sAdded;
            sDiv += "<br>" + sSubject +  " • Uploaded by " + sNickName + "<br>" + sFooter + "<br></div>";
            return sDiv;
        }

        protected string GetVideo()
        {
            string id = Request.QueryString["mediaid"].ToNonNullString();
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1","", "", "username,time,id,body,URL,subject", "", "");
            string html = "";
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string sURL = dt.Rows[y]["URL"].ToNonNullString();
                string sID = dt.Rows[y]["id"].ToNonNullString();
                if (sID == id)
                { 
                    string sVideo = CurateVideo(dt.Rows[y]["id"].ToNonNullString(), dt.Rows[y]["username"].ToNonNullString(), dt.Rows[y]["URL"].ToNonNullString(),
                        dt.Rows[y]["time"].ToNonNullString(), dt.Rows[y]["Subject"].ToNonNullString(), dt.Rows[y]["Body"].ToNonNullString());
                    html += sVideo;
                }
            }
            return html;
        }
    }
}
