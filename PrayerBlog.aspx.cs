using static Unchained.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;
using static Unchained.DataOps;

namespace Unchained
{
    public partial class PrayerBlog : Page
    {
        public int GetOwnerRow(DataTable dt, string id)
        {
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                if (dt.Rows[i]["id"].ToString() == id)
                {
                    return i;
                }
            }
            return -1;
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string sDel = Request.Form["btnDelete"].ToNonNullString();
            int nDeleted = 0;
            if (sDel == "1")
            {

                if (!gUser(this).LoggedIn)
                {
                    MsgBox("Not Logged In", "Sorry, you must be logged in to save a prayer comment.", this);
                    return;
                }
                DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "pray1", "", "", "username,time,id", "", "");

                foreach (string key in Request.Form)
                {
                    if (!key.StartsWith("delete_")) continue;
                    string id = key.Replace("delete_", "");
                    int row1= GetOwnerRow(dt, id);
                    string owner = dt.Rows[row1]["UserName"].ToString();
                    if (owner == gUser(this).UserName && Request.Form[key]=="on")
                    {
                        // Delete?
                        dynamic o = new System.Dynamic.ExpandoObject();
                        o.Subject = dt.Rows[row1]["subject"];
                        o.Body = "";
                        o.UserName = gUser(this).UserName.ToString();
                        string sID = GetSha256Hash(dt.Rows[row1]["subject"].ToString());

                        BiblePayDLL.SharedCommon.DACResult r = DataOps.InsertIntoTable(IsTestNet(this), o, "pray1", sID);
                        nDeleted++;
                    }
                }
                MsgBox("Success", "Successfully deleted " + nDeleted.ToString() + " records.", this);
            }
        }


        protected string GetPrayerBlogs()
        {
            // Harvest To Do:  Add Order by, PrayerRequest.Added desc (and inner join equiv for avatar display)
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "pray1", "", "", "username,time,id,body", "", "");
            string html = "<table class=saved><tr><th width=20%>User</th><th width=20%>Added<th width=50%>Subject<th>Delete";
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string sAnchor = "<a href='PrayerView.aspx?id=" + dt.Rows[y]["id"].ToString() + "'>";
                string sDele = "<input type='checkbox' name='delete_" + dt.Rows[y]["id"].ToString() + "'/> ";
                string sBody = dt.Rows[y]["body"].ToString();
                if (sBody != "")
                {
                    string div = "<tr><td>" + EmptyAvatar() + "&nbsp;"
                        + dt.Rows[y]["UserName"].ToString() + "</td>" + UICommon.GetTd(dt.Rows[y], "Time", sAnchor)
                        + UICommon.GetTd(dt.Rows[y], "subject", sAnchor) + "<td>" + sDele + "</tr>";
                    html += div + "\r\n";
                }
            }
            html += "</table><button id='btnDelete' name='btnDelete' value='1'>Delete Records</button>";
            return html;
        }
    }
}