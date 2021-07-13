using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;
using static Unchained.Common;

namespace Unchained
{
    public partial class PrayerView : Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string sSave = Request.Form["btnSaveComment"].ToNonNullString();
            string id = Request.QueryString["id"] ?? "";
            if (sSave != "")
            {
                if (gUser(this).UserName == "")
                {
                    MsgBox("Nick Name must be populated", "Sorry, you must have a username to add a prayer.  Please navigate to Account Settings | Edit to set your UserName.", this);
                    return;
                }

                dynamic o = new System.Dynamic.ExpandoObject();
                o.UserName = gUser(this).UserName;
                string theBody = Request.Form["txtComment"].ToString();
                o.Body = theBody;
                o.ParentID = id;
                string sID = GetSha256Hash(theBody);

                BiblePayDLL.SharedCommon.DACResult r = DataOps.InsertIntoTable(IsTestNet(this), o, "comment1", sID);
                if (r.Error == "")
                {
                    Response.Redirect("PrayerBlog");
                }
                else
                {
                    MsgBox("Error while inserting comment", "Sorry, the comment was not saved: " + r.Error, this);
                }

            }
        }

        public string GetPrayer()
        {
            // Displays the prayer that the user clicked on from the web list.
            string id = Request.QueryString["id"] ?? "";
            if (id == "")
                return "N/A";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "pray1", id, "", "time,subject,username", "", "");

            if (dt.Rows.Count < 1)
            {
                MsgBox("Not Found", "We are unable to find this prayer.", this);
                return "";
            }

            string sUserPic = EmptyAvatar();
            string sUserName = NotNull(dt.Rows[0]["subject"].ToString());
            if (sUserName == "")
                sUserName = "N/A";
            string sBody = " <textarea style='width: 70%;' id=txtbody rows=10 cols=65>" + dt.Rows[0]["body"].ToString() + "</textarea>";

            string div = "<table style='padding:10px;' width=100%><tr><td>User:<td>" + sUserPic + "</tr>"
                + "<tr><td>User Name:<td><h2>" + dt.Rows[0]["UserName"].ToString() + "</h2></tr>"
                + "<tr><td>Added:<td>" + dt.Rows[0]["time"].ToString() + "</td></tr>"
                + "<tr><td>Subject:<td>" + dt.Rows[0]["subject"].ToString() + "</td></tr>"
                + "<tr><td>Body:<td colspan=2>" + sBody + "</td></tr></table>";
            div += UICommon.GetComments(id,this);

            return div;
        }
    }
}