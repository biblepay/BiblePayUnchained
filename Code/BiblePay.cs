using System.Collections.Generic;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.BiblePay;
using static BiblePayCommonNET.StringExtension;
using System.Data;
using System;
using Newtonsoft.Json;
using System.Linq;
using BiblePayCommon;
using static BiblePayCommon.Encryption;
using static Unchained.Common;
using BiblePayCommonNET;

namespace Unchained
{
    public static class BiblePayUtilities
    {
        public static void GetCryptoPrices(bool fTestNet)
        {
            string[] vTickers = sTickers.Split(",");
            for (int i = 0; i < vTickers.Length; i++)
            {
                BiblePayCommon.Entity.price1 p = GetCryptoPrice(vTickers[i]);
                // Not signed... cant do insert (MISSION CRITICAL)
                DataOps.InsertIntoTable(null, fTestNet, p, Common.CoerceUser(fTestNet));
            }
        }


        public static string ExecutePortfolioBuilderExport(bool fTestNet, int nNextHeight)
        {
            Dictionary<string, PortfolioParticipant> u = GenerateUTXOReport(fTestNet);
            string sSummary = "<data>";

            foreach (KeyValuePair<string, PortfolioParticipant> pp in u)
            {
                {
                    if (pp.Value.Strength > 0)
                    {
                        string sSummaryRow = "<row>"
                        + pp.Value.RewardAddress
                        + "<col>" + pp.Value.NickName
                        + "<col>"
                        + "<col>" + pp.Value.AmountBBP.ToString()
                        + "<col>" + pp.Value.AmountForeign.ToString()
                        + "<col>" + pp.Value.AmountUSDBBP.ToString()
                        + "<col>" + pp.Value.AmountUSDForeign.ToString()
                        + "<col>" + pp.Value.AmountUSD.ToString()
                        + "<col>" + DoubleToString(pp.Value.Coverage, 4)
                        + "<col>" + DoubleToString(pp.Value.Strength, 4)
                        + "\r\n";
                        sSummary += sSummaryRow;
                    }
                }

            }
            sSummary += "</data>";
            string sHash = "<hash>" + BiblePayCommon.Encryption.GetSha256HashI(sSummary) + "</hash>";
            DateTime dt1 = System.DateTime.UtcNow;
            string sDate = "<DATE>" + dt1.ToString("MM_dd_yy") + "</DATE>";

            sSummary += sHash;
            sSummary += sDate;
            sSummary += "<height>" + nNextHeight.ToString() + "</height>";
            sSummary += "\r\n<EOF>\r\n";
            return sSummary;
        }

        public static bool PortfolioBuilderExportExists(bool fTestNet, out int nNextSuperblockHeight)
        {
            try
            {
                string data = BiblePayDLL.Sidechain.GetGSC(fTestNet);
                if (data == "")
                {
                    nNextSuperblockHeight = 0;
                    return false;
                }

                dynamic oJson = JsonConvert.DeserializeObject<dynamic>(data);
                nNextSuperblockHeight = oJson["nextdailysuperblock"];
                string hash = (oJson["hash"] ?? "").ToString();
                return hash.Length == 64 ? true : false;

            }
            catch (Exception ex)
            {
                nNextSuperblockHeight = 0;
                Common.Log("ExportExists" + ex.Message);
                return false;
            }

        }

        public static double GetSumOfUTXOs(bool fTestNet)
        {
            Dictionary<string, PortfolioParticipant> u = GenerateUTXOReport(fTestNet);
            double nTotal = 0;
            foreach (KeyValuePair<string, PortfolioParticipant> pp in u)
            {
                nTotal += pp.Value.AmountBBP;
            }
            return nTotal;
        }

        public static double GetDWU(bool fTestNet)
        {
            double nLimit = 350000;
            BiblePayCommon.Entity.price1 nBTCPrice = GetCryptoPrice("BTC");
            BiblePayCommon.Entity.price1 nBBPPrice = GetCryptoPrice("BBP");
            double nUSDBBP = nBTCPrice.Amount * nBBPPrice.Amount;
            double nAnnualReward = nLimit * 365 * nUSDBBP;
            double nNativeTotal = GetSumOfUTXOs(fTestNet);
            double nGlobalBBPPortfolio = nNativeTotal * nUSDBBP;
            double nDWU = nAnnualReward / (nGlobalBBPPortfolio + .01);
            //LogPrintf("\nReward %f, Total %f, DWU %f, USDBBP %f ", nAnnualReward, (double)nNativeTotal / COIN, nDWU, nUSDBBP);
            if (nDWU > 2.0)
                nDWU = 2.0;
            return nDWU;
        }


        public static Dictionary<string, PortfolioParticipant> GenerateUTXOReport(bool fTestNet)
        {
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "utxostake1");
            dt = dt.FilterDataTable("owneraddress is not null");

            Dictionary<string, PortfolioParticipant> dictParticipants = new Dictionary<string, PortfolioParticipant>();

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                PortfolioParticipant pp = new PortfolioParticipant();
                bool fPortfolioParticipantExists = dictParticipants.TryGetValue(dt.Rows[i]["UserID"].ToString(), out pp);
                if (!fPortfolioParticipantExists)
                {
                    pp.lPortfolios = new List<Portfolios>();
                    dictParticipants.Add(dt.Rows[i]["UserID"].ToString(), pp);
                }

                Portfolios p = new Portfolios();
                User u = UICommon.GetUserRecord(fTestNet, dt.Rows[i]["UserID"].ToString());

                try
                {
                    p = QueryUTXOList(fTestNet, dt.Rows[i]["OwnerAddress"].ToString(), dt.Rows[i]["Ticker"].ToString().ToUpper(), dt.Rows[i]["Address"].ToString(), 0);
                }
                catch (Exception)
                {

                }

                pp.NickName = u.FullUserName();

                pp.UserID = dt.Rows[i]["UserID"].ToString();
                pp.RewardAddress = dt.Rows[i]["OwnerAddress"].ToString();


                Portfolios pTotal = new Portfolios();
                try
                {
                    pTotal = GetPortfolioSum(p);
                }
                catch (Exception)
                {

                }
                pp.AmountForeign += pTotal.AmountForeign;
                pp.AmountUSDBBP += pTotal.AmountUSDBBP;
                pp.AmountUSDForeign += pTotal.AmountUSDForeign;
                pp.AmountBBP += pTotal.AmountBBP;
                p.AmountUSDBBP = pTotal.AmountUSDBBP;
                p.AmountUSDForeign = pTotal.AmountUSDForeign;
                if (p.Ticker == "BBP" && p.Address != null)
                {
                    if (BiblePayDLL.Sidechain.ValidateAddress(fTestNet, p.Address))
                    {
                       // pp.RewardAddress = p.Address;
                    }
                }
                pp.lPortfolios.Add(p);

                pp.Coverage = pp.AmountUSDBBP / (pp.AmountUSDForeign + .01);
                if (pp.Coverage > 1)
                    pp.Coverage = 1;
                dictParticipants[dt.Rows[i]["UserID"].ToString()] = pp;

            }

            double nTotalUSD = 0;
            double nParticipants = 0;
            foreach (KeyValuePair<string, PortfolioParticipant> pp in dictParticipants.ToList())
            {
                PortfolioParticipant p1 = dictParticipants[pp.Key];
                p1.AmountUSD = pp.Value.AmountUSDBBP + (pp.Value.AmountUSDForeign * pp.Value.Coverage);
                dictParticipants[pp.Key] = p1;
                nTotalUSD += p1.AmountUSD;
                nParticipants++;
            }
            // Assign Strength
            foreach (KeyValuePair<string, PortfolioParticipant> pp in dictParticipants.ToList())
            {
                PortfolioParticipant p1 = dictParticipants[pp.Key];
                p1.Strength = p1.AmountUSD / (nTotalUSD + .01);
                dictParticipants[pp.Key] = p1;
            }

            return dictParticipants;
        }


        public static Portfolios QueryUTXOList(bool fTestNet, string sOwnerAddress, string sTicker, string sAddress, int nTimestamp)
        {
            sTicker.ToUpper();

            bool fValid = ValidateTicker(sTicker);

            Portfolios p = new Portfolios();
            p.Address = sAddress;

            if (!fValid)
                return p;

            fValid = ValidateAddress3(sTicker, sAddress);
            if (!fValid)
                return p;

            // Cache Check
            bool fExists = dictUTXO.TryGetValue(sAddress, out p);
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
                p.lPositions = BiblePayDLL.Sidechain.GetBBPUTXOs(fTestNet, sOwnerAddress, sAddress);
                p.Ticker = "BBP";
                p.Address = sAddress;
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
            string sData = ExecMVCCommand(sURL, 30, "Action", sXML);
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
                int nPin = (int)BiblePayCommon.Common.AddressToPin(sOwnerAddress, u.Address);
                bool fPin = BiblePayCommon.Common.CompareMask(u.nAmount, nPin);
                if (!fPin)
                {
                    u.nAmount = 0;
                }

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


        public static Portfolios GetPortfolioSum(Portfolios p)
        {
            Portfolios n = new Portfolios();
            n.AmountForeign = 0;
            n.AmountBBP = 0;
            for (int j = 0; j < p.lPositions.Count; j++)
            {
                BiblePayCommon.Entity.price1 prc = GetCryptoPrice(p.lPositions[j].Ticker);
                if (p.lPositions[j].Ticker == "BBP")
                {
                    n.AmountBBP += p.lPositions[j].nAmount;
                    n.AmountUSDBBP += (prc.AmountUSD * p.lPositions[j].nAmount);
                }
                else
                {
                    n.AmountForeign += p.lPositions[j].nAmount;
                    n.AmountUSDForeign += (prc.AmountUSD * p.lPositions[j].nAmount);
                }
            }
            return n;
        }

        
        public struct Portfolios
        {
            public string UserID;
            public double AmountBBP;
            public double AmountForeign;
            public double AmountUSDBBP;
            public double AmountUSDForeign;
            public double Coverage;
            public string Ticker;
            public string Nickname;
            public double Strength;
            public string Address;
            public int Time;
            public List<SimpleUTXO> lPositions;
        }

        public struct PortfolioParticipant
        {
            public string UserID;
            public string RewardAddress;
            public double AmountBBP;
            public double AmountForeign;
            public double AmountUSD;
            public double AmountUSDBBP;
            public double AmountUSDForeign;
            public double Coverage;
            public string NickName;
            public double Strength;
            public List<Portfolios> lPortfolios;
        }
        public static Dictionary<string, Portfolios> dictUTXO = new Dictionary<string, Portfolios>();


        public static bool RPC_BuyNFT(BiblePayCommon.Entity.NFT n, Page p, double nAmount, string sBuyerUserID, string NewAddress, out string sError)
        {

            bool fResult = ProcessNFT(p, n, "BUY", sBuyerUserID, NewAddress, nAmount, false, out sError);
            return fResult;
        }

        public static BiblePayCommon.Entity.NFT GetSpecificNFT(bool fTestNet, string id)
        {
            BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "NFT");
            dt = dt.FilterBBPDataTable("id='" + id + "'");
            if (dt.Rows.Count < 1)
                return null;
            BiblePayCommon.Entity.NFT n = (BiblePayCommon.Entity.NFT)BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt, "NFT", 0);
            return n;
        }

        public static bool ProcessNFT(Page p, BiblePayCommon.Entity.NFT n, string Action, string sBuyerUserID, string BuyerAddress, double BuyPrice, bool fDryRun, out string sError)
        {
            double nSend = 100;
            string sToAddress = "";
            BiblePayCommon.Entity.NFT nOldNFT = GetSpecificNFT(IsTestNet(p), n.GetHash());
            string sSellerCPK = "";
            n.Action = Action;
            if (Action == "EDIT")
            {
                if (nOldNFT == null)
                {
                    sError = "Sorry, we cannot find this nft.";
                    return false;
                }
                if (nOldNFT.UserID != n.UserID)
                {
                    sError = "Sorry, this NFT is owned by someone else.";
                    return false;
                }
                sToAddress = BiblePayCommon.Encryption.GetBurnAddress(IsTestNet(p));
                if (n.Type == "ORPHAN" && !n.fDeleted)
                {
                    sError = "Sorry, you may not edit an orphan.  But you may delete an orphan.";
                    return false;
                }
            }
            else if (Action == "CREATE")
            {
                // Verify this NFT does not exist...
                if (nOldNFT != null)
                {
                    sError = "Sorry, this NFT already exists.";
                    return false;

                }
                sToAddress = BiblePayCommon.Encryption.GetBurnAddress(IsTestNet(p));
            }
            else if (Action == "BUY")
            {
                if (nOldNFT == null)
                {
                    sError = "Sorry, we can't locate this NFT.";
                    return false;
                }
                sSellerCPK = nOldNFT.UserID;

                if (BuyPrice < nOldNFT.LowestAcceptableAmount())
                {
                    sError = "Sorry, your buy price is less than the minimum sale amount.";
                    return false;
                }
                if (!nOldNFT.Marketable || nOldNFT.fDeleted)
                {
                    sError = "Sorry, this NFT is not for sale, or, this orphan is not available for sponsorship.";
                    return false;
                }
                n.Marketable = false;
                n.fDeleted = false;

                // Clear Flags:
                n.MinimumBidAmount = 0;
                n.ReserveAmount = 0;
                n.BuyItNowAmount = 0;
                // End of Flags
                n.UserID = BuyerAddress;
                n.OwnerUserID = sBuyerUserID;

                if (!BiblePayDLL.Sidechain.ValidateAddress(IsTestNet(p), sSellerCPK))
                {
                    sError = "Invalid seller Address " + sSellerCPK;
                    return false;
                }
                if (!BiblePayDLL.Sidechain.ValidateAddress(IsTestNet(p), BuyerAddress))
                {
                    sError = "Invalid buyer Address " + BuyerAddress;
                    return false;
                }
                p.Session["pendingpurchasenft"] = n;

                sToAddress = sSellerCPK;
                nSend = BuyPrice;
            }
            else
            {
                sError = "Sorry, the action failed.";
                return false;
            }
            if (!BiblePayDLL.Sidechain.ValidateAddress(IsTestNet(p), BuyerAddress))
            {
                sError = "Invalid owner address " + BuyerAddress;
                return false;
            }
            n.nIteration++;
            n.Action = Action;

            if (Action == "EDIT" || Action == "CREATE")
            {
                BiblePayCommon.Common.DACResult r1 = DataOps.InsertIntoTable(p, IsTestNet(p), n, gUser(p));

                if (!r1.fError())
                {
                    BiblePayCommon.EntityCommon.dictTableDirty["NFT"] = true;
                    p.Session["stack"] = UICommon.Toast("Saved", "Your NFT has been Saved!  Thank you for using BiblePay NFTs!");
                    p.Response.Redirect("NFTList");
                }
                else
                {
                    UICommon.MsgBox("Error while adding NFT", "Sorry, the NFT was not saved: " + r1.Error, p);
                }
            }

            // Send the funds
            p.Session["pendingpurchasenft"] = n;
            BiblePayCommon.Entity.invoice1 i = new BiblePayCommon.Entity.invoice1();
            i.BillFromAddress = sToAddress;
            i.BillToAddress = gUser(p).BiblePayAddress;
            i.Amount = nSend;
            i.Data = "NFT " + Action + " for " + i.Amount.ToString() + " BBP.";
            i.ProductID = n.GetHash();
            i.ServiceName = "NFT";
            i.InvoiceType = "NFT";
            i.InvoiceDate = System.DateTime.Now.ToString();
            UICommon.BuySomething(p, i, "BoughtNFT");
            sError = "";
            return true;
        }


        public static double GetHighBid(Page p, string nftid)
        {
            try
            {
                BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "NFTBid");
                object nAmt = dt.Compute("max(amount)", "nftid='" + nftid + "'");
                return GetDouble(nAmt);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public static double GetNextTicketNumber(Page p)
        {
            try
            {
                BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "Ticket");
                object nAmt = dt.Compute("max([TicketNumber])", "");
                return GetDouble(nAmt) + 1;
            }
            catch (Exception ex)
            {
                return 1;
            }
        }

        public static DACResult BuyNFT1(Page p, string sUserID, string sID, double nOfferPrice, bool fBidOnly, bool fTestNet)
        {
            DACResult d = new DACResult();

            if (gUser(p).BiblePayAddress == "")
            {
                d.Error = "Sorry, you must have a wallet to buy an NFT.  ";
                return d;
            }

            BiblePayCommon.Entity.NFT myNFT = GetSpecificNFT(IsTestNet(p), sID);
            if (myNFT == null)
            {
                d.Error = "Sorry, the NFT cannot be found.";
                return d;
            }

            if (!myNFT.Marketable || myNFT.fDeleted)
            {
                d.Error = "Sorry, this NFT is not for sale.";
                return d;
            }

            if (nOfferPrice < myNFT.MinimumBidAmount)
            {
                d.Error = "Sorry this NFT has a minimum bid price of " + myNFT.MinimumBidAmount.ToString();
                return d;
            }

            if (fBidOnly && nOfferPrice >= myNFT.LowestAcceptableAmount())
            {
                fBidOnly = false;
            }
            if (fBidOnly)
            {
                double nHighBid = GetHighBid(p, myNFT.GetHash());
                if (nOfferPrice < nHighBid)
                {
                    d.Error = "Sorry, this NFT has a bid of " + nHighBid.ToString() + ", please bid higher.";
                    return d;
                }
            }

            if (fBidOnly && nOfferPrice >= myNFT.MinimumBidAmount && myNFT.Marketable && !myNFT.fDeleted)
            {
                // Just bid
                BiblePayCommon.Entity.NFTBid n1 = new BiblePayCommon.Entity.NFTBid();
                n1.UserID = gUser(p).BiblePayAddress;
                n1.NFTId = myNFT.GetHash();
                n1.Amount = nOfferPrice;
                DACResult r = DataOps.InsertIntoTable(p, IsTestNet(p), n1, gUser(p));
                return r;
            }
            else if (fBidOnly)
            {
                d.Error = "Sorry, the bid failed.  You have not been charged. ";
                return d;
            }
            // BUY
            if (gUser(p).EmailAddress == "")
            {
                d.Error = "Sorry, the bid failed.  You must have an e-mail address populated first in your user record so we can send you the NFT information.  ";
                return d;
            }

            if (nOfferPrice >= myNFT.LowestAcceptableAmount() && myNFT.Marketable && !myNFT.fDeleted)
            {
                string sError = "";
                bool bResult = RPC_BuyNFT(myNFT, p, nOfferPrice, gUser(p).id,  gUser(p).BiblePayAddress, out sError);
                if (sError == "")
                {
                    d.TXID = myNFT.GetHash();
                    return d;
                }
                else
                {
                    d.Error = "Sorry, the purchase failed.  You have not been charged.  Exception [" + sError + "]";
                    return d;
                }
            }
            return d;
        }






    }

}
