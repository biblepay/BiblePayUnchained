using System;
using System.Data;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public partial class Admin : Page
    {
        protected void Page_Load(object sender, EventArgs e)
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
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), O, gUser(this));
            if (r.fError())
            {
                UICommon.MsgBox("Error while inserting object", "Sorry, the object was not saved: " + r.Error, this);
            }
            BiblePayCommonNET.UICommonNET.ToastLater(this, "Sucess", "Successfully added.");
        }

        protected void btnSaveRole_Click(object sender, EventArgs e)
        {
            if (gUser(this).Administrator != 1)
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
            BiblePayCommonNET.UICommonNET.ToastLater(this, "Sucess", "Successfully added.");
        }


        protected void btnSaveUserRole_Click(object sender, EventArgs e)
        {
            if (gUser(this).Administrator != 1)
            {
                UICommon.MsgBox("ERROR", "Sorry, you must be an administrator.", this);
            }

            BiblePayCommon.Entity.UserRole UR = new BiblePayCommon.Entity.UserRole();
            string sUser = Request.Form["ddusers"].ToNonNullString();
            if (sUser == "")
            {
                UICommon.MsgBox("ERROR", "User must be chosen", this);
            }
            string sRole = Request.Form["ddroles"].ToNonNullString();
            if (sRole == "")
            {
                UICommon.MsgBox("ERROR", "Role must be chosen", this);
            }
            UR.UserID = sUser;
            UR.RoleID = sRole;

            BiblePayCommon.IBBPObject o = GetObjectWithFilter(IsTestNet(this), "UserRole", "UserID='" + UR.UserID + "' and RoleID='" + UR.RoleID + "'");
            if (o.id != null)
            {
                UICommon.MsgBox("Error", "This user role already exists.", this);
            }
            UR.id = Guid.NewGuid().ToString();
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), UR, gUser(this));
            if (r.fError())
            {
                UICommon.MsgBox("Error while inserting object", "Sorry, the object was not saved: " + r.Error, this);
            }
            BiblePayCommonNET.UICommonNET.ToastLater(this, "Sucess", "Successfully added.");
        }

        protected void btnSavePermission_Click(object sender, EventArgs e)
        {
            if (gUser(this).Administrator != 1)
            {
                UICommon.MsgBox("ERROR", "Sorry, you must be an administrator.", this);
            }

            BiblePayCommon.Entity.Permission P = new BiblePayCommon.Entity.Permission();
            string sRoleOrg = Request.Form["ddroleorganization"].ToNonNullString();
            if (sRoleOrg == "")
            {
                UICommon.MsgBox("ERROR", "Organization must be chosen", this);
            }

            string sPermRole = Request.Form["ddpermissionroles"].ToNonNullString();
            if (sPermRole == "")
            {
                UICommon.MsgBox("ERROR", "Role must be chosen", this);
            }
            if (txtEntityName.Text == "")
            {
                UICommon.MsgBox("Error", "Entity Name must be chosen.", this);
            }
            P.id = Guid.NewGuid().ToString();

            BiblePayCommon.IBBPObject o = GetObjectWithFilter(IsTestNet(this), "Permission", "RoleID='" + sPermRole + "' and EntityName='" + txtEntityName.Text + "'");
            if (o.id != null)
            {
                // This is an Update; so lets pull up the old record first
                P = (BiblePayCommon.Entity.Permission)o;
            }


            P.RoleID = sPermRole;
            P.EntityName = txtEntityName.Text;
            P.ReadAccess = chkRead.Checked ? 1 : 0;
            P.AddAccess = chkAdd.Checked ? 1 : 0;
            P.DeleteAccess = chkDelete.Checked ? 1 : 0;
            P.UpdateAccess = chkUpdate.Checked ? 1 : 0;

            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), P, gUser(this));
            if (r.fError())
            {
                UICommon.MsgBox("Error while inserting object", "Sorry, the object was not saved: " + r.Error, this);
            }
            BiblePayCommonNET.UICommonNET.ToastLater(this, "Sucess", "Successfully added.");
        }

    }
}