using System;
using static Unchained.Common;
using static BiblePayCommon.Common;
using System.Web.UI.WebControls;
using BiblePayCommon;
using System.Web.UI;
using static BiblePayCommonNET.UICommonNET;
using static BiblePayCommonNET.StringExtension;

namespace Unchained
{
	public partial class NFTAdd : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

			if (gUser(this).LoggedIn == false)
			{
				UICommon.MsgBox("Error", "Sorry, you must be logged in first.", this);
			}
            if (!IsPostBack)
            {
                ddNFTType.Items.Clear();
                ddNFTType.Items.Add(new System.Web.UI.WebControls.ListItem("Digital Goods (MP3, PNG, GIF, JPEG, PDF)","DIGITALGOODS"));
                ddNFTType.Items.Add(new ListItem("Social Media (Tweet, Post, URL)", "SOCIALMEDIA"));
                ddNFTType.Items.Add(new ListItem("Orphan (Child to be sponsored)", "ORPHAN"));
                txtOwnerAddress.Text = gUser(this).BiblePayAddress.ToString();
            }
			string id = Request.QueryString["id"].ToNonNullString();
			string action = Request.QueryString["action"].ToNonNullString();
			UpdateDisplay(action, id);
        }
    

		private string _msMode = "";
		protected void UpdateDisplay(string sAction, string id)
		{
			if (sAction == "")
				sAction = "CREATE";
			sAction = sAction.ToUpper();
			// Action == CREATE or EDIT
			_msMode = sAction;
			if (IsPostBack)
				return;

			BiblePayCommon.Entity.NFT n = new BiblePayCommon.Entity.NFT();
			if (id != "")
			{
				n = BiblePayUtilities.GetSpecificNFT(IsTestNet(this), id);
			}
			if (sAction == "CREATE")
			{
				lblAction.Text = "Add new NFT";
			}
			else if (sAction == "EDIT")
			{
				lblAction.Text = "Edit NFT";
				lblAction.Text = "Edit NFT " + n.GetHash();
				txtName.Text = n.Name;
				txtDescription.Text = n.Description;
				txtLowQualityURL.Text = n.LowQualityURL;
				txtHighQualityURL.Text = n.HighQualityURL;
				txtReserveAmount.Text = n.ReserveAmount.ToString();
				txtBuyItNowAmount.Text = n.BuyItNowAmount.ToString();
				txtMinimumBidAmount.Text = n.MinimumBidAmount.ToString();
				ckMarketable.Checked = n.Marketable;
				ckDeleted.Checked = n.fDeleted;
				ddNFTType.SelectedValue = n.Type;
			}
			string sInfo = "Note: It costs 100 BBP to create or edit an NFT.";
			lblInfo.Text = sInfo;
		}

		protected void clear()
		{
			txtName.Text = "";
			txtDescription.Text = "";
			txtLowQualityURL.Text = "";
			txtHighQualityURL.Text = "";
			txtMinimumBidAmount.Text = "0";
			txtReserveAmount.Text = "0";
			txtBuyItNowAmount.Text = "0";
			ckDeleted.Checked = false;
			ckMarketable.Checked = false;
		}


		protected void btnSubmit_Click(object sender, EventArgs e)
		{
			BiblePayCommon.Entity.NFT n = new BiblePayCommon.Entity.NFT();

			n.Name = txtName.Text;
			n.UserID = txtOwnerAddress.Text;
			n.MinimumBidAmount = GetDouble(txtMinimumBidAmount.Text);
			n.ReserveAmount = GetDouble(txtReserveAmount.Text);
			n.BuyItNowAmount = GetDouble(txtBuyItNowAmount.Text);
			n.LowQualityURL = txtLowQualityURL.Text;
			n.HighQualityURL = txtHighQualityURL.Text;
			n.Action = _msMode;

			n.Description = txtDescription.Text;
			n.Marketable = ckMarketable.Checked;
			n.fDeleted = ckDeleted.Checked;
			n.OwnerUserID = gUser(this).id;

			string sError = "";
			if (n.Name.Length < 3)
				sError += "NFT Name must be populated. ";

			if (n.Description.Length < 5)
				sError += "NFT Description must be populated. ";

			if (n.LowQualityURL.Length < 10 || n.HighQualityURL.Length < 10)
				sError += "You must enter an asset URL. ";

			n.Type = ddNFTType.SelectedValue;
			if (n.Type == "" || n.Type == null)
				sError += "NFT Type must be chosen. ";

			if (n.Name.Length > 128)
				sError += "NFT Name must be < 128 chars.";

			if (n.Description.Length > 512)
				sError += "NFT Description must be < 512 chars.";
			if (n.LowQualityURL.Length > 512 || n.HighQualityURL.Length > 1024)
				sError += "URL Length must be < 512 (lo) and 1024 (hi) chars.";

			string sTXID = "";
			bool fCreated = Unchained.BiblePayUtilities.ProcessNFT(this, n, _msMode, gUser(this).id, n.UserID, 0, false, out sError);

			string sNarr = (sError == "") ? "Successfully " + _msMode + "(ed) NFT " + sTXID + ". <br><br>Thank you for using BiblePay Non Fungible Tokens." : sError;
			MsgModal(this, "Process NFT", sNarr, 400, 300);

			if (sError == "")
			{
				clear();
			}
		 }
	  }
 }
