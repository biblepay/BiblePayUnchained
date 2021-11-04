using BiblePayCommon;
using BiblePayCommonNET;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;
using static Unchained.DataOps;

namespace Unchained
{
    public partial class NFTBrowse : BBPPage
    {

        // NOTE: This code was PORTED from Biblepay-QT, thats why it looks similar to c++ 

        protected void chkDigital_Changed(object sender, EventArgs e)
        {

        }
        protected void chkTweet_Changed(object sender, EventArgs e)
        {

        }

        public static bool SessionToBool(HttpSessionState s, string sKey)
        {
            if (s[sKey] == null)
                return false;
            if (s[sKey].ToNonNullString() == "1")
                return true;
            return false;
        }
        public static string FreezerImage(string sURL)
        {
            string sFullURL = "http://api.screenshotlayer.com/api/capture?access_key=" + Config("freezerkey")
                + "&url=" + HttpUtility.UrlEncode(sURL) + "&viewport=1440x900&width=350&format=png";
            return sFullURL;
        }

        public static double GetScalarAge(Page p, string id, string table, string groupbycolumn, string maxcolumn)
        {
            // SQL Equivalent of : Select id,max(maxcolumn) group by groupbycolumn
            BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), table);
            dt = dt.FilterBBPDataTable(groupbycolumn + "='" + id + "'");
            var query = from row in dt.AsEnumerable()
                        group row by row.Field<string>(groupbycolumn) into grp
                        select new
                        {
                            nftid = grp.Key,
                            Id = grp.First().Field<string>("id"),
                            max1 = grp.Select(r => r.Field<string>(maxcolumn)).Max()
                        };
            foreach (var grp in query)
            {
                //Console.WriteLine(String.Format("The Max of '{0}' is {1}& {2}", grp.Id, grp.nftid, grp.max1));
                dt = dt.FilterBBPDataTable("id='" + grp.Id + "'");
                double nAdded = GetDouble(dt.Rows[0]["time"]);
                double nDiff = UnixTimestampUTC() - nAdded;
                return nDiff;
            }
            return 0;
        }


        public static List<BiblePayCommon.Entity.NFT> GetListOfNFTs(Page p, string sTypes)
        {
            List<BiblePayCommon.Entity.NFT> nList = new List<BiblePayCommon.Entity.NFT>();
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "NFT");
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                BiblePayCommon.Entity.NFT n = (BiblePayCommon.Entity.NFT)BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt, "NFT", i);
                if (sTypes.Contains(n.Type) && !n.fDeleted)
                {
                    nList.Add(n);
                }
            }
            return nList;
        }


        public static string GetNFTDisplayList(Page p, bool fOrphansOnly, bool fDigital, bool fTweet)
        {
            string sTypes = "";
            if (fOrphansOnly)
            {
                sTypes = "ORPHAN";
            }
            else
            {
                sTypes += fDigital ? "DIGITALGOODS," : "";
                sTypes += fTweet ? "SOCIALMEDIA" : "";
            }
            List<BiblePayCommon.Entity.NFT> n = GetListOfNFTs(p, sTypes);
            string sHTML = "<table><tr>";
            int iTD = 0;
            int nColsPerRow = 3;
            int nObjsPerPage = 4 * nColsPerRow;
            int nStartRow = 0 * nObjsPerPage;
            int nEndRow = nStartRow + nObjsPerPage - 1;
            int nRows = n.Count / nColsPerRow;
            double nTotalPages = (int)Math.Ceiling((double)(nRows / nObjsPerPage)) + 1;
            for (int i = nStartRow; i < nEndRow && i < n.Count; i++)
            {
                // Each Orphan should be a div with their picture in it
                string sURLLow1 = n[i].LowQualityURL;
                string sURLLow = "";
                if (false && n[i].Type.Contains("SOCIALMEDIA"))
                {
                    sURLLow = FreezerImage(sURLLow1);
                }
                else
                {
                    sURLLow = sURLLow1;
                }

                string sURLHi = n[i].HighQualityURL;
                string sName = n[i].Name;
                string sDesc = n[i].Description;
                string sID = n[i].GetHash();
                double nBid = BiblePayUtilities.GetHighBid(p, sID);
                bool fOrphan = n[i].Type.Contains("Orphan");
                string sBuyItCaption = fOrphansOnly ? "Sponsor Me for " : "Buy it Price ";

                if (sURLLow.Contains("hrefhttps://"))
                {
                    sURLLow = "https://foundation.biblepay.org/Images/404.png";
                }
                if (n[i].Marketable)
                {
                    string sButtonCaption = fOrphansOnly ? "Sponsor Now" : "Buy it Now";
                    string sSubCaption = fOrphansOnly ? "Sponsor this Orphan now" : "Purchase this NFT now";
                    string sBuyItNowPrice = n[i].BuyItNowAmount.ToString() + " BBP";
                    string sPurchaseCaption = "Are you sure you want to " + sSubCaption + " for " + sBuyItNowPrice + "?";
                    string sButton = UICommon.GetStandardButtonWithConfirm(sID, sButtonCaption, "btnBuyNFT", sPurchaseCaption, sPurchaseCaption);
                    string sPreviewURL = sURLLow + "?id=" + sID;
                    string sPreviewButton = "<input type='reset' onclick=\"window.open('" + sPreviewURL + "');\" value='Preview' />";

                    string sBidButton = "<button onclick=\"var amt=prompt('Please enter the bid "
                        + "amount you are offering', '0'); var Extra = {}; Extra.Amount = amt; Extra.Event='btnBidNFT_Click'; Extra.Value='" 
                        + sID + "';BBPPostBack2(null, Extra); return true;"
                        + "\" value='Make Offer'>Make Offer</button>";

                    string sAsset = "<iframe style='height: 200px;width:300px;' src='" + sURLLow + "'></iframe>";
                    if (sURLLow.Contains(".gif") || sURLLow.Contains(".jpg") || sURLLow.Contains(".jpeg") || sURLLow.Contains(".png"))
                    {
                        sAsset = "<img style='height:200px;width:300px;' src='" + sURLLow + "'/>";
                    }
                    else if (sURLLow.Contains(".mp4") || sURLLow.Contains(".mp3"))
                    {
                        sAsset = "<video style='height:200px;width:300px;' controls><source src='" + sURLLow + "' />        </video>";
                    }
                    string sScrollY = sDesc.Length > 500 ? "overflow-y:scroll;" : "";

                    if (sDesc.Length > 550)
                    {
                        //sDesc = Left(sDesc, 550) + " ...";
                    }
                    string s1 = "<td style='padding:7px;border:1px solid white' cellpadding=7 cellspacing=7>"
                        + "<b>" + sName + " • " + UnixTimeStampToDateControl(n[i].time) + "</b><br>" + sAsset
                        + "<br><div style='border=1px;height:75px;width:310px;" + sScrollY + "'><font style='font-size:11px;'>"
                        + sDesc + "</font></div><br><small><font color=green>" + sBuyItCaption + " " + sBuyItNowPrice;
                    if (!fOrphansOnly)
                    {
                        s1 += "&nbsp;•&nbsp;High Offer: " + nBid.ToString() + " BBP";
                    }
                    else
                    {
                        s1 += "&nbsp;•&nbsp;<small>" + sID.Substring(0, 8) + "</small>";

                    }

                    s1 += "<br>" + sButton;
                    if (!fOrphansOnly)
                    {
                        s1 += sBidButton;
                    }
                    s1 += "&nbsp;" + sPreviewButton + "</td>";
                    sHTML += s1;
                    iTD++;
                    if (iTD > nColsPerRow)
                    {
                        iTD = 0;
                        sHTML += "<td width=30%>&nbsp;</td></tr><tr>";
                    }
                }

                if (nBid > n[i].ReserveAmount && nBid > n[i].LowestAcceptableAmount() && n[i].Marketable)
                {
                    double nAge = GetScalarAge(p, n[i].GetHash(), "NFTBid", "nftid", "Amount");
                    if (nAge > (60 * 60 * 24 * 14))
                    {
                        BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(p), "nftbid");
                        dt = dt.FilterBBPDataTable("nftid='" + sID + "' and amount > " + n[i].LowestAcceptableAmount().ToString());
                        if (dt.Rows.Count > 0)
                        {
                            string sHighBidUser = dt.Rows[0]["UserID"].ToString();
                            if (sHighBidUser == gUser(p).BiblePayAddress)
                            {
                                BiblePayCommon.Entity.NFTBid n1 = (BiblePayCommon.Entity.NFTBid)BiblePayCommon.EntityCommon.TableRowToStronglyCastObject(dt, "NFTBid", 0);
                                n1.deleted = 1;
                                InsertIntoTable(p, IsTestNet(p), n1, gUser(p));

                                Log("Auto buying NFT " + n1.NFTId.ToString());

                                BiblePayUtilities.BuyNFT1(p, gUser(p).BiblePayAddress, n1.NFTId.ToString(), n1.Amount, false, IsTestNet(p));
                            }
                        }
                    }
                }
            }
            sHTML += "</TR></TABLE>";
            var uri = new Uri(p.Request.Url.AbsoluteUri);
            string path = uri.GetLeftPart(UriPartial.Path);
            string sURL = path + "?t=1";
            return sHTML;
        }

        protected override void Event(BBPEvent e)
        {

            if (e.EventAction == "btnBuyNFT_Click" && e.EventValue.Length > 10)
            {


                if (!gUser(this).LoggedIn)
                {
                    MsgModal(this, "NFT Buy Error", "Sorry, you must log in first to buy an NFT.", 450, 200);
                    return;
                }
                BiblePayCommon.Entity.NFT myNFT = BiblePayUtilities.GetSpecificNFT(IsTestNet(this), e.EventValue);
                if (myNFT == null)
                {
                    UICommon.MsgBox("Error", "Sorry, this NFT cannot be found!", this);
                    return;
                }
                DACResult d = BiblePayUtilities.BuyNFT1(this, gUser(this).id, e.EventValue, myNFT.BuyItNowAmount, false, IsTestNet(this));

            }
             else if (e.EventAction == "BoughtNFT")
            {
                string sPin = BiblePayCommon.Encryption.Base64DecodeWithFilter((e.Extra.Output ?? "").ToString());
		DACResult r30 = UICommon.BuySomething2(this, sPin);
                if (!r30.fError())
                {
                    BiblePayCommon.Entity.NFT n = (BiblePayCommon.Entity.NFT)Session["pendingpurchasenft"];
                    BiblePayCommon.Common.DACResult s = DataOps.InsertIntoTable(this, IsTestNet(this), n, gUser(this));
                    string sNarr = "You have successfully purchased NFT " + n.GetHash() + " on TXID " + s.Result + "!";
                    // Send the email, especially if its an orphan
                    BiblePayCommon.Entity.invoice1 i = (BiblePayCommon.Entity.invoice1)this.Session["PENDING_PURCHASE"];
                    UICommon.NotifyOfSale(this, IsTestNet(this), gUser(this), n, i.Amount, s.Result);
                    UICommon.MsgBox("Success", sNarr, this);
                }
                else
                {
                    UICommon.MsgBox("Purchase Failed", "Error", this);
                }
            }
            else if (e.EventAction == "btnBidNFT_Click" && e.EventValue.Length > 10)
            {
                if (!gUser(this).LoggedIn)
                {
                    MsgModal(this, "NFT Bid Error", "Sorry, you must log in first to bid on an NFT.", 450, 200, true);
                    return;
                }


                DACResult d = BiblePayUtilities.BuyNFT1(this, gUser(this).id, e.EventValue, GetDouble(e.Extra.Amount), true, IsTestNet(this));
                if (d.fError())
                {
                    MsgModal(this, "NFT Bid Error", d.Error, 450, 200, true);
                    return;
                }
                else
                {
                    MsgModal(this, "Success", "You have bidded " + e.Extra.Amount.ToString() + " BBP on this NFT.", 450, 250, true, true);
                    return;
                }
        
            }
        }

        protected new void Page_Load(object sender, EventArgs e)
        {

        }
    }
}