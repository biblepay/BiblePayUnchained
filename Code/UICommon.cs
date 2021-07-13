using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using static Unchained.Common;

namespace Unchained
{
    public static class UICommon
    {

        private static int item = 0;
        private static string AddMenuOption(string MenuName, string URLs, string LinkNames, string sIcon)
        {
            double nEnabled = GetDouble(Config(MenuName));
            if (nEnabled == -1)
                return "";

            string[] vURLs = URLs.Split(";");
            string[] vLinkNames = LinkNames.Split(";");
            

            var js2 = "   var xp = parseFloat(localStorage.getItem('bbpdd" + item.ToString() + "')); "
             + "   var xe = xp==0?1:0; localStorage.setItem('bbpdd" + item.ToString() + "', xe); var disp = xp == 0 ? 'none' : 'block';";

            var js3 = "   var xp = parseFloat(localStorage.getItem('bbpdd" + item.ToString() + "')); "
             + "   var xe = xp==0?1:0; var disp = xe == 0 ? 'none' : 'block';";

            string menu = "<li id ='button_" + MenuName + "' class='dropdown'>"
             + "	<a class='dropdown-toggle' href='#' data-toggle='dropdown' onclick=\"" + js2 + " $('#bbpdd" + item.ToString() + "').attr('expanded', xe); "
             + "     $('#bbpdd" + item.ToString() + "').css('display',disp);\" >"
             + "	<i class='fa " + sIcon + "'></i>&nbsp;<span>" + MenuName + "</span>"
             + "	<span class='pull-right-container'><i class='fa fa-angle-left pull-right'></i></span></a>"
             + "	<ul class='treeview-menu' id='bbpdd" + item.ToString() + "'><script>" + js3 + "$('#bbpdd" + item.ToString() + "').css('display',disp);</script>";


            //<button style='visibility:none;' type='submit' name='bpag1' id='bpag1'>
            //            string sPostAnchor = "<a href='javascript: void();' onclick='var z=document.getElementById(\"m1\");
            //z.value=\"12345\"; document.getElementById(\"m1\").click();'>" + sCustomHTML + "</a>";

            for (int i = 0; i < vLinkNames.Length; i++)
            {
                bool fPost = vLinkNames[i].Contains("[1]");
                if (!fPost)
                {
                    menu += "<li><a href='" + vURLs[i] + "'><span style='overflow:visibile;'>" + vLinkNames[i] + "</span></a></li>";
                }
                else
                {
                    string sLinkName = vLinkNames[i];
                    sLinkName = sLinkName.Replace("[1]", "");
                    string sPostAnchor = "<li><a href='javascript: void();' onclick='document.forms[0].action=\"" + vURLs[i]
                        + "\"; document.forms[0].submit(); '>"
                        + "<span style='overflow:visibile;'>" + sLinkName + "</span></a></li>";
                    menu += sPostAnchor;
                }
            }
            menu += "</ul></li>";
            item++;
            return menu;
        }
       
        public static string RenderGauge(int width, string Name, int value)
        {
            string s = "<div id='chart_div'></div><script type=text/javascript> google.load( *visualization*, *1*, {packages:[*gauge*]});"
                + "     google.setOnLoadCallback(drawChart);"
                + "      function drawChart() {"
                + "      var data = new google.visualization.DataTable();"
                + "      data.addColumn('string', 'item');"
                + "      data.addColumn('number', 'value');     "
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

        public static string GetTableBeginning(string sTableName)
        {
            string css = "<style> html {    font-size: 1em;    color: black;    font-family: verdana }  .r1 { font-family: verdana; font-size: 10; }</style>";
            string logo = "https://www.biblepay.org/wp-content/uploads/2018/04/Biblepay70x282_96px_color_trans_bkgnd.png";
            string sLogoInsert = "<img width=300 height=100 src='" + logo + "'>";
            string HTML = "<HTML>" + css + "<BODY><div><div style='margin-left:12px'><TABLE class=r1><TR><TD width=70%>" + sLogoInsert
                + "<td width=25% align=center>" + sTableName + "</td><td width=5%>" + DateTime.Now.ToShortDateString() + "</td></tr>";
            HTML += "<TR><TD><td></tr>" + "<TR><TD><td></tr>" + "<TR><TD><td></tr>";
            HTML += "</table>";
            return HTML;
        }

        public static string GetFooter(Page p)
        {
            string sOverridden = Common.Config("footer");
            if (sOverridden.Length > 0)
                return sOverridden;

            string sFooter = DateTime.Now.Year.ToString() + " - " + Common.GetLongSiteName(p);
            return sFooter;

        }
        public static string GetHeaderImage(Page p)
        {
            if (p.Request.Url.OriginalString.ToLower().Contains("saved"))
            {
                return "images/SavedOneLogo.png";
            }
            else
            {
                return "Images/bbphoriz.png";
            }
        }


        private static string GenPagI1(string sValue, string sCustomHTML, string sClass)
        {
            string a = "<a " + sClass + " href='javascript: void();' onclick='var z=document.getElementById(\"hpag1\"); z.value=\"" + sValue + "\"; document.getElementById(\"bpag1\").click();'>" + sCustomHTML + "</a>";
            return a;
        }
        public static string GetPagControl(Page p, double iTotalPages)
        {
            var uri = new Uri(p.Request.Url.AbsoluteUri);
            string sURL = uri.GetLeftPart(UriPartial.Path);


            int iActivePage = (int)GetDouble(p.Session["pag_" + sURL] ?? "");

            if (iActivePage > iTotalPages)
            {
                iActivePage = (int)iTotalPages;
            }


            if (iActivePage < 0)
                iActivePage = 0;
            int iPriorPage = iActivePage - 2;
            if (iPriorPage < 0)
                iPriorPage = 0;

            int iNextPage = iActivePage + 0;
            if (iNextPage >= iTotalPages)
                iNextPage = (int)iTotalPages - 1;

            string pag = "<div class='pagination'><button style='visibility:none;' type='submit' name='bpag1' id='bpag1'>"
                + "<input type='hidden' name='hpag1' value='0' id='hpag1'/>";

            pag += GenPagI1("0", "&laquo;", "");


            pag += GenPagI1((iPriorPage).ToString(), "&larr;", "");

            int iPos = 0;
            int iStartPage = iActivePage - (1 / 2);
            if (iStartPage < 1) iStartPage = 1;

            for (int i = iStartPage; i <= iTotalPages; i++)
            {
                string sActive = "";
                if ((i - 0) == iActivePage)
                    sActive = "class='active'";
                string sRow = GenPagI1((i - 1).ToString(), i.ToString(), sActive);
                pag += sRow;
                iPos++;
            }
            //Next Page
            pag += GenPagI1((iNextPage).ToString(), "&rarr;", "");

            //Last Page
            pag += GenPagI1((iTotalPages-1).ToString(), "&raquo;", "");

            return pag;
        }
        public static string GetBioImg(string orphanid)
        {
            string sql = "Select BioURL from SponsoredOrphan where orphanid=@orphanid";
            SqlCommand command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@orphanid", orphanid);
            string bio = gData.GetScalarString(command, "URL", false);
            return bio;
        }

        public static string GetTd(DataRow dr, string colname, string sAnchor)
        {
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
            p.Session[keyname + "_age"] = BiblePayDLL.Shared.UnixTimeStamp().ToString();
        }

        public static double? RetrieveSessionDouble(Page p, string keyname)
        {
            if (p.Session[keyname] == null)
            {
                return null;
            }
            double nAge = BiblePayDLL.Shared.UnixTimeStamp() - GetDouble(p.Session[keyname + "_age"].ToNonNullString());
            if (nAge > 60 * 15)
            {
                return null;
            }

            double nBal = GetDouble(p.Session["balance"].ToNonNullString());
            return nBal;
        }

        public static string GetAvatarBalance(Page p)
        {
            double? nVal = RetrieveSessionDouble(p, "balance");
            if (nVal == null)
            {
                BiblePayDLL.Data b = new BiblePayDLL.Data();
                string sPub = GetLocalStorage(p, "bbp_address");
                if (sPub == "")
                    return "0.00";

                nVal = BiblePayDLL.Data.QueryAddressBalance(IsTestNet(p), sPub);
                SetSessionDouble(p, "balance", (double)nVal);
            }

            return FormatUSD((double)nVal) + " BBP";
        }

        public static string GetSideBar(Page p)
        {
            item = 0;
            string sBalance = "<li>" + GetAvatarBalance(p) + "</li>";
            string sChain = IsTestNet(p) ? "<font color=lime><small>TESTNET</small></font>" : "<font color=GOLD><small>MAINNET</small></font>";
            string sKeys = gUser(p).UserName.Length > 0 ? "<li><a href='RegisterMe.aspx'><i class='fa fa-key'></i></a></li>" + sBalance + " • " + sChain : "";
            string html = "<aside class='main-sidebar' id='mySidenav'>";
            html += "<section class='sidebar'><div class='user-panel' style='z-index: 9000;'>"
              + "<a onclick='closeNav();' href = '#' class='sidebar-toggle' data-toggle='offcanvas' role='button'>"
              + "<i class='fa fa-close'></i></a>"
              + "<div class='pull-left myavatar'>" + gUser(p).AvatarURL + "</div>"
              + "<div class='pull-left info'><p>" + gUser(p).UserName + "</p>" + "</div><div class='myicons'><ul>" + sKeys + "</ul></div>"
              + "</div><ul class='sidebar-menu'>";

            html += AddMenuOption("Home", "Default.aspx;LandingPage?action=session&key=filetype&value=video&newpage=VideoList;LandingPage?action=session&key=filetype&value=pdf&newpage=VideoList;"
                + "LandingPage?action=session&key=filetype&value=image&newpage=VideoList", 
                   "Home;Video List;PDF List;Image List", "fa-home");

            html += AddMenuOption("Community", "PrayerBlog.aspx;PrayerAdd.aspx;UnchainedUpload.aspx", "Prayer Requests List Blog;Add New Prayer Request;Add New Video or Media", "fa-ambulance");
            html += AddMenuOption("Account", "RegisterMe", "Account[1]", "fa-unlock-alt");



            html += "</section></aside>";
            return html;
        }

        public static string GetComments(string id, Page p)
        {
            // Shows the comments section for the object.  Also shows the replies to the comments.
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "comment1", "", id, "body,username,time", "", "");

            string sHTML = "<div><h3>Comments:</h3><br>"
                + "<table style='padding:10px;' width=73%>"
                + "<tr><th width=14%>User<th width=10%>Added<th width=64%>Comment</tr>";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sUserPic = EmptyAvatar();
                string sBody = ReplaceURLs(dt.Rows[i]["Body"].ToString());
                string div = "<tr><td>" + sUserPic + "<br>" + dt.Rows[i]["UserName"].ToString() + "</br></td><td>" + dt.Rows[i]["time"].ToString() 
                    + "</td><td style='border:1px solid lightgrey'><br>" + sBody + "</td></tr>";
                sHTML += div;

            }
            sHTML += "</table><table width=100%><tr><th colspan=2><h3>Add a Comment:</h3></tr>";

            if (!gUser(p).LoggedIn)
            {
                sHTML += "<tr><td><font color=red>Sorry, you must be logged in to add a comment.</td></tr></table></div>";
                return sHTML;
            }

            string sButtons = "<tr><td width=10%>Comment:</td><td><textarea id='txtComment' name='txtComment' rows=10  style='width: 70%;' cols=10></textarea><br><br>"
                +"<button id='btnSaveComment' name='btnSaveComment' value='Save'>Save Comment</button></tr>";
            sButtons += "</table></div>";
            sHTML += sButtons;
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
    }
}
