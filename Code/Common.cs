using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;

namespace Unchained
{
    public static class Common
    {
        public static Data gData = new Data(Data.SecurityType.REQ_SA);
        public static string _cancelurl = "";

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
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
            } catch (Exception)
            {
                return String.Empty;
            }
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
            public string BiblePayAddress;
            public string EmailAddress;
            public int Verified;
            public string HashTags;
            public string Slogan;
            public string Testimony;

            public string GetAvatarImage()
            {

                if (AvatarURL.ToNonNullString() == "" || AvatarURL.Contains("emptyavatar"))
                {
                    return EmptyAvatar();
                }
                else
                {
                    return "<img src='" + AvatarURL + "' width=50 height=50>";
                }

            }
        }

        public static User gUser(Page p)
        {
            User u = new User();
            u.UserName = "Guest";
            if (p.Session["mainnet_mode"] == null)
            {
                p.Session["mainnet_mode"] = 1;
            }
            string sKey = UICommon.GetBBPAddress(p);
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "user1");
            dt = dt.FilterDataTable("biblepayaddress='" + sKey + "'");
            if (dt.Rows.Count < 1)
                return u;

            u.UserName = dt.Rows[0]["UserName"].ToString();
            u.BiblePayAddress = dt.Rows[0]["BiblePayAddress"].ToString();
            u.EmailAddress = dt.GetColValue("EmailAddress");
            u.Verified = dt.GetColInt("Verified");
            u.Testimony = dt.GetColValue("Testimony");
            u.Slogan = dt.GetColValue("Slogan");
            u.AvatarURL = dt.GetColValue("AvatarURL");
            u.LoggedIn = u.UserName.Length > 1 && u.BiblePayAddress.Length > 24;
            u.HashTags = dt.GetColValue("HashTags");
            return u;
        }
        public static string EmptyAvatar()
        {
            string s1 = "<img src='images/emptyavatar.png' width=50 height=50>";
            return s1;
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
            double nChain = GetDouble(p.Session["mainnet_mode"]);
            bool fProd = nChain != 2;
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

        public static int GetVoteCount(bool fTestNet, string sID, int nType)
        {
            VoteSums v = GetVoteSum(fTestNet, sID);
            if (nType == 0)
                return (int)v.nUpvotes;
            if (nType == 1)
                return (int)v.nDownvotes;
            return 0;
        }

        public static string GetObjectRating(bool fTestNet, string sID)
        {
            string sHTML = "";
            int nRating = 3;
            string sChecked = "";
            for (int i = 1; i <= 5; i++)
            {
                sChecked = "";
                if (i <= nRating)
                    sChecked = "checked";
                sHTML += "<span class='fa fa-star " + sChecked + "'></span>";
            }
            // Voting buttons
            sHTML = "";
            sHTML += "<a onclick='transmit(\"" + sID + "\", \"upvote\", \"upvote1" + sID + "\", \"downvote1" + sID + "\");'>"
                + "<span class='fa fa-thumbs-up'></span></a>"
                + "&nbsp;"
                + "<span id='upvote1" + sID + "'>" + GetVoteCount(fTestNet, sID, 0).ToString() + "</span>"
                + " &nbsp; "
                + "<a onclick='transmit(\"" + sID + "\", \"downvote\", \"upvote1" + sID + "\", \"downvote1" + sID + "\");'>"
                + "<span class='fa fa-thumbs-down'></span></a>&nbsp;"
                + "<span id='downvote1" + sID + "'>"
                + GetVoteCount(fTestNet, sID, 1).ToString() + "</span>";
            return sHTML;
        }

        public static string GetFollowControl(bool fTestNet, string sFollowedID, string sMyUserID)
        {
            string sHTML = "";
            string sStatus = GetFollowStatus(fTestNet, sFollowedID, sMyUserID);
            string sAction = sStatus == "Follow" ? "follow" : "unfollow";
            string sIcon = sStatus == "Follow" ? "fa-heart" : "fa-heart-broken";
            sHTML += "<a onclick='var o=document.getElementById(\"follow1" 
                + sFollowedID + "\");transmitfollow(\"" + sFollowedID + "\", o.innerHTML, \"follow1" 
                + sFollowedID + "\", \"\");'>"
                + "<span id='span1" + sFollowedID + "' class='fa " + sIcon + "'></span></a>"
                + "&nbsp;"
                + "<span id='follow1" + sFollowedID + "'>" + sStatus + "</span>";
            return sHTML;

        }

        public static void DeleteCookie(string sKey)
        {
            try
            {
                HttpContext.Current.Response.Cookies.Remove(sKey);
            }
            catch (Exception)
            {

            }
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

        public static string cookiePrefix = "cred12_";

        public struct VoteSums
        {
            public double nUpvotes;
            public double nDownvotes;
            public double nAvg;
        };
        public static VoteSums GetVoteSum(bool fTestNet, string sParentID)
        {
            VoteSums v = new VoteSums();

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "vote1");
            dt = dt.FilterDataTable("parentid='" + sParentID + "'");
            if (dt.Rows.Count == 0)
                return v;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double nVoteValue = GetDouble(dt.Rows[i]["VoteValue"]);
                if (nVoteValue == 1)
                    v.nUpvotes++;
                if (nVoteValue == -1)
                    v.nDownvotes++;
            }
            return v;
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

        

        // Save User Record
        public static bool SaveUserRecord(bool fTestNet, User u, Page p)
        {
            BiblePayCommon.Entity.user1 o = new BiblePayCommon.Entity.user1();

            o.UserName = u.UserName;
            o.EmailAddress = u.EmailAddress;
            o.BiblePayAddress = u.BiblePayAddress;
            o.Verified = u.Verified;
            o.AvatarURL = u.AvatarURL;
            o.HashTags = u.HashTags;
            o.Testimony = u.Testimony;
            o.Slogan = u.Slogan;

            if (o.BiblePayAddress == "" || o.BiblePayAddress == null)
            {
                string sNarr = "Sorry, you must create a wallet first.  (Wallets are completely free, but necessary for the system to store data).  Please click 'Generate New Address' in Account settings.  ";
                UICommon.MsgModal(p, "Error", sNarr, 500, 250);
                return false;
            }
            else
            {
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(fTestNet, o);
                return true;
            }
        }

        public static bool HasOwnership(bool fTestNet, string sObjectID, string sTable, string sUserID)
        {
            if (System.Diagnostics.Debugger.IsAttached)
                return true;

            if (sUserID == null || sUserID == "")
                return false;
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, sTable);
            DataOps.FilterDataTable(ref dt, "id='" + sObjectID + "'");
            if (dt.Rows.Count < 1)
                return false;
            string sOwnerID = dt.GetColValue("UserID");
            if (sOwnerID == sUserID || sOwnerID == "")
                return true;
            return false;
        }

        public static double GetSumOf(bool fTestNet, string sFilter, string sTable, string sField)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, sTable);
            dt = dt.FilterDataTable(sFilter);
            double nTotal = 0;
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                double nValue = dt.GetColDouble(i, sField);
                nTotal += nValue;
            }
            return nTotal;
        }

        public static Portfolios GetPortfolioSum(Portfolios p)
        {
            Portfolios n = new Portfolios();
            n.AmountForeign = 0;
            n.AmountBBP = 0;
            for (int j = 0; j < p.lPositions.Count; j++)
            {
                BiblePayCommon.Entity.price1 prc = Utils.GetCryptoPrice(p.lPositions[j].Ticker);

                if (p.lPositions[j].Ticker == "BBP")
                {
                    n.AmountBBP += p.lPositions[j].nAmount;
                    n.AmountUSDBBP += (prc.AmountUSD * p.lPositions[j].nAmount);

                }
                else
                {
                    n.AmountForeign += p.lPositions[j].nAmount;
                    n.AmountUSDForeign += (prc.AmountUSD * p.lPositions[j].nAmount);
                }
            }
            return n;
        }
        public struct Portfolios
        {
            public string UserID;
            public double AmountBBP;
            public double AmountForeign;
            public double AmountUSDBBP;
            public double AmountUSDForeign;
            public double Coverage;
            public string Ticker;
            public string Nickname;
            public double Strength;
            public string Address;
            public int Time;
            public List<SimpleUTXO> lPositions;
        }

        public struct PortfolioParticipant
        {
            public string UserID;
            public double AmountBBP;
            public double AmountForeign;
            public double AmountUSD;
            public double AmountUSDBBP;
            public double AmountUSDForeign;
            public double Coverage;
            public string NickName;
            public double Strength;
            public List<Portfolios> lPortfolios;
        }
        public static Dictionary<string, Portfolios> dictUTXO = new Dictionary<string, Portfolios>();

        public static Dictionary<string, PortfolioParticipant> GenerateUTXOReport(bool fTestNet)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, "utxostake1");
            Dictionary<string, PortfolioParticipant> dictParticipants = new Dictionary<string, PortfolioParticipant>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PortfolioParticipant pp = new PortfolioParticipant();
                bool fPortfolioParticipantExists = dictParticipants.TryGetValue(dt.Rows[i]["UserID"].ToString(), out pp);
                if (!fPortfolioParticipantExists)
                {
                    pp.lPortfolios = new List<Portfolios>();
                    dictParticipants.Add(dt.Rows[i]["UserID"].ToString(), pp);
                }

                Portfolios p = Utils.QueryUTXOList(fTestNet, dt.Rows[i]["Ticker"].ToString().ToUpper(), dt.Rows[i]["Address"].ToString(), 0);

                pp.NickName = UICommon.GetUserRecord(fTestNet, dt.Rows[i]["UserID"].ToString()).UserName;

                pp.UserID = dt.Rows[i]["UserID"].ToString();
                Portfolios pTotal = GetPortfolioSum(p);
                pp.AmountForeign += pTotal.AmountForeign;
                pp.AmountUSDBBP += pTotal.AmountUSDBBP;
                pp.AmountUSDForeign += pTotal.AmountUSDForeign;
                pp.AmountBBP += pTotal.AmountBBP;
                pp.lPortfolios.Add(p);
                
                pp.Coverage = pp.AmountUSDBBP / (pp.AmountUSDForeign + .01);
                if (pp.Coverage > 1)
                    pp.Coverage = 1;
                dictParticipants[dt.Rows[i]["UserID"].ToString()] = pp;

            }

            double nTotalUSD = 0;
            double nParticipants = 0;
            foreach (KeyValuePair<string, PortfolioParticipant> pp in dictParticipants.ToList())
            {
                PortfolioParticipant p1 = dictParticipants[pp.Key];
                p1.AmountUSD = pp.Value.AmountUSDBBP + (pp.Value.AmountUSDForeign * pp.Value.Coverage);
                dictParticipants[pp.Key] = p1;
                nTotalUSD += p1.AmountUSD;
                nParticipants++;
            }
            // Assign Strength
            foreach (KeyValuePair<string, PortfolioParticipant> pp in dictParticipants.ToList())
            {
                PortfolioParticipant p1 = dictParticipants[pp.Key];
                p1.Strength = p1.AmountUSD / (nTotalUSD + .01);
                dictParticipants[pp.Key] = p1;
            }

            return dictParticipants;

        }
    }
}
