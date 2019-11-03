using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;
using Globals;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class Invoices : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;
	protected Utils.Calendar calDayDate;
	protected Utils.Calendar calWeekDate;
	protected Utils.Calendar calMonthDate;
	protected Utils.Calendar calBegDateRange;
	protected Utils.Calendar calEndDateRange;
	protected Utils.Calendar calJobsNotInv;
	protected bool fReport = false;
	protected DateTime dtBeg;
	protected DateTime dtEnd;
	protected Int32 JobRno;
	protected bool fSummaryOnly;
	protected bool fNewPage;
	protected bool fFileCopy;
	protected bool fFinalPrint;
	protected bool fFileCopyParm;
	protected bool fCustomerCopyParm;
	protected bool fSingleJob;
    protected bool fDepositInvoice;
	protected decimal Total;
	protected bool fJobsNotInv;

	private void Page_Init(object sender, System.EventArgs e)
	{
		InitCalendars();
	}

	private void Page_Load(object sender, System.EventArgs e)
	{
		System.Diagnostics.Debug.WriteLine("Invoices_Load");
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
		calBegDateRange.HideControlsList = "ddlCustomerDate";

		calEndDateRange = new Utils.Calendar("EndDateRange", ref txtEndDateRange);
		calEndDateRange.ImageButton(imgEndDateRange);
		calEndDateRange.HideControlsList = "ddlCustomerDate";

		calJobsNotInv = new Utils.Calendar("JobsNotInv", ref txtJobsNotInv);
		calJobsNotInv.ImageButton(imgJobsNotInv);
	}

	private void Setup()
	{
		if (Request.QueryString["JobRno"] != null)
		{
			JobRno = Str.Num(Request.QueryString["JobRno"]);
			fReport = true;
			fSingleJob = true;

			fFinalPrint = (Request.QueryString["FinalPrint"] == "1");
			fFileCopyParm = (Request.QueryString["FileCopy"] == "1");
			fCustomerCopyParm = (Request.QueryString["CustomerCopy"] == "1");
            fDepositInvoice = (Request.QueryString["DepositInvoice"] == "1");
        }

		DateTime Today = DateTime.Today;

		txtDayDate.Text = Fmt.Dt(Today);
		txtWeekDate.Text = Fmt.Dt(Today.AddDays(-7));
		txtMonthDate.Text = Fmt.Dt(Today.AddDays(1 - Today.Day));
		txtBegDateRange.Text =
		txtEndDateRange.Text = txtDayDate.Text;
		txtJobsNotInv.Text = txtDayDate.Text;

		CustomerJobs();

		rdoDay.Attributes.Add("onClick", "EnableInvoice();");
		txtDayDate.Attributes.Add("onChange", "iSetChk('rdoDay', true);EnableInvoice();");
		rdoWeek.Attributes.Add("onClick", "EnableInvoice();");
		txtWeekDate.Attributes.Add("onChange", "iSetChk('rdoWeek', true);EnableInvoice();");
		rdoMonth.Attributes.Add("onClick", "EnableInvoice();");
		txtMonthDate.Attributes.Add("onChange", "iSetChk('rdoMonth', true);EnableInvoice();");
		rdoRange.Attributes.Add("onClick", "EnableInvoice();");
		txtBegDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);EnableInvoice();");
		txtEndDateRange.Attributes.Add("onChange", "iSetChk('rdoRange', true);EnableInvoice();");
		ddlCustomerDate.Attributes.Add("onChange", "iSetChk('rdoCustomerDate', true);EnableInvoice();");
		rdoJobsNotInv.Attributes.Add("onClick", "EnableJobsNotInv();EnableJobsNotInv();");
		txtJobsNotInv.Attributes.Add("onChange", "iSetChk('rdoJobsNotInv', true);EnableJobsNotInv();");


		chkNotInvoicedOnly.Attributes.Add("onClick", "OptionSet(this.checked, 'spNotInvoicedOnly');");
		chkSummaryOnly.Attributes.Add("onClick", "OptionSet(this.checked, 'spSummaryOnly');");
		chkCustomerCopy.Attributes.Add("onClick", "OptionSet(this.checked, 'spCustomerCopy');");
		chkFileCopy.Attributes.Add("onClick", "OptionSet(this.checked, 'spFileCopy');");

		chkIncludeAll.Attributes.Add("onClick", "SameWindow();");
		btnReport.Attributes.Add("onClick", "return Print();");
		btnJobsNotInv.Attributes.Add("onClick", "NewWindow();");
	}

	private void CustomerJobs()
	{
		string Sql =
            "Select JobRno, Coalesce(cu.Name, c.Name) as Customer, JobDate " +
            "From mcJobs j " +
            "Inner Join Contacts c On j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Where CancelledDtTm Is Null And InvCreatedDtTm Is Not Null And Status <> 'Proposal' ";
		if (!chkIncludeAll.Checked)
		{
			Sql += "And JobDate <= " + DB.PutDtTm(DateTime.Today) + " And InvoicedDtTm Is Null ";
		}
		Sql += "Order By Customer, JobDate";

		try
		{
			DataTable dt = db.DataTable(Sql);
			ddlCustomerDate.Items.Clear();
			foreach (DataRow dr in dt.Rows)
			{
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

		if (rdoCustomerDate.Checked)
		{
			JobRno = Str.Num(ddlCustomerDate.Items[ddlCustomerDate.SelectedIndex].Value);
		}

		fFinalPrint = true;
		fFileCopyParm = (chkFileCopy.Checked);
		fCustomerCopyParm = (chkCustomerCopy.Checked);

		fReport = true;
		fSingleJob = false;
	}

	protected void Report()
	{
		fNewPage = false;

		Response.Write("\t</head>\n\t<body>\n");

		if (fJobsNotInv)
		{
			JobsNotInv();
		}
		else
		{
			fSummaryOnly = (chkSummaryOnly.Checked && JobRno == 0);

			if (fSummaryOnly)
			{
				Summary();
			}

			PrintInvoices();
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
			WebPage.Tabs(3) + WebPage.Div("Invoices", "center", "RptTitle") +
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

	protected void PrintInvoices()
	{
		DateTime PrevDate = DateTime.MinValue;
		int cDays = 0;
		int cJobs = 0;
		int cServ = 0;

		string SqlWhere = "";
		SqlWhere = DB.And(SqlWhere, (fSingleJob ? "" : "CancelledDtTm Is Null And InvCreatedDtTm Is Not Null "));
		SqlWhere = DB.And(SqlWhere, (JobRno > 0 ? "JobRno = " + JobRno : "JobDate Between " + DB.PutDtTm(dtBeg) + " And " + DB.PutDtTm(dtEnd)));
		SqlWhere = DB.And(SqlWhere, (JobRno == 0 && chkNotInvoicedOnly.Checked ? "InvoicedDtTm Is Null" : ""));

		string Sql =
            "Select Coalesce(cu.Name, c.Name) as Customer, " +
            "j.JobRno, j.JobDate, j.NumMenServing, j.NumWomenServing, j.NumChildServing, " +
            "j.DepartureTime, j.MealTime, j.EventType, j.ServiceType, j.InvoicedDtTm " + 
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

					if (JobDate != PrevDate)
					{
						cDays++;
						PrevDate = JobDate;
					}

					cJobs++;
					cServ += cServings;

					Response.Write(
						WebPage.Tabs(1) + "<tr>\n" +
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
				else
				{
					if (fCustomerCopyParm)
					{
						fFileCopy = false;
						PrintInvoice(JobRno);
					}

					if (fFileCopyParm)
					{
						fFileCopy = true;
						PrintInvoice(JobRno);
					}

					if (fCustomerCopyParm)
					{
						if (fFinalPrint &&
							dr["InvoicedDtTm"] == DBNull.Value)
						{
							DateTime Tm = DateTime.Now;

							Sql =
								"Update mcJobs Set " +
								"Status = 'Invoiced', " +
								"InvoicedDtTm = " + DB.PutDtTm(Tm) + ", " +
								"InvoicedUser = " + DB.PutStr(g.User) + ", " +
                                (fDepositInvoice ?
                                "DepositInvPrintedDtTm = " + DB.PutDtTm(Tm) + ", " +
                                "DepositInvPrintedUser = " + DB.PutStr(g.User) + ", "
                                : string.Empty) + 
								"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
								"UpdatedUser = " + DB.PutStr(g.User) + " " +
								"Where JobRno = " + JobRno;
							db.Exec(Sql);
						}
					}

                    if (fDepositInvoice)
                    {
                        DateTime Tm = DateTime.Now;

                        Sql =
                            "Update mcJobs Set " +
                            "DepositInvPrintedDtTm = " + DB.PutDtTm(Tm) + ", " +
                            "DepositInvPrintedUser = " + DB.PutStr(g.User) + ", " +
                            "UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
                            "UpdatedUser = " + DB.PutStr(g.User) + " " +
                            "Where JobRno = " + JobRno;
                        db.Exec(Sql);
                    }
                }
            }

			if (dt.Rows.Count == 0)
			{
				Response.Write("<br><br><div class='JobSubTitle' align='center'>No Invoices Found to Print</div>\n");
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

	protected void PrintInvoice(Int32 JobRno)
	{
		string Sql =
            "Select Coalesce(cu.Name, c.Name) as Customer, c.Name, c.Phone, c.Cell, c.Fax, c.Email, " +
            "j.Location, j.PONumber, j.JobDate, j.ServiceType, cu.TaxExemptFlg, j.SubTotTaxPct, j.ServiceTaxPct, j.DeliveryTaxPct, j.ChinaTaxPct, j.AddServiceTaxPct, " +
            "j.FuelTravelTaxPct, j.FacilityTaxPct, j.GratuityTaxPct, j.RentalsTaxPct, j.Adj1TaxPct, j.Adj2TaxPct, j.CCFeeTaxPct, j.CCFeePct, " +
            "j.InvEventCount, j.InvPricePerPerson, " +
            "j.SubTotAmt, j.ServiceAmt, j.DeliveryAmt, j.ChinaAmt, j.AddServiceAmt, j.FuelTravelAmt, j.FacilityAmt, " +
            "j.GratuityAmt, j.VoluntaryGratuityAmt, j.RentalsAmt, j.RentalsDesc, j.Adj1Desc, j.Adj2Desc, j.Adj1Amt, j.Adj2Amt, j.CCFeeAmt, j.InvBalAmt, " +
            "j.ServiceSubTotPctFlg, j.ServiceSubTotPct, j.DeliverySubTotPctFlg, j.DeliverySubTotPct, j.ChinaSubTotPctFlg, j.ChinaSubTotPct, " +
            "j.AddServiceSubTotPctFlg, j.AddServiceSubTotPct, j.FuelTravelSubTotPctFlg, j.FuelTravelSubTotPct, " +
            "j.FacilitySubTotPctFlg, j.FacilitySubTotPct, j.GratuitySubTotPctFlg, j.GratuitySubTotPct, j.RentalsSubTotPctFlg, j.RentalsSubTotPct, " +
            "j.Adj1SubTotPctFlg, j.Adj1SubTotPct, j.Adj2SubTotPctFlg, j.Adj2SubTotPct, j.CCPmtFeeFlg, j.CCFeePct, " +
            "j.JobType, j.PmtTerms, j.PreTaxSubTotAmt, j.SalesTaxTotAmt, j.InvTotAmt, j.DepositAmt, j.JobRno " +
            "From mcJobs j " +
            "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Where JobRno = " + JobRno;

		try
		{
			DataTable dt = db.DataTable(Sql);
			DataRow dr = dt.Rows[0];

			string sPrinted = Fmt.Dt(DateTime.Now);

			if (fFileCopy)
			{
				Response.Write(
					WebPage.Tabs(0) + WebPage.Table("align='center'" + (fNewPage ? " style='page-break-before: always;'" : "")) +
					WebPage.Tabs(1) + "<tr>\n" +
					WebPage.Tabs(2) + "<td><img alt='' src='Images/Pot3.gif' border='0'></td>\n" +
					WebPage.Tabs(2) + WebPage.Space(100, 1) +
					WebPage.Tabs(2) + "<td class='JobTitle'>File Copy</td>\n" +
					WebPage.Tabs(2) + WebPage.Space(100, 1) +
					WebPage.Tabs(2) + "<td>Printed&nbsp;&nbsp;" + Fmt.DtTm12HrSec(DateTime.Now) + "</td>\n" +
					WebPage.Tabs(1) + "</tr>\n" +
					WebPage.Tabs(0) + WebPage.TableEnd()
					);
			}
			else
			{
				Response.Write(
					WebPage.Tabs(0) + WebPage.Table("align='center'" + (fNewPage ? " style='page-break-before: always;'" : "")) +
					WebPage.Tabs(1) + "<tr>\n" +
					WebPage.Tabs(2) + "<td><img alt='' src='Images/Pot3.gif' border='0'></td>\n" +
					//						WebPage.Tabs(2) + WebPage.Space(80, 1) +
					WebPage.Tabs(2) + "<td><img alt='' src='Images/HeaderWhtDrkInv.gif' border='0'></td>\n" +
					WebPage.Tabs(1) + "</tr>\n" +
					WebPage.Tabs(0) + WebPage.TableEnd()
					);
			}

			InvoiceTop(ref dr);
			InvoiceAmounts(JobRno, ref dr);

			//if (fFileCopy)
			//{
			//    FileCopyPaymentInfo(ref dr);
			//}
			//else
			//{
			//    string sInvPmtDate = Fmt.Dt(DB.DtTm(dr["InvPmtDate"]));

			//    if (sInvPmtDate.Length == 0)
			//    {
			//        PaymentInfo(ref dr);
			//    }
			//    else
			//    {
			//        PaymentMade(ref dr);
			//    }
			//}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		fNewPage = true;
	}

	protected void InvoiceTop(ref DataRow dr)
	{
		string sName     = DB.Str(dr["Name"]);
		string sPhone    = DB.Str(dr["Phone"]);
		string sCell     = DB.Str(dr["Cell"]);
		string sFax      = DB.Str(dr["Fax"]);
		string sEmail    = DB.Str(dr["Email"]);
		string sLocation = DB.Str(dr["Location"]);
	    string sPONumber = DB.Str(dr["PONumber"]);

		string Html = "";

		Html +=
			"<div style='width: 475px; margin: 0px auto; margin-bottom: 50px;'>\n" +
				"<p style='line-height: 1.8em; font-size: 125%'><span class='DimTitle'>Catering Services For:</span> <span style='font-size: 120%; font-weight: bold;'>" + DB.Str(dr["Customer"]) + "</span><br />\n" +
				"<span class='DimTitle'>Contact Info:</span> " + sName + (sPhone.Length > 0 ? ", " + sPhone : "") + (sCell.Length > 0 ? ", " + sCell : "") + (sEmail.Length > 0 ? ", " + sEmail : "") + "</p>\n" +
				"<div class=\"EventInfo\">" +
                			"<div><span class='DimTitle'>Event Location:</span> " + sLocation + "</div>\n" +
					"<div><span class='DimTitle'>Event Date:</span> " + Fmt.Dt(DB.DtTm(dr["JobDate"])) + "</div>\n" +
			                "<div><span class='DimTitle'>Service Type:</span> " + DB.Str(dr["ServiceType"]) + "</div>\n" +
			                (sPONumber.Length > 0 ? "<div><span class='DimTitle'>PO #:</span> " + sPONumber + "</div>\n" : string.Empty) + 
		            	"</div>" +
            		"</div>\n";

		//Html += 
		//    WebPage.Tabs(0) + WebPage.Table("align='center'") +
		//    WebPage.Tabs(1) + WebPage.SpaceTr(1, 20) +
		//    WebPage.Tabs(1) + "<tr>\n" +
		//    WebPage.Tabs(2) + "<td>\n" +
		//    WebPage.Tabs(3) + WebPage.Border(1, 5) +
		//    WebPage.Tabs(3) + WebPage.Table() +
		//    WebPage.Tabs(4) + "<tr><td><b>Catering Services for:</b></td></tr>\n" +
		//    WebPage.Tabs(4) + "<tr>\n" +
		//    WebPage.Tabs(5) + "<td>\n" +
		//    WebPage.Tabs(6) + WebPage.Table() +
		//    WebPage.Tabs(7) + "<tr>\n" +
		//    WebPage.Tabs(8) + WebPage.Space(15, 1) +
		//    WebPage.Tabs(8) + "<td></td>\n" +
		//    WebPage.Tabs(7) + "</tr>\n" +
		//    WebPage.Tabs(7) + "<tr>\n" +
		//    WebPage.Tabs(8) + "<td></td>\n" +
		//    WebPage.Tabs(8) + "<td>" + DB.Str(dr["Customer"]) + "</td>\n" +
		//    WebPage.Tabs(7) + "</tr>\n" +
		//    WebPage.Tabs(7) + "<tr>\n" +
		//    WebPage.Tabs(8) + "<td></td>\n" +
		//    WebPage.Tabs(8) + "<td>" + DB.Str(dr["ContactName"]) + "</td>\n" +
		//    WebPage.Tabs(7) + "</tr>\n" +
		//    WebPage.Tabs(7) + "<tr>\n" +
		//    WebPage.Tabs(8) + "<td></td>\n" +
		//    WebPage.Tabs(8) + "<td>\n" +
		//    WebPage.Tabs(9) + WebPage.Table() +
		//    WebPage.Tabs(10) + "<tr>\n" +
		//    WebPage.Tabs(11) + "<td></td>\n" +
		//    WebPage.Tabs(11) + WebPage.Space(10, 1) +
		//    WebPage.Tabs(11) + "<td></td>\n" +
		//    WebPage.Tabs(10) + "</tr>\n";

		//if (sContactPhone.Length > 0)
		//{
		//    Html +=
		//        WebPage.Tabs(10) + "<tr>\n" +
		//        WebPage.Tabs(11) + "<td align='right'>Phone:</td>\n" +
		//        WebPage.Tabs(11) + "<td></td>\n" +
		//        WebPage.Tabs(11) + "<td>" + sContactPhone + "</td>\n" +
		//        WebPage.Tabs(10) + "</tr>\n";
		//}
		//if (sContactCell.Length > 0)
		//{
		//    Html +=
		//        WebPage.Tabs(10) + "<tr>\n" +
		//        WebPage.Tabs(11) + "<td align='right'>Cell:</td>\n" +
		//        WebPage.Tabs(11) + "<td></td>\n" +
		//        WebPage.Tabs(11) + "<td>" + sContactCell + "</td>\n" +
		//        WebPage.Tabs(10) + "</tr>\n";
		//}
		//if (sContactFax.Length > 0)
		//{
		//    Html +=
		//        WebPage.Tabs(10) + "<tr>\n" +
		//        WebPage.Tabs(11) + "<td align='right'>Fax:</td>\n" +
		//        WebPage.Tabs(11) + "<td></td>\n" +
		//        WebPage.Tabs(11) + "<td>" + sContactFax + "</td>\n" +
		//        WebPage.Tabs(10) + "</tr>\n";
		//}
		//if (sContactEmail.Length > 0)
		//{
		//    Html +=
		//        WebPage.Tabs(10) + "<tr>\n" +
		//        WebPage.Tabs(11) + "<td align='right'>Email:</td>\n" +
		//        WebPage.Tabs(11) + "<td></td>\n" +
		//        WebPage.Tabs(11) + "<td>" + sContactEmail + "</td>\n" +
		//        WebPage.Tabs(10) + "</tr>\n";
		//}
		//if (sLocation.Length > 0)
		//{
		//    Html +=
		//        WebPage.Tabs(10) + "<tr>\n" +
		//        WebPage.Tabs(11) + "<td align='right'>Location:</td>\n" +
		//        WebPage.Tabs(11) + "<td></td>\n" +
		//        WebPage.Tabs(11) + "<td>" + sLocation + "</td>\n" +
		//        WebPage.Tabs(10) + "</tr>\n";
		//}

		//Html +=
		//    WebPage.Tabs(9) + WebPage.TableEnd() +
		//    WebPage.Tabs(8) + "</td>\n" +
		//    WebPage.Tabs(7) + "</tr>\n" +
		//    WebPage.Tabs(6) + WebPage.TableEnd() +
		//    WebPage.Tabs(5) + "</td>\n" +
		//    WebPage.Tabs(4) + "</tr>\n" +
		//    WebPage.Tabs(3) + WebPage.TableEnd() +
		//    WebPage.Tabs(3) + WebPage.BorderEnd() +
		//    WebPage.Tabs(2) + "</td>\n" +
		//    WebPage.Tabs(1) + WebPage.Space(200, 1) +
		//    WebPage.Tabs(2) + "<td>\n" +
		//    WebPage.Tabs(3) + WebPage.Table() +
		//    WebPage.Tabs(4) + "<tr>\n" +
		//    WebPage.Tabs(5) + "<td></td>\n" +
		//    WebPage.Tabs(5) + WebPage.Space(10, 1) +
		//    WebPage.Tabs(5) + "<td></td>\n" +
		//    WebPage.Tabs(4) + "</tr>\n" +
		//    WebPage.Tabs(4) + "<tr>\n" +
		//    WebPage.Tabs(5) + "<td align='right'><b>Invoice #:</b></td>\n" +
		//    WebPage.Tabs(5) + "<td></td>\n" +
		//    WebPage.Tabs(5) + "<td>" + JobRno + "</td>\n" +
		//    WebPage.Tabs(4) + "</tr>\n" +
		//    WebPage.Tabs(4) + "<tr>\n" +
		//    WebPage.Tabs(5) + "<td align='right'><b>Invoice Date:</b></td>\n" +
		//    WebPage.Tabs(5) + "<td></td>\n" +
		//    WebPage.Tabs(5) + "<td>" + Fmt.Dt(DB.DtTm(dr["InvDate"])) + "</td>\n" +
		//    WebPage.Tabs(4) + "</tr>\n" +
		//    WebPage.Tabs(4) + WebPage.SpaceTr(1, 20) +
		//    WebPage.Tabs(4) + "<tr>\n" +
		//    WebPage.Tabs(5) + "<td align='center' colspan='3'>\n";

		//if (Fmt.Dt(DB.DtTm(dr["InvoicedDtTm"])) != "")
		//{
		//    Html +=
		//        WebPage.Tabs(6) + WebPage.Table() +
		//        WebPage.Tabs(7) + "<tr>\n" +
		//        WebPage.Tabs(8) + "<td class='Reprint'>Reprint</td>\n" +
		//        WebPage.Tabs(7) + "</tr>\n" +
		//        WebPage.Tabs(6) + WebPage.TableEnd();
		//}

		//Html +=
		//    WebPage.Tabs(5) + "</td>\n" +
		//    WebPage.Tabs(4) + "</tr>\n" +
		//    WebPage.Tabs(3) + WebPage.TableEnd() +
		//    WebPage.Tabs(2) + "</td>\n" +
		//    WebPage.Tabs(1) + "</tr>\n" +
		//    WebPage.Tabs(0) + WebPage.TableEnd();

		Response.Write(Html);
	}

	protected void InvoiceAmounts(Int32 JobRno, ref DataRow dr)
	{
		bool fTaxExempt          = DB.Bool(dr["TaxExemptFlg"]);
		decimal SubtotalTaxPct   = DB.Dec(dr["SubTotTaxPct"]);
		decimal ServiceTaxPct    = DB.Dec(dr["ServiceTaxPct"]);
		decimal DeliveryTaxPct   = DB.Dec(dr["DeliveryTaxPct"]);
		decimal ChinaTaxPct      = DB.Dec(dr["ChinaTaxPct"]);
		decimal AddServiceTaxPct = DB.Dec(dr["AddServiceTaxPct"]);
		decimal FuelTravelTaxPct = DB.Dec(dr["FuelTravelTaxPct"]);
		decimal FacilityTaxPct   = DB.Dec(dr["FacilityTaxPct"]);
		decimal GratuityTaxPct   = DB.Dec(dr["GratuityTaxPct"]);
		decimal RentalsTaxPct    = DB.Dec(dr["RentalsTaxPct"]);
		decimal Adj1TaxPct       = DB.Dec(dr["Adj1TaxPct"]);
		decimal Adj2TaxPct       = DB.Dec(dr["Adj2TaxPct"]);
		decimal CCFeeTaxPct      = DB.Dec(dr["CCFeeTaxPct"]);
		decimal CCFeePct         = DB.Dec(dr["CCFeePct"]);

		Int32 EventCount = DB.Int32(dr["InvEventCount"]);
		decimal Price = DB.Dec(dr["InvPricePerPerson"]);
		//decimal Subtotal = EventCount * Price;
		//string sAdditionalPrices = AdditionalPrices(ref Subtotal, (fTaxExempt ? 0 : SubtotalTaxPct));
		string sAdditionalPrices = AdditionalPrices((fTaxExempt ? 0 : SubtotalTaxPct));
		decimal Subtotal = DB.Dec(dr["SubTotAmt"]);
		string sSubtotal = Fmt.Dollar(Subtotal);

		decimal Service           = DB.Dec(dr["ServiceAmt"]);
		decimal Delivery          = DB.Dec(dr["DeliveryAmt"]);
		decimal China             = DB.Dec(dr["ChinaAmt"]);
		decimal AddService        = DB.Dec(dr["AddServiceAmt"]);
		decimal FuelTravel        = DB.Dec(dr["FuelTravelAmt"]);
		decimal Facility          = DB.Dec(dr["FacilityAmt"]);
		decimal Gratuity          = DB.Dec(dr["GratuityAmt"]);
        decimal VoluntaryGratuity = DB.Dec(dr["VoluntaryGratuityAmt"]);
	    decimal Rentals           = DB.Dec(dr["RentalsAmt"]);
		string RentalsDesc        = DB.Str(dr["RentalsDesc"]);
		string Adj1Desc           = DB.Str(dr["Adj1Desc"]);
		string Adj2Desc           = DB.Str(dr["Adj2Desc"]);
		decimal Adj1Amt           = DB.Dec(dr["Adj1Amt"]);
		decimal Adj2Amt           = DB.Dec(dr["Adj2Amt"]);
		decimal CCFee             = DB.Dec(dr["CCFeeAmt"]);
		decimal Balance           = DB.Dec(dr["InvBalAmt"]);

		//string PmtType1 = DB.Str(dr["PmtType1"]);
		//string PmtType2 = DB.Str(dr["PmtType2"]);
		//string PmtType3 = DB.Str(dr["PmtType3"]);
		//decimal PmtAmt1 = DB.Dec(dr["PmtAmt1"]);
		//decimal PmtAmt2 = DB.Dec(dr["PmtAmt2"]);
		//decimal PmtAmt3 = DB.Dec(dr["PmtAmt3"]);
		//string PmtRef1 = DB.Str(dr["PmtRef1"]);
		//string PmtRef2 = DB.Str(dr["PmtRef2"]);
		//string PmtRef3 = DB.Str(dr["PmtRef3"]);
		//string PmtMethod1 = DB.Str(dr["PmtMethod1"]);
		//string PmtMethod2 = DB.Str(dr["PmtMethod2"]);
		//string PmtMethod3 = DB.Str(dr["PmtMethod3"]);
		//decimal CCReqAmt1 = DB.Dec(dr["CCReqAmt1"]);
		//decimal CCReqAmt2 = DB.Dec(dr["CCReqAmt2"]);
		//decimal CCReqAmt3 = DB.Dec(dr["CCReqAmt3"]);

		//decimal CCFee1 = 0;
		//decimal CCFee2 = 0;
		//decimal CCFee3 = 0;
		//decimal CCFeeBalance = 0;

		//if (PmtMethod1 == "CreditCard")
		//{
		//	CCFee1 = Math.Round(CCReqAmt1 * CCFeePct / 100, 2);
		//}
		//if (PmtMethod2 == "CreditCard")
		//{
		//	CCFee2 = Math.Round(CCReqAmt2 * CCFeePct / 100, 2);
		//}
		//if (PmtMethod3 == "CreditCard")
		//{
		//	CCFee3 = Math.Round(CCReqAmt3 * CCFeePct / 100, 2);
		//}

		//decimal SalesTax = Subtotal   * SubtotalTaxPct
		//				 + Service    * ServiceTaxPct
		//				 + Delivery   * DeliveryTaxPct
		//				 + China      * ChinaTaxPct
		//				 + AddService * AddServiceTaxPct
		//				 + FuelTravel * FuelTravelTaxPct
		//				 + Facility	 * FacilityTaxPct
		//				 + Gratuity	 * GratuityTaxPct
		//				 + Rentals	 * RentalsTaxPct
		//				 + Adj1Amt	 * Adj1TaxPct
		//				 + Adj2Amt	 * Adj2TaxPct
		//				 + CCFee1	 * CCFeeTaxPct
		//				 + CCFee2	 * CCFeeTaxPct
		//				 + CCFee3	 * CCFeeTaxPct;
		//SalesTax /= 100;
		//if (fTaxExempt)
		//{
		//	SalesTax = 0;
		//}

		//Subtotal += Service
		//		 + Delivery
		//		 + China
		//		 + AddService
		//		 + FuelTravel
		//		 + Facility
		//		 + Gratuity
		//		 + Rentals
		//		 + Adj1Amt
		//		 + Adj2Amt
		//		 + CCFee1
		//		 + CCFee2
		//		 + CCFee3;

		//Total = Subtotal + SalesTax;
		//decimal Balance = Total - (PmtAmt1 + PmtAmt2 + PmtAmt3);

		//bool fChargeCCFee = DB.Bool(dr["CCPmtFeeFlg"]);
		//if (fChargeCCFee)
		//{
		//	decimal Fee = Balance * CCFeePct / 100;
		//	decimal Tax = Fee * CCFeeTaxPct / 100;

		//	if (fTaxExempt)
		//	{
		//		Tax = 0;
		//	}

		//	CCFeeBalance += Fee;
		//	Subtotal += Fee;
		//	SalesTax += Tax;
		//	Total += Fee + Tax;
		//	Balance += Fee + Tax;
		//}

		string sServiceSubTotPct    = (DB.Bool(dr["ServiceSubTotPctFlg"])    ? Fmt.Pct(DB.Dec(dr["ServiceSubTotPct"]),    3, false) : "");
		string sDeliverySubTotPct   = (DB.Bool(dr["DeliverySubTotPctFlg"])   ? Fmt.Pct(DB.Dec(dr["DeliverySubTotPct"]),   3, false) : "");
		string sChinaSubTotPct      = (DB.Bool(dr["ChinaSubTotPctFlg"])      ? Fmt.Pct(DB.Dec(dr["ChinaSubTotPct"]),      3, false) : "");
		string sAddServiceSubTotPct = (DB.Bool(dr["AddServiceSubTotPctFlg"]) ? Fmt.Pct(DB.Dec(dr["AddServiceSubTotPct"]), 3, false) : "");
		string sFuelTravelSubTotPct = (DB.Bool(dr["FuelTravelSubTotPctFlg"]) ? Fmt.Pct(DB.Dec(dr["FuelTravelSubTotPct"]), 3, false) : "");
		string sFacilitySubTotPct   = (DB.Bool(dr["FacilitySubTotPctFlg"])   ? Fmt.Pct(DB.Dec(dr["FacilitySubTotPct"]),   3, false) : "");
		string sGratuitySubTotPct   = (DB.Bool(dr["GratuitySubTotPctFlg"])   ? Fmt.Pct(DB.Dec(dr["GratuitySubTotPct"]),   3, false) : "");
		string sRentalsSubTotPct    = (DB.Bool(dr["RentalsSubTotPctFlg"])    ? Fmt.Pct(DB.Dec(dr["RentalsSubTotPct"]),    3, false) : "");
		string sAdj1SubTotPct       = (DB.Bool(dr["Adj1SubTotPctFlg"])       ? Fmt.Pct(DB.Dec(dr["Adj1SubTotPct"]),       3, false) : "");
		string sAdj2SubTotPct       = (DB.Bool(dr["Adj2SubTotPctFlg"])       ? Fmt.Pct(DB.Dec(dr["Adj2SubTotPct"]),       3, false) : "");
		string sCCFeePct            = (DB.Bool(dr["CCPmtFeeFlg"])            ? Fmt.Pct(DB.Dec(dr["CCFeePct"]),            3, false) : "");

		string PmtTerms = (DB.Str(dr["JobType"]) == "Corporate" ? DB.Str(dr["PmtTerms"]) : string.Empty);

		string sServiceAmt           = Fmt.Dollar(Service, false);
		string sDeliveryAmt          = Fmt.Dollar(Delivery, false);
		string sChinaAmt             = Fmt.Dollar(China, false);
		string sAddServiceAmt        = Fmt.Dollar(AddService, false);
		string sFuelTravelAmt        = Fmt.Dollar(FuelTravel, false);
		string sFacilityAmt          = Fmt.Dollar(Facility, false);
		string sGratuityAmt          = Fmt.Dollar(Gratuity, false);
		string sVoluntaryGratuityAmt = Fmt.Dollar(VoluntaryGratuity, false);
		string sRentalsAmt           = Fmt.Dollar(Rentals, false);
		string sAdj1Desc             = Adj1Desc;
		string sAdj2Desc             = Adj2Desc;
		string sAdj1Amt              = Fmt.Dollar(Adj1Amt, false);
		string sAdj2Amt              = Fmt.Dollar(Adj2Amt, false);
		//string sCCFeeAmt           = Fmt.Dollar(CCFee1 + CCFee2 + CCFee3 + CCFeeBalance, false);
		string sCCFeeAmt             = (CCFee != 0 ? Fmt.Dollar(CCFee) : string.Empty);

		string sSubtotalTaxPct   = Fmt.Pct(SubtotalTaxPct,   3, false);
		string sServiceTaxPct    = Fmt.Pct(ServiceTaxPct,    3, false);
		string sDeliveryTaxPct   = Fmt.Pct(DeliveryTaxPct,   3, false);
		string sChinaTaxPct      = Fmt.Pct(ChinaTaxPct,      3, false);
		string sAddServiceTaxPct = Fmt.Pct(AddServiceTaxPct, 3, false);
		string sFuelTravelTaxPct = Fmt.Pct(FuelTravelTaxPct, 3, false);
		string sFacilityTaxPct   = Fmt.Pct(FuelTravelTaxPct, 3, false);
		string sGratuityTaxPct   = Fmt.Pct(GratuityTaxPct,   3, false);
		string sRentalsTaxPct    = Fmt.Pct(RentalsTaxPct,    3, false);
		string sAdj1TaxPct       = Fmt.Pct(Adj1TaxPct,       3, false);
		string sAdj2TaxPct       = Fmt.Pct(Adj2TaxPct,       3, false);
		string sCCFeeTaxPct      = Fmt.Pct(CCFeeTaxPct,      3, false);

		decimal PreTaxSubtotal = DB.Dec(dr["PreTaxSubTotAmt"]);
		string sPreTaxSubtotal = Fmt.Dollar(PreTaxSubtotal, false);
		decimal SalesTax       = DB.Dec(dr["SalesTaxTotAmt"]);
		string sSalesTax       = Fmt.Dollar(SalesTax);
		decimal Total          = DB.Dec(dr["InvTotAmt"]);
		string sTotal          = Fmt.Dollar(Total);
        	decimal DepositAmt     = Str.Dec(Request["DepositAmt"]);
        	string sDepositAmt     = Fmt.Dollar(DepositAmt);
        	//string sDeposit      = Fmt.Dollar(DepositAmt);
        	string sBalance        = Fmt.Dollar(Balance);

		decimal Gratuity15 = Math.Round(Subtotal * 0.15M, 2);
		decimal Gratuity20 = Math.Round(Subtotal * 0.2M, 2);
		string sGratuity15 = Fmt.Dollar(Gratuity15, true);
		string sGratuity20 = Fmt.Dollar(Gratuity20, true);
		string sGratuity15Balance = Fmt.Dollar(Balance + Gratuity15);
		string sGratuity20Balance = Fmt.Dollar(Balance + Gratuity20);

		string Html =
			"<div class='InvAmts'>\n" +
			WebPage.Tabs(0) + WebPage.Table("align='center'" + (fFileCopy ? " style='width: initial;'" : "")) +
			(fFileCopy ? 
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(20, 20) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(20, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(20, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			//WebPage.Tabs(2) + WebPage.Space(20, 1) +
			WebPage.Tabs(1) + "</tr>\n" : "");
			//WebPage.Tabs(1) + "<tr>\n" +
			//WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
			//WebPage.Tabs(2) + "<td align='right'>Event Date</td>\n" +
			//WebPage.Tabs(2) + "<td></td>\n" +
			//WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dt(DB.DtTm(dr["JobDate"])) + "</td>\n" +
			//WebPage.Tabs(1) + "</tr>\n" +
			//WebPage.Tabs(1) + "<tr>\n" +
			//WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
			//WebPage.Tabs(2) + "<td class='InvField'>Event Count</td>\n" +
			//WebPage.Tabs(2) + "<td></td>\n" +
			//WebPage.Tabs(2) + "<td align='right'>" + Fmt.Num(EventCount) + "</td>\n" +
			//WebPage.Tabs(1) + "</tr>\n" +
			//WebPage.Tabs(1) + "<tr>\n" +
			//WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
			//WebPage.Tabs(2) + "<td class='InvField'>Price Per Person</td>\n" +
			//WebPage.Tabs(2) + "<td></td>\n" +
			//WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dollar(Price) + "</td>\n" +
			//WebPage.Tabs(1) + "</tr>\n" +

		if ((EventCount != 0 || Price != 0) && sAdditionalPrices.Length == 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
				WebPage.Tabs(2) + "<td class='InvField'>" + Fmt.Num(EventCount) + " servings @ " + Fmt.Dollar(Price) + " per person</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dollar(EventCount * Price) + "</td>\n" +
				WebPage.Tabs(1) + "</tr>\n";
		}

		Html +=
			sAdditionalPrices +
			WebPage.Tabs(1) + "<tr>\n" +
			(fFileCopy ?
			WebPage.Tabs(2) + "<td align='right'>% of Subtotal</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" :
			WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
			WebPage.Tabs(2) + "<td colspan='3'><div class='hr' /></td>\n" +
			(fFileCopy ?
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right' colspan='2'>Sales Tax %</td>\n" : "") +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
			WebPage.Tabs(2) + "<td class='InvField'><b>Subtotal</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right'><b>" + sSubtotal + "</b></td>\n" +
			(fFileCopy ?
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right'>" + sSubtotalTaxPct + "</td>\n" : "") +
			WebPage.Tabs(1) + "</tr>\n";

		if (
			sGratuityAmt.Length > 0 ||
            		sVoluntaryGratuityAmt.Length > 0 ||
            		sDeliveryAmt.Length > 0 ||
			sChinaAmt.Length > 0 ||
			sAddServiceAmt.Length > 0 ||
			sRentalsAmt.Length > 0 ||
			sAdj1Desc.Length > 0 || sAdj1Amt.Length > 0 ||
			sAdj2Desc.Length > 0 || sAdj2Amt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + WebPage.SpaceTr(1, 10);
		}
		if (sServiceAmt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td align='right'>" + sServiceSubTotPct + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" :
				WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
				WebPage.Tabs(2) + "<td class='InvField'>On-Site Service Fee</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sServiceAmt + "</td>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sServiceTaxPct + "</td>\n" : "") +
				WebPage.Tabs(1) + "</tr>\n";
		}
		if (sDeliveryAmt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td align='right'>" + sDeliverySubTotPct + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" :
				WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
				WebPage.Tabs(2) + "<td class='InvField'>Delivery</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sDeliveryAmt + "</td>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sDeliveryTaxPct + "</td>\n" : "") +
				WebPage.Tabs(1) + "</tr>\n";
		}
		if (sChinaAmt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td align='right'>" + sChinaSubTotPct + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" :
				WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
				WebPage.Tabs(2) + "<td class='InvField'>China</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sChinaAmt + "</td>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sChinaTaxPct + "</td>\n" : "") +
				WebPage.Tabs(1) + "</tr>\n";
		}
		if (sAddServiceAmt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td align='right'>" + sAddServiceSubTotPct + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" :
				WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
				WebPage.Tabs(2) + "<td class='InvField'>Additional Service Fee</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sAddServiceAmt + "</td>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sAddServiceTaxPct + "</td>\n" : "") +
				WebPage.Tabs(1) + "</tr>\n";
		}
		if (sFuelTravelAmt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td align='right'>" + sFuelTravelSubTotPct + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" :
				WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
				WebPage.Tabs(2) + "<td class='InvField'>Fuel &amp; Travel</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sFuelTravelAmt + "</td>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sFuelTravelTaxPct + "</td>\n" : "") +
				WebPage.Tabs(1) + "</tr>\n";
		}
		if (sFacilityAmt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td align='right'>" + sFacilitySubTotPct + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" :
				WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
				WebPage.Tabs(2) + "<td class='InvField'>Venue Fee</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sFacilityAmt + "</td>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sFacilityTaxPct + "</td>\n" : "") +
				WebPage.Tabs(1) + "</tr>\n";
		}
		if (sRentalsAmt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td align='right'>" + sRentalsSubTotPct + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" :
				WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
				WebPage.Tabs(2) + "<td class='InvField'>Rentals " + (RentalsDesc.Length > 0 ? string.Format(" ({0})", RentalsDesc) : "") + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right' valign='top'>" + sRentalsAmt + "</td>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sRentalsTaxPct + "</td>\n" : "") +
				WebPage.Tabs(1) + "</tr>\n";
		}
        if (sGratuityAmt.Length > 0 && !fDepositInvoice)
        {
            Html +=
                WebPage.Tabs(1) + "<tr>\n" +
                (fFileCopy ?
                WebPage.Tabs(2) + "<td align='right'>" + sGratuitySubTotPct + "</td>\n" +
                WebPage.Tabs(2) + "<td></td>\n" :
                WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
                WebPage.Tabs(2) + "<td class='InvField'>Gratuity (for employee service)</td>\n" +
                WebPage.Tabs(2) + "<td></td>\n" +
                WebPage.Tabs(2) + "<td align='right'>" + sGratuityAmt + "</td>\n" +
                (fFileCopy ?
                WebPage.Tabs(2) + "<td></td>\n" +
                WebPage.Tabs(2) + "<td align='right'>" + sGratuityTaxPct + "</td>\n" : "") +
                WebPage.Tabs(1) + "</tr>\n";
        }
        if (sVoluntaryGratuityAmt.Length > 0 && !fDepositInvoice)
        {
            Html +=
                WebPage.Tabs(1) + "<tr>\n" +
                (fFileCopy ?
                WebPage.Tabs(2) + "<td colspan='2'></td>\n" :
                WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
                WebPage.Tabs(2) + "<td class='InvField'>Voluntary Gratuity (optional for employee service)</td>\n" +
                WebPage.Tabs(2) + "<td></td>\n" +
                WebPage.Tabs(2) + "<td align='right'>" + sVoluntaryGratuityAmt + "</td>\n" +
                WebPage.Tabs(1) + "</tr>\n";
        }
        if (sAdj1Amt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td align='right'>" + sAdj1SubTotPct + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" :
				WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
				WebPage.Tabs(2) + "<td class='InvField'>" + sAdj1Desc + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sAdj1Amt + "</td>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sAdj1TaxPct + "</td>\n" : "") +
				WebPage.Tabs(1) + "</tr>\n";
		}
		if (sAdj2Amt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td align='right'>" + sAdj2SubTotPct + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" :
				WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
				WebPage.Tabs(2) + "<td class='InvField'>" + sAdj2Desc + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sAdj2Amt + "</td>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sAdj2TaxPct + "</td>\n" : "") +
				WebPage.Tabs(1) + "</tr>\n";
		}
		if (sCCFeeAmt.Length > 0)
		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				(fFileCopy ?
				WebPage.Tabs(2) + "<td align='right'>" + sCCFeePct + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" :
				WebPage.Tabs(2) + "<td colspan='2'></td>\n") +
				WebPage.Tabs(2) + "<td class='InvField'>Credit Card Fee</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sCCFeeAmt + "</td>\n" +				
				(fFileCopy ?
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sCCFeeTaxPct + "</td>\n" : "") +
				WebPage.Tabs(1) + "</tr>\n";
		}

		{
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
				WebPage.Tabs(2) + "<td colspan='3'><div class='hr' /></td>\n" +
				WebPage.Tabs(1) + "</tr>\n" +
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
				WebPage.Tabs(2) + "<td class='InvField'>Pre-Tax Total</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sPreTaxSubtotal + "</td>\n" +
				WebPage.Tabs(1) + "</tr>\n";
		}

		Html +=
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
			WebPage.Tabs(2) + "<td class='InvField'>Utah State Food Tax" + (fTaxExempt ? " (Tax Exempt)" : "") + "</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right'>" + sSalesTax + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 10) +
			WebPage.Tabs(1) + "</tr>\n" +
			//WebPage.Tabs(1) + "<tr>\n" +
			//WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
			//WebPage.Tabs(2) + "<td colspan='3' class='InvTotal' style='padding-top: 8px;' align='right'>\n" +
			//WebPage.Tabs(3) + WebPage.Table() +
			//WebPage.Tabs(4) + "<tr>\n" +
			//WebPage.Tabs(5) + "<td align='right'><b>Invoice Total</b></td>\n" +
			//WebPage.Tabs(5) + WebPage.Space(10, 1) +
			//WebPage.Tabs(5) + "<td align='right'><b>" + sTotal + "</b></td>\n" +
			//WebPage.Tabs(4) + "</tr>\n" +
			//WebPage.Tabs(3) + WebPage.TableEnd() +
			//WebPage.Tabs(2) + "</td>\n" +
			//WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
			WebPage.Tabs(2) + "<td class='InvField' style='font-size: 12pt; padding: 8px 0px 2px 0px;'><b>Invoice Total</b></td>\n" +
			WebPage.Tabs(3) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td align='right' style='font-size: 12pt; padding: 8px 0px 2px 0px;'><b>" + sTotal + "</b></td>\n" +
			WebPage.Tabs(1) + "</tr>\n";
        if (fDepositInvoice)
        {
            Html +=
                WebPage.Tabs(1) + "<tr>\n" +
                WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
                WebPage.Tabs(2) + "<td class='InvField' style='font-size: 12pt; padding: 8px 0px 2px 0px;'><b>Deposit Payment Due</b></td>\n" +
                WebPage.Tabs(3) + WebPage.Space(10, 1) +
                WebPage.Tabs(2) + "<td align='right' style='font-size: 12pt; padding: 8px 0px 2px 0px;'><b>" + sDepositAmt + "</b></td>\n" +
                WebPage.Tabs(1) + "</tr>\n";
        }

        bool fPayments = false;

        string Sql = string.Format("Select Type, Amount, Reference From Payments Where JobRno = {0} Order By Seq", JobRno);
        try
        {
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow drPmt in dt.Rows)
            {
                string PmtType = DB.Str(drPmt["Type"]);
                decimal PmtAmt = DB.Dec(drPmt["Amount"]);
                string PmtRef = DB.Str(drPmt["Reference"]);

                if (PmtAmt != 0)
                {
                    fPayments = true;
                    Html +=
                        WebPage.Tabs(1) + "<tr>\n" +
                        WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
                        WebPage.Tabs(2) + "<td class='InvField'>" + PmtType + ": " + PmtRef + "</td>\n" +
                        WebPage.Tabs(3) + WebPage.Space(10, 1) +
                        WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dollar(PmtAmt) + "</td>\n" +
                        WebPage.Tabs(1) + "</tr>\n";
                }
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
        //if (PmtAmt1 != 0)
        //{
        //	Html +=
        //		WebPage.Tabs(1) + "<tr>\n" +
        //		WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
        //		WebPage.Tabs(2) + "<td class='InvField'>" + PmtType1 + ": " + PmtRef1 + "</td>\n" +
        //		WebPage.Tabs(3) + WebPage.Space(10, 1) +
        //		WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dollar(PmtAmt1) + "</td>\n" +
        //		WebPage.Tabs(1) + "</tr>\n";
        //}
        //if (PmtAmt2 != 0)
        //{
        //	Html +=
        //		WebPage.Tabs(1) + "<tr>\n" +
        //		WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
        //		WebPage.Tabs(2) + "<td class='InvField'>" + PmtType2 + ": " + PmtRef2 + "</td>\n" +
        //		WebPage.Tabs(3) + WebPage.Space(10, 1) +
        //		WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dollar(PmtAmt2) + "</td>\n" +
        //		WebPage.Tabs(1) + "</tr>\n";
        //}
        //if (PmtAmt3 != 0)
        //{
        //	Html +=
        //		WebPage.Tabs(1) + "<tr>\n" +
        //		WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
        //		WebPage.Tabs(2) + "<td class='InvField'>" + PmtType3 + ": " + PmtRef3 + "</td>\n" +
        //		WebPage.Tabs(3) + WebPage.Space(10, 1) +
        //		WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dollar(PmtAmt3) + "</td>\n" +
        //		WebPage.Tabs(1) + "</tr>\n";
        //}
        //if (PmtAmt1 != 0 || PmtAmt2 != 0 || PmtAmt3 != 0)
        if (fPayments)
        {
			Html +=
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
				WebPage.Tabs(2) + "<td class='InvField' style='font-size: 12pt; padding: 8px 0px 2px 0px;'><b>Invoice Balance</b></td>\n" +
				WebPage.Tabs(3) + WebPage.Space(10, 1) +
				WebPage.Tabs(2) + "<td align='right' style='font-size: 12pt; padding: 8px 0px 2px 0px;'><b>" + sBalance + "</b></td>\n" +
				WebPage.Tabs(1) + "</tr>\n";
		}

        if (sGratuityAmt.Length == 0 && sVoluntaryGratuityAmt.Length == 0 && !fDepositInvoice)
        {
            Html += string.Format(
                WebPage.Tabs(1) + "<tr><td colspan='5' style='height: 32px;'></td></tr>\n" +
                WebPage.Tabs(1) + "<tr>\n" +
                WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
                WebPage.Tabs(2) + "<td class='InvField'>Adjusted Total with <b>15% gratuity</b> ({0}):</td>\n" +
                WebPage.Tabs(2) + "<td></td>\n" +
                WebPage.Tabs(2) + "<td align='right'>{1}</td>\n" +
                WebPage.Tabs(1) + "</tr>\n",
                sGratuity15,
                sGratuity15Balance);

            Html += string.Format(
                WebPage.Tabs(1) + "<tr>\n" +
                WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
                WebPage.Tabs(2) + "<td class='InvField'>Adjusted Total with <b>20% gratuity</b> ({0}):</td>\n" +
                WebPage.Tabs(2) + "<td></td>\n" +
                WebPage.Tabs(2) + "<td align='right'>{1}</td>\n" +
                WebPage.Tabs(1) + "</tr>\n",
                sGratuity20,
                sGratuity20Balance);
        }

		Html +=
			WebPage.Tabs(0) + WebPage.TableEnd() +
			//WebPage.Tabs(0) + WebPage.Table("align='center' width='60%'") +
			//WebPage.Tabs(1) + "<tr>\n" +
			//WebPage.Tabs(2) + WebPage.Space(1, 20) +
			//WebPage.Tabs(1) + "</tr>\n" +
			//WebPage.Tabs(1) + "<tr>\n" +
			//WebPage.Tabs(2) + "<td class='NoteBox' align='center'>" + DB.Str(dr["InvoiceMsg"]) + "</td>\n" +
			//WebPage.Tabs(1) + "</tr>\n" +
			//WebPage.Tabs(0) + WebPage.TableEnd() +
			WebPage.Tabs(0) + "<div style='width: 700px; height: 34px; border: solid 2px #667B3E; margin: 0px auto; position: relative; top: -" + (sGratuityAmt.Length == 0 && sVoluntaryGratuityAmt.Length == 0 && !fDepositInvoice ? 108 : 32) + "px;'>\n" +
			WebPage.Tabs(0) + "<div style='font-weight: bold; font-size: 11pt; float: left; margin-left: 10px; line-height: 35px;'><span style='font-size: 8pt;'>Invoice #</span>" + DB.Int32(dr["JobRno"]) + "</div>\n";

		if (PmtTerms.Length > 0)
		{
			Html += WebPage.Tabs(0) + "<div style='font-weight: bold; font-size: 11pt; float: right; margin-right: 10px; line-height: 35px;'>" + PmtTerms + "</div>\n";
		}
		Html += WebPage.Tabs(0) + "</div>\n";

		//if (!fFileCopy)
		//{
		//    Html +=
		//        WebPage.Tabs(0) + WebPage.Table("align='center'") +
		//        WebPage.Tabs(1) + WebPage.SpaceTr(1, 10) +
		//        WebPage.Tabs(1) + WebPage.HorizRule(1, "250") +
		//        WebPage.Tabs(1) + WebPage.SpaceTr(1, 10) +
		//        WebPage.Tabs(1) + "<tr>\n" +
		//        WebPage.Tabs(2) + "<td>Please call Jeannine, (801) 374-0879, if you have any questions regarding this invoice.</td>\n" +
		//        WebPage.Tabs(1) + "</tr>\n" +
		//        WebPage.Tabs(0) + WebPage.TableEnd();
		//}

		Html +=
			"</div>\n" +
			"<div style='width: 700px; margin: 0px auto;'>\n";
		if (fFileCopy)
		{
			Html +=

				"<div style='border: solid 1px #667B3E; width: 300px; height: 119px; padding: 10px; float: left; font-size: 9pt;'>\n" +
				"Payment Method:<br />\n" +
				"<div class='Payment'><div><input type='checkbox' />Credit Card #&nbsp;</div><div></div></div>\n" +
				"<div class='Payment'><div class='Sub'>Exp. Date&nbsp;</div><div></div></div>\n" +
				"<div class='Payment'><div class='Sub'>Security Code&nbsp;</div><div></div></div>\n" +
				"<div class='Payment'><div class='Sub'>Billing Zip Code&nbsp;</div><div></div></div>\n" +
				"<div><input type='checkbox' />Cash</div>\n" +
				"<div class='Payment'><div><input type='checkbox' />Check #&nbsp;</div><div></div></div>\n" +
				"</div>\n";
		}
		Html +=
			"<div style='border: solid 1px #667B3E; height: 119px; font-size: 9pt; padding: 10px;" + (fFileCopy ? " float: right; text-align: right;" : " text-align: center;") + "'>\n" +
			"<p style='font-weight: bold; font-size: 11pt; margin: 0px; margin-top: 8px;'>Please make check payable to:<br />" + g.Company.Name + "</p>\n" +
			"<p style='font-weight: bold; margin: 10px 0px 0px 0px;'>Please reference Invoice #" + DB.Int32(dr["JobRno"]) + " on your check.</p>\n" +
            "<p style='margin: 0px 0px 10px 0px;'>765 W. Columbia Lane, Provo, UT 84604</p>\n" +
			"<p style='margin: 0px;'>For billing inquiries please call (801) 374-0879</p>\n" +
			"</div>\n" +
			"</div>\n";

		Response.Write(Html);
	}

//	private string AdditionalPrices(ref decimal Subtotal, decimal SubtotalTaxPct)
	private string AdditionalPrices(decimal SubtotalTaxPct)
	{
		string html = "";
		DataTable dt = db.DataTable("Select * From JobInvoicePrices Where JobRno = " + JobRno + " Order By Seq");
		//if (dt.Rows.Count > 0)
		//{
			//Subtotal = 0;
		//}

		foreach (DataRow drPrice in dt.Rows)
		{
			string PriceType = DB.Str(drPrice["PriceType"]);
			int Count = 0;
			decimal Price = 0;
			decimal ExtPrice = 0;
			string Desc = "";
			string ExtDesc = "";

			switch (PriceType)
			{
				case Misc.cnPerPerson:
					Count = DB.Int32(drPrice["InvPerPersonCount"]);
					Price = DB.Dec(drPrice["InvPerPersonPrice"]);
					//ExtPrice = Count * Price;
					ExtPrice = DB.Dec(drPrice["InvPerPersonExtPrice"]);
					Desc = DB.Str(drPrice["InvPerPersonDesc"]);
					ExtDesc = string.Format("{0} {1} @ {2} per person",
						Fmt.Num(Count),
						Desc,
						Fmt.Dollar(Price));
					break;

				case Misc.cnPerItem:
					Count = DB.Int32(drPrice["InvPerItemCount"]);
					Price = DB.Dec(drPrice["InvPerItemPrice"]);
					//ExtPrice = Count * Price;
					ExtPrice = DB.Dec(drPrice["InvPerItemExtPrice"]);
					Desc = DB.Str(drPrice["InvPerItemDesc"]);
					ExtDesc = string.Format("{0} {1} @ {2} per item",
						Fmt.Num(Count),
						Desc,
						Fmt.Dollar(Price));
					break;

				case Misc.cnAllIncl:
					Count = 1;
					Price = DB.Dec(drPrice["InvAllInclPrice"]);
					//ExtPrice = Price - Price * (1 - 1 / (1 + SubtotalTaxPct / 100));
					ExtPrice = DB.Dec(drPrice["InvAllInclExtPrice"]);
					Desc = DB.Str(drPrice["InvAllInclDesc"]);
					ExtDesc = string.Format("All inclusive {0} @ {1}",
						Desc,
						Fmt.Dollar(Price));
					break;

				default:
					break;
			}

			html +=
				WebPage.Tabs(1) + "<tr>\n" +
				WebPage.Tabs(2) + "<td colspan='2'></td>\n" +
				WebPage.Tabs(2) + "<td class='InvField'>" + ExtDesc + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dollar(ExtPrice) + "</td>\n" +
				WebPage.Tabs(1) + "</tr>\n";
			//Subtotal += ExtPrice;
		}
		return html;
	}

	protected void PaymentInfo(ref DataRow dr)
	{
		Response.Write(
			WebPage.Tabs(0) + WebPage.Table("width='100%'") +
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 20) +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td style='border-top: black 1px dotted'><img width='1' height='1' src='Images/Space.gif'></td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 20) +
			WebPage.Tabs(0) + WebPage.TableEnd() +
			WebPage.Tabs(0) + WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td>\n" +
			WebPage.Tabs(3) + WebPage.Table() +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td colspan='2'>Please submit payment to:</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + WebPage.Space(20, 1) +
			WebPage.Tabs(5) + "<td>" + g.Company.LegalName + "</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
            WebPage.Tabs(5) + "<td>" + g.Company.Addr1 + "</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
            WebPage.Tabs(5) + "<td>" + g.Company.Addr2 + "</td>\n" +
            WebPage.Tabs(4) + "</tr>\n" +
            WebPage.Tabs(4) + "<tr>\n" +
            WebPage.Tabs(5) + "<td></td>\n" +
            WebPage.Tabs(5) + "<td>" + g.Company.Addr3 + "</td>\n" +
            WebPage.Tabs(4) + "</tr>\n" +
            WebPage.Tabs(3) + WebPage.TableEnd() +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(2) + WebPage.Space(150, 1) +
			WebPage.Tabs(2) + "<td>\n" +
			WebPage.Tabs(3) + WebPage.Table() +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + WebPage.Space(1, 1, "PmtTotal") +
			WebPage.Tabs(5) + WebPage.Space(10, 1) +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td align='center' colspan='3'><b>" + DB.Str(dr["Customer"]) + "</b></td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td align='right'>Event Date:</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td>" + Fmt.Dt(DB.DtTm(dr["JobDate"])) + "</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td align='right'>Invoice #:</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td>" + JobRno + "</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td style='border: black 1px solid; padding: 2px;' colspan='3'>\n" +
			WebPage.Tabs(6) + WebPage.Table() +
			WebPage.Tabs(7) + "<tr>\n" +
			WebPage.Tabs(8) + WebPage.Space(1, 1, "PmtTotal") +
			WebPage.Tabs(8) + WebPage.Space(10, 1) +
			WebPage.Tabs(8) + "<td></td>\n" +
			WebPage.Tabs(7) + "</tr>\n" +
			WebPage.Tabs(7) + "<tr>\n" +
			WebPage.Tabs(8) + "<td align='right'><b>Invoice Total</b></td>\n" +
			WebPage.Tabs(8) + "<td></td>\n" +
			WebPage.Tabs(8) + "<td><b>" + Fmt.Dollar(Total) + "</b></td>\n" +
			WebPage.Tabs(7) + "</tr>\n" +
			WebPage.Tabs(6) + WebPage.TableEnd() +
			WebPage.Tabs(5) + "</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(3) + WebPage.TableEnd() +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 10) +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td align='center' colspan='3'>Please make check payable to \"" + g.Company.LegalName + ".</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(2) + WebPage.HorizRule(3, "250") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='3'>\n" +
			WebPage.Tabs(3) + WebPage.Table() +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + WebPage.Space(5, 1) +
			WebPage.Tabs(5) + WebPage.Space(200, 1) +
			WebPage.Tabs(5) + WebPage.Space(20, 1) +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + WebPage.Space(5, 1) +
			WebPage.Tabs(5) + WebPage.Space(70, 1) +
			WebPage.Tabs(5) + WebPage.Space(20, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(5) + WebPage.Space(5, 1) +
			WebPage.Tabs(5) + WebPage.Space(40, 1) +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td colspan='11'>To pay by credit card, please provide the following information:</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + WebPage.Space(1, 10) +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td colspan='11'>\n" +
			WebPage.Tabs(6) + "CC Type: \n" +
			WebPage.Tabs(6) + "<img src='Images/Box.gif' align='absBottom'>Visa&nbsp;\n" +
			WebPage.Tabs(6) + "<img src='Images/Box.gif' align='absBottom'>Mastercard &nbsp;\n" +
			WebPage.Tabs(6) + "<img src='Images/Box.gif' align='absBottom'>American Express &nbsp; \n" +
			WebPage.Tabs(6) + "<img src='Images/Box.gif' align='absBottom'>Discover\n" +
			WebPage.Tabs(5) + "</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + WebPage.Space(1, 5) +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td align='right'>Card #</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td class='Underline'>&nbsp;</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td align='right'>Expires</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td class='Underline'>&nbsp;</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td>Security Code</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td class='Underline'>&nbsp;</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + WebPage.Space(1, 5) +
			WebPage.Tabs(5) + "<td colspan='7'></td>\n" +
			WebPage.Tabs(5) + "<td colspan='3' rowspan='4' valign='top'>(usually a 3 digit # on<br>the back of the card)</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td align='right'>Name on Card</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td class='Underline'>&nbsp;</td>\n" +
			WebPage.Tabs(5) + "<td colspan='5'></td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + WebPage.Space(1, 5) +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + "<td align='right'>Billing Street Addr</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td class='Underline'>&nbsp;</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td align='right'>Zip Code</td>\n" +
			WebPage.Tabs(5) + "<td></td>\n" +
			WebPage.Tabs(5) + "<td class='Underline'>&nbsp;</td>\n" +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(4) + "<tr>\n" +
			WebPage.Tabs(5) + WebPage.Space(1, 20) +
			WebPage.Tabs(4) + "</tr>\n" +
			WebPage.Tabs(3) + WebPage.TableEnd() +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd() +
			WebPage.Tabs(0) + WebPage.Table("class='NoteBox' width='60%' align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td align='center'>" + DB.Str(dr["PaymentMsg"]) + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd()
			);
	}

	protected void PaymentMade(ref DataRow dr)
	{
		string sPmtMethod = DB.Str(dr["InvPmtMethod"]);
		string sPmtTypeTitle = "";
		string sPmtType = "";
		string sPmtRefTitle = "";
		string sPmtRef = "";
		bool fCCDeclined = false;

		switch (sPmtMethod)
		{
			case "CC":
				sPmtTypeTitle = "CC Type:";
				sPmtType = DB.Str(dr["CCType"]);
				sPmtRefTitle = "CC #:";
				string CCNum = DB.Str(dr["CCNum"]);
				sPmtRef = "xxxx" + CCNum.Substring(CCNum.Length - 4);
				fCCDeclined = DB.Bool(dr["CCFailedFlg"]);
				break;

			case "Check":
				sPmtTypeTitle = "Payment Method:";
				sPmtType = sPmtMethod;
				sPmtRefTitle = "Check #:";
				sPmtRef = DB.Str(dr["CheckRef"]);
				break;

			default:
				sPmtTypeTitle = "Payment Method:";
				sPmtType = sPmtMethod;
				break;
		}

		string Html =
			WebPage.Tabs(0) + WebPage.Table("width='100%'") +
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 20) +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td style='border-top: black 1px solid'><img width='1' height='1' src='Images/Space.gif'></td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 20) +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd() +
			WebPage.Tabs(0) + WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(40, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(40, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(40, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='15'>Payment Received:</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 5) +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td align='right'>Date:</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td>" + Fmt.Dt(DB.DtTm(dr["InvPmtDate"])) + "</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right'>" + sPmtTypeTitle + "</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td>" + sPmtType + "</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n";

		if (sPmtRefTitle.Length > 0)
		{
			Html +=
				WebPage.Tabs(2) + "<td align='right'>" + sPmtRefTitle + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td>" + sPmtRef + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n";
		}
		if (fCCDeclined)
		{
			Html +=
				WebPage.Tabs(2) + "<td colspan='3'><b>Credit Card Declined</b></td>\n";
		}
		else
		{
			Html +=
				WebPage.Tabs(2) + "<td align='right'>Amount:</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td>" + Fmt.Dollar(Total) + "</td>\n";
		}

		Html +=
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 15) +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd() +
			WebPage.Tabs(0) + WebPage.Table("class='NoteBox' width='60%' align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td align='center'>" + DB.Str(dr["PaymentMsg"]) + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd();
		Response.Write(Html);
	}

	protected void FileCopyPaymentInfo(ref DataRow dr)
	{
		string sInvDeliveryDate = Fmt.Dt(DB.DtTm(dr["InvDeliveryDate"]));
		string DeliveryMethod = DB.Str(dr["InvDeliveryMethod"]);
		string sInvDeliveryMethod = DeliveryMethod;
		string sEmailFaxTitle = "";
		string sEmailFax = "";

		switch (DeliveryMethod)
		{
			case "Email":
				sEmailFaxTitle = DeliveryMethod + ":";
				sEmailFax = DB.Str(dr["InvEmail"]);
				break;

			case "Fax":
				sEmailFaxTitle = DeliveryMethod + ":";
				sEmailFax = DB.Str(dr["InvFax"]);
				break;
		}

		string sInvPmtDate = Fmt.Dt(DB.DtTm(dr["InvPmtDate"]));
		string PmtMethod = DB.Str(dr["InvPmtMethod"]);

		string sPmtNote = "";
		if (sInvPmtDate.Length == 0)
		{
			sPmtNote = "No Payment Date";
		}

		bool fAllCCInfo = false;
		string sPmtTypeTitle = "";
		string sPmtType = "";
		string sPmtRefTitle = "";
		string sPmtRef = "";
		string sCCExpDt = "";
		string sCCResultTitle = "";
		string sCCResult = "";

		switch (PmtMethod)
		{
			case "CC":
				sPmtTypeTitle = "CC Type:";
				sPmtType = DB.Str(dr["CCType"]);
				sPmtRefTitle = "CC #:";
				string CCNum = DB.Str(dr["CCNum"]);
				sPmtRef = "xxxx" + CCNum.Substring(CCNum.Length - 4);
				sCCExpDt = DB.Str(dr["CCExpDt"]);
				fAllCCInfo = true;
				if (DB.Bool(dr["CCFailedFlg"]))
				{
					sCCResultTitle = "Failed Msg:";
					sCCResult = DB.Str(dr["CCFailedMsg"]);
					sPmtNote = "Failed Credit Card Payment";
				}
				else
				{
					sCCResultTitle = "Approval:";
					sCCResult = DB.Str(dr["CCApprCode"]);
				}
				break;

			case "Check":
				sPmtTypeTitle = "Payment Method:";
				sPmtType = PmtMethod;
				sPmtRefTitle = "Check #:";
				sPmtRef = DB.Str(dr["CheckRef"]);
				break;

			default:
				sPmtTypeTitle = "Payment Method:";
				sPmtType = PmtMethod;
				break;
		}

		string sPaymentMsg = DB.Str(dr["PaymentMsg"]);


		string Html =
			WebPage.Tabs(0) + WebPage.Table("width='100%'") +
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 10) +
			WebPage.Tabs(1) + WebPage.HorizRule(1, "70%") +
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 10) +
			WebPage.Tabs(0) + WebPage.TableEnd() +
			WebPage.Tabs(0) + WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(40, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n";

		if (sEmailFaxTitle.Length > 0)
		{
			Html +=
				WebPage.Tabs(2) + WebPage.Space(40, 1) +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + WebPage.Space(10, 1) +
				WebPage.Tabs(2) + "<td></td>\n";
		}

		Html +=
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='7'>Invoice Delivery:</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 5) +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td>Date:</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td>" + sInvDeliveryDate + "</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td>Method:</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td>" + sInvDeliveryMethod + "</td>\n";

		if (sEmailFaxTitle.Length > 0)
		{
			Html +=
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td>" + sEmailFaxTitle + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td>" + sEmailFax + "</td>\n";
		}

		Html +=
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd() +
			WebPage.Tabs(0) + WebPage.Table("width='100%' align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 10) +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td><hr width='70%'></td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 10) +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd() +
			WebPage.Tabs(0) + WebPage.Table("align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(30, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(30, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(30, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(30, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='3'>Payment Info:</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td colspan='8' class='InvPmtNote'>" + sPmtNote + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 5) +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td align='right'>Date:</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td>" + sInvPmtDate + "</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right'>" + sPmtTypeTitle + "</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td>" + sPmtType + "</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right'>" + sPmtRefTitle + "</td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td>" + sPmtRef + "</td>\n";

		if (fAllCCInfo)
		{
			Html +=
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>Expires:</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td>" + sCCExpDt + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td align='right'>" + sCCResultTitle + "</td>\n" +
				WebPage.Tabs(2) + "<td></td>\n" +
				WebPage.Tabs(2) + "<td>" + sCCResult + "</td>\n";
		}

		Html +=
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + WebPage.Space(1, 15) +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd() +
			WebPage.Tabs(0) + WebPage.Table("class='NoteBox' width='60%' align='center'") +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td align='center'>" + sPaymentMsg + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd();

		Response.Write(Html);
	}

	protected void btnJobsNotInv_Click(object sender, System.EventArgs e)
	{
		dtEnd = Convert.ToDateTime(txtJobsNotInv.Text);
		fReport = true;
		fJobsNotInv = true;
	}

	private void JobsNotInv()
	{
		JobsNotInvTitles();

		string Sql =
			"Select * From mcJobs " +
			"Where CancelledDtTm Is Null " +
			"And InvCreatedDtTm Is Null " +
			"And JobDate <= " + DB.PutDtTm(dtEnd) + " " +
			"Order By JobDate, Customer";

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				int Servings = DB.Int32(dr["NumMenServing"])
										+ DB.Int32(dr["NumWomenServing"])
										+ DB.Int32(dr["NumChildServing"]);
				decimal PricePerPerson = DB.Dec(dr["PricePerPerson"]);
				decimal Subtotal = Servings * PricePerPerson;

				Response.Write(
					WebPage.Tabs(1) + "<tr>\n" +
					WebPage.Tabs(2) + "<td align='center'>" + Fmt.Dt(DB.DtTm(dr["JobDate"])) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td>" + DB.Str(dr["Customer"]) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td>" + DB.Str(dr["EventType"]) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td>" + DB.Str(dr["ServiceType"]) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Num(Servings, false) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dollar(PricePerPerson, false) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right'>" + Fmt.Dollar(Subtotal, false) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td align='right' class='SmallPrint'>" + DB.Int32(dr["JobRno"]) + "</td>\n" +
					WebPage.Tabs(2) + "<td></td>\n" +
					WebPage.Tabs(2) + "<td class='SmallPrint'>" + DB.Str(dr["Status"]) + "</td>\n" +
					WebPage.Tabs(1) + "</tr>\n"
				);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		Response.Write(
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 20) +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td colspan='17' align='center' style='border: solid 1px #aaaaaa; padding: 2px;'>\n" +
			WebPage.Tabs(3) + "To make sure a job is invoiced, look it up from the Job screen, select \n" +
			WebPage.Tabs(3) + "the Invoice tab, enter the invoice data and save.\n" +
			WebPage.Tabs(2) + "</td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(0) + WebPage.TableEnd()
		);

	}

	private void JobsNotInvTitles()
	{
		Response.Write(
			WebPage.Table("align='center'") +
			WebPage.Tabs(2) + "<td align='center'>\n" +
			WebPage.Tabs(3) + WebPage.Div("Jobs Not Invoiced", "center", "RptTitle") +
			WebPage.Tabs(3) + WebPage.Div("Thru " + Fmt.Dt(dtEnd), "center", "RptSubTitle") +
			WebPage.Tabs(2) + "</td>\n" +
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
			WebPage.Tabs(2) + WebPage.Space(10, 1) +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(1) + "</tr>\n" +
			WebPage.Tabs(1) + "<tr><td colspan='17' align='right' id='Totals'></td></tr>\n" +
			WebPage.Tabs(1) + WebPage.SpaceTr(1, 10) +
			WebPage.Tabs(1) + "<tr>\n" +
			WebPage.Tabs(2) + "<td align='center'><b>Date</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td><b>Customer</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td><b>Event Type</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td><b>Service Type</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right'><b># Servings</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right'><b>$/Person</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right'><b>Subtotal</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td align='right' class='SmallPrint'><b>Job #</b></td>\n" +
			WebPage.Tabs(2) + "<td></td>\n" +
			WebPage.Tabs(2) + "<td class='SmallPrint'><b>Status</b></td>\n" +
			WebPage.Tabs(1) + "</tr>\n"
		);
	}
}