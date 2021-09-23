using BiblePayDLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using static BiblePayCommonNET.StringExtension;
using Newtonsoft.Json;
using static BiblePayCommonNET.CommonNET;
using static BiblePayCommon.Common;

namespace Unchained
{

    public class BBPPage : System.Web.UI.Page
    {
        protected BBPEvent _bbpevent = new BBPEvent();

        public BBPPage()
        {
            this.Load += new EventHandler(this.Page_Load);
            this.LoadComplete += new EventHandler(this.Page_LoadComplete);
        }

        protected void ProcessComments()
        {
            if (_bbpevent.EventName == "DeleteObject_Click")
            {
                string sID = _bbpevent.EventValue;
                bool fDeleted = BiblePayDLL.Sidechain.DeleteObject(Common.IsTestNet(this), _bbpevent.Extra.Table.ToString(),
                          sID,Common.gUser(this));
                if (fDeleted)
                {
                     BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Success!", "Your object was deleted!");
                }
                else
                {
                     UICommon.MsgBox("Error", "Sorry, the object could not be deleted. ", this);
                }
            }
            else if (_bbpevent.EventName == "SaveComments_Click")
            {
                string id = Request.QueryString["id"] ?? "";
                
                if (!Common.gUser(this).LoggedIn)
                {
                      UICommon.MsgBox("Error", "Sorry, you must be logged in.", this);
                      return;
                }

                if (id == "")
                {
                    UICommon.MsgBox("Error", "Unable to locate the object.", this.Page);
                    return;
                }

                BiblePayCommon.Entity.comment1 o = new BiblePayCommon.Entity.comment1();
                o.UserID = Common.gUser(this).id;
                o.Body = Request.Form["txtComment"].ToString();
                o.ParentID = id;

                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), o, Common.gUser(this));
                if (!r.fError())
                {
                      BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Saved", "Your comment has been Saved!");
                }
                else
                {
                      UICommon.MsgBox("Error while inserting comment", "Sorry, the comment was not saved: " + r.Error, this);
                }
            }
        }

        public void GetEventTarget()
        {
            _bbpevent = new BBPEvent();
            
            string hfPostback = (Request.Params["hfPostback"] ?? "").ToString();
            if (hfPostback != "")
            {
                if (hfPostback.Contains("},{"))
                {
                    //mangled
                    return;
                }
                dynamic oEventInfo1 = JsonConvert.DeserializeObject<dynamic>(BiblePayCommon.Encryption.Base64Decode(hfPostback));

                _bbpevent.Iteration = oEventInfo1["Iteration"];
                if (GetDouble(this.Page.Session["LastPostbackIteration2"]) == _bbpevent.Iteration)
                {
                    return;
                }
                _bbpevent.EventName = (oEventInfo1["Event"] ?? "").ToString();
                _bbpevent.TargetControl = (oEventInfo1["TargetControl"] ?? "").ToString();
                _bbpevent.EventAction = _bbpevent.EventName;
                _bbpevent.EventValue = (oEventInfo1["Value"] ?? "").ToString();
                _bbpevent.SourceControl = (oEventInfo1["SourceControl"] ?? "").ToString();
                _bbpevent.Extra = oEventInfo1;//["ExtraObjectValues"];
                this.Page.Session["LastPostbackIteration2"] = _bbpevent.Iteration;
                return;
            }
        }

        protected void ProcessToast()
        {
            // Check for TOAST:
            string sStack = Session["stack"].ToNonNullString();
            if (sStack != "")
            {
                Session["stack"] = null;
                // Todo - stack guid...
                UICommon.RunScript(this, sStack);
            }
            Common.Log("Accessing " + this.Request.RawUrl + " " + Common.gUser(this).FullUserName());
        }

        protected string _CollectionName = "";
        protected string _ObjectName = "";
        protected string _EntityName = "";
        private void SetEntityInformation(string sEntity)
        {
            if ((this.Page.Title == "Prayer Blog" || this.Page.Title == "Prayer View" || this.Page.Title == "Prayer Add") && sEntity == "")
                _EntityName = "pray1";
            else
            {
                _EntityName = sEntity;
            }

            if (_EntityName == "")
                return;
            
            BiblePayCommon.IBBPObject o = (BiblePayCommon.IBBPObject)BiblePayCommon.EntityCommon.GetInstance("BiblePayCommon.Entity+" + _EntityName);
            Dictionary<string, string> _oDict = BiblePayCommon.EntityCommon.GetStaticFieldValues(o);
            _CollectionName = _oDict["CollectionName"].ToString();
            _ObjectName = _oDict["ObjectName"].ToString();
        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string sURL = this.Request.RawUrl;
                bool fOK = false;
                if (sURL.Contains("VideoList"))
                    fOK = true;

                if (fOK)
                    Response.Redirect(sURL);
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            // Entity
            string sEntity = (Request.QueryString["entity"] ?? "").ToString();
            SetEntityInformation(sEntity);
            GetEventTarget();
            if (_bbpevent.EventAction != String.Empty)
            {
                if (_bbpevent.EventAction=="btnChangeChain_Click")
                {
                    double nNewValue = _bbpevent.EventValue == "MAINNET" ? 2 : 0;
                    Session["mainnet_mode"] = nNewValue;
                    Session["balance"] = null;
                    Response.Redirect("VideoList");
                }
                Event(_bbpevent);
            }
            // Comments
            ProcessComments();
            ProcessToast();
        }

        protected virtual void Event(BBPEvent e)
        {

        }
    }
}
