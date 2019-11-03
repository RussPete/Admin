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
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class DailyJobs : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;
	//protected Utils.Calendar calDayDate;
	//protected Utils.Calendar calWeekDate;
	//protected Utils.Calendar calMonthDate;
	//protected Utils.Calendar calBegDateRange;
	//protected Utils.Calendar calEndDateRange;
	protected bool fReport = false;
	protected DateTime dtBeg;
	protected DateTime dtEnd;
	protected Int32 JobRno;
	protected Int32[] JobRnos;
	protected bool fSummaryOnly;
	protected bool fNewPage;
	protected bool fSingleJob;
	protected int cJobServings;
	protected string RandomNoteColor = string.Empty;
	private bool fByKitchenLocation = true;
	private bool fKitchenLocationOnOwnPage = false;
	protected bool fLetterSize = true;
    protected String SlingEmail;
    protected String SlingPassword;
    //protected String SlingOrgId;
    protected Sling Sling;

    protected enum FoodType
	{
		Hot,
		Cold,
		Everything
	}
	protected FoodType eFoodType;

	private void Page_Init(object sender, System.EventArgs e)
	{
		InitCalendars();

		WebConfig Cfg = new WebConfig();
		fByKitchenLocation = Cfg.Bool("ByKitchenLocation");
	}

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        if (Request.QueryString["JobRno"] != null)
        {
            string sJobRno = Request.QueryString["JobRno"];
            JobRno = Misc.UnMangleNumber(sJobRno);
            if (JobRno == 0)
            {
                JobRno = Str.Num(sJobRno);
            }
            if (JobRno != 0)
            {
                fReport = true;
                fSingleJob = true;
            }
        }

        Pg = new WebPage("Images");
        if (JobRno == 0)
        {
            Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        }

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.Reports.Title;
			Setup();
		}
		else
		{
			fLetterSize = rdoLetter.Checked;
		}

        //WebConfig wc = new WebConfig();
        //// has to be the system parm sling user so we can see the pending assignments		
        SlingEmail = SysParm.Get(Misc.cnSlingUserEmail);
        SlingPassword = SysParm.Get(Misc.cnSlingUserPassword);
        //SlingOrgId = wc.Str("SlingOrgId");
    }

    private async void Page_PreRender(object sender, System.EventArgs e)
    {
        if (fReport)
        {
            await Report();
        }
    }

    private void InitCalendars()
	{
		//calDayDate = new Utils.Calendar("DayDate", ref txtDayDate);
		//calDayDate.ImageButton(imgDayDate);

		//calWeekDate = new Utils.Calendar("WeekDate", ref txtWeekDate);
		//calWeekDate.ImageButton(imgWeekDate);

		//calMonthDate = new Utils.Calendar("MonthDate", ref txtMonthDate);
		//calMonthDate.ImageButton(imgMonthDate);
		//calMonthDate.HideControlsList = "ddlCustomerDate";

		//calBegDateRange = new Utils.Calendar("BegDateRange", ref txtBegDateRange);
		//calBegDateRange.ImageButton(imgBegDateRange);
		//calBegDateRange.HideControlsList = "ddlCustomerDate, ddlCopies";

		//calEndDateRange = new Utils.Calendar("EndDateRange", ref txtEndDateRange);
		//calEndDateRange.ImageButton(imgEndDateRange);
		//calEndDateRange.HideControlsList = "ddlCustomerDate";
	}

	private void Setup()
	{
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
		lbJobs.Attributes.Add("onChange", "iSetChk('rdoCustomerDate', true);");

		chkSummaryOnly.Attributes.Add("onClick", "OptionSet(this.checked, 'tdSummaryOnly');");

		chkIncludeAll.Attributes.Add("onClick", "SameWindow();");
		btnReport.Attributes.Add("onClick", "NewWindow();");
	}

	private void CustomerJobs()
	{
		string Sql =
            "Select j.JobRno, Coalesce(cu.Name, c.Name) as Customer, j.JobDate " +
            "From mcJobs j " +
            "Inner Join Contacts c On j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Where j.CancelledDtTm Is Null And j.ProposalFlg <> 1 ";
		if (!chkIncludeAll.Checked)
		{
			Sql += "And j.JobDate >= " + DB.PutDtTm(DateTime.Today); // + " And j.PrintedDtTm Is Null ";
		}
		Sql += "Order By Customer, j.JobDate";

		try
		{
			DataTable dt = db.DataTable(Sql);
			ddlCustomerDate.Items.Clear();
			lbJobs.Items.Clear();
			foreach (DataRow dr in dt.Rows)
			{
				//ddlCustomerDate.Items.Add(new ListItem(DB.Str(dr["Customer"]) + " - " + Fmt.Dt(DB.DtTm(dr["JobDate"])), DB.Str(dr["JobRno"])));
				ddlCustomerDate.Items.Add(new ListItem(Fmt.Dt(DB.DtTm(dr["JobDate"])) + " - " + DB.Str(dr["Customer"]), DB.Str(dr["JobRno"])));
                ListItem li = new ListItem(Fmt.Dt(DB.DtTm(dr["JobDate"])) + " - " + DB.Str(dr["JobRno"]) + " " + DB.Str(dr["Customer"]), DB.Str(dr["JobRno"]));
                li.Attributes.Add("Date", Fmt.Dt(DB.DtTm(dr["JobDate"])));
                li.Attributes.Add("Job", DB.Str(dr["JobRno"]));
                li.Attributes.Add("Customer", DB.Str(dr["Customer"]));
                lbJobs.Items.Add(li);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		lbJobs.Rows = Math.Min(10, Math.Max(1, lbJobs.Items.Count));
	}

	protected void chkIncludeAll_CheckedChanged(object sender, System.EventArgs e)
	{
		CustomerJobs();
	}
	protected void btnReport_Click(object sender, System.EventArgs e)
	{
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

		if (rdoCustomerDate.Checked)
		{
            dtBeg = DateTime.Today;
            dtEnd = dtBeg.AddDays(28);

			if (ddlCustomerDate.Visible)
			{
				JobRno = Str.Num(ddlCustomerDate.Items[ddlCustomerDate.SelectedIndex].Value);
			}
			else
			{

				int[] SelectedIndices = lbJobs.GetSelectedIndices();
				JobRnos = new Int32[SelectedIndices.Length];
				int iJobRno = 0;
				for (int iItem = 0; iItem < SelectedIndices.Length; iItem++)
				{
					JobRnos[iJobRno++] = Str.Num(lbJobs.Items[SelectedIndices[iItem]].Value);
				}
			}
		}

		fReport = true;
		fSingleJob = false;
	}

	protected async Task<bool> Report()
	{
		fNewPage = false;
		fSummaryOnly = (chkSummaryOnly.Checked);

		Random Random = new Random();

		RandomNoteColor = Random.Next(1, 8).ToString();

		Response.Write(
            string.Format(
                "\t</head>\n" +
                "\t<body id='DailyJobs' class='RptBody'>\n" +
                "\t\t<input type='hidden' id='hfBegDate' value='{0}'><input type='hidden' id='hfEndDate' value='{1}'>\n", 
                dtBeg.ToString("MM/dd/yyyy"), 
                dtEnd.ToString("MM/dd/yyyy")));

		if (fSummaryOnly)
		{
			Summary();
		}

		string SqlByLocation =
			"Select KitchenLocRno, Name, SortOrder From KitchenLocations Where IsNull(HideFlg, 0) = 0 " +
			"Union " +
			"Select Null As KitchenLocRno, 'Other' As Name, 999999 As SortOrder " +
			"Order By SortOrder ";

		SqlByLocation = (fByKitchenLocation && fKitchenLocationOnOwnPage ? SqlByLocation : "Select 0 As KitchenLocRno, '' As Name");
		DataTable dtLocations = db.DataTable(SqlByLocation);
		foreach (DataRow dr in dtLocations.Rows)
		{
			int KitchenLocRno = DB.Int32(dr["KitchenLocRno"]);
			string KitchenLocation = DB.Str(dr["Name"]);

			string SqlWhere = "";
			//if (fSingleJob)
			{
				SqlWhere = DB.And(SqlWhere, "CancelledDtTm Is Null");
			}
			//else 
            if (JobRno > 0)
			{
				SqlWhere = DB.And(SqlWhere, "JobRno = " + JobRno);
			}
			else if (JobRnos != null)
			{
				string Rnos = string.Empty;
				foreach (Int32 Rno in JobRnos)
				{
					Rnos += ", " + Rno;
				}
				Rnos = Rnos.Substring(2);

				SqlWhere = DB.And(SqlWhere, "JobRno In (" + Rnos + ")");
			}
			else
			{
				SqlWhere = DB.And(SqlWhere, "JobDate Between " + DB.PutDtTm(dtBeg) + " And " + DB.PutDtTm(dtEnd) + " And ProposalFlg <> 1");
			}

			if (chkAM.Checked && chkPM.Checked)
			{

			}
			else if (chkAM.Checked)
			{
				SqlWhere = DB.And(SqlWhere, "Datepart(hour, Coalesce(DepartureTime, MealTime, '1/1/1 00:00:01')) < 12");
			}
			else if (chkPM.Checked)
			{
				SqlWhere = DB.And(SqlWhere, "Datepart(hour, Coalesce(DepartureTime, MealTime, '1/1/1 00:00:01')) >= 12");
			}

			string Sql =
                "Select j.JobRno, j.JobDate, Coalesce(j.DepartureTime, j.ArrivalTime, j.MealTime, j.JobDate) As DepartTime, " +
                "j.Location, j.NumMenServing, j.NumWomenServing, j.NumChildServing, j.Demographics, " +
                "j.DepartureTime, j.ArrivalTime, j.GuestArrivalTime, j.MealTime, j.EndTime, j.EventType, j.ServiceType, j.NumBuffets, " +
                "j.JobNotes, j.ProductionNotes, j.Crew, " +
                "Coalesce(cu.Name, c.Name) as Customer, " +
				"(Select Count(*) From mcJobFood f " +
				"Left Join mcJobMenuItems i On f.MenuItemRno = i.MenuItemRno " +
				"Left Join KitchenLocations l On i.KitchenLocRno = l.KitchenLocRno " +
				"Where f.JobRno = j.JobRno " +
				"And f.FoodSeq Is Not Null " +
				(fKitchenLocationOnOwnPage ? "And l.KitchenLocRno " + (KitchenLocRno > 0 ? "= " + KitchenLocRno : "Is Null") : "") +
				") As NumMenuItems " +
				"From mcJobs j " +
                "Inner Join Contacts c On j.ContactRno = c.ContactRno " +
                "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
                "Where " + SqlWhere +
				" Order By JobDate, Coalesce(DepartureTime, MealTime), Customer";
			//Response.Write(Sql);

			//for (DateTime dtCurr = dtBeg; dtCurr.CompareTo(dtEnd) <= 0; dtCurr = dtCurr.AddDays(1))
			//{
			//    if (!fSummaryOnly)
			//    {
			//        DayHeader(dtCurr);
			//    }

			//    string Sql =
			//        "Select *, Coalesce(DepartureTime, ArrivalTime, MealTime, JobDate) As DepartTime " +
			//        "From mcJobs Where JobDate = " + DB.PutDtTm(dtCurr) + " " +
			//        "And CancelledDtTm Is Null " +
			//        "Order By DepartTime";

				try
				{
					DataTable dt = db.DataTable(Sql);
					if (dt.Rows.Count > 0)
					{
						//if (!fSummaryOnly)
						//{
						//    if (chkHot.Checked)		ReportData(FoodType.Hot, ref dt);
						//    if (chkCold.Checked)	ReportData(FoodType.Cold, ref dt);
						//}
						//if (chkAllFood.Checked || fSummaryOnly)	ReportData(FoodType.Everything, ref dt);
                        if (!fSummaryOnly)// && dtBeg.Year > 1)
                        {
                            await SetupSlingShift();
                        }

						ReportData(ref dt, KitchenLocRno, KitchenLocation);
					}
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex, Sql);
					Response.Write(Err.Html());
				}
		//}
		}

        return true;
	}

	//protected void ReportData(FoodType eFoodType, ref DataTable dt)
	protected void ReportData(ref DataTable dt, int KitchenLocRno, string KitchenLocation)
	{
		//this.eFoodType = eFoodType;
		DateTime PrevDate = DateTime.MinValue;
		DateTime PrevDepartTime = DateTime.MinValue;
		int cDays = 0;
		int cJobs = 0;
		int cServ = 0;

		foreach (DataRow dr in dt.Rows)
		{
			int JobRno = DB.Int32(dr["JobRno"]);
			DateTime JobDate = DB.DtTm(dr["JobDate"]);
			DateTime DepartTime = DB.DtTm(dr["DepartTime"]);

			if (JobDate != PrevDate || DepartTime.Hour < 12 && !(PrevDepartTime.Hour < 12) || DepartTime.Hour >= 12 && !(PrevDepartTime.Hour >= 12))
			{
				if (JobDate != PrevDate)
				{
					cDays++;
					PrevDate = JobDate;
				}
				PrevDepartTime = DepartTime;

				if (!fSummaryOnly)
				{
					DayHeader(JobDate, DepartTime.Hour < 12, DepartTime.Hour >= 12, KitchenLocation);
				}
			}

			if (fSummaryOnly)
			{
				//int JobRno = DB.Int32(dr["JobRno"]);
				int cMen = DB.Int32(dr["NumMenServing"]);
				int cWomen = DB.Int32(dr["NumWomenServing"]);
				int cChild = DB.Int32(dr["NumChildServing"]);
				int cServings = cMen + cWomen + cChild;

				cJobs++;
				cServ += cServings;

				Response.Write(
					WebPage.Tabs(1) + "<tr>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dt(DB.DtTm(dr["JobDate"])) + "</td>\n" +
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
			else
			{
				//DayHeader(dtCurr);	//each job at the top of the page
				JobData(dr, KitchenLocRno);
			}
		}

		//if (dt.Rows.Count > 0)
		//{
		//    cDays++;
		//}


		if (fSummaryOnly)
		{
			Response.Write(WebPage.Tabs(0) + WebPage.TableEnd());

			string sTotals = "<b>" + cDays + "</b> Dates,&nbsp;&nbsp;<b>" + cJobs + "</b> Jobs,&nbsp;&nbsp;<b>" + Fmt.Num(cServ) + "</b> Servings";
			Response.Write("<script language='javascript'>oSetStr('Totals', '" + sTotals + "');</script>\n");
		}
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
			WebPage.Tabs(3) + WebPage.Div("Production Sheets", "center", "RptTitle") +
			WebPage.Tabs(3) + WebPage.Div(SubTitle, "center", "RptSubTitle") +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(2) + "<td width='30%' valign='bottom' align='right'>" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.TableEnd() +
			WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
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
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 10) +
			WebPage.Tabs(1) + "<tr>\n" +
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

	private void DayHeader(DateTime dtRpt, bool fAM, bool fPM, string KitchenLocation)
	{
		int cJobs = 0;
		int cServings = 0;

		string Sql =
			"Select Count(*) As Cnt, " +
			"(IsNull(Sum(NumMenServing), 0) + IsNull(Sum(NumWomenServing), 0) + IsNull(Sum(NumChildServing), 0)) As NumServings " +
			"From mcJobs Where JobDate = " + DB.PutDtTm(dtRpt) + " " +
			(fAM ? "And Datepart(hour, Coalesce(DepartureTime, MealTime, '1/1/1 00:00:01')) < 12 " : string.Empty) +
			(fPM ? "And Datepart(hour, Coalesce(DepartureTime, MealTime, '1/1/1 00:00:01')) >= 12 " : string.Empty) +
			"And CancelledDtTm Is Null";

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

		if (cJobs > 0)
		{
			if (fNewPage)
			{
				Response.Write(
						"<div style='page-break-before: always;'><img width='1' height='1' src='Images/Space.gif' alt='' /></div>");
			}

			string Desc = KitchenLocation;

			Response.Write(
				//WebPage.Table("width='760' align='center'" + (fNewPage ? " style='page-break-before: always;" : "") + "'") +
				WebPage.Table("width='500' align='center'") +
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td width='30%' valign='bottom'>\n" +
				WebPage.Tabs(3) + WebPage.Span(Desc, "RptNotice") + "\n" +
				WebPage.Tabs(2) + "</td>\n" +
				WebPage.Tabs(2) + "<td align='center'>\n" +
				WebPage.Tabs(3) + WebPage.Div("Production Sheets", "center", "RptTitle") +
				WebPage.Tabs(4) + WebPage.Table("class='RptSubTitleBorder' style='padding-left: 2px; padding-right: 2px;'") +
				WebPage.Tabs(5) + "<tr><td class='RptSubTitle'>" + (fAM ? "AM " : string.Empty) + (fPM ? "PM " : string.Empty) + Fmt.DayOfWeek(dtRpt) + ", " + Fmt.Dt(dtRpt) + "</td></tr>\n" +
				WebPage.Tabs(3) + WebPage.TableEnd() +
				WebPage.Tabs(2) + "</td>\n" +
				WebPage.Tabs(2) + "<td width='30%' valign='bottom' align='right'>" +
				WebPage.Span(Fmt.Num(cJobs), "RptNotice") + " Jobs, " +
				WebPage.Span(Fmt.Num(cServings), "RptNotice") + " Servings</td>\n" +
				WebPage.Tabs(1) + "</tr>\n" +
				WebPage.TableEnd() +
				WebPage.SpaceTable(1, 40)
				);

			fNewPage = true;
		}
	}

	private void JobData(DataRow dr, int KitchenLocRno)
	{
		int JobRno = DB.Int32(dr["JobRno"]);
		int NumMenuItems = DB.Int32(dr["NumMenuItems"]);

		//if (NumMenuItems > 0)
		{
            string Crew = FindCrew(JobRno);
			Response.Write(
				WebPage.SpaceTable(1, 10) +
                //WebPage.Table("width='500' align='center' class='RptJobBorder'") +
                //WebPage.Tabs(1) + "<tr>\n" +
                //WebPage.Tabs(2) + "<td>\n" +
                WebPage.Tabs(0) + (Crew.Length > 0 ? "<div class='Crew' data-jobrno='" + JobRno + "'>" + Crew + "</div>\n" : "") +
                WebPage.Tabs(0) + "<div class='RptJobBorder'>\n" +
                WebPage.Tabs(3) + WebPage.Table("width='100%'") +
				WebPage.Tabs(4) + "<tr>\n" +
				WebPage.Tabs(5) + WebPage.NoSpace(200, 1) +
				WebPage.Tabs(5) + WebPage.NoSpace(9, 1) +
				WebPage.Tabs(5) + WebPage.NoSpace(290, 1) +
				//WebPage.Tabs(5) + "<td></td>\n" +
				WebPage.Tabs(4) + "</tr>\n" +
				WebPage.Tabs(4) + "<tr>\n" +
				WebPage.Tabs(5) + "<td valign='top' style='padding-left: 3px;'>\n"
			);

			JobInfo(JobRno, dr);
			//Crew(JobRno);

			Response.Write(
				WebPage.Tabs(5) + "</td>\n" +
				WebPage.Tabs(5) + "<td></td>\n" +
				WebPage.Tabs(5) + "<td valign='top'>\n"
			);

			Menu(JobRno, KitchenLocRno);

			Response.Write(
				WebPage.Tabs(5) + "</td>\n" +
				WebPage.Tabs(4) + "</tr>\n" +
				WebPage.Tabs(3) + WebPage.TableEnd() +
                WebPage.Tabs(0) + "</div>\n" 
                //WebPage.Tabs(2) + "</td>\n" +
                //WebPage.Tabs(1) + "</tr>\n" +
                //WebPage.TableEnd()
            );
		}
	}

	private void JobInfo(int JobRno, DataRow dr)
	{
		int cMen = DB.Int32(dr["NumMenServing"]);
		int cWomen = DB.Int32(dr["NumWomenServing"]);
		int cChild = DB.Int32(dr["NumChildServing"]);
		int cServings = cMen + cWomen + cChild;
		cJobServings = cServings;

		string sArrivalTime = Fmt.Tm12Hr(DB.DtTm(dr["ArrivalTime"]));
		string Demographics = DB.Str(dr["Demographics"]);
		string JobNotes = DB.Str(dr["JobNotes"]);
        string ProdNotes = DB.Str(dr["ProductionNotes"]);
        string Crew = DB.Str(dr["Crew"]);

		Response.Write(
			//WebPage.Tabs(6) + WebPage.Table("width='300'") +
			WebPage.Tabs(6) + WebPage.Table() +
			WebPage.Tabs(7) + "<tr>\n" +
			WebPage.Tabs(8) + WebPage.NoSpace(60, 1) +
			WebPage.Tabs(8) + WebPage.NoSpace(8, 1) +
			WebPage.Tabs(8) + WebPage.NoSpace(130, 1) +
			WebPage.Tabs(7) + "</tr>\n" +
			Field(7, "Customer", DB.Str(dr["Customer"])) +
			Field(7, "Location", DB.Str(dr["Location"])) +
			Field(7, "Event Type", DB.Str(dr["EventType"])) +
			Field(7, WebPage.Span("Service Type", "RptNoticeTitle"), WebPage.Span(DB.Str(dr["ServiceType"]), "RptNotice")) +
			Field(7, WebPage.Span("Depart Time", "RptNoticeTitle"), WebPage.Span(Fmt.Tm12Hr(DB.DtTm(dr["DepartureTime"])), "RptNotice") + (sArrivalTime.Length > 0 ? "&nbsp;&nbsp; Arrival " + sArrivalTime : string.Empty)) +
			Field(7, "Guest Arrival", Fmt.Tm12Hr(DB.DtTm(dr["GuestArrivalTime"]))) +
			Field(7, "Meal Time", string.Format("{0} - {1}", Fmt.Tm12Hr(DB.DtTm(dr["MealTime"])), Fmt.Tm12Hr(DB.DtTm(dr["EndTime"])))) +
			//Field(7, WebPage.Span("# Servings", "RptNoticeTitle"), WebPage.Span(DB.Str(cServings), "RptNotice") + " (M " + cMen + ",  W " + cWomen + ",  C " + cChild + ")") +
			Field(7, WebPage.Span("# Servings", "RptNoticeTitle"), WebPage.Span(DB.Str(cServings), "RptNotice")) +
			(Demographics.Length > 0 ? Field(7, "Demographics", Demographics) : string.Empty) +
            Field(7, WebPage.Span("# Buffets", "RptNoticeTitle"), WebPage.Span(DB.Int32(dr["NumBuffets"]), "RptNotice")) +
            Field(7, "Job #", DB.Str(JobRno)) +
            //(JobNotes.Length > 0 ? Field(7, "Job Notes", JobNotes) : string.Empty) +
            (Crew.Length > 0 ? Field(7, "Crew", Crew) : string.Empty) + 
            (ProdNotes.Length > 0 ? Field(7, "Prod Notes", ProdNotes) : string.Empty) +
            WebPage.Tabs(6) + WebPage.TableEnd()
		);
	}

	private string Field(int nTabs, string Title, string Value)
	{
		return
			WebPage.Tabs(nTabs) + "<tr>\n" +
			WebPage.Tabs(nTabs + 1) + "<td align='right' valign='top'>" + Title + "</td>\n" +
			WebPage.Tabs(nTabs + 1) + "<td></td>\n" +
			WebPage.Tabs(nTabs + 1) + "<td>" + Value + "</td>\n" +
			WebPage.Tabs(nTabs) + "</tr>\n";
	}

	private void Crew(int JobRno)
	{
		bool fFirst = true;

		string Sql =
			"Select * " +
			"From mcJobCrew Where JobRno = " + JobRno + " " +
			"Order By ReportTime, CrewSeq";

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				if (fFirst)
				{
					fFirst = false;

					Response.Write(
						WebPage.HorizLine(6, "60%", 5) +
						WebPage.Tabs(6) + WebPage.Table("width='300'") +
						WebPage.Tabs(7) + "<tr>\n" +
						WebPage.Tabs(8) + WebPage.Space(80, 1) +
						WebPage.Tabs(8) + WebPage.Space(10, 1) +
						WebPage.Tabs(8) + WebPage.Space(120, 1) +
						WebPage.Tabs(8) + WebPage.Space(10, 1) +
						WebPage.Tabs(8) + WebPage.Space(80, 1) +
						WebPage.Tabs(7) + "</tr>\n" +
						WebPage.Tabs(7) + "<tr><td colspan='5' align='left'><b>Crew Assignments</b></td></tr>\n"
					);
				}

				string Html =
					WebPage.Tabs(7) + "<tr>\n" +
					WebPage.Tabs(8) + "<td align='right'>" + Fmt.Tm12Hr(DB.DtTm(dr["ReportTime"])) + "</td>\n" +
					WebPage.Tabs(8) + "<td></td>\n" +
					WebPage.Tabs(8) + "<td>" + DB.Str(dr["CrewMember"]) + "</td>\n" +
					WebPage.Tabs(8) + "<td></td>\n" +
					WebPage.Tabs(8) + "<td>" + DB.Str(dr["CrewAssignment"]) + "</td>\n" +
					WebPage.Tabs(7) + "</tr>\n";

				string Note = DB.Str(dr["Note"]);
				if (Note.Length > 0)
				{
					Html +=
						WebPage.Tabs(7) + "<tr><td colspan='2'></td><td colspan='3' align='center'><i>" + Note + "</i></td></tr>\n";
				}

				Response.Write(Html);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		Response.Write(
			WebPage.Tabs(6) + WebPage.TableEnd());
	}

	private void Menu(int JobRno, int KitchenLocRno)
	{
		Response.Write(
			WebPage.Tabs(6) + WebPage.Table("width='100%'" + (fByKitchenLocation && !fKitchenLocationOnOwnPage ? " class='Location'" : string.Empty)) +
			WebPage.Tabs(7) + "<tr>\n" +
			WebPage.Tabs(8) + WebPage.NoSpace(70, 1) +
			WebPage.Tabs(8) + WebPage.NoSpace(10, 1) +
			WebPage.Tabs(8) + WebPage.NoSpace(210, 1) +
			WebPage.Tabs(7) + "</tr>\n"
		);

		//FoodCategories FoodCat = new FoodCategories();

		//if (eFoodType == FoodType.Hot || eFoodType == FoodType.Everything)
		//{
		//    for (
		//        string Category = FoodCat.FirstCategory(FoodCategories.Type.Hot);
		//        Category != null;
		//        Category = FoodCat.NextCategory())
		//    {
		//        MenuCategory(JobRno, Category);
		//    }
		//}
		//if (eFoodType == FoodType.Cold || eFoodType == FoodType.Everything)
		//{
		//    for (
		//        string Category = FoodCat.FirstCategory(FoodCategories.Type.Cold);
		//        Category != null;
		//        Category = FoodCat.NextCategory())
		//    {
		//        MenuCategory(JobRno, Category);
		//    }
		//}

		//MenuCategory(JobRno, "");

		MenuCategory(JobRno, KitchenLocRno);

		Response.Write(
			WebPage.Tabs(6) + WebPage.TableEnd()
		);
	}

//	private void MenuCategory(int JobRno, string Category)
	private void MenuCategory(int JobRno, int KitchenLocRno)
	{
		//FoodCategories FoodCat = new FoodCategories();
		//string Sql =
		//    "Select * " +
		//    "From mcJobFood Where JobRno = " + JobRno + " " +
		//    "And Category " + (Category.Length > 0 ?
		//    "= " + DB.PutStr(Category) :
		//    //"Not In ('Meats', 'Soup', 'Pasta', 'Sandwich', 'Appetizers', 'Sides', 'Salads', 'Bread', 'Desserts', 'Drink')") + " " +
		//    "Not In (" + FoodCat.SqlList(FoodCategories.Type.Hot) + ", " + FoodCat.SqlList(FoodCategories.Type.Cold) + ")") + " " +
		//    "Order By Category, FoodSeq";

		string Sql = "";
		//switch (eFoodType)
		//{
		//    case FoodType.Hot:
		//    case FoodType.Cold:
		//        Sql = string.Format(
		//            "Select f.* " +
		//            "From mcJobFood f Inner Join McJobMenuItems i On f.Category = i.Category And f.MenuItem = i.MenuItem " +
		//            "Left Join mcJobMenuCategories c On i.Category = c.Category " +
		//            "Where JobRno = {0} " +
		//            "And i.{1}Flg = 1 " +
		//            "Order By c.SortOrder, f.Category, f.FoodSeq",
		//            JobRno,
		//            (eFoodType == FoodType.Hot ? "Hot" : "Cold"));
		//        break;

		//    case FoodType.Everything:
		//        Sql = string.Format(
		//            "Select f.* " +
		//            "From mcJobFood f " +
		//            "Left Join mcJobMenuCategories c On f.Category = c.Category " +
		//            "Where JobRno = {0} " +
		//            "Order By c.SortOrder, f.Category, f.FoodSeq",
		//            JobRno);
		//        break;
		//}
		Sql = string.Format(
			"Select IsNull(l.Name, 'Other') As Location, f.*, i.RecipeRno " +
			"From mcJobFood f " +
			//"Left Join McJobMenuItems i On f.Category = i.Category And f.MenuItem = i.MenuItem " +
			"Left Join McJobMenuItems i On f.MenuItemRno = i.MenuItemRno " +
			"Left Join KitchenLocations l On i.KitchenLocRno = l.KitchenLocRno " +
			"Left Join mcJobMenuCategories c On f.Category = c.Category " +
			"Where JobRno = {0} " +
			"And f.FoodSeq Is Not Null " +
			(fKitchenLocationOnOwnPage ? "And l.KitchenLocRno {1} " : "") +
			"Order By " + (fByKitchenLocation ? "IsNull(l.SortOrder, 9999), " : string.Empty) +
			"c.SortOrder, IsNull(i.CategorySortOrder, 9999), f.Category, Case When f.MenuItem = '' Then -1 Else f.FoodSeq End",
			JobRno, 
			(KitchenLocRno > 0 ? "= " + KitchenLocRno : "Is Null"));
		//Response.Write(Sql);
		string PrevLocation = "";
		string PrevCategory = "~~~~~";

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				string Location = DB.Str(dr["Location"]);
				//Category = DB.Str(dr["Category"]);
				string Category = DB.Str(dr["Category"]);
				string Item = DB.Str(dr["MenuItem"]);
				//string QtyNote = DB.Str(dr["QtyNote"]);
				int Qty = DB.Int32(dr["Qty"]);
				if (Qty == 0) Qty = cJobServings;
				string Note = DB.Str(dr["ServiceNote"]);
				int cMenuItems = 0;

				if (fByKitchenLocation && !fKitchenLocationOnOwnPage)
				{
					if (Location != PrevLocation)
					{
						PrevLocation = Location;
						string Html =
							WebPage.Tabs(7) + "<tr>\n" +
							WebPage.Tabs(8) + "<td colspan='3'><div class='Location'>---- " + Location + " ----</div></td>\n" +
							WebPage.Tabs(7) + "</tr>\n";
						Response.Write(Html);
						PrevCategory = "~~~~~";
					}
				}

				if (Category != PrevCategory)
				{
					// search ahead for number of menu items in this category
					int iBeg = dt.Rows.IndexOf(dr);
					int iEnd = iBeg + 1;

					while (iEnd < dt.Rows.Count && 
						(DB.Str(dt.Rows[iEnd]["Location"]) == Location || !(fByKitchenLocation && !fKitchenLocationOnOwnPage)) &&
						DB.Str(dt.Rows[iEnd]["Category"]) == Category)
					{
						iEnd++;
					}
					cMenuItems = iEnd - iBeg;

					PrevCategory = Category;
					Category = "<b>" + Category + "</b>";
				}
				//else
				//{
				//	// test for special case when the first category is blank
				//	cMenuItems = (Category.Length == 0 ? 1 : 0);
				//	Category = "";
				//}

				string IngredSel = string.Empty;
				bool fIngredSel = DB.Bool(dr["IngredSelFlg"]);
				if (fIngredSel)
				{
					IngredSel = DB.Str(dr["IngredSel"]);
					Sql = string.Format(
						"Select Coalesce(i.Name, r.Name) as Name " +
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
						IngredSel += string.Format("<li>{0}</li>\n", DB.Str(drXref["Name"]));
					}
					if (IngredSel.Length > 0)
						IngredSel = string.Format("<ul class='IngredSel'>{0}</ul", IngredSel);
				}

				//MenuItem(Category, Item, QtyNote, Note);
				MenuItem(Category, cMenuItems, Item, Qty.ToString("##,###"), Note, IngredSel);
				Category = "";
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	private void MenuItem(string Category, int cMenuItems, string Item, string QtyNote, string Note, string IngredSel)
	{
		string Html =
			WebPage.Tabs(7) + "<tr>\n" +
			(cMenuItems > 0 ? WebPage.Tabs(8) + "<td valign='top' align='right' rowspan='" + cMenuItems + "'>" + Category + "</td>\n" : string.Empty) +
			WebPage.Tabs(8) + "<td valign='top' align='right'><img src='Images/Diamond.gif' class='RptBullet'></td>\n" +
			WebPage.Tabs(8) + "<td valign='top'>\n" +
			//WebPage.Tabs(9) + WebPage.Table("width='100%'") +
			//WebPage.Tabs(10) + "<tr>\n" +
			//WebPage.Tabs(11) + "<td valign='top'>" + WebPage.Span(Item, "RptMenuItem") + "</td>\n" +
			//WebPage.Tabs(11) + WebPage.Space(5, 1) +
			//WebPage.Tabs(11) + "<td valign='top' align='right'>" + QtyNote + "</td>\n" +
			//WebPage.Tabs(10) + "</tr>\n" +
			//WebPage.Tabs(8) + WebPage.TableEnd() +
			WebPage.Tabs(9) + WebPage.Span(Item, "RptMenuItem") +
			(Note.Length > 0 ? WebPage.Tabs(9) + WebPage.Span(Note, "RptNote" + RandomNoteColor) : string.Empty) +
			WebPage.Tabs(9) + WebPage.Span(QtyNote, "RptQty") +
			WebPage.Tabs(9) + IngredSel +
			WebPage.Tabs(8) + "</td>\n" +
			WebPage.Tabs(7) + "</tr>\n";

		//if (Note.Length > 0)
		//{
		//    Html +=
		//        WebPage.Tabs(7) + "<tr><td colspan='2'></td><td align='center'><i>" + Note + "</i></td></tr>\n";
		//}


		Response.Write(Html);
	}


	protected void btnSelectDay_Click(object sender, EventArgs e)
	{
		rdoDay.Checked = true;
	}
	protected void btnSelectRange_Click(object sender, EventArgs e)
	{
		rdoRange.Checked = true;
	}

    protected async Task<bool> SetupSlingShift()
    {
        Sling = new Sling();
        if (await Sling.Login(SlingEmail, SlingPassword))
        {
            await Sling.LoadUsers();
            dtEnd = dtEnd.AddDays(1);
            await Sling.LoadCalendar(dtBeg, dtEnd);
        }

        return true;
    }

    protected string FindCrew(int JobRno)
    {
        Collection<Sling.CalendarEvent> cShifts = new Collection<Sling.CalendarEvent>();

        if (Sling != null)
        {
            foreach (Sling.CalendarEvent Shift in Sling.Shifts)
            {
                if (Shift.JobRno == JobRno && Shift.User != null)
                {
                    cShifts.Add(Shift);
                }
            }
            Sling.SortShiftCrew(cShifts);
        }

        string htmlCrew = string.Empty;
        int PrevUserId = 0;

        // published
        foreach (Sling.CalendarEvent Shift in cShifts)
        {
            if (Shift.User.ID == PrevUserId || Shift.Status == "planning")
            {
                continue;
            }

            htmlCrew += FormatCrew(Shift);
            PrevUserId = Shift.User.ID;
        }

        string htmlPlan = string.Empty;
        PrevUserId = 0;

        // planning
        foreach (Sling.CalendarEvent Shift in cShifts)
        {
            if (Shift.User.ID == PrevUserId || Shift.Status != "planning")
            {
                continue;
            }

            htmlPlan += FormatCrew(Shift);
            PrevUserId = Shift.User.ID;
        }

		if (htmlPlan.Length > 0)
		{
			if (htmlCrew.Length > 0)
			{
			    htmlCrew += "<hr>\n";
			}
            htmlCrew += "<div class=\"Planning\">Planning</div>\n" + htmlPlan;
		}

        return htmlCrew;
    }

    protected string FormatCrew(Sling.CalendarEvent Shift)
    {
        string htmlCrew =
            string.Format("<div class=\"Name\">{0} {1}</div>", Shift.User.FirstName, (Shift.User.LastName != null && Shift.User.LastName.Length > 0 ? Shift.User.LastName.Substring(0, 1) : string.Empty)) +
            string.Format("<div class=\"Time\">{0}-{1}</div>", FmtDateTime(Shift.Beg), FmtDateTime(Shift.End)) +
            "<div class=\"Reset\"></div>";

        return htmlCrew;
    }

    protected string FmtDateTime(DateTime dt)
    {
        int hr = dt.Hour;
        string ap = "am";
        if (hr >= 12)
        {
            ap = "pm";
            if (hr > 12)
            {
                hr -= 12;
            }
        }
        string min = "0" + dt.Minute.ToString();
        min = min.Substring(min.Length - 2, 2);

        return String.Format("{0}:{1}{2}", hr, min, ap);
    }
}