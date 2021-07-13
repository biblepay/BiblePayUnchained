using Microsoft.VisualBasic;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;

namespace Unchained
{

    public static class Common
    {
        public static Data gData = new Data(Data.SecurityType.REQ_SA);
        
        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }
        public static string GetLongSiteName(Page p)
        {
            return Config("longsitename");
        }

        public static string GetElement(string data, string delimiter, int n)
        {
            string[] vE = data.Split(delimiter);
            if (vE.Length > n)
            {
                return vE[n];
            }
            return "";
        }

        public static string RemoveCommas(string data)
        {
            string data1 = "";
            bool insidestr = false;
            for (int i = 0; i < data.Length; i++)
            {
                string ch = data.Substring(i, 1);
                if (ch == "\"")
                {
                    insidestr = !insidestr;
                }

                if (insidestr && ch == ",")
                    ch = "`";

                if (ch != "" && ch != "\"")
                {
                    data1 += ch;
                }
            }
            return data1;
        }
        public static string ReplaceURLs(string s)
        {
            s = s.Replace("\r\n", " <br><br>");

            string[] vWords = s.Split(" ");
            string sOut = "";
            for (int i = 0; i < vWords.Length; i++)
            {
                string v = vWords[i];
                if (v.Contains("https://"))
                {
                    v = v.Replace("<br>", "");

                    v = "<a target='_blank' href='" + v + "'><b>Link</b></a>";
                }
                sOut += v + " ";
            }
            return sOut;
        }
        public static string GetGlobalAlert(Page p)
        {
            return "";
        }

        public static string GetBaseHomeFolder()
        {
            string sHomePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
             Environment.OSVersion.Platform == PlatformID.MacOSX) ? Environment.GetEnvironmentVariable("HOME")
                 : Environment.ExpandEnvironmentVariables("%APPDATA%");
            sHomePath = "c:\\inetpub\\wwwroot\\";
            return sHomePath;
        }

        public static bool ToBool(object c)
        {
            if (c == null || c == DBNull.Value) return false;
            return Convert.ToBoolean(c);
        }

        public static string FormatUSD(double myNumber)
        {
            var s = string.Format("{0:0.00}", myNumber);
            return s;
        }

        public static void MsgBox(string sTitle, string sBody, System.Web.UI.Page p)
        {
            p.Session["MSGBOX_TITLE"] = sTitle;
            p.Session["MSGBOX_BODY"] = sBody;
            p.Response.Redirect("MessagePage.aspx");
        }

        public static string NotNull(object o)
        {

            if (o == null || o == DBNull.Value) return "";
            return o.ToString();
        }

        public static string GetSha256HashNoSpaces(string rawData)
        {
            rawData = rawData.Replace(" ", "");
            string s = GetSha256HashI(rawData);
            return s;
        }
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

        public static bool IsPasswordStrong(string pw)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");
            var isValidated = hasNumber.IsMatch(pw) && hasUpperChar.IsMatch(pw) && hasMinimum8Chars.IsMatch(pw);
            return isValidated;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string GetPlatformMoniker()
        {
            string sMoniker = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) ? "LIN" : "WIN";
            return sMoniker;
        }
        public static string GetPathDelimiter()
        {
            string sPathDelimiter = (Environment.OSVersion.Platform == PlatformID.Unix || Environment.OSVersion.Platform == PlatformID.MacOSX) ? "/" : "\\";
            return sPathDelimiter;
        }

        public static string GetFolderUnchained(string sType)
        {
            string sPath = "c:\\inetpub\\wwwroot\\Unchained\\" + sType;
            return sPath;
        }

        
        private static int iRowModulus = 0;
        private static object cs_log = new object();
        private static string mLastLogData = "";
        public static void Log(string sData, bool fQuiet = false)
        {
            lock (cs_log)
            {
                {
                    try
                    {
                        if (sData == mLastLogData)
                            return;
                        iRowModulus++;
                        if ((fQuiet && iRowModulus % 10 == 0) || (!fQuiet))
                        {
                            mLastLogData = sData;
                            string sPath = GetFolderUnchained("unchained.log");
                            System.IO.StreamWriter sw = new System.IO.StreamWriter(sPath, true);
                            string Timestamp = DateTime.Now.ToString();
                            sw.WriteLine(Timestamp + ": " + sData);
                            sw.Close();
                        }
                    }

                    catch (Exception ex)
                    {
                        string sMsg = ex.Message;
                    }
                }
            }
        }

        public static string GetExtConfigurationKeyValue(string sPath, string _Key)
        {
            if (!File.Exists(sPath))
                return string.Empty;

            string sData = System.IO.File.ReadAllText(sPath);
            string[] vData = sData.Split("\n");
            for (int i = 0; i < vData.Length; i++)
            {
                string sEntry = vData[i];
                sEntry = sEntry.Replace("\r", "");
                string[] vRow = sEntry.Split("=");
                if (vRow.Length >= 2)
                {
                    string sKey = vRow[0];
                    string sValue = vRow[1];
                    if (sKey.ToUpper() == _Key.ToUpper())
                        return sValue;
                }
            }
            return string.Empty;
        }
        
        public static string Mid(string data, int nStart, int nLength)
        {
            // Ported from VB6, except this version is 0 based (NOT 1 BASED)
            if (nStart > data.Length)
            {
                return "";
            }

            int nNewLength = nLength;
            int nEndPos = nLength + nStart;
            if (nEndPos > data.Length)
            {
                nNewLength = data.Length - nStart;
            }
            if (nNewLength < 1)
                return "";

            string sOut = data.Substring(nStart, nNewLength);
            if (sOut.Length > nLength)
            {
                sOut = sOut.Substring(0, nLength);
            }
            return sOut;
        }

        public static string GetFundingAddress(bool fTestNet)
        {
            string suffix = fTestNet ? "testnet" : "mainnet";
            string sKey = "FundingAddress_" + suffix;
            return Config(sKey);
        }

        public static string GetFundingKey(bool fTestNet)
        {
            string suffix = fTestNet ? "testnet" : "mainnet";
            string sKey = "FundingKey_" + suffix;
            return Config(sKey);
        }
        public static string Config(string _Key)
        {
            string sKV = GetExtConfigurationKeyValue(GetBaseHomeFolder() + "unchained.conf", _Key);
            return sKV;
        }

        public static Int32 instr(Int32 StartPos, String SearchString, String SearchFor, Int32 IgnoreCaseFlag)
        {
            Int32 result = -1;
            if (IgnoreCaseFlag == 1)
                result = SearchString.IndexOf(SearchFor, StartPos, StringComparison.OrdinalIgnoreCase);
            else
                result = SearchString.IndexOf(SearchFor, StartPos);
            return result;
        }
        public static string ExtractXML(string sData, string sStartKey, string sEndKey)
        {
            if (sData == null)
                return string.Empty;

            int iPos1 = (sData.IndexOf(sStartKey, 0) + 1);
            if (iPos1 < 1)
                return string.Empty;

            iPos1 = (iPos1 + sStartKey.Length);
            int iPos2 = (sData.IndexOf(sEndKey, (iPos1 - 1)) + 1);
            if ((iPos2 == 0))
            {
                return String.Empty;
            }
            string sOut = sData.Substring((iPos1 - 1), (iPos2 - iPos1));
            return sOut;
        }
       
        public static bool ByteArrayToFile(string fileName, byte[] byteArray)
        {
            try
            {
                using (var fs = new FileStream(fileName, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(byteArray, 0, byteArray.Length);
                    return true;
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine("Exception caught in process: {0}", ex);
                return false;
            }
        }
        public static string Left(string source, int iHowMuch)
        {
            if (source.Length < iHowMuch)
                return source;
            return source.Substring(0, iHowMuch);
        }

        public struct User
        {
            public string UserName;
            public string AvatarURL;
            public bool LoggedIn;
        }

        public static User gUser(Page p)
        {
            User u = new User();

            u.UserName = GetLocalStorage(p, "NickName");
            u.AvatarURL = "<img src='images/emptyavatar.png' width=50 height=50 >";
            u.LoggedIn = u.UserName.Length > 2;
            return u;
        }

        public static string EmptyAvatar()
        {
            string s = "<img src='images/emptyavatar.png' width=50 height=50 >";
            return s;

        }     

        public static double GetCompounded(double nROI)
        {
            double nBank = 10000;
            for (int nMonth = 1; nMonth <= 12; nMonth++)
            {
                double nReward = nBank * (nROI / 12);
                nBank += nReward;
            }
            double nCompounded = -1 * Math.Round(1 - (nBank / 10000), 2);
            return nCompounded;
        }

        public static bool IsTestNet(Page p)
        {
            double nChain  = BiblePayDLL.Shared.GetDouble(p.Session["mainnet_mode"]);
            bool fProd = nChain == 1;
            return !fProd;
        }

        public static string GetBBPAddressCookieName(Page p)
        {
            string sName = IsTestNet(p) ? "bbpaddress_testnet" : "bbpaddress_prod";
            return sName;
        }

        public static string GetBBPAddressPKCookieName(Page p)
        {
            string sName = IsTestNet(p) ? "bbpprivkey_testnet" : "bbpprivkey_prod";
            return sName;
        }

        public static string GetBBPEncFlagCookieName(Page p)
        {
            string sName = IsTestNet(p) ? "bbpencflag_testnet" : "bbpencflag_prod";
            return sName;
        }

        public static string Base65Encode(string sData)
        {
            string s1 = Base64Encode(sData);
            string s2 = s1.Replace("=", "[equal]");
            return s2;
        }

        public static string Base65Decode(string sData)
        {
            string s1 = sData.Replace("[equal]", "=");
            string s2 = Base64Decode(s1);
            return s2;
        }

        public static void DeleteCookie(string sKey)
        {
            try
            {
                HttpContext.Current.Response.Cookies.Remove(sKey);
            }
            catch(Exception)
            {

            }
        }
        public static string GetLocalStorage(Page p, string sKey1)
        {
            string sPrefix = IsTestNet(p) ? "testnet" : "mainnet";
            string sKey = sPrefix + "_" + sKey1;

            string[] vStorage = p.Session["localStorage"].ToNonNullString().Split("<ROW>");
            for (int i = 0; i < vStorage.Length; i++)
            {
                string[] rowStorage = vStorage[i].Split("<COL>");
                if (rowStorage.Length > 1)
                {
                    if (rowStorage[0] == sKey)
                        return rowStorage[1];
                }
            }
            return "";
        }

        public static string xGetCookie(string sKey)
        {
            try
            {
                HttpCookie c = HttpContext.Current.Request.Cookies[cookiePrefix + sKey];
                if (c != null)
                {
                    //string sOut = (c.Value ?? string.Empty).ToString();
                    string sOut = (c.Values[cookiePrefix + sKey] ?? string.Empty).ToString();

                    string sDeciphered = Base65Decode(sOut);
                    return sDeciphered;
                }
            }
            catch (Exception)
            {

            }
            return "";
        }

        public static string TriggerFormSubmit(Page p, string sURL, bool fWrite = true, string sOptData = "")
        {
            string sPA = "document.forms[0].action=\"" + sURL
                + "\"; document.forms[0].submit(); ";
            if (fWrite)
            {
                string sOut = "<script language='javascript'>" + sPA + "</script>";
                p.Response.Write(sOut);
                p.Response.Write(sOptData);
            }
            return sPA;
        } 

        public static string SetLocalStorage(Page p, string key1, string value1)
        {
            bool fTestNet = IsTestNet(p);
            string sPrefix = fTestNet ? "testnet" : "mainnet";
            string sKey = sPrefix + "_" + key1;
            string value = Base65Encode(value1);
            string sScript = "<script language='javascript'>localStorage.setItem('" + sKey + "','" + value1 + "')</script>";
            p.Response.Write(sScript);
            return sScript;
        }
        public static string cookiePrefix = "cred12_";

        public static void xSetCookie(string key1, string value1, TimeSpan expires)
        {
            string key = cookiePrefix + key1;
            string value = Base65Encode(value1);
            var cookie = new System.Web.HttpCookie(key);
            cookie.Values[key] = value;
            cookie.Expires = DateTime.Now.Add(expires);
            cookie.SameSite = System.Web.SameSiteMode.None;
            cookie.Secure = true;
            HttpContext.Current.Response.Cookies.Add(cookie);
        }


        public static double GetDouble(object o)
        {
            try
            {
                if (o == null) return 0;
                if (o.ToString() == string.Empty) return 0;
                double d = Convert.ToDouble(o.ToString());
                return d;
            }
            catch (Exception)
            {
                // Letters
                return 0;
            }
        }

    }
}
