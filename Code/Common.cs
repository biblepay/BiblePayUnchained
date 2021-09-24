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
using static BiblePayCommonNET.UICommonNET;
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommonNET.DataTableExtensions;


namespace Unchained
{
    public static class Common
    {
        public static Data gData = new Data(Data.SecurityType.REQ_SA);
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

        public static BiblePayCommon.Common.User CoerceUser(bool fTestNet)
        {
            // This allows the server to perform certain actions as Administrator (such as saving a User record the first time)...
            // But note; this function does not give the server owner unlimited permissions to alter chain data
            // It only allows the server to Add to a chain, or EDIT its own authorized chain data.
            // A server cannot edit another federated servers chain data, but a server may add a new record or edit its own data.
            BiblePayCommon.Common.User u = new BiblePayCommon.Common.User();
            u.BiblePayAddress = GetFundingAddress(fTestNet);
            u.UserName = "Administrator";
            u.FirstName = "Administrator";
            u.LastName = "";
            u.SignatureTimestamp = (int)BiblePayCommon.Common.UnixTimestampUTC();
            u.Signature = BiblePayDLL.Sidechain.SignMessage(fTestNet, GetFundingKey(fTestNet), u.SignatureTimestamp.ToString());
            return u;
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

        public static bool IsPasswordStrong(string pw)
        {
            var hasNumber = new Regex(@"[0-9]+");
            var hasUpperChar = new Regex(@"[A-Z]+");
            var hasMinimum8Chars = new Regex(@".{8,}");
            var isValidated = hasNumber.IsMatch(pw) && hasUpperChar.IsMatch(pw) && hasMinimum8Chars.IsMatch(pw);
            return isValidated;
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

                int nLoc = instr(0, sEntry, "=", 0);
                if (nLoc > 0)
                {
                    string sNewData = sEntry.Substring(0, nLoc) + "[coldelimiter]" + sEntry.Substring(nLoc + 1, sEntry.Length - nLoc-1);


                    string[] vRow = sNewData.Split("[coldelimiter]");
                    if (vRow.Length >= 2)
                    {
                        string sKey = vRow[0];
                        string sValue = vRow[1];
                        if (sKey.ToUpper() == _Key.ToUpper())
                            return sValue;
                    }
                }
            }
            return string.Empty;
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
            string sDOMAIN_NAME = DOMAIN_NAME;
            if (sDOMAIN_NAME.Contains("BiblePayUnchained"))
            {
                // This area allows a developer to choose the domain they log into from localhost (this does not get used in production):
                sDOMAIN_NAME = "unchained";
                //sDOMAIN_NAME = "dec.app";
            }
            string sKV = GetExtConfigurationKeyValue(GetBaseHomeFolder() + sDOMAIN_NAME + ".conf", _Key);
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


        public static bool IsTestNet(Page p)
        {
            double nChain = GetDouble(p.Session["mainnet_mode"]);
            bool fProd = nChain != 2;
            return !fProd;
        }

        /*
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
        */

        public static string GetChain0(bool fTestNet)
        {
            string sPrefix = fTestNet ? "test" : "main";
            return sPrefix;
        }

        public static User RetrieveUser(Page p, string sFilter)
        {
            User u = new User();
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(p), "user1");
            dt = dt.FilterDataTable(sFilter);
            if (dt.Rows.Count < 1)
                return u;

            u.UserName = dt.Rows[0]["UserName"].ToString();
            u.FirstName = dt.GetColValue("FirstName");
            u.LastName = dt.GetColValue("LastName");
            u.PasswordHash = dt.GetColValue("PasswordHash");
            u.EmailAddress = dt.GetColValue("EmailAddress");
            u.Verified = dt.GetColInt("Verified");
            u.Testimony = dt.GetColValue("Testimony");
            u.Slogan = dt.GetColValue("Slogan");
            u.AvatarURL = dt.GetColValue("AvatarURL");
            u.ThemeName = dt.GetColValue("ThemeName");
            u.Shared2FA = dt.GetColValue("Shared2FA");
            u.id = dt.GetColValue("id");
            u.FA2Verified = dt.GetColInt("FA2Verified");
            u.EmailVerified = dt.GetColInt("EmailVerified");
            u.HashTags = dt.GetColValue("HashTags");
            u.RSAKey = dt.GetColValue("RSAKey");
            u.BiblePayAddress = dt.GetColValue("BiblePayAddress");
            u.UserGuid = dt.GetColValue("UserGuid");
            u.PublicText = dt.GetColValue("PublicText");
            u.ProfessionalText = dt.GetColValue("ProfessionalText");
            u.PrivateText = dt.GetColValue("PrivateText");
            u.ReligiousText = dt.GetColValue("ReligiousText");
            u.Gender = dt.GetColValue("Gender");
            u.BirthDate = dt.GetColInt("BirthDate");
            u.TelegramLinkName = dt.GetColValue("TelegramLinkName");
            u.TelegramLinkDescription = dt.GetColValue("TelegramLinkDescription");
            u.TelegramLinkURL = dt.GetColValue("TelegramLinkURL");
            u.Administrator = dt.GetColInt("Administrator");

            return u;
        }

        public static User gUser(Page p)
        {
            User u = new User();
            if (p.Session["mainnet_mode"] == null)
            {
                p.Session["mainnet_mode"] = 1;
            }

            if (p.Session[GetChain0(IsTestNet(p)) + "user"] != null)
            {

                u = (User)p.Session[GetChain0(IsTestNet(p)) + "user"];
                u.LoggedIn = u.FirstName.ToNonNullString().Length > 1;
                if (!u.LoggedIn)
                {
                    u.UserName = "Guest";
                    u.FirstName = "Guest";
                }
                else
                {
                    u.UserName = u.FirstName;
                }

                return u;
            }
            u.FirstName = "Guest";
            u.UserName = "Guest";
            return u;
        }

        public static User gUser(Page p, string sEmailAddress)
        {
            User u = RetrieveUser(p, "emailaddress='" + sEmailAddress + "'");
            return u;
        }

        public static User gUser(Page p, string sFirstName, string sLastName)
        {
            User u = RetrieveUser(p, "firstname='" + sFirstName + "' and lastname='" + sLastName + "'");
            return u;
        }
        public static User gUserById(Page p, string id)
        {
            User u = RetrieveUser(p, "id='" + id + "'");
            return u;
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

        public static string GetObjectRating(bool fTestNet, string sID, string sTable, User u)
        {
            string sHTML = "";
            /*            Star Ratings... 1-5 stars...
            for (int i = 1; i <= 5; i++)
            {
                sChecked = "";                if (i <= nRating)                    sChecked = "checked";                sHTML += "<span class='fa fa-star " + sChecked + "'></span>";
            }
            */
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
            // Add on the delete button
            bool fOwns = HasOwnership(fTestNet, sID, sTable, u.id);
            string sDeleteAnchor = UICommon.GetStandardAnchor(u.id, "DeleteObject", sID, "<i class='fa fa-trash'></i>", sTable);
            if (fOwns)
                      sHTML += "&nbsp;" + sDeleteAnchor;
            return sHTML;
        }

/*
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
        */


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
       
        // Save User Record
        public static bool SaveUserRecord(bool fTestNet, User u, Page p)
        {
            BiblePayCommon.Entity.user1 o = new BiblePayCommon.Entity.user1();
            o.FirstName = u.FirstName;
            o.LastName = u.LastName;
            o.EmailAddress = u.EmailAddress.Trim();
            o.BiblePayAddress = u.BiblePayAddress;
            o.Verified = u.Verified;
            o.AvatarURL = u.AvatarURL;
            o.HashTags = u.HashTags;
            o.Testimony = u.Testimony;
            o.Slogan = u.Slogan;
            o.ThemeName = u.ThemeName;
            o.Shared2FA = u.Shared2FA;
            o.FA2Verified = u.FA2Verified;
            o.EmailVerified = u.EmailVerified;
            o.RSAKey = u.RSAKey;
            o.PasswordHash = u.PasswordHash;
            o.UserGuid = u.UserGuid;
            o.PrivateText = u.PrivateText;
            o.PublicText = u.PublicText;
            o.ProfessionalText = u.ProfessionalText;
            o.ReligiousText = u.ReligiousText;
            o.Gender = u.Gender;
            o.BirthDate = u.BirthDate;
            o.TelegramLinkDescription = u.TelegramLinkDescription;
            o.TelegramLinkName = u.TelegramLinkName;
            o.TelegramLinkURL = u.TelegramLinkURL;


            if (!IsEmailValid(o.EmailAddress))
            {
                MsgModal(p, "Error", "Sorry, the e-mail address is invalid.", 400, 200, true);
                return false;
            }
            if (o.UserGuid == null)
            {

                if (gUser(p).LoggedIn)
                {
                    MsgModal(p, "Error", "Sorry, you are already logged in as another user.", 400, 200, true);
                    return false;
                }
                // create a new user
                o.UserGuid = Guid.NewGuid().ToString();
                o.RSAKey = BiblePayDLL.Sidechain.RSAEncryptValue(Guid.NewGuid().ToString()); // This is the new biblepay public keypair... 
                // Public chains cannot steal the private key, since it is RSA encrypted.
                // The BiblePay Signer decrypts the private key with the biblepay foundation private key (not included in our source code).
                // This allows cross-site spending with trust.
                u.RSAKey = o.RSAKey;
                u.UserGuid = o.UserGuid;

                // Email must not exist:
                User uEmail = gUser(p, o.EmailAddress);
                if (uEmail.EmailAddress != null)
                {
                    MsgModal(p, "Error", "Sorry, this user already exists.", 400, 200, true);
                    return false;
                }

                if (u.FirstName.Length < 3 || u.LastName.Length < 3)
                {
                    MsgModal(p, "Error", "Your name must be longer than 3 chars.", 400, 200, true);
                    return false;
                }
            }

            if (u.PasswordHash.ToNonNullString().Length != 64)
            {
                MsgModal(p, "Error", "Sorry, your password did not meet the minimum complexity guidelines [8 characters+,1 Uppercase Letter+,1 Number+].", 400, 200, true);
                return false;
            }

            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(p, fTestNet, o, u);
            if (r.fError())
            {
                MsgModal(p, "Error", r.Error, 500, 250, true);
            }
            else
            {
                // With the user ID
                u = gUser(p, u.EmailAddress);
                // This is where we save the users session
                string sDomainName = HttpContext.Current.Request.Url.Host;
                BiblePayDLL.Sidechain.SetBiblePayAddressAndSignature(IsTestNet(p), sDomainName, ref u);
                p.Session[GetChain0(fTestNet) + "user"] = u;
            }
            return true;
        }
        public static bool DataExists(bool fTestNet, string sTable, string sFilter)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, sTable);
            DataOps.FilterDataTable(ref dt, sFilter);
            return dt.Rows.Count > 0;
        }

        public static BiblePayCommon.IBBPObject GetObject(bool fTestNet, string sTable, string sID)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(fTestNet, sTable);
            dt = dt.FilterDataTable("id='" + sID + "'");
            BiblePayCommon.IBBPObject o = BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt, sTable, 0);
            return o;
        }

        public static bool HasOwnership(bool fTestNet, string sObjectID, string sTable, string sUserID)
        {
            if (System.Diagnostics.Debugger.IsAttached)                return true;

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
    }
}
