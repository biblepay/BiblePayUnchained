using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BiblePayCommon.Common;

namespace BiblePayCommonNET
{
    public static class BiblePay
    {
        public struct SimpleUTXO
        {
            public double nAmount;
            public string TXID;
            public int nOrdinal;
            public int nHeight;
            public string Address;
            public string Ticker;
        };


        public static string ExecMVCCommand(string URL, int iTimeout = 30, string sOptionalHeaderName = "", string sOptionalHeaderValue = "")
        {
            BiblePayCommon.BiblePayClient wc = new BiblePayCommon.BiblePayClient();
            try
            {
                wc.SetTimeout(iTimeout);
                if (sOptionalHeaderName != "")
                    wc.Headers.Add(sOptionalHeaderName, sOptionalHeaderValue);
                string d = wc.FetchObject(URL).ToString();
                return d;
            }
            catch (Exception)
            {
                Console.WriteLine("Exec MVC Failed for " + URL);

                return "";
            }
        }

        public static double GetCryptoBasePrice(string sTicker)
        {
            double nPrice = BiblePayCommon.HalfordMemoryCache.Read(sTicker).ToDouble();
            if (nPrice > 0)
                return nPrice;

            string sURL = "https://foundation.biblepay.org/Server?action=" + sTicker + "_PRICE_QUOTE";
            string sData = ExecMVCCommand(sURL);
            string sMid = ExtractXML(sData, "<MIDPOINT>", "</MIDPOINT>");
            BiblePayCommon.HalfordMemoryCache.Write(sTicker, sMid.ToDouble(), 90);
            return sMid.ToDouble();
        }

        public static BiblePayCommon.Entity.price1 GetCryptoPrice(string sTicker)
        {
            BiblePayCommon.Entity.price1 p = new BiblePayCommon.Entity.price1();
            p.Amount = GetCryptoBasePrice(sTicker);
            double dUSDCryptoPrice = GetCryptoBasePrice("BTC");
            p.AmountUSD = dUSDCryptoPrice * p.Amount;
            p.Ticker = sTicker.ToUpper();
            if (sTicker.ToUpper() == "BTC")
                p.AmountUSD = dUSDCryptoPrice;
            return p;
        }


        public static string GetArrayElementAsString(string data, string delim, int iEle)
        {
            string[] vData = data.Split(delim);
            if (iEle <= vData.Length - 1)
            {
                string d = vData[iEle];
                return d;
            }
            return "";
        }

        public static string sTickers = "BBP,DASH,BTC,DOGE,ETH,LTC,XRP,XLM,ZEC,BCH";
        public static bool ValidateTicker(string sTicker)
        {
            string[] vTickers = sTickers.Split(",");
            for (int i = 0; i < vTickers.Length; i++)
            {
                if (vTickers[i] == sTicker)
                    return true;
            }
            return false;
        }


        public static bool ValidateAddressLength(string sAddress, int nReqLength)
        {
            int nLen = sAddress.Length;
            if (nLen != nReqLength)
                return false;
            return true;
        }

        public static bool ValidateAddress3(string sTicker, string sAddress)
        {
            if (sAddress.IsNullOrEmpty())
                return false;

            bool fValidateTicker = ValidateTicker(sTicker);
            if (!fValidateTicker)
                return false;

            if (sTicker == "DASH" || sTicker == "BTC" || sTicker == "DOGE" || sTicker == "BBP")
            {
                if (ValidateAddressLength(sAddress, 42))
                    return true;
                return ValidateAddressLength(sAddress, 34);
            }
            else if (sTicker == "LTC")
            {
                if (ValidateAddressLength(sAddress, 34))
                {
                    return true;
                }
                else
                {
                    return ValidateAddressLength(sAddress, 43);
                }
            }
            else if (sTicker == "ETH" || sTicker == "BCH")
            {
                return ValidateAddressLength(sAddress, 42);
            }
            else if (sTicker == "XRP")
            {
                return ValidateAddressLength(sAddress, 34);
            }
            else if (sTicker == "XLM")
            {
                return ValidateAddressLength(sAddress, 56);
            }
            else if (sTicker == "ZEC")
            {
                return ValidateAddressLength(sAddress, 35);
            }

            return false;
        }

    }

}
