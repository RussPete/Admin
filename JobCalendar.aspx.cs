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

public partial class JobCalendar : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;
	protected Utils.Calendar calWeekDate;
	protected Utils.Calendar calMonthDate;
	protected Utils.Calendar calBegDateRange;
	protected Utils.Calendar calEndDateRange;
	protected bool fReport = false;
	protected DateTime dtBeg;
	protected DateTime dtEnd;
	protected CalSched.ViewPeriod CalPeriod;
	protected int iJobNum = 0;
	protected string sPeriod;
	protected DateTime dtPrevDay;
	protected int cDays;
	protected bool fNoDepartTime = false;
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
		DateTime Tomorrow = DateTime.Today.AddDays(1);

		txtWeekDate.Text = Fmt.Dt(Tomorrow);
		txtMonthDate.Text = Fmt.Dt(Tomorrow.AddDays(1 - Tomorrow.Day).AddMonths(1));
		txtBegDateRange.Text =
		txtEndDateRange.Text = txtWeekDate.Text;

		txtWeekDate.Attributes.Add("onChange", "iSetChk('rdoWeek', true);");
		txtMonthDate.Attributes.Add("onChange", "iSetChk('rdoMonth', true);");
		txtBegDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
		txtEndDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
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
		Response.Write("</head>\n<body class='RptBody'>\n");

		int cServings = 0;
		int cJobs = 0;
		cDays = 0;
		dtPrevDay = DateTime.MinValue;

		string Sql = String.Format(
			"Select Count(*) As Cnt, " +
			"(IsNull(Sum(NumMenServing), 0) + IsNull(Sum(NumWomenServing), 0) + IsNull(Sum(NumChildServing), 0)) As NumServings " +
			"From mcJobs Where JobDate Between {0} And {1} " +
			"And CancelledDtTm Is Null And ProposalFlg <> 1",
			DB.PutDtTm(dtBeg),
			DB.PutDtTm(dtEnd));

		try
		{
			DataTable dt = db.DataTable(Sql);
			DataRow dr = dt.Rows[0];
			cJobs = DB.Int32(dr["Cnt"]);
			cServings = DB.Int32(dr["NumServings"]);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		string Html =
			WebPage.Tabs(0) + WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='2' align='center'>\n" +
			WebPage.Tabs(3) + WebPage.Div("Job Calendar", "center", "RptTitle") +
			WebPage.Tabs(5) + WebPage.Div("Scheduled Jobs for the<br>" + sPeriod, "center", "RptSubTitle") +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 15) +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td><span class='RptInfo'>Click an event depature time to show or hide job details.</span></td>\n" +
			WebPage.Tabs(2) + "<td valign='bottom' align='right'>" +
			WebPage.Span(Fmt.Num(cServings), "RptNotice") + " Servings, " +
			WebPage.Span(Fmt.Num(cJobs), "RptNotice") + " Jobs, " +
			"<span id='cDays' class='RptNotice'>0</span> Days</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr><td colspan='2' valign='top'>\n" +
			"<br>\n";
		Response.Write(Html);

		try
		{
			fNoDepartTime = false;
			scDays.Clear();

			CalSched Cal = new CalSched(dtBeg, dtEnd);
			try
			{
				Cal.BegTimeStr("11:00am");
				Cal.EndTimeStr("4:00pm");
				Cal.TimeBlock = 5 * 60;
				Cal.DBConnection = db.Conn;
				Cal.SqlDataQuery(
					"Select Coalesce(DepartureTime, MealTime, JobDate) As DepartTime, DepartureTime, MealTime, Location, EventType, " +
                    "ServiceType, NumMenServing, NumWomenServing, NumChildServing, Coalesce(cu.Name, c.Name) as Customer " + 
                    "From mcJobs j " +
                    "Right Join Contacts c On j.ContactRno = c.ContactRno " +
                    "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
					"Where JobDate Between '{0}' And '{1}' And " +
					"CancelledDtTm Is Null And " +
                    "IsNull(ProposalFlg, 0) = 0" + 
                    "Order By JobDate, DepartTime, MealTime",
					"DepartTime",
					"DepartTime");
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
			"<br>\n" +
			WebPage.Tabs(1) + "</td></tr>\n" +
			WebPage.Tabs(1) + "<tr><td colspan='2' align='center'>\n" +
			WebPage.Tabs(2) + "<input type=button value='Show Details' title='Click to show job details for all events.' onClick='ShowAll();' class='RptSmallBtn'>&nbsp;&nbsp;\n" +
			WebPage.Tabs(2) + "<input type=button value='Hide Details' title='Click to hide job details for all events.' onClick='HideAll();' class='RptSmallBtn'>\n" +
			WebPage.Tabs(1) + "</td></tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd() +
			"<script lang='javascript'>\n" +
			"var EndJobNum = " + --iJobNum + ";\n" +
			"oSetStr('cDays', '" + Fmt.Num(cDays) + "');\n" +
			"</script>\n";
		Response.Write(Html);

	}

	public string FormatJob(DataRow dr)
	{
		string sDepart = Fmt.Tm12Hr(DB.DtTm(dr["DepartureTime"]));
		string sDepartNote = sDepart;
		string sMeal = Fmt.Tm12Hr(DB.DtTm(dr["MealTime"]));
		Int32 cMen = DB.Int32(dr["NumMenServing"]);
		Int32 cWomen = DB.Int32(dr["NumWomenServing"]);
		Int32 cChild = DB.Int32(dr["NumChildServing"]);

		if (sDepart.Length == 0)
		{
			if (sMeal.Length > 0)
			{
				sDepart = sMeal + "<sup>*</sup>";
				sDepartNote = sMeal;
				fNoDepartTime = true;
			}
			else
			{
				sDepart = "N/A";	// + "<sup>*</sup>";
				sDepartNote = "N/A";
				//					fNoDepartTime = true;
			}
		}


		string Html =
			WebPage.Tabs(3) + WebPage.Table("width='100%' class='NoBorder'") +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td><span class='CalMainTime'><a href='javascript:ShowJob(" + iJobNum + ");' title='Departure Time: " + sDepartNote + "\nMeal Time: " + sMeal + "\nClick to see details of this job.'>" + sDepart + "</a></span></td>\n" +
			WebPage.Tabs(5) + "<td>&nbsp;</td>\n" +
			WebPage.Tabs(5) + "<td align='right'><span class='CalMinorTime'>" + sMeal + "</span></td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(3) + WebPage.TableEnd() +
			WebPage.Tabs(3) + WebPage.Table("class='NoBorder'") +
			WebPage.Tabs(4) + WebPage.SpaceTr(10, 1) +
			JobField(dr, "Customer") +
			WebPage.Tabs(4) + "<tbody id='tblJob" + iJobNum + "' style='display: none;'>\n" +
			JobField(dr, "Location") +
			JobField(dr, "EventType") +
			JobField(dr, "ServiceType") +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td>&bull;</td>\n" +
			WebPage.Tabs(5) + "<td>" +
			Fmt.Num(cMen + cWomen + cChild) +
			"<span class='RptSmall'>&nbsp;&nbsp;(M " + cMen +
			",&nbsp;&nbsp;W " + cWomen +
			",&nbsp;&nbsp;C " + cChild + ")</span></td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "</tbody>\n" +
			WebPage.Tabs(3) + WebPage.TableEnd();

		iJobNum++;

		DateTime dtDepart = DB.DtTm(dr["DepartTime"]);
		sDepart = new DateTime(dtDepart.Year, dtDepart.Month, dtDepart.Day, 0, 0, 0).ToString("M/d/yyyy");

		// if this date hasn't seen yet
		if (scDays.IndexOf(sDepart) < 0)
		{
			scDays.Add(sDepart);
			cDays++;
		}

		return Html;
	}

	public string EndCal()
	{
		string Html =
			(fNoDepartTime ? "<sup>*</sup>Note: Depature Time not set, Meal Time used instead.<br>	\n" : "");
		fNoDepartTime = false;

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