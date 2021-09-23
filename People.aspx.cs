using System;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.CommonNET;
using static Unchained.Common;

namespace Unchained
{
    public partial class People : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

        }


        protected override void Event(BBPEvent e)
        {

        }

        protected string GetPeople()
        {
            BiblePayCommon.BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable2(IsTestNet(this), "user1");
            // Reserved:Filter by Domain:
            if (txtSearch.Text != "")
            {
                dt = dt.FilterBBPDataTable("FirstName like '%" + txtSearch.Text
                    + "%' or LastName like '%" + txtSearch.Text + "%' or TelegramLinkName like '%"
                    + txtSearch.Text + "' or TelegramLinkURL like '%" + txtSearch.Text + "%' or TelegramLinkDescription like '%" + txtSearch.Text + "%'");
            }
            dt = dt.FilterBBPDataTable("PublicText is not null");
            string html = UICommon.GetUserGallery(this, dt, paginator1, 3);
            return html;

        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            // Refine the query
        }

    }
}
