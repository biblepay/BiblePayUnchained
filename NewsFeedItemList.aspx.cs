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
    public partial class NewsFeedItemList : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                _EntityName = "NewsFeedSource";
                BindGrid();
                if (Request.QueryString["Deleted"] != null)
                {
                    lblMsg.Text = "Record deleted successfully";
                }
            }
        }


        private void BindGrid()
        {
            _EntityName = "NewsFeedSource";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), _EntityName);
            GvNewsFeedItem.DataSource = dt;
            GvNewsFeedItem.DataBind();
        }

        protected void GvNewsFeedItem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            _EntityName = "NewsFeedSource";
            if (e.CommandName == "Delete")
            {
                string Id = e.CommandArgument.ToString();

                BiblePayCommon.Entity.NewsFeedSource objNewsFeedSource = (BiblePayCommon.Entity.NewsFeedSource)Common.
                                                GetObject(Common.IsTestNet(this), _EntityName, Id);


                objNewsFeedSource.deleted = 1;
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), 
                                                    objNewsFeedSource, gUser(this));

                bool fDeleted = true; //BiblePayDLL.Sidechain.DeleteObject(IsTestNet(this), _EntityName, Id, gUser(this));
                if (fDeleted)
                {
                    UICommon.RunScriptSM(this, UICommon.Toast("Deleted", "Your Object was Deleted!"));
                    Response.Redirect("NewsFeedItemList.aspx?Deleted=1");

                }
                else
                {
                    UICommon.RunScriptSM(this, UICommon.Toast("Not Deleted", "FAILURE: The object was not deleted."));
                }
            }
            else if (e.CommandName == "Edit")
            {
                string Id = e.CommandArgument.ToString();
                Session["Id"] = Id;
                Response.Redirect("AddNewsFeedItem.aspx"); 
            }
        }

        protected void btnAddNewFeed_Click(object sender, EventArgs e)
        {
            Response.Redirect("AddNewsFeedItem.aspx");
        }
    }
}