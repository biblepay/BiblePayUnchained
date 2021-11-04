using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using static BiblePayCommon.Common;
using static BiblePayCommon.Encryption;

namespace BiblePayCommon
{

    public static class HalfordMemoryCache
    {
        // This is an in memory only app-cache

        public struct AppEntry
        {
            public string Key;
            public object Value;
            public System.DateTime Expiration;
        }
        public static Dictionary<string, AppEntry> dictAppCache = new Dictionary<string, AppEntry>();
        public static void Write(string sKey, object oValue, int nExpirationSeconds)
        {
            AppEntry e = new AppEntry();
            e.Key = sKey;
            e.Value = oValue;
            e.Expiration = System.DateTime.Now.AddSeconds(nExpirationSeconds);
            dictAppCache[e.Key] = e;
        }
        public static object Read(string sKey)
        {
            try
            {
                AppEntry e = new AppEntry();
                bool f = dictAppCache.TryGetValue(sKey, out e);
                if (System.DateTime.Now > e.Expiration)
                    e.Value = null;
                return e.Value;
            }catch(Exception ex)
            {
                Log2("Unable to readhalfordcache::" + ex.Message);
                return "";
            }
        }
    }



    public class HalfordDatabase : Attribute
    {

        private static int DB_ENTROPY = 2;

        // This is a disk based KV Pair database

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

        
        // The disk based application cache calls "SetKV" and "GetKV"
        public static void SetKV(string sData, string sKey)
        {
            AppendData(sData, sKey);
        }
        public static string GetKV(string sKey)
        {
            return RetrieveKV("", sKey);
        }

        public static void SetKV(string sData, string sKey, int nSeconds)
        {
            double nExpiration = UnixTimestampUTC() + nSeconds;
            string sNewData = sData + "<zcolumn>" + nExpiration.ToString();
            AppendData(sNewData, sKey);
        }


        public static double GetKVDWX(string sKey)
        {
            double n1 = GetDouble(BiblePayCommon.HalfordDatabase.GetKVWithExpiration(sKey));
            return n1;
        }

        public static void SetKVDWX(string sKey, double nValue, double nExpirationSeconds)
        {
            BiblePayCommon.HalfordDatabase.SetKV(nValue.ToString(), sKey, (int)nExpirationSeconds);
        }         




        public static string GetKVWithExpiration(string sKey)
        {

            string sData = RetrieveKV("", sKey);
            if (sData == null || sData == "")
                return "";

            if (sData.Contains("<zcolumn>"))
            {
                string[] vData = sData.Split(new string[] { "<zcolumn>" }, StringSplitOptions.None);

                double nTimestamp = GetDouble(vData[1]);
                if (UnixTimestampUTC() > nTimestamp)
                {
                    return "";
                }
                return vData[0];
            }
            return sData;
        }


        public static void AppendData(string sData, string sKey)
        {
            string  hash = GetSha256HashI(sKey);
            string dbFile = GetFolderUnchained("Database") + "\\" + hash.Substring(0, DB_ENTROPY) + ".xdat";
            string dbIndex = GetFolderUnchained("Database") + "\\" + hash.Substring(0, DB_ENTROPY) + ".xindex";
            if (!Directory.Exists(GetFolderUnchained("Database")))
            {
                Directory.CreateDirectory(GetFolderUnchained("Database"));
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
            string dbIndex = GetFolderUnchained("Database") + "\\" + sHash.Substring(0, DB_ENTROPY) + ".xindex";
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
            string dbIndex = GetFolderUnchained("Database") + "\\" + sHash.Substring(0, DB_ENTROPY) + ".xindex";
            if (!System.IO.File.Exists(dbIndex))
            {
                return new HalfordFileIndex();
            }
            System.IO.StreamReader file = new System.IO.StreamReader(dbIndex);
            string sLine = "";
            while ((sLine = file.ReadLine()) != null)
            {
                if (sLine.Substring(0, 5) != "     ")
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
            }
            file.Close();
            return new HalfordFileIndex();
        }

        public static string FetchDatabaseObjectBase(string suffix)
        {
            if (suffix == "" || suffix == null)
            {
                return null;
            }
            string myRetrieve = RetrieveKV(GetSha256HashI(suffix));
            return myRetrieve;

        }

        public static string RetrieveKV(string sHash, string sKey = "")
        {
            sHash = sHash.Replace(".xdat", "");
            if (sKey != "")
            {
                sHash = GetSha256HashI(sKey);
            }
            HalfordFileIndex i = GetIndex(sHash);
            if (i.datalength > 0)
            {
                string dbFile = GetFolderUnchained("Database") + "\\" + sHash.Substring(0, DB_ENTROPY) + ".xdat";
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
