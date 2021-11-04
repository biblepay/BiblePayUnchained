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
using static BiblePayCommon.Common;
using static Unchained.BiblePayUtilities;
using BiblePayCommonNET;
using static BiblePayDLL.Sidechain;
using static BiblePayCommonNET.DataTableExtensions;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommon.Entity;

namespace Unchained
{
    public partial class Friends : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventName == "Unfriend_Click")
            {
                BiblePayCommon.Entity.Friend f = (Friend)GetObject(IsTestNet(this), "Friend", e.EventValue);
                if (e.EventValue == "" || f == null)
                {
                    UICommon.MsgBox("Error", "Sorry, we cannot find the friend. ", this);
                    return;
                }

                f.deleted = 1;

                DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), f, gUser(this));
                if (r.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, we failed to remove your friend.", 500, 200, true);
                    return;
                }
                else
                {
                    DataOps.InsertIntoTable(this, IsTestNet(this), f, gUser(this));
                    BiblePayCommonNET.UICommonNET.ToastLater(this, "Complete", "You are no longer friends.");
                }
            }
        }


        protected string GetFriends()
        {

            string html = "<table class=saved>";
            string sRow = "<tr><th>Name<th>Avatar<th>Since<th>Unfriend</tr>";
            html += sRow;
            DataTable dtFriends = RetrieveDataTable3(IsTestNet(this), "Friend");
            dtFriends = dtFriends.FilterDataTable("UserID='" + gUser(this).id + "' or RequesterID='" + gUser(this).id + "'");

            if (dtFriends.Rows.Count == 0)
            {
                html = "You do not have any friends yet.";
                return html;
            }

            for (int i = 0; i < dtFriends.Rows.Count; i++)
            {
                string sRequestor = dtFriends.Rows[i]["RequesterID"].ToString();
                string sUserID = dtFriends.Rows[i]["UserID"].ToString();
                User Friend = gUser(this).id == sUserID ? gUserById(this, sRequestor) : gUserById(this, sUserID);

                string sUnfriendButton = UICommon.GetStandardButton(dtFriends.Rows[i]["id"].ToString(), "<i class='fa fa-heart-broken'></i>", "Unfriend",
                    "Unfriend this person");
                string sVURL = "Person?id=" + Friend.id;
                string sUserAnchor = "<a href=''>" 
                    + Friend.FullUserName() + "</a>";
                string sID = dtFriends.Rows[i]["id"].ToString();

                sRow = "<tr><td>" + sUserAnchor
                        + "<td>" + Friend.GetAvatarImage()
                        + "<td>" + dtFriends.GetColDateTime(i, "time").ToString()
                        + "<td>" + sUnfriendButton + "</tr>";
                html += sRow;
            }
            html += "</table>";
            return html;
        }
    }
}
