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
using System.Web;
using static BiblePayCommon.DataTableExtensions;
using BiblePayCommonNET;

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

        private void FLO()
        {

            if (Common.gUser(this).LoggedIn == false)
            {
                UICommon.MsgBox("Error", "Sorry you must be logged on first.", this);
            }

        }
        protected void ProcessComments()
        {

            // Logged off or in:

            if (_bbpevent.EventName == "LogIn_Click")
            {
                // top banner
                Response.Redirect("LogIn.aspx");
            }
            else if (_bbpevent.EventName == "SignUp_Click")
            {
                // top banner
                Response.Redirect("RegisterMe.aspx");
            }
            else if (_bbpevent.EventName == "WatchVideos_Click")
            {
                Response.Redirect("VideoList");
            }
            else if (_bbpevent.EventName == "PeopleModule_Click")
            {
                Response.Redirect("Person?homogenized=1");
            }

            // Logged in:

            if (_bbpevent.EventName == "DeleteObject_Click")
            {
                FLO();
                string sID = _bbpevent.EventValue;
                bool fDeleted = BiblePayDLL.Sidechain.DeleteObject(Common.IsTestNet(this), _bbpevent.Extra.Table.ToString(), sID, Common.gUser(this));
                if (fDeleted)
                {
                    BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Success!", "Your object was deleted!");
                }
                else
                {
                    UICommon.MsgBox("Error", "Sorry, the object could not be deleted. ", this);
                }
            }
            else if (_bbpevent.EventName == "DeleteNotification_Click")
            {
                FLO();
                string sID = _bbpevent.EventValue;
                bool fDeleted = BiblePayDLL.Sidechain.DeleteObject(Common.IsTestNet(this), _bbpevent.Extra.Table.ToString(), sID, Common.gUser(this));
                /*
                if (fDeleted)
                {
                    BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Success!", "Your object was deleted!");
                }
                else
                {
                    UICommon.MsgBox("Error", "Sorry, the object could not be deleted. ", this);
                }
                */

                double nlu = 0;
                UICommon.ShowNotificationConsole(this,true,ref nlu);
                BiblePayCommonNET.CommonNET.SetSessionDouble(this, "unread_notifications", nlu);


            }
            else if (_bbpevent.EventName == "GetNotifications_Click")
            {
                FLO();
                double nlu = 0;
                UICommon.ShowNotificationConsole(this, false, ref nlu);
                BiblePayCommonNET.CommonNET.SetSessionDouble(this, "unread_notifications", nlu);
                
            }
            else if (_bbpevent.EventName == "CloseChat_Click")
            {
                FLO();

                UICommon.ChatStructure c = new UICommon.ChatStructure();
                bool fGot = UICommon.dictChats.TryGetValue(Common.gUser(this).id, out c);
                if (fGot)
                {
                    UICommon.dictChats.Remove(c.chattingWithUser);
                    UICommon.dictChats.Remove(c.startedByUser);
                    UICommon.dictChatHistory.Remove(c.chatGuid);
                }
                else
                {
                    BiblePayCommonNET.UICommonNET.MsgModal(this, "Failure", "The chat no longer exists.", 400, 300, true);
                }
            }
            else if (_bbpevent.EventName == "BanUser_Click")
            {
                FLO();

                User uBanned = Common.gUserById(this, _bbpevent.EventValue);
                uBanned.Banned = 1;
                BiblePayCommon.Entity.user1 o = Common.ConvertUserToUserEntity(uBanned);
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), o, Common.gUser(this));
                string sNarr = r.fError() ? "Failed" : "Successfully banned.";
                BiblePayCommonNET.UICommonNET.MsgModal(this, "Result", sNarr, 400, 300, true);
            }
            else if(_bbpevent.EventName == "ChatNow_Click")
            {
                FLO();

                Session["chatactiveuser"] = _bbpevent.EventValue;
                UICommon.ChatStructure c = new UICommon.ChatStructure();
                c.chattingWithUser = _bbpevent.EventValue;
                c.startedByUser = Common.gUser(this).id;
                c.chatGuid = Guid.NewGuid().ToString();
                UICommon.dictChats[Common.gUser(this).id] = c;
                UICommon.dictChats[_bbpevent.EventValue] = c;
                User uOther = Common.gUserById(this, c.chattingWithUser);
                string sURL = "Meeting?id=" + c.chatGuid.ToString() + "&type=private";
                string sClose = "onclick='closeModalDialog();'";
                string sNarr = "Click <a " + sClose + " href='" + sURL + "' target='_blank'>here to page " + uOther.FullUserName() + " and enter the chat room.  ";
                UICommonNET.MsgModalWithLinks(this, "Enter Chat Room", sNarr, 320, 200, true, true);
            }
            else if (_bbpevent.EventName=="RearrangeObjectUp_Click")
            {
                FLO();

                if (!Common.gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "You Must be logged in first.", this);
                    return;
                }
                BiblePayCommon.Entity.video1 v = (BiblePayCommon.Entity.video1)Common.GetObject(Common.IsTestNet(this), "video1", _bbpevent.EventValue);
                if (v == null)
                {
                    UICommon.MsgBox("Error", "Sorry, cannot locate object.", this.Page);
                    return;
                }
                double nCurrentOrdinal = 0; 
                v.Order = nCurrentOrdinal - 1.9;
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), v, Common.gUser(this));

                if (!r.fError())
                {
                    //BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Saved", "Your item has been reordered!");
                }
                else
                {
                    UICommon.MsgBox("Error while reordering item", "Sorry, the item could not be reordered: " + r.Error, this);
                }
            }
            else if (_bbpevent.EventName == "RearrangeObjectDown_Click")
            {
                FLO();

                if (!Common.gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "You Must be logged in first.", this);
                    return;
                }
                BiblePayCommon.Entity.video1 v = (BiblePayCommon.Entity.video1)Common.GetObject(Common.IsTestNet(this), "video1", _bbpevent.EventValue);
                if (v == null)
                {
                    UICommon.MsgBox("Error", "Sorry, cannot locate object.", this.Page);
                    return;
                }
                double nCurrentOrdinal = 0; // inal(this, "video1", "attachment=1 and parentid='" + v.ParentID + "'", v.id, _bbpevent.Extra.Snippet.ToString());
                v.Order = nCurrentOrdinal + 1.6;
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), v, Common.gUser(this));
                if (!r.fError())
                {
//                    BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Saved", "Your item has been reordered!");
                }
                else
                {
                    UICommon.MsgBox("Error while reordering", "Sorry, the item could not be reordered: " + r.Error, this);
                }
            }
            else if (_bbpevent.EventName == "EditObject_Click")
            {
                FLO();

                BiblePayCommon.Entity.comment1 c = (BiblePayCommon.Entity.comment1)Common.GetObject(Common.IsTestNet(this), "comment1", _bbpevent.EventValue);
                if (c == null)
                {
                    UICommon.MsgBox("Error", "Sorry, cannot locate object.", this.Page);
                    return;
                }

                UICommon.MsgInputRichText(this, "EditedComment_Click", "Edit Comment", "Edit your comment:"
                         , 1000, "", "", UICommon.InputType.multiline, false, _bbpevent.EventValue, c.Body);

            }
            else if (_bbpevent.EventName == "EditedComment_Click")
            {
                FLO();

                if (!Common.gUser(this).LoggedIn)
                {
                    UICommon.MsgBox("Error", "You Must be logged in first.", this);
                    return;
                }


                BiblePayCommon.Entity.comment1 c = (BiblePayCommon.Entity.comment1)Common.GetObject(Common.IsTestNet(this), "comment1", _bbpevent.EventValue);

                if (c == null)
                {
                    UICommon.MsgBox("Error", "Sorry, cannot locate object.", this.Page);
                    return;
                }

                c.Body = HttpUtility.UrlDecode(BiblePayCommon.Encryption.Base64DecodeStandard(_bbpevent.Extra.Output.ToString()));
                
                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), c, Common.gUser(this));
                if (!r.fError())
                {
                    BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Saved", "Your comment has been Edited!");
                }
                else
                {
                    UICommon.MsgBox("Error while editing", "Sorry, the comment was not edited: " + r.Error, this);
                }
            }
            else if (_bbpevent.EventName=="UploadVideo_Click")
            {
                FLO();

                Response.Redirect("UnchainedUpload");
            }
            else if (_bbpevent.EventName == "LogOut_Click")
            {
                this.Page.Session[Common.GetChain0(Common.IsTestNet(this)) + "user"] = null;
                BMS.StoreCookie("pwhash", "");
                this.Page.Session["stack"] = UICommon.Toast("Logging Off", "You are now logged off.");
                Response.Redirect("Login");
            }
            else if (_bbpevent.EventName == "SaveComments_Click")
            {
                FLO();

                string id = Request.QueryString["id"] ?? "";
                
                if (id == "")
                {
                    UICommon.MsgBox("Error", "Unable to locate the object.", this.Page);
                    return;
                }

                BiblePayCommon.Entity.comment1 o = new BiblePayCommon.Entity.comment1();
                o.UserID = Common.gUser(this).id;
                string sURL = HttpContext.Current.Request.Url.AbsoluteUri;
                string sParentType = Request.QueryString["Entity"] ?? "";

                if (sURL.Contains("Media"))
                {
                    sParentType = "video1";
                }
                string sOrig = Request.Form["txtComment"].ToNonNullString();
                string sDecoded = sOrig.Replace(@"&lt;", @"<").Replace(@"&gt;", @">");
                o.Body = sDecoded;
                o.ParentID = id;
                dynamic o1 = Common.GetObject(Common.IsTestNet(this), sParentType, o.ParentID);
                if (o1 == null || sOrig==null || sOrig=="")
                {
                    UICommon.MsgBox("Error", "Unable to locate the parent object.", this.Page);
                    return;

                }
                string sOriginalPosterID = o1.UserID;
                string sBlurb = "";
                if (sParentType == "video1")
                {
                    sBlurb = o1.Title;
                }
                else
                {
                    sBlurb = o1.Subject;
                }


                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(this, Common.IsTestNet(this), o, Common.gUser(this));
                if (!r.fError())
                {
                    // Add a notification also...
                    UICommon.SendNotification(sParentType, sBlurb, this, "commented on", sURL, sOriginalPosterID);
                    BiblePayCommonNET.UICommonNET.ToastNow(this.Page, "Saved", "Your comment has been Saved!");
                }
                else
                {
                    UICommon.MsgBox("Error while saving", "Sorry, the comment was not saved: " + r.Error, this);
                }
            }
        }

        public void GetEventTarget()
        {
            _bbpevent = new BBPEvent();
            bool fPostback = IsPostBack;
            string sViewState = Request["__VIEWSTATE"] ?? "";

            string hfPostback = (Request.Params["hfPostback"] ?? "").ToString();
            if (hfPostback != "")
            {
                if (hfPostback.Contains("},{"))
                {
                    //mangled
                    return;
                }
                string sPreJson = BiblePayCommon.Encryption.Base64Decode0(hfPostback);
                //sPreJson = System.Text.RegularExpressions.Regex.Unescape(sPreJson);

                //string sPreJson = hfPostback;


                dynamic oEventInfo1 = JsonConvert.DeserializeObject<dynamic>(sPreJson);

                _bbpevent.Iteration = oEventInfo1["Iteration"];
                double nLastPBIteration = GetDouble(this.Page.Session["LastPostbackIteration2"]);
                // double nLPI = GetDouble(BiblePayCommon.HalfordCache.Read("lpi_" + _bbpevent.Iteration.ToString()));

                if (nLastPBIteration == _bbpevent.Iteration)
                {
                    return;
                }
                _bbpevent.EventName = (oEventInfo1["Event"] ?? "").ToString();
                _bbpevent.TargetControl = (oEventInfo1["TargetControl"] ?? "").ToString();
                _bbpevent.EventAction = _bbpevent.EventName;
                _bbpevent.EventValue = (oEventInfo1["Value"] ?? "").ToString();
                _bbpevent.SourceControl = (oEventInfo1["SourceControl"] ?? "").ToString();
                _bbpevent.Extra = oEventInfo1;//["ExtraObjectValues"];
                //BiblePayCommon.HalfordCache.Write("lpi_" + _bbpevent.Iteration.ToString(), 1, 60 * 10);

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
            // Penetration testing and ddos prevention
            // If they attack our site, we detect this here and conserve our resources.
            try
            {
                string path = this.Request.Url.GetLeftPart(UriPartial.Path); // This is the part without the params
                double nLastWrite = GetDouble(Session["lastwrite_" + path].ToNonNullString());
                double nQty = GetDouble(Session["qty_" + path].ToNonNullString()) + 1;
                double nElapsed = BiblePayCommon.Common.UnixTimestampUTC() - nLastWrite;
                if (nElapsed > 60)
                {
                    nQty = 0;
                    nElapsed = 600;
                }


                double nTotalTime = GetDouble(Session["totaltime_" + path].ToNonNullString()) + nElapsed;
                Session["totaltime_" + path] = nTotalTime.ToString();
                Session["lastwrite_" + path] = UnixTimestampUTC().ToString();
                Session["qty_" + path] = (nQty).ToString();

                double nBehavior = nTotalTime / nQty;
                if (path.Contains("MessagePage"))
                {
                    nBehavior = 100;
                }
                if (nBehavior < 20)
                {
                    // ddos
                    if (nBehavior < 5 && nBehavior > 1)
                    {
                        // Warning
                        Log2("WARNING!   A DDOS WARNING HAS BEEN ISSUED.");

                        UICommon.MsgBox("Warning", "We have detected a d-dos coming from your network.  Please slow access down, otherwise you may be banned.", this);
                        return;
                    }
                    else if (nBehavior < 1)
                    {
                        Log2("ATTACK!  A ddos attack has been detected, level II.");

                        // block the user
                        Response.Write("ATTACK");
                        Response.End();
                        return;


                    }


                }


            }
            catch(Exception ex)
            {
                Log2("anti-ddos::" + ex.Message);
            }
        }

        protected string _CollectionName = "";
        protected string _ObjectName = "";
        protected string _EntityName = "";
        private void SetEntityInformation(string sEntity)
        {
            try
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
            }catch(Exception ex)
            {
                Log2("SetEntityInformation::" + sEntity + " :: " + ex.Message);
            }
        }
        protected void Page_LoadComplete(object sender, EventArgs e)
        {
            if (IsPostBack)
            {
                string sURL = this.Request.RawUrl;
                bool fOK = false;
                if (sURL.Contains("VideoList"))
                    fOK = true;

                /*
                if (fOK)
                    Response.Redirect(sURL);
                    *
                    */

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
                    // Erase the cookie
                    BMS.StoreCookie("pwhash", "");
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
