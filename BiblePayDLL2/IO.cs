using NBitcoin;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

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
}


namespace BiblePayDLL
{

    public class Data
    {

        string GenerateXMLSignature(bool fTestNet, string sPrimaryKey, string sSigningPublicKey, string sPrivKey)
        {
            string sBBPSig;
            string sXML;
            sBBPSig = SignChrome(fTestNet, sPrivKey, sPrimaryKey);
            sXML = "<BOSIGNER>" + sSigningPublicKey + "</BOSIGNER><BOSIG>" + sBBPSig + "</BOSIG><BOMSG>" + sPrimaryKey + "</BOMSG>";
            return sXML;
        }

        private static bool VerifySignature(bool fTestNet, string BBPAddress, string sMessage, string sSig)
        {
            if (BBPAddress == null || sSig == String.Empty)
                return false;
            try
            {
                // Determine the network:
                BitcoinPubKeyAddress bpk;
                if (!fTestNet)
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
        private static string SignChrome(bool fTestNet, string sPrivKey, string sMessage)
        {
            if (sPrivKey == null || sMessage == String.Empty || sMessage == null)
                return string.Empty;

            BitcoinSecret bsSec;
            if (!fTestNet)
            {
                bsSec = Network.BiblepayMain.CreateBitcoinSecret(sPrivKey);
            }
            else
            {
                bsSec = Network.BiblepayTest.CreateBitcoinSecret(sPrivKey);
            }
            string sSig = bsSec.PrivateKey.SignMessage(sMessage);
            string sPK = bsSec.GetAddress().ToString();
            var fSuc = VerifySignature(fTestNet, sPK, sMessage, sSig);
            return sSig;
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

    public class Bible
    {
        private NBitcoin.Crypto.BibleHash _kjv = new NBitcoin.Crypto.BibleHash();

        public string GetVerse(string sBook, int iChapter, int iVerse)
        {
            string sBook1 = _kjv.GetBookByName(sBook);

            int iStart = 0;
            int iEnd = 0;
            _kjv.GetBookStartEnd(sBook, ref iStart, ref iEnd);

            string h = _kjv.GetVerse(sBook, iChapter, iVerse, iStart, iEnd);
            return h.Trim();
            
        }
        public List<string> GetBookList()
        {
            return _kjv.GetBookList();
        }
        public string GetBookByName(string sBook)
        {
            return _kjv.GetBookByName(sBook);
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
     
        public static void Log(string sData, bool fQuiet = false)
        {
           BiblePayCommon.Common.Log2(sData, fQuiet);
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

