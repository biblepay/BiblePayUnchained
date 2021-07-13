using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static BiblePayDLL.Shared;

namespace BiblePayDLL
{
    public static class Encryption
    {
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
                int iMyKey = (int)GetDouble(vKey[i]);
                myBytedKey[i] = (byte)(iMyKey + 0);
            }
            for (int i = 0; i < vIV.Length; i++)
            {
                int iMyIV = (int)GetDouble(vIV[i]);
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

    }
    public static class Sidechain
    {
        public static string lastblockhash = "";
        // Security: Hierarchy:  Org_CPK -> Table -> Json_Object
        // Required Fields: Lastblockhash
        // Chain Integrity: Load tbl.bestblockhash, get hash
        // Load object->get lastblockhash->load object...
        // Scalability: Multiple sidechains (one per table name)
        // Performance: Multithreaded object loading, one thread per table

        public static int GetVersion()
        {
            return 1002;
        }

        public static List<string> GetHashes(bool fTestNet)
        {
            return NBitcoin.Crypto.BiblePay.GetHashList(fTestNet);
        }
        public static void UpdateChainTip(bool fTestNet, string sTable, string lastblockhash, int nHeight, string sPubKey, string sPrivKey)
        {
            NBitcoin.Crypto.BiblePay.UpdateChainTip(fTestNet, sTable, lastblockhash, nHeight, sPubKey, sPrivKey);
            UpdateTipHeight(sTable, nHeight);
        }
        public static void UpdateTipHeight(string sTable, int nHeight)
        {
            AppendData(nHeight.ToString(), sTable + "_tipheight");
        }

        public static int GetStoredTipHeight(string sTable)
        {
            int nHeight = (int)GetDouble(RetrieveKV("", sTable + "_tipheight"));
            return nHeight;
        }
        
        public static dynamic NewtonSoftToExpando(dynamic oNewton)
        {
            var o = new ExpandoObject() as IDictionary<string, Object>;
            foreach (var attribute in oNewton)
            {
                string sColName1 = attribute.Name;
                dynamic oValue = attribute.Value;
                o.Add(sColName1, oValue);
            }
            return o;
        }

        public static string StoreTempData(string sFilename, string sData)
        {
            string sFullPath = Path.Combine(Path.GetTempPath(), sFilename);
            IO.WriteToFile(sFullPath, sData);
            return sFullPath;
        }

        public static void EraseTempData(string sPath)
        {
            try
            {
                System.IO.File.Delete(sPath);
            }
            catch(Exception)
            {

            }
        }

        public static SharedCommon.DACResult UploadFileTypeVideo(bool fTestNet, string sFullPath, string sSpendAddress, string sSpendPrivKey)
        {
            // Spend 100 bbp per video, or fail.
            SharedCommon.DACResult rOut = new SharedCommon.DACResult();

            NBitcoin.Crypto.BBPShared.DACResult r1 = NBitcoin.Crypto.BiblePay.UploadFilebase(fTestNet, NBitcoin.Crypto.BiblePay.FileType.VIDEO, sFullPath, sSpendAddress, sSpendPrivKey);
            if (r1.Error != "")
            {
                rOut.Error = r1.Error;
                return rOut;
            }
            dynamic o = new System.Dynamic.ExpandoObject();
            FileInfo fi = new FileInfo(sFullPath);
            o.Size = fi.Length;
            o.FileName = fi.Name;
            o.DirectoryName = fi.DirectoryName;
            o.URL = r1.Result;
            string sID = Internals.Internal.GetSha256HashI(o.URL);
            rOut = SidechainDSQLInsert(fTestNet, ref o, "object", sID, sSpendAddress, sSpendPrivKey);
            if (rOut.Error != "")
            {
                return rOut;
            }
            // Return the video object so they can see the URL
            rOut.Result = r1.Result;
            return rOut;
        }
        public static SharedCommon.DACResult UploadFileTypeDSQL(bool fTestNet, string sFullPath, string sSpendAddress, string sSpendPrivKey)
        {
            // Spend 5 bbp per DSQL, or fail.
            SharedCommon.DACResult rOut = new SharedCommon.DACResult();
            NBitcoin.Crypto.BBPShared.DACResult r = NBitcoin.Crypto.BiblePay.UploadFilebase(fTestNet, NBitcoin.Crypto.BiblePay.FileType.DSQL, sFullPath, sSpendAddress, sSpendPrivKey);
            rOut.Error = r.Error;
            rOut.Result = r.Result;
            return rOut;
        }

        public static string GetChosenSanctuary(bool fTestNet)
        {
            return NBitcoin.Crypto.BBPTransaction.GetChosenSanctuary(fTestNet);
        }

        public static bool DownloadStoredObject(string sURL, string sNewLocation)
        {
            return NBitcoin.Crypto.BiblePay.DownloadStoredObject(sURL, sNewLocation);
        }
        public static SharedCommon.DACResult SidechainDSQLInsert(bool fTestNet, ref object o, string sTable, string sID, string sSpendAddress, string sSpendPrivKey)
        {
            SharedCommon.DACResult r = new SharedCommon.DACResult();
            string sChain = fTestNet ? "TestNet" : "MainNet";
            string sKey = sChain + "-" + sSpendAddress + "-" + sTable + "-" + sID;
            // Add required fields:
            dynamic o1 = (System.Dynamic.ExpandoObject)o;
            o1.time = Shared.UnixTimeStamp().ToString();
            o1.id = sID;
            o1.table = sTable;
            o1.chain = sChain;
            o1.lastblockhash = NBitcoin.Crypto.BiblePay.GetLastBlockHash(sTable);
            string sData = JsonConvert.SerializeObject(o1);
            string hash =  Internals.Internal.GetSha256HashI(sData);
            // Is it too big
            if (sData.Length > 256000)
            {
                r.Error = "Data too big.";
                return r;
            }

            string filename = hash + ".xdat";
            string sFullPath = StoreTempData(filename, sData);
            // Check to make sure its not already stored
            string data2 = FetchDatabaseObject(hash);
            if (data2 != null && data2 != "" && data2.Length > 32)
            {
                r.Error = "Data already exists";
                return r;
            }
 
            r = Sidechain.UploadFileTypeDSQL(fTestNet, sFullPath, sSpendAddress, sSpendPrivKey);
            if (r.Error != "")
            {
                return r;
            }
            string sURL = r.Result;
            // Verify
            string myVerify = FetchDatabaseObject(hash);
            string hashVerify = Internals.Internal.GetSha256HashI(myVerify);
            // 1. Verify the lastblockhash being pointed to matches an actual object 
            // 2. Verify the lastblockhash being referenced is at the correct height
            int nHeight = GetTipHeight(sTable);
            int nStoredHeight = GetStoredTipHeight(sTable);
            if (hashVerify == hash && nHeight >= nStoredHeight)
            {
                // Update last hash
                UpdateChainTip(fTestNet, sTable, hash, nHeight, sSpendAddress, sSpendPrivKey);
                EraseTempData(sFullPath);
                if (sURL.Length > 1)
                {
                    r.TXID = hash;
                    r.Error = "";
                }
            }
            else
            {
                r.Error = "Unable to verify hash.";
            }
            return r;
        }

        public static int GetTipHeight(string sTable)
        {
            string hash = NBitcoin.Crypto.BiblePay.GetLastBlockHash(sTable);
            if (hash == null || hash == "")
                return 0;

            int i = 0;
            while (true)
            {
                string data = FetchDatabaseObject(hash);
                dynamic oD = JsonConvert.DeserializeObject<dynamic>(data);
                /*
                var sID = (oD["id"] ?? "").ToString();
                var sTable1 = (oD["table"] ?? "").ToString();
                var sUserName = (oD["UserName"] ?? "").ToString();
                var sParentID = (oD["ParentID"] ?? "").ToString();
                */
                var sLastBlockHash = (oD["lastblockhash"] ?? "").ToString();
                hash = sLastBlockHash;
                i++;
                if (hash == "" || hash == null) 
                    break;
            }
            return i;
        }

        public static DataTable RetrieveDataTable2(bool fTestNet, string sTable, string sFilterID, string sFilterParentID, string sRequiredColumns, string sFilterContains, string sFilterContains2)
        {
            DataTable dt = new DataTable();
            string sChain = fTestNet ? "TestNet" : "MainNet";
            try
            {
                sFilterContains2 = sFilterContains2.ToLower();
                int nRow = 0;
                dt.Clear();
                string[] vRCols = sRequiredColumns.Split(new string[] { "," }, StringSplitOptions.None);
                string hash = NBitcoin.Crypto.BiblePay.GetLastBlockHash(sTable);

                int i = 0;
                while (true)
                {
                    string data = FetchDatabaseObject(hash);
                    if (data == null || data == "")
                        break;

                    dynamic oDSQLObject = JsonConvert.DeserializeObject<dynamic>(data);
                    hash = (oDSQLObject["lastblockhash"] ?? "").ToString();
                    i++;
                    double nTime = GetDouble((oDSQLObject["time"] ?? "").ToString());
                    var sID = (oDSQLObject["id"] ?? "").ToString();
                    var sTable1 = (oDSQLObject["table"] ?? "").ToString();
                    var sUserName = (oDSQLObject["UserName"] ?? "").ToString();
                    var sParentID = (oDSQLObject["ParentID"] ?? "").ToString();
                    var sURL1 = (oDSQLObject["URL"] ?? "").ToString();
                    var sObjChain = (oDSQLObject["chain"] ?? "").ToString();
                    string sMyTime = Shared.ConvertFromUnixTimestamp(nTime).ToString();

                    if (nTime != 0 && sID != "" && sTable != "" && sChain == sObjChain)
                    {
                         bool fInclude = true;
                         if (sTable != "" && sTable != sTable1)
                                fInclude = false;
                         if (sFilterID != "" && sFilterID != sID)
                                fInclude = false;
                         if (sFilterParentID != "" && sFilterParentID != sParentID)
                                fInclude = false;
                         if (sRequiredColumns != "" && sRequiredColumns.Contains("username") && sUserName == "")
                                fInclude = false;
                         if (sURL1.Contains("pdf"))
                         {
                             string sSub = (oDSQLObject["Subject"] ?? "").ToString();
                         }
                         if (sFilterContains != "" || sFilterContains2 != "")
                         {
                              bool fMyFound = false;
                              bool f1 = Data.FilterContains(oDSQLObject, sFilterContains);
                              bool f2 = Data.FilterContains(oDSQLObject, sFilterContains2);
                                fMyFound = f1 && f2;
                                if (!fMyFound)
                                {
                                    fInclude = false;
                                }
                            }

                         if (fInclude)
                         {
                                if (nRow == 0)
                                {
                                    // We need to populate the column name here
                                    foreach (var attribute in oDSQLObject)
                                    {
                                        string sColName = attribute.Name;
                                        dynamic oValue = attribute.Value;
                                        dt.Columns.Add(sColName);
                                    }
                                    dt.Columns.Add("objecthash");
                                }
                                DataRow _newrow = dt.NewRow();
                                foreach (var attribute in oDSQLObject)
                                {
                                    string sColName1 = attribute.Name;
                                    dynamic oValue = attribute.Value;
                                    _newrow[sColName1] = oValue;
                                    _newrow["objecthash"] = hash;
                                }
                                // Ensure all the columns exist, then add:
                                bool bFoundCol = true;
                                for (int j1 = 0; j1 < vRCols.Length; j1++)
                                {
                                    bool bF1 = false;
                                    for (int icol = 0; icol < dt.Columns.Count; icol++)
                                    {
                                        string sDTColName = dt.Columns[icol].ColumnName.ToLower();
                                        if (vRCols[j1].ToLower() == sDTColName)
                                        {
                                            bF1 = true;
                                            break;
                                        }
                                    }
                                    if (!bF1)
                                        bFoundCol = false;
                                }

                                if (bFoundCol)
                                {
                                    dt.Rows.Add(_newrow);
                                    nRow++;
                                }
                            }
                        }
                    }
                return dt;

            }
            catch (Exception ex)
            {
                IO.Log("unable to retrieve data in getdatatable::" + ex.Message);
                return dt;
            }
        }

        public class HalfordDatabase : Attribute
        {
            // Halford KV Pair database
        }

        [HalfordDatabase]
        private static void RemoveOldIndex(HalfordFileIndex ind)
        {
            string sIndex = ind.filename;
            var fs = new FileStream(sIndex, FileMode.Open, FileAccess.ReadWrite);
            string sData = "-1".PadRight(32);
            byte[] b = Encoding.ASCII.GetBytes(sData);
            fs.Position = ind.start;
            fs.Write(b, 0, b.Length);
            fs.Close();
        }

        private static void AppendData(string sData, string sKey = "")
        {
            string hash = Internals.Internal.GetSha256HashI(sData);
            if (sKey != "")
            {
                hash = Internals.Internal.GetSha256HashI(sKey);
            }
            string dbFile = Internals.Internal.GetFolderUnchained("Database") + "\\" + hash.Substring(0, 1) + ".xdat";
            string dbIndex = Internals.Internal.GetFolderUnchained("Database") + "\\" + hash.Substring(0, 1) + ".xindex";
            if (!Directory.Exists(Internals.Internal.GetFolderUnchained("Database")))
            {
                Directory.CreateDirectory(Internals.Internal.GetFolderUnchained("Database"));
            }
            long nStart = 0;
            if (System.IO.File.Exists(dbFile))
            {
                FileInfo fi = new FileInfo(dbFile);
                nStart = fi.Length;
            }
            string index = hash + "|" + dbFile + "|" + nStart.ToString() + "|" + sData.Length.ToString() + "|||||<MYEOF1>\r\n".PadRight(100);
            HalfordFileIndex OldIndex = GetIndexOfIndex(hash);
            if (OldIndex.datalength > 0)
            {
                // This old item exists... remove the key, then add the new key....
                RemoveOldIndex(OldIndex);
            }
            byte[] b = Encoding.ASCII.GetBytes(sData);
            var fileStream = new FileStream(dbFile, FileMode.Append, FileAccess.Write, FileShare.None);
            var bw = new BinaryWriter(fileStream);
            bw.Write(b);
            bw.Write("\r\n<MY_EOF_MONIKER>\r\n");
            bw.Close();
            fileStream.Close();
            // Write index
            System.IO.StreamWriter sw = new System.IO.StreamWriter(dbIndex, true);
            sw.WriteLine(index);
            sw.Close();
        }

        private static void InternalStoreValue(string sData)
        {
            AppendData(sData);
        }

        public static string GetElement(string data, string delimiter, int n)
        {
            string[] vE = data.Split(new string[] { delimiter }, StringSplitOptions.None);

            if (vE.Length > n)
            {
                return vE[n];
            }
            return "";
        }
        public struct HalfordFileIndex
        {
            public string hash;
            public int start;
            public string filename;
            public int datalength;
        };

        private static HalfordFileIndex GetIndexOfIndex(string sHash)
        {
            string dbIndex = Internals.Internal.GetFolderUnchained("Database") + "\\" + sHash.Substring(0, 1) + ".xindex";
            if (!System.IO.File.Exists(dbIndex))
            {
                return new HalfordFileIndex();
            }
            System.IO.StreamReader file = new System.IO.StreamReader(dbIndex);
            string sLine = "";
            int iPos = 0;
            while ((sLine = file.ReadLine()) != null)
            {
                HalfordFileIndex i = new HalfordFileIndex();
                i.hash = GetElement(sLine, "|", 0);
                i.filename = GetElement(sLine, "|", 1).Replace(".xdat", ".xindex");
                i.start = iPos;
                i.datalength = (int)GetDouble(GetElement(sLine, "|", 3));
                if (i.hash == sHash)
                {
                    file.Close();
                    return i;
                }
                iPos += sLine.Length + 2;
            }
            file.Close();
            return new HalfordFileIndex();
        }
        
        private static HalfordFileIndex GetIndex(string sHash)
        {
            string dbIndex = Internals.Internal.GetFolderUnchained("Database") + "\\" + sHash.Substring(0, 1) + ".xindex";
            if (!System.IO.File.Exists(dbIndex))
            {
                return new HalfordFileIndex();
            }
            System.IO.StreamReader file = new System.IO.StreamReader(dbIndex);
            string sLine = "";
            while ((sLine = file.ReadLine()) != null)
            {
                HalfordFileIndex i = new HalfordFileIndex();
                i.hash = GetElement(sLine, "|", 0);
                i.filename = GetElement(sLine, "|", 1);
                i.start = (int)GetDouble(GetElement(sLine, "|", 2));
                i.datalength = (int)GetDouble(GetElement(sLine, "|", 3));
                if (i.hash == sHash)
                {
                    file.Close();
                    return i;
                }
            }
            file.Close();
            return new HalfordFileIndex();
        }

        public static string FetchDatabaseObject(string suffix)
        {
            if (suffix == "" || suffix == null)
            {
                return null;
            }
            string myRetrieve = RetrieveKV(suffix);
            if (myRetrieve != null)
            {
                return myRetrieve;
            }
            if (!suffix.Contains(".xdat"))
            {
                suffix += ".xdat";
            }

            string d = NBitcoin.Crypto.BiblePay.FetchStoredObject(suffix);
            AppendData(d);
            return d;
        }

        public static string RetrieveKV(string sHash, string sKey = "")
        {
            sHash = sHash.Replace(".xdat", "");
            if (sKey != "")
            {
                sHash = Internals.Internal.GetSha256HashI(sKey);
            }
            HalfordFileIndex i = GetIndex(sHash);
            if (i.datalength > 0)
            {
                string dbFile = Internals.Internal.GetFolderUnchained("Database") + "\\" + sHash.Substring(0, 1) + ".xdat";
                var fs = new FileStream(dbFile, FileMode.Open);
                
                fs.Seek(i.start, SeekOrigin.Begin);
                byte[] bits = new byte[i.datalength];
                fs.Read(bits, 0, i.datalength);
                fs.Close();
                string result = System.Text.Encoding.UTF8.GetString(bits);
                return result;
            }
            return null;
        }
    }
}
