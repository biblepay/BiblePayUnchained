using BiblePayDLL;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.UI;
using static BiblePayCommon.Common;

namespace Unchained
{


    public static class StringExtension
    {

        public static bool IsNullOrWhitespace(this string s)
        {
            return String.IsNullOrWhiteSpace(s);
        }

        public static bool IsEmpty(this string s)
        {
            if (s == null) return true;
            if (s == "") return true;
            if (s.Trim() == "") return true;
            return false;
        }

        public static int ToInt(this string s)
        {
            double n = GetDouble(s);
            return (int)n;
        }


        public static double ToDouble(this string s)
        {
            double n = GetDouble(s);
            return n;
        }


        public static List<string> SplitCsv(this string csvList, bool nullOrWhitespaceInputReturnsNull = false)
        {
            if (string.IsNullOrWhiteSpace(csvList))
                return nullOrWhitespaceInputReturnsNull ? null : new List<string>();

            return csvList
                .TrimEnd(',')
                .Split(',')
                .AsEnumerable<string>()
                .Select(s => s.Trim())
                .ToList();
        }
        public static string Left(this string o, int oLength)
        {
            if (o.Length < oLength)
            {
                return o;
            }
            return o.Substring(0, oLength);
        }
        public static bool IsNullOrEmpty(this string str)
        {
            if (str == null || str == String.Empty)
                return true;
            return false;
        }

        public static string TrimAndReduce(this string str)
        {
            return str.Trim();
        }

        public static string ToNonNullString(this object o)
        {
            if (o == null)
                return String.Empty;
            return o.ToString();
        }

        

        public static string[] Split(this string str, string sDelimiter)
        {
            string[] vSplitData = str.Split(new string[] { sDelimiter }, StringSplitOptions.None);
            return vSplitData;
        }

        public static double ToDouble(this object o)
        {
            try
            {
                if (o == null)
                    return 0;
                if (o.ToString() == string.Empty)
                    return 0;
                double d = Convert.ToDouble(o.ToString());
                return d;
            }
            catch (Exception)
            {
                return 0;
            }
        }

    }



    public class BBPPage : System.Web.UI.Page
    {
        protected BBPEvent _bbpevent = new BBPEvent();

        public struct BBPEvent
        {
            public string EventName;
            public string EventArgs;
            public string EventRaw;
            public string EventValue;
            public double EventAmount;
            public string EventID;
            public string EventRedirect;
            public string MicrosoftTarget;
            public string MicrosoftArgs;
            public string EventAction;
        }


        public BBPPage()
        {
            this.Load += new EventHandler(this.Page_Load);
        }

        protected void ProcessComments()
        {
            string sSave = Request.Form["btnSaveComment"].ToNonNullString();
            string id = Request.QueryString["id"] ?? "";
            if (sSave != String.Empty)
            {
                if (Common.gUser(this).UserName == String.Empty)
                {
                    Common.MsgBox("Nick Name must be populated", "Sorry, you must have a username to add a comment.  Please navigate to Account Settings | Edit to set your UserName.", this);
                    return;
                }

                BiblePayCommon.Entity.comment1 o = new BiblePayCommon.Entity.comment1();
                o.UserID = Common.gUser(this).BiblePayAddress;
                o.Body = Request.Form["txtComment"].ToString();
                o.ParentID = id;

                BiblePayCommon.Common.DACResult r = DataOps.InsertIntoTable(Common.IsTestNet(this), o);
                if (r.Error == "")
                {
                    Session["stack"] = UICommon.Toast("Saved", "Your comment has been Saved!");
                    Response.Redirect(this.Request.RawUrl);
                }
                else
                {
                    Common.MsgBox("Error while inserting comment", "Sorry, the comment was not saved: " + r.Error, this);
                }

            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            GetEventTarget();
            if (_bbpevent.EventAction != String.Empty)
            {
                if (_bbpevent.EventRedirect.ToNonNullString() != String.Empty)
                {
                    if (Session["server.transfer"].ToDouble() != 1)
                    {
                        Session["server.transfer"] = 1;
                        Server.Transfer(_bbpevent.EventRedirect + ".aspx", true);
                        return;
                    }
                }
                Session["server.transfer"] = 0;
                Event(_bbpevent);
            }

            // Comments
            ProcessComments();


            // Check for anything on the stack
            string sStack = Session["stack"].ToNonNullString();
            if (sStack != "")
            {
                Session["stack"] = null;
                // Todo - stack guid...
                UICommon.RunScript(this, sStack);
            }
            Common.Log("Accessing " + this.Request.RawUrl + " " + Common.gUser(this).UserName.ToString());


        }

        protected virtual void Event(BBPEvent e)
        {

        }
        public void GetEventTarget()
        {
            _bbpevent = new BBPEvent();
            _bbpevent.EventName = String.Empty;
            _bbpevent.EventID = String.Empty;
            _bbpevent.EventAction = String.Empty;
            _bbpevent.MicrosoftArgs = Request["__EVENTARGUMENT"] ?? "";
            _bbpevent.MicrosoftTarget = Request["__EVENTTARGET"] ?? "";
            
            _bbpevent.EventAction = _bbpevent.MicrosoftArgs;


            if (_bbpevent.EventAction.ToNonNullString() == String.Empty)
                return;

            string[] vEvents = _bbpevent.MicrosoftTarget.Split("_");
            if (vEvents.Length > 0)
            {
                string sEVType = vEvents[0];
                if (sEVType.ToLower() == "event")
                {
                    _bbpevent.EventName = vEvents[0];
                    if (vEvents.Length > 1)
                         _bbpevent.EventID = vEvents[1];
                    if (vEvents.Length > 2)
                        _bbpevent.EventRedirect = vEvents[2];
                    if (vEvents.Length > 3)
                        _bbpevent.EventValue = vEvents[3];

                    if (vEvents.Length > 4)
                        _bbpevent.EventAmount = BiblePayCommon.Common.GetDouble(vEvents[4]);

                }
            }
          
        }

    }
}
