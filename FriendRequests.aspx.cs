using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Web;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommon.Entity;
using static BiblePayCommon.EntityCommon;
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


        private bool IsFriend(Friend f)
        {
            var builder = Builders<dynamic>.Filter;
            var filter = builder.Eq("userid", f.UserID) & builder.Eq("requesterid", f.RequesterID);
            IList<dynamic> l1 = BiblePayDLL.Sidechain.GetChainObjects<dynamic>(IsTestNet(this), "Friend", filter, SERVICE_TYPE.PUBLIC_CHAIN);
            return l1.Count > 0;
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

               
                if (IsFriend(f) && false)
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

            var builder = Builders<BiblePayCommon.Entity.FriendRequest>.Filter;
            var filter = builder.Eq("UserID", gUser(this).id);
            IList<BiblePayCommon.Entity.FriendRequest> dtFriends = BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.FriendRequest>(IsTestNet(this),
                "FriendRequest", filter, SERVICE_TYPE.PUBLIC_CHAIN);

            if (dtFriends.Count == 0)
            {
                html = "You do not have any friend requests.";
                return html;
            }
            for (int i = 0; i < dtFriends.Count; i++)
            {
                User requestor = gUserById(this, dtFriends[i].RequesterID);
                string sApproveButton = UICommon.GetStandardButton(dtFriends[i].id,
                    "<i style='color:black;' class='fa fa-heart'></i>", "ApproveFriendRequest", "Approve Friend Request");
                
                string sVURL = "Person?id=" + requestor.id + "'";
                string sUserAnchor = "<a href='" + sVURL + "'>" + requestor.FullUserName() + "</a>";
                string sID = dtFriends[i].id;

                sRow = "<tr><td>" + sUserAnchor
                        + "<td>" + requestor.GetAvatarImage()
                        + "<td>" + dtFriends[i].time.ToString()
                        + "<td>" + sApproveButton + "</tr>";
                html += sRow;
            }
            html += "</table>";
            return html;
        }
    }
}
