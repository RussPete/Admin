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

public partial class JobSheets : System.Web.UI.Page
{
	private bool fExtraBlanks = false;

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
	protected int NumCopies;
	protected int DefaultQty;
	protected bool fBlank;
	protected bool fFinalPrint;
	protected bool fSummaryOnly;
	protected bool fSingleJob;
	protected bool fNewPage;
	protected bool fByCreatedDt;
	private bool fByKitchenLocation = true;

	private void Page_Init(object sender, System.EventArgs e)
	{
		db = new DB();

		InitCalendars();

		WebConfig Cfg = new WebConfig();
		fByKitchenLocation = Cfg.Bool("ByKitchenLocation");
	}

	private void Page_Load(object sender, System.EventArgs e)
	{
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
		if (Request.QueryString["JobRno"] != null)
		{
			JobRno = Str.Num(Request.QueryString["JobRno"]);
			fReport = true;
			fSingleJob = true;

			fFinalPrint = (Request.QueryString["FinalPrint"] == "1");
			fByCreatedDt = false;
		}

		DateTime Today = DateTime.Today;

		txtDayDate.Text = Fmt.Dt(Today);
		txtWeekDate.Text = txtDayDate.Text;
		txtMonthDate.Text = Fmt.Dt(Today.AddDays(1 - Today.Day).AddMonths(1));
		txtBegDateRange.Text =
		txtEndDateRange.Text = txtDayDate.Text;

		CustomerJobs();

		txtDayDate.Attributes.Add("onChange", "iSetChk('rdoDay', true);");
		txtWeekDate.Attributes.Add("onChange", "iSetChk('rdoWeek', true);");
		txtMonthDate.Attributes.Add("onChange", "iSetChk('rdoMonth', true);");
		txtBegDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
		txtEndDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);");
		ddlCustomerDate.Attributes.Add("onChange", "iSetChk('rdoCustomerDate', true);");
		ddlCopies.Attributes.Add("onChange", "iSetChk('rdoBlank', true);");

		rdoJobDate.Attributes.Add("onClick", "OptionsSet();");
		rdoEnteredDate.Attributes.Add("onClick", "OptionsSet();");

		chkNotPrintedOnly.Attributes.Add("onClick", "OptionSet(this.checked, 'tdNotPrintedOnly');");
		chkSummaryOnly.Attributes.Add("onClick", "OptionSet(this.checked, 'tdSummaryOnly');");

		chkIncludeAll.Attributes.Add("onClick", "SameWindow();");
		btnReport.Attributes.Add("onClick", "NewWindow();");
	}

	private void CustomerJobs()
	{
		string Sql = "Select JobRno, Coalesce(cu.Name, c.Name) as Customer, JobDate " +
            "From mcJobs j " +
            "Inner Join Contacts c On j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " + 
            "Where CancelledDtTm Is Null And ProposalFlg <> 1 ";
		if (!chkIncludeAll.Checked)
		{
			Sql += "And JobDate >= " + DB.PutDtTm(DateTime.Today) + " And PrintedDtTm Is Null ";
		}
		Sql += "Order By Customer, JobDate";

		try
		{
			DataTable dt = db.DataTable(Sql);
			ddlCustomerDate.Items.Clear();
			foreach (DataRow dr in dt.Rows)
			{
				//ddlCustomerDate.Items.Add(new ListItem(DB.Str(dr["Customer"]) + " - " + Fmt.Dt(DB.DtTm(dr["JobDate"])), DB.Str(dr["JobRno"])));
				ddlCustomerDate.Items.Add(new ListItem(Fmt.Dt(DB.DtTm(dr["JobDate"])) + " - " + DB.Str(dr["Customer"]), DB.Str(dr["JobRno"])));
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void chkIncludeAll_CheckedChanged(object sender, System.EventArgs e)
	{
		CustomerJobs();
	}

	protected void btnReport_Click(object sender, System.EventArgs e)
	{
		JobRno = 0;
		fBlank = false;

		if (rdoDay.Checked)
		{
			dtBeg = Convert.ToDateTime(txtDayDate.Text);
			dtEnd = dtBeg;
		}

		if (rdoWeek.Checked)
		{
			dtBeg = Convert.ToDateTime(txtWeekDate.Text);
			dtEnd = dtBeg.AddDays(6);
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
		dtEnd = new DateTime(dtEnd.Year, dtEnd.Month, dtEnd.Day, 23, 59, 59);

		if (rdoBlank.Checked)
		{
			fBlank = true;
			NumCopies = Str.Num(ddlCopies.SelectedValue);
		}

		if (rdoCustomerDate.Checked)
		{
			JobRno = Str.Num(ddlCustomerDate.Items[ddlCustomerDate.SelectedIndex].Value);
		}

		fFinalPrint = true;
		fReport = true;
		fSingleJob = false;
		fByCreatedDt = rdoEnteredDate.Checked;
	}

	protected void Report()
	{
		fNewPage = false;
		fSummaryOnly = (chkSummaryOnly.Checked && !fBlank && JobRno == 0);

		Response.Write("\t</head>\n\t<body id='JobSheet' class='" + (fBlank ? "BlankBody" : "JobBody") + "'>\n");

		if (fSummaryOnly)
		{
			Summary();
		}

		if (fBlank)
		{
			for (int i = 0; i < NumCopies; i++)
			{
				BlankJobSheets();
			}
		}
		else
		{
			PrintJobSheets();
		}
	}

	protected void Summary()
	{
		string SubTitle = (fByCreatedDt ? "Entered Between<br />" : "") + Fmt.Dt(dtBeg) + " - " + Fmt.Dt(dtEnd);

		string Html =
			WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td width='30%' valign='bottom'>\n" +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(2) + "<td align='center'>\n" +
			WebPage.Tabs(3) + WebPage.Div("Job Sheets", "center", "RptTitle") +
			WebPage.Tabs(3) + WebPage.Div(SubTitle, "center", "RptSubTitle") +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(2) + "<td width='30%' valign='bottom' align='right'>" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.TableEnd() +
			WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n";

		//			if (fByCreatedDt)
		//			{
		//				Html +=
		//					WebPage.Tabs(2) + "<td></td>\n" + 
		//					WebPage.Tabs(2) + WebPage.Space(10, 1) +  
		//					WebPage.Tabs(2) + "<td></td>\n" + 
		//					WebPage.Tabs(2) + WebPage.Space(10, 1);
		//			}
		//
		Html +=
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 30) +
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
			WebPage.Tabs(1) + "<tr><td colspan='21' align='right' id='Totals'></td></tr>\n" +
			//				WebPage.Tabs(1) + "<tr><td colspan='" + (fByCreatedDt ? "25" : "21") + "' align='right' id='Totals'></td></tr>\n" +
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 10) +
			WebPage.Tabs(1) + "<tr>\n";

		//			if (fByCreatedDt)
		//			{
		//				Html +=
		//					WebPage.Tabs(2) + "<td align='center'><b>Entered</b></td>\n" + 
		//					WebPage.Tabs(2) + "<td></td>\n" + 
		//					WebPage.Tabs(2) + "<td align='center'><b>Time</b></td>\n" + 
		//					WebPage.Tabs(2) + "<td></td>\n";
		//			}

		Html +=
			WebPage.Tabs(2) + "<td align='center'><b>Job Date</b></td>\n" +
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
			WebPage.Tabs(1) + "</tr>\n";
		Response.Write(Html);
	}

	protected void PrintJobSheets()
	{
		DateTime PrevDate = DateTime.MinValue;
		int cDays = 0;
		int cJobs = 0;
		int cServ = 0;

		string SqlWhere = "ProposalFlg <> 1";
		SqlWhere = DB.And(SqlWhere, (fSingleJob ? "" : "CancelledDtTm Is Null"));
		SqlWhere = DB.And(SqlWhere, (JobRno > 0 ? "JobRno = " + JobRno : (fByCreatedDt ? "CreatedDtTm" : "JobDate") + " Between " + DB.PutDtTm(dtBeg) + " And " + DB.PutDtTm(dtEnd)));
		SqlWhere = DB.And(SqlWhere, (JobRno == 0 && chkNotPrintedOnly.Checked ? "PrintedDtTm Is Null" : ""));

		string Sql =
            "Select j.JobRno, j.NumMenServing, j.NumWomenServing, j.NumChildServing, j.JobDate, " +
            "j.DepartureTime, j.MealTime, Coalesce(cu.Name, c.Name) as Customer, j.EventType, j.ServiceType " + 
            "From mcJobs j " +
            "Inner Join Contacts c On j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
			"Where " + SqlWhere +
			" Order By JobDate, DepartureTime, Customer";
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				JobRno = DB.Int32(dr["JobRno"]);

				if (fSummaryOnly)
				{
					int cMen = DB.Int32(dr["NumMenServing"]);
					int cWomen = DB.Int32(dr["NumWomenServing"]);
					int cChild = DB.Int32(dr["NumChildServing"]);
					int cServings = cMen + cWomen + cChild;
					DateTime JobDate = DB.DtTm(dr["JobDate"]);

					DefaultQty = cServings;

					if (JobDate != PrevDate)
					{
						cDays++;
						PrevDate = JobDate;
					}

					cJobs++;
					cServ += cServings;

					string Html =
						WebPage.Tabs(1) + "<tr>\n";

					//						if (fByCreatedDt)
					//						{
					//							Html +=
					//								WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dt(DB.DtTm(dr["CreatedDtTm"])) + "</td>\n" + 
					//								WebPage.Tabs(2) + "<td></td>\n" + 
					//								WebPage.Tabs(2) + "<td align='right'>" + Fmt.Tm12Hr(DB.DtTm(dr["CreatedDtTm"])) + "</td>\n" + 
					//								WebPage.Tabs(2) + "<td></td>\n";
					//						}

					Html +=
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
						WebPage.Tabs(1) + "</tr>\n";
					Response.Write(Html);
				}
				else
				{
					PrintJobSheet(JobRno);
				}
			}

			if (dt.Rows.Count == 0)
			{
				Response.Write("<br /><br /><div class='JobSubTitle' align='center'>No Jobs Found to Print</div>\n");
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		if (fSummaryOnly)
		{
			Response.Write(WebPage.Tabs(0) + WebPage.TableEnd());

			string sTotals = "<b>" + cDays + "</b> Dates,&nbsp;&nbsp;<b>" + cJobs + "</b> Jobs,&nbsp;&nbsp;<b>" + Fmt.Num(cServ) + "</b> Servings";
			Response.Write("<script language='javascript'>oSetStr('Totals', '" + sTotals + "');</script>\n");
		}
	}

	protected void PrintJobSheet(Int32 JobRno)
	{
		string Sql =
            "Select j.Status, j.JobDate, j.ConfirmDeadlineDate, j.PrintedDtTm, j.InvoicedDtTm, j.CancelledDtTm, " +
            "j.NumMenServing, j.NumWomenServing, j.NumChildServing, j.Demographics, j.ResponsibleParty, j.EventType, " +
            "j.EventTypeDetails, j.CeremonyFlg, j.CeremonyTime, j.CeremonyLocation, j.ServiceType, j.DisposableFlg, " +
            "j.BookedBy, j.ConfirmedBy, j.PmtMethod, j.Location, j.PrintDirectionsFlg, j.LocationDirections, " +
            "j.Vehicle, j.Carts, j.GuestArrivalTime, j.LoadTime, j.DepartureTime, j.ArrivalTime, j.MealTime, j.EndTime, " +
            "j.PricePerPerson, j.DeliveryAmt, j.ServiceAmt, j.JobType, j.PmtTerms, j.JobNotes, " +
            "j.NumBuffets, j.BuffetSpace, j.Location, j.MenuType, " +
            "Coalesce(cu.Name, c.Name) as Customer, cu.TaxExemptFlg, c.Name, c.Phone, c.Cell, c.Fax, c.Email " +
            "From mcJobs j " +
            "Left Join Contacts c on j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Where JobRno = " + 
            JobRno;

		try
		{
			DataTable dt = db.DataTable(Sql);
			DataRow dr = dt.Rows[0];

			string sPrinted = Fmt.Dt(DateTime.Now);

			if (fNewPage)
			{
				Response.Write(
					"<div style='page-break-before: always;'><img width='1' height='1' src='Images/Space.gif' alt='' /></div>");
			}

			Response.Write(
				//WebPage.Table("align='center' height='100%'" + (fNewPage ? " style='page-break-before: always;'" : "")) +
				WebPage.Table("width='680'") +
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td colspan='3'>\n" +
				WebPage.Tabs(3) + WebPage.Table("align='center' width='100%'") +
				WebPage.Tabs(4) + "<tr>\n" +
				WebPage.Tabs(5) + "<td width='36%'>Printed: " + Fmt.DtTm12HrSec(DateTime.Now) + "</td>\n" +
				WebPage.Tabs(5) + "<td class='JobTitle' align='center'>Job # " + JobRno + "</td>\n" +
				WebPage.Tabs(5) + "<td align='right' width='36%'>Status: " + DB.Str(dr["Status"]) + "</td>\n" +
				WebPage.Tabs(4) + "</tr>\n" +
				WebPage.Tabs(3) + WebPage.TableEnd() +
				WebPage.Tabs(2) + "</td>\n" +
				WebPage.Tabs(1) + "</tr>\n" +
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + WebPage.Space(1, 10) +
				WebPage.Tabs(1) + "</tr>\n" +
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td style='PADDING-RIGHT: 5px; WIDTH: 300px' valign='top'>\n" +
				WebPage.Tabs(3) + WebPage.Table("align='center' width='100%'") +
				WebPage.Tabs(4) + "<tr>\n" +
				WebPage.Tabs(5) + "<td valign='top'>\n"
			);

			JobInfo(6, ref dr);

			Response.Write(
				WebPage.Tabs(5) + "</td>\n" +
				WebPage.Tabs(4) + "</tr>\n" +
				WebPage.Tabs(4) + "<tr>\n" +
				WebPage.Tabs(5) + "<td valign='top'>\n"
			);

            DateTime JobDate = DB.DtTm(dr["JobDate"]);
            string[] WeekDays = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };

            Response.Write(
                WebPage.Tabs(5) + "</td>\n" +
                WebPage.Tabs(4) + "</tr>\n" +
                WebPage.Tabs(3) + WebPage.TableEnd() +
                WebPage.Tabs(2) + "</td>\n" +
                WebPage.Tabs(2) + "<td style='BORDER-LEFT: black 2px solid' width='1'>&nbsp;</td>\n" +
                WebPage.Tabs(2) + "<td style='PADDING-LEFT: 5px; WIDTH: 300px' valign='top' align='center'>\n" +
                WebPage.Tabs(3) + WebPage.Table("align='center' width='100%'") +
                WebPage.Tabs(4) + "<tr>\n" +
                WebPage.Tabs(5) + "<td valign='top'>\n" +

                WebPage.Tabs(6) + "<div style='text-align: right;'>\n" +
                WebPage.Tabs(6) + WebPage.Table("class='jsJobInfo' style='text-align: right; display: inline-block;'") +
                WebPage.Tabs(7) + "<tr>\n" +
                WebPage.Tabs(8) + WebPage.Space(1, 1, "CustTitle") +
                WebPage.Tabs(8) + WebPage.Space(1, 1, "CustSpace") +
                WebPage.Tabs(8) + WebPage.Space(180, 1) +
                WebPage.Tabs(7) + "</tr>\n" +
                JobBlankField(8, "Job Date", "RptJobDate", Fmt.Dt(JobDate) + " " + WeekDays[(int)JobDate.DayOfWeek], "JobSheetDate") +
                JobBlankField(8, "Confirmation Deadline", Fmt.Dt(Misc.ConfirmationDeadline(JobDate)), "left") +
                WebPage.Tabs(6) + WebPage.TableEnd() +
                WebPage.Tabs(6) + "</div>\n"
			);

            //Response.Write(
            //    WebPage.Tabs(6) + WebPage.Table("class='PaymentType'") +
            //    WebPage.Tabs(7) + "<tr><td align='right'>Job Type</td><td></td><td>" + DB.Str(dr["JobType"]) + "</td></tr>\n" +
            //    WebPage.Tabs(7) + "<tr><td align='right'>Pmt Type</td><td></td><td><img src='Images/Box.gif' alt='' />Credit Card <img src='Images/Box.gif' alt='' />Check <img src='Images/Box.gif' alt='' />Cash <img src='Images/Box.gif' alt='' />Paid</td></tr>\n" +
            //    WebPage.Tabs(6) + WebPage.TableEnd());

			Services(6);

            Sql = string.Format("Select Description, Caterer, Other From JobTables Where JobRno = {0} Order By Seq", JobRno);
            DataTable dtTables = db.DataTable(Sql);
            Sql = "Select Description, Caterer, Other From JobTables Where JobRno = 0 Order By Seq";
            DataTable dtBlankTables = db.DataTable(Sql);

            Sql = string.Format("Select Description, Caterer, Color, Ordered, Other From JobLinens Where JobRno = {0} Order By Seq", JobRno);
            DataTable dtLinens = db.DataTable(Sql);
            Sql = "Select Description, Caterer, Color, Ordered, Other From JobLinens Where JobRno = 0 Order By Seq";
            DataTable dtBlankLinens = db.DataTable(Sql);

            Response.Write(
				WebPage.Tabs(5) + "</td>\n" +
				WebPage.Tabs(4) + "</tr>\n" +
                Tables(4, dtTables.Rows.Count > 0 ? dtTables : dtBlankTables) +
                Linens(4, dtLinens.Rows.Count > 0 ? dtLinens : dtBlankLinens) +
				WebPage.Tabs(4) + "<tr>\n" +
				WebPage.Tabs(5) + "<td valign='top'>\n"
			);

            Crew(6);

			Response.Write(
				WebPage.Tabs(5) + "</td>\n" +
				WebPage.Tabs(4) + "</tr>\n" +
                WebPage.Tabs(1) + "<tr>\n" +
                WebPage.Tabs(2) + "<td style='text-align: center; padding-top: 5px;'>\n" +
                WebPage.Tabs(3) + "<img src='Images/Box.gif' alt='' class='CheckBox'/> 2 minute Job Lead meeting\n" +
                WebPage.Tabs(2) + "</td>\n" +
                WebPage.Tabs(1) + "</tr>\n" +
                WebPage.Tabs(3) + WebPage.TableEnd() +
				WebPage.Tabs(2) + "</td>\n" +
				WebPage.Tabs(1) + "</tr>\n" +
				WebPage.TableEnd() + 
				"<div style='page-break-before: always;'><img width='1' height='1' src='Images/Space.gif' alt='' /></div>" + 
				WebPage.Table("width='680'") +
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td style='PADDING-RIGHT: 5px; WIDTH: 300px' valign='top'>\n" +
				WebPage.Tabs(3) + WebPage.Table("align='center' width='100%' style='line-height: 1.3em;'") +
				WebPage.Tabs(4) + "<tr>\n" +
				WebPage.Tabs(5) + "<td valign='top'>\n"
			);

            Response.Write(
                WebPage.Tabs(6) + WebPage.Table("class='jsJobInfo'") +
                WebPage.Tabs(7) + "<tr>\n" +
                WebPage.Tabs(8) + WebPage.Space(1, 1, "CustTitle") +
                WebPage.Tabs(8) + WebPage.Space(1, 1, "CustSpace") +
                WebPage.Tabs(8) + WebPage.Space(1, 1, "CustValue") +
                WebPage.Tabs(7) + "</tr>\n" +
                JobBlankField(7, "Customer", "RptJobDate", DB.Str(dr["Customer"]), "JobMainItem") +
                WebPage.Tabs(6) + WebPage.TableEnd() +
                WebPage.Tabs(5) + "</td>\n" +
                WebPage.Tabs(4) + "</tr>\n" +
                WebPage.Tabs(4) + "<tr>\n" +
                WebPage.Tabs(5) + "<td>\n"
            );

            Food(6);

            Response.Write(
                WebPage.Tabs(5) + "</td>\n" +
                WebPage.Tabs(4) + "</tr>\n" +
                WebPage.Tabs(3) + WebPage.TableEnd() +
                WebPage.Tabs(2) + "</td>\n" +
                WebPage.Tabs(2) + "<td style='BORDER-LEFT: black 2px solid' width='1'>&nbsp;</td>\n" +
                WebPage.Tabs(2) + "<td style='PADDING-LEFT: 5px; WIDTH: 300px' valign='top' align='center'>\n" +
                WebPage.Tabs(3) + WebPage.Table("align='center' width='100%'") +
                WebPage.Tabs(4) + "<tr>\n" +
                WebPage.Tabs(5) + "<td valign='top'>\n" +

                WebPage.Tabs(6) + "<div style='text-align: left;'>\n" +
                WebPage.Tabs(6) + WebPage.Table("class='jsJobInfo' style='text-align: right; display: inline-block;'") +
                WebPage.Tabs(7) + "<tr>\n" +
                WebPage.Tabs(8) + WebPage.Space(1, 1, "CustTitle") +
                WebPage.Tabs(8) + WebPage.Space(1, 1, "CustSpace") +
                WebPage.Tabs(8) + WebPage.Space(200, 1) +
                WebPage.Tabs(7) + "</tr>\n" +
                JobBlankField(8, "Job #", "RptJobDate", JobRno + " " + Fmt.Dt(DB.DtTm(dr["JobDate"])) + " " + WeekDays[(int)JobDate.DayOfWeek], "JobSheetDate") +
                WebPage.Tabs(6) + WebPage.TableEnd() +
                WebPage.Tabs(6) + "</div>\n"
            );

            Dishes(6);

            Supplies(6);

			Response.Write(
				WebPage.Tabs(5) + "</td>\n" +
				WebPage.Tabs(4) + "</tr>\n" +
				WebPage.Tabs(3) + WebPage.TableEnd() +
				WebPage.Tabs(2) + "</td>\n" +
				WebPage.Tabs(1) + "</tr>\n" +
				WebPage.TableEnd()
			);

			if (fFinalPrint &&
				dr["PrintedDtTm"] == DBNull.Value)
			{
				DateTime Tm = DateTime.Now;
				string Status = "Printed";

				if (dr["InvoicedDtTm"] != DBNull.Value) { Status = "Invoiced"; }
				if (dr["CancelledDtTm"] != DBNull.Value) { Status = "Cancelled"; }

				Sql =
					"Update mcJobs Set " +
					"Status = " + DB.PutStr(Status) + ", " +
					"PrintedDtTm = " + DB.PutDtTm(Tm) + ", " +
					"PrintedUser = " + DB.PutStr(g.User) + ", " +
					"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
					"UpdatedUser = " + DB.PutStr(g.User) + " " +
					"Where JobRno = " + JobRno;
				db.Exec(Sql);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		fNewPage = true;
	}

	protected void JobInfo(int cTabs, ref DataRow dr)
	{
		Int32 cMen = DB.Int32(dr["NumMenServing"]);
		Int32 cWomen = DB.Int32(dr["NumWomenServing"]);
		Int32 cChild = DB.Int32(dr["NumChildServing"]);
        string sDemographics = DB.Str(dr["Demographics"]);

		DefaultQty = cMen + cWomen + cChild;

		string sPhone = DB.Str(dr["Phone"]);
		string sCell = DB.Str(dr["Cell"]);
		string sFax = DB.Str(dr["Fax"]);
		string sEmail = DB.Str(dr["Email"]);
        string sResponsibleParty = DB.Str(dr["ResponsibleParty"]);
        string sEventType = DB.Str(dr["EventType"]);
        string sEventTypeDetails = DB.Str(dr["EventTypeDetails"]);
        string sCeremony = (DB.Bool(dr["CeremonyFlg"]) ? "Yes" : "No");
        string sCeremonyTime = Fmt.Tm12Hr(DB.DtTm(dr["CeremonyTime"]));
        string sCeremonyLoc = DB.Str(dr["CeremonyLocation"]);
        string sServiceType = DB.Str(dr["ServiceType"]);
        string sDisposable = (DB.Bool(dr["DisposableFlg"]) ? "Yes" : "No");
        string sBookedBy = DB.Str(dr["BookedBy"]);
        string sConfirmedBy = DB.Str(dr["ConfirmedBy"]);
        string sPaymentMethod = DB.Str(dr["PmtMethod"]);
        //string sPaidDate = Fmt.Dt(DB.DtTm(dr["PmtDt1"]));
		string sLocation = DB.Str(dr["Location"]);
		string sDirections = "";
		if (DB.Bool(dr["PrintDirectionsFlg"]))
		{
			sDirections = DB.Str(dr["LocationDirections"]).Replace("\n", "<br />");
		}
		string sVehicle = DB.Str(dr["Vehicle"]);
		string sCarts = DB.Str(dr["Carts"]);
		string sJobDate = Fmt.Dt(DB.DtTm(dr["JobDate"]));
		string sGuestArrivalTime = Fmt.Tm12Hr(DB.DtTm(dr["GuestArrivalTime"]));
        string sLoadTime = Fmt.Tm12Hr(DB.DtTm(dr["LoadTime"]));
        string sDepartureTime = Fmt.Tm12Hr(DB.DtTm(dr["DepartureTime"]));
		string sArrivalTime = Fmt.Tm12Hr(DB.DtTm(dr["ArrivalTime"]));
		string sMealTime = Fmt.Tm12Hr(DB.DtTm(dr["MealTime"]));
		string sEndTime = Fmt.Tm12Hr(DB.DtTm(dr["EndTime"]));

        string sNumServing = Fmt.Num(cMen + cWomen + cChild);
		string sNumMenServing = Fmt.Num(cMen);
		string sNumWomenServing = Fmt.Num(cWomen);
		string sNumChildServing = Fmt.Num(cChild);
		//string sServing = "<span class='JobMainItem'>" + sNumServing + "</span>&nbsp;<span style='font-size: 80%;'>(M " + sNumMenServing + ", W " + sNumWomenServing + ", C " + sNumChildServing + "</span>)";
        string sServing = "<span class='JobMainItem'>" + sNumServing + (sDemographics.Length > 0 ? "</span>&nbsp;<span>(" + sDemographics + "</span>)" : string.Empty);
        string sPricePerPerson = Fmt.Dollar(DB.Dec(dr["PricePerPerson"]), false);
        string sDeliveryService = Fmt.Dollar(DB.Dec(dr["DeliveryAmt"]) + DB.Dec(dr["ServiceAmt"]), false);
		string sPmtTerms = (DB.Str(dr["JobType"]) == "Corporate" ? DB.Str(dr["PmtTerms"]) : string.Empty);
		string sJobNotes = DB.Str(dr["JobNotes"]);
        string sTaxExempt = (DB.Bool(dr["TaxExemptFlg"]) ? "Yes" : "No");
        string sMenuType = DB.Str(dr["MenuType"]);

        if (sEventTypeDetails.Length > 0)
        {
            sEventType = string.Format("{0} - {1}", sEventType, sEventTypeDetails);
        }
		if (sEndTime.Length > 0)
		{
			sMealTime = string.Format("{0} to {1}", sMealTime, sEndTime);
		}

        string Html =
            WebPage.Tabs(cTabs + 0) + WebPage.Table("width='100%' ") +
            WebPage.Tabs(cTabs + 1) + "<tr>\n" +
            WebPage.Tabs(cTabs + 2) + "<td valign='top'>\n" +
            WebPage.Tabs(cTabs + 3) + WebPage.Table("class='jsJobInfo'") +
            WebPage.Tabs(cTabs + 4) + "<tr>\n" +
            WebPage.Tabs(cTabs + 5) + WebPage.Space(1, 1, "CustTitle") +
            WebPage.Tabs(cTabs + 5) + WebPage.Space(1, 1, "CustSpace") +
            WebPage.Tabs(cTabs + 5) + WebPage.Space(1, 1, "CustValue") +
            WebPage.Tabs(cTabs + 4) + "</tr>\n" +
            WebPage.Tabs(cTabs + 4) + "<tr>\n" +
            //JobBlankField(cTabs + 4, "Job Date", "RptJobDate", sJobDate, "JobSheetDate") +
            JobBlankField(cTabs + 4, "Customer", "RptJobDate", DB.Str(dr["Customer"]), "JobMainItem") +
            JobBlankField(cTabs + 4, "Contact", DB.Str(dr["Name"]), "JobMainSubItem") +
            JobBlankField(cTabs + 4, "Office", sPhone) +
            JobBlankField(cTabs + 4, "Mobile", sCell) +
            //JobBlankField(cTabs + 4, "Fax", sFax) +
            JobBlankField(cTabs + 4, "Email", sEmail) +
            //JobBlankField(cTabs + 4, "Responsible Party", sResponsibleParty, "AttentionItem") +
            JobBlankField(cTabs + 4, "Event Type", sEventType, "JobMainSubItem") +
            JobBlankField(cTabs + 4, "Ceremony", sCeremony) +
            JobBlankField(cTabs + 4, "Time", sCeremonyTime) + //, "JobMainItem") +
            //JobBlankField(cTabs + 4, "Location", sCeremonyLoc, "JobMainSubItem") +
            JobBlankField(cTabs + 4, "Service Type", sServiceType, "JobMainSubItem") +
            //JobBlankField(cTabs + 4, "Disposable", sDisposable) +
            JobBlankField(cTabs + 4, "Booked by", sBookedBy) +
            //JobBlankField(cTabs + 4, "Confirmed by", sConfirmedBy) +
            JobBlankField(cTabs + 4, "Payment Type", sPaymentMethod) +
            //JobBlankField(cTabs + 4, "Paid Date", sPaidDate) +
            JobBlankField(cTabs + 4, "# Buffets", Fmt.Num(DB.Int32(dr["NumBuffets"]), false)) +
            JobBlankField(cTabs + 4, "Buffet Space", DB.Str(dr["BuffetSpace"])) +
            JobBlankField(cTabs + 4, "Location", sLocation, "JobMainSubItem");

		if (sDirections.Length > 0)
		{
			Html +=
				JobBlankField(cTabs + 4, "Directions", sDirections);
		}

        /*
        Html +=
			WebPage.Tabs(cTabs + 4) + "<tr>\n" +
			WebPage.Tabs(cTabs + 5) + "<td colspan='3'>\n" +
			WebPage.Tabs(cTabs + 6) + WebPage.Table() +
			WebPage.Tabs(cTabs + 7) + "<tr>\n" +
			WebPage.Tabs(cTabs + 8) + WebPage.Space(1, 1, "CustTitle") +
			WebPage.Tabs(cTabs + 8) + WebPage.Space(1, 1, "CustSpace") +
			WebPage.Tabs(cTabs + 8) + WebPage.Space(1, 1, "CustShortValue") +
			WebPage.Tabs(cTabs + 8) + WebPage.Space(1, 1, "CustSpace") +
			WebPage.Tabs(cTabs + 8) + WebPage.Space(1, 1) +
			WebPage.Tabs(cTabs + 8) + WebPage.Space(1, 1, "CustSpace") +
			WebPage.Tabs(cTabs + 8) + WebPage.Space(1, 1, "CustShortValue") +
			WebPage.Tabs(cTabs + 7) + "</tr>\n" +
			JobBlankField(cTabs + 7, "Vehicle", sVehicle, "", "Carts", sCarts, "") +
			JobBlankField(cTabs + 7, "Guest Arrival", sGuestArrivalTime, "", "", "", "") +
			JobBlankField(cTabs + 7, "Meal Time", sMealTime, "") +
			JobBlankField(cTabs + 7, "Depart Time", sDepartureTime, "JobMainItem", "Arrival", sArrivalTime, "") +
			WebPage.Tabs(cTabs + 6) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 5) + "</td>\n" +
			WebPage.Tabs(cTabs + 4) + "</tr>\n";
        */

		Html +=
            //JobBlankField(cTabs + 4, "Times:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Load", sLoadTime) +
            JobBlankField(cTabs + 4, "Times:&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;Depart", sDepartureTime) +
            //JobBlankField(cTabs + 4, Globals.g.Company.Initials + " Arrival", sArrivalTime) +
            JobBlankField(cTabs + 4, "Guest Arrival", sGuestArrivalTime) +
            JobBlankField(cTabs + 4, "Date", sJobDate) +
            JobBlankField(cTabs + 4, "Meal", sMealTime, "JobMainSubItem") +
            //JobBlankField(cTabs + 4, "End", sEndTime) +
            JobBlankField(cTabs + 4, "Servings", sServing) +
			//JobBlankField(cTabs + 4, "Price / Person", sPricePerPerson) +
            //JobBlankField(cTabs + 4, "Delivery / Service", sDeliveryService) +
			//(sPmtTerms.Length > 0 ? JobBlankField(cTabs + 4, "Payment Terms", sPmtTerms) : string.Empty) + 
            JobBlankField(cTabs + 4, "Tax Exempt", sTaxExempt) +
            JobBlankField(cTabs + 4, "Terms of Service", "<img src='Images/Box.gif' border='0' alt='' />") +
            JobBlankField(cTabs + 4, "Menu", sMenuType) +
            WebPage.Tabs(cTabs + 3) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 2) + "</td>\n" +
			WebPage.Tabs(cTabs + 1) + "</tr>\n" +
			WebPage.Tabs(cTabs + 0) + WebPage.TableEnd();
		Response.Write(Html);

		// Job notes
		Html =
			WebPage.Tabs(cTabs + 0) + WebPage.Table("width='100%'") +
			WebPage.Tabs(cTabs + 1) + "<tr>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.Space(1, 10) +
			WebPage.Tabs(cTabs + 1) + "</tr>\n" +
			WebPage.Tabs(cTabs + 1) + "<tr>\n" +
			WebPage.Tabs(cTabs + 2) + "<td class='NoteBox'>" + sJobNotes + "</td>\n" +
			WebPage.Tabs(cTabs + 1) + "</tr>\n" +
			WebPage.Tabs(cTabs + 0) + WebPage.TableEnd();
		Response.Write(Html);
	}

	protected void Food(int cTabs)
	{
		string Html = 
			WebPage.Tabs(cTabs + 0) + WebPage.Table("align='center'" + (fExtraBlanks ? " width='100%'" : "")) + 
			WebPage.Tabs(cTabs + 1) + "<tr>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.Space(1, 10) +
			WebPage.Tabs(cTabs + 2) + WebPage.Space(20, 1);
		if (fExtraBlanks)
		{
			Html += WebPage.Tabs(cTabs + 2) + WebPage.Space(250, 1);
		}
		Html +=
			WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
			WebPage.Tabs(cTabs + 1) + "</tr>\n" +
			//WebPage.Tabs(cTabs + 1) + WebPage.HorizRule(4, "100%") +
			WebPage.Tabs(cTabs + 1) + "<tr>\n" +
            WebPage.Tabs(cTabs + 2) + "<td class='BlankSec' align='center' colspan='4'>FOOD</td>\n" +
			WebPage.Tabs(cTabs + 1) + "</tr>\n";
		Response.Write(Html);

		//FoodCategories FoodCat = new FoodCategories();
		//for (
		//	string Category = FoodCat.FirstCategory(FoodCategories.Type.Hot);
		//	Category != null;
		//	Category = FoodCat.NextCategory())
		//{
		//	FoodCategory(cTabs + 1, Category);
		//}
		//for (
		//	string Category = FoodCat.FirstCategory(FoodCategories.Type.Cold);
		//	Category != null;
		//	Category = FoodCat.NextCategory())
		//{
		//	FoodCategory(cTabs + 1, Category);
		//}
		FoodCategory(cTabs + 1, "");


		//FoodCategory(cTabs + 1, "Meats");
		//FoodCategory(cTabs + 1, "Soup");
		//FoodCategory(cTabs + 1, "Pasta");
		//FoodCategory(cTabs + 1, "Sandwich");
		//FoodCategory(cTabs + 1, "Appetizers");
		//FoodCategory(cTabs + 1, "Sides");
		//FoodCategory(cTabs + 1, "Salads");
		//FoodCategory(cTabs + 1, "Bread");
		//FoodCategory(cTabs + 1, "Desserts");
		//FoodCategory(cTabs + 1, "Drink");
		//FoodCategory(cTabs + 1, "");

		Response.Write(WebPage.Tabs(cTabs + 0) + WebPage.TableEnd());
	}

	protected void FoodCategory(int cTabs, string Category)
	{
		//bool fFirst = true;
		FoodCategories FoodCat = new FoodCategories();
		//string Sql =
		//	"Select * From mcJobFood " +
		//	"Where JobRno = " + JobRno + " " +
		//	"And Category " +
		//	(Category.Length > 0 ?
		//	"= " + DB.PutStr(Category) :
		//	//"Not In ('Meats', 'Soup', 'Pasta', 'Sandwich', 'Appetizers', 'Sides', 'Salads', 'Bread', 'Desserts', 'Drink')") + " " +
		//	"Not In (" + FoodCat.SqlList(FoodCategories.Type.Hot) + ", " + FoodCat.SqlList(FoodCategories.Type.Cold) + ")") + " " +
		//	"Order By FoodSeq";
		string Sql = string.Format(
			"Select IsNull(l.Name, 'Other') As Location, * " +
			"From mcJobFood f " +
//			"Left Join McJobMenuItems i On f.Category = i.Category And f.MenuItem = i.MenuItem " +
            "Left Join mcJobMenuItems i on f.MenuItemRno = i.MenuItemRno " +
            "Left Join KitchenLocations l On i.KitchenLocRno = l.KitchenLocRno " +
			"Left Join mcJobMenuCategories c On f.Category = c.Category " +
			"Left Join Recipes r On r.RecipeRno = i.RecipeRno " +
			"Where JobRno = {0} " +
			"And f.FoodSeq Is Not Null " +
			"Order By " + (fByKitchenLocation ? "IsNull(l.SortOrder, 9999), " : string.Empty) + 
			"c.SortOrder, IsNull(i.CategorySortOrder, 9999), f.Category, Case When f.MenuItem = '' Then -1 Else f.FoodSeq End",
			JobRno);

		try
		{
			string PrevLocation = "";
			string Prev = "~~~~~";
			int cRows = 0;
			DataTable dt = db.DataTable(Sql);
			for (int iRow = 0; iRow < dt.Rows.Count; iRow++)
			{
				DataRow dr = dt.Rows[iRow];
				string Location = DB.Str(dr["Location"]);

				if (fByKitchenLocation)
				{
					if (Location != PrevLocation)
					{
						PrevLocation = Location;
						string Html =
							WebPage.Tabs(7) + "<tr>\n" +
							WebPage.Tabs(8) + "<td colspan='4'><div class='Location'>---- " + Location + " ----</div></td>\n" +
							WebPage.Tabs(7) + "</tr>\n";
						Response.Write(Html);
						Prev = "~~~~~";
					}
				}

				Category = DB.Str(dr["Category"]);
				//FoodItem(cTabs, Category, fFirst, ref dr);
				bool fFirst = (Category != Prev);
				int cCatRows = 0;
				int cSrvNotes = 0;

				if (fFirst)
				{
                    Response.Write(WebPage.Tabs(cTabs) + "<tr><td colspan='3' style='padding-top: 7px;'></td></tr>\n");
					while (iRow + cCatRows < dt.Rows.Count && 
						(DB.Str(dt.Rows[iRow + cCatRows]["Location"]) == Location || !fByKitchenLocation) &&
						DB.Str(dt.Rows[iRow + cCatRows]["Category"]) == Category)
					{
						if (DB.Str(dt.Rows[iRow + cCatRows]["ServiceNote"]).Length > 0)
						{
							cSrvNotes++;
						}
						cCatRows++;
					}
				}

				FoodItem(cTabs, Category, fFirst, cCatRows + cSrvNotes, ref dr);
				Prev = Category;
				cRows++;
			}

			if (fExtraBlanks && Category.Length > 0)
			{
				while (cRows++ < 2)
				{
					Response.Write(FoodBlankLine(cTabs, (cRows == 1 ? Category : "")));
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void FoodItem(int cTabs, string Category, bool fFirst, int cCatRows, ref DataRow dr)
	{
		string Html;
		string MenuItem     = DB.Str(dr["MenuItem"]);
		bool fGlutenFree    = DB.Bool(dr["GlutenFreeFlg"]);
        bool fVegan         = DB.Bool(dr["VeganFlg"]);
        bool fVegetarian    = DB.Bool(dr["VegetarianFlg"]);
        bool fDairyFree     = DB.Bool(dr["DairyFreeFlg"]);
        bool fNuts          = DB.Bool(dr["NutsFlg"]);
        //string QtyNote    = DB.Str(dr["QtyNote"]);
        int Qty             = DB.Int32(dr["Qty"]);
		if (Qty == 0)
		{
			Qty = DefaultQty;
		}
		string QtyNote      = Qty.ToString("##,###");
		bool fIngredSelect  = DB.Bool(dr["IngredSelFlg"]);
		string ServiceNote  = DB.Str(dr["ServiceNote"]);
		string ItemClass    = " class='" + (fExtraBlanks ? "ul " : "") + "JobItem'";
		string OtherClass   = (fExtraBlanks ? " class='ul'" : "");

		if (fGlutenFree)
		{
			MenuItem += "<span class='GlutenFree'></span>";
		}
        if (fVegan)
        {
            MenuItem += "<span class='Vegan'></span>";
        }
        if (fVegetarian)
        {
            MenuItem += "<span class='Vegetarian'></span>";
        }
        if (fDairyFree)
        {
            MenuItem += "<span class='DairyFree'></span>";
        }
        if (fNuts)
        {
            MenuItem += "<span class='Nuts'></span>";
        }

        if (MenuItem.Length > 0 ||
			QtyNote.Length > 0 ||
			ServiceNote.Length > 0)
		{
			if (QtyNote.Length > 0)
			{
				MenuItem =
					"\n" +
					WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
					WebPage.Tabs(cTabs + 3) + "<tr>\n" +
					WebPage.Tabs(cTabs + 4) + "<td valign='top' " + ItemClass + ">" + MenuItem + "</td>\n" +
					WebPage.Tabs(cTabs + 4) + "<td" + OtherClass + ">" + WebPage.NoOp(1, 1, "CustSpace") + "</td>\n" +
					WebPage.Tabs(cTabs + 4) + "<td align='right'" + OtherClass + ">" + QtyNote + "</td>\n" +
					WebPage.Tabs(cTabs + 3) + "</tr>\n" +
					WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
					WebPage.Tabs(cTabs + 1);
				ItemClass = "";
			}

			if (fIngredSelect)
			{
				string IngredSel = string.Empty;
				IngredSel = DB.Str(dr["IngredSel"]);
				string Sql = string.Format(
					"Select Coalesce(i.Name, r.Name) as Name, " +
                    "r.GlutenFreeFlg, r.VeganFlg, r.VegetarianFlg, r.DairyFreeFlg, r.NutsFlg " +
                    "From RecipeIngredXref x " +
					"Left Join Ingredients i On i.IngredRno = x.IngredRno " +
					"Left Join Recipes r on r.RecipeRno = x.SubrecipeRno " +
					"Where x.RecipeRno = {0} " +
					"And RecipeIngredRno In ({1}) " +
					"Order By x.RecipeSeq",
					DB.Int32(dr["RecipeRno"]),
					(IngredSel != string.Empty ? IngredSel : "null"));

				IngredSel = string.Empty;
				DataTable dtXref = db.DataTable(Sql);
				foreach (DataRow drXref in dtXref.Rows)
				{
                    string Extra =
                        (DB.Bool(drXref["GlutenFreeFlg"]) ? "<span class=\"GlutenFree\" />" : string.Empty) +
                        (DB.Bool(drXref["VeganFlg"])      ? "<span class=\"Vegan\" />"      : string.Empty) +
                        (DB.Bool(drXref["VegetarianFlg"]) ? "<span class=\"Vegetarian\" />" : string.Empty) +
                        (DB.Bool(drXref["DairyFreeFlg"])  ? "<span class=\"DairyFree\" />"  : string.Empty) +
                        (DB.Bool(drXref["NutsFlg"])       ? "<span class=\"Nuts\" />"       : string.Empty);
                    IngredSel += string.Format("<li>{0}{1}</li>\n", DB.Str(drXref["Name"]), Extra);
				}
				if (IngredSel.Length > 0)
				{
					MenuItem += "<ul class='IngredSel'>" + IngredSel + "</ul>\n";
				}
			}

			Html =
				//(fFirst ? WebPage.Tabs(cTabs + 0) + WebPage.SpaceTr(1, 5) : "") +
				WebPage.Tabs(cTabs + 0) + "<tr>\n";
				//WebPage.Tabs(cTabs + 1) + "<td valign='top' align='right'>" + (fFirst ? "<b>" + Category + "</b>" : "") + "</td>\n" +
			if (fFirst)
			{
				Html +=
					WebPage.Tabs(cTabs + 1) + string.Format("<td valign='top' align='right' rowspan='{0}'><b>{1}</b></td>\n", cCatRows, Category);
			}
			Html +=
				WebPage.Tabs(cTabs + 1) + "<td valign='top' align='right'>" + (MenuItem.Length > 0 ? "<img src='Images/Diamond.gif' class='JobBullet' alt='' />" : "") + "</td>\n" +
				WebPage.Tabs(cTabs + 1) + "<td valign='top' colspan='2' " + ItemClass + ">" + MenuItem + "</td>\n" +
				WebPage.Tabs(cTabs + 0) + "</tr>\n";

			if (ServiceNote.Length > 0)
			{
				Html +=
					WebPage.Tabs(cTabs + 0) + "<tr>\n" +
					WebPage.Tabs(cTabs + 1) + "<td colspan='2'></td>\n" +
					WebPage.Tabs(cTabs + 1) + "<td><i>" + ServiceNote + "</i></td>\n" +
					WebPage.Tabs(cTabs + 0) + "</tr>\n";
			}

			Response.Write(Html);
		}
	}

	protected string FoodBlankLine(int cTabs, string Category)
	{
		return
			WebPage.Tabs(cTabs + 0) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + (Category.Length == 0 ? "<td></td>\n" : "<td align='right'><b>" + Category + "</b></td>\n") +
			WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
			WebPage.Tabs(cTabs + 1) + "<td colspan='2' class='ul'>&nbsp;</td>\n" +
			WebPage.Tabs(cTabs + 0) + "</tr>\n";
	}

	protected void Services(int cTabs)
	{
		Response.Write(
			WebPage.Tabs(cTabs + 0) + "<center class='BlankSec' style='padding-top: 10px;'>SERVICES / SET UP</center>\n" +
			WebPage.Tabs(cTabs + 0) + WebPage.Table("align='center' class='Services'") +
			WebPage.Tabs(cTabs + 1) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + "</tr>\n" +
			WebPage.Tabs(cTabs + 1) + "<tr>\n" +
			WebPage.Tabs(cTabs + 2) + "<td align='right'><b>" + g.Company.Initials + "</b> <b>Cust</b></td>\n" +
			WebPage.Tabs(cTabs + 2) + "<td align='right'><b>" + g.Company.Initials + "</b> <b>Cust</b></td>\n" +
			WebPage.Tabs(cTabs + 1) + "</tr>\n"
		);

		//string Sql =
		//    "Select * From mcJobServices " +
		//    "Where JobRno = " + JobRno + " " +
		//    "Order By ServiceSeq";
		string Sql = string.Format(
			"Select d.ServiceItem, s.MCCRespFlg, s.CustomerRespFlg, s.Note, d.ServiceSeq As Seq " +
			"From mcJobServices d Left Join mcJobServices s On d.ServiceItem = s.ServiceItem And s.JobRno = {0} " +
			"Where d.JobRno = 0 " +
			"Union " +
			"Select ServiceItem, MCCRespFlg, CustomerRespFlg, Note, ServiceSeq + 1000 As Seq " +
			"From mcJobServices " +
			"Where JobRno = {0} " +
			"And ServiceItem Not In (Select ServiceItem From mcJobServices Where JobRno = 0) " +
			"Order By Seq", 
			JobRno);
		try
		{
			string Html;
			DataTable dt = db.DataTable(Sql);
			for (int i = 0; i < dt.Rows.Count / 2; i++)
			{
				Html = 
					WebPage.Tabs(cTabs + 0) + "<tr>\n" +
					Service(cTabs + 1, dt.Rows[i]) +
					Service(cTabs + 1, dt.Rows[i + dt.Rows.Count / 2]) +
					WebPage.Tabs(cTabs + 0) + "</tr>\n";
				Response.Write(Html);
			}
			if (dt.Rows.Count > dt.Rows.Count / 2 * 2)
			{
				Html =
					WebPage.Tabs(cTabs + 0) + "<tr>\n" +
					Service(cTabs + 1, dt.Rows[dt.Rows.Count - 1]) +
					BlankService(cTabs + 1) +
					WebPage.Tabs(cTabs + 0) + "</tr>\n";
				Response.Write(Html);
			}
			for (int i = 0; i < 2; i++)
			{
				Html =
					WebPage.Tabs(cTabs + 0) + "<tr>\n" +
					BlankService(cTabs + 1) +
					BlankService(cTabs + 1) +
					WebPage.Tabs(cTabs + 0) + "</tr>\n";
				Response.Write(Html);
			}
			Html =
				WebPage.Tabs(cTabs + 0) + "<tr>\n" +
				WebPage.Tabs(cTabs + 1) + "<td><img src='Images/Space.gif' alt='' width='1' height='3'></td>\n" +
				WebPage.Tabs(cTabs + 1) + "<td><img src='Images/Space.gif' alt='' width='1' height='3'></td>\n" +
				WebPage.Tabs(cTabs + 0) + "</tr>\n";
			Response.Write(Html);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		Response.Write(WebPage.Tabs(cTabs + 0) + WebPage.TableEnd());
	}

	protected string Service(int cTabs, DataRow dr)
	{
		string Service = DB.Str(dr["ServiceItem"]);
		bool MCC = DB.Bool(dr["MCCRespFlg"]);
		bool Customer = DB.Bool(dr["CustomerRespFlg"]);
		string Note = DB.Str(dr["Note"]);
		string Class = "JobItem";
		string sMCC = "<img src='Images/" + (MCC ? "BoxChecked.gif" : "Box.gif") + "' border='0' alt='' />";
		string sCustomer = "<img src='Images/" + (Customer ? "BoxChecked.gif" : "Box.gif") + "' border='0' alt='' />";
		//string NoteClass = "";

		if (Service.Length == 0)
		{
			Class = "BlankService";
			Service = "&nbsp;";
		}

		string Html =
			WebPage.Tabs(cTabs + 1) + "<td valign='top'>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
			WebPage.Tabs(cTabs + 3) + "<tr>\n" +
			WebPage.Tabs(cTabs + 4) + "<td valign='top' align='left' nowrap class='" + Class + "'>" + Service + "</td>\n" +
			WebPage.Tabs(cTabs + 4) + "<td valign='top' align='right'>" + sMCC + sCustomer + "</td>\n" +
			WebPage.Tabs(cTabs + 3) + "</tr>\n";
		if (Note.Length > 0)
		{
			Html +=
				WebPage.Tabs(cTabs + 3) + "<tr>\n" +
				WebPage.Tabs(cTabs + 4) + "<td colspan='2' align='right' class='Note'><i>" + Note + "</i></td>\n" +
				WebPage.Tabs(cTabs + 3) + "</tr>\n";
		}
		Html += 
			WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 1) + "</td>\n";

		return Html;
	}

	protected string BlankService(int cTabs)
	{
		string Box = "<img src='Images/Box.gif' border='0' alt='' />";
		string Html =
			WebPage.Tabs(cTabs + 1) + "<td>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
			WebPage.Tabs(cTabs + 3) + "<tr>\n" +
			WebPage.Tabs(cTabs + 4) + "<td class='BlankService'>&nbsp;</td>\n" +
			WebPage.Tabs(cTabs + 4) + "<td valign='top' align='center'>" + Box + Box + "</td>\n" +
			WebPage.Tabs(cTabs + 3) + "</tr>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 1) + "</td>\n";

		return Html;
	}

	protected void Linens(DataRow dr, int cTabs)
	{
		string Html = "";

		try
		{
			Html =
				WebPage.Tabs(cTabs - 2) + "</tr>\n" +
				WebPage.Tabs(cTabs - 1) + "</td>\n" +
				LinenItem(cTabs, "MCC", DB.Str(dr["MCCLinens"])) +
				LinenItem(cTabs, "Diamond", DB.Str(dr["DiamondLinens"])) + 
				LinenItem(cTabs, "Susan", DB.Str(dr["SusanLinens"])) +
				LinenItem(cTabs, "CUE", DB.Str(dr["CUELinens"])) +
				LinenItem(cTabs, "Shirts", DB.Str(dr["Shirts"])) +
				LinenItem(cTabs, "Aprons", DB.Str(dr["Aprons"]));

			if (Html.Length > 0)
			{
				Html =
					WebPage.Tabs(cTabs + 0) + "<div><img src='Images/Space.gif' alt='' class='SecBreak'></div>\n" +
					WebPage.Tabs(cTabs + 0) + "<center class='JobSubTitle'>LINENS</center>\n" +
					WebPage.Tabs(cTabs + 0) + WebPage.Table("width='100%' class='BlankLinens'") +
					//WebPage.Tabs(cTabs + 1) + "<tr>\n" +
					//WebPage.Tabs(cTabs + 2) + WebPage.Space(1, 10) +
					//WebPage.Tabs(cTabs + 1) + "</tr>\n" +
					WebPage.Tabs(cTabs + 1) + "<tr>\n" +
					//WebPage.Tabs(cTabs + 2) + "<td colspan='3'><center class='JobSubTitle'>LINENS</center></td>\n" +
					WebPage.Tabs(cTabs + 1) + "</tr>\n" +
					WebPage.Tabs(cTabs + 1) + "<tr>\n" +
					WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
					WebPage.Tabs(cTabs + 2) + WebPage.Space(5, 1) +
					WebPage.Tabs(cTabs + 2) + "<td align='right' class='BlankLinens'>\n" +
					WebPage.Tabs(cTabs + 3) + "Overlay&nbsp;&nbsp;Sheer&nbsp;&nbsp;Napkins</td>\n" +
					WebPage.Tabs(cTabs + 2) + "</td>\n" +
					WebPage.Tabs(cTabs + 1) + "</tr>\n" +
					Html +
					WebPage.Tabs(cTabs + 0) + WebPage.TableEnd() +
					WebPage.Tabs(cTabs - 1) + "</td>\n" +
					WebPage.Tabs(cTabs - 2) + "</tr>\n";
				Response.Write(Html);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}
	}

	protected string LinenItem(int cTabs, string Name, string Item)
	{
		string Html = "";
		string Class = "ul";

		if (Item.Length == 0)
		{
			Item = "&nbsp;";
			Class = "BlankLinen";
		}
		if (Item != "")
		{
			Html =
				WebPage.Tabs(cTabs + 1) + "<tr>\n" +
				WebPage.Tabs(cTabs + 2) + "<td align='right' class='JobItem'>" + Name + "</td>\n" +
				WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
				WebPage.Tabs(cTabs + 2) + "<td align='left' class='" + Class + "'>" + Item + "</td>\n" +
				WebPage.Tabs(cTabs + 1) + "</tr>\n";
		}

		return Html;
	}

	protected void Dishes(int cTabs)
	{
		Response.Write(
			WebPage.Tabs(cTabs + 0) + "<div><img src='Images/Space.gif' alt='' class='SecBreak'></div>\n" +
            WebPage.Tabs(cTabs + 0) + "<center class='BlankSec'>DISHES</center>\n" +
			WebPage.Tabs(cTabs + 0) + WebPage.Table("width='100%' class='BlankDishes'")
		);

		//string Sql =
		//    "Select * From mcJobDishes " +
		//    "Where JobRno = " + JobRno + " " +
		//    "Order By DishSeq";
		string Sql = string.Format(
			"Select z.DishItem, d.Qty, d.Note, z.DishSeq As Seq  " +
			"From mcJobDishes z Left Join mcJobDishes d On z.DishItem = d.DishItem And d.JobRno = {0} " +
			"Where z.JobRno = 0 " +
			"Union " +
			"Select DishItem, Qty, Note, DishSeq + 1000 As Seq " +
			"From mcJobDishes " +
			"Where JobRno = {0} " +
			"And DishItem Not In (Select DishItem From mcJobDishes Where JobRno = 0) " +
			"Order By z.DishSeq",
			JobRno);

		try
		{
			DataTable dt = db.DataTable(Sql);
			for (int i = 0; i < dt.Rows.Count / 2; i++)
			{
				string Html =
					WebPage.Tabs(cTabs + 1) + "<tr>\n" +
					Dish(cTabs + 2, dt.Rows[i]) +
					Dish(cTabs + 2, dt.Rows[i + dt.Rows.Count / 2]) +
					WebPage.Tabs(cTabs + 1) + "</tr>\n";
				Response.Write(Html);
			}
			if (dt.Rows.Count > dt.Rows.Count / 2 * 2)
			{
				string Html =
					WebPage.Tabs(cTabs + 1) + "<tr>\n" +
					Dish(cTabs + 2, dt.Rows[dt.Rows.Count - 1]) +
					WebPage.Tabs(cTabs + 2) + "<td>&nbsp;</td>\n" +
					WebPage.Tabs(cTabs + 1) + "</tr>\n";
				Response.Write(Html);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		Response.Write(WebPage.Tabs(cTabs + 0) + WebPage.TableEnd());
	}

	protected string Dish(int cTabs, DataRow dr)
	{
        string Dish = DishDetails(DB.Str(dr["DishItem"]));
        if (Dish.Length == 0)
            Dish = "&nbsp;";

		string Html =
            WebPage.Tabs(cTabs + 0) + "<td width='50%'>\n" +
			WebPage.Tabs(cTabs + 1) + WebPage.Table("width='100%'") +
			WebPage.Tabs(cTabs + 2) + "<tr>\n" +
			WebPage.Tabs(cTabs + 3) + "<td valign='top' align='left'>" + Dish + "</td>\n" +
			WebPage.Tabs(cTabs + 3) + "<td valign='top' align='right'>" + Fmt.Num(DB.Int32(dr["Qty"]), false) + " <i>" + DB.Str(dr["Note"]) + "</i></td>\n" +
			WebPage.Tabs(cTabs + 2) + "</tr>\n" +
			WebPage.Tabs(cTabs + 1) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 0) + "</td>\n";

		return Html;
	}

	protected void Supplies(int cTabs)
	{
		Response.Write(
			WebPage.Tabs(cTabs + 0) + "<div><img src='Images/Space.gif' alt='' class='SecBreak'></div>\n" +
            WebPage.Tabs(cTabs + 0) + "<center class='BlankSec' style='padding-top: 10px;'>SUPPLIES</center>\n");

        string Sql = string.Empty;
        try
        {
            DataTable dt;
            bool fOldStyle = false;

            // selected supplies
            Sql =
                "Select * From mcJobSupplies " +
                "Where JobRno = " + JobRno + " " +
                "Order By SupplySeq";
            dt = db.DataTable(Sql);

            if (dt.Rows.Count == 0)
            {
                // standard supplies
                Sql = string.Format(
                    "Select z.SupplyItem, s.Qty, s.Note, z.SupplySeq As Seq " +
                    "From mcJobSupplies z Left Join mcJobSupplies s On z.SupplyItem = s.SupplyItem And s.JobRno = {0} " +
                    "Where z.JobRno = 0 " +
                    "Order By Seq",
                    JobRno);
                dt = db.DataTable(Sql);
                fOldStyle = true;
            }

            int iMid = (dt.Rows.Count + 1) / 2;

            if (!fOldStyle)
            {
                Response.Write(WebPage.Tabs(cTabs + 0) + "<div><div class='Supplies'>");
                string HtmlLeft = string.Empty;
                string HtmlRight = string.Empty;
                for (int i = 0; i < iMid; i++)
                {
                    HtmlLeft += Supply1(cTabs + 2, dt.Rows[i]);
                    HtmlRight += (i + iMid < dt.Rows.Count ? Supply1(cTabs + 2, dt.Rows[i + iMid]) : "<div>&nbsp;</div>");
                }
                Response.Write(HtmlLeft + WebPage.Tabs(cTabs + 0) + "</div>");

                Response.Write(WebPage.Tabs(cTabs + 0) + "<div class='Supplies'>");
                Response.Write(HtmlRight + WebPage.Tabs(cTabs + 0) + "</div></div>");
            }
            else
            {
                Response.Write(WebPage.Tabs(cTabs + 0) + WebPage.Table("width='100%' class='BlankSupply'"));

                for (int i = 0; i < iMid; i++)
                {
                    string Html =
                        WebPage.Tabs(cTabs + 1) + "<tr>\n" +
                        Supply(cTabs + 2, dt.Rows[i], fOldStyle) +
                        (i + iMid < dt.Rows.Count ? Supply(cTabs + 2, dt.Rows[i + iMid], fOldStyle) : "<td>&nbsp;</td>") +
                        WebPage.Tabs(cTabs + 1) + "</tr>\n";
                    Response.Write(Html);
                }

                Response.Write(WebPage.Tabs(cTabs + 0) + WebPage.TableEnd());
            }
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

    protected string Supply1(int cTabs, DataRow dr)
    {
        int Qty = DB.Int32(dr["Qty"]);
        bool fSelected = (dr["Qty"] != DBNull.Value || dr["Note"] != DBNull.Value);
        string Supply = DB.Str(dr["SupplyItem"]);
        if (Supply.Length == 0)
            Supply = "&nbsp;";
        string Note = DB.Str(dr["Note"]);
        if (Note.Length > 0)
            Note = " &ndash; " + Note;

        string Html =
            WebPage.Tabs(cTabs + 0) + "<div class='Supply'>" +
            WebPage.Tabs(cTabs + 1) + "<div class='Qty'>" + Fmt.Num(Qty, false) + "</div>" +
            WebPage.Tabs(cTabs + 1) + "<span class='" + (fSelected ? "" : "NoQty") + "Item'>" + Supply + "</span>" +
            WebPage.Tabs(cTabs + 1) + "<span class='Note'>" + Note + "</span>" +
            WebPage.Tabs(cTabs + 0) + "</div>";

        return Html;
    }
    
    protected string Supply(int cTabs, DataRow dr, bool fOldStyle = true)
	{
        int Qty = DB.Int32(dr["Qty"]);
        bool fSelected = (dr["Qty"] != DBNull.Value);
        string Supply = DB.Str(dr["SupplyItem"]);
        if (Supply.Length == 0)
            Supply = "&nbsp;";
        string Note = DB.Str(dr["Note"]);

        string Html =
            WebPage.Tabs(cTabs + 0) + "<td valign='top' width='50%'>\n" +
			WebPage.Tabs(cTabs + 1) + WebPage.Table("width='100%'") +
			WebPage.Tabs(cTabs + 2) + "<tr>\n" +
            WebPage.Tabs(cTabs + 2) + "<td valign='top' align='right' style='width: 10px;'>" + Fmt.Num(Qty, false) + "</td>\n" +
            WebPage.Tabs(cTabs + 3) + "<td valign='top' align='left'" + (fSelected || fOldStyle ? "" : " class='NoQtySupply'") + ">" + Supply + "</td>\n" +
			WebPage.Tabs(cTabs + 3) + "<td valign='top' align='right'><i>" + Note + "</i></td>\n" +
			WebPage.Tabs(cTabs + 2) + "</tr>\n" +
			WebPage.Tabs(cTabs + 1) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 0) + "</td>\n";

		return Html;
	}

	protected void Crew(int cTabs)
	{
		Response.Write(
			WebPage.Tabs(cTabs + 0) + "<div><img src='Images/Space.gif' alt='' class='SecBreak'></div>\n" +
			WebPage.Tabs(cTabs + 0) + "<center class='BlankSec' style='padding-top: 10px;'>CREW</center>\n" +
            WebPage.Tabs(cTabs + 0)  + "<div style='padding: 3px; border: solid 2px #999999;'>\n" +
			WebPage.Tabs(cTabs + 0) + WebPage.Table("width='100%' class='Crew'") +
/*
			WebPage.Tabs(cTabs + 1) + "<tr>\n" +
			WebPage.Tabs(cTabs + 2) + "<td><b>Crew Member</b></td>\n" +
			WebPage.Tabs(cTabs + 2) + "<td><img src='Images/Space.gif' alt='' class='CrewSpace'></td>\n" +
			WebPage.Tabs(cTabs + 2) + "<td><b>Assignment</b></td>\n" +
			WebPage.Tabs(cTabs + 2) + "<td><img src='Images/Space.gif' alt='' class='CrewSpace'></td>\n" +
			WebPage.Tabs(cTabs + 2) + "<td><b>Crew Member</b></td>\n" +
			WebPage.Tabs(cTabs + 2) + "<td><img src='Images/Space.gif' alt='' class='CrewSpace'></td>\n" +
			WebPage.Tabs(cTabs + 2) + "<td><b>Assignment</b></td>\n" +
			WebPage.Tabs(cTabs + 1) + "</tr>\n"
*/ 
			WebPage.Tabs(cTabs + 1) + "<tr>\n" +
			WebPage.Tabs(cTabs + 2) + "<td style='width: 49%;'></td>\n" +
			WebPage.Tabs(cTabs + 2) + "<td><img src='Images/Space.gif' alt='' class='CrewSpace'></td>\n" +
            WebPage.Tabs(cTabs + 2) + "<td style='width: 49%;'></td>\n" +
            WebPage.Tabs(cTabs + 1) + "</tr>\n"
			);

		string Sql =
			"Select * From mcJobCrew " +
			"Where JobRno = " + JobRno + " " +
			"Order By CrewSeq";

		try
		{
			int cRows = 0;
            /*
			DataTable dt = db.DataTable(Sql);
			for (int i = 0; i < dt.Rows.Count / 2; i++)
			{
				string Html =
					WebPage.Tabs(cTabs + 1) + "<tr>\n" +
					Crew(cTabs + 2, dt.Rows[i]) +
					WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
					Crew(cTabs + 2, dt.Rows[i + dt.Rows.Count / 2]) +
					WebPage.Tabs(cTabs + 1) + "</tr>\n";
				Response.Write(Html);
				cRows++;
			}
			if (dt.Rows.Count > dt.Rows.Count / 2 * 2)
			{
				string Html =
					WebPage.Tabs(cTabs + 1) + "<tr>\n" +
					Crew(cTabs + 2, dt.Rows[dt.Rows.Count - 1]) +
					WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
					WebPage.Tabs(cTabs + 2) + "<td class='ul'>&nbsp;</td>\n" +
					WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
					WebPage.Tabs(cTabs + 2) + "<td class='ul'>&nbsp;</td>\n" +
					WebPage.Tabs(cTabs + 1) + "</tr>\n";
				Response.Write(Html);
				cRows++;
			}
            */
			while (cRows++ < 13)
			{
				string Html =
					WebPage.Tabs(cTabs + 1) + "<tr>\n" +
					WebPage.Tabs(cTabs + 2) + "<td class='ul'>&nbsp;</td>\n" +
					//WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
					//WebPage.Tabs(cTabs + 2) + "<td class='ul'>&nbsp;</td>\n" +
					WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
					WebPage.Tabs(cTabs + 2) + "<td class='ul'>&nbsp;</td>\n" +
					//WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
					//WebPage.Tabs(cTabs + 2) + "<td class='ul'>&nbsp;</td>\n" +
					WebPage.Tabs(cTabs + 1) + "</tr>\n";
				Response.Write(Html);
			}

			//foreach (DataRow dr in dt.Rows)
			//{
			//    string Html =
			//        WebPage.Tabs(cTabs + 1) + "<tr>\n" +
			//        WebPage.Tabs(cTabs + 2) + "<td valign='top' class='JobItem'>" + DB.Str(dr["CrewMember"]) + "</td>\n" +
			//        WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
			//        WebPage.Tabs(cTabs + 2) + "<td valign='top'>" + DB.Str(dr["CrewAssignment"]) + "</td>\n" +
			//        WebPage.Tabs(cTabs + 2) + "<td></td>\n" +
			//        WebPage.Tabs(cTabs + 2) + "<td valign='top' align='right'>" + Fmt.Tm12Hr(DB.DtTm(dr["ReportTime"])) + "</td>\n" +
			//        WebPage.Tabs(cTabs + 1) + "</tr>\n";

			//    string Note = DB.Str(dr["Note"]);
			//    if (Note.Length > 0)
			//    {
			//        Html +=
			//            WebPage.Tabs(cTabs + 1) + "<tr>\n" +
			//            WebPage.Tabs(cTabs + 2) + "<td colspan='5' align='left'><img width='20' height='1' src='Images/Space.gif' alt='' /><i>" + Note + "</i></td>\n" +
			//            WebPage.Tabs(cTabs + 1) + "</tr>\n";
			//    }
			//    Response.Write(Html);
			//}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		Response.Write(
            WebPage.Tabs(cTabs + 0) + WebPage.TableEnd() +
            WebPage.Tabs(cTabs + 0) + "</div>\n"
        );
	}

	protected string Crew(int cTabs, DataRow dr)
	{
		string Html =
			WebPage.Tabs(cTabs + 0) + "<td valign='top' class='JobItem'>" + DB.Str(dr["CrewMember"]) + "</td>\n" +
			WebPage.Tabs(cTabs + 0) + "<td></td>\n" +
			WebPage.Tabs(cTabs + 0) + "<td valign='top'>" + DB.Str(dr["CrewAssignment"]) + "</td>\n";

		return Html;
	}

	protected string JobBlankField(int cTabs, string Field, string Value)
	{
		return JobBlankField(cTabs, Field, Value, "");
	}

    protected string JobBlankField(int cTabs, string Field, string Value, string ValueClass)
    {
        return JobBlankField(cTabs, Field, string.Empty, Value, ValueClass);
    }

	protected string JobBlankField(int cTabs, string Field, string FieldClass, string Value, string ValueClass)
	{
		if (Value.Length == 0)
		{
			Value = "&nbsp;";
		}

        string fClass = (FieldClass.Length > 0 ? string.Format("class='{0}'", FieldClass) : string.Empty);
		string vClass = (fExtraBlanks ? "ul" : "");
		if (ValueClass.Length > 0)
		{
			vClass += (vClass.Length > 0 ? " " : "") + ValueClass;
		}
		if (vClass.Length > 0)
		{
			vClass = " class='" + vClass + "'";
		}

		return
			WebPage.Tabs(cTabs + 0) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + string.Format("<td align='right' valign='top'{1}>{0}</td>\n", Field, fClass) +
			WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
			WebPage.Tabs(cTabs + 1) + string.Format("<td{1}>{0}</td>\n", Value, vClass) +
			WebPage.Tabs(cTabs + 0) + "</tr>\n";
	}

	protected string JobBlankField(int cTabs, string Field1, string Value1, string ValueClass1, string Field2, string Value2, string ValueClass2)
	{
		if (Value1.Length == 0)
		{
			Value1 = "&nbsp;";
		}
		if (Value2.Length == 0)
		{
			Value2 = "&nbsp;";
		}

		string Class1 = (fExtraBlanks ? "ul" : "");
		string Class2 = Class1;
		if (ValueClass1.Length > 0)
		{
			Class1 += (Class1.Length > 0 ? " " : "") + ValueClass1;
		}
		if (ValueClass2.Length > 0)
		{
			Class2 += (Class2.Length > 0 ? " " : "") + ValueClass2;
		}
		if (Class1.Length > 0)
		{
			Class1 = " class='" + Class1 + "'";
		}
		if (Class2.Length > 0)
		{
			Class2 = " class='" + Class2 + "'";
		}

		return
			WebPage.Tabs(cTabs + 0) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + "<td align='right'>" + Field1 + "</td>\n" +
			WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
			WebPage.Tabs(cTabs + 1) + string.Format("<td{1}>{0}</td>\n", Value1, Class1) +
			WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
			WebPage.Tabs(cTabs + 1) + "<td align='right'>" + Field2 + "</td>\n" +
			WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
			WebPage.Tabs(cTabs + 1) + string.Format("<td{1}>{0}</td>\n", Value2, Class2) +
			WebPage.Tabs(cTabs + 0) + "</tr>\n";
	}

	protected void BlankJobSheets()
	{
		int i;

		if (fNewPage)
		{
			Response.Write(
					"<div style='page-break-before: always;'><img width='1' height='1' src='Images/Space.gif' alt='' /></div>");
		}

        Response.Write(
            //WebPage.Tabs(0) + "<div class='RptTitle' align='center'" + (fNewPage ? " style='page-break-before: always;'" : "") + ">Job Sheet</div>\n" +
            WebPage.Tabs(0) + "<div class='RptTitle' align='center'>Job Sheet</div>\n" +
            // page
            WebPage.Tabs(0) + WebPage.Table("align='center'") +
            WebPage.Tabs(1) + "<tr>\n" +
            WebPage.Tabs(2) + "<td style='padding-right: 5px; width: 400px;' valign='top'>\n" +
            // left side frame
            WebPage.Tabs(3) + WebPage.Table("width='100%'") +

            // Job info
            WebPage.Tabs(4) + "<tr>\n" +
            WebPage.Tabs(5) + "<td>\n" +
            WebPage.Tabs(6) + WebPage.Table("width='100%' style='padding-top: 5px; line-height: 1.85em;'") +
            WebPage.Tabs(7) + "<tr>\n" +
            WebPage.Tabs(8) + WebPage.Space(140, 1) +   // 140
            WebPage.Tabs(8) + WebPage.Space(5, 1) +
            WebPage.Tabs(8) + WebPage.Space(240, 1) +   // 200
            WebPage.Tabs(7) + "</tr>\n" +
            //BlankField(7, "<span class='RptJobDate'>Job Date</span>") +
            //BlankField(7, "Confirmation Deadline") +
            BlankField(7, "<span class='RptJobDate'>Customer</span>") +
            BlankField(7, "Contact") +
            BlankField(7, "Office") +
            BlankField(7, "Mobile") +
            //BlankField(7, "Fax") +
            BlankField(7, "Email") +
            //BlankField(7, "Responsible Party") +
            BlankField(7, "Event Type") +
            WebPage.Tabs(7) + "</tr><tr><td align='right'>Wedding Ceremony</td><td></td><td><img src='Images/Box.gif' alt='' class='CheckBox'/>Yes <img src='Images/Box.gif' alt='' class='CheckBox'/>No\n" +
            BlankField(7, "Ceremony Time") +
            //BlankField(7, "Ceremony Location") +
            BlankField(7, "Service Type") +
            //WebPage.Tabs(7) + "</tr><tr><td align='right'>Disposable</td><td></td><td><img src='Images/Box.gif' alt='' class='CheckBox'/>Yes <img src='Images/Box.gif' alt='' class='CheckBox'/>No\n" +
            BlankField(7, "Booked by") +
            //BlankField(7, "Confirmed by") +
            //WebPage.Tabs(7) + "</tr><tr><td align='right'>Payment Type</td><td></td><td><img src='Images/Box.gif' alt='' class='CheckBox'/>CC <img src='Images/Box.gif' alt='' class='CheckBox'/>Check <img src='Images/Box.gif' alt='' class='CheckBox'/>Cash\n" +
            //WebPage.Tabs(7) + "</tr><tr><td align='right'>Paid</td><td></td><td><img src='Images/Box.gif' alt='' />Yes <img src='Images/Box.gif' alt='' />No\n" +
            //BlankField(7, "Paid Date") +
            BlankField(7, "# Buffets") +
            BlankField(7, "Buffet Space") +
            BlankField(7, "") +
            BlankField(7, "Job Location") +
            BlankField(7, "") +
            //Blank2Fields(7, "Vehicle", "Carts") +
            //BlankField(7, "Times: &nbsp;&nbsp;&nbsp;&nbsp;Load") +
            BlankField(7, "Times: &nbsp;&nbsp;&nbsp;&nbsp;Depart") +
            //BlankField(7, "MCC Arrival") +
            BlankField(7, "Guest Arrival") +
            BlankField(7, "Meal") +
            BlankField(7, "End") +
            BlankField(7, "# Servings") +
            //BlankField(7, "Price / Person", "$") +
            //BlankField(7, "Delivery / Service", "$") +
            WebPage.Tabs(7) + "</tr><tr><td align='right'>Tax Exempt</td><td></td><td><img src='Images/Box.gif' alt='' class='CheckBox'/>Yes <img src='Images/Box.gif' alt='' class='CheckBox'/>No\n" +
            BlankField(7, "Menu Type") +

            WebPage.Tabs(6) + WebPage.TableEnd() +
            WebPage.Tabs(5) + "</td>\n" +
            WebPage.Tabs(4) + "</tr>\n"
        );

        Response.Write(
            // end of left side frame
            WebPage.Tabs(3) + WebPage.TableEnd() +
            WebPage.Tabs(2) + "</td>\n" +
            // middle vertical line
            WebPage.Tabs(2) + "<td style='border-left: solid 2px #999999;'><img src='Images/Space.gif' alt='' />\n" +
            WebPage.Tabs(2) + "</td>\n" +
            WebPage.Tabs(2) + "<td style='padding-left: 5px; width: 400px;' valign='top'>\n" +

            // right side frame
            WebPage.Tabs(3) + WebPage.Table("width='100%'")
        );

		// Services
		Response.Write(
            WebPage.Tabs(4) + "<tr>\n" +
            WebPage.Tabs(5) + "<td align='right'>\n" +
            WebPage.Tabs(6) + WebPage.Table("width='100%' style='padding-top: 5px; line-height: 1.85em;'") +
            WebPage.Tabs(7) + "<tr>\n" +
            WebPage.Tabs(8) + WebPage.Space(1, 1) +   // 140
            WebPage.Tabs(8) + WebPage.Space(5, 1) +
            WebPage.Tabs(8) + WebPage.Space(120, 1) +   // 200
            WebPage.Tabs(7) + "</tr>\n" +
            BlankField(7, "<span class='RptJobDate'>Job Date</span>") +
            BlankField(7, "Confirmation Deadline") +
            WebPage.Tabs(3) + WebPage.TableEnd() +
            WebPage.Tabs(5) + "</td>\n" +
            WebPage.Tabs(4) + "</tr>\n" +

            WebPage.Tabs(4) + WebPage.SpaceTr(1, 10) +
			WebPage.Tabs(4) + "<tr><td class='BlankSec' align='center'>SERVICES / SET UP</td></tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td>\n" +
			WebPage.Tabs(6) + WebPage.Table("width='100%' class='BlankSrv'") +
			WebPage.Tabs(7) + "<tr>\n" +
			WebPage.Tabs(8) + WebPage.Space(3, 1) +
			WebPage.Tabs(8) + WebPage.Space(1, 1) + 
			WebPage.Tabs(8) + WebPage.Space(5, 1) +
			WebPage.Tabs(8) + WebPage.Space(1, 1) +
			WebPage.Tabs(8) + WebPage.Space(5, 1) +
			WebPage.Tabs(8) + WebPage.Space(1, 1) +
			WebPage.Tabs(8) + "<td style='border-left: solid 1px #999999;'><img width='3' height='1' src='Images/Space.gif' alt='' /></td>\n" +
			WebPage.Tabs(8) + WebPage.Space(1, 1) +
			WebPage.Tabs(8) + WebPage.Space(5, 1) +
			WebPage.Tabs(8) + WebPage.Space(1, 1) +
			WebPage.Tabs(8) + WebPage.Space(5, 1) +
			WebPage.Tabs(8) + WebPage.Space(1, 1) +
			WebPage.Tabs(7) + "</tr>\n" +
			WebPage.Tabs(7) + "<tr>\n" +
			WebPage.Tabs(8) + "<td colspan='4' align='right' class='BlankSmTitle'>" + g.Company.Initials + "</td>\n" +
			WebPage.Tabs(8) + WebPage.Space(1, 1) +
			WebPage.Tabs(8) + "<td class='BlankSmTitle'>Cust</td>\n" +
			WebPage.Tabs(8) + "<td colspan='4' align='right' class='BlankSmTitle' style='border-left: solid 1px #999999;'>" + g.Company.Initials + "</td>\n" +
			WebPage.Tabs(8) + WebPage.Space(1, 1) +
			WebPage.Tabs(8) + "<td class='BlankSmTitle'>Cust</td>\n" +
			WebPage.Tabs(7) + "</tr>\n"
        );

		string Sql =
			"Select ServiceItem From mcJobServices Where JobRno = 0 Order By ServiceSeq";
		try
		{
			DataTable dt = db.DataTable(Sql);
			for (i = 0; i < (dt.Rows.Count + 1) / 2; i++)
			{
				string Item1 = DB.Str(dt.Rows[i]["ServiceItem"]);
				int i2 = (dt.Rows.Count + 1) / 2 + i;
				string Item2 = "&nbsp;";
                string Class2 = " class='BlankService'";
                string Class1 = "";
                if (Item1.Length == 0)
                {
                    Item1 = "&nbsp;";
                    Class1 = " class='BlankService'";
                }

				if (i2 < dt.Rows.Count)
				{
					Item2 = DB.Str(dt.Rows[i2]["ServiceItem"]);
                    if (Item2.Length > 0)
					    Class2 = "";
				}

				Response.Write(
					WebPage.Tabs(7) + "<tr>\n" +
					WebPage.Tabs(8) + "<td></td>\n" +
                    WebPage.Tabs(8) + "<td" + Class1 + ">" + Item1 + "</td>\n" +
					WebPage.Tabs(8) + "<td></td>\n" +
					WebPage.Tabs(8) + "<td align='right'><img src='Images/Box.gif' alt='' /></td>\n" +
					WebPage.Tabs(8) + "<td></td>\n" +
					WebPage.Tabs(8) + "<td><img src='Images/Box.gif' alt='' /></td>\n" +
					WebPage.Tabs(8) + "<td style='border-left: solid 1px #999999;'><img src='Images/Space.gif' alt='' /></td>\n" +
					WebPage.Tabs(8) + "<td" + Class2 + ">" + Item2 + "</td>\n" +
					WebPage.Tabs(8) + "<td></td>\n" +
					WebPage.Tabs(8) + "<td align='right'><img src='Images/Box.gif' alt='' /></td>\n" +
					WebPage.Tabs(8) + "<td></td>\n" +
					WebPage.Tabs(8) + "<td><img src='Images/Box.gif' alt='' /></td>\n" +
					WebPage.Tabs(7) + "</tr>\n"
				);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

        //for (i = 0; i < 2; i++)
        //{
        //    Response.Write(
        //        WebPage.Tabs(7) + "<tr>\n" +
        //        WebPage.Tabs(8) + "<td></td>\n" +
        //        WebPage.Tabs(8) + "<td class='ul'>&nbsp;</td>\n" +
        //        WebPage.Tabs(8) + "<td></td>\n" +
        //        WebPage.Tabs(8) + "<td align='right'><img src='Images/Box.gif' alt='' /></td>\n" +
        //        WebPage.Tabs(8) + "<td></td>\n" +
        //        WebPage.Tabs(8) + "<td><img src='Images/Box.gif' alt='' /></td>\n" +
        //        WebPage.Tabs(8) + "<td style='border-left: solid 1px #999999;'><img src='Images/Space.gif' alt='' /></td>\n" +
        //        WebPage.Tabs(8) + "<td class='ul'>&nbsp;</td>\n" +
        //        WebPage.Tabs(8) + "<td></td>\n" +
        //        WebPage.Tabs(8) + "<td align='right'><img src='Images/Box.gif' alt='' /></td>\n" +
        //        WebPage.Tabs(8) + "<td></td>\n" +
        //        WebPage.Tabs(8) + "<td><img src='Images/Box.gif' alt='' /></td>\n" +
        //        WebPage.Tabs(7) + "</tr>\n"
        //    );
        //}

		Response.Write(
			WebPage.Tabs(7) + "<tr><td colspan='6' width='50%'></td><td colspan='6' width='50%' style='border-left: solid 1px #999999;'><img height='3' src='Images/Space.gif' alt='' /></td></tr>\n" +
			WebPage.Tabs(6) + WebPage.TableEnd()

			//// Service Notes
			//WebPage.Tabs(4) + "<tr>\n" +
			//WebPage.Tabs(5) + "<td>\n" +
			//WebPage.Tabs(6) + WebPage.Table("width='100%' style='padding-top: 5px;'") +
			//BlankFieldLine(7, "Service Notes") +
			//BlankLine(7, 1) +
			//BlankLine(7, 1) +
			//WebPage.Tabs(6) + WebPage.TableEnd() +
			//WebPage.Tabs(5) + "</td>\n" +
			//WebPage.Tabs(4) + "</tr>\n" +
		);
        // end of Services

        Sql = "Select Description, Caterer, Other From JobTables Where JobRno = 0 Order By Seq";
        DataTable dtBlankTables = db.DataTable(Sql);

        Sql = "Select Description, Caterer, Color, Ordered, Other From JobLinens Where JobRno = 0 Order By Seq";
        DataTable dtBlankLinens = db.DataTable(Sql);

        Response.Write(
        // Tables
            Tables(4, dtBlankTables) +

            // Linens
            Linens(4, dtBlankLinens) +

            // Crew
            WebPage.Tabs(4) + WebPage.SpaceTr(1, 10) +
            WebPage.Tabs(4) + "<tr><td class='BlankSec' align='center'>CREW</td></tr>\n" +
            WebPage.Tabs(4) + "<tr>\n" +
            WebPage.Tabs(5) + "<td style='padding: 3px; border: solid 2px #999999;'>\n" +
            WebPage.Tabs(6) + WebPage.Table("class='BlankCrew'") +
            WebPage.Tabs(7) + "<tr>\n" +
            WebPage.Tabs(8) + "<td class='BlankSmTitle' align='center' width='49%'></td>\n" +
            WebPage.Tabs(8) + WebPage.Space(5, 1) +
            WebPage.Tabs(8) + "<td class='BlankSmTitle' align='center' width='49%'></td>\n" +
            WebPage.Tabs(7) + "</tr>\n" 
            //WebPage.Tabs(7) + "<tr>\n" +
            //WebPage.Tabs(8) + "<td class='ulBlankSmall' align='left'>Job Lead</td>\n" +
            //WebPage.Tabs(8) + "<td></td>\n" +
            //WebPage.Tabs(8) + "<td class='ul'>&nbsp;</td>\n" +
            //WebPage.Tabs(7) + "</tr>\n"
        );

		for (i = 0; i < 13; i++)
		{
			Response.Write(
				WebPage.Tabs(7) + "<tr>\n" +
				WebPage.Tabs(8) + "<td class='ul'>&nbsp;</td>\n" +
				WebPage.Tabs(8) + "<td></td>\n" +
				WebPage.Tabs(8) + "<td class='ul'>&nbsp;</td>\n" +
				WebPage.Tabs(7) + "</tr>\n"
			);
		}

        Response.Write(
            WebPage.Tabs(6) + WebPage.TableEnd() +
            WebPage.Tabs(5) + "</td>\n" +
            WebPage.Tabs(4) + "</tr>\n" +
            WebPage.Tabs(1) + "<tr>\n" +
            WebPage.Tabs(2) + "<td>\n" +
            WebPage.Tabs(8) + WebPage.Space(1, 7) +
            WebPage.Tabs(2) + "</td>\n" +
            WebPage.Tabs(1) + "</tr>\n" +
            WebPage.Tabs(1) + "<tr>\n" +
            WebPage.Tabs(2) + "<td style='text-align: center;'>\n" +
            WebPage.Tabs(3) + "<img src='Images/Box.gif' alt='' class='CheckBox'/> 2 minute Job Lead meeting\n" +
            WebPage.Tabs(2) + "</td>\n" +
            WebPage.Tabs(1) + "</tr>\n" +
            WebPage.Tabs(3) + WebPage.TableEnd() +
            WebPage.Tabs(2) + "</td>\n" +
            WebPage.Tabs(1) + "</tr>\n" +
            WebPage.Tabs(0) + WebPage.TableEnd()
        );

   		Response.Write(
            // page
            WebPage.Tabs(0) + WebPage.Table("align='center' style='page-break-before: always;'") +
            WebPage.Tabs(1) + "<tr>\n" +
            WebPage.Tabs(2) + "<td style='padding-right: 5px; width: 350px;' valign='top'>\n" +

            // left side frame
            WebPage.Tabs(3) + WebPage.Table("width='100%'") +

            WebPage.Tabs(4) + "<tr>\n" +
            WebPage.Tabs(5) + "<td align='right'>\n" +
            WebPage.Tabs(6) + WebPage.Table("width='100%' style='padding-top: 5px; line-height: 1.85em;'") +
            WebPage.Tabs(7) + "<tr>\n" +
            WebPage.Tabs(8) + WebPage.Space(80, 1) +   // 140
            WebPage.Tabs(8) + WebPage.Space(5, 1) +
            WebPage.Tabs(8) + WebPage.Space(240, 1) +   // 200
            WebPage.Tabs(7) + "</tr>\n" +
            BlankField(7, "<span class='RptJobDate'>Customer</span>") +
            WebPage.Tabs(3) + WebPage.TableEnd() +
            WebPage.Tabs(5) + "</td>\n" +
            WebPage.Tabs(4) + "</tr>\n" +

			// Food
			WebPage.Tabs(4) + WebPage.SpaceTr(1, 15) +
            //WebPage.Tabs(4) + "<tr><td class='BlankSec' align='center'>FOOD (Servings)</td></tr>\n" +
            WebPage.Tabs(4) + "<tr><td class='BlankSec' align='center'><table width='100%'><tr><td width='33%'></td><td width='33%' align='center'>FOOD</td><td width='33%' align='right'>(Servings)</td></tr></table></td></tr>\n" +
            WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td>\n" +
			WebPage.Tabs(6) + WebPage.Table("width='100%' style='padding-top: 5px; line-height: 1.4em;'") +
            BlankFieldLineWithBox(7, "Entrée") +
			BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
			BlankLine(7, 1) +
            WebPage.Tabs(7) + WebPage.SpaceTr(1, 10) +
            BlankFieldLineWithBox(7, "Sides") +
			BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
			BlankLine(7, 1) +
            WebPage.Tabs(7) + WebPage.SpaceTr(1, 10) +
            BlankFieldLineWithBox(7, "Salads") +
			BlankLine(7, 1) +
			BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            WebPage.Tabs(7) + WebPage.SpaceTr(1, 10) +
            BlankFieldLineWithBox(7, "Bread") +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            WebPage.Tabs(7) + WebPage.SpaceTr(1, 10) +
            BlankFieldLineWithBox(7, "Butter") +
            WebPage.Tabs(7) + WebPage.SpaceTr(1, 10) +
            BlankFieldLineWithBox(7, "Dessert") +
			BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            WebPage.Tabs(7) + WebPage.SpaceTr(1, 10) +
            BlankFieldLineWithBox(7, "Dessert Garnish") +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            WebPage.Tabs(7) + WebPage.SpaceTr(1, 10) +
            BlankFieldLineWithBox(7, "Drink") +
			BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            WebPage.Tabs(7) + WebPage.SpaceTr(1, 10) +
            BlankFieldLineWithBox(7, "Appetizers") +
			BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
            BlankLine(7, 1) +
			WebPage.Tabs(6) + WebPage.TableEnd() +
			WebPage.Tabs(5) + "</td>\n" +
			WebPage.Tabs(4) + "</tr>\n"
		);


		Response.Write(
			// end of left side frame
			WebPage.Tabs(3) + WebPage.TableEnd() +
			WebPage.Tabs(2) + "</td>\n" +
			// middle vertical line
			WebPage.Tabs(2) + "<td style='border-left: solid 2px #999999;'><img src='Images/Space.gif' alt='' />\n" +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(2) + "<td style='padding-left: 5px; width: 350px;' valign='top'>\n" +

			// right side frame
			WebPage.Tabs(3) + WebPage.Table("width='100%'")
        );

        Response.Write(
            WebPage.Tabs(4) + "<tr>\n" +
            WebPage.Tabs(5) + "<td align='right'>\n" +
            WebPage.Tabs(6) + WebPage.Table("width='100%' style='padding-top: 5px; line-height: 1.85em;'") +
            WebPage.Tabs(7) + "<tr>\n" +
            WebPage.Tabs(8) + WebPage.Space(1, 1) +   // 140
            WebPage.Tabs(8) + WebPage.Space(5, 1) +
            WebPage.Tabs(8) + WebPage.Space(180, 1) +   // 200
            WebPage.Tabs(7) + "</tr>\n" +
            BlankField(7, "<span class='RptJobDate'>Job Date</span>") +
            WebPage.Tabs(3) + WebPage.TableEnd() +
            WebPage.Tabs(5) + "</td>\n" +
            WebPage.Tabs(4) + "</tr>\n"
        );

        Response.Write(
            // Dishes
			WebPage.Tabs(4) + WebPage.SpaceTr(1, 10) +
			WebPage.Tabs(4) + "<tr><td class='BlankSec' align='center'>DISHES</td></tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td>\n" +
			WebPage.Tabs(6) + WebPage.Table("align='center' width='100%' class='BlankDishes'")
		);

		Sql =
			"Select DishItem From mcJobDishes Where JobRno = 0 Order By DishSeq";
		try
		{
			DataTable dt = db.DataTable(Sql);
			for (i = 0; i < dt.Rows.Count / 2; i++)
			{
				string Item1 = DishDetails(DB.Str(dt.Rows[i]["DishItem"]));
                //if (Item1 == "Paper/Glass")
                //{
                //    //Item1 = "<div align='right'>Plastic<input type='checkbox'>&nbsp; Paper<input type='checkbox'>&nbsp; Glass<input type='checkbox'></div>";
                //    //Item1 = "<div align='right'><img src='Images/Box.gif' alt='' />Plastic<input type='checkbox'>&nbsp; <img src='Images/Box.gif' alt='' />Paper&nbsp;<img src='Images/Box.gif' alt='' />Glass</div>";
                //    // don't know why there is a problem or why this solution works, but without some type of input (check box or text), all the box images don't show up on the page
                //    //Item1 = "<div align='left'><input type='text' style='width: 1px; height: 2px;'><img src='Images/Box.gif' alt='' />Plastic&nbsp;<img src='Images/Box.gif' alt='' />Paper&nbsp;<img src='Images/Box.gif' alt='' />Glass</div>";
                //    Item1 = "<div align='left' style='font-size: 7pt;'><img src='Images/Box.gif' alt='' />Plastic&nbsp;<img src='Images/Box.gif' alt='' />Paper&nbsp;<img src='Images/Box.gif' alt='' />Glass</div>";
                //}
				//int i2 = (dt.Rows.Count + 1) / 2 + i;
				//string Item2 = (i2 < dt.Rows.Count ? DishDetails(DB.Str(dt.Rows[i2]["DishItem"])) : "&nbsp;");
                string Item2 = DishDetails(DB.Str(dt.Rows[i + dt.Rows.Count / 2]["DishItem"]));

				Response.Write(
					WebPage.Tabs(7) + "<tr>\n" +
					WebPage.Tabs(8) + "<td width='50%'>" + (Item1.Length > 0 ? Item1 : "&nbsp;") + "</td>\n" +
                    WebPage.Tabs(8) + "<td width='50%'>" + (Item2.Length > 0 ? Item2 : "&nbsp;") + "</td>\n" +
					WebPage.Tabs(7) + "</tr>\n"
				);
			}
            if (dt.Rows.Count > dt.Rows.Count / 2 * 2)
            {
                string Html =
                    WebPage.Tabs(7) + "<tr>\n" +
                    WebPage.Tabs(8) + "<td>" + DishDetails(DB.Str(dt.Rows[dt.Rows.Count - 1]["DishItem"])) + "</td>\n" +
                    WebPage.Tabs(8) + "<td>&nbsp;</td>\n" +
                    WebPage.Tabs(7) + "</tr>\n";
                Response.Write(Html);
            }
        }
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		Response.Write(
			//WebPage.Tabs(7) + "<tr><td width='50%'>&nbsp;</td><td>&nbsp;</td></tr>\n" +
			//WebPage.Tabs(7) + "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>\n" +
			//WebPage.Tabs(7) + "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>\n" +
			WebPage.Tabs(6) + WebPage.TableEnd() +
			WebPage.Tabs(5) + "</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
	
			// Supplies
			WebPage.Tabs(4) + WebPage.SpaceTr(1, 10) +
            WebPage.Tabs(4) + "<tr><td class='BlankSec' align='center'>SUPPLIES</td></tr>\n" +
            WebPage.Tabs(4) + "<tr>\n" +
            WebPage.Tabs(5) + "<td>\n" +
			WebPage.Tabs(6) + WebPage.Table("align='center' width='100%' class='BlankSupply'")
		);

		Sql =
			"Select SupplyItem From mcJobSupplies Where JobRno = 0 Order By SupplySeq";
		try
		{
			DataTable dt = db.DataTable(Sql);
			for (i = 0; i < (dt.Rows.Count + 1) / 2; i++)
			{
				string Item1 = DB.Str(dt.Rows[i]["SupplyItem"]);
				int i2 = (dt.Rows.Count + 1) / 2 + i;
				string Item2 = (i2 < dt.Rows.Count ? DB.Str(dt.Rows[i2]["SupplyItem"]) : "&nbsp;");
				if (Item1.Trim().Length == 0) { Item1 = "&nbsp;"; }
				if (Item2.Trim().Length == 0) { Item2 = "&nbsp;"; }
				Response.Write(
					WebPage.Tabs(7) + "<tr>\n" +
                    WebPage.Tabs(8) + "<td width='50%'>" + Item1 + "</td>\n" +
                    WebPage.Tabs(8) + "<td width='50%'>" + Item2 + "</td>\n" +
					WebPage.Tabs(7) + "</tr>\n"
				);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

        Response.Write(
            //WebPage.Tabs(7) + "<tr><td width='50%'>&nbsp;</td><td>&nbsp;</td></tr>\n" +
            //WebPage.Tabs(7) + "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>\n" +
            ////WebPage.Tabs(7) + "<tr><td>&nbsp;</td><td>&nbsp;</td></tr>\n" +
            WebPage.Tabs(6) + WebPage.TableEnd() +
            WebPage.Tabs(5) + "</td>\n" +
            WebPage.Tabs(4) + "</tr>\n"
        );

            // Supply Notes
            //WebPage.Tabs(4) + "<tr>\n" +
            //WebPage.Tabs(5) + "<td>\n" +
            //WebPage.Tabs(6) + WebPage.Table("width='100%' style='padding-top: 5px;'") +
            //BlankFieldLine(7, "Supply Notes") +
            //BlankLine(7, 1) +
            //BlankLine(7, 1) +
            //WebPage.Tabs(6) + WebPage.TableEnd() +
            //WebPage.Tabs(5) + "</td>\n" +
            //WebPage.Tabs(4) + "</tr>\n" +

		Response.Write(
			WebPage.Tabs(3) + WebPage.TableEnd() +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
            WebPage.Tabs(0) + WebPage.TableEnd()
		);

		fNewPage = true;
	}

    protected string Tables(int cTabs, DataTable dt)
    {
        string html =
            WebPage.Tabs(cTabs) + WebPage.SpaceTr(1, 10) +
            // Tables
            WebPage.Tabs(cTabs + 0) + "<tr><td class='BlankSec' align='center'>TABLES</td></tr>\n" +
            WebPage.Tabs(cTabs + 0) + "<tr>\n" +
            WebPage.Tabs(cTabs + 1) + "<td>\n" +
            WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%' class='BlankLinens'") +
            WebPage.Tabs(cTabs + 3) + "<col width='80'></col><col span='2' width='160'></col>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><th></th><th>" + g.Company.Initials + "</th><th style='padding-bottom: 5px;'>Other _________</th></tr>\n";

        foreach (DataRow dr in dt.Rows)
        {
            html +=
                WebPage.Tabs(cTabs + 3) +
                string.Format("<tr><td align='right'>{0}</td><td align='center'>{1}</td><td align='center'>{2}</td></tr>\n",
                    DB.Str(dr["Description"]),
                    DB.Str(dr["Caterer"]),
                    DB.Str(dr["Other"]));
        }

        html += 
            WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
            WebPage.Tabs(cTabs + 1) + "</td>\n" +
            WebPage.Tabs(cTabs + 0) + "</tr>\n";

        return html;
    }

    protected string BlankTables(int cTabs)
    {
        return
            WebPage.Tabs(cTabs) + WebPage.SpaceTr(1, 10) +
            // Linens
            WebPage.Tabs(cTabs + 0) + "<tr><td class='BlankSec' align='center'>TABLES</td></tr>\n" +
            WebPage.Tabs(cTabs + 0) + "<tr>\n" +
            WebPage.Tabs(cTabs + 1) + "<td>\n" +
            WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%' class='BlankLinens'") +
            WebPage.Tabs(cTabs + 3) + "<col width='80'></col><col span='2' width='160'></col>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><th></th><th>" + g.Company.Initials + "</th><th style='padding-bottom: 5px;'>Other _________</th></tr>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>4 ft</td><td></td><td></td></tr>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>6 ft</td><td></td><td></td></tr>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>8 ft</td><td></td><td></td></tr>\n" +
            WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
            WebPage.Tabs(cTabs + 1) + "</td>\n" +
            WebPage.Tabs(cTabs + 0) + "</tr>\n";
    }

    protected string Linens(int cTabs, DataTable dt)
    {
        string html =
            WebPage.Tabs(cTabs) + WebPage.SpaceTr(1, 10) +
            // Linens
            WebPage.Tabs(cTabs + 0) + "<tr><td class='BlankSec' align='center'>LINENS</td></tr>\n" +
            WebPage.Tabs(cTabs + 0) + "<tr>\n" +
            WebPage.Tabs(cTabs + 1) + "<td>\n" +
            WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%' class='BlankLinens'") +
            WebPage.Tabs(cTabs + 3) + "<col></col><col style='width: 25%;'></col><col style='width: 15%;'></col><col style='width: 15%;'></col><col style='width: 25%;'></col>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><th></th><th>" + g.Company.Initials + "</th><th style='padding-bottom: 5px;'>Other _________</th></tr>\n";

        foreach (DataRow dr in dt.Rows)
        {
            html +=
                WebPage.Tabs(cTabs + 3) +
                string.Format("<tr><td align='right'>{0}</td><td align='center'>{1}</td><td align='center'>{2}</td><td align='center'>{3}</td><td align='center'>{4}</td></tr>\n",
                    DB.Str(dr["Description"]),
                    DB.Str(dr["Caterer"]),
                    DB.Str(dr["Color"]),
                    DB.Str(dr["Ordered"]),
                    DB.Str(dr["Other"]));
        }

        html +=
            WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
            WebPage.Tabs(cTabs + 1) + "</td>\n" +
            WebPage.Tabs(cTabs + 0) + "</tr>\n";

        return html;
    }

    protected string BlankLinens(int cTabs)
	{
		return
			WebPage.Tabs(cTabs) + WebPage.SpaceTr(1, 10) +
			// Linens
			WebPage.Tabs(cTabs + 0) + "<tr><td class='BlankSec' align='center'>LINENS</td></tr>\n" +
			WebPage.Tabs(cTabs + 0) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + "<td>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%' class='BlankLinens'") +
			WebPage.Tabs(cTabs + 3) + "<col width='80'></col><col span='2' width='160'></col>\n" +
			WebPage.Tabs(cTabs + 3) + "<tr><th></th><th>" + g.Company.Initials + "</th><th style='padding-bottom: 5px;'>Other _________</th></tr>\n" +
			WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>Base Color</td><td></td><td></td></tr>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>Alsco</td><td></td><td></td></tr>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>Alsco Floor</td><td></td><td></td></tr>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>90 x 132</td><td></td><td></td></tr>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>90 x 156</td><td></td><td></td></tr>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>Rounds</td><td></td><td></td></tr>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>Napkins</td><td></td><td></td></tr>\n" +
            WebPage.Tabs(cTabs + 3) + "<tr><td align='right'>Overlays</td><td></td><td></td></tr>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 1) + "</td>\n" +
			WebPage.Tabs(cTabs + 0) + "</tr>\n";
	}

	protected string BlankField(int cTabs, string Field)
	{
		return
			WebPage.Tabs(cTabs + 0) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + "<td align='right'>" + Field + "</td>\n" +
			WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
			WebPage.Tabs(cTabs + 1) + "<td class='ul'>&nbsp;</td>\n" +
			WebPage.Tabs(cTabs + 0) + "</tr>\n";
	}

    protected string BlankField(int cTabs, string Field, string Value)
    {
        return
            WebPage.Tabs(cTabs + 0) + "<tr>\n" +
            WebPage.Tabs(cTabs + 1) + "<td align='right'>" + Field + "</td>\n" +
            WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
            WebPage.Tabs(cTabs + 1) + "<td class='ul'>" + Value + "</td>\n" +
            WebPage.Tabs(cTabs + 0) + "</tr>\n";
    }

    protected string Blank2Fields(int cTabs, string Field1, string Field2)
	{
		return
			WebPage.Tabs(cTabs + 0) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + "<td align='right'>" + Field1 + "</td>\n" +
			WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
			WebPage.Tabs(cTabs + 1) + "<td>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
			WebPage.Tabs(cTabs + 3) + "<tr>\n" +
			WebPage.Tabs(cTabs + 4) + "<td class='ul' width='45%'>&nbsp;</td>\n" +
			WebPage.Tabs(cTabs + 4) + "<td align='right' nowrap>" + Field2 + "</td>\n" +
			WebPage.Tabs(cTabs + 4) + "<td class='ul' width='45%'>&nbsp;</td>\n" +
			WebPage.Tabs(cTabs + 3) + "</tr>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 1) + "<td>\n" +
			WebPage.Tabs(cTabs + 0) + "</tr>\n";
	}

    protected string Blank2Choices(int cTabs, string Field, string Choice1, string Choice2)
    {
        return
            WebPage.Tabs(cTabs + 0) + "<tr>\n" +
            WebPage.Tabs(cTabs + 1) + "<td align='right'>" + Field + "</td>\n" +
            WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
            WebPage.Tabs(cTabs + 1) + "<td>\n" +
            WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
            WebPage.Tabs(cTabs + 3) + "<tr>\n" +
            WebPage.Tabs(cTabs + 4) + "<td width='10%'>&nbsp;</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td align='right' nowrap>" + Choice1 + "</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td width='30%'>&nbsp;</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td align='right' nowrap>" + Choice2 + "</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td width='30%'>&nbsp;</td>\n" +
            WebPage.Tabs(cTabs + 3) + "</tr>\n" +
            WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
            WebPage.Tabs(cTabs + 1) + "<td>\n" +
            WebPage.Tabs(cTabs + 0) + "</tr>\n";
    }

    protected string Blank3Fields(int cTabs, string Field1, string Field2, string Field3)
    {
        return
            WebPage.Tabs(cTabs + 0) + "<tr>\n" +
            WebPage.Tabs(cTabs + 1) + "<td align='right'>" + Field1 + "</td>\n" +
            WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
            WebPage.Tabs(cTabs + 1) + "<td>\n" +
            WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
            WebPage.Tabs(cTabs + 3) + "<tr>\n" +
            WebPage.Tabs(cTabs + 4) + "<td class='ul' width='30%'>&nbsp;</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td align='right' nowrap>" + Field2 + "</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td class='ul' width='30%'>&nbsp;</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td align='right' nowrap>" + Field3 + "</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td class='ul' width='30%'>&nbsp;</td>\n" +
            WebPage.Tabs(cTabs + 3) + "</tr>\n" +
            WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
            WebPage.Tabs(cTabs + 1) + "<td>\n" +
            WebPage.Tabs(cTabs + 0) + "</tr>\n";
    }

    protected string Blank4Fields(int cTabs, string Field1, string Field2, string Field3, string Field4)
    {
        return
            WebPage.Tabs(cTabs + 0) + "<tr>\n" +
            WebPage.Tabs(cTabs + 1) + "<td align='right'>" + Field1 + "</td>\n" +
            WebPage.Tabs(cTabs + 1) + "<td></td>\n" +
            WebPage.Tabs(cTabs + 1) + "<td>\n" +
            WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
            WebPage.Tabs(cTabs + 3) + "<tr>\n" +
            WebPage.Tabs(cTabs + 4) + "<td align='right' nowrap>" + Field2 + "</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td class='ul' width='28%'>&nbsp;</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td align='right' nowrap>" + Field3 + "</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td class='ul' width='28%'>&nbsp;</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td align='right' nowrap>" + Field4 + "</td>\n" +
            WebPage.Tabs(cTabs + 4) + "<td class='ul' width='28%'>&nbsp;</td>\n" +
            WebPage.Tabs(cTabs + 3) + "</tr>\n" +
            WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
            WebPage.Tabs(cTabs + 1) + "<td>\n" +
            WebPage.Tabs(cTabs + 0) + "</tr>\n";
    }

    protected string BlankLine(int cTabs, int cCols)
	{
		return
			WebPage.Tabs(cTabs + 0) + "<tr><td colspan='" + cCols + "' class='ul'>&nbsp;</td></tr>\n";
	}

	protected string BlankFieldLine(int cTabs, string Field)
	{
		return
			WebPage.Tabs(cTabs + 0) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + "<td>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
			WebPage.Tabs(cTabs + 3) + "<tr><td nowrap width='10%'>" + Field + "</td><td class='ul'>&nbsp;</td></tr>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 1) + "</td>\n" +
			WebPage.Tabs(cTabs + 0) + "</tr>\n";
	}

	protected string BlankFieldLineWithBox(int cTabs, string Field)
	{
		return
			WebPage.Tabs(cTabs + 0) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + "<td>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
			WebPage.Tabs(cTabs + 3) + "<tr><td nowrap width='10%'>" + Field + "</td><td class='ul' align='right'><div class='QtyBox'></div></td></tr>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 1) + "</td>\n" +
			WebPage.Tabs(cTabs + 0) + "</tr>\n";
	}

	protected string Blank2FieldsLine(int cTabs, string Field1, string Field2)
	{
		return
			WebPage.Tabs(cTabs + 0) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + "<td>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
			WebPage.Tabs(cTabs + 3) + "<tr>\n" +
			WebPage.Tabs(cTabs + 4) + "<td nowrap width='5%'>" + Field1 + "</td><td class='ul'>&nbsp;</td>\n" +
			WebPage.Tabs(cTabs + 4) + "<td nowrap width='5%'>" + Field2 + "</td><td class='ul'>&nbsp;</td>\n" +
			WebPage.Tabs(cTabs + 3) + "</tr>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 1) + "</td>\n" +
			WebPage.Tabs(cTabs + 0) + "</tr>\n";
	}

	protected string Blank2FieldsLineWithBox(int cTabs, string Field1, string Field2)
	{
		return
			WebPage.Tabs(cTabs + 0) + "<tr>\n" +
			WebPage.Tabs(cTabs + 1) + "<td>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.Table("width='100%'") +
			WebPage.Tabs(cTabs + 3) + "<tr>\n" +
			WebPage.Tabs(cTabs + 4) + "<td nowrap width='5%'>" + Field1 + "</td><td width='45%' class='ul'>&nbsp;</td>\n" +
			WebPage.Tabs(cTabs + 4) + "<td nowrap width='5%'>" + Field2 + "</td><td width='45%' class='ul'><td class='ul' align='right'><div class='QtyBox'></div></td>\n" +
			WebPage.Tabs(cTabs + 3) + "</tr>\n" +
			WebPage.Tabs(cTabs + 2) + WebPage.TableEnd() +
			WebPage.Tabs(cTabs + 1) + "</td>\n" +
			WebPage.Tabs(cTabs + 0) + "</tr>\n";
	}

    string DishDetails(string Dish)
    {
        switch (Dish)
        {
            case "Paper/Plastic/Glass":
                Dish = "<div align='left' style='font-size: 7pt;'><img src='Images/Box.gif' alt='' />Paper&nbsp;<img src='Images/Box.gif' alt='' />Plastic&nbsp;<img src='Images/Box.gif' alt='' />Glass</div>";
                break;
            case "Glasses":
                Dish = "<div align='left' style='font-size: 7pt;'>Glass<img src='Images/Box.gif' alt='' />9oz&nbsp;<img src='Images/Box.gif' alt='' />10oz&nbsp;<img src='Images/Box.gif' alt='' />12oz</div>";
                break;
            case "Boats":
                Dish = "<div align='left' style='font-size: 7pt;'>Boats<img src='Images/Box.gif' alt='' />1/2 lb&nbsp;<img src='Images/Box.gif' alt='' />1 lb</div>";
                break;
        }

        return Dish;
    }
}