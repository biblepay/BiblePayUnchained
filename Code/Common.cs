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
using System.Dynamic;
using System.Net.Mail;
using static Unchained.UICommon;
using MongoDB.Driver;
using static BiblePayCommon.EntityCommon;

namespace Unchained
{
    public static class Common
    {
        public static int nVersionLocal = 1401;

        public static Data gData = new Data(Data.SecurityType.REQ_SA);
        public static string GetSiteTitle(Page p)
        {
            return Config("sitetitle");
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
            s = s.Replace("\n", " <br>");

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
        public static bool IsTestNetFromSession(System.Web.SessionState.HttpSessionState s)
        {
            double nChain = GetDouble(s["mainnet_mode"]);
            bool fProd = nChain != 2;
            return !fProd;
        }

        public static string GetChain0(bool fTestNet)
        {
            string sPrefix = fTestNet ? "test" : "main";
            return sPrefix;
        }

        public static User RetrieveUser(Page p, FilterDefinition<dynamic> oFilter)
        {
            IList<dynamic> dt = BiblePayDLL.Sidechain.GetChainObjects<dynamic>(false, "user1", oFilter, SERVICE_TYPE.PUBLIC_CHAIN);
            Object o = new User();
            if (dt.Count < 1)
                return (User)o;
            BiblePayCommon.EntityCommon.CastExpandoToBiblePayObject(dt[0], ref o);
            return (User)o;
        }

        public static User gUserSession(System.Web.SessionState.HttpSessionState s)
        {
            User u = new User();

            if (s[GetChain0(IsTestNetFromSession(s)) + "user"] != null)
            {
                u = (User)s[GetChain0(IsTestNetFromSession(s)) + "user"];
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
            return u;

        }

        public static User gUser(Page p)
        {

            double nTriedFromCookie = GetDouble(p.Session["cookielogin"]);
            if (nTriedFromCookie == 0)
            {
                p.Session["cookielogin"] = 1;
                UICommon.LoginWithCookie(p);
            }


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
            var builder = Builders<dynamic>.Filter;
            var filter = builder.Eq("EmailAddress", sEmailAddress);
            User u = RetrieveUser(p, filter);
            return u;
        }

        public static User gUser(Page p, string sFirstName, string sLastName)
        {
            var builder = Builders<dynamic>.Filter;
            var filter = builder.Eq("firstname", sFirstName) & builder.Eq("lastname", sLastName);
            User u = RetrieveUser(p, filter);
            return u;
        }
        public static User gUserById(Page p, string id)
        {
            var builder = Builders<dynamic>.Filter;
            var filter = builder.Eq("_id", id);
            User u = RetrieveUser(p, filter);
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
            string sDeleteAnchor = UICommon.GetStandardAnchor(u.id, "DeleteObject", sID, "<i class='fa fa-trash'></i>", "Delete this object", sTable);
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

            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "vote1");
            dt = dt.FilterDataTable("parentid='" + sParentID + "' and deleted=0");
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
       
        public static BiblePayCommon.Entity.user1 ConvertUserToUserEntity(User u)
        {
            ExpandoObject oExpando = BiblePayCommon.EntityCommon.CastObjectToExpando(u);
            BiblePayCommon.Entity.user1 u10 = (BiblePayCommon.Entity.user1)BiblePayCommon.EntityCommon.ExpandoToStronglyCastObject(oExpando, "user1");
            return u10;
        }

        public static void NotifyOfNewUser(User u, Page p)
        {
            try
            {

                double nReplay = BiblePayCommon.Common.GetDouble(BiblePayCommon.HalfordDatabase.GetKVDWX("NotifyOfNewUser" + u.EmailAddress));
                if (nReplay == 1)
                {
                    // already notified
                    return;
                }
                BiblePayCommon.HalfordDatabase.SetKVDWX("NotifyOfNewUser" + u.EmailAddress, 1, 60 * 60 * 24 * 30);

                string sTemplate = Config("TemplateNewUser");
                if (sTemplate != "")
                {
                    string sPath = "c:\\inetpub\\wwwroot\\Templates\\" + sTemplate;
                    string sData = System.IO.File.ReadAllText(sPath);

                    MailMessage m = new MailMessage();
                    EmailNarr e = GetEmailFooter(p);
                    m.IsBodyHtml = true;
                    //string sNarr = "Dear " + u.FullUserName() + ",<br><br>Thank you for registering with our platform."
                    //    + "<br><br>" + sData + "<br><br><br>" +"The "+ e.DomainName + " Team<br>";
                    sData = sData.Replace("[FullUserName]", u.FullUserName());

                    m.Subject = "[Transactional Message] Welcome to " + e.DomainName + "!";
                    m.Body = sData;
                    m.IsBodyHtml = true;
                    m.To.Add(new MailAddress(u.EmailAddress, u.FullUserName()));


                    string sNotifyExtra = Config("NotifyUser");
                    if (sNotifyExtra != "")
                    {
                        m.Bcc.Add(new MailAddress(sNotifyExtra));
                    }
                    m.Bcc.Add(new MailAddress("rob@biblepay.org"));
                   
                    DACResult r = BiblePayDLL.Sidechain.SendMail(IsTestNet(p), m, e.DomainName);
                }
            }catch(Exception ex)
            {

            }

        }
        // Save User Record
        public static DACResult SaveUserRecord(bool fTestNet, User u, Page p)
        {
            DACResult r = new DACResult();

            bool fIsNew = false;

            BiblePayCommon.Entity.user1 o = ConvertUserToUserEntity(u);
            
            if (!IsEmailValid(o.EmailAddress))
            {
                r.Error = "Sorry, the E-mail address is invalid.";
                MsgModal(p, "Error", r.Error, 400, 200, true);
                return r;
            }
            if (o.UserGuid == null)
            {

                if (gUser(p).LoggedIn)
                {
                    r.Error = "Sorry, you are already logged in as another user.";
                    MsgModal(p, "Error", r.Error, 400, 200, true);
                    return r;
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
                    r.Error = "Sorry, this user already exists.";
                    MsgModal(p, "Error",  r.Error, 400, 200, true);
                    return r;
                }

                if (u.FirstName.Length < 3 || u.LastName.Length < 3)
                {
                    r.Error = "Your name must be longer than 3 characters.";
                    MsgModal(p, "Error", r.Error, 400, 200, true);
                    return r;
                }
                fIsNew = true;
            }

            if (u.LoggedIn && u.EmailAddress != gUser(p).EmailAddress)
            {
                // User wants to change email address

                User uEmailUser = gUser(p, u.EmailAddress);
                if (uEmailUser.id == null)
                {
                    o.EmailVerified = 0;
                }
                else
                {
                    r.Error = "Sorry, this new e- mail address is already taken.";
                    MsgModal(p, "Error", r.Error, 500, 250, true);
                    return r;
                }
            }

            if (u.PasswordHash.ToNonNullString().Length != 64)
            {
                r.Error = "Sorry, your password did not meet the minimum complexity guidelines [8 characters+,1 Uppercase Letter+,1 Number+].";
                MsgModal(p, "Error", r.Error, 400, 200, true);
                return r;
            }

            string sDomainName = HttpContext.Current.Request.Url.Host;
            o.domain = sDomainName;
            u.Domain = sDomainName;

            r = DataOps.InsertIntoTable(p, fTestNet, o, u);
            if (r.fError())
            {
                MsgModal(p, "Error", r.Error, 500, 250, true);
                return r;
            }
            else
            {
                // With the user ID
                if (fIsNew)
                    NotifyOfNewUser(u, p);
                u = gUser(p, u.EmailAddress);
                // This is where we save the users session
                BiblePayDLL.Sidechain.SetBiblePayAddressAndSignature(IsTestNet(p), sDomainName, ref u);
                p.Session[GetChain0(fTestNet) + "user"] = u;
            }
            return r;
        }

        public static BiblePayCommon.IBBPObject GetObject(bool fTestNet, string sTable, string sID)
        {
            var builder = Builders<dynamic>.Filter;
            var filter = builder.Eq("_id", sID) | builder.Eq("id", sID);

            IList<dynamic> l1 = BiblePayDLL.Sidechain.GetChainObjects<dynamic>(fTestNet, sTable, filter,
                SERVICE_TYPE.PUBLIC_CHAIN);
            //DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2Retired(fTestNet, sTable);
            //BiblePayCommon.IBBPObject o = BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt, sTable, 0);
            BiblePayCommon.IBBPObject o = ExpandoToStronglyCastObject(l1[0], sTable);
            return o;
        }

        public static BiblePayCommon.IBBPObject GetObjectWithFilter(bool fTestNet, string sTable, string sFilter)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, sTable);
            dt = dt.FilterDataTable(sFilter);
            BiblePayCommon.IBBPObject o = BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt, sTable, 0);
            return o;
        }


        public static bool HasOwnership(bool fTestNet, string sObjectID, string sTable, string sUserID)
        {
            if (System.Diagnostics.Debugger.IsAttached) return true;

            if (sUserID == null || sUserID == "")
                return false;
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, sTable);
            DataOps.FilterDataTable(ref dt, "id='" + sObjectID + "'");
            if (dt.Rows.Count < 1)
                return false;
            string sOwnerID = dt.GetColValue("UserID");
            if (sOwnerID == sUserID || sOwnerID == "")
                return true;
            return false;
        }

        /*
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
        */

    }
}
