using BiblePayCommon;
using System;
using System.Collections.Generic;
using System.Data;
using System.Dynamic;
using System.Reflection;
using System.Web.UI;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommonNET.StringExtension;
using static Unchained.Common;

namespace Unchained
{
    public partial class Admin : BBPPage
    {

        protected override void Event(BBPEvent e)
        {
            if (e.EventName == "DeleteRole_Click")
            {
                string sID = _bbpevent.EventValue;

                dynamic oUserRole = GetObjectWithFilter(IsTestNet(this), "UserRole", "ID='" + sID + "'");
                dynamic oRole = GetObjectWithFilter(IsTestNet(this), "Role", "ID='" + oUserRole.RoleID + "'");

                dynamic oOrg = GetObjectWithFilter(IsTestNet(this), "Organization", "id='" + oRole.OrganizationID + "'");

                bool fPlays = BiblePayDLL.Sidechain.UserPlaysRole(IsTestNet(this), "Superuser", gUser(this).id, oOrg.BiblePayAddress);
                if (!fPlays)
                {
                    UICommon.MsgBox("Error", "Sorry, access denied. ", this);
                }

                bool fDeleted = BiblePayDLL.Sidechain.DeleteObject(Common.IsTestNet(this), "UserRole", sID, Common.gUser(this));
                if (fDeleted)
                {
                    BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Success!", "Role Removed!");
                    Response.Redirect("Admin");
                }
                else
                {
                    UICommon.MsgBox("Error", "Sorry, the object could not be deleted. ", this);
                }


            }
        }

        public static string RunUserRolesReport(bool fTestNet, string sUserID, bool fIncludeDelete, string sDeleteUserID)
        {
            BBPDataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(fTestNet, "UserRole");
            dt = dt.FilterBBPDataTable("UserID='" + sUserID + "'");

            string html = "<table><tr><th>Organization<th>Role Name</th><th>Action</th></tr>";
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                dynamic oRole = GetObjectWithFilter(fTestNet, "Role", "ID='" + dt.GetColValue(i, "RoleID") + "'");
                dynamic oOrg = GetObjectWithFilter(fTestNet, "Organization", "id='" + oRole.OrganizationID + "'");

                bool fPlays = BiblePayDLL.Sidechain.UserPlaysRole(fTestNet, "Superuser", sDeleteUserID, oOrg.BiblePayAddress);

                string sTrashAnchor = UICommon.GetStandardAnchor("ancDeleteRole" + dt.Rows[i]["id"].ToString(),
                    "DeleteRole", dt.Rows[i]["id"].ToString(), "<i class='fa fa-trash'></i>",
                            "Delete Role", "Role", "", "");
                if (!fPlays || !fIncludeDelete)
                    sTrashAnchor = String.Empty;

                if (oOrg != null && oRole.id != null)
                {
                    string sRow = "<tr><td width=30%>" + oOrg.Name + "<td width=30%>" + oRole.Name + "</td><td>" + sTrashAnchor + "</td></tr>";
                    html += sRow;
                }
            }
            html += "</table>";
            return html;
        }
        protected new void Page_Load(object sender, EventArgs e)
        {
            bool fPlays = BiblePayDLL.Sidechain.UserPlaysRole(IsTestNet(this), "Superuser", gUser(this).id, "");

            if (!fPlays)
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
        }



        protected void btnListUserRoles_Click(object sender, EventArgs e)
        {
            string sUser = Request.Form["ddusers_list"].ToNonNullString();
            string html = RunUserRolesReport(IsTestNet(this),sUser, true, gUser(this).id);
            lblRoles.Text = html;
           
        }

        protected void btnSaveUserRole_Click(object sender, EventArgs e)
        {
            bool fPlays = BiblePayDLL.Sidechain.UserPlaysRole(IsTestNet(this), "Superuser", gUser(this).id, "");

            if (!fPlays)
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



            dynamic oRole = GetObjectWithFilter(IsTestNet(this), "Role", "ID='" + UR.RoleID + "'");
            dynamic oOrg = GetObjectWithFilter(IsTestNet(this), "Organization", "id='" + oRole.OrganizationID + "'");

            bool fPlaysDistinct = BiblePayDLL.Sidechain.UserPlaysRole(IsTestNet(this), "Superuser", gUser(this).id, oOrg.BiblePayAddress);

            if (!fPlaysDistinct)
            {
                UICommon.MsgBox("ERROR", "Sorry, you must be a distinct superuser.", this);
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