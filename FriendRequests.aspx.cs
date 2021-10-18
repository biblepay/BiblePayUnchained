using System;
using System.Data;
using System.Web;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommon.Entity;
using static BiblePayCommonNET.CommonNET;
using static BiblePayDLL.Sidechain;
using static Unchained.Common;

namespace Unchained
{
    public partial class FriendRequests : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventName == "ApproveFriendRequest_Click")
            {
                BiblePayCommon.Entity.FriendRequest fr = (FriendRequest)GetObject(IsTestNet(this), "FriendRequest", e.EventValue);
                if (e.EventValue == "" || fr == null)
                {
                    UICommon.MsgBox("Error", "Sorry, we cannot find the friend request. ", this);
                    return;
                }
                BiblePayCommon.Entity.Friend f = new Friend();
                f.RequesterID = fr.RequesterID;
                f.UserID = gUser(this).id;
                // delete the friends request; add the friend

                bool fExists = DataExists(IsTestNet(this), "Friend", "userid='" + f.UserID + "' and requesterid='" + f.RequesterID + "'");
                if (fExists && false)
                {
                    UICommon.MsgBox("Error", "Sorry, you are already friends with this person. ", this);
                    return;
                }

                DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), f, gUser(this));
                if (r.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, the new friend was not saved.", 500, 200, true);
                    return;
                }
                else
                {
                    fr.deleted = 1;
                    DataOps.InsertIntoTable(this, IsTestNet(this), fr, gUser(this));
                    BiblePayCommonNET.UICommonNET.ToastLater(this, "Success", "You are now friends!");
                }
            }
        }

        protected string GetFriendRequests()
        {
            string html = "<table class=saved>";
            string sRow = "<tr><th>Requestor Name<th>Avatar<th>Request Date<th>Approve</tr>";
            html += sRow;
            DataTable dtFriends = RetrieveDataTable2(IsTestNet(this), "FriendRequest");
            dtFriends = dtFriends.FilterDataTable("UserID='" + gUser(this).id + "'");

            if (dtFriends.Rows.Count == 0)
            {
                html = "You do not have any friend requests.";
                return html;
            }
            for (int i = 0; i < dtFriends.Rows.Count; i++)
            {
                User requestor = gUserById(this, dtFriends.Rows[i]["RequesterID"].ToString());
                string sApproveButton = UICommon.GetStandardButton(dtFriends.Rows[i]["id"].ToString(), "<i style='color:black;' class='fa fa-heart'></i>", "ApproveFriendRequest", "Approve Friend Request");
                
                string sVURL = "Person?id=" + requestor.id + "'";
                string sUserAnchor = "<a href='" + sVURL + "'>" + requestor.FullUserName() + "</a>";
                string sID = dtFriends.Rows[i]["id"].ToString();

                sRow = "<tr><td>" + sUserAnchor
                        + "<td>" + requestor.GetAvatarImage()
                        + "<td>" + dtFriends.GetColDateTime(i,"time").ToString()
                        + "<td>" + sApproveButton + "</tr>";
                html += sRow;
            }
            html += "</table>";
            return html;
        }
    }
}
