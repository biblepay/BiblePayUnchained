using BiblePayCommon;
using BiblePayCommonNET;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Web;
using System.Web.Helpers;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unchained.Common;

namespace Unchained
{
    public partial class SyncFeedItems : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

            if (!Page.IsPostBack)
            {
                _EntityName = "NewsFeedItem";
                BindGrid();
                if (Request.QueryString["Deleted"] != null)
                {
                    lblMsg.Text = "Record deleted successfully";
                }
            }
        }
        private void BindGrid()
        {
            _EntityName = "NewsFeedItem";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), _EntityName);

            _EntityName = "NewsFeedSource";
            DataTable dt2 = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), _EntityName);

            var JoinResult = (from item in dt.AsEnumerable()
                              join source in dt2.AsEnumerable()
                              on item.Field<string>("newsFeedSourceID") equals source.Field<string>("Id")
                              select new
                              {
                                  id = item.Field<string>("id"),
                                  URL = item.Field<string>("URL"),
                                  Title = item.Field<string>("Title"),
                                  Body = item.Field<string>("Body"),
                                  Expiration = item.Field<string>("Expiration"),
                                  FeedName = source.Field<string>("FeedName")
                              }).ToList();

            GvNewsFeedItem.DataSource = JoinResult;
            GvNewsFeedItem.DataBind();
        }

        protected void GvNewsFeedItem_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            _EntityName = "NewsFeedItem";
            if (e.CommandName == "Delete")
            {
                string Id = e.CommandArgument.ToString();

                BiblePayCommon.Entity.NewsFeedItem objNewsFeedItem = (BiblePayCommon.Entity.NewsFeedItem)Common.
                                                GetObject(Common.IsTestNet(this), _EntityName, Id);


                objNewsFeedItem.deleted = 1;
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this),
                                                    objNewsFeedItem, gUser(this));

                bool fDeleted = true; //BiblePayDLL.Sidechain.DeleteObject(IsTestNet(this), _EntityName, Id, gUser(this));
                if (fDeleted)
                {
                    UICommon.RunScriptSM(this, UICommon.Toast("Deleted", "Your Object was Deleted!"));
                    Response.Redirect("SyncFeedItems.aspx?Deleted=1");

                }
                else
                {
                    UICommon.RunScriptSM(this, UICommon.Toast("Not Deleted", "FAILURE: The object was not deleted."));
                }
            }

        }

        protected void btnSyncFeedItem_Click(object sender, EventArgs e)
        {
            _EntityName = "NewsFeedSource";
            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), _EntityName);
            dt = dt.OrderBy("Weight");
            foreach (DataRow dtRow in dt.Rows)
            {
                string feedName = dtRow["FeedName"].ToString();
                string url = dtRow["URL"].ToString();
                Double weight = double.Parse(dtRow["Weight"].ToString());
                string newsFeedSourceID =  dtRow["id"].ToString();

                List<Entity.NewsFeedItem> lstNewsFeedItems = GetFeedItems(feedName, url, weight);
                AddNewsFeedItem(lstNewsFeedItems, newsFeedSourceID);
                
            }

            Response.Redirect("SyncFeedItems.aspx");
        }

        private bool AddNewsFeedItem(List<Entity.NewsFeedItem> lstNewsFeedItems, string newsFeedSourceID)
        {
            bool isAdded = false;
            _EntityName = "NewsFeedItem";

            foreach (Entity.NewsFeedItem objNewsFeedItem in lstNewsFeedItems)
            {
                BiblePayCommon.IBBPObject o = (BiblePayCommon.IBBPObject)BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + _EntityName);
                BiblePayCommon.EntityCommon.SetEntityValue(o, "NewsFeedSourceID", newsFeedSourceID);
                BiblePayCommon.EntityCommon.SetEntityValue(o, "URL", objNewsFeedItem.URL);
                BiblePayCommon.EntityCommon.SetEntityValue(o, "Title", objNewsFeedItem.Title);
                BiblePayCommon.EntityCommon.SetEntityValue(o, "Body", objNewsFeedItem.Body);
                BiblePayCommon.EntityCommon.SetEntityValue(o, "ImageURL", objNewsFeedItem.ImageURL);
                BiblePayCommon.EntityCommon.SetEntityValue(o, "Expiration", objNewsFeedItem.Expiration);
                BiblePayCommon.EntityCommon.SetEntityValue(o, "id", Guid.NewGuid().ToString());
                BiblePayCommon.EntityCommon.SetEntityValue(o, "UserID", gUser(this).id);

                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), o, gUser(this));

                lblMsg.Text = "Record Added Successfully";
            }

            return isAdded;
        }


        private List<Entity.NewsFeedItem> GetFeedItems(string feedName, string url, double weight)
        {
            List<Entity.NewsFeedItem> lstNewsFeedItems = new List<Entity.NewsFeedItem>();

            try
            {
                if (Code.FeedItemMethods.Method1(url, out lstNewsFeedItems).Count() > 0)
                {
                    return lstNewsFeedItems;
                }
                else if (Code.FeedItemMethods.Method2(url, out lstNewsFeedItems).Count() > 0)
                {
                    return lstNewsFeedItems;
                }
            }
            catch (Exception ex)
            {

            }
           
            return lstNewsFeedItems;
        }

        protected void btnDeleteAll_Click(object sender, EventArgs e)
        {
            _EntityName = "NewsFeedItem";


            DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), _EntityName);

            foreach (DataRow dataRow in dt.Rows)
            {
                BiblePayCommon.Entity.NewsFeedItem objNewsFeedItem = (BiblePayCommon.Entity.NewsFeedItem)Common.
                                                    GetObject(Common.IsTestNet(this), _EntityName, dataRow["Id"].ToString());


                objNewsFeedItem.deleted = 1;
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this),
                                                    objNewsFeedItem, gUser(this));

            }
            UICommon.RunScriptSM(this, UICommon.Toast("Deleted", "Your Object was Deleted!"));
            Response.Redirect("SyncFeedItems.aspx?Deleted=1");
        }
    }
}