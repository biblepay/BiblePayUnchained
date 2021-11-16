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
            string html = "";

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
                User u = UICommon.GetUserRecord(IsTestNet(this), dtFriends[i].id);

                string sApproveButton = UICommon.GetStandardButton(dtFriends[i].id,
                    "<i style='color:black;' class='fa fa-check'></i> Accept Request", "ApproveFriendRequest", "Approve Friend Request","", "btnacceptfreindreq btn btn-sm p-0");
                string personlink = "<a href = \"Person?id=" + u.id + "\" class=\"tile-link\"></a>";
                string h = "<div class=\"col-md-6 col-xl-4\">"
                + "<div class=\"card\">" +
                "<div class=\"card-body d-flex align-items-center\">" +
                  "<div class=\"flex-shrink-0 align-items-center\"><span style = \"background-image: url(" + u.GetAvatarUrl() + ")\" class=\"avatar avatar-xl mr-3\">" + personlink + "</span></div>"
                    + "<div class=\"flex-grow-1 ms-2 overflow-hidden\">"
                      + "<h6 class=\"card-text mb-0 position-relative\">" + u.FullUserName() + personlink + " </h6> "
                      + "<p class=\"card-text small\"> "
                        + BiblePayCommon.Common.UnixTimeStampToDateControl(dtFriends[i].time)
                      + "</p>"
                      + "<p class=\"card-link text-center border-top mb-0\"> "
                       + sApproveButton
                      + "</p>"
                  + "</div>"
                + "</div>"
              + "</div>"
            + "</div>";
                html += h;

            }
            return html;
        }

        protected string GetFriendRequests1()
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
