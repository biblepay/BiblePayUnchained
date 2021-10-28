using System;
using System.Web;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommonNET.UICommonNET;
using static Unchained.Common;

namespace Unchained
{
    public partial class PetShop : BBPPage
    {
        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "BoughtPet")
            {
                string sPin = BiblePayCommon.Encryption.Base64DecodeWithFilter(e.Extra.Output.ToString());
                DACResult r30 = UICommon.BuySomething2(this, sPin);
            }
            else if (e.EventAction == "Buy_Click")
            {
                string sPub = gUser(this).BiblePayAddress;
                if (sPub == String.Empty)
                {
                    UICommon.MsgBox("Error", "Sorry, you must have a wallet in order to buy a pet.", this);
                }
                // Send an invoice to the buyer, and, send the payment to the seller.
                BiblePayCommon.Entity.invoice1 i = new BiblePayCommon.Entity.invoice1();
                i.BillFromAddress = BiblePayCommon.Encryption.GetBurnAddress(IsTestNet(this));
                i.BillToAddress = gUser(this).BiblePayAddress;
                i.Amount = e.Extra.Amount;
                i.Data = "Bought " + e.Extra.Name.ToString() + " [PET] for " + i.Amount.ToString() + " BBP.";
                i.ProductID = e.Extra.Name.ToString();
                i.ServiceName = "Universal Pet Store";
                i.InvoiceType = "Retail";
                i.InvoiceDate = System.DateTime.Now.ToString();
                UICommon.BuySomething(this, i, "BoughtPet");
            }
        }

        public string GetPet(string Name, string Notes, int nPrice, string sURL)
        {
            string sButton = UICommon.GetStandardButton(Name, "Buy Me Now", "Buy", "Buy Me Now Please!", "e.Name='" + Name + "';e.Amount='" + nPrice.ToString() + "';");
            string sDiv = "<div style='width:400;height:300'><img src='" + sURL
                + "' width=370 height=200 /><br>" + Name + " - " + Notes + "<br> " + nPrice.ToString() + " BBP - " + sButton;
            return sDiv;
        }

        protected string RetrievePets()
        {
            string html = "<table><tr><td>" + GetPet("Alfonso", "One of the worlds cutest dogs", 1000, "https://foundation.biblepay.org/Images/dog1.jpg")
            +"</td><td>" + GetPet("Amy", "She wins pageants all over the world", 2000, "https://foundation.biblepay.org/Images/dog2.jpg")
            +"</td><td>" + GetPet("Persian Cat", "An abandoned cat from the streets of Iraq", 3000, "https://foundation.biblepay.org/Images/cat1.jpg");
            html += "</table>";
            return html;
        }

        protected void btnBuy_Click(object sender, EventArgs e)
        {
            
            
        }
    }
}