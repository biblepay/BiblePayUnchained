using System;
using System.Collections.Generic;
using System.Web.Services;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public partial class LP : BBPPage
    {
        // List of Notification Events
        // Upvote Comment for Prayer, Town Hall, Video
        // Upvote Video


        protected void StoreNotification(BiblePayCommon.IBBPObject oParent, string sParentType, string sVotingOn, string sVoteType)
        {
            string sHRObject = String.Empty;
            string sBlurb = "";
            string sAnchor = "";
            string sSourcePage = String.Empty;
            if (sVoteType != "upvote")
                return;
            if (sVotingOn == "video1")
            {
                sHRObject = " your Video";
                BiblePayCommon.Entity.video1 v = (BiblePayCommon.Entity.video1)oParent;
                sBlurb = v.Title;
                sAnchor = "Media?id=" + v.id + "";
            }
            else if (sVotingOn == "comment1")
            {
                BiblePayCommon.Entity.comment1 c = (BiblePayCommon.Entity.comment1)oParent;
                dynamic oGrandParent = GetObject(IsTestNet(this), sParentType, c.ParentID);
                sHRObject = " your Comment";
                if (sParentType == "townhall1")
                {
                    sSourcePage = "PrayerView";
                    sAnchor = "PrayerView?id=" + c.ParentID + "&entity=" + sParentType + "";
                    sBlurb = Mid(oGrandParent.Subject, 0, 100);

                }
                else if (sParentType == "diary1")
                {
                    sSourcePage = "PrayerView";
                    sAnchor = "PrayerView?id=" + c.ParentID + "&entity=" + sParentType + "";
                    sBlurb = Mid(oGrandParent.Subject, 0, 100);

                }
                else if (sParentType == "pray1")
                {
                    sSourcePage = "PrayerView";
                    sAnchor = "PrayerView?id=" + c.ParentID + "&entity=" + sParentType + "";
                    sBlurb = Mid(oGrandParent.Subject, 0, 100);
                }
                else if (sParentType == "video1")
                {
                    sSourcePage = "Media";
                    sAnchor = "Media?id=" + c.ParentID + "";
                    BiblePayCommon.Entity.video1 v1 = (BiblePayCommon.Entity.video1)oGrandParent;
                    sBlurb = Mid(v1.Title, 0, 100);
                }
            }

            UICommon.SendNotification(sVotingOn, sBlurb, this, "liked", sAnchor, oParent.UserID);
        }
        protected bool StoreVote(string parentID, string sVoteType, string sVotingOn, string sParentType)
        {
            if (!gUser(this).LoggedIn)
            {
                return false;
            }
            BiblePayCommon.IBBPObject oParent = GetObjectWithFilter(IsTestNet(this), sVotingOn, "id='" + parentID +"'");
            // If they already voted, use that object
            BiblePayCommon.Entity.vote1 o = (BiblePayCommon.Entity.vote1)GetObjectWithFilter(IsTestNet(this), "vote1", "userid='" + gUser(this).id 
                + "' and parentid='" + parentID + "'");
            StoreNotification(oParent, sParentType, sVotingOn, sVoteType);

            if (o == null || o.id == null)
            {
                o.UserID = gUser(this).id;
                o.ParentID = parentID;
            }
            o.VoteType = sVoteType;
            if (sVoteType == "upvote")
            {
                o.VoteValue = 1;
            }
            else if (sVoteType == "downvote")
            {
                o.VoteValue = -1;
            }
            else
            {
                return false;
            }
            if (o.UserID == "")
                return false;
            BiblePayCommon.Common.DACResult r = BiblePayDLL.Sidechain.InsertIntoDSQL(IsTestNet(this), o, gUser(this), true);
            return true;
        }

        protected bool UpdateFollowStatus(string sFollowedID, bool fFollowing)
        {
            if (!gUser(this).LoggedIn)
                return false;
            // First get the object (for the narrative)
            BiblePayCommon.Entity.follow1 o = (BiblePayCommon.Entity.follow1)GetObjectWithFilter(IsTestNet(this), "follow1", "FollowedID='" + sFollowedID + "' and UserID='" + gUser(this).id + "'");
            if (o == null)
            {
                o = new BiblePayCommon.Entity.follow1();
            }
            o.UserID = gUser(this).id;
            o.FollowedID = sFollowedID;
            o.deleted = fFollowing ? 0 : 1;
            o.Status = fFollowing ? "FOLLOWING" : "NOT FOLLOWING";
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), o, gUser(this));
            return !r.fError();
        }

        public struct AjaxResponse
        {
            public string Error;
            public string Div1;
            public string Div2;
            public string CustomResponse;
            public string CustomResponseValue;
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            string s1 = Request.Headers["headeraction"].ToNonNullString();
            string sID = Common.GetElement(s1, "|", 0);
            string sAct1 = Common.GetElement(s1, "|", 1);
            string sObjectTable = Common.GetElement(s1, "|", 2);
            string sParentType = Common.GetElement(s1, "|", 3);
            AjaxResponse response = new AjaxResponse();

            if (Request.Path.Contains("refreshnotifications"))
            {
                if (sAct1 == "refreshnotifications")
                {
                    string sNC = UICommon.GetNotificationConsole(this);
                    response.Div1 = sNC;
                    string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    Response.Write(sJson);
                    Response.End();
                }
            }
            else if (Request.Path.Contains("read_notification"))
            {

                BiblePayCommon.Entity.Notification n = (BiblePayCommon.Entity.Notification)GetObjectWithFilter(IsTestNet(this), "Notification", "id='" + sID + "'");
                if (n== null)
                {
                    response.Error = "Unable to find notification";
                    string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    Response.Write(sJson);
                    Response.End();
                }
                n.Read = 1;
                DACResult r = BiblePayDLL.Sidechain.InsertIntoDSQL(IsTestNet(this), n, gUser(this), true);


            }
            else if (Request.Path.Contains("voting"))
            {
                if (!gUser(this).LoggedIn)
                {
                    response.Error = "Not logged in.";
                    string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    Response.Write(sJson);
                    Response.End();
                }
                else if (sAct1 == "upvote")
                {
                    bool fResult = StoreVote(sID, sAct1, sObjectTable, sParentType);
                    VoteSums v = GetVoteSum(IsTestNet(this), sID);
                    response.Div1 = v.nUpvotes.ToString();
                    response.Div2 = v.nDownvotes.ToString();
                    string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    Response.Write(sJson);
                    Response.End();
                }
                else if (sAct1 == "downvote")
                {
                    StoreVote(sID, sAct1, sObjectTable, sParentType);
                    VoteSums v = GetVoteSum(IsTestNet(this), sID);
                    response.Div1 = v.nUpvotes.ToString();
                    response.Div2 = v.nDownvotes.ToString();
                    string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                    Response.Write(sJson);
                    Response.End();
                }
                else if (sAct1 == "follow")
                {
                    UpdateFollowStatus(sID, true);
                    string sStatus = UICommon.GetFollowStatus(IsTestNet(this), sID, gUser(this).id);
                    response.Div1 = sStatus;
                    string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);

                    Response.Write(response);
                    Response.End();
                }
                else if (sAct1 == "unfollow")
                {
                    UpdateFollowStatus(sID, false);
                    string sStatus = UICommon.GetFollowStatus(IsTestNet(this), sID, gUser(this).id);
                    response.Div1 = sStatus;
                    string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);

                    Response.Write(sJson);
                    Response.End();
                }
                else if (sID == "pollchat")
                {
                    UICommon.ChatStructure myChat;
                    bool fGot = UICommon.GetChatStruct(gUser(this).id, out myChat);
                    if (fGot)
                    {
                        response.CustomResponse = "ChatResponse";
                        response.CustomResponseValue = myChat.NeedsRefreshed && myChat.LastTyper != gUser(this).id ? "1" : "0";
                        string sJson = Newtonsoft.Json.JsonConvert.SerializeObject(response);
                        if (response.CustomResponseValue == "1")
                        {
                            myChat.NeedsRefreshed = false;
                            UICommon.dictChats2[myChat.chatGuid] = myChat;
                        }
                        Response.Write(sJson);
                        Response.End();
                    }

                }
                else if (sID == "chat")
                {
                    if (gUser(this).LoggedIn == false)
                    {
                        Response.Write("||||");
                        Response.End();
                        return;
                    }

                    UICommon.ChatStructure myChat;
                    bool fGot = UICommon.GetChatStruct(gUser(this).id, out myChat);
                    string sDec = BiblePayCommon.Encryption.Base64DecodeWithFilter(sAct1);
                    if (!fGot)
                    {
                        Response.Write("||||");
                        Response.End();
                    }
                    
                    if (sDec != "")
                    {
                        List<string> lChat = null;
                        fGot = UICommon.dictChatHistory.TryGetValue(myChat.chatGuid, out lChat);
                        if (!fGot)
                        {
                            UICommon.dictChatHistory[myChat.chatGuid] = new List<string>();
                            lChat = UICommon.dictChatHistory[myChat.chatGuid];
                        }
                        lChat.Add(sDec);
                    }
                    String sInnerDiv = UICommon.GetChatInner(this);
                    Response.Write(sInnerDiv + "|chatreply");
                    Response.End();
                }
            }
            string sAction = Request.QueryString["action"].ToNonNullString();
            string sValue = Request.QueryString["value"].ToNonNullString();
            if (sAction == "session")
            {
                string sKey = Request.QueryString["key"].ToNonNullString();
                string sNewPage = Request.QueryString["newpage"].ToNonNullString();
                Session["key_" + sKey] = sValue;
                Session["PageNumber"] = 0;
                var uri = new Uri(this.Request.Url.AbsoluteUri);
                string sURL = uri.GetLeftPart(UriPartial.Path);
                sURL = sURL.Replace("LP", sNewPage);
                Session["pag_" + sURL] = "0";
                Response.Redirect(sNewPage);
            }
        }

        [WebMethod(EnableSession = true)]
        public static void SampleWebMethod(string h)
        {
        }

    }
}