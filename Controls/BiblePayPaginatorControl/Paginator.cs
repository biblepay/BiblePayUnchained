using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace BiblePayPaginator
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:Paginator1 runat=server></{0}:Paginator1>")]
    public class Paginator : WebControl
    {
        [Bindable(true)]
        [Category("Appearance")]
        [DefaultValue("")]
        [Localizable(true)]
        public int ColumnsPerRow
        {
            get
            {
                return Convert.ToInt32(ViewState["ColumnsPerRow"]);
            }
            set
            {
                ViewState["ColumnsPerRow"] = value;
            }
        }

        public int RowsPerPage
        {
            get
            {
                return Convert.ToInt32(ViewState["RowsPerPage"]);
            }
            set
            {
                ViewState["RowsPerPage"] = value;
                GeneratePaginatorControl();

            }
        }

        public int PageNumber
        {
            get
            {
                return Convert.ToInt32(ViewState["PageNumber"]);
            }
            set
            {
                ViewState["PageNumber"] = value;
            }
        }

        public int StartRow
        {
            get
            {
                return Convert.ToInt32(ViewState["StartRow"]);
            }
            set
            {
                ViewState["StartRow"] = value;
            }
        }

        public int EndRow
        {
            get
            {
                return Convert.ToInt32(ViewState["EndRow"]);
            }
            set
            {
                ViewState["EndRow"] = value;
            }
        }

        public int Rows
        {
            get
            {
                return Convert.ToInt32(ViewState["Rows"]);
            }
            set
            {
                ViewState["Rows"] = value;
            }
        }

        public int TotalPages
        {
            get
            {
                return Convert.ToInt32(ViewState["TotalPages"]);
            }
            set
            {
                ViewState["TotalPages"] = value;
            }
        }

        protected override void RenderContents(HtmlTextWriter output)
        {
            output.Write(GeneratePaginatorControl());
        }


        public Paginator(Page p)
        {
            this.Page = p;
            //this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "speed1", sJavascript, true);
        }
        public Paginator()
        {
            this.Load += new EventHandler(this.Page_Load);


        }

        protected void Page_Load(object sender, EventArgs e)
        {

            string ea = this.Page.Request["__EVENTARGUMENT"] ?? "";
            string et = this.Page.Request["__EVENTTARGET"] ?? "";
            string[] vArgs = et.Split(new string[] { "<col>" }, StringSplitOptions.None);

            if (et == "btnPaginate_Click")
            {
                PageNumber = (int)BiblePayCommon.Common.GetDouble(ea);
                if (PageNumber < 1)
                    PageNumber = 1;
            }

        }



        private static string GenPagI1(string sValue, string sCustomHTML, string sClass)
        {
            string a = "<a " + sClass + " href='javascript: void(0);' onclick=\"__doPostBack('btnPaginate_Click', " + sValue + ");\">" + sCustomHTML + "</a>";
            return a;
        }
        private string GeneratePaginatorControl()
        {
            int nObjsPerPage = ColumnsPerRow * RowsPerPage;

            StartRow = (PageNumber-1) * nObjsPerPage;
            if (StartRow < 0)
                StartRow = 0;
            EndRow = StartRow + nObjsPerPage - 1;
            if (EndRow >= Rows)
                EndRow = Rows - 1;
            if (nObjsPerPage < 1)
                nObjsPerPage = 1;

            
            TotalPages = (int)Math.Ceiling((double)(Rows / nObjsPerPage));
            
            if (PageNumber > TotalPages)
            {
                PageNumber = TotalPages;
            }
            
            if (PageNumber < 1)
                PageNumber = 1;
            int iPriorPage = PageNumber - 1;
            if (iPriorPage < 1)
                iPriorPage = 1;

            int iNextPage = PageNumber + 1;
            if (iNextPage >= TotalPages)
                iNextPage = (int)TotalPages;

            string pag = "<div class='pagination' style='margin:0 0;'><table width=100%><tr><td width=100%>";

            pag += GenPagI1("0", "&laquo;", "");

            pag += GenPagI1((iPriorPage).ToString(), "&larr;", "");

            int iPos = 0;
            int iStartPage = PageNumber - (1 / 2);
            if (iStartPage < 1) 
                iStartPage = 1;

            for (int i = iStartPage; i <= TotalPages; i++)
            {
                string sActive = "";
                if ((i - 0) == PageNumber)
                    sActive = "class='active'";
                string sRow = GenPagI1(i.ToString(), i.ToString(), sActive);
                if (iPos > 2 && i < TotalPages)
                {
                    sRow = GenPagI1(TotalPages.ToString(), TotalPages.ToString(), sActive);
                }
                pag += sRow;
                iPos++;
                if (iPos > 3)
                    break;
            }
            //Next Page
            pag += GenPagI1((iNextPage).ToString(), "&rarr;", "");
            //Last Page
            pag += GenPagI1((TotalPages).ToString(), "&raquo;", "");
            pag += "</td></tr></table></div>";
            return pag;
        }
    }
}
