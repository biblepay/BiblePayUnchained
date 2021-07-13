using NBitcoin;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static BiblePayDLL.Shared;

namespace BiblePayDLL
{
    public static class Shared
    {

        public struct KeyType
        {
            public string PrivKey;
            public string PubKey;
        }
        public static KeyType DeriveNewKey(bool fTestNet, string sSha)
        {
            NBitcoin.Mnemonic m = new NBitcoin.Mnemonic(sSha);
            ExtKey k = m.DeriveExtKey(null);
            KeyType k1 = new KeyType();
            k1.PrivKey  = k.PrivateKey.GetWif(fTestNet ? Network.BiblepayTest : Network.BiblepayMain).ToWif().ToString();
            k1.PubKey = k.ScriptPubKey.GetDestinationAddress(fTestNet ? Network.BiblepayTest : Network.BiblepayMain).ToString();
            return k1;
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

        public static int UnixTimeStamp()
        {
            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
            return unixTimestamp;
        }
    }
}
