using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Web.UI;
using static BiblePayDLL.Shared;

namespace System.Net
{
    using System.Security.Authentication;
    public static class SecurityProtocolTypeExtensions
    {
        public const SecurityProtocolType Tls12 = (SecurityProtocolType)SslProtocolsExtensions.Tls12;
        public const SecurityProtocolType Tls11 = (SecurityProtocolType)SslProtocolsExtensions.Tls11;
        public const SecurityProtocolType SystemDefault = (SecurityProtocolType)0;
    }
}
namespace System.Security.Authentication
{
    public static class SslProtocolsExtensions
    {
        public const SslProtocols Tls12 = (SslProtocols)0x00000C00;
        public const SslProtocols Tls11 = (SslProtocols)0x00000300;
    }
}

namespace Internals
{
    public class BBPWebClient : System.Net.WebClient
    {
        private int DEFAULT_TIMEOUT = 30000;

        protected override WebRequest GetWebRequest(Uri address)
        {
            WebRequest r = base.GetWebRequest(address);
            r.Timeout = DEFAULT_TIMEOUT;
            var request = r as HttpWebRequest;
            return r;
        }

        public BBPWebClient()
        {
            
        }

        public void SetTimeout(int nTimeOut)
        {
            DEFAULT_TIMEOUT = nTimeOut;
        }
    }

    public static class Internal
    {

        public static string GetFolderUnchained(string sType)
        {
            string sPath = "c:\\inetpub\\wwwroot\\Unchained\\" + sType;
            return sPath;
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
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption    
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream    
                    // to encrypt    
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream    
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data    
            return encrypted;
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

    }

}


namespace BiblePayDLL
{

    public static class SharedCommon
    {
        public struct DACResult
        {
            public string Result;
            public string Error;
            public double Fee;
            public bool OverallResult;
            public string URL;
            public string TXID;
        }
    }

    public class rLB2N
    {

        public static long UnixTimestampUTC()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        }
        public static long FileAge(string sPath)
        {
            long nAge = UnixTimestampUTC() - UnixTimeStampFromFile(sPath);
            return nAge;
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

        Type IB2N_TYPE = null;
        private string DEPENDENCY_PATH = AppDomain.CurrentDomain.BaseDirectory + "BiblePay.NET.dll";
        private void GetDependency()
        {
            long nAge = FileAge(DEPENDENCY_PATH);
            if (nAge > 86400 || nAge < 0)
            {
                Internals.BBPWebClient w = new Internals.BBPWebClient();
                string sURL = "https://foundation.biblepay.org/Uploads/BiblePay.NET.dll";
                w.DownloadFile(sURL, DEPENDENCY_PATH);
            }
        }
        public rLB2N()
        {
            GetDependency();
            var DLL = Assembly.LoadFile(DEPENDENCY_PATH);
            IB2N_TYPE  = DLL.GetType("B2N.IB2N");
        }
        
        public double GetUploadPercent()
        {
            var method = IB2N_TYPE.GetMethod("GetPercCompleted");
            object nPerc = method.Invoke(null, null);
            return (double)nPerc;
        }
        public void UploadLocal(string sPath, string sFileName)
        {
            var method = IB2N_TYPE.GetMethod("Upload");
            method.Invoke(null, new object[] { sPath, sFileName });
        }
    }



    
    public class Data
    {

        public static bool IsProdChain(string sKey)
        {
            return sKey.StartsWith("y") ? false : true;
        }

        string GenerateXMLSignature(string sPrimaryKey, string sSigningPublicKey, string sPrivKey)
        {
            string sBBPSig;
            string sXML;
            sBBPSig = SignChrome(sPrivKey, sPrimaryKey, IsProdChain(sSigningPublicKey));
            sXML = "<BOSIGNER>" + sSigningPublicKey + "</BOSIGNER><BOSIG>" + sBBPSig + "</BOSIG><BOMSG>" + sPrimaryKey + "</BOMSG>";
            return sXML;
        }

        public static string GetRandomWords()
        {
            NBitcoin.Crypto.BibleHash b = new NBitcoin.Crypto.BibleHash();
            string w = b.GetRandomWords(10);
            return w;
        }

        private static bool VerifySignature(string BBPAddress, string sMessage, string sSig)
        {
            if (BBPAddress == null || sSig == String.Empty)
                return false;
            try
            {
                // Determine the network:
                BitcoinPubKeyAddress bpk;
                if (BBPAddress.StartsWith("y"))
                {
                    bpk = new BitcoinPubKeyAddress(BBPAddress, Network.BiblepayTest);
                }
                else if (BBPAddress.StartsWith("X"))
                {
                    bpk = new BitcoinPubKeyAddress(BBPAddress, Network.DashMain);
                }
                else
                {
                    bpk = new BitcoinPubKeyAddress(BBPAddress, Network.BiblepayMain);
                }

                bool b1 = bpk.VerifyMessage(sMessage, sSig);
                return b1;
            }
            catch (Exception ex)
            {
                IO.Log("VerifySignature::" + ex.Message + " for key " + BBPAddress);
                return false;
            }
        }

        private static string SignChrome(string sPrivKey, string sMessage, bool fProd)
        {
            if (sPrivKey == null || sMessage == String.Empty || sMessage == null)
                return string.Empty;

            BitcoinSecret bsSec;
            if (fProd)
            {
                bsSec = Network.BiblepayMain.CreateBitcoinSecret(sPrivKey);
            }
            else
            {
                bsSec = Network.BiblepayTest.CreateBitcoinSecret(sPrivKey);
            }
            string sSig = bsSec.PrivateKey.SignMessage(sMessage);
            string sPK = bsSec.GetAddress().ToString();
            var fSuc = VerifySignature(sPK, sMessage, sSig);
            return sSig;
        }

        
        public static double QueryAddressBalance(bool fTestNet, string sAddress)
        {
            return NBitcoin.Crypto.BBPTransaction.QueryAddressBalance(fTestNet, sAddress);
        }
        public static SharedCommon.DACResult SpendMoney(bool fTestnet, Page p, double nAmount, string sSpendAddress, string sSpendPrivKey, string sXML)
        {
            p.Session["balance"] = null;
            NBitcoin.Crypto.BBPShared.DACResult r = NBitcoin.Crypto.BBPTransaction.CreateFundingTransaction(fTestnet, nAmount, sSpendAddress, sSpendPrivKey, sXML, true);
            SharedCommon.DACResult q = new SharedCommon.DACResult();
            q.Error = r.Error;
            q.Result = r.Result;
            q.TXID = r.TXID;
            return q;
        }


        public NBitcoin.Crypto.BBPShared.DACResult DSQLInsertLegacy(bool fTestNet, ref object o, string sTable, string sID, string sSpendAddress, string sSpendPrivKey)
        {
            NBitcoin.Crypto.BBPShared.DACResult r = new NBitcoin.Crypto.BBPShared.DACResult();
            string sKey = sTable + "-" + sSpendAddress + "-" + sID;
            double nAmount = 1;
            // Add required fields:
            dynamic o1 = (System.Dynamic.ExpandoObject)o;
            o1.time = Shared.UnixTimeStamp().ToString();
            o1.id = sID;
            o1.table = sTable;
            string sData = JsonConvert.SerializeObject(o1);
            string sSig = GenerateXMLSignature(sKey, sSpendAddress, sSpendPrivKey);
            string sXML = "<MT>DSQL</MT><MK>" + sKey + "</MK><MV><dsql><OwnerAddress>" + sSpendAddress + "</OwnerAddress>"
                + "<ID>" + sID + "</ID><Time>" + UnixTimeStamp().ToString() + "</Time><data>" + sData + "</data><table>" + sTable + "</table>"
                + sSig + "</dsql></MV>";
            // Harvest Todo: Calculate Fee properly for dsql
            string sBurnAddress = NBitcoin.Crypto.BBPTransaction.GetBurnAddress(false);
            r = NBitcoin.Crypto.BBPTransaction.CreateFundingTransaction(fTestNet, nAmount, sBurnAddress, sSpendPrivKey, sXML, true);
            return r;
        }

        public NBitcoin.Crypto.BBPShared.DACResult CreateFundingTx(bool fTestNet, double nAmount, string sSpendAddress, string sSpendPrivKey, string sPayload, bool fSendForReal)
        {
            NBitcoin.Crypto.BBPShared.DACResult r = NBitcoin.Crypto.BBPTransaction.CreateFundingTransaction(fTestNet, nAmount, sSpendAddress, sSpendPrivKey, sPayload, fSendForReal);
            return r;
        }

        public static bool FilterContains(dynamic oDSQLobject, string sFilter)
        {
            bool fMyFound = false;
            sFilter = sFilter.ToLower();

            foreach (var attribute in oDSQLobject)
            {
                string sColName = attribute.Name;
                dynamic oValue = attribute.Value;
                string o1 = oValue.ToString() ?? "";
                o1 = o1.ToLower();

                if (o1 != "")
                {
                    bool f1 = (sFilter != "" && o1.Contains(sFilter)) || sFilter == "";
                    if (f1)
                    {
                        fMyFound = true;
                    }
                    if (fMyFound)
                        break;
                }
            }
            return fMyFound;
        }
}

class IO
{
        public static string GetAppDataFolder()
        {
            string sHomePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
             Environment.OSVersion.Platform == PlatformID.MacOSX) ? Environment.GetEnvironmentVariable("HOME")
                 : Environment.ExpandEnvironmentVariables("%APPDATA%");
            return sHomePath;
        }

        public static string GetSANFolder(string sFN)
        {
            string path = GetAppDataFolder() + "biblepay" + "\\SAN";
            string fullpath = Path.Combine(path, sFN);
            return fullpath;
        }

        private static string GetFolderLog(string sFileName)
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory + "Logs\\" + sFileName;
            return dir;
        }

        public static void Log(string sData, bool fQuiet = false)
        {
             try
             {
                   string sPath = GetFolderLog("biblepaydll.log");
                   System.IO.StreamWriter sw = new System.IO.StreamWriter(sPath, true);
                   string Timestamp = DateTime.Now.ToString();
                   sw.WriteLine(Timestamp + ": " + sData);
                   sw.Close();
             }
             catch (Exception ex)
             {
                   string sMsg = ex.Message;
             }
        }

        public static void WriteToFile(string path, string data)
        {
            File.WriteAllText(path, data);
            // This function is here in case we need to update the write time to UTC: (a port from BMS)
            // File.SetLastWriteTimeUtc(path, FromUnixTimeStamp(TimeStamp));
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

        public static string SubmitPart(string sAPI_KEY, string sURL, string sOriginalName, string sFileName, int iPartNumber, int iTotalParts)
        {
            try
            {
                Internals.BBPWebClient w = new Internals.BBPWebClient();
                w.Headers.Add("APIKey", sAPI_KEY);
                w.Headers.Add("Part", sFileName);
                w.Headers.Add("PartNumber", iPartNumber.ToString());
                w.Headers.Add("OriginalName", sOriginalName);
                w.Headers.Add("TotalParts", iTotalParts.ToString());
                if (iTotalParts == iPartNumber)
                    w.SetTimeout(240000);

                byte[] b = System.IO.File.ReadAllBytes(sFileName);
                byte[] e = w.UploadData(sURL, b);
                string sData = Encoding.UTF8.GetString(e, 0, e.Length);
                return sData;
            }
            catch(Exception ex)
            {
                Log("Unable to submit part #" + iPartNumber.ToString() + "; " + ex.Message);
                return "<status>0</status>";
            }
        }
    }

    public static class Splitter
    {
        public static int MAX_PARTS = 7000;

        public static bool RelinquishSpace(string sPath)
        {
            FileInfo fi = new FileInfo(sPath);
            string sDir = Path.Combine(Path.GetTempPath(), sPath.GetHashCode().ToString());
            Directory.Delete(sDir, true);
            return true;

        }

        public static string SplitFile(string sPath)
        {
            FileInfo fi = new FileInfo(sPath);

            int iPart = 0;
            string sDir = Path.Combine(Path.GetTempPath(), sPath.GetHashCode().ToString());
            
            using (Stream source = File.OpenRead(sPath))
            {
                byte[] buffer = new byte[10000000];
                int bytesRead;
                while ((bytesRead = source.Read(buffer, 0, buffer.Length)) > 0)
                {

                    string sPartPath = sDir + "\\" + iPart.ToString() + ".dat";

                    if (!System.IO.Directory.Exists(sDir))
                        System.IO.Directory.CreateDirectory(sDir);

                    Stream dest = new FileStream(sPartPath, FileMode.Create);

                    dest.Write(buffer, 0, bytesRead);
                    dest.Close();
                    iPart++;
                }
            }
            return sDir;

        }

        public static void ResurrectFile(string sFolder, string sFinalFileName)
        {
            DirectoryInfo di = new DirectoryInfo(sFolder);
            string sMasterOut = Path.Combine(sFolder, sFinalFileName);
            Stream dest = new FileStream(sMasterOut, FileMode.Create);

            for (int i = 0; i < MAX_PARTS; i++)
            {
                string sPath = di.FullName + "\\" + i.ToString() + ".dat";
                if (File.Exists(sPath))
                {
                    byte[] b = System.IO.File.ReadAllBytes(sPath);
                    dest.Write(b, 0, b.Length);

                }
                else
                {
                    break;
                }
            }
            dest.Close();
        }
    }
}

