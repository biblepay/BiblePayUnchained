using BiblePayCommon;
using BiblePayCommonNET;
using Newtonsoft.Json;
using System;
using System.Collections;
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
    public partial class NewsView : BBPPage
    {
        private int iPageSize = 10;

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                if (Request.QueryString["PageNo"] != null)
                {
                    ViewState["PageNumber"] = Request.QueryString["PageNo"];
                }
                else
                {
                    ViewState["PageNumber"] = 1;
                }
                GetNews();
            }
        }

        private void GetNews()
        {
            _EntityName = "NewsFeedSource";
            DataTable dtDataSource = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), _EntityName);

            _EntityName = "NewsFeedItem";
            DataTable dtData = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), _EntityName);
            dtData = dtData.SortDataTable("time desc");

            List<Entity.NewsFeedItem> lstItemSource = new List<Entity.NewsFeedItem>();
            Dictionary<Entity.NewsFeedSource, List<Entity.NewsFeedItem>> ObjFeeds = new Dictionary<Entity.NewsFeedSource, List<Entity.NewsFeedItem>>();
            for (int iRowNumber = 0; iRowNumber < 5000; iRowNumber++)
            {
                foreach (DataRow item in dtDataSource.Rows)
                {
                    Entity.NewsFeedSource newsFeedSource = new Entity.NewsFeedSource();
                    newsFeedSource.FeedName = item["FeedName"].ToString();
                    newsFeedSource.id = item["id"].ToString();
                    newsFeedSource.URL = item["URL"].ToString();
                    newsFeedSource.time = Convert.ToInt64(item["time"]);
                    newsFeedSource.Weight = item["Weight"].ToDouble();
                    //newsFeedSource.PoliticalLeaning = item["PoliticalLeaning"].ToDouble();

                    string sqlWhere = $"NewsFeedSourceID={item["id"].ToString()}";
                    DataRow[] dataRows = dtData.Select(sqlWhere);

                    int iNFI = 0;
                    foreach (DataRow tempRow in dataRows)
                    {
                        if (iNFI >= iRowNumber)
                        {

                            Random random = new Random();
                            var randomWeight = random.Next(0, 100);

                            if (newsFeedSource.Weight <= randomWeight)
                            {
                                Entity.NewsFeedItem ObjNewsFeedItem = new Entity.NewsFeedItem();
                                ObjNewsFeedItem.Body = tempRow["Body"].ToString();
                                ObjNewsFeedItem.Title = tempRow["Title"].ToString();
                                ObjNewsFeedItem.ImageURL = tempRow["ImageURL"].ToString();
                                ObjNewsFeedItem.NewsFeedSourceID = tempRow["NewsFeedSourceID"].ToString();
                                ObjNewsFeedItem.id = tempRow["id"].ToString();
                                ObjNewsFeedItem.URL = tempRow["URL"].ToString();
                                ObjNewsFeedItem.time = Convert.ToInt64(tempRow["time"]);
                                ObjNewsFeedItem.Expiration = Convert.ToInt32(tempRow["Expiration"]);
                                lstItemSource.Add(ObjNewsFeedItem);
                            }
                            // break whether we have chosen one for this row or not
                            break;
                        }
                        iNFI++;
                    }
                    if (lstItemSource.Count > 0)
                        ObjFeeds.Add(newsFeedSource, lstItemSource);
                }
                // Break out when we have 50 *chosen* items:
                if (ObjFeeds.Count >= 50)
                    break;

            }


            // To display the feeds in v1.0, you can loop through the dictionary of items by time descending to start.
            // In v2.0 we can apply the politicalLeaning to the dictionary to show the user results based on their slider value (however, that will require you to load up the dictionary with more than 50 and just show the top 50 on the screen).



            DataTable dt = new DataTable();
            dt.Columns.Add("URL");
            dt.Columns.Add("Title");
            dt.Columns.Add("Body");
            dt.Columns.Add("Expiration");
            dt.Columns.Add("ImageURL");
            dt.Columns.Add("id");
            dt.Columns.Add("Display");
            dt.Columns.Add("URLCol2");
            dt.Columns.Add("TitleCol2");
            dt.Columns.Add("BodyCol2");
            dt.Columns.Add("ExpirationCol2");
            dt.Columns.Add("ImageURLCol2");
            dt.Columns.Add("idCol2");
            dt.Columns.Add("DisplayCol2");

 
            List<BiblePayCommon.Entity.NewsFeedItem> newsFeedItem = new List<Entity.NewsFeedItem>();

            int Index = 0;

            foreach (Entity.NewsFeedItem item in lstItemSource)
            {
                if (Index == 0)
                {
                    BiblePayCommon.Entity.NewsFeedItem ObjNewsFeedItem = new Entity.NewsFeedItem();
                    ObjNewsFeedItem.Body = item.Body;
                    ObjNewsFeedItem.Title = item.Title;
                    ObjNewsFeedItem.URL = item.URL;
                    ObjNewsFeedItem.Expiration = item.Expiration;
                    ObjNewsFeedItem.ImageURL = item.ImageURL;
                    newsFeedItem.Add(ObjNewsFeedItem);
                    Index += 1;
                }
                else if (Index == 1)
                {
                    var newsFeedObject = newsFeedItem.FirstOrDefault();
                    string Display = (newsFeedObject.ImageURL == null || newsFeedObject.ImageURL == "") ? "none" : "block";
                    string Display2 = (item.ImageURL == null || item.ImageURL.ToString() == "") ? "none" : "block";


                    dt.Rows.Add(newsFeedObject.URL,newsFeedObject.Title,newsFeedObject.Body,newsFeedObject.Expiration, newsFeedObject.ImageURL,newsFeedObject.id,Display,
                                item.URL, item.Title, item.Body, item.Expiration, item.ImageURL, item.id,Display2);

                    newsFeedItem.Clear();
                    Index = 0;
                } 
            }


            PagedDataSource pdsData = new PagedDataSource();
            DataView dv = new DataView(dt);
            pdsData.DataSource = dv;
            pdsData.AllowPaging = true;
            pdsData.PageSize = iPageSize;
            if (ViewState["PageNumber"] != null)
                pdsData.CurrentPageIndex = Convert.ToInt32(ViewState["PageNumber"])-1;
            else
                pdsData.CurrentPageIndex = 0;
            if (pdsData.PageCount > 1)
            {
                Repeater2.Visible = true;
                ArrayList alPages = new ArrayList();
                for (int i = 1; i <= pdsData.PageCount; i++)
                    alPages.Add((i).ToString());
                Repeater2.DataSource = alPages;
                Repeater2.DataBind();
            }
            else
            {
                Repeater2.Visible = false;
            }
            Repeater1.DataSource = pdsData;
            Repeater1.DataBind();

        }

        protected void Repeater2_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            int PageNo = Convert.ToInt32(e.CommandArgument);
            Response.Redirect($"NewsProofOfConcept.aspx?PageNo={PageNo}");
        }

        //protected string GetNews()
        //{
        //    string StrNews = "";
        //    using (WebClient wc = new WebClient())
        //    {
        //        var json = wc.DownloadString("https://www.australiannationalreview.com/wp-json/wp/v2/posts?per_page=5");

        //        var dynamicObject = Json.Decode(json);
        //        var jsonSettings = new JsonSerializerSettings()
        //        {
        //            DefaultValueHandling = DefaultValueHandling.Ignore,
        //            Formatting = Newtonsoft.Json.Formatting.Indented,
        //            TypeNameHandling = TypeNameHandling.All,
        //            NullValueHandling = NullValueHandling.Include
        //        };

        //        var dataList = JsonConvert.DeserializeObject<List<ExpandoObject>>(json, jsonSettings);

        //        List<News> LstNews = new List<News>();
        //        int Index = 0;
        //        string sHTML = "<table class='news'>";
        //        foreach (var item in dataList)
        //        {
        //            News news = new News();

        //            var Title = dataList[Index].ToList().Where(o => o.Key == "title").FirstOrDefault();

        //            news.Title = ((ExpandoObject)Title.Value).FirstOrDefault(x => x.Key == "rendered").Value.ToString();
        //            news.Link = dataList[Index].ToList().Where(o => o.Key == "link").FirstOrDefault().Value.ToString();
        //            news.Body = ((ExpandoObject)((ExpandoObject)dataList[Index]).ToList()[24].Value)
        //                        .FirstOrDefault(o => o.Key == "og_description").Value.ToString();
        //            LstNews.Add(news);

        //            Index += 1;

        //            sHTML += "<tr><td>";
        //            sHTML += "<a target='_blank' href='" + news.Link + "'><h2 class='headline'>" + news.Title + "</h2></a><br>";
        //            sHTML += "<span class='headline'>" + news.Body + "</span><br><br><a target='_blank' href='" + news.Link + "' style='text-decoration: underline;'>Read more</a><br><br>";
        //            sHTML += "</td>";
        //            sHTML += "</tr>";
        //        }

        //        StrNews = sHTML;
        //    }
        //    return StrNews;
        //}
    }
}