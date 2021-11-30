using System;
using System.Data;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public partial class OrganizationAdd : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {

         
        }
        



        protected void btnSaveOrganization_Click(object sender, EventArgs e)
        {

            if (gUser(this).Administrator != 1)
            {
                UICommon.MsgBox("ERROR", "Sorry, you must be an administrator.", this);
            }

            if (txtOrganizationName.Text == "" || txtDomainName.Text == "")
            {
                UICommon.MsgBox("ERROR", "Org and Domain must be populated.", this);
            }
            BiblePayCommon.Entity.Organization O = new BiblePayCommon.Entity.Organization();
            O.Domain = txtDomainName.Text;
            O.Name = txtOrganizationName.Text;
            BiblePayCommon.IBBPObject oOld = GetObjectWithFilter(IsTestNet(this), "Organization", "Name='" + O.Name + "'");
            if (oOld.id != null)
            {
                UICommon.MsgBox("Error", "This Organization already exists.", this);
            }


            O.id = Guid.NewGuid().ToString();
            O.BiblePayAddress = txtBiblePayAddress.Text;
            if (!BiblePayDLL.Sidechain.ValidateAddress(IsTestNet(this),O.BiblePayAddress))
            {
                UICommon.MsgBox("Error", "Bad public key.", this);
            }

            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), O, gUser(this));
            if (r.fError())
            {
                UICommon.MsgBox("Error while inserting object", "Sorry, the object was not saved: " + r.Error, this);
            }
            BiblePayCommonNET.UICommonNET.ToastNow(this, "Sucess", "Successfully added.");
        }


    }
}