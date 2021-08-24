using System;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using static Unchained.Common;
using static BiblePayCommon.Common;
using System.IO;
using System.Text;
using System.Web.UI.WebControls;
using static BiblePayCommon.DataTableExtensions;

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


            //<button style='visibility:none;' type='submit' name='' id=''>
            //            string sPostAnchor = "<a href='javascript: void();' onclick='var z=document.getElementById(\"m1\");
            //z.value=\"\"; document.getElementById(\"m1\").click();'>" + sCustomHTML + "</a>";

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
       
        public static void MsgModal(Page p, string sTitle, string sNarrative, int nWidth, int nHeight)
        {
            string sJavascript = "showModalDialog(\"" + sTitle + "\",\"" + sNarrative + "\", " + nWidth.ToString() + ", " + nHeight.ToString() + ");";
            p.ClientScript.RegisterStartupScript(p.GetType(), "modalid1" + Guid.NewGuid().ToString(), sJavascript, true);
        }
        public static void MsgModalSM(Page p, string sTitle, string sNarrative, int nWidth, int nHeight)
        {
            string sJavascript = "showModalDialog(\"" + sTitle + "\",\"" + sNarrative + "\", " + nWidth.ToString() + ", " + nHeight.ToString() + ");";
            ScriptManager.RegisterStartupScript(p,p.GetType(), "modalid1" + Guid.NewGuid().ToString(), sJavascript, true);
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

        /*
        public static string GetTableB(string sTableName)
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
        */


        public static string GetFooter(Page p)
        {
            return "";
            /*
            string sOverridden = Common.Config("footer");
            if (sOverridden.Length > 0)
                return sOverridden;

            string sFooter = DateTime.Now.Year.ToString() + " - " + Common.GetLongSiteName(p);
            return sFooter;
            */
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


        public static string GetBioImg(string orphanid)
        {
            string sql = "Select BioURL from SponsoredOrphan where orphanid=@orphanid";
            SqlCommand command = new SqlCommand(sql);
            command.Parameters.AddWithValue("@orphanid", orphanid);
            string bio = gData.GetScalarString(command, "URL", false);
            return bio;
        }

        public static string GetTd(DataRow dr, string colname, string sDestination)
        {
            string sAnchor = "<a href='"+ sDestination + ".aspx?id=" + dr["id"].ToString() + "'>";
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

        public static string GetBBPAddress(Page p)
        {
            string sChain = IsTestNet(p) ? "test" : "main";
            string sPub = (p.Session[sChain + "_biblepayaddress"] ?? "").ToString();
            return sPub;
        }
        public static string GetAvatarBalance(Page p)
        {
            double? nVal = RetrieveSessionDouble(p, "balance");
            if (nVal == null)
            {
                string sPub = GetBBPAddress(p);
                if (sPub == "")
                    return "0.00";

                nVal = BiblePayDLL.Sidechain.QueryAddressBalance(IsTestNet(p), sPub);
                SetSessionDouble(p, "balance", (double)nVal);
            }

            return FormatUSD((double)nVal) + " BBP";
        }

        public static string GetSideBar(Page p)
        {
            item = 0;
            string sWallet = "<a style='color:gold;' onclick=\"document.getElementById('btnshowwallet').click();\">"
                +"<i class='fa fa-wallet'></i>&nbsp;"
                + GetAvatarBalance(p) + "</a>";
            string sChain = IsTestNet(p) ? "<font color=lime><small>TESTNET</small></font>" : "<font color=GOLD><small>MAINNET</small></font>";
            string sKeys = gUser(p).UserName.Length > 0 ? sWallet + " • " + sChain : "";
            string html = "<aside class='main-sidebar' id='mySidenav'>";
            html += "<section class='sidebar'><div class='user-panel' style='z-index: 9000;'>"
              + "<a onclick='closeNav();' href = '#' class='sidebar-toggle' data-toggle='offcanvas' role='button'>"
              + "<i class='fa fa-close'></i></a>"
              + "<div class='pull-left myavatar'>" + gUser(p).GetAvatarImage() + "</div>"
              + "<div class='pull-left info'><p>" + gUser(p).UserName + "</p>" + "</div><div class='myicons'><ul>" 
              + sKeys + "</ul></div>"
              + "</div><ul class='sidebar-menu'>";

            html += AddMenuOption("Home", "Default.aspx;"
                +"LandingPage?action=session&key=filetype&value=pdf&newpage=VideoList;"
                + "LandingPage?action=session&key=filetype&value=image&newpage=VideoList;"
                + "LandingPage?action=session&key=filetype&value=wiki&newpage=VideoList",
                   "Home;PDF List;Image List;Wikipedia", "fa-home");

            html += AddMenuOption("Video", "LandingPage?action=session&key=filetype&value=video&newpage=VideoList;"
                  + "LandingPage?action=session&key=filetype&value=mychannel&newpage=VideoList;"
                  + "LandingPage?action=session&key=filetype&value=following&newpage=VideoList;"
                  + "LandingPage?action=session&key=filetype&value=hashtags&newpage=VideoList;"
                  + "LandingPage?action=session&key=filetype&value=trending&newpage=VideoList;"
                  + "LandingPage?action=session&key=filetype&value=recentlyuploaded&newpage=VideoList"
                     , "Videos;My Channel;Following;My Hashtags;Trending;Recently Uploaded", "fa-video");

            html += AddMenuOption("Community", "TownHallList.aspx;PrayerBlog.aspx;UnchainedUpload.aspx;CreateNewDocument.aspx", 
                "Town Hall;Prayer Requests;Add New Video or Media;Add New Wikipedia Page", "fa-ambulance");

            html += AddMenuOption("Account", "RegisterMe", "My User Record[1]", "fa-unlock-alt");

            html += AddMenuOption("Status", "Status.aspx",
               "Status", "fa-sitemap");
            html += AddMenuOption("Demos", "ServiceProviderDemo.aspx;PetShop.aspx;ListView?objecttype=invoice1&filtertype=mine;"
                +"FormView.aspx?action=add&table=versememorizer1;ListView?objecttype=price1;PortfolioBuilder;Chat", 
                "Service Provider;Pet Shop;My Invoices;Add Scripture;Crypto Prices;Portfolio Builder;Chat Room", "fa-wrench");

            html += AddMenuOption("News", "NewsList.aspx;FormView.aspx?action=add&table=news1", "Unchained News;Add News Headline", "fa-newspaper");

            html += AddMenuOption("Gospel", "MemorizeScriptures.aspx;BBPUniversity.aspx;CoreGospel;BibleViewer", "Memorize Scriptures;BiblePay University;Core Beliefs;Bible Viewer", "fa-cross");

            html += "</section></aside>";

            
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
            dtUsers = dtUsers.FilterDataTable("BiblePayAddress='" + id + "'");
            User u = new User();
            if (dtUsers.Rows.Count < 1)
                return u;

            u.BiblePayAddress = dtUsers.GetColValue("BiblePayAddress");
            u.AvatarURL = dtUsers.GetColValue("AvatarURL");
            if (u.AvatarURL == String.Empty)
                u.AvatarURL = EmptyAvatar();

            u.EmailAddress = dtUsers.GetColValue("EmailAddress");
            u.UserName = dtUsers.GetColValue("UserName");
            u.Verified = dtUsers.GetColInt("Verified");
            return u;
        }
        public static string GetComments(bool fTestNet, string id, Page p)
        {
            // Shows the comments section for the object.  Also shows the replies to the comments.
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "comment1");
            
            dt = dt.FilterDataTable("parentid='" + id + "'");
           
            string sHTML = "<div><h3>Comments:</h3><br>"
                + "<table style='padding:10px;' width=73%>"
                + "<tr><th width=14%>User<th width=10%>Added<th>Rating<th width=64%>Comment</tr>";

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string sBody = ReplaceURLs(dt.Rows[i]["Body"].ToString());
                string div = "<tr><td>" + UICommon.GetUserAvatarAndName(p, dt.GetColValue(i, "UserID"))
                    + "<td>" + UnixTimeStampToDateTime(dt.GetColDouble(i, "time")).ToString()
                    + "</td><td>" + GetObjectRating(fTestNet, dt.Rows[i]["id"].ToString()) 
                    + "<td style='border:1px solid lightgrey'><br>" + sBody + "</td></tr>";
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


        public static string ExtensionToClassification(string ext)
        {
            string sClassification = "Unknown";
            if (ext == "jpg" || ext == "png" || ext == "jpeg" || ext == "bmp" || ext == "gif")
            {
                sClassification = "image";
            }
            else if (ext == "pdf")
            {
                sClassification = "pdf";
            }
            else if (ext == "mp4")
            {
                sClassification = "video";
            }
            else if (ext == "mp3")
            {
                sClassification = "audio";
            }
            else if (ext == "htm")
            {
                sClassification = "wiki";
            }
            return sClassification;
        }

        public static string GetUserAvatarAndName(Page p, string sUserID, bool fHorizontal = false)
        {
            User u = GetUserRecord(IsTestNet(p), sUserID);
            string s = u.GetAvatarImage() + "<br>" + u.UserName;
            if (fHorizontal)
                s = u.GetAvatarImage() + " • " + u.UserName;
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
            string[] vMenuItemCaptions = sMenuItemCaptions.Split(";");
            string[] vMenuItemActions = sMenuItemActions.Split(";");
            
            string htmlCMI = "";
            for (int i = 0; i < vMenuItemCaptions.Length; i++)
            {
                string sRow = "";
                sRow = "   '" + vMenuItemActions[i]  + "': {name: '" + vMenuItemCaptions[i] + "', icon: 'icon" + i.ToString() + "'},";
                htmlCMI += sRow;
            }

            string sContextEvent = " var sUSGDID = $(this)[0].getAttribute('unchainedid');__doPostBack('Event_ContextMenu_" + "_" + sID 
                + "_', key);";
            string sContextMenuCssClass = "context-menu-1";

            string sContextMenu = "  $(function() {   $.contextMenu({     selector: '." + sContextMenuCssClass + "',        callback: function(key, options) { " +
                "       " + sContextEvent + "            },"
               + "       items: {  " + htmlCMI + "                     }                    });"
               + "       $('." + sContextMenuCssClass + "').on('click', function(e){      console.log('clicked', this);        })        });";
            return "<script>" + sContextMenu + "</script>";
        }
        public static bool StoreCount(string parentID, Page p, string sCountType)
        {
            if (!gUser(p).LoggedIn)
            {
                return false;
            }
            BiblePayCommon.Entity.objectcount1 o = new BiblePayCommon.Entity.objectcount1();
            o.UserID = gUser(p).BiblePayAddress;
            o.ParentID = parentID;
            o.CountType = sCountType;
            o.CountValue = 1;
            o.UserID = gUser(p).BiblePayAddress;
           
            if (o.UserID == "")
                return false;
            double nWS = GetWatchSumUser(IsTestNet(p), parentID, gUser(p).BiblePayAddress);
            if (nWS > 0)
                return true;
            DataOps.InsertIntoTable_Background(IsTestNet(p), "objectcount1", o);
            return true;
        }

    }
}
