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

        protected bool StoreVote(string parentID, string sVoteType)
        {
            if (!gUser(this).LoggedIn)
            {
                return false;
            }
            // If they already voted, use that object
            BiblePayCommon.Entity.vote1 o = (BiblePayCommon.Entity.vote1)GetObjectWithFilter(IsTestNet(this), "vote1", "userid='" + gUser(this).id 
                + "' and parentid='" + parentID + "'");
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
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), o, gUser(this));
            if (r.fError())
                return false;
            return true;
        }

        protected bool UpdateFollowStatus(string sFollowedID, bool fFollowing)
        {
            if (!gUser(this).LoggedIn)
                return false;
            BiblePayCommon.Entity.follow1 o = new BiblePayCommon.Entity.follow1();
            o.UserID = gUser(this).id;
            o.FollowedID = sFollowedID;
            o.deleted = fFollowing ? 0 : 1;
            o.Status = fFollowing ? "FOLLOWING" : "NOT FOLLOWING";
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), o, gUser(this));
            return !r.fError();
        }

        protected new void Page_Load(object sender, EventArgs e)
        {
            if (Request.Path.Contains("voting"))
            {
                string s1 = Request.Headers["headeraction"].ToNonNullString();
                string sID = Common.GetElement(s1, "|", 0);
                string sAct1 = Common.GetElement(s1, "|", 1);
                
                if (!gUser(this).LoggedIn)
                {
                    Response.Write("notloggedin||||");
                    Response.End();
                }
                else if (sAct1 == "upvote")
                {
                    bool fResult = StoreVote(sID, sAct1);
                    VoteSums v = GetVoteSum(IsTestNet(this), sID);
                    Response.Write(v.nUpvotes.ToString() + "|" + v.nDownvotes.ToString());
                    Response.End();
                }
                else if (sAct1 == "downvote")
                {
                    StoreVote(sID, sAct1);
                    VoteSums v = GetVoteSum(IsTestNet(this), sID);
                    Response.Write(v.nUpvotes.ToString() + "|" + v.nDownvotes.ToString());
                    Response.End();
                }
                else if (sAct1 == "follow")
                {
                    UpdateFollowStatus(sID, true);
                    string sStatus = UICommon.GetFollowStatus(IsTestNet(this), sID, gUser(this).id);
                    Response.Write(sStatus + "|");
                    Response.End();
                }
                else if (sAct1 == "unfollow")
                {
                    UpdateFollowStatus(sID, false);
                    string sStatus = UICommon.GetFollowStatus(IsTestNet(this), sID, gUser(this).id);
                    Response.Write(sStatus + "|");
                    Response.End();
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

                    bool fGot = UICommon.dictChats.TryGetValue(gUser(this).id, out myChat);

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
            else if (Request.Path.Contains("wiki"))
            {
                // The headeraction is populated with the source URL...
                string sSourceURL = Request.Headers["headeraction"].ToNonNullString();
                string sData = BiblePayDLL.Sidechain.DownloadResourceAsString(sSourceURL);
                // Strip out side nav of Forum, and Sidenav of Wiki
                string sExtract = ExtractXML(sData, "<div class=\"navbar navbar-inverse navbar-fixed-top\"", "</div>");
                if (sExtract.Length > 0)
                                    sData = sData.Replace(sExtract, "");
                string sExtract2 = ExtractXML(sData, "<aside class='main-sidebar' id='mySidenav'>", "</aside>");
                if (sExtract2.Length > 0)
                                    sData = sData.Replace(sExtract2, "");

                sExtract2 = ExtractXML(sData, "<head>", "</head>");
                if (sExtract2.Length > 0)
                                    sData = sData.Replace(sExtract2, "");
                sExtract2 = ExtractXML(sData, "<aside", "</aside>");
                if (sExtract2.Length > 0)
                                    sData = sData.Replace(sExtract2, "");
                
                sExtract2 = ExtractXML(sData, "var refTagger", "</script>");
                if (sExtract2.Length > 0)
                                    sData = sData.Replace(sExtract2, "");

                sData = sData.Replace("var refTagger", "");
                sExtract2 = ExtractXML(sData, "<div class=\"printfooter\">", "</html>");
                if (sExtract2.Length > 0)
                   sData = sData.Replace(sExtract2, "");

                Response.Write(sData);
                Response.End();
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