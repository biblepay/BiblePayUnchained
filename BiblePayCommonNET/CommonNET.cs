using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BiblePayCommonNET
{
    public static class CommonNET
    {
        public static string GetSha256HashI(string rawData)
        {
            // The I means inverted (IE to match a uint256)
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = bytes.Length - 1; i >= 0; i--)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string GetSha256Hash(string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        public static string DoubleToString(double nDouble, int nPlaces)
        {
            double nNewDouble = Math.Round(nDouble, nPlaces);
            string sData = ((decimal)nNewDouble).ToString();
            return sData;
        }

        public struct BBPEvent
        {
            public string EventName;
            public string EventArgs;
            public string EventRaw;
            public string EventValue;
            public double EventAmount;
            public string EventID;
            public string EventRedirect;
            public string MicrosoftTarget;
            public string MicrosoftArgs;
            public string EventAction;
            public string TargetControl;
            public string SourceControl;
            public dynamic Extra;
            public int Iteration;
        }

        public static double GetSessionDouble(Page p, string sKey)
        {
            string sValue = (p.Session[sKey] ?? "").ToString();
            double nValue = BiblePayCommon.Common.GetDouble(sValue);
            return nValue;
        }
        private struct IKeySet
        {
            public string PrivateKey;
            public string PublicKey;
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
            double nAge = BiblePayCommon.Common.UnixTimeStamp() - BiblePayCommon.Common.GetDouble(p.Session[keyname + "_age"]);
            if (nAge > 60 * 15)
            {
                return null;
            }

            double nBal = BiblePayCommon.Common.GetDouble(p.Session["balance"]);
            return nBal;
        }
    }

    public static class UICommonNET
    {
        static Regex MobileCheck = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows ce|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        static Regex MobileVersionCheck = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.Compiled);
        public static bool fBrowserIsMobile(Page p)
        {
            if (p.Request != null && p.Request.ServerVariables["HTTP_USER_AGENT"] != null)
            {
                var u = p.Request.ServerVariables["HTTP_USER_AGENT"].ToString();

                if (u.Length < 4)
                    return false;

                if (MobileCheck.IsMatch(u) || MobileVersionCheck.IsMatch(u.Substring(0, 4)))
                    return true;
            }
            return false;
        }
        public static void MsgModal(Page p, string sTitle, string sNarrative, int nWidth, int nHeight, bool fUseScriptManager = true, bool fRedirectToPage = false)
        {
            sTitle = BiblePayCommon.Encryption.CleanModalString(sTitle);
            sNarrative = sNarrative.Replace("\r\n", "<br>");
            sNarrative = sNarrative.Replace("'", "&apos;");
            sNarrative = sNarrative.Replace("\"", "&quot;");
            string sRedirect = fRedirectToPage ? "true" : "false";
            string sJavascript = "showModalDialog(\"" + sTitle + "\",\"" + sNarrative + "\", " + nWidth.ToString() + ", " + nHeight.ToString() + "," 
                + sRedirect + ");";
            if (fUseScriptManager)
                ScriptManager.RegisterStartupScript(p, p.GetType(), "modalid1" + Guid.NewGuid().ToString(), sJavascript, true);
            else
                p.ClientScript.RegisterStartupScript(p.GetType(), "modalid1" + Guid.NewGuid().ToString(), sJavascript, true);
        }


        public static void MsgModalWithLinks(Page p, string sTitle, string sNarrative, int nWidth, int nHeight, bool fUseScriptManager = true, bool fShowCancelButton = false)
        {
            sTitle = BiblePayCommon.Encryption.CleanModalString(sTitle);
            sNarrative = sNarrative.Replace("\r\n", "<br>");
            string sJavascript = "";
            if (fShowCancelButton)
            {
                sJavascript = "function closeModalDialog() { $('#divdialog').dialog('close');  }"
                + "showModalDialogWithCancel(\"" + sTitle + "\",\"" + sNarrative + "\", " + nWidth.ToString() + ", " + nHeight.ToString() + ");";
            }
            else
            {
                sJavascript = "function closeModalDialog() { $('#divdialog').dialog('close');  }"
                    + "showModalDialog(\"" + sTitle + "\",\"" + sNarrative + "\", " + nWidth.ToString() + ", " + nHeight.ToString() + ",\"\");";
            }
            if (fUseScriptManager)
                ScriptManager.RegisterStartupScript(p, p.GetType(), "modalid1" + Guid.NewGuid().ToString(), sJavascript, true);
            else
                p.ClientScript.RegisterStartupScript(p.GetType(), "modalid1" + Guid.NewGuid().ToString(), sJavascript, true);
        }

        public static void ModalEmpty(Page p, string sCallerDivID,  string sTitle, string sHTML, int nWidth, int nHeight, bool fUseScriptManager = true, bool fUpdate = false , string sOptJS = "")
        {
            string sJavascript = "";
            sHTML = sHTML.Replace("'", "\\'");
            sHTML = sHTML.Replace("\"", "\\\"");
            sHTML = sHTML.Replace("\n", "<br>");
            sHTML = sHTML.Replace("\r", "");

            if (fUpdate)
            {
                sJavascript = sOptJS + "updateModalEmptyDialog(\"" + sHTML + "\");";
            }
            else
            {
                sJavascript = sOptJS + "showModalEmptyDialog(\"" + sCallerDivID + "\",\"" + sTitle + "\",\"" + sHTML + "\", " + nWidth.ToString() + ", " + nHeight.ToString() + ",\"\");";
            }
            if (fUseScriptManager)
                ScriptManager.RegisterStartupScript(p, p.GetType(), "modalid1" + Guid.NewGuid().ToString(), sJavascript, true);
            else
                p.ClientScript.RegisterStartupScript(p.GetType(), "modalid1" + Guid.NewGuid().ToString(), sJavascript, true);
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
        public static void ToastNow(Page p, string sTitle, string sBody)
        {
            string sToast = Toast(sTitle, sBody);
            RunScriptSM(p, sToast);
            // p.Session["stack"] = Toast(sTitle, sBody);
        }

        public static void ToastLater(Page p, string sTitle, string sBody)
        {
            p.Session["stack"] = Toast(sTitle, sBody);
        }
        public static string FormatUSD(double myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);
            return s;
        }

        public static void RunScriptSM(Page p, string sJavascript)
        {
            ScriptManager.RegisterStartupScript(p, p.GetType(), "script" + Guid.NewGuid().ToString(), sJavascript, true);
        }

        public static void SetSessionDouble1(Page p, string key, double nValue)
        {
            p.Session[key] = nValue;
        }

        public static bool IsTestNet0(Page p)
        {
            double nChain = BiblePayCommon.Common.GetDouble(p.Session["mainnet_mode"]);
            bool fProd = nChain != 2;
            return !fProd;
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

        public static string GetButton0(string sID, string sEventValue, string sCaption, string sOnClientClick = "", string sCSSClass = "")
        {
            string sHTML = "<input type='hidden' id='event" + sID + "' name='event" + sID + "' value='' />";
            Button btn1 = new Button();
            btn1.Text = sCaption;
            btn1.ID = sID;
            btn1.CssClass = sCSSClass;
            opass o1 = new opass();
            o1.eventname = sEventValue;
            Random r = new Random();
            o1.niteration = r.Next(1000);
            string sData = Newtonsoft.Json.JsonConvert.SerializeObject(o1);
            btn1.OnClientClick = "var e=document.getElementById('event" + sID + "');e.value='" + sData + "';" + sOnClientClick;
            string sBtn1 = RenderControl(btn1);
            sHTML += sBtn1;
            return sHTML;
        }

        public struct opass
        {
            public string eventname;
            public int niteration;
        }

        public static string GetButtonTypeSubmit(string sID, string sEventName, string sCaption, string sJS="", string sEventValue = "")
        {
            string sHTML = "";
            opass o1 = new opass();
            o1.eventname = sEventValue;
            Random r = new Random();
            o1.niteration = r.Next(1000);
            string sData = Newtonsoft.Json.JsonConvert.SerializeObject(o1);
            sData = sData.Replace("\"", "\\\"");
            string sOnClientClick = sJS + "var e={};e.Value=\"" + sEventValue + "\";e.Event=\"" + sEventName + "\";BBPPostBack2(this,e);";
            string sButton = "<button id='" + sID + "' name='" + sID + "' type='reset' value='Submit' onclick='" + sOnClientClick + "'>" + sCaption + "</button>";
            sHTML += sButton;
            return sHTML;
        }
    }

    public class Reserved
    {
        // This is a dialog that includes the JS jquery function inline (which is normally in the master page permanently).. We are not using this, but Im leaving in case we need an example of one.
        public static void showmodaldialogReserved(Page p, string sTitle, string sBody, int nWidth, int nHeight)
        {
            string js = " <div id='divdialog2' title='' style='background-color:grey;color:gold;'><span id='spandialog2'></span></div>"
                + "<script> function showModalDialog2(title, body, width, height)            {             $('#divdialog2').dialog({                    'body': body,"
                   + "                'title': title,                'show': true,'width': width,  'height': height,   buttons:       {"
                   + "          OK: function() { $(this).dialog('close'); }       },       });      var e = document.getElementById('spandialog2');    e.innerHTML = body;        }</script>";
            p.ClientScript.RegisterStartupScript(p.GetType(), "modalid100", js, false);
            string js2 = "showModalDialog2('" + sTitle + "','" + sBody + "'," + nWidth.ToString() + "," + nHeight.ToString() + ");";
            p.ClientScript.RegisterStartupScript(p.GetType(), "modalid" + Guid.NewGuid().ToString(), js2, true);
        }
    }
}
