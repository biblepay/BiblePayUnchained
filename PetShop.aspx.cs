using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unchained.StringExtension;
using static Unchained.Common;
using static BiblePayCommon.Common;

namespace Unchained
{
    public partial class PetShop : BBPPage
    {


        protected new void Page_Load(object sender, EventArgs e)
        {
           
        }

        protected override void Event(BBPEvent e)
        {
            if (e.EventAction == "Buy_Click")
            {

                string sPub = UICommon.GetBBPAddress(this);
                if (sPub == String.Empty)
                {
                    MsgBox("Error", "Sorry, you must have a wallet in order to buy a pet.", this);
                }

                string sProductID = e.EventValue; // This is for demonstration purposes;  
                // Send an invoice to the buyer, and, send the payment to the seller.
                BiblePayCommon.Entity.invoice1 i = new BiblePayCommon.Entity.invoice1();
                i.BillFromAddress = BiblePayCommon.Encryption.GetBurnAddress(IsTestNet(this));
                i.BillToAddress = gUser(this).BiblePayAddress;
                i.Amount = e.EventAmount;
                i.Data = "Bought " + e.EventValue + " [PET] for " + i.Amount.ToString() + " BBP.";
                i.ProductID = e.EventValue;
                i.ServiceName = "Universal Pet Store";
                i.InvoiceType = "Retail";
                i.InvoiceDate = System.DateTime.Now.ToString();
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(IsTestNet(this), i);

                BiblePayWallet.WalletControl w = (BiblePayWallet.WalletControl)this.Master.FindControl("wallet1");

                w.Maximized = true;
                w.BuySomething(i);
            }
        }

        public string GetPet(string Name, string Notes, int nPrice, string sURL)
        {
            string sJavascriptPrefix = "";
            
            string sButton = "<button id='btnBuy' onclick=\"" + sJavascriptPrefix 
                + "__doPostBack('Event_" + Name + "_" + "_" + Name + "_" + nPrice.ToString() + "', 'Buy_Click');\">Buy Me Now</button> ";
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