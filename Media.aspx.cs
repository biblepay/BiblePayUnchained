using static Unchained.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;
using static BiblePayCommon.DataTableExtensions;

namespace Unchained
{
    public partial class Media : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "EditVideo_Click")
            {
                Response.Redirect("FormView?action=edit&id=" + e.EventValue + "&table=video1");
            }
            else if (e.EventAction == "ContextMenu_Back")
            {
                Response.Redirect("VideoList");
            }
            
        }


        protected string GetVideo()
        {
            BiblePayVideo.Video video1 = new BiblePayVideo.Video(this);

            string sID = Request.QueryString["id"].ToNonNullString();
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "video1");
            dt=dt.FilterDataTable("id='" + sID + "'");

            if (dt.Rows.Count < 1)
                return String.Empty;
            string html = String.Empty;
            
            video1.URL = dt.GetColValue("URL");
            video1.SVID = dt.GetColValue("SVID");
            video1.Body = dt.GetColValue("Body");
            video1.Title = dt.GetColValue("Title");
            
            video1.Width = 900;
            video1.Height = 600;
            string sFID = dt.GetColValue("FID");

            video1.Footer = "Uploaded by " 
                + UICommon.GetUserAvatarAndName(this, dt.GetColValue("userid"), true)
                + " • " + GetObjectRating(IsTestNet(this), sID) + " • "
                + GetFollowControl(IsTestNet(this), dt.GetColValue("userid"), gUser(this).BiblePayAddress)
                + "<br>" + GetWatchSum(IsTestNet(this), sID) + " view(s) • " 
                + UnixTimeStampToDateTime(dt.GetColDouble(0, "time")).ToString();
            video1.Playable = true;
            if (HasOwnership(IsTestNet(this), sID, "video1", gUser(this).BiblePayAddress))
            {
                string sButton = "<button id='btnEdit' onclick=\""
                    + "__doPostBack('Event_EditVideo_" + "_" + sID + "_', 'EditVideo_Click');\">Edit</button> ";

                video1.Footer += sButton;
                string sContext = UICommon.GetContextMenu(sID, "Edit Video;Back to Video List", 
                    "EditVideo_Click;ContextMenu_Back");
                video1.Footer += sContext;
            }

            html += UICommon.RenderControl(video1);
            html += UICommon.GetComments(IsTestNet(this), sID, this);

            // Increment the count
            UICommon.StoreCount(sID, this, "video");
            return html;
        }
    }
}
