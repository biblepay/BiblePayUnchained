using System;
using System.Data;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public partial class OrganizationView : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

            bool fPlays = BiblePayDLL.Sidechain.UserPlaysRole(IsTestNet(this), "Superuser", gUser(this).id, "");

            if (!fPlays)
            {
                UICommon.MsgBox("ERROR", "Sorry, you must be an administrator.", this);
            }
            Populate();
        }

        protected void Populate()
        {
            string sID = Request.QueryString["id"] ?? "";
            if (sID == "")
                return;
            BiblePayCommon.IBBPObject o = GetObjectWithFilter(IsTestNet(this), "Organization", "id='" + sID + "'");
            if (o.id == null)
            {
                UICommon.MsgBox("Error", "Org not found.", this);
                return;
            }
            BiblePayCommon.Entity.Organization org = (BiblePayCommon.Entity.Organization)o;
            txtName.Text = org.Name;
            txtPublicKey.Text = org.BiblePayAddress;

        }

        protected void btnFindOrganization_Click(object sender, EventArgs e)
        {
            string sOrg = Request.Form["ddorganization"].ToNonNullString();
            if (sOrg == "")
            {
                UICommon.MsgBox("ERROR", "Org must be chosen", this);
            }
            Response.Redirect("OrganizationView?id=" + sOrg);
        }

        protected void btnEditOrganization_Click(object sender, EventArgs e)
        {

        }


    }
}