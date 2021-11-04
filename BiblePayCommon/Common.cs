using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace BiblePayCommon
{


    public class BiblePayClient : System.Net.WebClient
    {
        static bool ValidateCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors errors)
        {
            return true;
        }

        private int DEFAULT_TIMEOUT = 30000;

        public object FetchObject(string URL)
        {
            object o = this.DownloadString(URL);
            return o;
        }

        public void SetTimeout(int iTimeout)
        {
            DEFAULT_TIMEOUT = iTimeout * 1000;
        }
        protected override System.Net.WebRequest GetWebRequest(Uri uri)
        {
            System.Net.WebRequest w = base.GetWebRequest(uri);
            w.Timeout = DEFAULT_TIMEOUT;
            return w;
        }
    }

    public static class Common
    {
        public static string DOMAIN_NAME = "";
        public static string EmptyAvatar()
        {
            return "<img src='images/emptyavatar.png' width='50' height='50'>";
        }
        public static string EmptyAvatarNoDims(string sClass)
        {
            return "<img src='images/emptyavatar.png' class='" + sClass + "'>";
        }
        public static string NotNull(object o)
        {

            if (o == null || o == DBNull.Value) return "";
            return o.ToString();
        }

        public static bool IsEmailValid(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
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


        public static string ToNonNullString2(this object o)
        {
            if (o == null)
                return String.Empty;
            return o.ToString();
        }

        [Serializable]
        public struct TelegramMessage
        {
            public long MessageID;
            public long ChatID;
            public string ChatRoomTitle;
            public string ContentType;
            public string Text;
            public string Path;
            public long Date;
            public string Title;
            public long NextMessageID;
            public string WebPagePhotoPath;
            public string WebPageDescription;
            public string WebPageDisplayURL;
            public string WebEmbedType;
            public string WebEmbedURL;
            public string WebURL;
            public string WebPageTitle;
            public string UserFirstName;
            public string UserLastName;
            public long UserID;
            public string UserProfilePhoto;
            
        }

        public struct User
        {
            // User Record Specific
            public bool LoggedIn;
            public string Signature;
            public int SignatureTimestamp;

            // User Record + Entity Overlapping fields:

            public string UserName { get; set; }
            public string FirstName { get; set; }
            public string PasswordHash { get; set; }
            public string LastName { get; set; }
            public string UserGuid { get; set; }
            public string AvatarURL { get; set; }
            public string BiblePayAddress { get; set; }
            public string EmailAddress { get; set; }
            public int Verified { get; set; }
            public string HashTags { get; set; }
            public string Slogan { get; set; }
            public string Testimony { get; set; }

            public string ThemeName { get; set; }
            public string Domain { get; set; }
            public string RSAKey { get; set; }
            public string Shared2FA { get; set; }
            public int FA2Verified { get; set; }

            public int EmailVerified { get; set; }
            public string id { get; set; }
            public string PublicText { get; set; }
            public string ProfessionalText { get; set; }
            public string PrivateText { get; set; }
            public string ReligiousText { get; set; }
            public int BirthDate { get; set; }
            public string Gender { get; set; }
            public string TelegramLinkName { get; set; }
            public string TelegramLinkURL { get; set; }
            public string TelegramLinkDescription { get; set; }

            public int Tickets { get; set; }
            public int Administrator { get; set; }
            public int Banned { get; set; }

            public string FullUserName()
            {
                string sFullName = this.FirstName.ToNonNullString2() + " " + this.LastName.ToNonNullString2();

                if (sFullName.Trim() == "")
                {
                    sFullName = UserName.ToNonNullString2();
                }
                if (sFullName.Trim() == "")
                {
                    sFullName = "N/A";
                }
                return sFullName;
            }
            public string GetAvatarImage()
            {

                if (AvatarURL == "" || AvatarURL == null || AvatarURL.Contains("emptyavatar"))
                {
                    return EmptyAvatar();
                }
                else
                {
                    return "<img src='" + AvatarURL + "' width=50 height=50>";
                }
            }
            public string GetAvatarImageNoDims(string sClass)
            {
                if (AvatarURL == "" || AvatarURL == null || AvatarURL.Contains("emptyavatar"))
                {
                    return EmptyAvatarNoDims(sClass);
                }
                else
                {
                    return "<img src='" + AvatarURL + "' class='" + sClass + "'>";
                }
            }
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

        public static void EraseTempData(string sPath)
        {
            try
            {
                System.IO.File.Delete(sPath);
            }
            catch (Exception)
            {

            }
        }

        public static dynamic CastDataTableRowToExpando(DataRow dr, string sTable)
        {
            dynamic expando = new System.Dynamic.ExpandoObject();
            var dictionary = (IDictionary<string, object>)expando;
            for (int i = 0; i < dr.Table.Columns.Count; i++)
            {
                dictionary.Add(dr.Table.Columns[i].ColumnName, dr[i]);
            }
            expando.table = sTable;
            return expando;
        }

        public static dynamic NewtonSoftToExpando(dynamic oNewton)
        {
            var o = new System.Dynamic.ExpandoObject() as IDictionary<string, Object>;
            foreach (var attribute in oNewton)
            {
                string sColName1 = attribute.Name;
                dynamic oValue = attribute.Value;
                o.Add(sColName1, oValue);
            }
            return o;
        }

        public static void CleanDirectoryOfOldFiles(string sPattern)
        {
            string[] f = System.IO.Directory.GetFiles(Path.GetTempPath(), sPattern);
            for (int i = 0; i < f.Length; i++)
            {
                string filename = f[i];
                FileInfo fi = new FileInfo(filename);
                double nElapsed = DateTime.Now.Subtract(fi.LastWriteTime).TotalSeconds;
                if (nElapsed > (60 * 60 * 24))
                {
                    EraseTempData(filename);
                }
            }
        }

        public static int UnixTimestamp(DateTime dt)
        {
            DateTime dt2 = Convert.ToDateTime(new DateTime(dt.Year, dt.Month, dt.Day, dt.Hour, dt.Minute, dt.Second));
            Int32 unixTimestamp = (Int32)(dt2.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public static int UnixTimeStampFromFile(string sPath)
        {
            try
            {
                FileInfo fi = new FileInfo(sPath);
                return UnixTimestamp(fi.LastWriteTimeUtc);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        public static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            return origin.AddSeconds(timestamp);
        }

        public static string UnixTimeStampToDisplayAge(double timestamp)
        {
            if (timestamp == 0)
                return "N/A";
            DateTime dtBirthday = BiblePayCommon.Common.ConvertFromUnixTimestamp(timestamp);
            TimeSpan t = DateTime.Now.Subtract(dtBirthday);
            int nAge = (t.Days / 365);
            return nAge.ToString();
        }

        public static int DateToUnixTimeStamp(string sDate)
        {
            try
            {
                int d = (int)DateTimeToUnixTimestamp(Convert.ToDateTime(sDate));
                return d;
            }
            catch(Exception ex)
            {
                return 0;
            }
        }

        public static String UnixTimeStampToDateControl(double timestamp)
        {
            // From Unix UTC to yyyy-MM-dd for a datepicker control
            if (timestamp == 0)
                return "";
            string dt = BiblePayCommon.Common.ConvertFromUnixTimestamp(timestamp).ToString("yyyy-MM-dd");
            return dt;
        }

        public static int UnixTimeStamp()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base65Encode(string sData)
        {
            string s1 = Base64Encode(sData);
            string s2 = s1.Replace("=", "[equal]");
            return s2;
        }

        private static string sLastLog = "";

        public static void Log2(string sData, bool fQuiet = false)
        {
            try
            {
                string sPath = GetFolderUnchained("unchained.log");
                System.IO.StreamWriter sw = new System.IO.StreamWriter(sPath, true);
                string Timestamp = DateTime.Now.ToString();
                if (fQuiet && sLastLog == sData)
                    return;
                sLastLog = sData;

                sw.WriteLine(Timestamp + ": " + sData);
                sw.Close();
            }
            catch (Exception ex)
            {
                string sMsg = ex.Message;
            }
        }

        public static double DateTimeToUnixTimestamp(DateTime dateTime)
        {
            DateTime unixStart = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            long unixTimeStampInTicks = (dateTime.ToUniversalTime() - unixStart).Ticks;
            return (double)unixTimeStampInTicks / TimeSpan.TicksPerSecond;
        }

        public static long UnixTimestampUTC()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        public static long FileAge(string sPath)
        {
            long nAge = UnixTimestampUTC() - UnixTimeStampFromFile(sPath);
            return nAge;
        }
       
        public static void WriteToFile(string path, string data)
        {
            File.WriteAllText(path, data);
        }

        public static string StoreTempData(string sFilename, string sData)
        {
            string sFullPath = Path.Combine(Path.GetTempPath(), sFilename);
            WriteToFile(sFullPath, sData);
            return sFullPath;
        }

        public static string ByteArrayToHexString(byte[] arrInput)
        {
            StringBuilder sOutput = new StringBuilder(arrInput.Length);

            for (int i = 0; i < arrInput.Length; i++)
            {
                sOutput.Append(arrInput[i].ToString("X2"));
            }
            return sOutput.ToString().ToLower();
        }
        public static string GetMd5String(string sData)
        {
            byte[] arrData = System.Text.Encoding.UTF8.GetBytes(sData);
            var hash = System.Security.Cryptography.MD5.Create().ComputeHash(arrData);
            return ByteArrayToHexString(hash);
        }

        public static int HexToInteger(string hex)
        {
            int d = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            return d;
        }

        public static double ConvertHexToDouble(string hex)
        {
            int d = HexToInteger(hex);
            double dOut = (double)d;
            return dOut;
        }

        public static bool CompareMask(double nAmount, int nPin)
        {
            string sAmount = nAmount.ToString();
            bool fPin = sAmount.Contains(nPin.ToString());
            if (fPin)
                return fPin;
            string sPin = nPin.ToString();
            if (sPin.Substring(sPin.Length-1,1) == "0")
            {
                sPin = sPin.Substring(0, sPin.Length-1);
                fPin = sAmount.Contains(sPin);
            }
            return fPin;
        }
        public static double AddressToPin(string sBBPAddress, string sCryptoAddress)
        {
            string sConcat = sBBPAddress + sCryptoAddress;
            return AddressToPin0(sConcat);
        }
        public static double AddressToPin0(string sAddress)
        {
            if (sAddress.Length < 20)
                return -1;

            string sHash = GetMd5String(sAddress);
            string sMath5 = sHash.Substring(0, 5); // 0 - 1,048,575
            double d = ConvertHexToDouble("" + sMath5) / 11.6508;

            int nMin = 10000;
            int nMax = 99999;
            d += nMin;

            if (d > nMax)
                d = nMax;

            d = Math.Floor(d);
            return d;

            // Why a 5 digit pin?  Looking at the allowable suffix size (digits of scale after the decimal point), we have 8 in scale to work with.  
            // With BTC at $32,000 this would be $250 of value tied up at a minimum, in the pin suffix.
            // Therefore, we moved down to a 5 digit pin to make the reqt around $22 in latent value.
            // Note that this monetary overhead is not actually *lost*, it is simply tied up in the stake.
        }

        public static string GetFolderUnchained(string sType)
        {
            string sPath = "c:\\inetpub\\wwwroot\\Unchained\\" + sType;
            return sPath;
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


        public enum FileType
        {
            VIDEO,
            DSQL
        };
        public class DACResult
        {
            public string Result;
            public string Error;
            public string Event;
            public string Alt;
            public bool fError()
            {
                bool fHasError = (Error != null && Error != "" && Error.Length > 0);
                return fHasError;
            }
            public double Fee;
            public bool OverallResult;
            public string URL;
            public string TXID;
            public string ObjectHash;
            public string Table;
            public dynamic ExpandoObject;
            public object Invoice;
            public double Amount;
            public double UTXOBalance;
            public double AccountingBalance;
            public DACResult()
            {
                Error = "";
            }
        }

        public static int GetInt(object o)
        {
            double n = GetDouble(o);
            return (int)n;
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


    public static class Encryption
    {
        public struct KeySet
        {
            public string TestNetPrivateKey;
            public string MainNetPrivateKey;
            public BiblePayCommon.Common.User userTestNet;
            public BiblePayCommon.Common.User userProd;
            public double TESTNET_SERVER_BALANCE;
            public double MAINNET_SERVER_BALANCE;
        }
        public static KeySet _keyset = new KeySet();

        public static string GetFundingKey(bool fTestNet)
        {
            string suffix = fTestNet ? "testnet" : "mainnet";
            string sKey = "FundingKey_" + suffix;
            return CommonConfig(sKey);
        }
        public static void SetPrivateKey(BiblePayCommon.Common.User uTestNet, BiblePayCommon.Common.User uProd)
        {
            _keyset.TestNetPrivateKey = GetFundingKey(true);
            _keyset.MainNetPrivateKey = GetFundingKey(false);
            _keyset.userTestNet = uTestNet;
            _keyset.userProd = uProd;
        }

        public static string GetBaseHomeFolder()
        {
            string sHomePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
             Environment.OSVersion.Platform == PlatformID.MacOSX) ? Environment.GetEnvironmentVariable("HOME")
                 : Environment.ExpandEnvironmentVariables("%APPDATA%");
            sHomePath = "c:\\inetpub\\wwwroot\\";
            return sHomePath;
        }

        public static DateTime TimestampToDateTime(double nStamp)
        {
            DateTime dt = new DateTime();
            dt = BiblePayCommon.Encryption.UnixTimeStampToDateTime(nStamp);
            return dt;
        }


        public static string GetExtConfigurationKeyValue(string sPath, string _Key)
        {
            if (!File.Exists(sPath))
                return string.Empty;

            string sData = System.IO.File.ReadAllText(sPath);
            string[] vData = sData.Split(new string[] { "\n" }, StringSplitOptions.None);

            for (int i = 0; i < vData.Length; i++)
            {
                string sEntry = vData[i];
                sEntry = sEntry.Replace("\r", "");
                string[] vRow = sEntry.Split(new string[] { "=" }, StringSplitOptions.None);
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
        public static string CommonConfig(string _Key)
        {
            string sKV = GetExtConfigurationKeyValue(GetBaseHomeFolder() + "unchained.conf", _Key);
            return sKV;
        }

        public static string GetSha256HashI(string rawData)
        {
            if (rawData == null){
                rawData = String.Empty;
            }
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

        public static string GetBurnAddress(bool fTestNet)
        {
            // These are hardcoded in the biblepaycore wallet:
            string sBurnAddress = !fTestNet ? "B4T5ciTCkWauSqVAcVKy88ofjcSasUkSYU" : "yLKSrCjLQFsfVgX8RjdctZ797d54atPjnV";
            return sBurnAddress;
        }

        public static string GetDSQLAddress(bool fTestNet)
        {
            // These are hardcoded in the biblepaycore wallet:
            // Mission critical todo: Change these from our burn address to our DSQL orphan charity addresses:
            string sBurnAddress = !fTestNet ? "B4T5ciTCkWauSqVAcVKy88ofjcSasUkSYU" : "yLKSrCjLQFsfVgX8RjdctZ797d54atPjnV";
            return sBurnAddress;
        }

        static byte[] EncryptAES256(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            using (AesManaged aes = new AesManaged())
            {
                ICryptoTransform encryptor = aes.CreateEncryptor(Key, IV);
                // Create MemoryStream    
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
        }
        static string DecryptAES256(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create AesManaged    
            using (AesManaged aes = new AesManaged())
            {
                // Create a decryptor    
                ICryptoTransform decryptor = aes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.    
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream    
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream    
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }

        private static byte[] GetBytedKeyFromPassword(string password)
        {
            InitializeAES();
            if (password == null || password == "")
            {
                return myBytedKey;
            }
            byte[] myByted = new byte[32];
            password = password.PadRight(64);
            byte[] bytes = Encoding.ASCII.GetBytes(password);
            for (int i = 0; i < 32; i++)
            {
                myByted[i] = bytes[i];
            }
            return myByted;
        }

        private static byte[] myBytedKey = new byte[32];
        private static byte[] myBytedIV = new byte[16];
        private static void InitializeAES()
        {
            // These static bytes were ported in from Biblepay-QT, because OpenSSL uses a proprietary method to create the 256 bit AES-CBC key: EVP_BytesToKey(EVP_aes_256_cbc(), EVP_sha512()
            string sAdvancedKey = "98,-5,23,119,-28,-99,-5,90,62,-63,82,39,63,-67,-85,37,-29,-65,97,80,57,-24,71,67,119,14,-67,12,-96,99,-84,-97";
            string sIV = "29,44,121,61,-19,-62,55,-119,114,105,-123,-101,52,-45,29,-109";
            var vKey = sAdvancedKey.Split(new string[] { "," }, StringSplitOptions.None);
            var vIV = sIV.Split(new string[] { "," }, StringSplitOptions.None);
            myBytedKey = new byte[32];
            myBytedIV = new byte[16];

            for (int i = 0; i < vKey.Length; i++)
            {
                int iMyKey = (int)Common.GetDouble(vKey[i]);
                myBytedKey[i] = (byte)(iMyKey + 0);
            }
            for (int i = 0; i < vIV.Length; i++)
            {
                int iMyIV = (int)Common.GetDouble(vIV[i]);
                myBytedIV[i] = (byte)(iMyIV + 0);
            }
        }
        public static string EncryptAES256(string sData, string password)
        {
            byte[] myBytedLocal = GetBytedKeyFromPassword(password);
            byte[] myByteOut = EncryptAES256(sData, myBytedLocal, myBytedIV);
            return System.Convert.ToBase64String(myByteOut);
        }
        public static string DecryptAES256(string sData, string password)
        {
            if (sData == "")
                return "";
            try
            {
                byte[] myBytedLocal = GetBytedKeyFromPassword(password);
                byte[] b = System.Convert.FromBase64String(sData);
                string plainText = DecryptAES256(b, myBytedLocal, myBytedIV);
                return plainText;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }
        public static string Base64DecodeWithFilter(string b)
        {
            return Base64Decode0(b, true);
        }

        public static string CleanseXSS(string d)
        {
            d = d.Replace("<br>", "[br]");
            d = d.Replace("<script>", "[script]");
            d = d.Replace("</script>", "[/script]");
            d = d.Replace("<", "[lessthan]");
            d = d.Replace(">", "[greaterthan]");
            return d;
        }
        public static string Base64Decode0(string base64EncodedData, bool fApplyFilter = false)
        {
            try
            {
                var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
                string d = System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
                if (fApplyFilter)
                {
                    d = CleanseXSS(d);
                }
                return d;

            }
            catch (Exception)
            {
                return String.Empty;
            }
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
            string s2 = Base64Decode0(s1);
            return s2;
        }

        public static string CleanModalString(string sData)
        {
            sData = sData.Replace("'", "");
            sData = sData.Replace("\r\n", "<br>");
            sData = sData.Replace("\r", "<br>");
            sData = sData.Replace("\n", "<br>");
            return sData;
        }
    }
}
