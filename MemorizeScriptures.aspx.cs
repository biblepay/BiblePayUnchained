using static Unchained.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using static BiblePayDLL.Shared;
using static Unchained.DataOps;
using static BiblePayCommon.Common;
using static BiblePayCommon.DataTableExtensions;
using static BiblePayCommonNET.UICommonNET;
using BiblePayCommonNET;

namespace Unchained
{

	public enum TestingMode
	{
		TESTING = 0,
		TRAIN = 1
	}

	public struct TestingMemory
	{
		public TestingMode TestMode;
		public int nCorrectVerse;
		public int nCorrectChapter;
		public string sCorrectBook;
		public int nTrainingQuestionsTaken;
		public int nTestingQuestionsTaken;
		public double nTrainingScore;
		public double nTestingScore;
	};
	public partial class MemorizeScriptures : BBPPage
    {

		protected TestingMemory _testingmemory = new Unchained.TestingMemory();
		private BiblePayDLL.Bible _bible = new BiblePayDLL.Bible();
		protected void Page_Load(object sender, EventArgs e)
        {
			if (Session["testingmemory"] != null)
			{
				_testingmemory = (TestingMemory)Session["testingmemory"];

			}
			else
			{
				_testingmemory.TestMode = TestingMode.TRAIN;
				Session["testingmemory"] = _testingmemory;
			}
			List<string> l = _bible.GetBookList();
			ddBook.Items.Clear();

			ddBook.Items.Add("");
			for (int i = 0; i < l.Count; i++)
			{
				ddBook.Items.Add(new ListItem(l[i], l[i].ToUpper()));
			}
			UpdateDisplay();
	    }

		void PopulateNewVerse()
		{
			DataTable dt = BiblePayDLL.Sidechain.RetrieveDataTable3(IsTestNet(this), "versememorizer1");
			if (dt.Rows.Count < 1)
				return;
			Random r = new Random();
			int iChapter = 0;
			string sBook = "";
			int iVerseStart = 0, iVerseEnd = 0;
			string sTotalVerses = "";
			for (int y = 0; y < 9; y++)
			{
				int iRow = (int)r.Next(0, dt.Rows.Count);
				iVerseStart = (int)dt.GetColDouble(iRow, "VerseFrom");
				iVerseEnd = (int)dt.GetColDouble(iRow, "VerseTo");
				if (iVerseEnd < iVerseStart)
					iVerseEnd = iVerseStart;
				sBook = dt.GetColValue(iRow, "BookFrom");
				iChapter = (int)dt.GetColDouble(iRow, "ChapterFrom");
				sTotalVerses = "";
				for (int j = iVerseStart; j <= iVerseEnd && j >= iVerseStart; j++)
				{
					string sPrefix = _testingmemory.TestMode == TestingMode.TESTING ? "" : j.ToString() + ".  ";
					string sVerse = sPrefix + _bible.GetVerse(sBook, iChapter, j);
					sTotalVerses += sVerse + "\r\n";
				}
				if (iVerseStart > 1 && iVerseEnd > 1 && sTotalVerses.Length > 7)
					break;
			}

			if (iVerseStart == 0)
				return;
		

			clear();

			txtChapter.Text = iChapter.ToString();
			txtVerse.Text = iVerseStart.ToString();
			string sLocalBook = _bible.GetBookByName(sBook);
			if (sLocalBook == "")
				sLocalBook = sBook;

			_testingmemory.sCorrectBook = sLocalBook;
			_testingmemory.nCorrectVerse = iVerseStart;
			_testingmemory.nCorrectChapter = iChapter;
			string sTest = "";
			for (int i = 0; i < ddBook.Items.Count; i++)
			{
				sTest += ddBook.Items[i].Value + " ";
			}
			if (sLocalBook.ToUpper() == "ROM")
			{
				sLocalBook = "1 PAUL TO THE ROMANS";
			}
			ddBook.SelectedValue = sLocalBook;
			txtScripture.Text = sTotalVerses;
			// Set read only
			txtChapter.ReadOnly = _testingmemory.TestMode == TestingMode.TRAIN;
			ddBook.Enabled = _testingmemory.TestMode == TestingMode.TESTING;
			
			if (_testingmemory.TestMode == TestingMode.TESTING)
			{
				// In test mode we need to clear the fields
				txtChapter.Text = "";
				txtVerse.Text = "";
				ddBook.SelectedValue = "";
				ddBook.Focus();
			}
			else
			{
				txtPractice.Focus();
			}

			string sCaption = _testingmemory.TestMode == TestingMode.TRAIN ? "Switch to TEST Mode" : "Switch to TRAIN Mode";
			btnSwitchToTest.Text = sCaption;

			Session["testingmemory"] = _testingmemory;
		}


		double WordComparer(string Verse, string UserEntry)
		{
			Verse = Verse.ToUpper();
			UserEntry = UserEntry.ToUpper();
			string[] vVerse = Verse.Split(" ");
			string[] vUserEntry = UserEntry.Split(" ");
			double dTotal = vVerse.Length;
			double dCorrect = 0;
			for (int i = 0; i < vVerse.Length; i++)
			{
				bool f = UserEntry.Contains(vVerse[i]);
				if (f)
					dCorrect++;
			}
			double dPct = dCorrect / (dTotal + .01);
			return dPct;
		}

		void ShowResults()
		{
			string sTitle = "Results";
			double nTrainingPct =_testingmemory.nTrainingScore / (_testingmemory.nTrainingQuestionsTaken + .01);
			double nTestingPct = _testingmemory.nTestingScore / (_testingmemory.nTestingQuestionsTaken + .01);

			string sSummary = "Congratulations!";
			if (_testingmemory.nTrainingQuestionsTaken > 0)
			{
				sSummary += "<br>In training mode you worked through " 
					+ _testingmemory.nTrainingQuestionsTaken.ToString() 
					+ ", and your score is " + Math.Round(nTrainingPct * 100, 2).ToString()
					+ "%!  ";
			}

			if (_testingmemory.nTestingQuestionsTaken > 0)
			{
				sSummary += "<br>In testing mode you worked through " 
					+ _testingmemory.nTestingQuestionsTaken.ToString()
					+ ", and your score is " + Math.Round(nTestingPct * 100, 2).ToString()
					+ "%! ";
			}
			sSummary += "<br>Please come back and see us again. ";
			// Clear the results so they can start again if they want:
			_testingmemory.nTrainingQuestionsTaken = 0;
			_testingmemory.nTestingQuestionsTaken = 0;
			_testingmemory.nTrainingScore = 0;
			_testingmemory.nTestingScore = 0;

			Session["testingmemory"] = _testingmemory;
			MsgModal(this, sTitle, sSummary, 500, 300, true, false);
		}

		void UpdateDisplay()
		{
			string sMode = _testingmemory.TestMode==TestingMode.TRAIN ? "<font color=red>TRAINING MODE</font>" : "<font color=red>TESTING MODE</font>";
			string sInfo = sMode + "<br><br>Welcome to the Scripture Memorizer, " + gUser(this).FullUserName() + "!";
			lblInfo.Text = sInfo;
			// Find the first verse to do the initial population.
			PopulateNewVerse();
		}

		void clear()
		{
			txtChapter.Text = "";
			txtVerse.Text = "";
			ddBook.SelectedValue = "";
			ddBook.Focus();

			txtScripture.Text = "";
			txtPractice.Text = "";
		}


		protected void btnNextScripture_Click(object sender, EventArgs e)
        {
			Score();
			PopulateNewVerse();
		}
		protected void btnSwitchToTestMode_Click(object sender, EventArgs e)
        {
			if (_testingmemory.TestMode  == TestingMode.TRAIN)
			{
				_testingmemory.TestMode = TestingMode.TESTING;
			}
			else
			{
				_testingmemory.TestMode = TestingMode.TRAIN;
			}
			Session["testingmemory"] = _testingmemory;

			UpdateDisplay();
		}

		protected void btnGrade_Click(object sender, EventArgs e)
        {
			Score();
			ShowResults();
		}

		double Grade()
		{
			string sUserBook = ddBook.SelectedValue;
			int iChapter = (int)GetDouble(txtChapter.Text);
			int iVerse = (int)GetDouble(txtVerse.Text);
			sUserBook = sUserBook.ToUpper();
			_testingmemory.sCorrectBook = _testingmemory.sCorrectBook.ToUpper();

			double nResult = 0;
			if (sUserBook == _testingmemory.sCorrectBook)
				nResult += .3333;
			if (_testingmemory.nCorrectChapter == iChapter)
				nResult += .3333;

			if (_testingmemory.nCorrectVerse == iVerse)
				nResult += .3334;
			Session["testingmemory"] = _testingmemory;

			return nResult;
		}

		void Score()
		{
			if (_testingmemory.TestMode == TestingMode.TESTING)
			{
				double nPct = WordComparer(txtScripture.Text, txtPractice.Text);
				_testingmemory.nTrainingQuestionsTaken++;
				_testingmemory.nTrainingScore += nPct;
			}
			// Score the current Testing session
			if (_testingmemory.TestMode == TestingMode.TRAIN)
			{
				_testingmemory.nTestingQuestionsTaken++;
				double nTestPct = Grade();
				_testingmemory.nTestingScore += nTestPct;
			}
			Session["testingmemory"] = _testingmemory;

		}
	}
}