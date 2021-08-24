using System.Collections.Generic;
using System.Web.UI;
using static BiblePayCommon.Common;

namespace Unchained
{
    public static class Utils
    {
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

        static string sTickers = "BBP,DASH,BTC,DOGE,ETH,LTC,XRP,XLM,ZEC,BCH";
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

        public static Common.Portfolios QueryUTXOList(bool fTestNet, string sTicker, string sAddress, int nTimestamp)
        {
            sTicker.ToUpper();

            bool fValid = ValidateTicker(sTicker);

            Common.Portfolios p = new Common.Portfolios();
            
            if (!fValid)
                return p;

            fValid = ValidateAddress3(sTicker, sAddress);
            if (!fValid)
                return p;

            // Cache Check
            bool fExists = Common.dictUTXO.TryGetValue(sAddress, out p);
            if (fExists)
            {
                int nElapsed = UnixTimeStamp() - p.Time;
                if (p.Time < (60 * 60))
                    return p;
            }
            else
            {
                p.lPositions = new List<SimpleUTXO>();
            }

            
            if (sTicker == "BBP")
            {
                p.lPositions = BiblePayDLL.Sidechain.GetBBPUTXOs(fTestNet, sAddress);
                p.Ticker = "BBP";
                double nTotal = 0;
                for (int i = 0; i < p.lPositions.Count; i++)
                {
                    nTotal += p.lPositions[i].nAmount;
                }
                p.AmountBBP = nTotal;
                return p;
                // Todo - pull these from nft.biblepay
            }

            string sURL = "https://foundation.biblepay.org/Server?action=QUERY_UTXOS";
            string sXML = "<ticker>" + sTicker + "</ticker><address>" + sAddress + "</address><timestamp>" 
                + nTimestamp.ToString() + "</timestamp>";
            string sData = BMS.ExecMVCCommand(sURL, 30, "Action", sXML);

            string[] vL = sData.Split("<record>");
            
            for (int i = 0; i < vL.Length; i++)
            {
                SimpleUTXO u;
                u.nAmount = ExtractXML(vL[i], "<amount>", "</amount>").ToDouble();
                u.TXID = ExtractXML(vL[i], "<txid>", "</txid>");
                u.nOrdinal = ExtractXML(vL[i], "<ordinal>", "</ordinal>").ToInt();
                u.nHeight = ExtractXML(vL[i], "<height>", "</height>").ToInt();
                u.Address = ExtractXML(vL[i], "<address>", "</address>");
                u.Ticker = ExtractXML(vL[i], "<ticker>", "</ticker>");
                if (u.nAmount > 0)
                {
                    p.lPositions.Add(u);
                    p.Ticker = u.Ticker;
                    if (p.Ticker == "BBP")
                    {
                        p.AmountBBP += u.nAmount;
                    }
                    else
                    {
                        p.AmountForeign += u.nAmount;
                    }
                }
            }
            p.Address = sAddress;
            p.Time = UnixTimeStamp();

            return p;
        }
        

        public static double GetCryptoBasePrice(string sTicker)
        {
            double nPrice = BiblePayCommon.HalfordCache.Read(sTicker).ToDouble();
            if (nPrice > 0)
                return nPrice;

            string sURL = "https://foundation.biblepay.org/Server?action=" + sTicker + "_PRICE_QUOTE";
            string sData = BMS.ExecMVCCommand(sURL);
            string sMid = ExtractXML(sData, "<MIDPOINT>", "</MIDPOINT>");
            BiblePayCommon.HalfordCache.Write(sTicker, sMid.ToDouble(), 90);
            return sMid.ToDouble();
        }

        public static BiblePayCommon.Entity.price1 GetCryptoPrice(string sTicker)
        {
            BiblePayCommon.Entity.price1 p = new BiblePayCommon.Entity.price1();
            p.Amount= GetCryptoBasePrice(sTicker);
            double dUSDCryptoPrice = GetCryptoBasePrice("BTC");
            p.AmountUSD = dUSDCryptoPrice * p.Amount;
            p.Ticker = sTicker.ToUpper();
            if (sTicker.ToUpper() == "BTC")
                p.AmountUSD = dUSDCryptoPrice;
            return p;

        }
        // "BBP/BTC": "0.000000005350",  "DASH/BTC": "0.004020896000",  "LTC/BTC": 0.0035892455,  "DOGE/BTC": 5.09e-006,  "XMR/BTC": 0.0060722945,
        // "ETH/BTC": 0.067982627,   "XRP/BTC": 1.8436168e-005,   "XLM/BTC": 7.116754e-006,   "ZEC/BTC": 0.003006425602,   "BCH/BTC": 0.013754702784,
        // "BTC/USD": 39552.516601,   "DASH/USD": 159.0365557908945,   "XMR/USD": 240.174529017411,   "ETH/USD": 2688.883982997091, 
        // "XLM/USD": 0.2814855307302332,   "XRP/USD": 0.7291968408788251,   "DOGE/USD": 0.20132230949909,   "LTC/USD": 141.9636922238146,  
        // "ZEC/USD": 118.9116985327764,   "BCH/USD": 544.033110205981,   "BBP/USD": 0.00021160596381535
        public static void GCP(bool fTestNet)
        {
            string[] vTickers = sTickers.Split(",");
            for (int i = 0; i < vTickers.Length; i++)
            {
                BiblePayCommon.Entity.price1 p = GetCryptoPrice(vTickers[i]);
                DataOps.InsertIntoTable(fTestNet, p);
            }




        }
    }


}
