using System;
using System.Data;
using static BiblePayCommon.DataTableExtensions;
using static Unchained.Common;
using static BiblePayCommon.Common;
using MongoDB.Driver;
using static BiblePayCommon.EntityCommon;

namespace Unchained
{
    public partial class PrayerView : BBPPage
    {

        protected new void Page_Load(object sender, EventArgs e)
        {
            this.Title = _CollectionName + " - Add";
        }
        public string GetPrayer()
        {
            // Displays the prayer that the user clicked on from the web list.
            string id = Request.QueryString["id"] ?? "";
            if (id == "")
                return "N/A";

            var builder = Builders<dynamic>.Filter;
            var filter = builder.Eq("_id", id);
            
            dynamic dt = BiblePayDLL.Sidechain.GetChainObjects<dynamic>(IsTestNet(this), _EntityName, filter,
                SERVICE_TYPE.PUBLIC_CHAIN);

            if (dt.Count < 1)
            {
                UICommon.MsgBox("Not Found", "We are unable to find this object.", this);
                return "";
            }

            string sBody = " <textarea class='comments' id=txtbody rows='10' cols='65' readonly>" + dt[0].Body + "</textarea>";

            string div = "<table class='comments'>"
                + "<tr><th class='objheader' colspan=3><h3>" + _ObjectName + " - View</h3><th class='objheader' colspan=3><div class='prayer'>"
                + UICommon.GetStandardAnchor(id, "DeleteObject", id, "<i class='fa fa-trash'></i>","View " + _EntityName, _EntityName ) 
                + "</div></th></tr>"
                + "<tr><td width=10%>User:<td>" + UICommon.GetUserAvatarAndName(this, dt[0].UserID) 
                + "<tr><td>Added:<td>" + BiblePayCommon.Common.UnixTimeStampToDisplayAge(dt[0].time) + "</td></tr>"
                + "<tr><td>Subject:<td>" + dt[0].Subject + "</td></tr>"
                + "<tr><td>Body:<td colspan=2>" + sBody + "<br>" 
                + GetObjectRating(IsTestNet(this), id, _EntityName, gUser(this), _EntityName) + "</td></tr></table>";
            div += UICommon.GetComments(IsTestNet(this), id, this, _EntityName);
            return div;
        }
    }
}