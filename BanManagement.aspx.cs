using BiblePayCommonNET;
using System;
using System.Data;
using System.Linq;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.CommonNET;
using static Unchained.Common;

namespace Unchained
{
    public partial class BanManagement : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            if (gUser(this).Administrator != 1)
            {
                UICommon.MsgBox("Error", "You are not authorized", this);
            }
        }
        public static string ConstructURL(Page p, string sParentType, string sID)
        {
            // Takes you to a part of the system
            dynamic o1 = Common.GetObject(Common.IsTestNet(p), sParentType, sID);
            string sURL = "";
            if (sParentType.ToLower() == "video1")
            {
                sURL = "Media?id=" + sID;
            }
            else if (sParentType.ToLower() == "timeline")
            {
                sURL = "Person?post=" + sID;
            }
            else
            {
                // We need to build all of them out...
            }
            string sFullURL = "<a target='_blank' href='" + sURL + "'>" + sParentType + "</a>";
            return sFullURL;
        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventName == "RemoveContent_Click")
            {
                if (!Common.gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "You Must be logged in first.", this);
                    return;
                }

                dynamic f = Common.GetObject(Common.IsTestNet(this), "FlaggedContent", _bbpevent.EventValue);
                if (f == null)
                {
                    UICommon.MsgBox("Error", "No such flagged content.", this);
                }

                
                dynamic oOriginalObject = Common.GetObject(Common.IsTestNet(this), f.ParentType, f.ParentID);

                if (oOriginalObject == null)
                {
                    UICommon.MsgBox("Error", "Unable to locate the parent object.", this.Page);
                    return;
                }
                f.Banned = UnixTimestampUTC();
                f.Reviewed = UnixTimestampUTC();
                f.BannedBy = gUser(this).id.ToString();

                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), f, Common.gUser(this));
                if (!r.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this.Page, "Success", "Your item has been banned.", 400, 200);
                }
                else
                {
                    UICommon.MsgBox("Error", "Sorry, the item could not be banned. ", this);
                }
                // to do : update the actual item with the ban content flag?
            }
            else if (e.EventName == "AllowContent_Click")
            {
                if (!Common.gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "You Must be logged in first.", this);
                    return;
                }

                dynamic f = Common.GetObject(Common.IsTestNet(this), "FlaggedContent", _bbpevent.EventValue);
                if (f == null)
                {
                    UICommon.MsgBox("Error", "No such flagged content.", this);
                }
                dynamic oOriginalObject = Common.GetObject(Common.IsTestNet(this), f.ParentType, f.ParentID);

                if (oOriginalObject == null)
                {
                    UICommon.MsgBox("Error", "Unable to locate the parent object.", this.Page);
                    return;
                }
                f.Reviewed = UnixTimestampUTC();
                f.ReviewedBy = gUser(this).id.ToString();

                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), f, Common.gUser(this));
                if (!r.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this.Page, "Success", "Your item has been reviewed.", 400, 200);
                }
                else
                {
                    UICommon.MsgBox("Error", "Sorry, the item could not be updated as reviewed. ", this);
                }
            }
        }

        protected string GetBanReport()
        {
            string html = "<table class=saved>";
            // Column headers
            string sRow = "<tr><th>Added<th>Type<th>ID<th>Original User<th>Reported By User<th>URL<th>Action</tr>";
            html += sRow;
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "FlaggedContent",true,true);
            // They belong to you and have not been successfully added to a link yet ... At the end, use the final URL.
            long nEarlyTime = UnixTimestampUTC() - (60 * 60 * 24 * 180);
            dt = dt.FilterDataTable("time > " + nEarlyTime.ToString() + " and isnull(Reviewed,0)=0");
            dt = dt.OrderBy("time desc");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //string sURL = dt.GetColValue(i, "FinalURL");
                string sLink = ConstructURL(this, dt.Rows[i]["ParentType"].ToString(), dt.Rows[i]["ParentID"].ToString());
                string sRemoveContent = UICommon.GetStandardButton(dt.Rows[i]["id"].ToString(), "Remove", "RemoveContent", "Remove this Content");
                string sAllowContent = UICommon.GetStandardButton(dt.Rows[i]["id"].ToString(), "Allow", "AllowContent", "Allow this Content");

                sRow = "<tr><td>" + GetPrettyDate(dt.GetColDouble(i, "time")) 
                    + "</td><td>" + dt.Rows[i]["ParentType"].ToString()
                    + "</td><td>" + dt.Rows[i]["id"].ToString()
                    + "</td><td>" + dt.GetColValue(i, "OriginalUserID")
                    + "</td><td>" + dt.GetColValue(i, "UserID")
                    + "</td><td>" + sLink 
                    + "</td><td>" + sRemoveContent + sAllowContent + "</td></tr>";
                html += sRow;
            }
            html += "</table>";
            return html;
        }

    }
}
