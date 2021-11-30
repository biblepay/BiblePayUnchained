using System;
using System.Collections.Generic;
using System.Web;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.CommonNET;
using System.Data;
using System.Web;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommon.Entity;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;

namespace Unchained
{
    public partial class People : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Event(BBPEvent e)
        {

            //string data1 = Request.Form.ToString();
            if (e.EventAction == "btnTip_Click")
            {
                if (gUser(this).LoggedIn == false)
                {
                    MsgModal(this, "Error", "Sorry, You must log in first.", 400, 200);
                    return;

                }
                User uTip = UICommon.GetUserRecord(IsTestNet(this), e.Extra.TipTo.ToString());
                string sChannelName = uTip.FullUserName();
                if (uTip.BiblePayAddress.ToNonNullString2() == "")
                {
                    MsgModal(this, "Error", "Sorry, this user does not have a biblepay address.", 400, 240, true);
                    return;
                }
                Session["tipto"] = uTip.id.ToString();
                Session["videoid"] = e.Extra.ContentID.ToString();

                UICommon.MsgInput(this, "Tipping_Click", "Tipping", "Enter tip amount for "
                        + sChannelName + ":", 700,
                        "", "", UICommon.InputType.number, false, "","");
            }
            else if (e.EventName == "AddFriendRequest_Click")
            {
                BiblePayCommon.Entity.FriendRequest f = new FriendRequest();
                f.RequesterID = gUser(this).id;
                f.UserID = e.EventValue;
                if (e.EventValue == "" || f.UserID == f.RequesterID)
                {
                    UICommon.MsgBox("Error", "Sorry, you cannot be friends with yourself. ", this);
                    return;
                }
                DACResult r = AmIFriend(this.Page, f.UserID, f.RequesterID);
                if (r.fError())
                {
                    UICommon.MsgBox("Error", r.Error, this);
                    return;
                }
                DACResult r1 = DataOps.InsertIntoTable(this, IsTestNet(this), f, gUser(this));
                if (r1.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, the friend request was not saved.", 500, 200, true);
                    return;
                }
                else
                {
                    ToastLater(this, "Success", "Your Friends Request has been sent!");
                }
            }
            else if (e.EventName == "Tipping_Click")
            {
                string sPub = gUser(this).BiblePayAddress;
		        double nAmt = BiblePayCommon.Common.GetDouble(BiblePayCommon.Encryption.Base64DecodeWithFilter(e.Extra.Output.ToString()));

                string sTipTo = (Session["tipto"] ?? "").ToString();
                User uTip = UICommon.GetUserRecord(IsTestNet(this), sTipTo);
                string sChannelName = uTip.FullUserName();
                if (sPub == String.Empty)
                {
                    MsgModal(this, "Error", "Sorry, you must have a wallet in order to tip.", 400, 200);
                    return;
                }
                if (sTipTo == String.Empty)
                {
                    MsgModal(this, "Error", "Sorry, the user you are trying to tip does not exist.", 400, 200);
                    return;
                }

                string sProductID = e.EventValue;  // Verify if this is the video ID 
                BiblePayCommon.Entity.invoice1 i = new BiblePayCommon.Entity.invoice1();
                i.BillFromAddress = uTip.BiblePayAddress;
                i.BillToAddress = gUser(this).BiblePayAddress;
                i.Amount = nAmt;
                i.Data = "Tipped " + sChannelName + " [Video] for " + nAmt.ToString() + " BBP.";
                i.ProductID = e.EventValue;
                i.ServiceName = sChannelName;
                i.InvoiceType = "Tip";
                i.InvoiceDate = System.DateTime.Now.ToString();
                UICommon.BuySomething(this, i, "Tipped_Click");
            }
            else if (e.EventName == "Tipped_Click")
            {
                string sPin = BiblePayCommon.Encryption.Base64DecodeWithFilter(e.Extra.Output.ToString());

                DACResult r30 = UICommon.BuySomething2(this, sPin);
                if (r30.fError())
                {
                    this.Page.Session["stack"] = Toast("Failure", "The tip failed! " + r30.Error);

                }
                else
                {
                    this.Page.Session["stack"] = Toast("Tipped", "You have tipped this channel on TXID " + r30.Result);
                }
            }
            else if (e.EventName == "ApproveFriendRequest_Click")
            {
                BiblePayCommon.Entity.FriendRequest f = new FriendRequest();
                f.RequesterID = gUser(this).id;
                f.UserID = e.EventValue;
                if (e.EventValue == "" || f.UserID == f.RequesterID)
                {
                    UICommon.MsgBox("Error", "Sorry, you cannot be friends with yourself. ", this);
                    return;
                }
                DACResult r = AmIFriend(this.Page, f.UserID, f.RequesterID);
                if (r.fError())
                {
                    UICommon.MsgBox("Error", r.Error, this);
                    return;
                }
                f.deleted = 1;
                DACResult r1 = DataOps.InsertIntoTable(this, IsTestNet(this), f, gUser(this));
                BiblePayCommon.Entity.Friend f1 = new Friend();
                f1.RequesterID = f.RequesterID;
                f1.UserID = f.UserID;
                r1 = DataOps.InsertIntoTable(this, IsTestNet(this), f1, gUser(this));

                if (r1.fError())
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, the friend could not be added.", 500, 200, true);
                    return;
                }
                else
                {
                    ToastLater(this, "Success", "You are now friends!");
                    Response.Redirect(Request.Url.AbsoluteUri);

                }

            }
            else if (e.EventName == "Unfriend_Click")
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
                    Response.Redirect(Request.Url.AbsoluteUri);
                }
            }
        }


        public static DACResult AmIFriend(Page p, string sFriendUserGuid, string sMyUserGuid)
        {
            DACResult r = new DACResult();

            DataTable dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "FriendRequest");
            string sSnippet1 = "userid='" + sFriendUserGuid + "' and requesterid='" + sMyUserGuid + "'";
            DataTable dt1 = dtOriginal.FilterDataTable(sSnippet1);
            if (sMyUserGuid == sFriendUserGuid)
            {
                r.Result = "Me";
                r.Alt = "Your profile";
                r.Event = "";
                r.TXID = "SELF";
                r.Error = "Sorry, you cannot become a friend with yourself.";
                return r;
            }
            if (dt1.Rows.Count > 0)
            {
                r.Result = "Waiting for their Acceptance";
                r.Alt = "Waiting for them to accept your request.";
                r.Event = "";
                r.TXID = "FRIEND_REQUEST_SENT";
                r.Error = "Sorry, you already have a friend request in to this person.";
                return r;
            }
            string sSnippet2 = "requesterid='" + sFriendUserGuid + "' and userid='" + sMyUserGuid + "'";
            dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "FriendRequest");

            dt1 = dtOriginal.FilterDataTable(sSnippet2);
            if (dt1.Rows.Count > 0)
            {
                r.Result = "Accept friend Request";
                r.Alt = "Accept this person as a friend.";
                r.Event = "AcceptFriendRequest";
                r.TXID = "WAITING_FOR_MY_ACCEPTANCE";
                r.Error = "Sorry, this person already has a friend request in to you.";
                return r;
            }

            dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "Friend");
            dt1 = dtOriginal.FilterDataTable(sSnippet1);
            if (dt1.Rows.Count > 0)
            {
                r.Result = "Friends";
                r.Alt = "You are friends with this person. ";
                r.Event = "";
                r.TXID = "FRIENDS";
                r.Error = "Sorry, this person is already friends with you.";
                return r;
            }
            dtOriginal = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "Friend");

            dt1 = dtOriginal.FilterDataTable(sSnippet2);
            if (dt1.Rows.Count > 0)
            {
                r.Result = "Friends";
                r.TXID = "FRIENDS";
                r.Error = "Sorry, you are already friends with this person.";
                r.Event = "";
                r.TXID = "FRIENDS";
                r.Alt = "You are friends with this person. ";
                return r;
            }
            // By default, DACResult returns true if there is no error:
            r.TXID = "";
            r.Event = "AddFriendRequest";
            r.Result = "Friend Request";
            return r;
        }


        protected string GetPeople()
        {
            BiblePayCommon.BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "user1");
            string sFilter = Config("hideusersdomain");
            if (sFilter != "")
            {
                dt = dt.FilterBBPDataTable(sFilter);
            }

            // Reserved:Filter by Domain:
            if (txtSearch.Text != "")
            {
                dt = dt.FilterBBPDataTable("FirstName + ' ' + LastName like '%" + txtSearch.Text + "%' or FirstName like '%" + txtSearch.Text
                    + "%' or LastName like '%" + txtSearch.Text + "%' or TelegramLinkName like '%"
                    + txtSearch.Text + "' or TelegramLinkURL like '%" + txtSearch.Text + "%' or TelegramLinkDescription like '%" + txtSearch.Text + "%'");
            }
            List<BiblePayCommon.Entity.user1> l = new List<BiblePayCommon.Entity.user1>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BiblePayCommon.Entity.user1 o = (BiblePayCommon.Entity.user1)BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt, "user1", i);
                l.Add(o);
            }
            string html = UICommon.GetUserGallery(this, l, paginator1, 3);
            return html;
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Refine the query
        }

    }
}
