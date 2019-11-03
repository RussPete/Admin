using System;
using System.Collections;
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

public partial class JobsEntered : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;
	protected Utils.Calendar calDayDate;
	protected Utils.Calendar calWeekDate;
	protected Utils.Calendar calMonthDate;
	protected Utils.Calendar calBegDateRange;
	protected Utils.Calendar calEndDateRange;
	protected bool fReport = false;
	protected DateTime dtBeg;
	protected DateTime dtEnd;
	protected Int32 JobRno;
	protected bool fBlank;

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
		calDayDate = new Utils.Calendar("DayDate", ref txtDayDate);
		calDayDate.ImageButton(imgDayDate);

		calWeekDate = new Utils.Calendar("WeekDate", ref txtWeekDate);
		calWeekDate.ImageButton(imgWeekDate);

		calMonthDate = new Utils.Calendar("MonthDate", ref txtMonthDate);
		calMonthDate.ImageButton(imgMonthDate);
		calMonthDate.HideControlsList = "ddlCustomerDate";

		calBegDateRange = new Utils.Calendar("BegDateRange", ref txtBegDateRange);
		calBegDateRange.ImageButton(imgBegDateRange);
		calBegDateRange.HideControlsList = "ddlCustomerDate, ddlCopies";

		calEndDateRange = new Utils.Calendar("EndDateRange", ref txtEndDateRange);
		calEndDateRange.ImageButton(imgEndDateRange);
		calEndDateRange.HideControlsList = "ddlCustomerDate";
	}

	private void Setup()
	{
		DateTime Today = DateTime.Today;

		txtDayDate.Text = Fmt.Dt(Today);
		txtWeekDate.Text = Fmt.Dt(Today.AddDays(-7));
		txtMonthDate.Text = Fmt.Dt(Today.AddDays(1 - Today.Day));
		txtBegDateRange.Text =
		txtEndDateRange.Text = txtDayDate.Text;

		txtDayDate.Attributes.Add("onChange", "iSetChk('rdoDay', true);");
		txtWeekDate.Attributes.Add("onChange", "iSetChk('rdoWeek', true);");
		txtMonthDate.Attributes.Add("onChange", "iSetChk('rdoMonth', true);");
		txtBegDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
		txtEndDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");

		btnReport.Attributes.Add("onClick", "NewWindow();");
	}

	private void btnReport_Click(object sender, System.EventArgs e)
	{
		fBlank = false;

		if (rdoDay.Checked)
		{
			dtBeg = Convert.ToDateTime(txtDayDate.Text);
			dtEnd = dtBeg;
		}

		if (rdoWeek.Checked)
		{
			dtBeg = Convert.ToDateTime(txtWeekDate.Text);
			dtEnd = dtBeg.AddDays(7);
		}
		if (rdoMonth.Checked)
		{
			dtBeg = Convert.ToDateTime(txtMonthDate.Text);
			dtBeg = dtBeg.AddDays(1 - dtBeg.Day);
			dtEnd = dtBeg.AddMonths(1).AddDays(-1);
		}
		if (rdoRange.Checked)
		{
			dtBeg = Convert.ToDateTime(txtBegDateRange.Text);
			dtEnd = Convert.ToDateTime(txtEndDateRange.Text);
		}

		fReport = true;
	}

	protected void Report()
	{
		Response.Write("\t</head>\n\t<body class='JobBody'>\n");

		Summary();
		PrintJobs();
	}

	protected void Summary()
	{
		string SubTitle = Fmt.Dt(dtBeg) + " - " + Fmt.Dt(dtEnd);

		Response.Write(
			WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td width='30%' valign='bottom'>\n" +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(2) + "<td align='center'>\n" +
			WebPage.Tabs(3) + WebPage.Div("Jobs Entered", "center", "RptTitle") +
			WebPage.Tabs(3) + WebPage.Div(SubTitle, "center", "RptSubTitle") +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(2) + "<td width='30%' valign='bottom' align='right'>" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.TableEnd() +
			WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(20, 30) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(5, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(5, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr><td colspan='25' align='right' id='Totals'></td></tr>\n" +
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 10) +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td align='center'><b>Entered</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='center'><b>Time</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='center'><b>Date</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='center'><b>Depart Time</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='center'><b>Meal Time</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td><b>Customer</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td><b>Event Type</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td><b>Service Type</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right'><b>Servings</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right' class='SmallPrint'><b>M</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right' class='SmallPrint'><b>W</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right' class='SmallPrint'><b>C</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right' class='SmallPrint'><b>Job #</b></td>\n" +
			WebPage.Tabs(1) + "</tr>\n"
			);
	}

	protected void PrintJobs()
	{
		DateTime PrevDate = DateTime.MinValue;
		int cDays = 0;
		int cJobs = 0;
		int cServ = 0;

		string SqlWhere = "";
		SqlWhere = DB.And(SqlWhere, "CreatedDtTm Between " + DB.PutDtTm(dtBeg) + " And " + DB.PutDtTm(dtEnd));

		string Sql =
			"Select * From mcJobs " +
			"Where " + SqlWhere +
			" Order By CreatedDtTm";
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				JobRno = DB.Int32(dr["JobRno"]);

				int cMen = DB.Int32(dr["NumMenServing"]);
				int cWomen = DB.Int32(dr["NumWomenServing"]);
				int cChild = DB.Int32(dr["NumChildServing"]);
				int cServings = cMen + cWomen + cChild;
				DateTime JobDate = DB.DtTm(dr["JobDate"]);

				if (JobDate != PrevDate)
				{
					cDays++;
					PrevDate = JobDate;
				}

				cJobs++;
				cServ += cServings;

				Response.Write(
					WebPage.Tabs(1) + "<tr>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dt(DB.DtTm(dr["CreatedDtTm"])) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Tm12Hr(DB.DtTm(dr["CreatedDtTm"])) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dt(JobDate) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Tm12Hr(DB.DtTm(dr["DepartureTime"])) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Tm12Hr(DB.DtTm(dr["MealTime"])) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td>" + DB.Str(dr["Customer"]) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td>" + DB.Str(dr["EventType"]) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td>" + DB.Str(dr["ServiceType"]) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Num(cServings, false) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right' class='SmallPrint'>" + Fmt.Num(cMen, false) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right' class='SmallPrint'>" + Fmt.Num(cWomen, false) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right' class='SmallPrint'>" + Fmt.Num(cChild, false) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right' class='SmallPrint'>" + JobRno + "</td>\n" +
					WebPage.Tabs(1) + "</tr>\n"
					);
			}

			if (dt.Rows.Count == 0)
			{
				Response.Write("<br><br><div class='JobSubTitle' align='center'>No Jobs Found to Print</div>\n");
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		Response.Write(WebPage.Tabs(0) + WebPage.TableEnd());

		string sTotals = "<b>" + cDays + "</b> Dates,&nbsp;&nbsp;<b>" + cJobs + "</b> Jobs,&nbsp;&nbsp;<b>" + Fmt.Num(cServ) + "</b> Servings";
		Response.Write("<script language='javascript'>oSetStr('Totals', '" + sTotals + "');</script>\n");
	}
}