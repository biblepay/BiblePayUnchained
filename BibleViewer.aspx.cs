using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.WebControls;
using static Unchained.Common;
using static BiblePayCommonNET.StringExtension;


namespace Unchained
{

    public partial class BibleViewer : BBPPage
    {
        private BiblePayDLL.Bible _bible = new BiblePayDLL.Bible();

        protected void LoadChapters(int iSelectedIndex)
        {
            ddChapter.Items.Clear();

            for (int iChapter = 1; iChapter < 99; iChapter++)
            {
                string sVerse = _bible.GetVerse(ddBook.SelectedValue, iChapter, 1);
                if (!sVerse.IsEmpty())
                {
                    ddChapter.Items.Add(new ListItem("Chapter " + iChapter.ToString(), iChapter.ToString()));
                }
                else
                {
                    break;
                }
            }
            if (ddChapter.Items.Count >= iSelectedIndex)
            {
                ddChapter.SelectedIndex = iSelectedIndex;
            }
        }


        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                List<string> l = _bible.GetBookList();
                ddBook.Items.Clear();
                ddBook.Items.Add("");
                for (int i = 0; i < l.Count; i++)
                {
                    string sBookShort = _bible.GetBookByName(l[i]);
                    ddBook.Items.Add(new ListItem(l[i], sBookShort));

                }
                // Chapter
                string sBook = ddBook.SelectedValue;

                if (ddBook.SelectedValue == "")
                {
                    ddBook.SelectedValue = "Gen";
                    ddBook.SelectedIndex = 1;

                }
                LoadChapters(0);
            }
            else
            {
                LoadChapters(ddChapter.SelectedIndex);
            }

        }
        protected string GetContent()
        {
            string sHTML = "";
            for (int i = 1; i < 999; i++)
            {
                string sBookName = _bible.GetBookByName(ddBook.SelectedValue);
                string sVerse = _bible.GetVerse(ddBook.SelectedValue, ddChapter.SelectedValue.ToInt(), i);
                if (sVerse.IsEmpty())
                    break;
                sHTML += i.ToString() + ". " + sVerse + "<br>\r\n";
            }
            return sHTML;
        }

    }
}