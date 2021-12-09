using System;
using System.Data;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public partial class RoleAdd : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            bool fPlays = BiblePayDLL.Sidechain.UserPlaysRole(IsTestNet(this), "Superuser", gUser(this).id, "");

            if (!fPlays)
            {
                UICommon.MsgBox("ERROR", "Sorry, you must be an administrator.", this);
            }
        }


        protected void btnSaveRole_Click(object sender, EventArgs e)
        {

            bool fPlays = BiblePayDLL.Sidechain.UserPlaysRole(IsTestNet(this), "Superuser", gUser(this).id, "");

            if (!fPlays)
            {
                UICommon.MsgBox("ERROR", "Sorry, you must be an administrator.", this);
            }

            BiblePayCommon.Entity.Role R = new BiblePayCommon.Entity.Role();
            string sOrg = Request.Form["ddorganization"].ToNonNullString();
            if (sOrg == "")
            {
                UICommon.MsgBox("ERROR", "Org must be chosen", this);
            }
            if (txtRoleName.Text == "")
            {
                UICommon.MsgBox("ERROR", "Role Name must be chosen", this);
            }
            R.OrganizationID = Request.Form["ddorganization"];
            R.Name = txtRoleName.Text;

            BiblePayCommon.IBBPObject o = GetObjectWithFilter(IsTestNet(this), "Role", "OrganizationID='" + R.OrganizationID + "' and Name='" + R.Name + "'");
            if (o.id != null)
            {
                UICommon.MsgBox("Error", "This role already exists.", this);
            }


           



            R.id = Guid.NewGuid().ToString();
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), R, gUser(this));
            if (r.fError())
            {
                UICommon.MsgBox("Error while inserting object", "Sorry, the object was not saved: " + r.Error, this);
            }
            BiblePayCommonNET.UICommonNET.ToastNow(this, "Sucess", "Successfully added.");
        }


    }
}