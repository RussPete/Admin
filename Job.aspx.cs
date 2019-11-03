using System;
using System.Collections;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Utils;
using Globals;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class Job : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;
	protected string FocusField = "";
	protected int JobRno;
	//protected SelectList slCustomer;
	//protected string[] aCustomers;
	protected SelectList slContact;
	//protected SelectList slEventType;
	protected string[] aEventTypes;
	//protected SelectList slServiceType;
	protected string[] aServiceTypes;
	//protected SelectList slLocation;
	//protected SelectList slVehicle;
	protected string[] aVehicles;
	//protected SelectList slCarts;
	protected Utils.Calendar calJobDate;
	protected bool fNew;
	protected string Warn;
	protected int InvEventCount;
	protected decimal InvPricePerPerson;
	protected bool fQtyWarning = false;
	protected bool fProposal = false;
    protected string SaveOrigStartDateTime;

    private void Page_Init(object sender, System.EventArgs e)
	{
		Pg = new WebPage("Images");
		Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

		SelectListsSetup();

		//calJobDate = new Utils.Calendar("JobDate", ref txtJobDate);
		//calJobDate.ImageButton(imgJobDate);

		fNew = true;
	}

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		lblInfo.Text = "";

		JobRno = Str.Num((String)Session["JobRno"]);
		if (JobRno == 0)
		{
			FocusField = "txtCustomer";
		}

		// Put user code to initialize the page here
		if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.Jobs.Title;

            int QueryJobRno = Str.Num((string)Request.QueryString["JobRno"]);
			if (QueryJobRno != 0)
			{
				JobRno = QueryJobRno;
			}

			Setup();
		}

		SelectListsData();
    }

    private void Page_PreRender(object sender, System.EventArgs e)
	{
		//btnCopy.Enabled =
		//btnDelete.Enabled = (Str.Num(txtJobRno.Text) > 0);

		//btnSave.Enabled = (txtCustomer.Text.Length > 0 && txtJobDate.Text.Length > 0);

		trMsg.Visible = (lblMsg.Text.Length > 0);
	}

	private void Setup()
	{
		//lblInfo.Text += "<br />Setup " + Session["Customer"];

		bool fAddlSearch = DB.Bool(Session["AddlSearch"]);		
		hfAddlSearch.Value = fAddlSearch.ToString();
		txtSearchCustomer.Text = DB.Str(Session["Customer"]);
		txtSearchContact.Text = DB.Str(Session["Contact"]);
		txtSearchPhone.Text = DB.Str(Session["Phone"]);
		txtSearchEmail.Text = DB.Str(Session["Email"]);
		txtSearchLocation.Text = DB.Str(Session["Location"]);
		txtSearchDate.Text = Fmt.Dt(Str.DtTm(Session["Date"]));

		switch ((String)Session["JobsSortedBy"])
		{
			case "Date":
			default:
				rbSortDate.Checked = true;
				break;

			case "Name":
				rbSortName.Checked = true;
				break;
		}

		chkCancelledJobs.Checked = DB.Bool(Session["CancelledJobs"]);
		chkPastJobs.Checked = DB.Bool(Session["PastJobs"]);
		chkJobs.Checked = DB.Bool(Session["Jobs"]);
		chkProposals.Checked = DB.Bool(Session["Proposals"]);

		if (!chkJobs.Checked && !chkProposals.Checked)
		{
			chkJobs.Checked = true;
		}

		ClearValues();
		LoadList(JobRno);

		//lstList.Attributes.Add("onChange", "lstListOnChange();");
		lstList.Attributes.Add("onClick", "lstListOnChange();");	// Chrome ver 27 bug
		//txtCustomer.Attributes.Add("onChange", "CheckRequiredField('txtCustomer', 'Customer');");
		txtEmail.Attributes.Add("onChange", "SetTitle('txtEmail', this.value);");
		txtLocation.Attributes.Add("onChange", "SetTitle('txtLocation', this.value);");
        //txtConfirmDeadlineDate.Attributes.Add("onChange", "ValidateDate(this);");
		txtJobDate.Attributes.Add("onChange", "JobDateChange('txtJobDate');");
        txtLoadTime.Attributes.Add("onChange", "ValidateTime(this);");
		txtDepartTime.Attributes.Add("onChange", "ValidateTime(this);");
		txtGuestArrival.Attributes.Add("onChange", "ValidateTime(this);");
        txtWeddingTime.Attributes.Add("onChange", "ValidateTime(this)");
		txtArrivalTime.Attributes.Add("onChange", "ValidateTime(this);");
		txtMealTime.Attributes.Add("onChange", "ValidateTime(this);");
		txtEndTime.Attributes.Add("onChange", "ValidateTime(this);");
		txtServingMen.Attributes.Add("onChange", "CheckEventCount();");
		//txtServingWomen.Attributes.Add("onChange", "CheckEventCount();");
		//txtServingChild.Attributes.Add("onChange", "CheckEventCount();");
		txtDemographics.Attributes.Add("onChange", "CheckEventCount();");
		txtPricePerPerson.Attributes.Add("onChange", "CheckPricePerPerson();");
		rbPaymentType.Attributes.Add("onClick", "");
		txtJobNotes.Attributes.Add("onChange", "");
        txtProdNotes.Attributes.Add("onChange", "");
		//chkCompleted.Attributes.Add("onClick", "SetStatus();");
		//chkConfirmed.Attributes.Add("onClick", "SetStatus();");
		//chkPrinted.Attributes.Add("onClick", "SetStatus();");
		//chkInvoiced.Attributes.Add("onClick", "SetStatus();");
		//chkCancelled.Attributes.Add("onClick", "SetStatus();");

		//btnSave.Attributes.Add("onClick", "return CanSave();");
		//btnDelete.Attributes.Add("onClick", "return window.confirm('Delete this Job?');");

		if (JobRno > 0)
		{
			ReadValues(JobRno);
		}
	}

	private void SelectListsSetup()
	{
		SelectList.Clear();

		//slCustomer = new SelectList("Customer", ref txtCustomer);
		//slCustomer.ImageButton(ddCustomer);
		//slCustomer.AutoPostBack = true;

		slContact = new SelectList("Contact", ref txtContact);
		slContact.ImageButton(ddContact);
		slContact.AutoPostBack = true;

		//slEventType = new SelectList("EventType", ref txtEventType);
		//slEventType.ImageButton(ddEventType);

		//slServiceType = new SelectList("ServiceType", ref txtServiceType);
		//slServiceType.ImageButton(ddServiceType);

		//slLocation = new SelectList("Location", ref txtLocation);
		//slLocation.ImageButton(ddLocation);
		//slLocation.AutoPostBack = true;

		//slVehicle = new SelectList("Vehicle", ref txtVehicle);
		//slVehicle.ImageButton(ddVehicle);

		//slCarts = new SelectList("Carts", ref txtCarts);
		//slCarts.ImageButton(ddCarts);
	}

	private void SelectListsData()
	{
		String Sql;

		//Sql = "Select Distinct Customer From mcJobs Where Customer <> '' Order By Customer";
		//slCustomer.ClearValues();
		//slCustomer.AddDBValues(db, Sql);
		//aCustomers = db.StrArray(Sql);

		//Sql = "Select Distinct ContactName From mcJobs Where Customer = " + DB.PutStr(txtCustomer.Text) + "Order By ContactName";
		//slContact.ClearValues();
		//slContact.AddDBValues(db, Sql);

		//Sql = "Select Distinct EventType From mcJobs Where EventType <> '' Order By EventType";
        Sql =
            "with TopEventTypes as \n" +
            "(\n" +
            "        select top 35 EventType, Count(*) Cnt from mcJobs where EventType <> '' group by EventType order By Cnt Desc\n" +
            ")\n" +
            "select EventType from TopEventTypes order by EventType";
		//slEventType.ClearValues();
		//slEventType.AddDBValues(db, Sql);
		aEventTypes = db.StrArray(Sql);

		Sql = "Select Distinct ServiceType From mcJobs Where ServiceType <> '' Order By ServiceType";
		//slServiceType.ClearValues();
		//slServiceType.AddDBValues(db, Sql);
		aServiceTypes = db.StrArray(Sql);

		//Sql = "Select Distinct Location From mcJobs Order By Location";
		//slLocation.ClearValues();
		//slLocation.AddDBValues(db, Sql);

		Sql = "Select Distinct Vehicle From mcJobs Where Vehicle <> '' Order By Vehicle";
		//slVehicle.ClearValues();
		//slVehicle.AddDBValues(db, Sql);
		aVehicles = db.StrArray(Sql);

		//Sql = "Select Distinct Carts From mcJobs Order By Carts";
		//slCarts.ClearValues();
		//slCarts.AddDBValues(db, Sql);
	}

	private void LoadList(int Rno)
	{
        //lblInfo.Text += "<br />LoadList " + hfAddlSearch.Value;
        int PrevRno = 0;
        int NextRno = 0;

		String Sql =
            "Select JobRno, Coalesce(cu.Name, c.Name) as Customer, JobDate, ProposalFlg " +
            "From mcJobs j " +
            "Left Join Contacts c on j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno ";
		String Where = "";

		bool fAddlSearch = Str.Bool(hfAddlSearch.Value);

		DateTime SearchDate = Str.DtTm(txtSearchDate.Text.Trim());

		if (fAddlSearch)
		{
			string SearchCustomer = txtSearchCustomer.Text.Trim();
			string SearchContact = txtSearchContact.Text.Trim();
			string SearchPhone = txtSearchPhone.Text.Trim();
			string SearchEmail = txtSearchEmail.Text.Trim();
			string SearchLocation = txtSearchLocation.Text.Trim();

			if (SearchCustomer.Length > 0)
			{
				Where = DB.And(Where) + "cu.Name Like '%" + SearchCustomer + "%'";
            }

            if (SearchContact.Length > 0)
			{
				Where = DB.And(Where) + "c.Name Like '%" + SearchContact + "%'";
			}

			if (SearchPhone.Length > 0)
			{
				Where = DB.And(Where) + "c.Phone Like '%" + SearchPhone + "%'";
			}

			if (SearchEmail.Length > 0)
			{
				Where = DB.And(Where) + "c.Email Like '%" + SearchEmail + "%'";
			}

			if (SearchLocation.Length > 0)
			{
				Where = DB.And(Where) + "Location Like '%" + SearchLocation + "%'";
			}

			if (SearchDate != DateTime.MinValue)
			{
				Where = DB.And(Where) + "JobDate Between " + DB.PutDtTm(SearchDate) + " And " + DB.PutDtTm(SearchDate.AddDays(1).AddSeconds(-1));
			}
		}

		if (!chkPastJobs.Checked && !(fAddlSearch && SearchDate != DateTime.MinValue))
		{
			Where = DB.And(Where) + "JobDate >= " + DB.PutDtTm(DateTime.Today) + " ";
		}

		if (!chkCancelledJobs.Checked)
		{
			Where = DB.And(Where) + "CancelledDtTm Is Null ";
		}

		if (!(chkJobs.Checked && chkProposals.Checked))
		{
			Where = DB.And(Where) + "ProposalFlg = " + DB.PutBool(chkProposals.Checked);
		}		

		Sql += DB.Where(Where);

		if (rbSortDate.Checked)
		{
			Sql += " Order By JobDate, Coalesce(cu.Name, c.Name)";
		}
		else
		{
			Sql += " Order By Coalesce(cu.Name, c.Name), JobDate";
		}

		lstList.Items.Clear();

		lblSql.Text = Sql;
		try
		{
            int PrevRnoValue = 0;
            bool fSaveNextRno = false;
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				int JobRno = DB.Int32(r["JobRno"]);
				String Customer = DB.Str(r["Customer"]);
				String JobDate = Fmt.Dt(DB.DtTm(r["JobDate"]));
				ListItem Item = new ListItem(JobDate + " - " + Customer, Convert.ToString(JobRno));
                if (JobRno == Rno)
                {
                    Item.Selected = true;
                    PrevRno = PrevRnoValue;
                    fSaveNextRno = true;
                }
                else
                {
                    Item.Selected = false;
                    if (fSaveNextRno)
                    {
                        NextRno = JobRno;
                        fSaveNextRno = false;
                    }
                }

                if (DB.Bool(r["ProposalFlg"]))
				{
					Item.Attributes.Add("class", "Proposal");
				}

				lstList.Items.Add(Item);
                PrevRnoValue = JobRno;
			}

			lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Jobs";
            lnkPrev.Visible = (PrevRno != 0);
            lnkNext.Visible = (NextRno != 0);
            if (lnkPrev.Visible)
            {
                lnkPrev.NavigateUrl = "~/Job.aspx?JobRno=" + PrevRno;
            }
            if (lnkNext.Visible)
            {
                lnkNext.NavigateUrl = "~/Job.aspx?JobRno=" + NextRno;
            }
        }
        catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void UpdateList(object sender, System.EventArgs e)
	{
		//lblInfo.Text += "<br />UpdateList " + txtSearchCustomer.Text;

		JobRno = Str.Num(txtJobRno.Text);
		LoadList(JobRno);

		Session["AddlSearch"] = hfAddlSearch.Value;
		Session["Customer"] = txtSearchCustomer.Text;
		Session["Contact"] = txtSearchContact.Text;
		Session["Phone"] = txtSearchPhone.Text;
		Session["Email"] = txtSearchEmail.Text;
		Session["Location"] = txtSearchLocation.Text;
		Session["Date"] = txtSearchDate.Text;

		Session["JobsSortedBy"] = (rbSortDate.Checked ? "Date" : "Name");
		Session["CancelledJobs"] = chkCancelledJobs.Checked;
		Session["PastJobs"] = chkPastJobs.Checked;
		Session["Jobs"] = chkJobs.Checked;
		Session["Proposals"] = chkProposals.Checked;

		ScriptManager.RegisterStartupScript(upSearch, upSearch.GetType(), "SetupSearch", "SetupSearch();", true);
	}

	/*
	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		JobRno = Convert.ToInt32(lstList.SelectedItem.Value);
		ReadValues(JobRno);
		FocusField = "txtCustomer";
	}
	*/

	protected void btnSave_Click(object sender, System.EventArgs e)
	{
		Save();
		//SelectListsData();
	}

	protected void Save()
	{
		JobRno = Str.Num(txtJobRno.Text);
		JobRno = SaveValues();
		new RecentJobs().Push(JobRno);		
		LoadList(JobRno);
		ReadValues(JobRno);
	}

	protected void btnNew_Click(object sender, System.EventArgs e)
	{
		ClearValues();
		SetStatus();
		LoadList(0);
		Session["JobRno"] = "";
		FocusField = "txtCustomer";
	}

	private void SetStatus()
	{
		String Status = "New";

		//if (chkCompleted.Checked)	{ Status = "Completed"; }
		//if (chkConfirmed.Checked)	{ Status = "Confirmed"; }
		//if (chkPrinted.Checked)		{ Status = "Printed"; }
		//if (chkInvoiced.Checked)	{ Status = "Invoiced"; }
		//if (chkCancelled.Checked)	{ Status = "Cancelled"; }

		if (txtStatus.Text.Length == 0)
		{
			txtStatus.Text = Status;
		}
	}

	private void ClearValues()
	{
        lblJobDesc.Text =
		txtJobRno.Text =
		txtCustomer.Text =
        hfCustomerRno.Value =
		txtContact.Text =
        hfContactRno.Value =
		txtStatus.Text =
		txtPhone.Text =
		txtCell.Text =
		//txtFax.Text =
		txtEmail.Text =
        txtResponsibleParty.Text = 
		txtEventType.Text =
        txtEventTypeDetails.Text = 
        txtJobDesc.Text = 
        txtCeremonyLocation.Text =
		txtServiceType.Text =
        txtNumBuffets.Text = 
        txtBuffetSpace.Text =
		txtLocation.Text =
		//txtDirections.Text =
		//txtVehicle.Text =
		//txtCarts.Text =
        txtBookedBy.Text =
        //txtConfirmDeadlineDate.Text =
        //txtConfirmedBy.Text =
		txtJobDate.Text =
		txtJobDayOfWeek.Text =
        txtLoadTime.Text =
        txtDepartTime.Text =
		txtArrivalTime.Text =
		txtGuestArrival.Text = 
        txtWeddingTime.Text = 
		txtMealTime.Text =
		txtEndTime.Text = 
		txtServingMen.Text =
		//txtServingWomen.Text =
		//txtServingChild.Text =
		//txtServingTotal.Text =
		//hfServingTotal.Value =
		txtDemographics.Text =
		txtPricePerPerson.Text =
		txtSubTotal.Text =
        txtPONumber.Text =
		txtJobNotes.Text =
        txtProdNotes.Text = 
        txtCrew.Text = 
		txtEntDt.Text =
		txtEntUser.Text =
		txtUpdDt.Text =
		txtUpdUser.Text =
		//txtCompDt.Text =
		//txtCompUser.Text =
		//txtConfDt.Text =
		//txtConfUser.Text =
		txtPrintDt.Text =
		txtPrintUser.Text =
		txtInvDt.Text =
		txtInvUser.Text =
		txtConfirmEmailedDt.Text =
		txtConfirmEmailedUser.Text =
		//txtCanDt.Text =
		//txtCanUser.Text =
		txtInvCreatedDt.Value =
        txtFinalConfirmEmailedDt.Text =
        txtFinalConfirmEmailedUser.Text = 
        txtConfirmedDt.Text = 
        txtConfirmedUser.Text =
		lblMsg.Text = "";

		chkProposal.Checked =
        chkConfirmed.Checked =
        rbCeremonyYes.Checked =
        rbCeremonyYes.Checked =
        rbDisposableYes.Checked =
        rbDisposableNo.Checked =
		//chkCompleted.Checked =
		//chkConfirmed.Checked =
		//chkPrinted.Checked =
		//chkPrintDir.Checked = 
			false;
		//chkInvoiced.Checked =
		//chkCancelled.Checked = false;

		ddlJobType.SelectedIndex = 
		ddlPmtTerms.SelectedIndex =
        ddlMenuType.SelectedIndex = -1;

		Fmt.SetRadioButtonList(ref rbPaymentType, "Check");

		fNew = true;
		fProposal = false;
		txtDirty.Value = "false";

		InvEventCount = 0;
		InvPricePerPerson = 0M;
	}

	private void ReadValues(int Rno)
	{
		ClearValues();

        String Sql = string.Format(
            "Select cu.CustomerRno, cu.Name as Customer, c.ContactRno, c.Name, c.Phone, c.Cell, c.Email, j.*  " +
            "From mcJobs j " +
            "Left Join Contacts c on j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Where j.JobRno = {0}",
            Rno);
        try
		{
			DataTable dt = db.DataTable(Sql);
			if (dt.Rows.Count > 0)
			{
				DataRow dr = dt.Rows[0];

				string Status = DB.Str(dr["Status"]);

                int cBuffets = DB.Int32(dr["NumBuffets"]);
				int cMen = DB.Int32(dr["NumMenServing"]);
				int cWomen = DB.Int32(dr["NumWomenServing"]);
				int cChild = DB.Int32(dr["NumChildServing"]);
				InvEventCount = DB.Int32(dr["InvEventCount"]);
				decimal Price = DB.Dec(dr["PricePerPerson"]);
				InvPricePerPerson = DB.Dec(dr["InvPricePerPerson"]);

                lblJobDesc.Text = DB.Str(dr["JobDesc"]);
				txtJobRno.Text = Convert.ToString(DB.Int32(dr["JobRno"]));
                hfCustomerRno.Value = Convert.ToString(DB.Int32(dr["CustomerRno"]));
				txtCustomer.Text = DB.Str(dr["Customer"]);
                hfContactRno.Value = Convert.ToString(DB.Int32(dr["ContactRno"]));
                txtContact.Text = DB.Str(dr["Name"]);
				txtStatus.Text = Status;
				txtPhone.Text = DB.Str(dr["Phone"]);
				txtCell.Text = DB.Str(dr["Cell"]);
				//txtFax.Text = DB.Str(dr["Fax"]);
				txtEmail.Text = DB.Str(dr["Email"]);
				txtEmail.ToolTip = txtEmail.Text;
                txtResponsibleParty.Text = DB.Str(dr["ResponsibleParty"]);
                ddlJobType.Text = DB.Str(dr["JobType"]);
				ddlPmtTerms.Text = DB.Str(dr["PmtTerms"]);
				fProposal =
				chkProposal.Checked = DB.Bool(dr["ProposalFlg"]);
				txtEventType.Text = DB.Str(dr["EventType"]);
                txtEventTypeDetails.Text = DB.Str(dr["EventTypeDetails"]);
                txtJobDesc.Text = DB.Str(dr["JobDesc"]);
                rbCeremonyYes.Checked = DB.Bool(dr["CeremonyFlg"]);
                rbCeremonyNo.Checked = !rbCeremonyYes.Checked;
                txtCeremonyLocation.Text = DB.Str(dr["CeremonyLocation"]);
                txtCeremonyLocation.ToolTip = txtCeremonyLocation.Text;
                txtServiceType.Text = DB.Str(dr["ServiceType"]);
                rbDisposableYes.Checked = DB.Bool(dr["DisposableFlg"]);
                rbDisposableNo.Checked = !rbDisposableYes.Checked;
                txtNumBuffets.Text = Fmt.Num(cBuffets, false);
                txtBuffetSpace.Text = DB.Str(dr["BuffetSpace"]);
				txtLocation.Text = DB.Str(dr["Location"]);
				txtLocation.ToolTip = txtLocation.Text;
				//txtDirections.Text = DB.Str(dr["LocationDirections"]);
				//txtVehicle.Text = DB.Str(dr["Vehicle"]);
				//txtCarts.Text = DB.Str(dr["Carts"]);
                txtBookedBy.Text = DB.Str(dr["BookedBy"]);
                chkConfirmed.Checked = (DB.DtTm(dr["ConfirmedDtTm"]) != DateTime.MinValue);
                //txtConfirmDeadlineDate.Text = Fmt.Dt(DB.DtTm(dr["ConfirmDeadlineDate"]));
                //txtConfirmedBy.Text = DB.Str(dr["ConfirmedBy"]);
                txtJobDate.Text = Fmt.Dt(DB.DtTm(dr["JobDate"]));
				txtJobDayOfWeek.Text = Fmt.DayOfWeek(DB.DtTm(dr["JobDate"]));
                txtLoadTime.Text = Fmt.Tm12Hr(DB.DtTm(dr["LoadTime"]));
				txtDepartTime.Text = Fmt.Tm12Hr(DB.DtTm(dr["DepartureTime"]));
				txtArrivalTime.Text = Fmt.Tm12Hr(DB.DtTm(dr["ArrivalTime"]));
				txtArrivalTime.Text = Fmt.Tm12Hr(DB.DtTm(dr["ArrivalTime"]));
				txtGuestArrival.Text = Fmt.Tm12Hr(DB.DtTm(dr["GuestArrivalTime"]));
                txtWeddingTime.Text = Fmt.Tm12Hr(DB.DtTm(dr["CeremonyTime"]));
				txtMealTime.Text = Fmt.Tm12Hr(DB.DtTm(dr["MealTime"]));
				txtEndTime.Text = Fmt.Tm12Hr(DB.DtTm(dr["EndTime"]));
				txtServingMen.Text = Fmt.Num(cMen, false);
				//txtServingWomen.Text = Fmt.Num(cWomen, false);
				//txtServingChild.Text = Fmt.Num(cChild, false);
				//txtServingTotal.Text = Fmt.Num(cMen + cWomen + cChild, false);
				//hfServingTotal.Value = txtServingTotal.Text;
				txtDemographics.Text = DB.Str(dr["Demographics"]);
				txtPricePerPerson.Text = Fmt.Dollar(Price, false);
				txtSubTotal.Text = Fmt.Dollar(Price * (cMen + cWomen + cChild), false);
                ddlMenuType.Text = DB.Str(dr["MenuType"]);
                txtPONumber.Text = DB.Str(dr["PONumber"]);
                txtJobNotes.Text = DB.Str(dr["JobNotes"]);
                txtProdNotes.Text = DB.Str(dr["ProductionNotes"]);
                txtCrew.Text = DB.Str(dr["Crew"]);
                txtEntDt.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
				txtEntUser.Text = DB.Str(dr["CreatedUser"]);
				txtUpdDt.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
				txtUpdUser.Text = DB.Str(dr["UpdatedUser"]);
				//txtCompDt.Text = Fmt.DtTm(DB.DtTm(r["CompletedDtTm"]));
				//txtCompUser.Text = DB.Str(r["CompletedUser"]);
				//txtConfDt.Text = Fmt.DtTm(DB.DtTm(r["ConfirmedDtTm"]));
				//txtConfUser.Text = DB.Str(r["ConfirmedUser"]);
				txtPrintDt.Text = Fmt.DtTm(DB.DtTm(dr["PrintedDtTm"]));
				txtPrintUser.Text = DB.Str(dr["PrintedUser"]);
				txtInvDt.Text = Fmt.DtTm(DB.DtTm(dr["InvoicedDtTm"]));
				txtInvUser.Text = DB.Str(dr["InvoicedUser"]);
				txtConfirmEmailedDt.Text = Fmt.DtTm(DB.DtTm(dr["ConfirmEmailedDtTm"]));
				txtConfirmEmailedUser.Text = DB.Str(dr["ConfirmEmailedUser"]);
				//txtCanDt.Text = Fmt.DtTm(DB.DtTm(r["CancelledDtTm"]));
				//txtCanUser.Text = DB.Str(r["CancelledUser"]);
				txtInvCreatedDt.Value = Fmt.DtTm(DB.DtTm(dr["InvCreatedDtTm"]));
                txtFinalConfirmEmailedDt.Text = Fmt.DtTm(DB.DtTm(dr["FinalConfirmEmailedDtTm"]));
                txtFinalConfirmEmailedUser.Text = DB.Str(dr["FinalConfirmEmailedUser"]);
                txtConfirmedDt.Text = Fmt.DtTm(DB.DtTm(dr["ConfirmedDtTm"]));
                txtConfirmedUser.Text = DB.Str(dr["ConfirmedUser"]);

                //chkCompleted.Checked = (txtCompDt.Text.Length > 0);
                //chkConfirmed.Checked = (txtConfDt.Text.Length > 0);
                //chkPrinted.Checked = (txtPrintDt.Text.Length > 0);
                //chkPrintDir.Checked = DB.Bool(dr["PrintDirectionsFlg"]);
                //chkInvoiced.Checked = (txtInvDt.Text.Length > 0);
                //chkCancelled.Checked = (txtCanDt.Text.Length > 0);

                Fmt.SetRadioButtonList(ref rbPaymentType, DB.Str(dr["PmtMethod"]));

				fNew = false;

				//SetStatus();

				Session["JobRno"] = txtJobRno.Text;

				if (txtJobDate.Text.Length > 0)
				{
					//calJobDate.Date = Str.DtTm(txtJobDate.Text);
				}

                hfOrigStartDateTime.Value = StartDateTime().ToString();
            }
			else
			{
				txtJobRno.Text = Rno.ToString();
				lblMsg.Text = "Job not found.";
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		SelectListsData();
		new RecentJobs().Push(Rno);
	}

	private int SaveValues()
	{
		JobRno = Str.Num(txtJobRno.Text);
        int CustomerRno = Str.Num(hfCustomerRno.Value);
        int ContactRno = Str.Num(hfContactRno.Value);
		String Sql = "";

		try
		{
			bool fNewRec = false;
			DateTime Tm = DateTime.Now;

            SetStatus();

			if (JobRno == 0)
			{
				JobRno = db.NextRno("mcJobs", "JobRno");
				Sql =
					"Insert Into mcJobs (JobRno, CreatedDtTm, CreatedUser) Values (" +
					JobRno + ", " +
					DB.PutDtTm(Tm) + ", " +
					DB.PutStr(g.User) + ")";
				db.Exec(Sql);

				fNewRec = true;
			}

            // if not a selected customer and contact and a customer name was given but a contact name was not
            // set the contact name to what was given for the customer and blank the customer name
            if (CustomerRno == 0 && ContactRno == 0)
            {
                if (txtCustomer.Text.Length > 0 && txtContact.Text.Length == 0)
                {
                    txtContact.Text = txtCustomer.Text;
                    txtCustomer.Text = string.Empty;
                }
            }


            if (txtCustomer.Text.Length > 0 && CustomerRno == 0)
            {
                Sql = string.Format(
                    "Insert Into Customers (Name, TaxExemptFlg, CompanyFlg, OnlineOrderFlg, EmailConfirmationsFlg, CreatedDtTm, CreatedUser) " + 
                    "Values ({0}, {1}, {2}, {3}, {4}, GetDate(), {5}); Select @@Identity",
                    DB.Put(txtCustomer.Text, 50),
                    DB.Put(Parm.Bool("hfCustTaxExempt")),
                    DB.Put(Parm.Bool("hfCustCompany")),
                    DB.Put(Parm.Bool("hfCustOnlineOrders")),
                    DB.Put(Parm.Bool("hfCustEmailConfirmations")),
                    DB.Put(g.User));
                CustomerRno = db.SqlNum(Sql);
            }

            if (txtContact.Text.Length > 0 && ContactRno == 0)
            {
                string Title = 
                Sql = string.Format(
                    "Insert Into Contacts (CustomerRno, Name, Title, Fax, CreatedDtTm, CreatedUser) Values ({0}, {1}, {2}, {3}, GetDate(), {4}); Select @@Identity",
                    (CustomerRno == 0 ? "Null" : CustomerRno.ToString()),
                    DB.Put(txtContact.Text, 50),
                    DB.Put(hfTitle.Value, 30),
                    DB.Put(hfFax.Value, 20),
                    DB.Put(g.User));
                ContactRno = db.SqlNum(Sql);
            }

            if (ContactRno != 0)
            {
                Sql =
                    "Update Contacts Set " +
                    "Name = " + DB.Put(txtContact.Text, 50) + ", " +
                    "Phone = " + DB.Put(txtPhone.Text, 20) + ", " +
                    "Cell = " + DB.Put(txtCell.Text, 20) + ", " +
                    "Email = " + DB.Put(txtEmail.Text, 80) + " " +
                    "Where ContactRno = " + ContactRno.ToString();
                db.SqlNum(Sql);
            }

            Sql =
                "Update mcJobs Set " +
                "JobDesc = " + DB.Put(txtJobDesc.Text, 80) + ", " +
                "ContactRno = " + (ContactRno == 0 ? "Null" : ContactRno.ToString()) + ", " +
                //"Customer = " + DB.PutStr(txtCustomer.Text, 50) + ", " +
                //"ContactName = " + DB.PutStr(txtContact.Text, 50) + ", " +
                "Status = " + DB.PutStr(txtStatus.Text, 20) + ", " +
                //"ContactPhone = " + DB.PutStr(txtPhone.Text, 20) + ", " +
                //"ContactCell = " + DB.PutStr(txtCell.Text, 20) + ", " +
                //"ContactFax = " + DB.PutStr(txtFax.Text, 20) + ", " +
                //"ContactEmail = " + DB.PutStr(txtEmail.Text, 80) + ", " +
                "JobType = " + DB.PutStr(ddlJobType.SelectedValue, 20) + ", " +
				"PmtTerms = " + DB.PutStr(ddlPmtTerms.SelectedValue, 20) + ", " +
                "ProposalFlg = " + DB.PutBool(chkProposal.Checked) + ", " +
                "ResponsibleParty = " + DB.PutStr(txtResponsibleParty.Text, 50) + ", " +
                "EventType = " + DB.PutStr(txtEventType.Text, 50) + ", " +
                "EventTypeDetails = " + DB.PutStr(txtEventTypeDetails.Text, 200) + ", " +
                "CeremonyFlg = " + DB.PutBool(rbCeremonyYes.Checked) + ", " +
                "CeremonyLocation = " + DB.PutStr(txtCeremonyLocation.Text) + ", " +
                "ServiceType = " + DB.PutStr(txtServiceType.Text, 50) + ", " +
                "DisposableFlg = " + DB.PutBool(rbDisposableYes.Checked) + ", " +
                "NumBuffets = " + Str.Num(txtNumBuffets.Text) + ", " +
                "BuffetSpace = " + DB.PutStr(txtBuffetSpace.Text, 200) + ", " +
				"Location = " + DB.PutStr(txtLocation.Text, 80) + ", " +
				//"LocationDirections = " + DB.PutStr(txtDirections.Text, 1024) + ", " +
				//"Vehicle = " + DB.PutStr(txtVehicle.Text, 50) + ", " +
				//"Carts = " + DB.PutStr(txtCarts.Text, 50) + ", " +
                "BookedBy = " + DB.PutStr(txtBookedBy.Text, 50) + ", " +
                //"ConfirmDeadlineDate = " + DB.PutDtTm(Str.DtTm(txtConfirmDeadlineDate.Text)) + ", " +
                //"ConfirmedBy = " + DB.PutStr(txtConfirmedBy.Text, 50) + ", " +
                "ConfirmedDtTm = " + (chkConfirmed.Checked ? DB.PutDtTm(Tm) : "NULL") + ", " +
                "ConfirmedUser = " + (chkConfirmed.Checked ? DB.PutStr(g.User) : "NULL") + ", " +
                "JobDate = " + DB.PutDtTm(Str.DtTm(txtJobDate.Text)) + ", " +
                "LoadTime = " + DB.PutDtTm(txtJobDate.Text, txtLoadTime.Text) + ", " +
				"DepartureTime = " + DB.PutDtTm(txtJobDate.Text, txtDepartTime.Text) + ", " +
				"ArrivalTime = " + DB.PutDtTm(txtJobDate.Text, txtArrivalTime.Text) + ", " +
				"GuestArrivalTime = " + DB.PutDtTm(txtJobDate.Text, txtGuestArrival.Text) + ", " +
                "CeremonyTime = " + DB.PutDtTm(txtJobDate.Text, txtWeddingTime.Text) + ", " +
				"MealTime = " + DB.PutDtTm(txtJobDate.Text, txtMealTime.Text) + ", " +
				"EndTime = " + DB.PutDtTm(txtJobDate.Text, txtEndTime.Text) + ", " +
				"NumMenServing = " + Str.Num(txtServingMen.Text) + ", " +
				//"NumWomenServing = " + Str.Num(txtServingWomen.Text) + ", " +
				//"NumChildServing = " + Str.Num(txtServingChild.Text) + ", " +
				"Demographics = " + DB.PutStr(txtDemographics.Text, 1024) + ", " +
				"PricePerPerson = " + Str.Dec(txtPricePerPerson.Text) + ", " +
                "MenuType = " + DB.Put(ddlMenuType.Text, 30) + ", " + 
                "PONumber = " + DB.Put(txtPONumber.Text) + ", " + 
				"JobNotes = " + DB.PutStr(txtJobNotes.Text) + ", " +
                "ProductionNotes = " + DB.PutStr(txtProdNotes.Text) + ", " +
                "Crew = " + DB.PutStr(txtCrew.Text) + ", " + 
                //BoxChecked("Completed", chkCompleted.Checked, txtCompDt.Text, Tm, ", ") +
				//BoxChecked("Confirmed", chkConfirmed.Checked, txtConfDt.Text, Tm, ", ") +
				//BoxChecked("Printed", chkPrinted.Checked, txtPrintDt.Text, Tm, ", ") +
				//BoxChecked("Invoiced", chkInvoiced.Checked, txtInvDt.Text, Tm, ", ") +
				//BoxChecked("Cancelled", chkCancelled.Checked, txtCanDt.Text, Tm, ", ") +
				//"PrintDirectionsFlg = " + DB.PutBool(chkPrintDir.Checked) + ", " +
				"PmtMethod = " + DB.PutStr(rbPaymentType.SelectedItem.Value) + "" +
				(fNewRec ? " " : ", " +
				"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
				"UpdatedUser = " + DB.PutStr(g.User) + " ") +
				"Where JobRno = " + JobRno;
			db.Exec(Sql);

			// update Crew ReportTime values 
			Sql =
				"Update mcJobCrew Set ReportTime = " +
				DB.PutStr(txtJobDate.Text) + " + ' ' + " +
				"Cast(DatePart(hh, ReportTime) As varchar) + ':' + " +
				"Cast(DatePart(mi, ReportTime) As varchar) + ':00' " +
				"Where JobRno = " + JobRno + " " +
				"And ReportTime Is Not Null";
			db.Exec(Sql);

			//fQtyWarning = (Str.Num(txtServingMen.Text) + Str.Num(txtServingWomen.Text) + Str.Num(txtServingChild.Text) != Str.Num(hfServingTotal.Value));

			SaveAtSling();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		return JobRno;
	}

	private String BoxChecked(
		String sField,
		Boolean fChecked,
		String sCheckedDtTm,
		DateTime Tm,
		String sConnector)
	{
		String Sql = "";

		//if a new check
		if (fChecked && sCheckedDtTm.Length == 0)
		{
			Sql =
				sField + "DtTm = " + DB.PutDtTm(Tm) + ", " +
				sField + "User = " + DB.PutStr(g.User) + sConnector;
		}
		else
			if (!fChecked && sCheckedDtTm.Length != 0)
			{
				Sql =
					sField + "DtTm = Null, " +
					sField + "User = Null" + sConnector; ;
			}

		return Sql;
	}

	private void txtCustomer_TextChanged(object sender, System.EventArgs e)
	{
		SelectListsData();
		/*
					String Sql = 
						"Select Top 1 ContactName, ContactPhone, ContactCell, Location, LocationDirections, PrintDirectionsFlg " + 
						"From mcJobs Where Customer = " + DB.PutStr(txtCustomer.Text) + " " + 
						"Order By CreatedDtTm Desc";

					try
					{
						DataTable dt = db.DataTable(Sql);
						if (dt.Rows.Count > 0)
						{
							DataRow r = dt.Rows[0];

							if (txtContact.Text == "")	{ txtContact.Text	= DB.Str(r["ContactName"]); }
							if (txtPhone.Text == "")	{ txtPhone.Text		= DB.Str(r["ContactPhone"]); }
							if (txtCell.Text == "")		{ txtCell.Text		= DB.Str(r["ContactCell"]); }
							if (txtLocation.Text == "")	{ txtLocation.Text	= DB.Str(r["Location"]); }
							if (txtDirections.Text == "")
							{
								txtDirections.Text		= DB.Str(r["LocationDirections"]);
								chkPrintDir.Checked		= DB.Bool(r["PrintDirectionsFlg"]);
							}
						}
					}
					catch (Exception Ex)
					{
						Err Err = new Err(Ex, Sql);
						Response.Write(Err.Html());
					}
		*/
		this.FocusField = "txtContact";
	}

	private void txtContact_TextChanged(object sender, System.EventArgs e)
	{
		String Sql =
			"Select Top 1 ContactPhone, ContactCell, ContactFax, ContactEmail " +
			"From mcJobs " +
			"Where Customer = " + DB.PutStr(txtCustomer.Text) + " " +
			"And ContactName = " + DB.PutStr(txtContact.Text) + " " +
			"Order By CreatedDtTm Desc";

		try
		{
			txtPhone.Text = "";
			txtCell.Text = "";
			//txtFax.Text = "";
			txtEmail.Text = "";

			DataTable dt = db.DataTable(Sql);
			if (dt.Rows.Count > 0)
			{
				DataRow r = dt.Rows[0];

				txtPhone.Text = DB.Str(r["ContactPhone"]);
				txtCell.Text = DB.Str(r["ContactCell"]);
				//txtFax.Text = DB.Str(r["ContactFax"]);
				txtEmail.Text = DB.Str(r["ContactEmail"]);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		FocusField = "txtEventType";
		if (txtEmail.Text.Length == 0) { FocusField = "txtEmail"; }
		//if (txtFax.Text.Length == 0) { FocusField = "txtFax"; }
		if (txtCell.Text.Length == 0) { FocusField = "txtCell"; }
		if (txtPhone.Text.Length == 0) { FocusField = "txtPhone"; }
	}

	private void txtLocation_TextChanged(object sender, System.EventArgs e)
	{
		txtLocation.ToolTip = txtLocation.Text;

		//String Sql =
		//	"Select Top 1 LocationDirections, PrintDirectionsFlg " +
		//	"From mcJobs " +
		//	"Where Location = " + DB.PutStr(txtLocation.Text) + " " +
		//	"Order By CreatedDtTm Desc";

		//try
		//{
		//	DataTable dt = db.DataTable(Sql);
		//	if (dt.Rows.Count > 0)
		//	{
		//		DataRow r = dt.Rows[0];

		//		txtDirections.Text = DB.Str(r["LocationDirections"]);
		//		chkPrintDir.Checked = DB.Bool(r["PrintDirectionsFlg"]);
		//	}
		//}
		//catch (Exception Ex)
		//{
		//	Err Err = new Err(Ex, Sql);
		//	Response.Write(Err.Html());
		//}

		this.FocusField = "txtJobDate";
	}

	protected void btnCopy_Click(object sender, System.EventArgs e)
	{
		DateTime Tm = DateTime.Now;
		string Sql = string.Empty;

        try
        {
            decimal SubTotTaxPct     = 0;
            decimal ServiceTaxPct    = 0;
            decimal DeliveryTaxPct   = 0;
            decimal ChinaTaxPct      = 0;
            decimal AddServiceTaxPct = 0;
            decimal FuelTravelTaxPct = 0;
            decimal FacilityTaxPct   = 0;
            decimal RentalsTaxPct    = 0;
            decimal Adj1TaxPct       = 0;
            decimal Adj2TaxPct       = 0;

            decimal ConfigGratuitySubTotPct             = 0;
            decimal ConfigServiceSubTotPct              = 0;
            decimal ConfigDeliverySubTotPct             = 0;
            decimal ConfigChinaSubTotPct                = 0;
            decimal ConfigAddServiceSubTotPct           = 0;
            decimal ConfigFuelTravelSubTotPct           = 0;
            decimal ConfigFacilitySubTotPct             = 0;
            decimal ConfigRentalsSubTotPct              = 0;

            bool GratuitySubTotPctFlg           = false;
            bool VoluntaryGratuitySubTotPctFlg  = false;
            bool ServiceSubTotPctFlg            = false;
            bool DeliverySubTotPctFlg           = false;
            bool ChinaSubTotPctFlg              = false;
            bool AddServiceSubTotPctFlg         = false;
            bool FuelTravelSubTotPctFlg         = false;
            bool FacilitySubTotPctFlg           = false;
            bool RentalsSubTotPctFlg            = false;

            decimal GratuitySubTotPct           = 0;
            decimal VoluntaryGratuitySubTotPct  = 0;
            decimal ServiceSubTotPct            = 0;
            decimal DeliverySubTotPct           = 0;
            decimal ChinaSubTotPct              = 0;
            decimal AddServiceSubTotPct         = 0;
            decimal FuelTravelSubTotPct         = 0;
            decimal FacilitySubTotPct           = 0;
            decimal RentalsSubTotPct            = 0;

            decimal GratuityAmt             = 0;
            decimal VoluntaryGratuityAmt    = 0;
            decimal ServiceAmt              = 0;
            decimal DeliveryAmt             = 0;
            decimal ChinaAmt                = 0;
            decimal AddServiceAmt           = 0;
            decimal FuelTravelAmt           = 0;
            decimal FacilityAmt             = 0;
            decimal RentalsAmt              = 0;

            Sql = "Select * From Settings Where SettingRno = 1";
            DataRow drSettings = db.DataRow(Sql);
            if (drSettings != null)
            {
                SubTotTaxPct     = DB.Dec(drSettings["SubTotTaxPct"]);
                ServiceTaxPct    = DB.Dec(drSettings["ServiceTaxPct"]);
                DeliveryTaxPct   = DB.Dec(drSettings["DeliveryTaxPct"]);
                ChinaTaxPct      = DB.Dec(drSettings["ChinaTaxPct"]);
                AddServiceTaxPct = DB.Dec(drSettings["AddServiceTaxPct"]);
                FuelTravelTaxPct = DB.Dec(drSettings["FuelTravelTaxPct"]);
                FacilityTaxPct   = DB.Dec(drSettings["FacilityTaxPct"]);
                RentalsTaxPct    = DB.Dec(drSettings["RentalsTaxPct"]);
                Adj1TaxPct       = DB.Dec(drSettings["Adj1TaxPct"]);
                Adj2TaxPct       = DB.Dec(drSettings["Adj2TaxPct"]);

                ConfigGratuitySubTotPct             = DB.Dec(drSettings["GratuitySubTotPct"]);
                ConfigServiceSubTotPct              = DB.Dec(drSettings["ServiceSubTotPct"]);
                ConfigDeliverySubTotPct             = DB.Dec(drSettings["DeliverySubTotPct"]);
                ConfigChinaSubTotPct                = DB.Dec(drSettings["ChinaSubTotPct"]);
                ConfigAddServiceSubTotPct           = DB.Dec(drSettings["AddServiceSubTotPct"]);
                ConfigFuelTravelSubTotPct           = DB.Dec(drSettings["FuelTravelSubTotPct"]);
                ConfigFacilitySubTotPct             = DB.Dec(drSettings["FacilitySubTotPct"]);
                ConfigRentalsSubTotPct              = DB.Dec(drSettings["RentalsSubTotPct"]);
            }

            Sql = "Select " +
                "GratuitySubTotPctFlg, GratuitySubTotPct, GratuityAmt, " +
                "VoluntaryGratuitySubTotPctFlg, VoluntaryGratuitySubTotPct, VoluntaryGratuityAmt, " +
                "ServiceSubTotPctFlg, ServiceSubTotPct, ServiceAmt, " +
                "DeliverySubTotPctFlg, DeliverySubTotPct, DeliveryAmt, " +
                "ChinaSubTotPctFlg, ChinaSubTotPct, ChinaAmt, " +
                "AddServiceSubTotPctFlg, AddServiceSubTotPct, AddServiceAmt, " +
                "FuelTravelSubTotPctFlg, FuelTravelSubTotPct, FuelTravelAmt, " +
                "FacilitySubTotPctFlg, FacilitySubTotPct, FacilityAmt, " +
                "RentalsSubTotPctFlg, RentalsSubTotPct, RentalsAmt " +
                "From mcJobs Where JobRno = " + JobRno;
            DataRow drJob = db.DataRow(Sql);
            if (drJob != null)
            { 
                GratuitySubTotPctFlg            = DB.Bool(drJob["GratuitySubTotPctFlg"]);
                VoluntaryGratuitySubTotPctFlg   = DB.Bool(drJob["VoluntaryGratuitySubTotPctFlg"]);
                ServiceSubTotPctFlg             = DB.Bool(drJob["ServiceSubTotPctFlg"]);
                DeliverySubTotPctFlg            = DB.Bool(drJob["DeliverySubTotPctFlg"]);
                ChinaSubTotPctFlg               = DB.Bool(drJob["ChinaSubTotPctFlg"]);
                AddServiceSubTotPctFlg          = DB.Bool(drJob["AddServiceSubTotPctFlg"]);
                FuelTravelSubTotPctFlg          = DB.Bool(drJob["FuelTravelSubTotPctFlg"]);
                FacilitySubTotPctFlg            = DB.Bool(drJob["FacilitySubTotPctFlg"]);
                RentalsSubTotPctFlg             = DB.Bool(drJob["RentalsSubTotPctFlg"]);

                GratuitySubTotPct           = DB.Dec(drJob["GratuitySubTotPct"]);
                VoluntaryGratuitySubTotPct  = DB.Dec(drJob["VoluntaryGratuitySubTotPct"]);
                ServiceSubTotPct            = DB.Dec(drJob["ServiceSubTotPct"]);
                DeliverySubTotPct           = DB.Dec(drJob["DeliverySubTotPct"]);
                ChinaSubTotPct              = DB.Dec(drJob["ChinaSubTotPct"]);
                AddServiceSubTotPct         = DB.Dec(drJob["AddServiceSubTotPct"]);
                FuelTravelSubTotPct         = DB.Dec(drJob["FuelTravelSubTotPct"]);
                FacilitySubTotPct           = DB.Dec(drJob["FacilitySubTotPct"]);
                RentalsSubTotPct            = DB.Dec(drJob["RentalsSubTotPct"]);

                GratuityAmt             = DB.Dec(drJob["GratuityAmt"]);
                VoluntaryGratuityAmt    = DB.Dec(drJob["VoluntaryGratuityAmt"]);
                ServiceAmt              = DB.Dec(drJob["ServiceAmt"]);
                DeliveryAmt             = DB.Dec(drJob["DeliveryAmt"]);
                ChinaAmt                = DB.Dec(drJob["ChinaAmt"]);
                AddServiceAmt           = DB.Dec(drJob["AddServiceAmt"]);
                FuelTravelAmt           = DB.Dec(drJob["FuelTravelAmt"]);
                FacilityAmt             = DB.Dec(drJob["FacilityAmt"]);
                RentalsAmt              = DB.Dec(drJob["RentalsAmt"]);

                //if (GratuitySubTotPctFlg)
                //{
                //    GratuitySubTotPct = ConfigGratuitySubTotPct;
                //    GratuityAmt = 0;
                //}
                //else
                //{
                //    GratuitySubTotPct = 0;
                //}

                //if (VoluntaryGratuitySubTotPctFlg)
                //{
                //    VoluntaryGratuitySubTotPct = 0;
                //    VoluntaryGratuityAmt = 0;
                //}
                //else
                //{
                //    VoluntaryGratuitySubTotPct = 0;
                //}

                if (ServiceSubTotPctFlg)
                {
                    ServiceSubTotPct = ConfigServiceSubTotPct;
                    ServiceAmt = 0;
                }
                else
                {
                    ServiceSubTotPct = 0;
                }

                if (DeliverySubTotPctFlg)
                {
                    DeliverySubTotPct = ConfigDeliverySubTotPct;
                    DeliveryAmt = 0;
                }
                else
                {
                    DeliverySubTotPct = 0;
                }

                if (ChinaSubTotPctFlg)
                {
                    ChinaSubTotPct = ConfigChinaSubTotPct;
                    ChinaAmt = 0;
                }
                else
                {
                    ChinaSubTotPct = 0;
                }

                if (AddServiceSubTotPctFlg)
                {
                    AddServiceSubTotPct = ConfigAddServiceSubTotPct;
                    AddServiceAmt = 0;
                }
                else
                {
                    AddServiceSubTotPct = 0;
                }

                if (FuelTravelSubTotPctFlg)
                {
                    FuelTravelSubTotPct = ConfigFuelTravelSubTotPct;
                    FuelTravelAmt = 0;
                }
                else
                {
                    FuelTravelSubTotPct = 0;
                }

                if (FacilitySubTotPctFlg)
                {
                    FacilitySubTotPct = ConfigFacilitySubTotPct;
                    FacilityAmt = 0;
                }
                else
                {
                    FacilitySubTotPct = 0;
                }

                if (RentalsSubTotPctFlg)
                {
                    RentalsSubTotPct = ConfigRentalsSubTotPct;
                    RentalsAmt = 0;
                }
                else
                {
                    RentalsSubTotPct = 0;
                }
            }

            int NewJobRno = db.NextRno("mcJobs", "JobRno");

			Sql =
				"Insert Into mcJobs (" +
                "JobRno, " +
                "JobDesc, " +
                "ContactRno, " +
                "ResponsibleParty, " +
                "BookedBy, " +
                "ConfirmedDtTm, " +
                "ConfirmedUser, " + 
                "JobType, " +
                "PmtTerms, " +
                "ProposalFlg, " +
                "EventType, " +
                "EventTypeDetails, " +
                "ServiceType, " +
                "Location, " +
                "LocationDirections, " +
                "PrintDirectionsFlg, " +
                "Vehicle, " +
                "Carts, " +
                "NumBuffets, " +
                "BuffetSpace, " +
                "JobDate, " +
                "LoadTime, " +
                "DepartureTime, " +
                "ArrivalTime, " +
                "GuestArrivalTime, " +
                "MealTime, " +
                "EndTime, " +
                "CeremonyTime, " +
                "CeremonyFlg, " +
                "CeremonyLocation, " +
                "NumMenServing, " +
                "NumWomenServing, " +
                "NumChildServing, " +
                "PricePerPerson, " +
                "PmtMethod, " +
                "MenuType, " +
                "Demographics, " +
                "JobNotes, " +
                "ProductionNotes, " +
                "Crew, " +
                "Status, " +
//                "ConfirmDeadlineDate, " +
                "InvEventCount, " +
                "InvPricePerPerson, " +
                "SubTotAmt, " +
                "SubTotTaxPct, " +
                "ServiceSubTotPctFlg, " +
                "ServiceSubTotPct, " +
                "ServiceAmt, " +
                "ServiceTaxPct, " +
                "ServicePropDesc, " +
                "ServicePropDescCustFlg, " +
                "ServicePropOptions, " +
                "DeliverySubTotPctFlg, " +
                "DeliverySubTotPct, " +
                "DeliveryAmt, " +
                "DeliveryTaxPct, " +
                "DeliveryPropDesc, " +
                "DeliveryPropDescCustFlg, " +
                "DeliveryPropOptions, " +
                "ChinaDesc, " +
                "ChinaSubTotPctFlg, " +
                "ChinaSubTotPct, " +
                "ChinaAmt, " +
                "ChinaTaxPct, " +
                "ChinaPropDesc, " +
                "ChinaPropDescCustFlg, " +
                "ChinaPropOptions, " +
                "AddServiceSubTotPctFlg, " +
                "AddServiceSubTotPct, " +
                "AddServiceAmt, " +
                "AddServiceTaxPct, " +
                "AddServicePropDesc, " +
                "AddServicePropDescCustFlg, " +
                "AddServicePropOptions, " +
                "FuelTravelSubTotPctFlg, " +
                "FuelTravelSubTotPct, " +
                "FuelTravelAmt, " +
                "FuelTravelTaxPct, " +
                "FuelTravelPropDesc, " +
                "FuelTravelPropDescCustFlg, " +
                "FuelTravelPropOptions, " +
                "FacilitySubTotPctFlg, " +
                "FacilitySubTotPct, " +
                "FacilityAmt, " +
                "FacilityTaxPct, " +
                "FacilityPropDesc, " +
                "FacilityPropDescCustFlg, " +
                "FacilityPropOptions, " +
                "GratuitySubTotPctFlg, " +
                "GratuitySubTotPct, " +
                "GratuityAmt, " +
                "GratuityTaxPct, " +
                "VoluntaryGratuitySubTotPctFlg, " +
                "VoluntaryGratuitySubTotPct, " +
                "VoluntaryGratuityAmt, " +
                "RentalsDesc, " +
                "RentalsSubTotPctFlg, " +
                "RentalsSubTotPct, " +
                "RentalsAmt, " +
                "RentalsTaxPct, " +
                "RentalsPropDesc, " +
                "RentalsPropDescCustFlg, " +
                "RentalsPropOptions, " +
                "Adj1Desc, " +
                "Adj1SubTotPctFlg, " +
                "Adj1SubTotPct, " +
                "Adj1Amt, " +
                "Adj1TaxPct, " +
                "Adj1PropDesc, " +
                "Adj2Desc, " +
                "Adj2SubTotPctFlg, " +
                "Adj2SubTotPct, " +
                "Adj2Amt, " +
                "Adj2TaxPct, " +
                "Adj2PropDesc, " +
                "EstTotPropDesc, " +
                "EstTotPropDescCustFlg, " +
                "EstTotPropOptions, " +
                //"DepositPropDesc, " +
                //"DepositPropDescCustFlg, " +
                //"DepositPropOptions, " +
                "InvoiceMsg, " +
                "InvDeliveryMethod, " +
                "InvDeliveryDate, " +
                "InvFax, " +
                //"DepositAmt, " +
                "PreTaxSubTotAmt, " +
                "SalesTaxTotAmt, " +
                "InvTotAmt, " +
                "DisposableFlg, " + 
                "CreatedDtTm, " +
                "CreatedUser, " +
                "InvCreatedDtTm, " +
                "InvCreatedUser) " +
                "Select " + 
                NewJobRno + ", " +
                "JobDesc, " +
                "ContactRno, " +
                "ResponsibleParty, " +
                "BookedBy, " +
                "ConfirmedDtTm, " +
                "ConfirmedUser, " +
                "JobType, " +
                "PmtTerms, " +
                "ProposalFlg, " +
                "EventType, " +
                "EventTypeDetails, " +
                "ServiceType, " +
                "Location, " +
                "LocationDirections, " +
                "PrintDirectionsFlg, " +
                "Vehicle, " +
                "Carts, " +
                "NumBuffets, " +
                "BuffetSpace, " +
                DB.PutDtTm(DateTime.Today) + ", " +
                "LoadTime, " +
                "DepartureTime, " +
                "ArrivalTime, " +
                "GuestArrivalTime, " +
                "MealTime, " +
                "EndTime, " +
                "CeremonyTime, " +
                "CeremonyFlg, " +
                "CeremonyLocation, " +
                "NumMenServing, " +
                "NumWomenServing, " +
                "NumChildServing, " +
                "PricePerPerson, " +
                "PmtMethod, " +
                "MenuType, " +
                "Demographics, " +
                "JobNotes, " +
                "ProductionNotes, " +
                "Crew, " +
                "Status, " +
//                "ConfirmDeadlineDate, " +
                "InvEventCount, " +
                "InvPricePerPerson, " +
                "SubTotAmt, " +
                SubTotTaxPct + ", " +
                DB.Put(ServiceSubTotPctFlg) + ", " +
                ServiceSubTotPct + ", " +
                ServiceAmt + ", " +
                ServiceTaxPct + ", " +
                "ServicePropDesc, " +
                "ServicePropDescCustFlg, " +
                "ServicePropOptions, " +
                DB.Put(DeliverySubTotPctFlg) + ", " +
                DeliverySubTotPct + ", " +
                DeliveryAmt + ", " +
                DeliveryTaxPct + ", " +
                "DeliveryPropDesc, " +
                "DeliveryPropDescCustFlg, " +
                "DeliveryPropOptions, " +
                "ChinaDesc, " +
                DB.Put(ChinaSubTotPctFlg) + ", " +
                ChinaSubTotPct + ", " +
                ChinaAmt + ", " +
                ChinaTaxPct + ", " +
                "ChinaPropDesc, " +
                "ChinaPropDescCustFlg, " +
                "ChinaPropOptions, " +
                DB.Put(AddServiceSubTotPctFlg) + ", " +
                AddServiceSubTotPct + ", " +
                AddServiceAmt + ", " +
                AddServiceTaxPct + ", " +
                "AddServicePropDesc, " +
                "AddServicePropDescCustFlg, " +
                "AddServicePropOptions, " +
                DB.Put(FuelTravelSubTotPctFlg) + ", " +
                FuelTravelSubTotPct + ", " +
                FuelTravelAmt + ", " +
                FuelTravelTaxPct + ", " +
                "FuelTravelPropDesc, " +
                "FuelTravelPropDescCustFlg, " +
                "FuelTravelPropOptions, " +
                DB.Put(FacilitySubTotPctFlg) + ", " +
                FacilitySubTotPct + ", " +
                FacilityAmt + ", " +
                FacilityTaxPct + ", " +
                "FacilityPropDesc, " +
                "FacilityPropDescCustFlg, " +
                "FacilityPropOptions, " +
                DB.Put(GratuitySubTotPctFlg) + ", " +
                GratuitySubTotPct + ", " +
                GratuityAmt + ", " +
                "0, " +
                DB.Put(VoluntaryGratuitySubTotPctFlg) + ", " +
                VoluntaryGratuitySubTotPct + ", " +
                VoluntaryGratuityAmt + ", " +
                "RentalsDesc, " +
                DB.Put(RentalsSubTotPctFlg) + ", " +
                RentalsSubTotPct + ", " +
                RentalsAmt + ", " +
                RentalsTaxPct + ", " +
                "RentalsPropDesc, " +
                "RentalsPropDescCustFlg, " +
                "RentalsPropOptions, " +
                "Adj1Desc, " +
                "Adj1SubTotPctFlg, " +
                "Adj1SubTotPct, " +
                "Adj1Amt, " +
                Adj1TaxPct + ", " +
                "Adj1PropDesc, " +
                "Adj2Desc, " +
                "Adj2SubTotPctFlg, " +
                "Adj2SubTotPct, " +
                "Adj2Amt, " +
                Adj2TaxPct + ", " +
                "Adj2PropDesc, " +
                "EstTotPropDesc, " +
                "EstTotPropDescCustFlg, " +
                "EstTotPropOptions, " +
                //"DepositPropDesc, " +
                //"DepositPropDescCustFlg, " +
                //"DepositPropOptions, " +
                "InvoiceMsg, " +
                "InvDeliveryMethod, " +
                "InvDeliveryDate, " +
                "InvFax, " +
                //"DepositAmt, " +
                "PreTaxSubTotAmt, " +
                "SalesTaxTotAmt, " +
                "InvTotAmt, " +
                "DisposableFlg, " +
                DB.PutDtTm(Tm) + ", " +
				DB.PutStr(g.User) + ", " +
                DB.PutDtTm(Tm) + ", " +
                DB.PutStr(g.User) + " " +
                "From mcJobs Where JobRno = " + JobRno;
			db.Exec(Sql);

			Sql =
				"Insert Into mcJobFood (" +
                "JobRno, " +
                "FoodSeq, " +
                "Category, " +
                "MenuItem, " +
                "MenuItemRno, " +
                "Qty, " +
                "QtyNote, " +
                "ServiceNote, " +
                "ProposalSeq, " +
                "ProposalMenuItem, " +
                "ProposalTitleFlg, " +
                "ProposalHideFlg, " +
                "IngredSelFlg, " +
                "IngredSel, " +
                "CreatedDtTm, " +
                "CreatedUser) Select " +
				NewJobRno + ", " +
                "FoodSeq, " +
                "Category, " +
                "MenuItem, " +
                "MenuItemRno, " +
                "Qty, " +
                "QtyNote, " +
                "ServiceNote, " +
                "ProposalSeq, " +
                "ProposalMenuItem, " +
                "ProposalTitleFlg, " +
                "ProposalHideFlg, " +
                "IngredSelFlg, " +
                "IngredSel, " +
				DB.PutDtTm(Tm) + ", " +
				DB.PutStr(g.User) + " " +
				"From mcJobFood Where JobRno = " + JobRno;
			db.Exec(Sql);

			Sql =
                "Insert Into mcJobServices (" +
                "JobRno, " +
                "ServiceSeq, " +
                "ServiceItem, " +
                "MCCRespFlg, " +
                "CustomerRespFlg, " +
                "Note, " +
                "CreatedDtTm, " +
                "CreatedUser) Select " +
				NewJobRno + ", " +
                "ServiceSeq, " +
                "ServiceItem, " +
                "MCCRespFlg, " +
                "CustomerRespFlg" +
                ", Note, " +
				DB.PutDtTm(Tm) + ", " +
				DB.PutStr(g.User) + " " +
				"From mcJobServices Where JobRno = " + JobRno;
			db.Exec(Sql);

            Sql =
                "Insert Into mcJobDishes (" +
                "JobRno, " +
                "DishSeq, " +
                "DishItem, " +
                "Qty, " +
                "Note, " +
                "CreatedDtTm, " +
                "CreatedUser) Select " +
                NewJobRno + ", " +
                "DishSeq, " +
                "DishItem, " +
                "Qty, " +
                "Note, " +
                DB.PutDtTm(Tm) + ", " +
                DB.PutStr(g.User) + " " +
                "From mcJobDishes Where JobRno = " + JobRno;
            db.Exec(Sql);

            Sql =
                "Insert Into mcJobSupplies (" +
                "JobRno, " +
                "SupplySeq, " +
                "SupplyItem, " +
                "Qty, " +
                "Note, " +
                "CreatedDtTm, " +
                "CreatedUser) Select " +
                NewJobRno + ", " +
                "SupplySeq, " +
                "SupplyItem, " +
                "Qty, " +
                "Note, " +
                DB.PutDtTm(Tm) + ", " +
                DB.PutStr(g.User) + " " +
                "From mcJobSupplies Where JobRno = " + JobRno;
            db.Exec(Sql);

            Sql = String.Format("Select Rno From JobInvoicePrices Where JobRno = {0} Order By Seq", JobRno);
            DataTable dt = db.DataTable(Sql);
            if (dt != null)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    Sql = String.Format("Insert Into JobInvoicePrices (" +
                        "Rno, " +
                        "JobRno, " +
                        "Seq, " +
                        "PriceType, " +
                        "InvPerPersonCount, " +
                        "InvPerPersonDesc, " +
                        "InvPerPersonPrice, " +
                        "InvPerPersonExtPrice, " +
                        "InvPerItemCount, " +
                        "InvPerItemDesc, " +
                        "InvPerItemPrice, " +
                        "InvPerItemExtPrice, " +
                        "InvAllInclDesc, " +
                        "InvAllInclPrice, " +
                        "InvAllInclExtPrice, " +
                        "PropPerPersonDesc, " +
                        "PropPerPersonDescCustFlg, " +
                        "PropPerPersonOptions, " +
                        "PropPerItemDesc, " +
                        "PropPerItemDescCustFlg, " +
                        "PropAllInclDesc, " +
                        "PropAllInclDescCustFlg, " +
                        "CreatedDtTm, " +
                        "CreatedUser) Select " +
                        "{1}, " +
                        "{2}, " +
                        "Seq, " +
                        "PriceType, " +
                        "InvPerPersonCount, " +
                        "InvPerPersonDesc, " +
                        "InvPerPersonPrice, " +
                        "InvPerPersonExtPrice, " +
                        "InvPerItemCount, " +
                        "InvPerItemDesc, " +
                        "InvPerItemPrice, " +
                        "InvPerItemExtPrice, " +
                        "InvAllInclDesc, " +
                        "InvAllInclPrice, " +
                        "InvAllInclExtPrice, " +
                        "PropPerPersonDesc, " +
                        "PropPerPersonDescCustFlg, " +
                        "PropPerPersonOptions, " +
                        "PropPerItemDesc, " +
                        "PropPerItemDescCustFlg, " +
                        "PropAllInclDesc, " +
                        "PropAllInclDescCustFlg, " +
                        "{3}, " +
                        "{4} " +
                        "From JobInvoicePrices Where Rno = {0}",
                        DB.Int32(dr["Rno"]), 
                        db.NextRno("JobInvoicePrices", "Rno"),
                        NewJobRno,
                        DB.PutDtTm(Tm),
                        DB.PutStr(g.User));
                    db.Exec(Sql);
                }
            }

            LoadList(NewJobRno);
			ReadValues(NewJobRno);
			//SelectListsData();

			FocusField = "txtJobDate";
            fNew = true;
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected async void btnDelete_Click(object sender, System.EventArgs e)
	{
		JobRno = Str.Num(txtJobRno.Text);

        if (JobRno != 0)
        {
            string Sql = "Select SlingShiftId From mcJobs Where JobRno = " + JobRno;
            int SlingShiftId = db.SqlNum(Sql);
            if (SlingShiftId > 0)
            {
                Sling Sling = new Sling();

                if (await Sling.Login(Sling.Email, Sling.Password))
                {
                    await Sling.DeleteShift(SlingShiftId);
                }
            }

            try
            {
				Sql = "Delete From mcJobCrew Where JobRno = " + JobRno;
				db.Exec(Sql);

				Sql = "Delete From mcJobFood Where JobRno = " + JobRno;
				db.Exec(Sql);

				Sql = "Delete From mcJobServices Where JobRno = " + JobRno;
				db.Exec(Sql);

				Sql = "Delete From mcJobSupplies Where JobRno = " + JobRno;
				db.Exec(Sql);

				Sql = "Delete From mcJobs Where JobRno = " + JobRno;
				db.Exec(Sql);
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}

		ClearValues();
		LoadList(0);
		Session["JobRno"] = "";
		FocusField = "txtCustomer";
	}

	protected string JobDates()
	{
		string Dates = "";
		string Sql =
			"Select JobDate From mcJobs " +
			"Where JobDate >= " + DB.PutDtTm(DateTime.Today) +
			"And JobRno <> " + JobRno + " " +
			"And ContactRno In (" +
            "Select ContactRno From mcJobs Where JobRno = " + JobRno + ") " +
			"Order By JobDate";

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				Dates += ", new Date('" + Fmt.Dt(DB.DtTm(dr["JobDate"])) + "')";
			}
			if (Dates.Length > 0)
			{
				Dates = Dates.Substring(2);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		return Dates;
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

	protected string Warning()
	{
		string Warning = "";

		if ((string)Session["Warn"] == "SelectJob")
		{
			Session["Warn"] = null;
			Warning = "Please select a Job.";
		}
		else
			if (fQtyWarning)
			{
				Warning = QtyChangeWarning();
			}

		if (Warning.Length > 0)
		{
			Warning = "alert(\"" + Warning + "\");";
		}

		return Warning;
	}

	protected string QtyChangeWarning()
	{
		string Warning = "";
		if (fQtyWarning)
		{
			string Sql = string.Format("Select MenuItem, Qty From mcJobFood Where JobRno = {0} And Qty != 0", JobRno);
			try
			{
				DataTable dt = DB.DBDataTable(Sql);
				if (dt.Rows.Count > 0)
				{
					Warning = "The Number of servings has changed. Please double check the 'Food' page for any quantities based on the old number of servings.\\n";
					foreach (DataRow dr in dt.Rows)
					{
						Warning += "\\n\" +\n\"" + DB.Str(dr["MenuItem"]) + " - " + Fmt.Num(DB.Int32(dr["Qty"]));
					}
				}
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
		return Warning;
	}

	protected string CustomerData()
	{
        string Sql = string.Empty;
        StringBuilder sb = new StringBuilder();

        try
        {
            //foreach (string Customer in aCustomers)
            //{
            //sb.AppendFormat("{0}{{ value: \"{1}\", contacts: [", (sb.Length == 0 ? string.Empty : ", "), Str.js(Customer));

            //string Sql = "Select Distinct ContactName, ContactPhone, ContactCell, ContactFax, ContactEmail From mcJobs Where Customer = " + DB.PutStr(Customer) + "Order By ContactName";
            Sql =
                "Select c.CustomerRno, c.Name, t.ContactRno, t.Name as Contact, t.Phone, t.Cell, t.Fax, t.Email, " +
                "Case " +
                "	When IsNull(t.Phone, '') <> '' And IsNull(t.Email, '') <> '' Then 3 " +
                "	When IsNull(t.Phone, '') <> '' And IsNull(t.Email, '') = ''  Then 2 " +
                "	When IsNull(t.Phone, '') = ''  And IsNull(t.Email, '') <> '' Then 1 " +
                "	Else 0 " +
                "End As SortValue " +
                "From Customers c Left Join Contacts t on c.CustomerRno = t.CustomerRno " +
                "Order By c.Name, " +
                "Case " +
                "	When IsNull(t.Phone, '') <> '' And IsNull(t.Email, '') <> '' Then 3 " +
                "	When IsNull(t.Phone, '') <> '' And IsNull(t.Email, '') = ''  Then 2 " +
                "	When IsNull(t.Phone, '') = ''  And IsNull(t.Email, '') <> '' Then 1 " +
                "	Else 0 " +
                "End Desc, " +
                "t.Name";
            DataTable dtContacts = db.DataTable(Sql);

            int PrevCustomerRno = 0;
            string PrevCustomer = string.Empty;
            int iContact = 0;
            foreach (DataRow dr in dtContacts.Rows)
            {
                int CustomerRno = DB.Int32(dr["CustomerRno"]);
                string Customer = DB.Str(dr["Name"]);
                if (CustomerRno != PrevCustomerRno)
                {
                    sb.AppendFormat("{0}{{ \"value\": \"{1}\", \"id\":{2}, \"contacts\": [", (sb.Length == 0 ? string.Empty : "]}, "), Str.js(Customer), CustomerRno);
                    PrevCustomerRno = CustomerRno;
                    iContact = 0;
                }

                sb.AppendFormat("{0}{{ \"value\": \"{1}{7}\", \"id\":{2}, \"phone\": \"{3}\", \"cell\": \"{4}\", \"fax\": \"{5}\", \"email\": \"{6}\" }}",
                    (iContact++ == 0 ? string.Empty : ", "),
                    Str.js(DB.Str(dr["Contact"])),
                    DB.Int32(dr["ContactRno"]),
                    Str.js(DB.Str(dr["Phone"])),
                    Str.js(DB.Str(dr["Cell"])),
                    Str.js(DB.Str(dr["Fax"])),
                    Str.js(DB.Str(dr["Email"])),
                    (DB.Int32(dr["SortValue"]) == 3 ? " *" : string.Empty));
            }
            sb.Append("]}");
            //}

        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return string.Format("[{0}]", sb.ToString());
	}

    protected string ContactData()
    {
        StringBuilder sb = new StringBuilder();
        string Sql = 
            "Select ContactRno, Name, Phone, Cell, Fax, Email, " +
            "Case " +
            "	When IsNull(Phone, '') <> '' And IsNull(Email, '') <> '' Then 3 " +
            "	When IsNull(Phone, '') <> '' And IsNull(Email, '') = ''  Then 2 " +
            "	When IsNull(Phone, '') = ''  And IsNull(Email, '') <> '' Then 1 " +
            "	Else 0 " +
            "End As SortValue " +
            "From Contacts Where CustomerRno Is Null " +
            "Order By " +
            "Case " +
            "	When IsNull(Phone, '') <> '' And IsNull(Email, '') <> '' Then 3 " +
            "	When IsNull(Phone, '') <> '' And IsNull(Email, '') = ''  Then 2 " +
            "	When IsNull(Phone, '') = ''  And IsNull(Email, '') <> '' Then 1 " +
            "	Else 0 " +
            "End Desc, " +
            "Name";

        try
        {
            string Comma = "";
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendFormat("{0}{{ \"value\": \"{1}{7}\", \"id\":{2}, \"phone\": \"{3}\", \"cell\": \"{4}\", \"fax\": \"{5}\", \"email\": \"{6}\" }}",
                    Comma,
                    Str.js(DB.Str(dr["Name"])),
                    DB.Int32(dr["ContactRno"]),
                    Str.js(DB.Str(dr["Phone"])),
                    Str.js(DB.Str(dr["Cell"])),
                    Str.js(DB.Str(dr["Fax"])),
                    Str.js(DB.Str(dr["Email"])),
                    (DB.Int32(dr["SortValue"]) == 3 ? " *" : string.Empty));
                Comma = ", ";
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return string.Format("[{0}]", sb.ToString());
    }

    protected async void SaveAtSling()
	{
        bool fSaveAtSling = true;
        SaveOrigStartDateTime = hfOrigStartDateTime.Value;

        if (chkProposal.Checked || txtCrew.Text.Length == 0)
        {
            fSaveAtSling = false;
        }

        DateTime dtJob = Str.DtTm(txtJobDate.Text);
        DateTime tmBeg = StartDateTime();

        StringBuilder sbLog = new StringBuilder()
            .AppendFormat("Job #{0} on {1} at {2}\n", JobRno, Fmt.Dt(dtJob), Fmt.Tm(tmBeg));

        if (fSaveAtSling && dtJob > DateTime.MinValue && tmBeg > DateTime.MinValue)
        {
            ArrayList aSummary = new ArrayList();
            foreach (string SummaryPart in new string[]
            {
                (txtCustomer.Text.Trim().Length > 0 ? txtCustomer.Text.Trim() : txtContact.Text.Trim()),
                string.Format("Job #{0}", JobRno),
                string.Format("{0} Servings", txtServingMen.Text),
                txtLocation.Text,
                txtCrew.Text,
                txtProdNotes.Text
            })
            {
                if (SummaryPart.Length > 0)
                {
                    aSummary.Add(SummaryPart);
                }
            }
            string Summary = string.Join("\\n", aSummary.ToArray());

            DateTime tmEnd = Str.DtTm(txtEndTime.Text);
            if (tmEnd > DateTime.MinValue)
            {
                tmEnd = new DateTime(dtJob.Year, dtJob.Month, dtJob.Day, tmEnd.Hour, tmEnd.Minute, tmEnd.Second).AddHours(2);   // end shift time = end meal time + 2 hours
            }
            else
            {
                DateTime tmBegMeal = Str.DtTm(txtMealTime.Text);
                tmEnd = new DateTime(dtJob.Year, dtJob.Month, dtJob.Day, tmBegMeal.Hour, tmBegMeal.Minute, tmBegMeal.Second).AddHours(4);      // if no end meal time, then beg meal time + 4 hours
            }

            sbLog.AppendFormat("{0} - {1}\n{2}\n", Fmt.Tm12Hr(tmBeg), Fmt.Tm12Hr(tmEnd), Summary.Replace("\\n", "\n"));

            Sling Sling = new Sling();

            if (await Sling.Login(Sling.Email, Sling.Password))
            {
                int SlingShiftId = GetShiftId();

                // if not previously saved on Sling
                if (SlingShiftId == 0)
                {
                    // create the Sling Shift
                    SlingShiftId = await Sling.CreateShift(tmBeg, tmEnd, Sling.Location, Sling.Position, Summary);
                    sbLog.AppendFormat("Create Shift {0}\n", SlingShiftId);

                    if (SlingShiftId != 0)
                    { 
                        SetShiftId(SlingShiftId);
                        await Sling.PublishShift(SlingShiftId);
                        SendNewJobNotification(Sling, tmBeg, true);
                        sbLog.AppendLine("Send New Job Notification");
                    }
                    else
                    {
                        if (Sling.ErrorMessage.Length > 0)
                        {
                            Response.Write(Sling.ErrorMessage);
                            sbLog.AppendLine(Sling.ErrorMessage);
                        }
                    }
                }
                else
                {
                    // update the Sling Shift
                    await Sling.UpdateShift(SlingShiftId, tmBeg, tmEnd, Sling.Location, Sling.Position, Summary);
                    await Sling.PublishShift(SlingShiftId);
                    sbLog.AppendLine("Update Shift");

                    DateTime OrigStartDateTime = DateTime.Parse(SaveOrigStartDateTime);
                    if (tmBeg != OrigStartDateTime)
                    {
                        SendNewJobNotification(Sling, tmBeg, false);
                        sbLog.AppendLine("Send New Job Notification");
                    }
                    if (Sling.ErrorMessage.Length > 0)
                    {
                        Response.Write(Sling.ErrorMessage);
                        sbLog.AppendLine(Sling.ErrorMessage);
                    }
                }
            }
            else
            {
                sbLog.AppendLine("Sling login failed");
                sbLog.AppendLine(Sling.ErrorMessage);
            }

            LogData(sbLog);
        }
        else
        {
            // shouldn't be saved at Sling
            // if this job has been previously saved on Sling, remove it
            int SlingShiftId = GetShiftId();
            if (SlingShiftId != 0)
            {
                sbLog.AppendFormat("Empty Crew with a Sling Shift ID - Remove Sling Shift\nSling Shift ID {0}\n", SlingShiftId);
                Sling Sling = new Sling();

                if (await Sling.Login(Sling.Email, Sling.Password))
                {
                    await Sling.DeleteShift(SlingShiftId);   // delete on Sling
                    SetShiftId(0);
                    await Sling.PublishShift(SlingShiftId);
                    sbLog.AppendLine("Sling Shift removed");
                }
                else
                {
                    sbLog.AppendLine("Sling login failed");
                    sbLog.AppendLine(Sling.ErrorMessage);
                }

                LogData(sbLog);
            }
        }
    }

    protected DateTime StartDateTime()
    {
        DateTime dtJob = Str.DtTm(txtJobDate.Text);
        DateTime tmBeg = Str.DtTm(
            txtLoadTime.Text.Length > 0 ? txtLoadTime.Text :
            txtDepartTime.Text.Length > 0 ? txtDepartTime.Text : 
            txtArrivalTime.Text.Length > 0 ? txtArrivalTime.Text : 
            txtMealTime.Text.Length > 0 ? txtMealTime.Text : 
            "00:00:00");
        tmBeg = new DateTime(dtJob.Year, dtJob.Month, dtJob.Day, tmBeg.Hour, tmBeg.Minute, tmBeg.Second);

        return tmBeg;
    }

    protected int GetShiftId()
    {
        int ID = 0;

        string Sql = string.Format("Select SlingShiftId From mcJobs Where JobRno = {0}", JobRno);
        try
        {
            ID = db.SqlNum(Sql);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return ID;
    }

    protected void SetShiftId(int ID)
    {
        string Sql = string.Format("Update mcJobs Set SlingShiftId = {1} Where JobRno = {0}", JobRno, (ID != 0 ?  ID.ToString() : "NULL"));
        try
        {
            db.Exec(Sql);
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    protected async void SendNewJobNotification(Sling Sling, DateTime tmBeg, bool fNew)
    {
        // test if Sling Admin should be notified of a new job 
        if (tmBeg > DateTime.Now)
        {
            DateTime dtCheck = new DateTime(tmBeg.Year, tmBeg.Month, tmBeg.Day);    // find the beginning of the day
            dtCheck = dtCheck.AddDays(-7);                                          // go back a week
            dtCheck = dtCheck.AddDays(6 - (int)dtCheck.DayOfWeek);                  // slide up to the next saturday
            if (DateTime.Today >= dtCheck)                                          // is this new job being added after the previous planning day for the comming week
            {
                // notify the planner about this new job
                // lookup user's id
                
                Sling.User User = await Sling.FindUserBy("Email", Sling.UserEmail);
                if (User != null)
                {
                    string[] Days = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
                    string Start = string.Format("{0} {1}", Days[(int)tmBeg.DayOfWeek], tmBeg.ToString("h:mmtt M/d/yyyy").ToLower());
                    string Msg = string.Format(
                        "{{" +
                            "\"content\": \"{0} Job\\n{1}\\n{2}\\nJob #{3}\\n{4}://{5}{6}/DailyJobs.aspx?JobRno={7}\"," +
                            "\"name\": \"{0} Job\"," +
                            "\"personas\": " +
                                "[{{" +
                                    "\"id\": {8}," +
                                    "\"type\": \"user\"" +
                                "}}]," +
                            "\"private\": true" +
                        "}}",
                        (fNew ? "New" : "Updated"),
                        Start,
                        txtCustomer.Text,
                        JobRno,
                        Sling.Scheme,
                        Sling.Host,
                        Sling.Port,
                        HttpUtility.UrlEncode(Misc.MangleNumber(JobRno)),
                        User.ID);
                    //Debug.WriteLine(Msg);
                    await Sling.AddToPrivateConversation(Msg);
                }
            }
        }
    }

    private static DateTime dtLog;
    private static string sLogName;

    protected void LogData(StringBuilder sbLog)
    {
        if (dtLog != DateTime.Today)
        {
            dtLog = DateTime.Today;
            sLogName = String.Format("Sling.{0:yyyyMMdd}.log", dtLog);
        }

        File.AppendAllText(Path.Combine(Server.MapPath("~/Logs"), sLogName), 
            String.Format("--------------------------------------------------------------------------------\n{0}\n{1}", Fmt.DtTm12HrSec(DateTime.Now), sbLog.ToString()));
    }
}