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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommon.Entity;

namespace Unchained
{
    public partial class TelegramChat : BBPPage
    {
        public static long lANRSocialChatID = -1001508518578;

        protected new void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventName == "TelegramComment_Click")
            {

            }
            else if (_bbpevent.EventName == "TelegramShare_Click")
            {

                if (!Common.gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "You Must be logged in first.", this);
                    return;
                }

                Timeline t = new Timeline();
                t.Privacy = "Public";
                string sTM = RenderTelegramMessage((long)GetDouble(_bbpevent.EventValue));
                if (sTM != null)
                {
                    string sAnchor = "<a href='TelegramChat'><font color=green>Forwarded from Telegram Chat</font></a><br>";

                    t.Body = sAnchor + sTM;
                    t.UserID = gUser(this).id;
                    BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), t, gUser(this));
                    if (r.fError())
                    {
                        BiblePayCommonNET.UICommonNET.MsgModal(this, "Error", "Sorry, the timeline was not saved.", 500, 200, true);
                        return;
                    }
                    else
                    {
                        UICommonNET.ToastNow(this, "Success", "Your timeline entry has been saved!");
                    }
                }

            }
        }
        public static List<TelegramMessage> GetArchivedTelegramMessages(long lChatID)
        {
            List<TelegramMessage> ltm2 = new List<TelegramMessage>();
          
            string sDbPath = "c:\\inetpub\\wwwroot\\Unchained\\Telegram\\";

            string[] filePaths = Directory.GetFiles(sDbPath);

            for (int i = 0; i < filePaths.Length; i++)
            {
                FileInfo fi = new FileInfo(filePaths[i]);

                string[] vEntry = fi.Name.Split(new string[] { "." }, StringSplitOptions.None);

                if (vEntry.Length >= 1)
                {
                    double nChatID = GetDouble(vEntry[0]);
                    if (lChatID == nChatID)
                    {
                            using (var file = File.OpenRead(filePaths[i]))
                            {
                                var reader = new BinaryFormatter();
                                ltm2 = (List<TelegramMessage>)reader.Deserialize(file); // Reads the entire list.
                            }
                            return ltm2;
                    }
                }
            }
            return ltm2;
        }

        public static int ChooseSmallestLength(List<string> s)
        {
            int nPointer = -1;
            int nSmallest = 99999;
            for (int i = 0; i < s.Count; i++)
            {
                int nLen = s[i].Length;
                if (nLen > 0 && nLen < nSmallest)
                {
                    nPointer = i;
                }

            }
            return nPointer;
        }

        public static TelegramMessage GetTelegramMessageById(long nMsgID)
        {
            List<BiblePayCommon.Common.TelegramMessage> lT = GetArchivedTelegramMessages(lANRSocialChatID);
            for (int i = 0; i < lT.Count; i++)
            {
                if (lT[i].MessageID == nMsgID)
                    return lT[i];
            }
            return new TelegramMessage();
        }
        public static string RenderTelegramMessage(long nMsgID)
        {
            TelegramMessage t = GetTelegramMessageById(nMsgID);
            if (t.MessageID == null || t.MessageID == 0)
                return null;
            string sOut = RenderTelegramMessage(t);
            return sOut;
        }
        public static string ReplaceTelegramLinks(string s)
        {
            // Replace any non https t.me with t.me
            s = s.Replace(" t.me", " https://t.me");
            string sOut = "";
            for (int i = 0; i < s.Length; i++)
            {
                string sSearchPhrase = "https://t.me";
                string sMid = Mid(s, i, sSearchPhrase.Length);
                string sOld = Mid(s, i - 1, 1);
                string sLeftOver = Mid(s, i,  256);

                if (sMid==sSearchPhrase  && sOld != "'" && sOld != "\"")
                {
                    // this needs replaced
                    string sExtract = "";
                    List<string> s1 = new List<string>();
                    s1.Add(ExtractXML(sLeftOver, sSearchPhrase, "&nbsp;"));
                    s1.Add(ExtractXML(sLeftOver, sSearchPhrase, " "));
                    s1.Add(ExtractXML(sLeftOver, sSearchPhrase, "\n"));
                    int nPos = ChooseSmallestLength(s1);
                    if (nPos > -1)
                    {
                        sExtract = s1[nPos];
                    }

                    if (sExtract =="")
                    {
                        sExtract = ExtractXML(sLeftOver, sSearchPhrase, "<");
                    }
                    if (sExtract != "")
                    {
                        string sOrigLink = sSearchPhrase + sExtract;
                        string sLink = "<a href='" + sOrigLink + "'>" + sOrigLink + "</a>";
                        i = i + sExtract.Length + sSearchPhrase.Length - 1;
                        sOut += sLink;
                    }
                    else
                    {
                        sOut += Mid(s, i, 1);
                    }
                }
                else
                {
                    sOut += Mid(s, i, 1);
                }
            }
            return sOut;
        }
        public static string RenderTelegramMessage(TelegramMessage t)
        {
            string sValueControl = t.Text + "";
            string sIntro = "<font color=blue><span>Forwarded from " + t.UserFirstName + " " + t.UserLastName + "</span></font><br>";
            string sWriter = "<font color=green><span>" + t.UserFirstName + " " + t.UserLastName + "</span></font><br><br>";
            string sOutro = "&nbsp;<div style='float:right;'>" + BiblePayCommon.Common.DisplayDateTime(t.Date) + "</div>";
            string sSerial = "msgid:" + t.MessageID + ",chatid:" + t.ChatID + ", UserID:" + t.UserID + ", UserName: " + t.UserFirstName + " " + t.UserLastName;
            if (t.ContentType == "messageText")
            {
                sValueControl = "<div>" + sWriter + t.Text + sOutro + "</div>";
                if (t.WebPagePhotoPath != null)
                {
                    string sWPD = t.WebPageDescription;
                    if (sWPD.Length > 500)
                    {
                        sWPD = Mid(sWPD, 0, 500) + " ...";
                    }
                    string sWebPage = "<div>" + sIntro + "<b>" + t.WebPageTitle + "</b><br>" + sWPD + "<br>";
                    sWebPage += "<span>" + t.WebPageDisplayURL + "</span>";
                    sWebPage += "<img style='max-width:100%;max-height:500px' src='" + MakeTelegramURL(t.WebPagePhotoPath) + "' />";
                    if (t.WebEmbedType != null && t.WebEmbedType != "")
                    {
                        if (t.WebEmbedType == "iframe")
                        {
                            string sIframe = "<br><br><iframe style='border:1px;width:100%;height:500px;' src='" + t.WebEmbedURL + "'></iframe>";
                            sWebPage += sIframe;
                        }
                        else
                        {
                        }
                    }
                    sWebPage += "</div>";
                    sValueControl += sWebPage;
                }

                // Reserved to replace telegram links here...

            }
            else if (t.ContentType == "messageVideo")
            {
                sValueControl = "<div>" + sIntro + "<video style='height:500px;max-width:100%;' controls><source src='" + MakeTelegramURL(t.Path) + "'></source></video>";
                sValueControl += "<br><span style='text-align:bottom;'>" + t.Text + "</span></div>";
            }
            else if (t.ContentType == "messageChatAddMembers")
            {
                sValueControl += t.UserFirstName + " " + t.UserLastName + " has been added to the chat.";
            }
            else if (t.ContentType == "messagePhoto")
            {

                sValueControl = "<a href='" + MakeTelegramURL(t.Path) + "' target=_blank><img src='" + MakeTelegramURL(t.Path) + "' style='width:100%;'/></a>";
                sValueControl += "<span>" + sIntro + t.Text + "</span>";

                sValueControl = ReplaceTelegramLinks(sValueControl);
            }
            else if (t.ContentType == "messageSticker")
            {
                sValueControl = sIntro + "Sticker:<br><img src='" + MakeTelegramURL(t.Path) + "' style='height:200px;width:300px;'/>";
                sValueControl = "Sticker";
            }
            else
            {
                // Reserved for Telegram unsupported messages
            }
            if (sValueControl.Contains("https://t.me"))
            {
                string s9099 = "";
            }
            // Replace any t.me links:
            sValueControl = ReplaceTelegramLinks(sValueControl);
            return sValueControl;
        }

        public static string MakeTelegramURL(string sPath)
        {
            if (sPath == null)
            {
                return "";
            }
            string sPath2 = sPath.Replace("c:\\inetpub\\wwwroot\\Unchained\\", "");
            sPath2 = sPath2.Replace("\\", "/");
            string sURL = "https://anrsocial.com/" + sPath2;
            return sURL;
        }

        public static string TelegramUserInitials(TelegramMessage t)
        {
            string UI = Mid(t.UserFirstName, 0, 1)+ Mid(t.UserLastName, 0,1);
            return UI;
        }
        public static string GetTelegramAvatar(TelegramMessage t)
        {
            string bioPhoto = MakeTelegramURL(t.UserProfilePhoto);
            string sValueControl = bioPhoto.Length > 0 ? "<img src='" + bioPhoto + "' width=100 height=100/>"  : TelegramUserInitials(t);
            if (sValueControl.Length == 1)
                sValueControl += ".";

            string s = "";
            if (bioPhoto.Length > 0)
            {
                s = "<avatar-element><img style=\"background-image:url('" + bioPhoto + "');\" class='avatar-normal'/></avatar-element>";
            }
            else
            {
                s = "<avatar-element class='avatar-element' data-color='cyan'>" + sValueControl + "</avatar-element>";
            }
            //if (bioPhoto.Length > 0)
            //  s += "<br>" + UserInitials(t);
            return s;
                
        }

        protected string GetTelegram()
        {
            string html = "<h3>TruthBook.Social - Telegram</h3><br><table>";
            
            List<BiblePayCommon.Common.TelegramMessage> lT = GetArchivedTelegramMessages(lANRSocialChatID);
            // Last :
            double nPag = GetDouble(Request.QueryString["pag"] ?? "");
            double nPriorPosts = 31 + nPag;
            if (nPriorPosts > 512)
                nPriorPosts = 512;
            if (nPriorPosts > lT.Count-1)
                nPriorPosts = lT.Count - 1;
            for (int i = (int)nPriorPosts; i >= 0; i--)
            {
                BiblePayCommon.Common.TelegramMessage t = lT[i];

                string sValueControl = RenderTelegramMessage(t);

                string sShare = "<svg xmlns='http://www.w3.org/2000/svg' width='24' height='24' viewBox='0 0 24 24' fill='none' stroke='currentColor' stroke-width='2' stroke-linecap='round' stroke-linejoin='round' class='feather feather-share icon-md'>"
                + "<path d='M4 12v8a2 2 0 0 0 2 2h12a2 2 0 0 0 2-2v-8'></path><polyline points='16 6 12 2 8 6'></polyline><line x1= '12' y1='2' x2='12' y2='15'></line>  </svg>"
                + "<!--<p class='d-none d-md-block ms-2 mb-0'>Share</p>-->";
                string sShareAnchor = UICommon.GetStandardAnchor("tShare1", "TelegramShare", t.MessageID.ToString(), sShare, "Share on my Timeline", "Timeline", "", "");

                string sData = "<tr><td width='10%'>" + GetTelegramAvatar(t) +
                    "<td width='70%' ><div class='bubble-content telegram-message'>"
                   + sValueControl + "</div></td>";
                if (gUser(this).LoggedIn)
                {
                    sData += "<td width='5%' style='text-align:left;'>" + sShareAnchor + "</td>";
                }
                sData += "</tr>";

                html += sData;
            }
            html += "</table>";
            // Post to chat room
            string sCommentButton = "<input class='pc90' autocomplete='off' id='telegram1'></input><button id='btnTelegram1' onclick=\""
                + "var o=document.getElementById('telegram1');var e={};e.Event='TelegramComment_Click';e.Value='"
                + "';e.Body=XSS(o.value);send_out(o.value);\">Say Something, "
                + gUser(this).FirstName + "</button> ";

            if (Common.gUser(this).LoggedIn && false)
            {
                html += "<br>" + sCommentButton;
            }

            return html;
        }
    }
}
