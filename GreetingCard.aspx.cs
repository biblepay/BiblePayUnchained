using static Unchained.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;
using static Unchained.DataOps;
using static BiblePayCommon.Common;
using static BiblePayCommonNET.UICommonNET;
using static BiblePayCommonNET.StringExtension;
using static BiblePayCommonNET.CommonNET;

namespace Unchained
{
    public partial class GreetingCard : BBPPage
    {
		protected new void Page_Load(object sender, EventArgs e)
		{
			if (!IsPostBack)
			{
				ddCardType.Items.Clear();
				ddCardType.Items.Add("Christmas Card");
				ddCardType.Items.Add("Easter Card");
				ddCardType.Items.Add("Kittens Card");
				string sInfo = "<small><font color=red>Note: It costs $1 + gift-card-value to deliver a card.  Example:  Card with no gift:  $1; Card with $5 gift = $6.00. </small>";
				lblInfo.Text = sInfo;
				ddCardType_SelectedIndexChanged(this, null);
			}

		}



		protected void ddCardType_SelectedIndexChanged(object sender, System.EventArgs e)
		{

			string sType = ddCardType.SelectedValue;
			if (sType == "Christmas Card")
			{
				txtOpeningSalutation.Text = "Merry Christmas!";
				txtClosingSalutation.Text = "Warm Christmas Greetings";
				txtParagraph1.Text = "Peace and Joy to you and your family this Christmas Season.  We are thinking warmly of you.  May your family be blessed with the Richest Blessings of Abraham, Isaac and Jacob. ";

			}
			else if (sType == "Easter Card")
			{
				txtOpeningSalutation.Text = "Happy Easter!";
				txtClosingSalutation.Text = "He is Risen";
				txtParagraph1.Text = "Peace and Joy to you and your family this Easter and Passover week! We are thinking warmly of you.  May your family be blessed with the Richest Blessings of Abraham, Isaac and Jacob. ";
			}
			else if (sType == "Kittens Card")
			{
				txtOpeningSalutation.Text = "May Kittens Surround You!";
				txtClosingSalutation.Text = "With Kitten Love";
				txtParagraph1.Text = "I've been thinking of you and had to write you.";
			}
		}

		void clear()
		{
			txtRecipientName.Text = "";
			txtParagraph1.Text = "";
			txtParagraph2.Text = "";
			txtOpeningSalutation.Text = "";
			txtClosingSalutation.Text = "";
	        txtAddress1.Text = "";
			txtAddress2.Text = "";
			txtCity.Text = "";
			txtState.Text = "";
			txtZipCode.Text = "";
			txtAmount.Text = "";
			chkDeliver.Checked = true;
		}

		string CleanseMe(string sStr)
		{
			sStr = sStr.Replace( "\t", "");
			sStr = sStr.Replace( "\r\n", "<br>");
			sStr = sStr.Replace( "\r", "<br>");
			sStr = sStr.Replace( "\n", "<br>");
			return sStr;
		}


		void BtnCSVClicked()
		{
			string sFileName = ""; // GUIUtil::getOpenFileName(this, tr("Select CSV file to import"), "", "", nullptr);

			if (sFileName == "")
			{
				return;
			}
			List<DMAddress> dm = null; // ImportGreetingCardCSVFile(sFileName);
			int iQuestion = 0;
			bool fConfirming = true;
			bool fCancelled = false;
			for (int i = 0; i < dm.Count(); i++)
			{
				string sRow = dm[i].Name + "," + dm[i].AddressLine1 + "," + dm[i].City + "," + dm[i].State + "," + dm[i].Zip;
				if (fConfirming)
				{
					iQuestion++;
					string sConfirm = "Warning #" + iQuestion.ToString()
						+ ":  <br>We will confirm the first 5 addresses, giving you a chance to cancel the batch.  <br><br>After the fifth prompt, we will process the rest unless you CANCEL.  <br><br>"
						+ "The next record looks like: " + sRow + ".  <br><br>Press [OK] to process this record and continue, or [CANCEL] to cancel the entire batch.  ";

					//MsgInput::warning(this, tr("Automated Mail Delivery System"), GUIUtil::TOQS(sConfirm), QMessageBox::Ok | QMessageBox::Cancel, QMessageBox::Ok);
				}
				if (!fCancelled)
				{
					txtRecipientName.Text = dm[i].Name;
					txtAddress1.Text = dm[i].AddressLine1;
					txtAddress2.Text = dm[i].AddressLine2;
					txtCity.Text = dm[i].City;
					txtState.Text = dm[i].State;
					txtZipCode.Text = dm[i].Zip;
					btnSend_Click(this, null);
				}
			}
		}

		public class DMAddress
		{
			public string Name;
			public string AddressLine1;
			public string AddressLine2;
			public string Template;
			public string City;
			public string State;
			public string Zip;
			public string Paragraph1;
			public string Paragraph2;
			public string OpeningSalutation;
			public string ClosingSalutation;
			public double Amount;
			public DMAddress()
			{
				Name = "";
				AddressLine1 = "";
				AddressLine2 = "";
				City = "";
				State = "";
				Zip = "";
				Amount = 0;
			}
		};

		string SerializeDMAddress(DMAddress d)
		{
			string sXML = "<Name>" + d.Name + "</Name><AddressLine1>" + d.AddressLine1 + "</AddressLine1><AddressLine2>" 
				+ d.AddressLine2 + "</AddressLine2><City>" + d.City + "</City><State>" + d.State + "</State><Zip>" + d.Zip + "</Zip>";
			return sXML;
		}

		public double GetBBPValueUSD(double nUSDAmount)
		{
			BiblePayCommon.Entity.price1 eBBP = BiblePayCommonNET.BiblePay.GetCryptoPrice("BBP");
			double nUSD = nUSDAmount / (eBBP.AmountUSD + .000001);
			return nUSD;
		}

		DACResult MailLetter(DMAddress dmFrom, DMAddress dmTo, string sTXID, bool fDryRun)
		{
			string sXML = "<cpk>" +gUser(this).BiblePayAddress + "</cpk><paragraph1>" + dmTo.Paragraph1 + "</paragraph1><paragraph2>"
				+ dmTo.Paragraph2 + "</paragraph2><Template>" + dmTo.Template + "</Template>" + "<from>" + SerializeDMAddress(dmFrom)
				+ "</from>" + "<to>" + SerializeDMAddress(dmTo) + "<OpeningSalutation>" + dmTo.OpeningSalutation + "</OpeningSalutation><ClosingSalutation>"
				+ dmTo.ClosingSalutation + "</ClosingSalutation></to><dryrun>"
				+ (fDryRun ? "1" : "0") + "</dryrun><txid>" 
				+ sTXID + "</txid>";

			string sXML2 =BiblePayCommon.Encryption.Base64Encode(sXML);
			string sURL = "https://foundation.biblepay.org/Server?action=MAIL";
			string sData = BiblePayCommonNET.BiblePay.ExecMVCCommand(sURL, 40, "Action", sXML2);
			DACResult b = new DACResult();
			b.Result = sData;
			return b;
		}

		private DMAddress DeserializeAddress(string sAddress)
		{
			DMAddress d1 = new DMAddress();
			string[] vAddress = sAddress.Split(",");
			if (vAddress.Length < 5)
				return d1;
			d1.Name = vAddress[0];
			d1.AddressLine1 = vAddress[1];
			d1.City = vAddress[2];
			d1.State = vAddress[3];
			d1.Zip = vAddress[4];
			return d1;
		}

		protected override void Event(BBPEvent e)
		{
			if (e.EventAction == "BoughtGreetingCard")
			{
				string sPin = BiblePayCommon.Encryption.Base64DecodeWithFilter(e.Extra.Output.ToString());

				DACResult r30 = UICommon.BuySomething2(this, sPin);
				if (!r30.fError())
				{
					DACResult b = MailLetter((DMAddress)Session["_dmFrom"], (DMAddress)Session["_dmTo"], e.EventValue, !chkDeliver.Checked);
					string sError = BiblePayCommon.Common.ExtractXML(b.Result, "<error>", "</error>");
					string sPDF = BiblePayCommon.Common.ExtractXML(b.Result, "<pdf>", "</pdf>");
					string sSuffix = chkDeliver.Checked ? "This greeting card will be physically delivered. " : "";
					string sNarr = (sError == "") ? "<small>Successfully created Mail Delivery "
						+ ". <br><small><a target='_blank' href='" + sPDF + "' style='text-decoration:none;color:pink;font-size:150%'>"
						+ "<b>Click HERE to Review PROOF</b></a><br>" + sSuffix + "<br>Thank you for using BiblePay Physical Mail Delivery." : sError;

					lblInfo.Text = sNarr;
					MsgModalWithLinks(this, "Mail Delivery Result", sNarr, 600, 300, true);
				}
				else
				{
					//Error already on the screen
				}
			}
		}

		

		protected void btnSend_Click(object sender, EventArgs e)
        {
			DMAddress _dmFrom;
			DMAddress _dmTo;


			string sFromAddress = BMS.GetCookie("setaddress");
			_dmFrom = DeserializeAddress(sFromAddress);
			_dmTo = new DMAddress();
			_dmTo.Name = txtRecipientName.Text;
			_dmTo.AddressLine1 = txtAddress1.Text;
			_dmTo.AddressLine2 = txtAddress2.Text;
			_dmTo.City = txtCity.Text;
			_dmTo.State = txtState.Text;
			_dmTo.Zip = txtZipCode.Text;
			double dSrcAmount = GetDouble(txtAmount.Text);
			_dmTo.Amount = dSrcAmount;
			_dmTo.Paragraph1 = CleanseMe(txtParagraph1.Text);
			_dmTo.Paragraph2 = CleanseMe(txtParagraph2.Text);
			_dmTo.OpeningSalutation = txtOpeningSalutation.Text;
			_dmTo.ClosingSalutation = txtClosingSalutation.Text;
			bool fDryRun = !(chkDeliver.Checked);
			string sError = "";
			if (_dmTo.Name.Length < 3)
				sError += "Name must be populated. ";

			
			if (_dmFrom.Name.Length < 3 || _dmFrom.AddressLine1.Length < 4 || _dmFrom.City.Length < 4 || _dmFrom.State.Length < 2 || _dmFrom.Zip.Length < 4)
			{
				sError += "From information is not populated.  Please set your address, from the RPC console, with:<br><br> <code>setaddress \"name,address,city,state,zip\"   </code><br><br><br>";
			}

			if (gUser(this).BiblePayAddress == "")
				sError = "Sorry, you must have a wallet in order to buy a greeting card.";

			if (_dmTo.AddressLine1.Length < 4 || _dmTo.City.Length < 3 || _dmTo.State.Length < 2 || _dmTo.Zip.Length < 4)
				sError += "Sorry, for the Destination address:  You must enter the name, address, city, state and zip. ";
			_dmTo.Template = ddCardType.SelectedValue;

			if (_dmTo.Template ==  "")
						sError += "Type must be chosen. ";
			if (_dmTo.Name.Length > 128)
							sError += "Name must be < 128 chars.";
	    	if (_dmTo.Paragraph1.Length > 777)
						sError += "Paragraph 1 length must be < 777 chars.";
			if (_dmTo.Paragraph2.Length > 777)
						sError += "Paragraph 2 length must be < 777 chars.";
			string sTXID = "";
			if (dSrcAmount > 0 && sError == "")
			{
   				//DACResult d = MakeDerivedKey(dmTo.AddressLine1);
				string sPayload = "<giftcard>" + _dmTo.Amount.ToString() + "</giftcard>";
				//Virtual gift card payment here	sTXID = RPCSendMessage(dmTo.Amount, d.Address, fDryRun, sError, sPayload);
				string sPhrase = "<span style='white-space: nowrap;'><font color=lime> \"" + _dmTo.AddressLine1 + "\"</font></span>";
				string sInstructions = "<br><small>To redeem your gift, download biblepay Desktop PC wallet from www.biblepay.org | Tools | Debug Console | acceptgift " + sPhrase + "</small>";
				string sNarr1 = "<p><br>A gift of $" + dSrcAmount.ToString() 
					      + " (" + _dmTo.Amount.ToString()  + " BBP) has been sent to you!<br>" + sInstructions;
				_dmTo.Paragraph2 = _dmTo.Paragraph2 + sNarr1;
			}
			string sPDF = "";
			DACResult b = new DACResult();
			double nCost = 0;
			if (!fDryRun)
			{
				nCost = GetBBPValueUSD(1);
				if (nCost > 250000)
					nCost = 250000;
				string sPayload = "<mail>" + nCost.ToString() + "</mail>";
				//send the $1 fee::sTXID = RPCSendMessage(nCost, sFeeAddress, fDryRun, sError, sPayload);
			}

			if (sError == "")
			{
				// Todo Harvest: change true to fDryRun = !fSend

				Session["_dmFrom"] = _dmFrom;
				Session["_dmTo"] = _dmTo;

				if (true)
				{
					if (nCost == 0)
						nCost = 1;
					if (fDryRun)
						nCost = 1;

					string sProductID = "Greeting Card";
					BiblePayCommon.Entity.invoice1 i = new BiblePayCommon.Entity.invoice1();
					i.BillFromAddress = BiblePayCommon.Encryption.GetBurnAddress(IsTestNet(this));
					i.BillToAddress = gUser(this).BiblePayAddress;
					i.Amount = nCost;
					i.Data = "Bought " + sProductID  + " for " + i.Amount.ToString() + " BBP.";
					i.ProductID = sProductID;
					i.ServiceName = "BiblePay Greeting Cards";
					i.InvoiceType = "GreetingCard";
					i.InvoiceDate = System.DateTime.Now.ToString();
					
					UICommon.BuySomething(this, i, "BoughtGreetingCard");
					
				}
			}
			else
			{
				MsgModal(this, "Error", sError, 400, 250);
			}

		}
	}
}
