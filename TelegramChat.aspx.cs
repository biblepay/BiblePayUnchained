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

namespace Unchained
{
    public partial class TelegramChat : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventName == "TelegramComment_Click")
            {

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

        protected string MakeURL(string sPath)
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

        protected string UserInitials(TelegramMessage t)
        {
            string UI = Mid(t.UserFirstName, 0, 1)+ Mid(t.UserLastName, 0,1);
            return UI;
        }
        protected string GetAvatar(TelegramMessage t)
        {
            string bioPhoto = MakeURL(t.UserProfilePhoto);
            string sValueControl = bioPhoto.Length > 0 ? "<img src='" + bioPhoto + "' width=100 height=100/>"  : UserInitials(t);
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

        public string DisplayDateTime(long nUnixTime)
        {
            long nElapsed = UnixTimestampUTC() - nUnixTime;
            DateTime theTime = BiblePayCommon.Common.ConvertFromUnixTimestamp(nUnixTime);

            if (nElapsed < (60 * 60 * 24))
            {

                String hourMinute = theTime.ToString("HH:mm");
                return hourMinute;
            }
            else
            {
                string longDate = theTime.ToString();
                return longDate;
            }
        }

        protected string GetTelegram()
        {
            string html = "<h3>TruthBook.Social - Telegram</h3><br><table>";
            long lANRSocialChatID = -1001508518578;

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

                string sValueControl =  t.Text + "";
                string sIntro = "<font color=blue><span>Forwarded from " + t.UserFirstName + " " + t.UserLastName + "</span></font><br>";
                string sWriter = "<font color=green><span>" + t.UserFirstName + " " + t.UserLastName + "</span></font><br><br>";
                string sOutro = "&nbsp;<div style='float:right;'>" + DisplayDateTime(t.Date) + "</div>";

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
                        sWebPage += "<img style='max-width:100%;max-height:500px' src='" + MakeURL(t.WebPagePhotoPath) + "' />";
                        if (t.WebEmbedType != null && t.WebEmbedType != "")
                        {
                            if (t.WebEmbedType == "iframe")
                            {
                                string sIframe = "<br><br><iframe style='border:1px;width:100%;height:500px;' src='" + t.WebEmbedURL + "'></iframe>";
                                sWebPage += sIframe;
                            }
                            else
                            {
                                string s2003 = "";
                            }
                            string s2002 = "";
                        }
                        sWebPage += "</div>";
                        sValueControl += sWebPage;

                        string s2001 = "";
                    }
                }
                else if (t.ContentType == "messageVideo")
                {
                    
                    sValueControl = "<div>" + sIntro + "<video style='height:500px;max-width:100%;' controls><source src='" + MakeURL(t.Path) + "'></source></video>";
                    sValueControl += "<br><span style='text-align:bottom;'>" + t.Text + "</span></div>";

                }
                else if (t.ContentType == "messageChatAddMembers")
                {
                    sValueControl += t.UserFirstName + " " + t.UserLastName + " has been added to the chat.";
                }
                else if (t.ContentType == "messagePhoto")
                {

                    sValueControl = "<a href='" + MakeURL(t.Path) + "' target=_blank><img src='" + MakeURL(t.Path) + "' style='width:100%;'/></a>";
                    sValueControl += "<span>" + sIntro + t.Text + "</span>";

                }
                else if (t.ContentType == "messageSticker")
                {
                    sValueControl = sIntro + "Sticker:<br><img src='" + MakeURL(t.Path) + "' style='height:200px;width:300px;'/>";
                    sValueControl = "Sticker";
                }
                else
                {
                    // Reserved for Telegram unsupported messages
                    string s1099 = "";
                }
                string sData = "<tr><td width='10%'>" + GetAvatar(t) + 
                     "<td width='60%' ><div class='bubble-content telegram-message'>" 
                    + sValueControl + "</div></td>"
                    +"</tr>";
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
