using BiblePayCommon;
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
            if (gUser(this).Administrator != 1)
            {
                UICommon.MsgBox("Error", "You are not authorized", this);
            }

            string sID = Request.QueryString["Organization"] ?? "";
            dynamic o = GetObjectWithFilter(IsTestNet(this), "Organization", "id='" + sID + "'");
            if (o != null)
            {
                lblOrganizationName.Text = o.Name;
            }
            // Does the current user play the role by name, and, is the Server authorized to Execute this role?
            // Add configuration keys too (Key for d660,feature path, value)
            bool fPlays = BiblePayDLL.Sidechain.UserPlaysRole(IsTestNet(this), "LocalhostOrg2Admin", gUser(this).id);
        }


        protected void btnListUserRoles_Click(object sender, EventArgs e)
        {
            string sUser = Request.Form["ddusers_list"].ToNonNullString();
            BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "UserRole");
            dt = dt.FilterBBPDataTable("UserID='" + sUser + "'");
            string html = "<table><tr><th>Role Name</th></tr>";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                //string sRoleID = dt.GetColValue(i, "RoleID");
                dynamic oRole = GetObjectWithFilter(IsTestNet(this), "Role", "ID='" + dt.GetColValue(i,"RoleID") + "'");
                string sRow = "<tr><td>" + oRole.Name + "</td></tr>";
                html += sRow;
            }
            html += "</table>";
            lblRoles.Text = html;
           
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
            BiblePayCommonNET.UICommonNET.ToastNow(this, "Sucess", "Successfully added.");

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
            BiblePayCommonNET.UICommonNET.ToastNow(this, "Sucess", "Successfully added.");
        }

    }
}