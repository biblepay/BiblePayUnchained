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

namespace Unchained
{

	public enum ExamMode
	{
		TESTING = 0,
		TRAIN = 1
	}

	public struct ExamMemory
	{
		public int nCurrentQuestion;
		public List<string> vecQ;
		public List<string> vecA;
		public Dictionary<int, string> mapChosen;
		public string[] vecAnswerKey;
		public bool fTesting;
		public ExamMode ExamMode;
		public int nTrainingQuestionsTaken;
		public int nTestingQuestionsTaken;
		public double nTrainingScore;
		public double nTestingScore;
		public string Course;
		public int StartTime;
	};
	public partial class BBPUnivFinalExam : BBPPage
    {
		private BiblePayDLL.Bible _bible = new BiblePayDLL.Bible();

		protected ExamMemory _exammemory = new Unchained.ExamMemory();

		protected void Timer1_Tick(object sender, EventArgs e)
		{
			int nElapsed = UnixTimeStamp() - _exammemory.StartTime;
			int nMins = 0;
			int nSeconds = nElapsed;
			if (nElapsed > 60)
			{
				nMins = nElapsed / 60;
				nSeconds = nElapsed - (nMins * 60);
			}
			
			lblElapsed.Text = string.Format("{0:D2}", nMins)
				+ ":" + String.Format("{0:D2}", nSeconds);
		}

		private void InitializeTest()
		{
			string sFinalExam = Request.QueryString["test"].ToNonNullString();
			string sURL = "https://foundation.biblepay.org/Univ/" + sFinalExam + "_key.xml";
			string sExam = BiblePayDLL.Sidechain.DownloadResourceAsString(sURL);
			string sAnswerKey = Common.ExtractXML(sExam, "<KEY>", "</KEY>");
			if (sAnswerKey == "")
			{
				MsgBox("Error", "Answer key missing", this);
			}

			_exammemory.vecAnswerKey = sAnswerKey.Split(",");
			string sQ = Common.ExtractXML(sExam, "<QUESTIONS>", "</QUESTIONS>").ToString();
			_exammemory.Course = "Final Exam: " + Common.ExtractXML(sExam, "<COURSE>", "</COURSE>");
			_exammemory.vecQ.Clear();
			_exammemory.vecA.Clear();

			string[] vQ = sQ.Split("<QUESTIONRECORD>");
			for (int i = 0; i < (int)vQ.Count(); i++)
			{
				string sQ1 = Common.ExtractXML(vQ[i], "<Q>", "</Q>");
				string sA1 = Common.ExtractXML(vQ[i], "<A>", "</A>");
				if (!sQ1.IsEmpty() && !sA1.IsEmpty())
				{
					_exammemory.vecQ.Add(sQ1);
					_exammemory.vecA.Add(sA1);
				}
			}
			_exammemory.fTesting = true;
			_exammemory.StartTime = UnixTimeStamp();
			Session["exammemory"] = _exammemory;
		}
		protected new void Page_Load(object sender, EventArgs e)
        {
			if (Session["exammemory"] != null)
			{
				_exammemory = (ExamMemory)Session["exammemory"];
			}
			else
			{
				_exammemory.ExamMode = ExamMode.TRAIN;
				_exammemory.mapChosen = new Dictionary<int, string>();

				_exammemory.vecA = new List<string>();
				_exammemory.vecQ = new List<string>();
				_exammemory.mapChosen.Clear();
				_exammemory.nCurrentQuestion = 0;

				Session["exammemory"] = _exammemory;
				InitializeTest();

				PopulateQuestion();

			}

			lblTitle.Text = _exammemory.Course;

		}

		string ExtractAnswer(string sLetter)
		{
			string[] vAnswers = _exammemory.vecA[_exammemory.nCurrentQuestion].Split("|");
			
			if (sLetter == "A")
			{
				return vAnswers[0];
			}
			else if (sLetter == "B")
			{
				return vAnswers[1];
			}
			else if (sLetter == "C")
			{
				return vAnswers[2];
			}
			else if (sLetter == "D")
			{
				return vAnswers[3];
			}
			return "N/A";
		}

		void ResetRadios()
		{
			radioAnswerA.Checked = false;
			radioAnswerB.Checked = false;
			radioAnswerC.Checked = false;
			radioAnswerD.Checked = false;
		}

		void StripNumber()
		{
			string sSource = _exammemory.vecQ[_exammemory.nCurrentQuestion];
			int pos = sSource.IndexOf("."); 
			string sPrefix = sSource.Substring(0, pos + 1);
			string sMyQ = sSource.Replace(sPrefix, "");
			sMyQ = sMyQ.Trim();
			int nQN = (int)GetDouble(sPrefix + "0");
			txtQuestionNo.Text = nQN.ToString();
			txtQuestion.Text = sMyQ;
		}

		string GetPopUpVerses(string sRange)
		{
			string[] vR = sRange.Split(" ");
			if (vR.Length < 2)
				return "";

			string sBook = vR[0];
			string sChapterRange = vR[1];
			string[] vChap = sChapterRange.Split(":");
			if (vChap.Length < 2)
				return "";

			double nChapter = GetDouble(vChap[0]);
			if (nChapter < 1)
				return "";
			string[] vChapRange = vChap[1].Split("-");

			if (vChapRange.Length < 2)
			{
				vChap[1] = vChap[1] + "-" + vChap[1];
				vChapRange = vChap[1].Split("-");
			}
			int nVerseStart = (int)GetDouble(vChapRange[0]);
			double nVerseEnd = GetDouble(vChapRange[1]);
			if (nVerseStart < 1 || nVerseEnd < 1)
				return "";

			if (sBook == "I Corinthians")
				sBook = "1 Corinthians"; // Harvest Time format->KJV format
			if (sBook == "I John")
				sBook = "1 John";
			if (sBook == "Corinthians")
				sBook = "1 Corinthians";

			string sShortBook = _bible.GetBookByName(sBook);

			string sTotalVerses = sRange + "\r\n";
			
      		for (int j = nVerseStart; j <= nVerseEnd; j++)
			{
				string sVerse =_bible.GetVerse(sShortBook, (int)nChapter, j);
				sTotalVerses += sVerse + "\r\n";
			}
			return sTotalVerses;
		}


		string ScanAnswerForPopUpVerses(string sRefText)
		{
			string[] vSourceScripture = sRefText.Split(" ");
			string sExpandedAnswer = "";
			for (int i = 0; i < (int)vSourceScripture.Length - 1; i++)
			{
				string sScrip = vSourceScripture[i] + " " + vSourceScripture[i + 1];
				string sExpandedVerses = GetPopUpVerses(sScrip);
				if (!sExpandedVerses.IsEmpty())
				{
					sExpandedAnswer += sExpandedVerses + "\r\n\r\n";
				}
			}
			return sExpandedAnswer;
		}

		void PopulateQuestion()
		{
			if (_exammemory.nCurrentQuestion > _exammemory.vecA.Count)
			{
				MsgBox("Error", "FinalExam::Error Size too small", this);
			}

			string[] vAnswers = _exammemory.vecA[_exammemory.nCurrentQuestion].Split("|");
			if (vAnswers.Length > 3)
			{
				lblA.Text = vAnswers[0];
				lblB.Text = vAnswers[1];
				lblC.Text = vAnswers[2];
				lblD.Text = vAnswers[3];
				radioAnswerC.Visible = true;
				radioAnswerD.Visible = true;
			}
			else if (vAnswers.Length > 1)
			{
				lblA.Text = vAnswers[0];
				lblB.Text = vAnswers[1];
				lblC.Text = "";
				lblD.Text = "";
				radioAnswerC.Visible = false;
				radioAnswerD.Visible = false;
			}


			
			string sChosen = "";
			_exammemory.mapChosen.TryGetValue(_exammemory.nCurrentQuestion, out sChosen);

			if (_exammemory.ExamMode == ExamMode.TRAIN)
			{
				
			}
			if (sChosen == "A")
			{
				radioAnswerA.Checked = true;
			}
			else if (sChosen == "B")
			{
				radioAnswerB.Checked = true;
			}
			else if (sChosen == "C")
			{
				radioAnswerC.Checked = true;
			}
			else if (sChosen == "D")
			{
				radioAnswerD.Checked = true;
			}

			if (sChosen.IsEmpty())
			{
				ResetRadios();
			}

			StripNumber();

			if (_exammemory.ExamMode == ExamMode.TESTING)			{
				// TEST mode
				txtAnswer.Text = "";
			}
			else
			{
				// In Training Mode, if we have any bible verses in the answer, lets include the actual scripture to help the student:
				string sExpandedAnswer = ExtractAnswer(_exammemory.vecAnswerKey[_exammemory.nCurrentQuestion]);
				string sRefText = sExpandedAnswer + " " + _exammemory.vecQ[_exammemory.nCurrentQuestion];
				string sBiblicalRefs = ScanAnswerForPopUpVerses(sRefText);
				sExpandedAnswer += "\r\n\r\n" + sBiblicalRefs;
				txtAnswer.Text = sExpandedAnswer;
			}

			string sCaption = _exammemory.ExamMode == ExamMode.TRAIN ? "Switch to TEST Mode" : "Switch to REVIEW Mode";
		 	btnSwitch.Text = sCaption;

			// Load initial values
			string sMode = _exammemory.ExamMode == ExamMode.TRAIN ? "<font color=red>LEARNING MODE</font>" : "<font color=red>TESTING MODE</font>";
			string sInfo = sMode + "<br><br>Welcome to your Final Exam, " + gUser(this).UserName + "!";
			lblInfo.Text = sInfo;
		}

		double CalculateScores()
		{
			double nTotalCorrect = 0;
			double nTaken = 0;
			for (int i = 0; i < _exammemory.vecAnswerKey.Length; i++)
			{
				string sChosen = "";
				_exammemory.mapChosen.TryGetValue(i, out sChosen);

				double nCorrect = _exammemory.vecAnswerKey[i] == sChosen ? 1 : 0;
				nTotalCorrect += nCorrect;
				if (!sChosen.IsEmpty())
				{
					nTaken++;
				}

			}
			_exammemory.nTestingQuestionsTaken = (int)nTaken;
			double nScore = nTotalCorrect / (_exammemory.vecAnswerKey.Length + .001);
			Session["exammemory"] = _exammemory;
			return nScore;
		}

		void ShowResults()
		{
			string sTitle = "Results";
			double nTestingPct = CalculateScores();

			string sSummary = "Congratulations!";
			sSummary += "<br>You worked through " + _exammemory.nTestingQuestionsTaken.ToString()
				+ ", and your score is " + Math.Round(nTestingPct * 100, 2).ToString() + "%! ";
			sSummary += "<br>Please come back and see us again. ";
			// Clear the results so they can start again if they want:
			_exammemory.nTrainingQuestionsTaken = 0;
			_exammemory.nTestingQuestionsTaken = 0;
			_exammemory.nTrainingScore = 0;
			_exammemory.nTestingScore = 0;
			Session["exammemory"] = _exammemory;

			UICommon.MsgModal(this, sTitle, sSummary, 500, 300);
		}

		protected void btnGrade_Click(object sender, EventArgs e)
		{
			RecordAnswer();

			_exammemory.fTesting = false;
			Score();
			ShowResults();
		}

		void RecordAnswer()
		{
			string sChosen = "";
			if (radioAnswerA.Checked)
			{
				sChosen = "A";
			}
			else if (radioAnswerB.Checked)
			{
				sChosen = "B";
			}
			else if (radioAnswerC.Checked)
			{
				sChosen = "C";
			}
			else if (radioAnswerD.Checked)
			{
				sChosen = "D";
			}
			_exammemory.mapChosen[_exammemory.nCurrentQuestion] = sChosen;
			Session["exammemory"] = _exammemory;
		}

		double Grade()
		{
			string sChosen = "";
			_exammemory.mapChosen.TryGetValue(_exammemory.nCurrentQuestion, out sChosen);

			double nCorrect = (_exammemory.vecAnswerKey[_exammemory.nCurrentQuestion] == sChosen) ? 1 : 0;
			return nCorrect;
		}

		void OnAfterUpdated()
		{
			double nCorrect = Grade();
			if (_exammemory.ExamMode == ExamMode.TESTING)
			{
				lblGrade.Text = "";
			}
			else
			{
				string sChosen = "";
				_exammemory.mapChosen.TryGetValue(_exammemory.nCurrentQuestion, out sChosen);
				if (!sChosen.IsEmpty())
				{
					string sCorrNarr = nCorrect == 1 ? "<font color=red>Correct</font>" : "<font color=red>Incorrect</font>";
					lblGrade.Text = sCorrNarr;
				}
				else
				{
					lblGrade.Text = "";
				}
			}
		}

		void Score()
		{
			double nTestPct = Grade();
			if (_exammemory.ExamMode == ExamMode.TESTING)
			{
				_exammemory.nTrainingQuestionsTaken++;
				_exammemory.nTrainingScore += nTestPct;
			}
			// Score the current Testing session
			if (_exammemory.ExamMode == ExamMode.TRAIN)
			{
				_exammemory.nTestingQuestionsTaken++;
				_exammemory.nTestingScore += nTestPct;
			}
			Session["exammemory"] = _exammemory;
		}

		void ContinueTimer()
		{
			
		}

		protected void btnNext_Click(object sender, EventArgs e)
		{

			RecordAnswer();
			Score();
			_exammemory.nCurrentQuestion++;

			Session["exammemory"] = _exammemory;

			if (_exammemory.nCurrentQuestion > _exammemory.vecQ.Count - 1)
			{
				_exammemory.nCurrentQuestion = _exammemory.vecQ.Count - 1;

				Session["exammemory"] = _exammemory;

				return;
			}
			PopulateQuestion();
			OnAfterUpdated();


			ContinueTimer();
		}

		protected void btnBack_Click(object sender, EventArgs e)
		{
			RecordAnswer();

			Score();
			_exammemory.nCurrentQuestion--;
			Session["exammemory"] = _exammemory;

			if (_exammemory.nCurrentQuestion < 0)
			{
				_exammemory.nCurrentQuestion = 0;
				Session["exammemory"] = _exammemory;
				return;
			}
			PopulateQuestion();
			OnAfterUpdated();

			ContinueTimer();
		}

		protected void btnSwitch_Click(object sender, EventArgs e)
		{

			if (!_exammemory.fTesting)
			{
				_exammemory.fTesting = true;
				
			}
			
			if (_exammemory.ExamMode == ExamMode.TESTING)
			{
				_exammemory.ExamMode = ExamMode.TRAIN;

			}
			else
			{
				_exammemory.ExamMode = ExamMode.TESTING;

			}
			Session["exammemory"] = _exammemory;
		}

	}
}
