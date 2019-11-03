using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using Utils;
using Globals;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class CrewCalendar : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;
	protected string FocusField = "";
	protected Utils.Calendar calWeekDate;
	protected Utils.Calendar calMonthDate;
	protected Utils.Calendar calBegDateRange;
	protected Utils.Calendar calEndDateRange;
	protected bool fReport = false;
	protected DateTime dtBeg;
	protected DateTime dtEnd;
	protected CalSched.ViewPeriod CalPeriod;
	protected bool fMaster;
	protected int iJobNum = 0;
	protected bool fNewPage;
	protected string sPeriod;
	protected int iCal = 0;
	protected int cDays;
	protected bool fNoReportTime = false;
	protected StringCollection scDays = new StringCollection();

	private void Page_Init(object sender, System.EventArgs e)
	{
		InitCalendars();
	}

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Setup();
		}
	}

	private void InitCalendars()
	{
		calWeekDate = new Utils.Calendar("WeekDate", ref txtWeekDate);
		calWeekDate.ImageButton(imgWeekDate);

		calMonthDate = new Utils.Calendar("MonthDate", ref txtMonthDate);
		calMonthDate.ImageButton(imgMonthDate);

		calBegDateRange = new Utils.Calendar("BegDateRange", ref txtBegDateRange);
		calBegDateRange.ImageButton(imgBegDateRange);

		calEndDateRange = new Utils.Calendar("EndDateRange", ref txtEndDateRange);
		calEndDateRange.ImageButton(imgEndDateRange);
	}

	private void Setup()
	{
		FocusField = "ddlCrewMember";

		String Sql =
			"Select Distinct CrewMember From mcJobCrew " +
			"Where CrewMember Is Not Null Order By CrewMember";
		try
		{
			DataTable dt = db.DataTable(Sql);
			ddlCrewMember.Items.Clear();
			foreach (DataRow dr in dt.Rows)
			{
				string CrewMember = DB.Str(dr["CrewMember"]);
				ddlCrewMember.Items.Add(new ListItem(CrewMember, CrewMember));
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		DateTime Tomorrow = DateTime.Today.AddDays(1);

		txtWeekDate.Text = Fmt.Dt(Tomorrow);
		txtMonthDate.Text = Fmt.Dt(Tomorrow.AddDays(1 - Tomorrow.Day).AddMonths(1));
		txtBegDateRange.Text =
		txtEndDateRange.Text = txtWeekDate.Text;

		ddlCrewMember.Attributes.Add("onChange", "iSetChk('chkCrewMember', true);");
		txtWeekDate.Attributes.Add("onChange", "iSetChk('rdoWeek', true);");
		txtMonthDate.Attributes.Add("onChange", "iSetChk('rdoMonth', true);");
		txtBegDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
		txtEndDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
		btnReport.Attributes.Add("OnClick", "return Print();");
	}

	protected string SetFocus()
	{
		string Focus = "";

		if (FocusField.Length > 0)
		{
			Focus = "SetFocus(\"" + FocusField + "\");";
		}

		return Focus;
	}

	protected void btnReport_Click(object sender, System.EventArgs e)
	{
		if (rdoWeek.Checked)
		{
			dtBeg = Convert.ToDateTime(txtWeekDate.Text);
			dtEnd = EndDate(dtBeg.AddDays(6));
			CalPeriod = CalSched.ViewPeriod.Week;
			sPeriod = "Week Beginning " + Fmt.Dt(dtBeg);
		}
		if (rdoMonth.Checked)
		{
			dtBeg = Convert.ToDateTime(txtMonthDate.Text);
			dtBeg = dtBeg.AddDays(1 - dtBeg.Day);
			dtEnd = EndDate(dtBeg.AddMonths(1).AddDays(-1));
			CalPeriod = CalSched.ViewPeriod.Month;
			sPeriod = "Month of " + dtBeg.ToString("MMMM yyyy");
		}
		if (rdoRange.Checked)
		{
			dtBeg = Convert.ToDateTime(txtBegDateRange.Text);
			dtEnd = Convert.ToDateTime(txtEndDateRange.Text);
			CalPeriod = ((dtEnd.Ticks - dtBeg.Ticks) / TimeSpan.TicksPerDay < 7 ? CalSched.ViewPeriod.Week : CalSched.ViewPeriod.Month);
			dtEnd = EndDate(dtEnd);
			sPeriod = "Date Range " + Fmt.Dt(dtBeg) + " - " + Fmt.Dt(dtEnd);
		}

		fReport = true;
	}

	private DateTime EndDate(DateTime dt)
	{
		return new DateTime(dt.Year, dt.Month, dt.Day, 23, 59, 59);
	}

	protected void Report()
	{
		fNewPage = false;
		fMaster = false;

		Response.Write("</head>\n<body class='RptBody'>\n");

		if (chkAllCrewMembers.Checked ||
			chkMaster.Checked)
		{
			Response.Write(
				"<div align='center' class='RptTopSpace'><br><br>\n" +
				"<span class='RptInfo'>Click an event time to show or hide job details.</span>\n<br><br>" +
				"<input type=button value='Show Details' title='Click to show job details for all events.' onClick='ShowAll();' class='RptSmallBtn'>\n" +
				"&nbsp;&nbsp;<input type=button value='Hide Details' title='Click to hide job details for all events.' onClick='HideAll();' class='RptSmallBtn'><br><br><br></div>\n"
				);
		}

		if (chkMaster.Checked)
		{
			fMaster = true;
			ReportCrew("All Crew Members", false, true);
			fMaster = false;
		}

		if (chkCrewMember.Checked)
		{
			ReportCrew(ddlCrewMember.SelectedValue, true, false);
		}

		if (chkAllCrewMembers.Checked)
		{
			int cDataFound = 0;

			String Sql = "Select Distinct CrewMember From mcJobCrew Order By CrewMember";
			try
			{
				DataTable dt = db.DataTable(Sql);
				foreach (DataRow dr in dt.Rows)
				{
					if (ReportCrew(DB.Str(dr["CrewMember"]), false, false))
					{
						cDataFound++;
					}
				}
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}

			if (cDataFound == 0)
			{
				Response.Write("<div align='center' class='RptTitle'>No Jobs Found</div>\n");
			}
		}

		Response.Write(
			"<div align='center'><input type=button value='Show Details' title='Click to show job details for all events.' onClick='ShowAll();' class='RptSmallBtn'>\n" +
			"&nbsp;&nbsp;<input type=button value='Hide Details' title='Click to hide job details for all events.' onClick='HideAll();' class='RptSmallBtn'></div>\n"
			);
		Response.Write("<script lang='javascript'>var EndJobNum = " + --iJobNum + ";</script>\n");
	}

	protected bool ReportCrew(string CrewMember, bool fSingle, bool fMaster)
	{
		bool fDataFound = false;

		int cAssignments = 0;
		int cJobs = 0;
		cDays = 0;

		string Sql = String.Format(
			"Select Count(*) As Cnt " +
			"From mcJobCrew c Inner Join mcJobs j On c.JobRno = j.JobRno " +
			"Where JobDate Between {0} And {1} " +
			(fMaster ? "" : "And CrewMember = {2} ") +
			"And CancelledDtTm Is Null",
			DB.PutDtTm(dtBeg),
			DB.PutDtTm(dtEnd),
			DB.PutStr(CrewMember));
		cAssignments = db.SqlNum(Sql);

		Sql = String.Format(
			"Select Distinct c.JobRno " +
			"From mcJobCrew c Inner Join mcJobs j On c.JobRno = j.JobRno " +
			"Where JobDate Between {0} And {1} " +
			(fMaster ? "" : "And CrewMember = {2} ") +
			"And CancelledDtTm Is Null",
			DB.PutDtTm(dtBeg),
			DB.PutDtTm(dtEnd),
			DB.PutStr(CrewMember));
		try
		{
			DataTable dt = db.DataTable(Sql);
			cJobs = dt.Rows.Count;
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		if (cAssignments > 0 || fSingle)
		{
			fDataFound = true;

			string Html =
				WebPage.Tabs(0) + WebPage.Table("align='center'" + (fNewPage ? " style='page-break-before: always;'" : "")) +
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td colspan='2' align='center'>\n" +
				WebPage.Tabs(3) + WebPage.Div("Crew Calendar", "center", "RptTitle") +
				WebPage.Tabs(5) + WebPage.Div("Job Assignments for <span class='RptSubTitle'>" + CrewMember + "</span><br>" + sPeriod, "center", "RptSubTitleSmall") +
				WebPage.Tabs(2) + "</td>\n" +
				WebPage.Tabs(1) + "</tr>\n" +
				WebPage.Tabs(1) + WebPage.SpaceTr(1, 15) +
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td><span class='RptInfo'>Click an event time to show or hide job details.</span></td>\n" +
				WebPage.Tabs(2) + "<td valign='bottom' align='right'>" +
				WebPage.Span(Fmt.Num(cAssignments), "RptNotice") + " Assignments, " +
				WebPage.Span(Fmt.Num(cJobs), "RptNotice") + " Jobs, " +
				"<span id='cDays" + ++iCal + "' class='RptNotice'>0</span> Days</td>\n" +
				WebPage.Tabs(1) + "</tr>\n" +
				WebPage.Tabs(1) + "<tr><td colspan='2' valign='top'>\n" +
				"<br>\n";
			Response.Write(Html);

			try
			{
				fNoReportTime = false;
				scDays.Clear();

				CalSched Cal = new CalSched(dtBeg, dtEnd);
				try
				{
					Cal.BegTimeStr("11:00am");
					Cal.EndTimeStr("4:00pm");
					Cal.TimeBlock = 5 * 60;
					Cal.DBConnection = db.Conn;
					Cal.SqlDataQuery(
						"Select Coalesce(ReportTime, MealTime, JobDate) As ReportTm, ReportTime, MealTime, " +
						"CrewMember, CrewAssignment, Note, Customer, Location, EventType, " +
						"ServiceType " +
						"From mcJobCrew c Inner Join mcJobs j On c.JobRno = j.JobRno " +
						"Where JobDate Between '{0}' And '{1}' " +
						(fMaster ? "" : "And CrewMember = " + DB.PutStr(CrewMember) + " ") +
						"And CancelledDtTm Is Null " +
						"Order By JobDate, ReportTime, MealTime",
						"ReportTm",
						"ReportTm");
					Cal.FormatData = new CalSched.DataFormater(FormatJob);
					Cal.EndCal = new CalSched.CalEvent(EndCal);
					Cal.ShowTimeColumn = false;
					Cal.View(Response, this.CalPeriod);
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex, Cal.SqlQuery);
					Response.Write(Err.Html());
				}
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}

			Html =
				WebPage.Tabs(1) + "</td></tr>\n" +
				WebPage.Tabs(1) + WebPage.SpaceTr(1, 20) +
				WebPage.Tabs(0) + WebPage.TableEnd() +
				"<script language='javascript'>oSetStr('cDays" + iCal + "', '" + Fmt.Num(cDays) + "');</script>\n";
			Response.Write(Html);

			fNewPage = true;
		}

		return fDataFound;
	}

	public string FormatJob(DataRow dr)
	{
		string sReport = Fmt.Tm12Hr(DB.DtTm(dr["ReportTime"]));
		string sReportNote = sReport;
		string sMeal = Fmt.Tm12Hr(DB.DtTm(dr["MealTime"]));

		if (sReport.Length == 0)
		{
			if (sMeal.Length > 0)
			{
				sReport = sMeal + "<sup>*</sup>";
				sReportNote = sMeal;
				fNoReportTime = true;
			}
			else
			{
				sReport = "N/A";
				sReportNote = "N/A";
				fNoReportTime = false;
			}
		}


		string Html =
			WebPage.Tabs(3) + WebPage.Table("width='100%' class='NoBorder'") +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td><span class='CalMainTime'><a href='javascript:ShowJob(" + iJobNum + ");' title='Report Time: " + sReportNote + "\nMeal Time: " + sMeal + "\nClick to see details of this job.'>" + sReport + "</a></span></td>\n" +
			WebPage.Tabs(5) + "<td>&nbsp;</td>\n" +
			WebPage.Tabs(5) + "<td align='right'><span class='CalMinorTime'>" + sMeal + "</span></td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(3) + WebPage.TableEnd() +
			WebPage.Tabs(3) + WebPage.Table("class='NoBorder'") +
			WebPage.Tabs(4) + WebPage.SpaceTr(10, 1) +
			(fMaster ? WebPage.Tabs(4) + "<tr><td colspan='2'><b>" + DB.Str(dr["CrewMember"]) + "</b></td></tr>\n" : "") +
			JobField(dr, "Customer") +
			JobField(dr, "CrewAssignment") +
			JobField(dr, "Note") +
			WebPage.Tabs(4) + "<tbody id='tblJob" + iJobNum + "' style='display: none;'>\n" +
			JobField(dr, "Location") +
			JobField(dr, "EventType") +
			JobField(dr, "ServiceType") +
			WebPage.Tabs(4) + "</tbody>\n" +
			WebPage.Tabs(3) + WebPage.TableEnd();

		iJobNum++;

		DateTime dtReport = DB.DtTm(dr["ReportTm"]);
		sReport = new DateTime(dtReport.Year, dtReport.Month, dtReport.Day, 0, 0, 0).ToString("M/d/yyyy");

		// if this date hasn't seen yet
		if (scDays.IndexOf(sReport) < 0)
		{
			scDays.Add(sReport);
			cDays++;
		}

		return Html;
	}

	public string EndCal()
	{
		string Html =
			(fNoReportTime ? "<sup>*</sup>Note: Report Time not set, Meal Time used instead.<br>\n" : "");
		fNoReportTime = false;

		return Html;
	}

	protected string JobField(DataRow dr, string Field)
	{
		string Html = "";
		string sField = DB.Str(dr[Field]);

		if (sField.Length > 0)
		{
			Html =
				WebPage.Tabs(4) + "<tr>\n" +
				WebPage.Tabs(5) + "<td valign='top'>&bull;</td>\n" +
				WebPage.Tabs(5) + "<td>" + sField + "</td>\n" +
				WebPage.Tabs(4) + "</tr>\n";
		}

		return Html;
	}
}