using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BiblePayVideo
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:Video1 runat=server></{0}:Video1>")]
    public class Video : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public string Footer
        {
            get
            {
                String s = (String)ViewState["Footer"];
                return ((s == null) ? "[" + this.ID + "]" : s);
            }

            set
            {
                ViewState["Footer"] = value;
            }

           
        }
        public bool Playable
        {
            get
            {
                return Convert.ToBoolean(ViewState["Playable"]);
            }
            set
            {
                ViewState["Playable"] = value;

            }
        }

        public new int Width
        {
            get
            {
                return Convert.ToInt32(ViewState["Width"]);
            }
            set
            {
                ViewState["Width"] = value;
            }
        }

        public new int Height
        {
            get
            {
                return Convert.ToInt32(ViewState["Height"]);
            }
            set
            {
                ViewState["Height"] = value;
            }
        }

        public string URL
        {
            get
            {
                return Convert.ToString(ViewState["URL"]);
            }
            set
            {
                ViewState["URL"] = value;
            }
        }

        public string SVID
        {
            get
            {
                return Convert.ToString(ViewState["SVID"]);
            }
            set
            {
                ViewState["SVID"] = value;
            }
        }
        public string Body
        {
            get
            {
                return Convert.ToString(ViewState["Body"]);

            }
            set
            {
                ViewState["Body"] = value;
            }
        }
        public string Title
        {
            get
            {
                return Convert.ToString(ViewState["Title"]);
            }
            set
            {
                ViewState["Title"] = value;
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write(CurateVideo());
        }

        private string GetInnerVideoL()
        {
          
            string sAutoPlay = Playable ? "autostart autoplay controls playsinline" : "preload='metadata'";
           
            string sHTML = "<video id='video1' class='videosize' "  
                + sAutoPlay + ">";
            string sLoc = !Playable ? "#t=7" : "#t=7";
            sHTML += "<source src='" + URL + sLoc + "' type='video/mp4'></video>";
            return sHTML;
        }

        private string GetInnerVideo()
        {
            string sHTML = "<div class='videosize muse-video-player' data-autoplay='1' data-resume='1' data-video='" + SVID + "' data-height='" + Height.ToString() + "' data-width='" + Width.ToString() + "'></div>";
            return sHTML;
        }
        public Video(Page p)
        {
            this.Page = p;
            /*
            string sJavascript = "function changePlaySpeed(factor)   {"
                + "           var vid = document.getElementById('video1');"
                + "vid.playbackRate = factor;  }  ";

            
            //this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "speed1", sJavascript, true);
            */

        }
        public Video()
        {
            
            
        }


        private string CurateVideo()
        {
           string sDiv = "<div style='height:100%; width:100%; '>";

            sDiv += GetInnerVideo();
            
            if (Title != null && Title != "")
            {
                sDiv += "<br><h2>" + Title + "</h2><br>" + Footer + "<br><hr>" + Body + "<br><hr>";
            }
            sDiv += "</div>";
            return sDiv;
        }

    }
}
