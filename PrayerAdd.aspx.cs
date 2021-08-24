using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;
using static Unchained.Common;

namespace Unchained
{
    public partial class PrayerAdd : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!gUser(this).LoggedIn)
            {
                MsgBox("Not Logged In", "Sorry, you must be logged in to save a prayer request.", this);
                return;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            
            if (!gUser(this).LoggedIn)
            {
                MsgBox("Not Logged In", "Sorry, you must be logged in to save a prayer request.", this);
                return;
            }
            if (txtSubject.Text.Length < 5 || txtBody.Text.Length < 25)
            {
                MsgBox("Content Too Short", "Sorry, the content of the Body or the Subject must be longer.", this);
                return;
            }
            if (gUser(this).UserName == "")
            {
                MsgBox("Nick Name must be populated", "Sorry, you must have a username to add a prayer.  Please navigate to Account Settings | Edit to set your username.", this);
                return;
            }

            BiblePayCommon.Entity.pray1 o = new BiblePayCommon.Entity.pray1();
            o.Subject = txtSubject.Text;
            o.Body = txtBody.Text;
            o.UserID = gUser(this).BiblePayAddress.ToString();
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(IsTestNet(this), o);
            if (r.Error == "")
            {
                Response.Redirect("PrayerBlog");
            }
            else
            {
                MsgBox("Error while inserting prayer", "Sorry, the prayer was not saved: " + r.Error, this);
            }
        }
    }
}

