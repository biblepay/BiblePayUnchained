using System;
using static Unchained.Common;

namespace Unchained
{
    public partial class PrayerAdd : BBPPage
    {
        protected new void Page_Load(object sender, EventArgs e)
        {
            if (!gUser(this).LoggedIn)
            {
                UICommon.MsgBox("Not Logged In", "Sorry, you must be logged in to add a new item.", this);
                return;
            }
            this.Title = _ObjectName + " Add";
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            
            if (!gUser(this).LoggedIn)
            {
                UICommon.MsgBox("Not Logged In", "Sorry, you must be logged in first.", this);
                return;
            }
            if (txtSubject.Text.Length < 4 || txtBody.Text.Length < 10)
            {
                UICommon.MsgBox("Content Too Short", "Sorry, the content of the Body or the Subject must be longer.", this);
                return;
            }
            if (gUser(this).FirstName == "")
            {
                UICommon.MsgBox("Name must be populated", "Please navigate to Account Settings | Edit to set your username.", this);
                return;
            }

            BiblePayCommon.IBBPObject o = (BiblePayCommon.IBBPObject)BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + _EntityName);
            BiblePayCommon.EntityCommon.SetEntityValue(o, "Subject", txtSubject.Text);
            BiblePayCommon.EntityCommon.SetEntityValue(o, "Body", txtBody.Text);
            BiblePayCommon.EntityCommon.SetEntityValue(o, "UserID", gUser(this).id);
            BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, IsTestNet(this), o, gUser(this));
            if (!r.fError())
            {
                Response.Redirect("PrayerBlog?entity=" + _EntityName);
            }
            else
            {
                UICommon.MsgBox("Error while inserting object", "Sorry, the object was not saved: " + r.Error, this);
            }
        }
    }
}

