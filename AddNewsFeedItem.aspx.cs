using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unchained.Common;


namespace Unchained
{
    public partial class AddNewsFeedItem : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

            bool fPlays = BiblePayDLL.Sidechain.UserPlaysRole(IsTestNet(this), "Superuser", gUser(this).id, "");
            if (!fPlays)
            {
                UICommon.MsgBox("Error", "You are not authorized to edit newsfeeditems: You must have superuser.", this);
            }

            _EntityName = "NewsFeedSource";
            if (!Page.IsPostBack)
            {
                if (Session["Id"] != null)
                {
                    Edit(Session["Id"].ToString());
                }
            }
        }
        private void Edit(string Id)
        {
            BiblePayCommon.Entity.NewsFeedSource objNewsFeedSource = (BiblePayCommon.Entity.NewsFeedSource)Common.
                                                GetObject(Common.IsTestNet(this), _EntityName, Id);


            txtFeedName.Text = objNewsFeedSource.FeedName;
            txtUrl.Text = objNewsFeedSource.URL;
            txtNotes.Text = objNewsFeedSource.Notes;
            txtWeight.Text = objNewsFeedSource.Weight.ToString();
            txtPoliticalLeaning.Text = objNewsFeedSource.PoliticalLeaning.ToString();
            hdnID.Value = objNewsFeedSource.id;
            Session["Id"] = null;
         }

        protected void btnSave_Click(object sender, EventArgs e)
        { 
            BiblePayCommon.IBBPObject o = (BiblePayCommon.IBBPObject)BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + _EntityName);

                        
            BiblePayCommon.EntityCommon.SetEntityValue(o, "FeedName", txtFeedName.Text);
            BiblePayCommon.EntityCommon.SetEntityValue(o, "URL", txtUrl.Text);
            BiblePayCommon.EntityCommon.SetEntityValue(o, "Notes", txtNotes.Text);
            BiblePayCommon.EntityCommon.SetEntityValue(o, "Weight", txtWeight.Text);
            BiblePayCommon.EntityCommon.SetEntityValue(o, "PoliticalLeaning", txtPoliticalLeaning.Text);
            if (hdnID.Value != "")
            {
                BiblePayCommon.EntityCommon.SetEntityValue(o, "id", hdnID.Value);
            }
            else
            {
                BiblePayCommon.EntityCommon.SetEntityValue(o, "id", Guid.NewGuid().ToString());
            }

            BiblePayCommon.EntityCommon.SetEntityValue(o, "UserID", gUser(this).id);
            
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), o, gUser(this));
            
            lblMsg.Text = "Record Added Successfully";
            Response.Redirect("NewsFeedItemList.aspx");
        }

        protected void btnList_Click(object sender, EventArgs e)
        {
            Response.Redirect("NewsFeedItemList.aspx");
        }
    }
}