using BiblePayCommon;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public static class UICommon
    {
        

        public static string GetCurrentThemeName(Page p)
        {
            string sTheme = gUser(p).ThemeName;
            if (sTheme == null || sTheme=="")
                sTheme = "black";
            return sTheme;
        }

        public static bool ReskinCSS(string sPrimaryColor, string sSecondaryColor, string sPrimaryDarkerColor, string sNewName)
        {
            // this reskins our left vertical menu and our top header bg-color.
            string sPath = System.Web.HttpContext.Current.Server.MapPath("Content\\sidenav.css");
            string sData = System.IO.File.ReadAllText(sPath);
            sData = sData.Replace("black_black", sPrimaryColor);
            sData = sData.Replace("maroon_maroon", sSecondaryColor);
            sData = sData.Replace("grey_grey", sPrimaryColor); //this is the user avatar background only
            sData = sData.Replace("primary_darker_color", sPrimaryDarkerColor);
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

            //string sOldJs = "myfunc();";

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
            number
        };

        public static void MsgInput(Page p, string sCallBackEvent, string sTitle, string sNarrative,
                     int nWidth, string sAddress, string sAmt, InputType eInputType, bool fInputHidden, string sEventValue = "")
        {
            string sType = eInputType.ToString();
            if (fInputHidden)
                sType = "hidden";

            string sJavascript = "$('<form id=\"wform1\" method=\"POST\">" + sNarrative
                + "<br><input type=\"" + sType + "\" class=\"trump\"  name=\"q1\"><br></form>').dialog({"
                + "  modal: true, width: " + nWidth.ToString() + ", title: '" + sTitle + "', buttons: {    'OK': function() {"
                + "  var name = $('input[name=\"q1\"]').val(); "
                + "  var Extra={};Extra.Value='" + sEventValue + "';Extra.Address='" + sAddress + "';Extra.Amount='" + sAmt + "';"
                + "Extra.Output=window.btoa(name);Extra.Event='" + sCallBackEvent + "';BBPPostBack2(this, Extra);"
                + "$(this).dialog('close');"
                + "  },     'Cancel': function() {       $(this).dialog('close');                 }            }        });";
            p.ClientScript.RegisterStartupScript(p.GetType(), "msginput1", sJavascript, true);
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
            string sImg = "Images/"+ Config("logo");
            return sImg;
        }

        public static string GetBioImg(string orphanid)
        {
            string sql = "Select BioURL from SponsoredOrphan where orphanid=@orphanid";
            SqlCommand command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@orphanid", orphanid);
            string bio = gData.GetScalarString(command, "URL", false);
            return bio;
        }

        public static string GetTd(DataRow dr, string colname, string sDestination, string sExtra = "")
        {
            string sAnchor = "<a href='"+ sDestination + ".aspx?id=" + dr["id"].ToString() + sExtra + "'>";
            string val = dr[colname].ToString();
            string td = "<td>" + sAnchor + val + "</a></td>";
            return td;
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
            MsgInput(p, sEvent, "Pending Purchase", sNarr, 700, "", "", InputType.number, !(gUser(p).FA2Verified==1));
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
                    + i.ProductID.ToNonNullString()+ "] for " + i.Amount.ToNonNullString() + " BBP.", 600, 300, true);
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

        public static string GetSideBar(Page p)
        {
            item = 0;
            string sWallet = "<a class='wallet' href='RegisterMe'><i class='fa fa-wallet'></i>&nbsp;"
                + GetAvatarBalance(p) + "</a>";

            string sChain = IsTestNet(p) ? "TESTNET" : "MAINNET";
            string sDecoratedChain = IsTestNet(p) ? "<font color=lime>TESTNET</font>" : "<font color=gold>MAINNET</font>";
            string sChainAnchor = GetStandardAnchor("ancChain","btnChangeChain",sChain,sDecoratedChain);
            string sKeys = gUser(p).FirstName.ToNonNullString().Length > 0 ? sWallet + " • " + sChainAnchor : "";
            string html = "<aside class='main-sidebar' id='mySidenav'>";
            html += "<section class='sidebar'><div class='user-panel' class='trump'>"
              + "<a onclick='closeNav();' href = '#' class='sidebar-toggle' data-toggle='offcanvas' role='button'>"
              + "<i class='fa fa-close'></i></a>"
              + "<div class='pull-left myavatar'>" + gUser(p).GetAvatarImage() + "</div>"
              + "<div class='pull-left info'><p>" + gUser(p).FirstName.ToNonNullString() + "</p>" + "</div><div class='myicons'><ul>" 
              + sKeys + "</ul></div>"
              + "</div><ul class='sidebar-menu'>";
            html += AddMenuOptions(gUser(p).LoggedIn);
            html += "</section></aside>";
            string sFunction = "<script>function myfunc(iActive) { $('#bbpdd' + iActive.toString()).attr('expanded',0);  for (var i = 0; i < 50; i++) {   "
                + "$('#bbpdd' + i.toString() ).attr('xexpanded', 0); $('#bbpdd' + i.toString()).css('display','none'); localStorage.setItem('bbpdd' + i.toString() , 1);   } localStorage.setItem('bbpdd' + iActive.toString() , 1);      }</script>";
            html += sFunction;
            return html;
        }

        public static bool RecordExists(bool fTestNet, string sTable, string sClause)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, sTable);
            dt = dt.FilterDataTable(sClause);
            if (dt.Rows.Count < 1)
                return false;
            return true;
        }

        public static User GetUserRecord(bool fTestNet, string id)
        {
            DataTable dtUsers = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "user1");
            dtUsers = dtUsers.FilterDataTable("BiblePayAddress='" + id + "' or id='" + id + "'");
            User u = new User();
            if (dtUsers.Rows.Count < 1)
                return u;

            u.BiblePayAddress = dtUsers.GetColValue("BiblePayAddress");
            u.AvatarURL = dtUsers.GetColValue("AvatarURL");
            if (u.AvatarURL == String.Empty)
                u.AvatarURL = EmptyAvatar();

            u.EmailAddress = dtUsers.GetColValue("EmailAddress");
            u.UserName = dtUsers.GetColValue("UserName");
            u.FirstName = dtUsers.GetColValue("FirstName");
            u.LastName = dtUsers.GetColValue("LastName");
            u.Verified = dtUsers.GetColInt("Verified");
            u.id = dtUsers.GetColValue("id");
            u.Gender = dtUsers.GetColValue("Gender");
            u.BirthDate = (int)dtUsers.GetColDouble("BirthDate");
            u.TelegramLinkName = dtUsers.GetColValue("TelegramLinkName");
            u.TelegramLinkURL = dtUsers.GetColValue("TelegramLinkURL");
            u.TelegramLinkDescription = dtUsers.GetColValue("TelegramLinkDescription");
            u.PublicText = dtUsers.GetColValue("PublicText");
            u.PrivateText = dtUsers.GetColValue("PrivateText");
            u.ReligiousText = dtUsers.GetColValue("ReligiousText");

            return u;
        }
       
        private static string GetInnerPoster(string sFID, string sURL2, int nWidth)
        {
            if (sFID.Length < 10)
                return "";
            string sURL1 = (sURL2 == null || sURL2 == "") ? "/images/jc2.png" : sURL2.Replace("/data", "/thumbnails/video.jpg");
            string HTML = "<img src='" + sURL1 + "' class='gallery' />";
            return HTML;
        }

        private static string CurateVideo(Page p, int nWidth, string sID, User u, string sURL2, string SVID, 
            string FID, int nAdded, string sTitle, string sBody)
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
                + BiblePayCommon.Common.ConvertFromUnixTimestamp(nAdded).ToShortDateString() + "</small></span>";

            if (HasOwnership(IsTestNet(p), sID, "video1", gUser(p).id))
            {
                string sTrashAnchor = GetStandardAnchor("ancDelete", "DeleteObject", sID, "<i class='fa fa-trash'></i>", "video1");
                sDiv += sTrashAnchor;
            }
            sDiv += "</div>";
            return sDiv;
        }

        private static string CurateImage(Page p, int nWidth, string sID, User u, string sURL, int nAdded, string sTitle, string sBody)
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
                + BiblePayCommon.Common.ConvertFromUnixTimestamp(nAdded).ToShortDateString() + "</small></span>";
            if (HasOwnership(IsTestNet(p), sID, "video1", gUser(p).id))
            {
                string sTrashAnchor = GetStandardAnchor(sID, "DeleteObject", sID, "<i class='fa fa-trash'></i>", "video1");
                sDiv += sTrashAnchor;
            }
            sDiv += "</div>";
            return sDiv;
        }

        public static string GetGallery(Page p, DataTable dt, BiblePayPaginator.Paginator pag, string sViewType, int nWidthPct, int nHeight, int nWidth)
        {
            string html = "<table width='100%'><tr>";
            int iCol = 0;
            if (dt.Rows.Count < 1)
            {
                html += "</table>";
                return html;
            }

            bool fMobile = BiblePayCommonNET.UICommonNET.fBrowserIsMobile(p);

            int nColsPerRow = fMobile ? 1 : 3;

            pag.Rows = dt.Rows.Count;
            pag.ColumnsPerRow = nColsPerRow;
            pag.RowsPerPage = 3;

            //int nPage = _paginator.PageNumber;
            
            for (int y = pag.StartRow; y <= pag.EndRow; y++)
            {
                string sURL = dt.Rows[y]["URL"].ToNonNullString();
                if (sURL != "")
                {
                    User u = UICommon.GetUserRecord(IsTestNet(p), dt.GetColValue(y, "UserID"));
                    string sUserName = u.FullUserName();
                    string sElement = "";
                    if ((sViewType == "video" || sViewType=="any") && sURL.Contains(".mp4"))
                    {
                        sElement = CurateVideo(p, nWidth, dt.Rows[y]["id"].ToNonNullString(), u, dt.Rows[y]["URL2"].ToNonNullString(),
                            dt.GetColValue(y, "SVID"), dt.GetColValue(y, "FID"), (int)dt.GetColDouble(y,"time"),
                            dt.Rows[y]["Title"].ToNonNullString(), dt.Rows[y]["Body"].ToNonNullString());

                    }
                    else if ((sViewType == "pdf" || sViewType == "any") && sURL.Contains(".pdf"))
                    {
                        string sPDFLink = "GospelViewer?pdfsource=" + BiblePayCommon.Encryption.Base64Encode(sURL);
                        string sAsset = "<a target='_blank' href='" + sPDFLink + "'>"
                            + "<img class='gallerypdf' src='https://foundation.biblepay.org/images/pdf_icon.png'></a>";
                        string sDiv = "<div class='gallery'>" + sAsset + "</div>";
                        sDiv += "<div class='gallery-description'>" + dt.Rows[y]["Title"].ToNonNullString() + " • Uploaded by " + sUserName + "</div>";
                        sElement = sDiv;
                    }
                    else if ((sViewType == "wiki" || sViewType == "any") && sURL.Contains(".htm"))
                    {
                        string sAsset = "<a href='" + sURL + "'><iframe class='gallery' src='" + sURL + "'></iframe></a>";
                        string sDiv = "<div class='gallery'>" + sAsset;
                        sDiv += "<br>" + dt.Rows[y]["Subject"].ToNonNullString() + " • Uploaded by " 
                            + sUserName + "</div>";
                        string sEdit = "<input type='button' onclick=\"location.href='CreateNewDocument?file=" + sURL + "';\" id='w" + y.ToString() + "' value='Edit' />";
                        sElement = sDiv;
                    }
                    else if ((sViewType == "image" || sViewType == "any"))
                    {
                        if (sURL.Contains(".png") || sURL.Contains(".jpg") || sURL.Contains(".jpeg") || sURL.Contains(".bmp") || sURL.Contains(".gif"))
                        {
                            sElement = CurateImage(p, nWidth, dt.Rows[y]["id"].ToNonNullString(), u, dt.Rows[y]["URL"].ToNonNullString(),
                                 (int)dt.GetColDouble(y, "time"), dt.Rows[y]["Title"].ToNonNullString(), 
                                 dt.Rows[y]["Body"].ToNonNullString());
                        }
                    }

                    string sRow = "<td width='" + nWidthPct.ToString() + "%' class='gallery'>" + sElement + "</td>";
                    html += sRow;
                    iCol++;

                    if (iCol == pag.ColumnsPerRow)
                    {
                        html += "</tr>\r\n<tr>";
                        iCol = 0;
                    }
                }
            }
            html += "</table>";
            return html;
        }
        public static BiblePayCommon.BBPDataTable GetGroup(bool fTestNet, string table, string sFilter, string groupbycolumn)
        {
            // SQL Equivalent of : Select id,max(maxcolumn) group by groupbycolumn
            BiblePayCommon.BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, table);
            dt = dt.FilterBBPDataTable(sFilter);
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

                //Console.WriteLine(String.Format("The Max of '{0}' is {1}& {2}", grp.Id, grp.nftid, grp.max1));
                dtOut.Rows.Add(_newrow);
            }
            return dtOut;
        }

        public static string GetStandardButton(string sID, string sCaption, string sEvent, string sAltText, string sOptJS="")
        {
            string sButton = "<button id='btn" + sID + "' onclick=\""
                   + "var e={};" + sOptJS + "e.Event='" + sEvent + "_Click';e.Value='"
                   + sID + "';BBPPostBack2(null, e);\" title='" + sAltText + "'>"
                   + sCaption + "</button>";
            return sButton;
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

        public static string GetUserGallery(Page p, DataTable dt, BiblePayPaginator.Paginator pag, int nCols)
        {
            string html = "<table width='100%'><tr>";
            int iCol = 0;
            if (dt.Rows.Count < 1)
            {
                html += "</table>";
                return html;
            }
            bool fMobile = BiblePayCommonNET.UICommonNET.fBrowserIsMobile(p);
            pag.ColumnsPerRow = fMobile ? 1 : 3;
            pag.Rows = dt.Rows.Count;
            pag.RowsPerPage = 3;
            double nWidthPct = 33;
            for (int y = pag.StartRow; y <= pag.EndRow; y++)
            {
                User u = UICommon.GetUserRecord(IsTestNet(p), dt.GetColValue(y, "id"));
                string sURL = "Person?firstname=" + dt.Rows[y]["firstname"].ToNonNullString() + "&lastname=" + dt.Rows[y]["lastname"].ToNonNullString();
                string sUserName = u.FullUserName();
                string sAvatar = u.AvatarURL;
                sAvatar = sAvatar.Replace("<img src='images/emptyavatar.png' width=50 height=50>", "images/emptyavatar.png");

                string sAnchor = "<a href='" + sURL + "'>"
                    + "<img class='gallerypdf' src='" + sAvatar + "'</a>";

                string sDiv = "<div class='gallery'>" + sAnchor + "</div>";
                sDiv += "<div class='gallery-description'>" + u.FullUserName() + " • Since " 
                    + dt.GetColDateTime(y, "time").ToShortDateString()
                    + "<br>" + dt.Rows[y]["domain"].ToNonNullString() + "";

                string sTelegram = " • <a href='" + u.TelegramLinkURL.ToNonNullString() + "'>" + u.TelegramLinkName.ToNonNullString() + "</a>"
                    + "<br>" + u.TelegramLinkDescription.ToNonNullString();

                if (u.TelegramLinkURL.ToNonNullString().Length > 1)
                {
                    sDiv += sTelegram;
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
        }
        public static string GetStandardAnchor(string sID, string sEvent, string sValue, string sCaption, string sOptTable = "")
        {
            string sAnchor = "<a id='" + sID + "' onclick=\"var e={};e.Event='" + sEvent + "_Click';e.Value='" + sValue + "';e.Table='"
                + sOptTable + "';BBPPostBack2(this, e);\">" + sCaption + "</a>";
            return sAnchor;
        }

        public static string GetComments(bool fTestNet, string id, Page z, bool fMaskIfNone = false)
        {
            // Shows the comments section for the object.  Also shows the replies to the comments.
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "comment1");
            dt = dt.FilterDataTable("parentid='" + id + "'");
            string sHTML = "";

            if (!fMaskIfNone)
            {
                sHTML += "<h3>Comments:</h3><br>";
            }

            sHTML += "<table class='saved2'>"
                + "<tr><th width=4%><th width=14%>User</th><th width=10%>Added</th><th width=11%>Rating</th><th width=64%>Comment</th></tr>";

            if (dt.Rows.Count == 0 && fMaskIfNone)
            {
                return "";
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sBody = ReplaceURLs(dt.Rows[i]["Body"].ToString());
                string div = "<tr><td><td>" + UICommon.GetUserAvatarAndName(z, dt.GetColValue(i, "UserID"))
                    + "</td><td>" + dt.GetColDateTime(i, "time").ToString()
                    + "</td><td>" + GetObjectRating(fTestNet, dt.Rows[i]["id"].ToString(), "comment1", gUser(z))
                    + "</td><td class='comments'><br>" + sBody + "</td></tr>";
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
            string allowed = "jpg;jpeg;gif;bmp;png;pdf;csv;mp4";
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
            string s = u.GetAvatarImage() + "<br>" + u.FullUserName();
            if (fHorizontal)
                s = u.GetAvatarImage() + " • " + u.FullUserName();
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
            string sTipButton = "<a title='Tip this channel' id='btnTip' onclick=\""
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
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "follow1");
            dt = dt.FilterDataTable("followedid='" + sFollowedID + "' and userid='" + sMyUserID + "' and deleted=0");
            if (dt.Rows.Count < 1)
                return "Follow";
            return "Unfollow";
        }

        public static double GetWatchSum(bool fTestNet, string sParentID)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "objectcount1");
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
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "objectcount1");
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
    }
}
