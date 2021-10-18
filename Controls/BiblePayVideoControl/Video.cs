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
        public bool AutoPlay
        {
            get
            {
                return Convert.ToBoolean(ViewState["AutoPlay"]);
            }
            set
            {
                ViewState["AutoPlay"] = value;

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
            string sAutoPlay = AutoPlay ? "data-autoplay='1'" : "";
            string sHTML = "<div id='player1' style='width:" 
                + Width.ToString() + "px;height:" + Height.ToString() + "px;' name='player1' class='videoinner videosize muse-video-player' " + sAutoPlay + " data-resume='1' data-video='" 
                + SVID + "' data-height='" + Height.ToString() + "' data-width='" + Width.ToString() + "'></div>";

            if (URL.EndsWith("png") || URL.EndsWith("jpeg") || URL.EndsWith("jpg") || URL.EndsWith("PNG") || URL.EndsWith("JPEG") || URL.EndsWith("JPG") || URL.EndsWith("bmp"))
            {
                sHTML = "<img width='" + (Width-100).ToString() + "px;' height='" + (Height-100).ToString() + "px;' src='" + URL.ToString() + "'/>";
            }

            //js
            string js = "<script>const player = MusePlayer({   container: '#player1',  video: '" + SVID + "',  sizing: 'fill' });</script>";
            sHTML += js;

            return sHTML;
        }
        public Video(Page p)
        {
            this.Page = p;
         
        }
        public Video()
        {
            
        }

        private string CurateVideo()
        {
           string sDiv = "<div id='vidcontainer" + this.ID + "'>";

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
