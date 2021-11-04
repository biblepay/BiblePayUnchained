using BiblePayCommon;
using BiblePayCommonNET;
using MongoDB.Bson;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommon.EntityCommon;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public static class UICommon
    {

        public static string GetCurrentThemeName(Page p)
        {
            string sDomainDefault = Config("defaulttheme");
            if (sDomainDefault != "")
            {
                return sDomainDefault;
            }
            string sTheme = gUser(p).ThemeName;
            if (sTheme == null || sTheme == "")
                sTheme = "black";
            return sTheme;
        }

        public static bool ReskinCSS(string sPrimaryColor, string sSecondaryColor, string sPrimaryDarkerColor, string sNewName, string sPrimaryTextColor, string sBorderColor, string sNavBarBGColor, string sLinkVisitedColor)
        {
            // this reskins our left vertical menu and our top header bg-color.
            string sPath = System.Web.HttpContext.Current.Server.MapPath("Content\\sidenav.css");
            string sData = System.IO.File.ReadAllText(sPath);
            sData = sData.Replace("black_black", sPrimaryColor);
            sData = sData.Replace("maroon_maroon", sSecondaryColor);
            sData = sData.Replace("grey_grey", sPrimaryColor); //this is the user avatar background only
            sData = sData.Replace("primary_text_color", sPrimaryTextColor);
            sData = sData.Replace("primary_darker_color", sPrimaryDarkerColor);
            sData = sData.Replace("navbar_background_color", sNavBarBGColor);
            sData = sData.Replace("link_visited_color", sLinkVisitedColor);
            sData = sData.Replace("border_color", sBorderColor);
            string sOutPath = System.Web.HttpContext.Current.Server.MapPath("Content\\sidenav_" + sNewName + ".css");
            System.IO.File.WriteAllText(sOutPath, sData);
            return true;
        }

        private static bool InList(string[] vList, string sTarget)
        {
            for (int i = 0; i < vList.Length; i++)
            {
                if (vList[i].ToLower() == sTarget.ToLower())
                    return true;
            }
            return false;
        }

        private static string AddMenuOptions(bool fLoggedIn)
        {
            string sMenu = "";
            for (int i = 1; i < 99; i++)
            {
                string sMenuName = Config("menu" + i.ToString() + "name");
                string sMenuPaths = Config("menu" + i.ToString() + "paths");
                string sMenuPages = Config("menu" + i.ToString() + "pages");
                string sMenuIcon = Config("menu" + i.ToString() + "icon");
                if (sMenuName != "")
                {
                    sMenu += AddMenuOption(fLoggedIn, sMenuName, sMenuPaths, sMenuPages, sMenuIcon);
                }
                else
                {
                    break;
                }
            }
            return sMenu;
        }
        private static int item = 0;
        private static string AddMenuOption(bool fLoggedIn, string MenuName, string URLs, string LinkNames, string sIcon)
        {
            string sHWLO = Config("HideWhenLoggedOut");
            string[] vHWLO = sHWLO.Split(";");

            double nEnabled = GetDouble(Config(MenuName));
            if (nEnabled == -1)
                return "";
            int nAdded = 0;

            string[] vURLs = URLs.Split(";");
            string[] vLinkNames = LinkNames.Split(";");

            var js2 = "   var xp = parseFloat(localStorage.getItem('bbpdd" + item.ToString() + "')); "
             + "   var xe = xp==0?1:0; localStorage.setItem('bbpdd" + item.ToString() + "', xe); var disp = xp == 0 ? 'none' : 'block';";

            var js3 = "   var xp = parseFloat(localStorage.getItem('bbpdd" + item.ToString() + "')); "
             + "   var xe = xp==0?1:0; var disp = xe == 0 ? 'none' : 'block';";

            string menu = "<li id ='button_" + MenuName + "' class='dropdown'>"
             + "	<a class='dropdown-toggle' href='#' data-toggle='dropdown' onclick=\" myfunc(" + item.ToString() + "); "
             + js2 + " $('#bbpdd" + item.ToString() + "').attr('expanded', xe); "
             + "    $('#bbpdd" + item.ToString() + "').css('display',disp);\" >"
             + "	<i class='fa " + sIcon + "'></i>&nbsp;<span>" + MenuName + "</span>"
             + "	<span class='pull-right-container'><i class='fa fa-angle-left pull-right'></i></span></a>"
             + "	<ul class='treeview-menu' id='bbpdd" + item.ToString() + "'><script>" + js3 + "$('#bbpdd" + item.ToString() + "').css('display',disp);</script>";

            for (int i = 0; i < vLinkNames.Length; i++)
            {
                bool fPost = vLinkNames[i].Contains("[1]");
                if (!fPost)
                {
                    bool fMasked = InList(vHWLO, vLinkNames[i]) && !fLoggedIn;
                    if (!fMasked)
                    {
                        menu += "<li><a href='" + vURLs[i] + "'><span class='contextsensitive'>" + vLinkNames[i] + "</span></a></li>";
                    }
                    nAdded++;
                }
                else
                {
                    string sLinkName = vLinkNames[i];
                    sLinkName = sLinkName.Replace("[1]", "");
                    string sPostAnchor = "<li><a href='javascript: void();' onclick='document.forms[0].action=\"" + vURLs[i]
                        + "\"; document.forms[0].submit(); '>"
                        + "<span class='contextsensitive' >" + sLinkName + "</span></a></li>";
                    menu += sPostAnchor;
                }
            }
            menu += "</ul></li>";
            item++;
            if (nAdded == 0)
                return "";
            else
                return menu;
        }

        public enum InputType
        {
            password,
            text,
            number,
            multiline
        };

        public static void MsgInput(Page p, string sCallBackEvent, string sTitle, string sNarrative,
                     int nWidth, string sAddress, string sAmt, InputType eInputType, bool fInputHidden, string sEventValue = "", string sInitialTextValue = "")
        {
            string sType = eInputType.ToString();
            if (fInputHidden)
                sType = "hidden";
            string sInputControl = "";
            string sGrabber = "";
            sInitialTextValue = sInitialTextValue.Replace("\n", "\\n");
            sInitialTextValue = sInitialTextValue.Replace("'", "\\'");

            if (eInputType == InputType.multiline)
            {
                sInputControl = "<textarea autocomplete=\"nope\" id=\"q1\" class=\"comments\" name=\"q1\" rows=\"6\" cols=\"10\">" + sInitialTextValue + "</textarea>";
                sGrabber = "var v=$(\"textarea#q1\").val(); ";
            }
            else
            {
                sInputControl = "<input autocomplete=\"nope\" type=\"" + sType + "\" class=\"trump\" id=\"q1\" name=\"q1\" value=\"" + sInitialTextValue + "\"/>";
                sGrabber = "var v=$(\"#q1\").val();";
            }
            string sClear = "var o = document.getElementById('q1'); o.value='';";
            //this used to say b t o a(escape(v)); test this 

            string sJavascript = "$('<form id=\"wform1\" method=\"POST\">" + sNarrative
                + "<br>" + sInputControl + "<br></form>').dialog({"
                + "  modal: true, width: " + nWidth.ToString() + ", title: '" + sTitle + "', buttons: {    'OK': function() {"
                + "  " + sGrabber
                + "  var Extra={};Extra.Value='" + sEventValue + "';Extra.Address='" + sAddress + "';Extra.Amount='" + sAmt + "';"
                + "Extra.Output=XSS(v);Extra.Event='" + sCallBackEvent + "';" + sClear + "BBPPostBack2(this, Extra);"
                + "$(this).dialog('close');"
                + " },     'Cancel': function() {       $(this).dialog('close');                 }            }        });";
            //p.ClientScript.RegisterStartupScript(p.GetType(), "msginput1", sJavascript, true);
            ScriptManager.RegisterStartupScript(p,p.GetType(), "msginput1", sJavascript, true);
        }

        public static void GenerateJsTag(Page page, string jsCode)
        {
            var jsLink = new System.Web.UI.HtmlControls.HtmlGenericControl { TagName = "script", InnerHtml = jsCode };
            jsLink.Attributes.Add("type", "text/javascript");
            page.Controls.Add(jsLink);
        }


        public static string RenderGauge(int width, string Name, int value)
        {
            // This is a google gauge control (it looks like a PSI gauge on an A/C charge tank); we don't use this but it looks cool; may be good for an ROI figure.
            string s = "<div id='chart_div'></div><script type=text/javascript> google.load( *visualization*, *1*, {packages:[*gauge*]});"
                + "     google.setOnLoadCallback(drawChart);"
                + "     function drawChart() {"
                + "     var data = new google.visualization.DataTable();"
                + "     data.addColumn('string', 'item');"
                + "     data.addColumn('number', 'value');     "
                + "     data.addRows(1);";
            s += "data.setValue(0,0,'" + Name + "');";
            s += "data.setValue(0,1," + value.ToString() + ");";
            s += "var options = {width: " + width.ToString() + ", height: " + width.ToString() + ",redFrom: 90, redTo: 100,yellowFrom:75, yellowTo: 90,minorTicks: 5};";
            s += " var chart = new google.visualization.Gauge(document.getElementById('chart_div'));";
            s += "chart.draw(data, options); }";
            s += "</script>";
            s = s.TrimEnd(',').Replace('*', '"');
            return s;
        }

        public static void RunScript(Page p, string sJavascript)
        {
            p.ClientScript.RegisterStartupScript(p.GetType(), "script" + Guid.NewGuid().ToString(), sJavascript, true);
        }

        public static void RunScriptSM(Page p, string sJavascript)
        {
            ScriptManager.RegisterStartupScript(p, p.GetType(), "script" + Guid.NewGuid().ToString(), sJavascript, true);
        }

        public static string Toast(string sTitle, string sBody)
        {
            string sJavascript = "$.toast({  heading: '" + sTitle + "',"
                + "  text: '" + sBody + "',"
                + "   icon: 'info',"
                + "   loader: true,"
                + "   loaderBg: '#9EC600' });";
            return sJavascript;
        }

        public static string GetFooter(Page p)
        {
            return "";
        }
        public static string GetHeaderImage(Page p)
        {
            string sImg = "Images/" + Config("logo");
            return sImg;
        }

        public static string GetBeta(Page p)
        {
            string sBeta = Config("beta");
            return sBeta;
        }
        public static string GetBioImg(string orphanid)
        {
            string sql = "Select BioURL from SponsoredOrphan where orphanid=@orphanid";
            SqlCommand command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@orphanid", orphanid);
            string bio = gData.GetScalarString(command, "URL", false);
            return bio;
        }

        public static string RenderControlToHtml(Control ControlToRender)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.IO.StringWriter stWriter = new System.IO.StringWriter(sb);
            System.Web.UI.HtmlTextWriter htmlWriter = new System.Web.UI.HtmlTextWriter(stWriter);
            ControlToRender.RenderControl(htmlWriter);
            return sb.ToString();
        }

        public static void SetSessionDouble(Page p, string keyname, double nValue)
        {
            p.Session[keyname] = nValue.ToString();
            p.Session[keyname + "_age"] = BiblePayCommon.Common.UnixTimeStamp().ToString();
        }

        public static double? RetrieveSessionDouble(Page p, string keyname)
        {
            if (p.Session[keyname] == null)
            {
                return null;
            }
            double nAge = BiblePayCommon.Common.UnixTimeStamp() - GetDouble(p.Session[keyname + "_age"].ToNonNullString());
            if (nAge > 60 * 15)
            {
                return null;
            }

            double nBal = GetDouble(p.Session["balance"].ToNonNullString());
            return nBal;
        }

        public static void BuySomething(Page p, BiblePayCommon.Entity.invoice1 i, string sEvent)
        {
            string sNarr = "This vendor is requesting payment for the following invoice:<br>" + i.InvoiceDate + "<br>" + i.ProductID + "<br>"
                            + i.InvoiceType + "<br>" + i.ServiceName + "<br>For " + i.Amount.ToString() + "<br>";
            if (gUser(p).FA2Verified == 1)
            {
                sNarr += "Enter 2FA pin and press OK to approve this purchase, or Cancel to reject.<br>";
            }
            else
            {
                sNarr += "Press OK to approve this purchase, or Cancel to reject.<br>";
            }
            // Determine whether encrypted:
            p.Session["PENDING_PURCHASE"] = i;
            MsgInput(p, sEvent, "Pending Purchase", sNarr, 700, "", "", InputType.number, !(gUser(p).FA2Verified == 1));
        }

        public static DACResult BuySomething2(Page p, string sPin)
        {
            DACResult r25 = new DACResult();
            if (p.Session["PENDING_PURCHASE"] == null)
            {
                BiblePayCommonNET.UICommonNET.MsgModal(p, "Failure", "The purchase failed!", 600, 300, true);
                r25.Error = "Invoice is null";
                return r25;
            }
            BiblePayCommon.Entity.invoice1 i = (BiblePayCommon.Entity.invoice1)p.Session["PENDING_PURCHASE"];
            DACResult s9 = BiblePayDLL.Sidechain.BuySomething2(IsTestNet(p), i, gUser(p), sPin);
            if (!s9.fError())
            {
                p.Session["balance"] = null;
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(p, IsTestNet(p), i, gUser(p));
                BiblePayCommonNET.UICommonNET.MsgModal(p, "Success", "You have successfully paid this invoice ["
                    + i.ProductID.ToNonNullString() + "] on TXID " + s9.TXID + " for " + i.Amount.ToNonNullString() + " BBP.", 500, 250, true);
                return s9;
            }
            else
            {
                BiblePayCommonNET.UICommonNET.MsgModal(p, "Failure", "The purchase failed: " + s9.Error, 600, 300, true);
                return s9;

            }
        }

        private static long nLastBalance = 0;
        public static string GetAvatarBalance(Page p)
        {
            double? nVal = RetrieveSessionDouble(p, "balance");
            long nElapsed = BiblePayCommon.Common.UnixTimestampUTC() - nLastBalance;
            if (nElapsed > (60 * 3.5))
                nVal = null;

            if (nVal == null)
            {
                string sPub = gUser(p).BiblePayAddress;
                if (sPub == "")
                    return "0.00";

                nVal = BiblePayDLL.Sidechain.QueryAddressBalance(IsTestNet(p), sPub);
                nLastBalance = BiblePayCommon.Common.UnixTimestampUTC();

                SetSessionDouble(p, "balance", (double)nVal);
            }

            return FormatUSD((double)nVal) + " BBP";
        }

        public struct ChatStructure
        {
            public string startedByUser;
            public string chattingWithUser;
            public string chatGuid;
            public bool NotifiedOfChatRequest;
            public bool NotifiedOfEntry;
        }

        public static Dictionary<string, List<string>> dictChatHistory = new Dictionary<string, List<string>>();
        public static Dictionary<string, ChatStructure> dictChats = new Dictionary<string, ChatStructure>();
        public static string GetChatInner(Page p)
        {
            if (gUser(p).LoggedIn == false)
            {
                return "<div id='chatinner'></div>";
            }

            UICommon.ChatStructure myChat;
            bool fGot = UICommon.dictChats.TryGetValue(gUser(p).id, out myChat);
            if (!fGot)
            {
                return "<div id='chatinner'>...</div>";
            }

            List<string> lChat = null;
            fGot = dictChatHistory.TryGetValue(myChat.chatGuid, out lChat);
            if (!fGot)
                return "<div id='chatinner'>...</div>";
            string sScreen = "<div id='chatinner'>";
            for (int i = 0; i < lChat.Count; i++)
            {
                sScreen += "" + lChat[i] + "<br>";
            }
            if (lChat.Count > 14)
            {
                lChat.RemoveAt(0);
            }
            sScreen += "</div>";
            return sScreen;
        }

        public static string GetNotificationConsole(Page p)
        {
            string sNotify = Config("notifications");
            if (sNotify == "")
                return "";

            string sHTML = "";
            string sIcon  = "<i id='iconBell' class='facircle fa fa-bell'></i>";
            string sAnchor = GetStandardAnchor("ancNotificationBell", "GetNotifications", "GN", sIcon, "Get my Notifications", "", "");
            sHTML = sAnchor;
            return sHTML;
        }

        public static string GetChatBox2(Page p)
        {
            // Check for Paging

            if (gUser(p).LoggedIn == false)
                return "";

            UICommon.ChatStructure myChat;

            bool fGot = dictChats.TryGetValue(gUser(p).id, out myChat);

            if (!fGot)
                return "";

            if (myChat.startedByUser == gUser(p).id)
            {
                // This chat was started by me...
                return "";
            }
            if (myChat.NotifiedOfChatRequest == true)
                return "";

            string sURL = "Meeting?type=private&chat=" + myChat.chatGuid;
            string sTheOtherUser = myChat.chattingWithUser == gUser(p).id ? myChat.startedByUser : myChat.chattingWithUser;
            User u = gUserById(p, sTheOtherUser);
            string sClose = "onclick='closeDialog();'";
            string sNarrative = "Hi " + gUser(p).FirstName + ", " + u.FullUserName() 
                + " is calling you into a chat.  <a " + sClose + " href='" + sURL + "'><br><b>Click here if you would like to go. </b></a>";

            BiblePayCommonNET.UICommonNET.MsgModalWithLinks(p, "Chat Request", sNarrative, 600, 300, false, true);
            myChat.NotifiedOfChatRequest = true;
            dictChats[gUser(p).id] = myChat;
            return "";
        }



        public static string GetChatBox(Page p)
        {
            // Check for Paging

            if (gUser(p).LoggedIn == false)
                return "";

            UICommon.ChatStructure myChat;

            bool fGot = dictChats.TryGetValue(gUser(p).id, out myChat);

            if (!fGot)
                return "";

            
            string sTheOtherUser = myChat.chattingWithUser == gUser(p).id ? myChat.startedByUser : myChat.chattingWithUser;

            User u = gUserById(p, sTheOtherUser);

            string sHeaderName = "Chat with " + u.FirstName;
            string sCloseAnchor = GetStandardAnchor("ancClose", "CloseChat", "chat001", "<i class='fa fa-close'></i>", "Close Chat", "");

            string sScreen = "<div class='screenOuter'>";
            sScreen += "<div id='chat1' class='chat' onmousemove='UpdateChatWindow();'>"
                + "<div class='tab' id='header1'>&nbsp;&nbsp;" + sHeaderName + "<div style='float:right;'>" + sCloseAnchor + "</div></div>";
            sScreen += GetChatInner(p);
            
            string sJS = " "
                + ""
                + "var myname='" + gUser(p).FirstName 
                + "';var theirname = '" + u.FirstName + "';"
                + "function sendit(o) {if (event.key != 'Enter') return true; "
                + "var data = XSS(myname + ' said: ' + o.value); transmit(\"" + "chat" + "\", data, 'chatinner', \"\"); o.value=''; "
                + "var owindow = document.getElementById('chat1'); "
                + "owindow.scrollIntoView(false); return false;}";

            sScreen += "<br><script>" + sJS + "</script>Chat and press [ENTER]:<br><input autocomplete=\"nope\" onkeydown='return sendit(this); ' type='text' id='txtChatter' value=''></input>";
            sScreen += "<br>&nbsp;</div></div>";
            return sScreen;
        }
        public static string GetSideBar(Page p)
        {
            item = 0;
            string sWallet = "<a class='decoratedlink' href='Wallet'><i class='fa fa-wallet'></i>&nbsp;"
                + GetAvatarBalance(p) + "</a>";
            //  "<div class='pull-left info'><p>" + gUser(p).FirstName.ToNonNullString() + "</p>" + "</div><div class='myicons'><ul>" + sKeys + "</ul></div>"

            string sChain = IsTestNet(p) ? "TESTNET" : "MAINNET";
            string sDecoratedChain = IsTestNet(p) ? "<font color=lime>TESTNET</font>" : "<font color=silver>MAINNET</font>";
            string sChainAnchor = GetStandardAnchor("ancChain", "btnChangeChain", sChain, sDecoratedChain, "Change your Chain");
            string sKeys = gUser(p).FirstName.ToNonNullString().Length > 0 ? sWallet + " • <small>" + sChainAnchor  + "</small>" : "";
            string sChangeProfileAction = gUser(p).LoggedIn ? "UnchainedUpload?action=setavatar&parentid=" + gUser(p).id : "";

            string sChangeProfileLink = "<a href='" + sChangeProfileAction + "'>" + gUser(p).GetAvatarImage() + "</a>";

            string html = "<div id='entireleftmenu'><aside class='main-sidebar' id='mySidenav'>";
            html += "<section class='sidebar'><div class='user-panel' class='trump'>"
              + "<a style='padding-left:20px;z-index:9999;font-size:150%;background-color:transparent;' onclick='closeNav();' class='sidebar-toggle' data-toggle='offcanvas' role='button'>"
              + "<b><i class='fa fa-close'></i></b></a>"
              + "<div class='pull-left myavatar'>" + sChangeProfileLink + "</div>"
              + "<div class='pull-left info'><p><small>" + gUser(p).FirstName.ToNonNullString() + "</small></p><small><p>" + sKeys + "</p></small>" + "</div>"
              + "</div><div id='divsidebar-menu' class='divsidebar'><ul class='sidebar-menu'>";
            html += AddMenuOptions(gUser(p).LoggedIn);
            html += "</div></section></aside></div>";
            string sFunction = "<script>function myfunc(iActive) { $('#bbpdd' + iActive.toString()).attr('expanded',0);  for (var i = 0; i < 50; i++) {   "
                + "$('#bbpdd' + i.toString() ).attr('xexpanded', 0); $('#bbpdd' + i.toString()).css('display','none'); localStorage.setItem('bbpdd' + i.toString() , 1);   } localStorage.setItem('bbpdd' + iActive.toString() , 1);      }</script>";
            sFunction = "<script>function myfunc(iActive) { }</script>";

            html += sFunction;
            return html;
        }

        public static bool UTXORecordExists(bool fTestNet, string sTable, string sClause)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, sTable);
            dt = dt.FilterDataTable(sClause);
            if (dt.Rows.Count < 1)
                return false;
            return true;
        }

        public static User GetUserRecord(bool fTestNet, string id)
        {
            DataTable dtUsers = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "user1");
            dtUsers = dtUsers.FilterDataTable("BiblePayAddress='" + id + "' or id='" + id + "'");
            User u = new User();
            if (dtUsers.Rows.Count < 1 || id == null || id=="")
            {
                u.AvatarURL = EmptyAvatar();
                u.FirstName = "Guest";
                return u;
            }

            u.BiblePayAddress = dtUsers.Rows[0]["BiblePayAddress"].ToString();
            u.AvatarURL = dtUsers.Rows[0]["AvatarURL"].ToString();
            if (u.AvatarURL == String.Empty)
                u.AvatarURL = EmptyAvatar();

            u.EmailAddress = dtUsers.Rows[0]["EmailAddress"].ToString();
            u.UserName = dtUsers.Rows[0]["UserName"].ToString();
            u.FirstName = dtUsers.Rows[0]["FirstName"].ToString();
            u.LastName = dtUsers.Rows[0]["LastName"].ToString();
            u.Verified = dtUsers.GetColInt("Verified");
            u.id = dtUsers.Rows[0]["id"].ToString();
            u.Gender = dtUsers.Rows[0]["Gender"].ToString();
            u.BirthDate = dtUsers.GetColInt("BirthDate");
            u.TelegramLinkName = dtUsers.Rows[0]["TelegramLinkName"].ToString();
            u.TelegramLinkURL = dtUsers.Rows[0]["TelegramLinkURL"].ToString();
            u.TelegramLinkDescription = dtUsers.Rows[0]["TelegramLinkDescription"].ToString();
            u.PublicText = dtUsers.Rows[0]["PublicText"].ToString();
            u.PrivateText = dtUsers.Rows[0]["PrivateText"].ToString();
            u.ReligiousText = dtUsers.Rows[0]["ReligiousText"].ToString();
            return u;
        }

        private static string GetInnerPoster(string sFID, string sURL2, int nWidth)
        {
            if (sFID == null)
                return "";

            if (sFID.Length < 10)
                return "";
            string sURL1 = (sURL2 == null || sURL2 == "") ? "/images/jc2.png" : sURL2.Replace("/data", "/thumbnails/video.jpg");
            string HTML = "<img src='" + sURL1 + "' class='gallery' />";
            return HTML;
        }

        private static string CurateVideo(Page p, int nWidth, string sID, User u, string sURL2, string SVID,
            string FID, int nAdded, string sTitle, string sBody, bool fShowRearrange, double nOrder, string sSnippet)
        {
            string sDiv = "<div class='gallery'><a href=Media.aspx?id=" + sID + ">";
            string sVideo1 = GetInnerPoster(FID, sURL2, nWidth);
            sDiv += sVideo1;
            if (sBody.Length > 255)
            {
                sBody = sBody.Substring(0, 255) + "...";
            }
            sDiv += "</a></div><div class='gallery-description'><small>"
                + sTitle + " • " + sBody + "<br>Uploaded by " + u.FullUserName() + " "
                + BiblePayCommon.Common.ConvertFromUnixTimestamp(nAdded).ToShortDateString();

            if (fShowRearrange)
            {
                sDiv += " • O:" + nOrder.ToString();
                // Video count
                sDiv += " • " + GetObjectRating(IsTestNet(p), sID, "video1", gUser(p));
                sDiv += " • " + UICommon.GetWatchSum(IsTestNet(p), sID) + " view(s)";
            }
            sDiv += "</small></span>";

            if (HasOwnership(IsTestNet(p), sID, "video1", gUser(p).id))
            {
                string sTrashAnchor = GetStandardAnchor("ancDelete", "DeleteObject", sID, "<i class='fa fa-trash'></i>", "Delete Video Object", "video1");
                if (!fShowRearrange)
                {
                    sDiv += "&nbsp;" + sTrashAnchor;
                }
                if (fShowRearrange)
                {
                    string sRearrangeUp = GetStandardAnchor(sID, "RearrangeObjectUp", sID, "<i class='fa fa-arrow-up'></i>", "Rearrange Item - Move Up", "video1", sSnippet);
                    string sRearrangeDown = GetStandardAnchor(sID, "RearrangeObjectDown", sID, "<i class='fa fa-arrow-down'></i>", "Rearrange Item - Move Down", "video1", sSnippet);
                    sDiv += "&nbsp;" + sRearrangeUp + "&nbsp;" + sRearrangeDown;
                }
            }
            sDiv += "</div>";
            return sDiv;
        }

        private static string CurateImage(Page p, int nWidth, string sID, User u, string sURL, int nAdded, string sTitle, string sBody, bool fShowRearrange,
            double nOrder, string sOptSnippet)
        {
            string sDiv = "<div class='gallery'><a href=Media.aspx?id=" + sID + ">";
            string sImg = "<img class='gallery' src='" + sURL + "'/>";
            sDiv += sImg;
            if (sBody.Length > 255)
            {
                sBody = sBody.Substring(0, 255) + "...";
            }
            sDiv += "</a></div><div class='gallery-description'><small>"
                + sTitle + " • " + sBody + "<br>Uploaded by " + u.FullUserName() + " "
                + BiblePayCommon.Common.ConvertFromUnixTimestamp(nAdded).ToShortDateString();

            if (fShowRearrange)
            {
                sDiv += " • " + nOrder.ToString();
            }
            sDiv += "</small></span>";
            
            if (HasOwnership(IsTestNet(p), sID, "video1", gUser(p).id))
            {
                string sTrashAnchor = GetStandardAnchor(sID, "DeleteObject", sID, "<i class='fa fa-trash'></i>", "Delete Video Object", "video1");
                sDiv += "&nbsp;" + sTrashAnchor;
                if (fShowRearrange)
                {
                    string sRearrangeUp = GetStandardAnchor(sID, "RearrangeObjectUp", sID, "<i class='fa fa-arrow-up'></i>", "Rearrange Item - Move Up", "video1", sOptSnippet);
                    string sRearrangeDown = GetStandardAnchor(sID, "RearrangeObjectDown", sID, "<i class='fa fa-arrow-down'></i>", "Rearrange Item - Move Down", "video1", sOptSnippet);
                    sDiv += "&nbsp;" + sRearrangeUp + "&nbsp;" + sRearrangeDown;
                }
            }
            sDiv += "</div>\r\n";
            return sDiv;
        }

        public static string GetDomainFromURL(string sURL)
        {
            string[] vDomain = sURL.Split("/");
            if (vDomain.Length >= 2)
            {
                string sDomain = vDomain[2];
                return sDomain;
            }
            return "";
        }

        public static string GetTitleFromURL(string sDocument)
        {
            string sData = BiblePayDLL.Sidechain.DownloadString(sDocument);
            if (sData == "")
                return "";
            string sXML = ExtractXML(sData, "<title>", "</title>");
            if (sXML.Length > 500)
            {
                sXML = sXML.Substring(0, 499);
            }
            if (sXML == "")
                sXML = "Unknown Title";
            return sXML;
        }
        public static string MakeShareableLink(string sDocument, string sShareableName)
        {
            string sDomain = GetDomainFromURL(sDocument);
            if (sDomain == "")
                return "";

            string sTitle = GetTitleFromURL(sDocument);
            if (sTitle == "")
                return "";

            
            string sIcon = "https://www.google.com/s2/favicons?domain_url=" + sDomain;
            string sShareable = "<div><img sitedata='shareablelink' width=32 height=32 src='" + sIcon + "'>"
                + "<br><a target='_blank' href='" + sDocument + "'>" + sTitle + "<br>" + sDomain + "<br>" + sShareableName + "</a></div>";
            return sShareable;
        }

        public static DataTable resort(DataTable dt, string colName, string direction)
        {
            try
            {
                DataTable dtOut = null;
                dt.DefaultView.Sort = colName + " " + direction;
                dtOut = dt.DefaultView.ToTable();
                return dtOut;
            }catch(Exception ex)
            {
                return dt;
            }
        }

        public static string GetAttachments(Page p, string sParentID, string sFilter, string sHeaderName, string sStyle)
        {
            //DataTable dtAttachments = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "video1");
            var builder = Builders<BiblePayCommon.Entity.video1>.Filter;
            var filter = builder.Eq("attachment", 1) & builder.Eq("parentid", sParentID);
            //mission critical check the Optional Filter (sFilter) and tack that on...
            IList<BiblePayCommon.Entity.video1> dtAttachments = BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.video1>(false, "video1",
                filter, SERVICE_TYPE.PUBLIC_CHAIN);
            //dtAttachments = dtAttachments.FilterAndSort("attachment=1 and parentid='" + sParentID + "' " + sFilter, "Order");
            string sSnip = sFilter.Contains("Profile") ? "profile" : "";
            if (dtAttachments.Count > 0)
            {
                string sGallery = "<br><div " + sStyle + " class='tab'>&nbsp;&nbsp;" + sHeaderName + "</div><div style='overflow-y:scroll;max-height:300px;' class='border'>"
                    + UICommon.GetGallery(p, dtAttachments, null, "any", 25, 250, 250, false, true, sSnip) + "</div>";
                return sGallery;
            }
            return String.Empty;
        }

        public static string GetGallery(Page p, IList<Entity.video1> dt, BiblePayPaginator.Paginator pag, string sViewType,
            int nWidthPct, int nHeight, int nWidth, bool fVideoContainer, bool fShowRearrangeOption, string sOptSnippet)
        {
            string html = "<table width='100%'><tr>";

            string sClass = fVideoContainer ? "gallerycontainer" : "imgcontainer";
            sClass = "gallerycontainer";

            int iCol = 0;
            if (dt.Count < 1)
            {
                html = "";
                return html;
            }
 
            bool fMobile = BiblePayCommonNET.UICommonNET.fBrowserIsMobile(p);

            int nColsPerRow = fMobile ? 1 : 3;
            nWidthPct = fMobile ? 100 : 33;

            int iItem = 0;
            double nPag = GetDouble(p.Request.QueryString["pag"] ?? "");
            if (nPag > dt.Count)
            {
                nPag = dt.Count - 30;
            }
            if (nPag < 0)
                nPag = 0;
            double nStart = nPag;
            double nEnd = nStart + 30;
            int iRowNumber = 0;
            
            foreach (Entity.video1 v in dt)
            {

                if (iRowNumber >= nStart && iRowNumber <= nEnd)
                {
                    string sURL = v.URL;

                    if (fVideoContainer && v.SVID == null)
                    {
                        sURL = "";
                    }
                    if (sURL != "")
                    {
                        User u = UICommon.GetUserRecord(IsTestNet(p), v.UserID);
                        string sUserName = u.FullUserName();
                        string sElement = "";
                        if ((sViewType == "video" || sViewType == "any") && (sURL.ToLower().Contains(".mp4") || sURL.ToLower().Contains(".webm")))
                        {
                            sElement = CurateVideo(p, nWidth, v.id, u,v.URL2,
                                v.SVID,
                                v.FID, (int)v.time,
                                v.Title,v.Body.ToNonNullString(), fShowRearrangeOption,
                                v.Order, sOptSnippet);

                        }
                        else if ((sViewType == "pdf" || sViewType == "any") && sURL.Contains(".pdf"))
                        {
                            string sPDFLink = "GospelViewer?pdfsource=" + BiblePayCommon.Encryption.Base64Encode(sURL);
                            string sAsset = "<a target='_blank' href='" + sPDFLink + "'>"
                                + "<img class='gallerypdf' src='https://foundation.biblepay.org/images/pdf_icon.png'></a>";
                            string sDiv = "<div class='gallery'>" + sAsset + "</div>";
                            sDiv += "<div class='gallery-description'>" + v.Title.ToNonNullString() + " • Uploaded by " + sUserName + "</div>";
                            sElement = sDiv;
                        }
                        else if ((sViewType == "wiki" || sViewType == "any") && sURL.Contains(".htm"))
                        {
                            string sAsset = "<a href='" + sURL + "'><iframe class='gallery' src='" + sURL + "'></iframe></a>";
                            string sDiv = "<div class='gallery'>" + sAsset;
                            sDiv += "<br>" + v.Subject.ToNonNullString() + " • Uploaded by "
                                + sUserName + "</div>";
                            string sEdit = "<input type='button' onclick=\"location.href='CreateNewDocument?file=" + sURL + "';\" id='w"
                                + v.id + "' value='Edit' />";
                            sElement = sDiv;
                        }
                        else if ((sViewType == "image" || sViewType == "any"))
                        {
                            if (sURL.ToLower().Contains(".png") || sURL.ToLower().Contains(".jpg") || sURL.ToLower().Contains(".jpeg") || sURL.ToLower().Contains(".bmp") || sURL.ToLower().Contains(".gif"))
                            {
                                sElement = CurateImage(p, nWidth, v.id, u, v.URL.ToNonNullString(),
                                     (int)v.time, v.Title.ToNonNullString(),
                                     v.Body.ToNonNullString(), fShowRearrangeOption,
                                     v.Order, sOptSnippet);
                            }
                        }

                        string sVisibility = iItem < 29 ? "galleryvisibile" : "galleryinvisible";


                        string sRow = "<td id='xgtd" + iItem.ToString() + "' width='" + nWidthPct.ToString() + "%' ><div id='gtd" + iItem.ToString() + "' class='gallerycontainer " + sVisibility + "'>"
                            + sElement + "</div></td>";
                        //string sRow = "<div id='gtd" + iItem.ToString() + "' style='width:'" + nWidthPct.ToString() + "%' class='gallerycontainer'>" + sElement + "</div>";
                        //string sRow = sElement;

                        html += sRow;
                        iCol++;
                        iItem++;

                        if (iCol == nColsPerRow)
                        {
                            html += "</tr>\r\n<tr id='gallery" + iItem.ToString() + "'>";
                            //html += "</div><div id='gallery" + iItem.ToString() + "' class='gallerycontainer " + sVisibility + "'>";
                            iCol = 0;
                        }
                    }
                }
                iRowNumber++;
            }
            html += "</table>";
            //html += "&nbsp;</div>";
            return html;
        }

        /*
        public static string GetTopOneByTime(Page p, string table, string sFilter, string sColName)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), table);
            dt = dt.FilterDataTable(sFilter);

            dt = dt.OrderBy("time desc");
            if (dt.Rows.Count < 1)
                return "";
            string sVal = dt.Rows[0][sColName].ToString();
            return sVal;
        }
        */


            /*
        public static BiblePayCommon.BBPDataTable GetGroup(bool fTestNet, string table, string sFilter, string groupbycolumn)
        {
            // SQL Equivalent of : Select id,max(maxcolumn) group by groupbycolumn
            BiblePayCommon.BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, table);
            dt = dt.FilterBBPDataTable(sFilter);

            try
            {
                var query = from row in dt.AsEnumerable()
                            group row by row.Field<string>(groupbycolumn) into grp
                            select new
                            {
                                Key = grp.Key,
                                Id = grp.First().Field<string>("id")
                            };
                BiblePayCommon.BBPDataTable dtOut = new BiblePayCommon.BBPDataTable();
                dtOut.Columns.Add("id");
                dtOut.Columns.Add(groupbycolumn);
                foreach (var grp in query)
                {
                    DataRow _newrow = dtOut.NewRow();
                    _newrow["id"] = grp.Id;
                    _newrow[groupbycolumn] = grp.Key;

                    dtOut.Rows.Add(_newrow);
                }
                return dtOut;
            } catch (Exception ex)
            {
                Log("GetGroup " + ex.Message);
                return dt;
            }
        }
        */

        public static string GetStandardButton(string sID, string sCaption, string sEvent, string sAltText, string sOptJS = "", string sOptClass = "")
        {
            string sButton = "<button class='" + sOptClass + "' id='btn" + sEvent + "_" + sID + "' onclick=\""
                   + "var e={};" + sOptJS + "e.Event='" + sEvent + "_Click';if (e.Event=='_Click') return false;e.Value='"
                   + sID + "';BBPPostBack2(null, e);return false;\" title='" + sAltText + "'>"
                   + sCaption + "</button>";
            return sButton;
        }

        public static string GetStandardAnchor(string sID, string sEvent, string sValue, string sCaption, string sAltText, string sOptTable = "", string sOptSnippet = "")
        {
            string sAnchor = "<a id='" + sID + "' onclick=\"var e={};e.Event='" + sEvent + "_Click';e.Value='" + sValue + "';e.Table='"
                + sOptTable + "';e.Snippet='" + sOptSnippet + "';BBPPostBack2(this, e);\" title='" + sAltText + "'>" + sCaption + "</a>";
            return sAnchor;
        }


        public static string GetStandardButtonWithConfirm(string sID, string sCaption, string sEvent, string sAltText, string sConfirmCaption)
        {
            string sConfirm = "var fConfirm=confirm('" + sConfirmCaption + "'); if (!fConfirm) { return false; } ";

            string sButton = "<button id='attach" + sID + "' onclick=\"" + sConfirm
                   + "var e={};e.Event='" + sEvent + "_Click';e.Value='"
                   + sID + "';BBPPostBack2(null, e);\" title='" + sAltText + "'>"
                   + sCaption + "</button>";
            return sButton;
        }
        public static string CleanseURL(string sURL)
        {
            // Prevents URL display from becoming XSS  vulnerability
            string sData = BiblePayCommon.Encryption.CleanseXSS(sURL);
            bool fOK = true;
            if (sURL.ToLower().Contains("script") || sURL.ToLower().Contains("javascript") || sURL.ToLower().Contains("(") || sURL.Contains(")"))
                fOK = false;
            if (sURL.ToLower().Contains("javas	cript"))
                fOK = false;
            string sDecoded = System.Web.HttpUtility.UrlDecode(sURL);
            sDecoded = sDecoded.Replace("\t", "");

            if (sDecoded.ToLower().Contains("script"))
                fOK = false;
            if (!fOK)
            {
                return "N/A";
            }
            //sData = System.Web.HttpUtility.UrlEncode(sURL);
            return sData;
        }

        public static string GetUserGallery(Page p, IList<BiblePayCommon.Entity.user1> dt, BiblePayPaginator.Paginator pag, int nCols)
        {
            
            string html = "<table width='100%'><tr>";
            int iCol = 0;
            if (dt.Count < 1)
            {
                html += "</table>";
                return html;
            }
            try
            {
                bool fMobile = BiblePayCommonNET.UICommonNET.fBrowserIsMobile(p);
                pag.ColumnsPerRow = fMobile ? 1 : 3;
                pag.Rows = dt.Count;
                pag.RowsPerPage = 3;
                double nWidthPct = 33;

                for (int y = pag.StartRow; y <= pag.EndRow; y++)
                {
                    User u = UICommon.GetUserRecord(IsTestNet(p), dt[y].id);
                    string sURL = "Person?id=" + u.id;
                    string sUserName = u.FullUserName();
                    string sAnchor = "<a href='" + sURL + "'>"
                        + u.GetAvatarImageNoDims("gallerypdf") + "'</a>";
                    string sDiv = "<div class='gallery'>" + sAnchor + "</div>";
                    sDiv += "<div class='gallery-description'>" + u.FullUserName() + " • Since "
                        + BiblePayCommon.Common.UnixTimeStampToDateControl(dt[y].time)
                        + "<br>" + dt[y].domain;

                    string sTelegram = " • <a target='_blank' href='" + CleanseURL(u.TelegramLinkURL.ToNonNullString()) + "'>" 
                        + BiblePayCommon.Encryption.CleanseXSS(u.TelegramLinkName.ToNonNullString()) + "</a>"
                        + "<br>" + BiblePayCommon.Encryption.CleanseXSS(u.TelegramLinkDescription.ToNonNullString());
                    string sChatAnchor = GetStandardAnchor("ancChat", "ChatNow", u.id.ToString(),
                            " • Chat <i class='fa fa-chat'></i>", "Chat with this user Now", "");
                    string sBanAnchor = GetStandardAnchor("ancBan", "BanUser", u.id.ToString(), "<i class='fa fa-ban'></i>", "Ban this User", "user1");

                    if (u.TelegramLinkURL.ToNonNullString().Length > 1)
                    {
                        sDiv += sTelegram;
                    }
                    if (u.id != gUser(p).id)
                    {
                        sDiv += sChatAnchor;
                        sDiv += " • " + UICommon.GetTipControl(IsTestNet(p), u.id, u.id);
                    }
                    if (gUser(p).Administrator==1)
                    {
                        sDiv += " • "+ sBanAnchor;
                    }

                    sDiv += "</div>";
                    string sRow = "<td width='" + nWidthPct.ToString() + "%' class='gallery'>" + sDiv + "</td>";
                    html += sRow;
                    iCol++;

                    if (iCol == pag.ColumnsPerRow)
                    {
                        html += "</tr>\r\n<tr>";
                        iCol = 0;
                    }
                }
                html += "</table>";
                return html;
            }catch(Exception ex)
            {
                Log("GetUserGallery: " + ex.Message);
                return "";
            }
        }

        public static string GetComments(bool fTestNet, string id, Page z, bool fMaskIfNone = false)
        {
            // Shows the comments section for the object.  Also shows the replies to the comments.
            
            var builder = Builders<BiblePayCommon.Entity.comment1>.Filter;
            var filter = builder.Eq("ParentID", id);
            filter &= builder.Regex("ParentID", new BsonRegularExpression(".*" + id + "*", "i"));
            filter &= builder.Ne("deleted", 1);

            IList<BiblePayCommon.Entity.comment1> dt = BiblePayDLL.Sidechain.GetChainObjects<BiblePayCommon.Entity.comment1>(false, "comment1", 
                filter, SERVICE_TYPE.PUBLIC_CHAIN, "time", false);

            string sHTML = "";

            if (!fMaskIfNone)
            {
                sHTML += "<h3>Comments:</h3><br>";
            }

            sHTML += "<table class='saved2' width=100%>"
                + "<tr><th width=4%><th width=14%>User</th><th width=10%>Added</th><th width=11%>Rating</th><th width=64%>Comment</th><th width=1%>Actions</th></tr>";

            if (dt.Count == 0 && fMaskIfNone)
            {
                return "";
            }
            for (int i = 0; i < dt.Count; i++)
            {
                string sBody = "<div class='comments'>" + ReplaceURLs(dt[i].Body) + "</div>";

                // Edit comment and delete comment options
                string sCluster = String.Empty;
                if (HasOwnership(IsTestNet(z), dt[i].id, "comment1", gUser(z).id))
                {
                    string sTrashAnchor = GetStandardAnchor("ancDelete", "DeleteObject", dt[i].id,
                        "<i class='fa fa-trash'></i>", "Delete Comment", "comment1");
                    string sEditAnchor = GetStandardAnchor("ancEdit", "EditObject", dt[i].id,
                        "<i class='fa fa-edit'></i>", "Edit Comment", "comment1");

                    sCluster = sEditAnchor + "&nbsp;" + sTrashAnchor;
                }

                string div = "<tr><td><td>" + UICommon.GetUserAvatarAndName(z, dt[i].UserID)
                    + "</td><td>" + BiblePayCommon.Common.UnixTimeStampToDateControl(dt[i].time)
                    + "</td><td>" + GetObjectRating(fTestNet, dt[i].id, "comment1", gUser(z))
                    + "</td><td>" + sBody + "</td><td>"  + sCluster + "</td></tr>";
                sHTML += div;

            }
            sHTML += "</table>";
            if (fMaskIfNone == true)
            {
                sHTML += "<br>";
                return sHTML;
            }

            sHTML += "<table class='comments'><tr><th colspan=2><h3>Add a Comment:</h3></th></tr>";

            if (!gUser(z).LoggedIn)
            {
                sHTML += "<tr><td><font color=red>Sorry, you must be logged in to add a comment.</td></tr></table>";
                return sHTML;
            }

            string sSaveArea = "<tr><td width=10%>Comment:</td><td><textarea id='txtComment' class='comments' name='txtComment' rows=10 cols=10></textarea><br><br></td></tr></table>";
            sHTML += sSaveArea;
            sHTML += BiblePayCommonNET.UICommonNET.GetButtonTypeSubmit("btnSaveComments1", "SaveComments_Click", "Save Comments");

            return sHTML;
        }

        public static string GetUserName(DataTable dt, int nRow)
        {
            string sLastName = dt.GetColValue(nRow,"lastname");
            string sFirstName = dt.GetColValue(nRow, "firstname");
            if (sLastName != "" && sFirstName != "")
            {
                return sLastName + ", " + sFirstName;
            }
            else if (sLastName != "" && sFirstName == "")
            {
                return sLastName;
            }
            else
            {
                return sFirstName;
            }
        }
        public static string GetDropDownUser(Page p, string sHTMLID, string sSelectedValue, string sOriginatorID, bool fAdminOnly)
        {
            BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "user1");
            if (fAdminOnly)
            {
                dt = dt.FilterBBPDataTable("administrator=1 or id='" + gUser(p).id + "' or id='" + sOriginatorID + "'");
                dt = dt.OrderBy0("lastname,firstname desc");
            }
            string sOptions = "";
            sOptions += "<option></option>";
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string sFullName = GetUserName(dt, y);
                bool fSelected = sSelectedValue == dt.GetColValue(y, "id");
                string sSel = fSelected ? " SELECTED " : "";
                string sRow = "<option " + sSel + "value='" + dt.Rows[y]["id"].ToString() + "'>"
                    + sFullName + "</option>\r\n";
                sOptions += sRow;
            }
            string sDD = "<select name='" + sHTMLID + "' id='" + sHTMLID + "'>";
            sDD += sOptions;
            sDD += "</select>";
            return sDD;
        }

        public static string GetDropDownFromDataTable(DataTable dt, string sColName, string sHTMLID, string sSelectedValue)
        {
            string sOptions = "";
            for (int y = 0; y < dt.Rows.Count; y++)
            {
                string sColValue = dt.Rows[y][sColName].ToString(); // + " " + dt.Rows[y]["lastname"];
                bool fSelected = sSelectedValue.ToLower() == sColValue.ToLower();
                string sSel = fSelected ? " SELECTED " : "";
                string sRow = "<option " + sSel + "value='" + dt.Rows[y]["id"].ToString() + "'>"
                    + sColValue + "</option>\r\n";
                sOptions += sRow;
            }
            string sDD = "<select name='" + sHTMLID + "' id='" + sHTMLID + "'>";
            sDD += sOptions;
            sDD += "</select>";
            return sDD;
        }

        /*
        public static string GetDropDownOrganization(Page p, string sHTMLID, string sSelectedValue)
        {
            BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "Organization");
            string dd = GetDropDownFromDataTable(dt, "name", sHTMLID, sSelectedValue);
            return dd;
        }

        public static string GetDropDownRole(Page p, string sHTMLID, string sSelectedValue)
        {
            BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "Role");
            string dd = GetDropDownFromDataTable(dt, "name", sHTMLID, sSelectedValue);
            return dd;
        }
        */


        public static string GetDispositions(string HTMLID, string sSelectedValue)
        {
            string sDispositions = "Change Approval;Customer Acceptance;Code Review;Closed;Configuration Change;Customer Service;Programming;Research;Release to Test;Release to Production;Test";
            DataTable dt = BiblePayDLL.Sidechain.StringToDataTable(sDispositions);
            string sDisp = GetDropDownFromDataTable(dt, "column1", HTMLID, sSelectedValue);
            return sDisp;
        }

        public static string VideoCategories = ";Autos & Vehicles;Comedy/Entertainment;Computers;Education;Health/Medical;Film/Animation;Finance & Cryptocurrency;Gaming;How-To/Style;Interview/Documentary;Music;"
            + "News & Politics;Nonprofits & Activism;Pets & Animals;People & Blogs;Sports;"
            + "Science/Technology;Religion;Travel & Events";

        public static string GetVideoCategories(string HTMLID, string sSelectedValue)
        {
            DataTable dt = BiblePayDLL.Sidechain.StringToDataTable(VideoCategories);
            string sDisp = GetDropDownFromDataTable(dt, "column1", HTMLID, sSelectedValue);
            return sDisp;
        }


        /*
        public static string GetStandardDropDown(Page p, string sID, string sTable, string sColumn, string sSelectedValue)
        {
            DataTable dtGroup = UICommon.GetGroup(IsTestNet(p), "video1", "url like '%mp4%'", "Category");
            string sOptions = "";
            for (int y = 0; y < dtGroup.Rows.Count; y++)
            {
                bool fSelected = sSelectedValue.ToLower() == dtGroup.Rows[y]["category"].ToString().ToLower();
                string sSel = fSelected ? " SELECTED " : "";

                string sRow = "<option " + sSel + "value='" + dtGroup.Rows[y]["category"].ToString() + "'>"
                    + dtGroup.Rows[y]["category"].ToString() + "</option>\r\n";
                sOptions += sRow;
            }
            string sDD = "<select name='" + sID + "' id='" + sID + "'>";
            sDD += sOptions;
            sDD += "</select>";
            return sDD;
        }
        */


        public static void NotifyOfSale(Page p, bool fTestNet, User u, BiblePayCommon.Entity.NFT n, double nOfferPrice, string sTXID)
        {
            MailAddress r = new MailAddress("rob@saved.one", "The BiblePay Team");
            MailAddress t = new MailAddress(u.EmailAddress, u.UserName);
            MailAddress bcc = new MailAddress("rob@biblepay.org", "Rob Andrews");
            MailMessage m = new MailMessage(r, t);
            string sChainPrefix = fTestNet ? "[! TESTNET ! ] " : "";

            m.Bcc.Add(bcc);
            // Add the Seller too
            User uSeller = gUserById(p, n.OwnerUserID);
            if (uSeller.EmailAddress.ToNonNullString() != "")
            {
                m.CC.Add(uSeller.EmailAddress);
            }

            bool fOrphan = n.Type.ToLower().Contains("orphan");
            string sNarr = fOrphan ? "sponsored" : "purchased";
            m.Subject = sChainPrefix + "You have successfully " + sNarr + " NFT ID " + n.id + "!";

            if (fOrphan)
            {
                m.Subject += " [orphan]";
                bool fCameroon = n.LowQualityURL.ToLower().Contains("cameroon");
                if (fCameroon)
                {
                    MailAddress newcc = new MailAddress("todd.justin@cameroonone.org", "Todd Finklestone");
                    m.CC.Add(newcc);
                }
            }

            string sBody = sChainPrefix + "<br>Dear " + u.UserName + ",<br><br>Congratulations, you " + sNarr + " '" + n.Name + "', '" + n.id + "' for " 
                + nOfferPrice.ToString() + " BBP in TXID " + sTXID + "!  <br><br>To view your NFT's "
                + " please navigate <a href='https://unchained.biblepay.org/NFTList'>here</a>.<br><br>Thank you for using Biblepay.  <br><br>Sincerely Yours,<br>The BiblePay Team";

            m.IsBodyHtml = true;
            m.Body = sBody;
            EmailNarr e = GetEmailFooter(p);

            BiblePayDLL.Sidechain.SendMail(fTestNet, m, e.DomainName);
        }


        public static bool LogIn(Page p, User u)
        {
            string sDomainName = HttpContext.Current.Request.Url.Host;
            BiblePayDLL.Sidechain.SetBiblePayAddressAndSignature(IsTestNet(p), sDomainName, ref u);
            p.Session[GetChain0(IsTestNet(p)) + "user"] = u;
            p.Session["balance"] = null;
            if (u.EmailAddress.ToNonNullString() == "")
            {
                p.Session["stack"] = UICommon.Toast("ERROR", "There was an error while logging you in.");

                return false;
            }

            p.Session["stack"] = UICommon.Toast("Logging In", "You are now logged in.");

            // store cookie
            BMS.StoreCookie("email", u.EmailAddress);
            BMS.StoreCookie("pwhash", u.PasswordHash);
            BMS.StoreCookie("sessiontime", UnixTimestampUTC().ToString());

            // This should be configurable by key also
            string sDefaultDocument = Config("DefaultDocument");
            if (sDefaultDocument == "")
            {
                p.Response.Redirect("VideoList");
            }
            else
            {
                p.Response.Redirect(sDefaultDocument);
            }
            return true;
        }
        public static bool LoginWithCookie(Page p)
        {
            string sEmailAddress = BMS.GetCookie("email");
            string sPWHash = BMS.GetCookie("pwhash");
            int nCookieTime = (int)GetDouble(BMS.GetCookie("sessiontime"));
            double nAge = UnixTimestampUTC() - nCookieTime;
            User u = gUser(p, sEmailAddress);
            bool fPasswordPassed = (sPWHash == u.PasswordHash && u.PasswordHash.ToNonNullString() != "");
            if (fPasswordPassed && nAge < (60 * 60 * 24 * 7))
            {
                UICommon.LogIn(p, u);
                return true;
            }
            return false;

        }



        public static string GetBannerControls(Page p)
        {
            string sUpload = GetStandardButton("btnUpload","Upload <i class='fa fa-camera'></i>", "UploadVideo", "Upload a new Video", "", "largebuttondark");

            string sSwitchToVideoModule = GetStandardButton("btnVideoModule", "<i class='fa fa-video'></i>", "WatchVideos", "Watch Decentralized Videos", "", "largebuttondark");
            string sSwitchToPeopleModule= GetStandardButton("btnPeopleModule", "<i class='fa fa-users'></i>","PeopleModule", "Connect with Friends", "", "largebuttondark");

            string s1 = GetStandardButton("btnLogIn", gUser(p).LoggedIn ? "Log Out" : "Log In", gUser(p).LoggedIn ? "LogOut" : "LogIn", 
                gUser(p).LoggedIn ? "Log Out of the system" : "Log into the system", "" , "largebuttondark");
            string s2 = GetStandardButton("btnLogIn", gUser(p).LoggedIn ? "My Account" : "Sign Up", 
                "SignUp", gUser(p).LoggedIn ? "Edit my user record" : "Join our community!", "", "largebuttondark");
            
            string sOut = String.Empty;
            if (gUser(p).LoggedIn)
                sOut += sUpload + "&nbsp;&nbsp;";

            sOut += sSwitchToVideoModule + "&nbsp;&nbsp;";
            if (gUser(p).LoggedIn)
                sOut += sSwitchToPeopleModule + "&nbsp;&nbsp;";
            sOut += s1 + "&nbsp;&nbsp;" +  s2;


            return sOut;
        }
        
        public static string GetHeaderBanner(Page p)
        {
            return Config("sitename");
        }

        public static string GetTd2(DataRow dr, string colname, string sAnchor, string sPrefix = "", bool fBold = false)
        {
            string val = dr[colname].ToString();
            string sBold = fBold ? "<b>" : "";
            string td = "<td><nobr>" + sBold + sPrefix + sAnchor + val + "</a></td>";
            return td;
        }

        public static bool IsAllowableExtension(string path)
        {
            string ext = Path.GetExtension(path).ToLower();
            if (ext.Length < 1) return false;
            ext = ext.Substring(1, ext.Length - 1);
            string allowed = "jpg;jpeg;gif;bmp;png;pdf;csv;mp4;webm";
            string[] vallowed = allowed.Split(";");
            for (int i = 0; i < vallowed.Length; i++)
            {
                if (vallowed[i] == ext)
                    return true;
            }
            return false;
        }

        public static string GetUserAvatarAndName(Page p, string sUserID, bool fHorizontal = false)
        {
            User u = GetUserRecord(IsTestNet(p), sUserID);
            string sURL = "Person?id=" + u.id;
            string sProfileAnchor = "<a href='" + sURL + "'>";
            string s = sProfileAnchor + u.GetAvatarImage() + "<br>" + u.FullUserName() + "</a>";
            if (fHorizontal)
                s = sProfileAnchor + u.GetAvatarImage() + " • " + u.FullUserName() + "</a>";
            return s;
        }

        public static string RenderControl(WebControl u)
        {
            string html = String.Empty;
            using (TextWriter myTextWriter = new StringWriter(new StringBuilder()))
            {
                using (HtmlTextWriter myWriter = new HtmlTextWriter(myTextWriter))
                {
                    u.RenderControl(myWriter);
                    html = myTextWriter.ToString();
                    return html;
                }
            }
        }
        public static string GetContextMenu(string sID, string sMenuItemCaptions, string sMenuItemActions)
        {
            // This is for a right-click context sensitive programmable on-demand menu, which we do not use yet, but may come in handy because we are running out of menu space...
            string[] vMenuItemCaptions = sMenuItemCaptions.Split(";");
            string[] vMenuItemActions = sMenuItemActions.Split(";");
            string htmlCMI = "";
            for (int i = 0; i < vMenuItemCaptions.Length; i++)
            {
                string sRow = "";
                sRow = "   '" + vMenuItemActions[i]  + "': {name: '" + vMenuItemCaptions[i] + "', icon: 'icon" + i.ToString() + "'},";
                htmlCMI += sRow;
            }
            string sContextEvent = " var sUSGDID = $(this)[0].getAttribute('unchainedid');var e={};e.Event='Event_ContextMenu';e.Value='key"
                +sID + "';BBPPostBack2(this, e);";
            string sContextMenuCssClass = "context-menu-1";
            string sContextMenu = "  $(function() {   $.contextMenu({     selector: '." 
                + sContextMenuCssClass + "',        callback: function(key, options) { " 
                + "       " + sContextEvent + "            },"
                + "       items: {  " + htmlCMI + "                     }                    });"
                + "       $('." + sContextMenuCssClass + "').on('click', function(e){      console.log('clicked', this);        })        });";
            return "<script>" + sContextMenu + "</script>";
        }
        public static void MsgBox(string sTitle, string sBody, System.Web.UI.Page p)
        {
            p.Session["MSGBOX_TITLE"] = sTitle;
            p.Session["MSGBOX_BODY"] = sBody;
            p.Response.Redirect("MessagePage.aspx");
        }

        public static string GetTipControl(bool fTestNet, string sContentID, string sTipToUserID)
        {
            string sTipButton = "<a title='Tip this person' id='btnTip' onclick=\""
                + "var Extra={};Extra.ContentID='" + sContentID + "';Extra.TipTo='"
                + sTipToUserID + "';Extra.Event='btnTip_Click';BBPPostBack2(this,Extra);\">"
                + "<span id = 'spantip1" + sContentID + "' class='fa fa-dollar-sign'></span></a>";
            return sTipButton;
        }
        public static string GetFollowControl(bool fTestNet, string sFollowedID, string sMyUserID)
        {
            string sHTML = "";
            string sStatus = GetFollowStatus(fTestNet, sFollowedID, sMyUserID);
            string sAction = sStatus == "Follow" ? "follow" : "unfollow";
            string sIcon = sStatus == "Follow" ? "fa-heart" : "fa-heart-broken";
            sHTML += "<a title='Follow this channel' onclick='var o=document.getElementById(\"follow1"
                + sFollowedID + "\");transmitfollow(\"" + sFollowedID + "\", o.innerHTML, \"follow1"
                + sFollowedID + "\", \"\");'>"
                + "<span id='span1" + sFollowedID + "' class='fa " + sIcon + "'></span></a>"
                + "&nbsp;"
                + "<span id='follow1" + sFollowedID + "'>" + sStatus + "</span>";
            return sHTML;
        }


        public static string GetFollowStatus(bool fTestNet, string sFollowedID, string sMyUserID)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "follow1");
            dt = dt.FilterDataTable("followedid='" + sFollowedID + "' and userid='" + sMyUserID + "' and deleted=0");
            if (dt.Rows.Count < 1)
                return "Follow";
            return "Unfollow";
        }

        public static double GetWatchSum(bool fTestNet, string sParentID)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "objectcount1");
            dt = dt.FilterDataTable("parentid='" + sParentID + "'");
            double nTotal = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double nCountValue = GetDouble(dt.Rows[i]["CountValue"]);
                nTotal += nCountValue;
            }
            return nTotal;
        }

        public static double GetWatchSumUser(bool fTestNet, string sParentID, string sUserId)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "objectcount1");
            dt = dt.FilterDataTable("parentid='" + sParentID + "' and userid='" + sUserId + "'");
            double nTotal = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double nCountValue = GetDouble(dt.Rows[i]["CountValue"]);
                nTotal += nCountValue;
            }
            return nTotal;
        }

        public static bool StoreCount(string parentID, Page p, string sCountType)
        {
            // This increments the float (IE the count) in the object table for the said primary key
            // For example, for a VIDEO, how many distinct viewers (by user.id) looked at one video parent guid
            if (!gUser(p).LoggedIn)
            {
                return false;
            }
            BiblePayCommon.Entity.objectcount1 o = new BiblePayCommon.Entity.objectcount1();
            o.UserID = gUser(p).id;
            o.ParentID = parentID;
            o.CountType = sCountType;
            o.CountValue = 1;
            if (o.UserID == String.Empty)
                return false;
            double nWS = GetWatchSumUser(IsTestNet(p), parentID, gUser(p).id);
            if (nWS > 0)
                return true;
            DataOps.InsertIntoTable_Background(IsTestNet(p), o, gUser(p));
            return true;
        }

        public struct EmailNarr
        {
            public string Footer;
            public string ClosingSalutation;
            public string DomainName;
        }
        public static EmailNarr GetEmailFooter(Page p)
        {

            EmailNarr e = new EmailNarr();

            e.DomainName = HttpContext.Current.Request.Url.Host;
            e.DomainName = e.DomainName.Replace("www.", "");

            e.Footer = "";
            return e;
        }

    }
}
