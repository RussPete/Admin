using System;
using System.Data;
using System.IO;
using System.Net.Mail;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Utils;
using Globals;

public partial class Invoice : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";
	protected Int32 JobRno;
	protected bool fNew = false;
	protected SelectList slAdj1Desc;
	protected SelectList slAdj2Desc;
	protected SelectList slInvMsg;
	protected SelectList slInvDeliveryMethod;
	protected SelectList slPaymentMsg;
	protected SelectList slInvPmtMethod;
	//protected SelectList slCCType;
	protected Utils.Calendar calInvDate;
	protected Utils.Calendar calInvDeliveryDate;
	protected Utils.Calendar calInvPmtDate;
	protected bool fPrinted = false;
	protected bool fJobCancelled = false;
	protected Int32 EventCount;
	protected string EventCountInfo;
	protected decimal PricePerPerson;
	protected bool fSaveFinal = false;
	protected int DefaultServings;
	protected const string CreditCard = "CreditCard";
	protected bool fChargeCCFee = false;
    protected string USAePayUrl;
    protected string USAePayKey;

    private void Page_Init(object sender, System.EventArgs e)
	{
		WebConfig Conf = new WebConfig();
		fChargeCCFee = Conf.Bool("Charge CC Fee");
        USAePayUrl = Conf.Str("USAePay Gateway URL");
        USAePayKey = Conf.Str("USAePay Gateway Key");

        db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        Int32 ParmJobRno = Str.Num(Request.Params["JobRno"]);
		if (ParmJobRno != 0)
		{
			JobRno = ParmJobRno;
			Session["JobRno"] = JobRno.ToString();
		}

		if (JobRno == 0)
		{
			JobRno = Pg.JobRno();
		}

		SelectListsSetup();

		calInvDate = new Utils.Calendar("InvDate", ref txtInvDate);
		calInvDate.ImageButton(imgInvDate);

		//calInvDeliveryDate = new Utils.Calendar("InvDeliveryDate", ref txtInvDeliveryDate);
		//calInvDeliveryDate.ImageButton(imgInvDeliveryDate);

		//calInvPmtDate = new Utils.Calendar("InvPmtDate", ref txtInvPmtDate);
		//calInvPmtDate.ImageButton(imgInvPmtDate);
    }

	private void Page_Load(object sender, System.EventArgs e)
	{
		// Put user code to initialize the page here
		if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.Jobs.Title;

            if (Request.Params["UMstatus"] != null)
            {
                SaveCreditCardPayment();
            }

            Setup();
        }
        else
		{
			if (hfEmail.Value == "true")
			{
				SendEmail();
			}
		}

		SelectListsData();
	}

	private void Setup()
	{
		ReadValues();

		txtInvDate.Attributes.Add("onChange", "ValidateDateField('txtInvDate');CheckDate('txtInvDate');SetDirty();");
		//txtInvEventCount.Attributes.Add("onChange", "ValidateNum(this);CheckEventCount();SetDirty();");
		//txtInvPricePerPerson.Attributes.Add("onChange", "ValidateCurrency(this);CheckPricePerPerson();SetDirty();");
		txtSubTotTaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtServiceSubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtServiceAmt');SetDirty();");
		txtServiceAmt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtServiceSubTotPct');SetDirty();");
		txtServiceTaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtDeliverySubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtDeliveryAmt');SetDirty();");
		txtDeliveryAmt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtDeliverySubTotPct');SetDirty();");
		txtDeliveryTaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtChinaSubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtChinaAmt');SetDirty();");
		txtChinaAmt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtChinaSubTotPct');SetDirty();");
		txtChinaTaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtAddServiceSubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtAddServiceAmt');SetDirty();");
		txtAddServiceAmt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtAddServiceSubTotPct');SetDirty();");
		txtAddServiceTaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtFuelTravelSubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtFuelTravelAmt');SetDirty();");
		txtFuelTravelAmt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtFuelTravelSubTotPct');SetDirty();");
		txtFuelTravelTaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtFacilitySubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtFacilityAmt');SetDirty();");
		txtFacilityAmt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtFacilitySubTotPct');SetDirty();");
		txtFacilityTaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtGratuitySubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtGratuityAmt');SetDirty();");
		txtGratuityAmt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtGratuitySubTotPct');SetDirty();");
		txtGratuityTaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
        txtVoluntaryGratuitySubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtVoluntaryGratuityAmt');SetDirty();");
        txtVoluntaryGratuityAmt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtVoluntaryGratuitySubTotPct');SetDirty();");
        txtRentalsSubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtRentalsAmt');SetDirty();");
		txtRentalsAmt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtRentalsSubTotPct');SetDirty();");
		txtRentalsTaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtAdj1SubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtAdj1Amt');SetDirty();");
		txtAdj1Desc.Attributes.Add("onChange", "SetDirty();");
		txtAdj1Amt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtAdj1SubTotPct');SetDirty();");
		txtAdj1TaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtAdj2SubTotPct.Attributes.Add("onChange", "ValidatePct(this, 3);ChangePct(this, 'txtAdj2Amt');SetDirty();");
		txtAdj2Desc.Attributes.Add("onChange", "SetDirty();");
		txtAdj2Amt.Attributes.Add("onChange", "ValidateCurrency(this);ChangeAmt(this, 'txtAdj2SubTotPct');SetDirty();");
		txtAdj2TaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtCCFeePct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		txtCCFeeTaxPct.Attributes.Add("onChange", "ValidatePct(this, 3);InvoiceTotal();SetDirty();");
		//txtDepositDate.Attributes.Add("style", "text-align: left;");
		//txtDepositAmt.Attributes.Add("onChange", "ValidateCurrency(this);InvoiceTotal();SetDirty();");
		//txtDepositRef.Attributes.Add("style", "text-align: left;");
		//txtInvMessage.Attributes.Add("onChange", "SetDirty();");
		//txtInvDeliveryMethod.Attributes.Add("onChange", "CheckDeliveryMethod();SetDirty();");
		//txtInvDeliveryDate.Attributes.Add("onChange", "ValidateDateField('txtInvDeliveryDate');CheckDate('txtInvDeliveryDate');SetDirty();");
		//txtInvEmail.Attributes.Add("onBlur", "CheckEmailDeliveryMethod();SetDirty();");
		//txtInvFax.Attributes.Add("onBlur", "CheckFaxDeliveryMethod();SetDirty();");
		//txtPaymentMsg.Attributes.Add("onChange", "SetDirty();");
		//txtInvPmtDate.Attributes.Add("onChange", "CheckDate('txtInvPmtDate');SetDirty();");
		//txtInvPmtMethod.Attributes.Add("onChange", "SetDirty();");
		//txtCheckRef.Attributes.Add("onChange", "SetDirty();");
		//txtCCType.Attributes.Add("onChange", "SetDirty();");
		//txtCCNum.Attributes.Add("onChange", "SetDirty();");
		//txtCCExpDt.Attributes.Add("onChange", "CheckCCExpDate();SetDirty();");
		//txtCCSecCode.Attributes.Add("onChange", "SetDirty();");
		//txtCCName.Attributes.Add("onChange", "SetDirty();");
		//txtCCStreet.Attributes.Add("onChange", "SetDirty();");
		//txtCCZip.Attributes.Add("onChange", "SetDirty();");
		//txtCCBatchID.Attributes.Add("onChange", "SetDirty();");
		//txtCCInvNum.Attributes.Add("onChange", "SetDirty();");
		//txtCCApprCode.Attributes.Add("onChange", "CheckCCFailed();SetDirty();");
		//txtCCAVSResp.Attributes.Add("onChange", "SetDirty();");
		//txtCCSettleDate.Attributes.Add("onChange", "CheckDate('txtCCSettleDate');SetDirty();");
		//chkFailedFlg.Attributes.Add("onClick", "CheckCCFailed();SetDirty();");
		//txtFailedMsg.Attributes.Add("onChange", "SetDirty();");

		btnSave.Attributes.Add("onClick", "ClearDirty();");
		//btnSave1.Attributes.Add("onClick", "ClearDirty();");
		chkCustomerCopy.Attributes.Add("OnClick", "SyncCustomerCopy(this);");
		//chkCustomerCopy1.Attributes.Add("OnClick", "SyncCustomerCopy(this);");
		chkFileCopy.Attributes.Add("OnClick", "SyncFileCopy(this);");
		//chkFileCopy1.Attributes.Add("OnClick", "SyncFileCopy(this);");

		chkCustomerCopy.Checked = true;
		//chkCustomerCopy1.Checked = true;
		chkFileCopy.Checked = false;
		//chkFileCopy1.Checked = false;
	}

	private void SelectListsSetup()
	{
		SelectList.Clear();

		slAdj1Desc = new SelectList("Adj1Desc", ref txtAdj1Desc);
		slAdj1Desc.ImageButton(ddAdj1Desc);

		slAdj2Desc = new SelectList("Adj2Desc", ref txtAdj2Desc);
		slAdj2Desc.ImageButton(ddAdj2Desc);

		//slInvMsg = new SelectList("InvMessage", ref txtInvMessage);
		//slInvMsg.ImageButton(ddInvMessage);

		//slInvDeliveryMethod = new SelectList("InvDeliveryMethod", ref txtInvDeliveryMethod);
		//slInvDeliveryMethod.ImageButton(ddInvDeliveryMethod);

		//slPaymentMsg = new SelectList("PaymentMsg", ref txtPaymentMsg);
		//slPaymentMsg.ImageButton(ddPaymentMsg);

		//slInvPmtMethod = new SelectList("InvPmtMethod", ref txtInvPmtMethod);
		//slInvPmtMethod.ImageButton(ddInvPmtMethod);

		//slCCType = new SelectList("CCType", ref txtCCType);
		//slCCType.ImageButton(ddCCType);
	}

	private void SelectListsData()
	{
		String Sql;

		Sql =
			"Select Distinct Adj1Desc From mcJobs " +
			"Union " +
			"Select Distinct Adj2Desc As Adj1Desc From mcJobs " +
			"Order By Adj1Desc";

		slAdj1Desc.ClearValues();
		slAdj1Desc.AddDBValues(db, Sql);

		slAdj2Desc.ClearValues();
		slAdj2Desc.AddDBValues(db, Sql);

		//Sql =
		//    "Select Distinct InvoiceMsg From mcJobs Order By InvoiceMsg";
		//slInvMsg.ClearValues();
		//slInvMsg.AddValue("Thank you for your business.");
		//slInvMsg.AddValue("Thank you for your prompt payment.");
		//slInvMsg.AddValue("We look forward to serving you again in the near future.");
		//slInvMsg.AddDBValues(db, Sql);

		//Sql =
		//    "Select Distinct InvDeliveryMethod From mcJobs Order By InvDeliveryMethod";
		//slInvDeliveryMethod.ClearValues();
		//slInvDeliveryMethod.AddValue("Customer Pick-up");
		//slInvDeliveryMethod.AddValue("Deliver with Job");
		//slInvDeliveryMethod.AddValue("Email");
		//slInvDeliveryMethod.AddValue("Fax");
		//slInvDeliveryMethod.AddValue("US Mail");
		//slInvDeliveryMethod.AddDBValues(db, Sql);

		//Sql =
		//    "Select Distinct PaymentMsg From mcJobs Order By PaymentMsg";
		//slPaymentMsg.ClearValues();
		//slPaymentMsg.AddValue("You will see the name PeteSoft, LLC on your credit card statement for these charges.");
		//slPaymentMsg.AddValue("You will see the name Rust Coin & Gift on your credit card statement for these charges.");
		//slPaymentMsg.AddDBValues(db, Sql);

		//Sql =
		//	"Select Distinct InvPmtMethod From mcJobs Order By InvPmtMethod";
		//slInvPmtMethod.ClearValues();
		//slInvPmtMethod.AddValue("Check");
		//slInvPmtMethod.AddValue("Cash");
		//slInvPmtMethod.AddValue("CC");
		//slInvPmtMethod.AddDBValues(db, Sql);

		//Sql =
		//	"Select Distinct CCType From mcJobs Order By CCType";
		//slCCType.ClearValues();
		//slCCType.AddDBValues(db, Sql);
	}

	private void ClearValues()
	{
		lblJobRno.Text =
		lblCustomer.Text =
		lblName.Text =
		txtInvDate.Text =
		txtJobDate.Text =
		//txtInvEventCount.Text =
		//txtInvPricePerPerson.Text =
		txtInvSubTotal.Text =
		txtSubTotTaxPct.Text =
		txtServiceSubTotPctFlg.Value =
		txtServiceSubTotPct.Text =
		txtServiceAmt.Text =
		txtServiceTaxPct.Text =
		txtDeliverySubTotPctFlg.Value =
		txtDeliverySubTotPct.Text =
		txtDeliveryAmt.Text =
		txtDeliveryTaxPct.Text =
		txtChinaSubTotPctFlg.Value =
		txtChinaSubTotPct.Text =
		txtChinaAmt.Text =
		txtChinaTaxPct.Text =
		txtAddServiceSubTotPctFlg.Value =
		txtAddServiceSubTotPct.Text =
		txtAddServiceAmt.Text =
		txtAddServiceTaxPct.Text =
		txtFuelTravelSubTotPctFlg.Value =
		txtFuelTravelSubTotPct.Text =
		txtFuelTravelAmt.Text =
		txtFuelTravelTaxPct.Text =
		txtFacilitySubTotPctFlg.Value =
		txtFacilitySubTotPct.Text =
		txtFacilityAmt.Text =
		txtFacilityTaxPct.Text =
		txtGratuitySubTotPctFlg.Value =
		txtGratuitySubTotPct.Text =
		txtGratuityAmt.Text =
		txtGratuityTaxPct.Text =
        txtVoluntaryGratuitySubTotPctFlg.Value =
        txtVoluntaryGratuitySubTotPct.Text =
        txtVoluntaryGratuityAmt.Text =
		txtRentalsSubTotPctFlg.Value =
		txtRentalsSubTotPct.Text =
		txtRentalsAmt.Text =
		txtRentalsTaxPct.Text =
		txtRentalsDesc.Text =
		txtCCFeePct.Text =
		txtCCFeeAmt.Text =
		txtCCFeeTaxPct.Text =
		txtPreTaxTotal.Text =
		txtSalesTaxTotal.Text =
		txtInvTotal.Text =
		lblSaveDt.Text =
		lblSaveFinalDt.Text =
		lblPrintDt.Text =
		lblEmailDt.Text = 
		txtInvCreatedDt.Text =
		txtInvCreatedUser.Text =
		txtInvUpdatedDt.Text =
		txtInvUpdatedUser.Text =
        txtDepositInvPrintedDtTm.Text =
        txtDepositInvPrintedUser.Text =
        txtDepositInvEmailedDtTm.Text =
        txtDepositInvEmailedUser.Text =
        txtEmail.Text =
		txtSubject.Text =
		txtMessage.Text = string.Empty;
		//chkFailedFlg.Checked = false;

		//			chkCustomerCopy.Checked		=
		//			chkCustomerCopy1.Checked	= true;
		//			chkFileCopy.Checked			=
		//			chkFileCopy1.Checked		= false;

		EventCount = 0;
		EventCountInfo = "";
		PricePerPerson = 0M;
	}

	private void ReadValues()
	{
		ClearValues();

		String Sql =
            "Select j.InvCreatedDtTm, j.CancelledDtTm, j.InvEventCount, j.NumMenServing, j.NumWomenServing, j.NumChildServing, " +
            "j.InvPricePerPerson, j.PricePerPerson, j.InvDate, " +
            "j.ServiceAmt, j.DeliveryAmt, j.ChinaAmt, j.AddServiceAmt, j.FuelTravelAmt, j.FacilityAmt, j.GratuityAmt, " +
            "j.VoluntaryGratuityAmt, j.RentalsAmt, j.Adj1Amt, j.Adj2Amt, " +
            "j.SubTotTaxPct, j.ServiceTaxPct, j.DeliveryTaxPct, j.ChinaTaxPct, j.AddServiceTaxPct, j.FuelTravelTaxPct, " +
            "j.FacilityTaxPct, j.GratuityTaxPct, j.RentalsTaxPct, j.Adj1TaxPct, j.Adj2TaxPct, j.CCFeeTaxPct, j.Adj1SubTotPct, j.Adj2SubTotPct, " +
            "j.ServiceSubTotPct, j.DeliverySubTotPct, j.ChinaSubTotPct, j.AddServiceSubTotPct, j.FuelTravelSubTotPct, " +
            "j.FacilitySubTotPct, j.GratuitySubTotPct, j.VoluntaryGratuitySubTotPct, j.RentalsSubTotPct, j.CCFeePct, " +
            "j.ServiceSubTotPctFlg, j.DeliverySubTotPctFlg, j.ChinaSubTotPctFlg, j.AddServiceSubTotPctFlg, " +
            "j.FuelTravelSubTotPctFlg, j.FacilitySubTotPctFlg, j.GratuitySubTotPctFlg, j.VoluntaryGratuitySubTotPctFlg, j.RentalsSubTotPctFlg, " +
            "j.Adj1SubTotPctFlg, j.Adj2SubTotPctFlg, " +
            "j.RentalsDesc, j.Adj1Desc, j.Adj2Desc, j.CCPmtFeeFlg, j.CCFeeAmt, j.PmtMethod, j.PONumber, " +
            "j.DepositAmt, j.InvEmail, j.InvFax, j.InvEmailedDtTm, " +
            "j.SubTotAmt, j.JobDate, j.InvCreatedDtTm, j.InvCreatedUser, j.InvUpdatedDtTm, j.InvUpdatedUser, " +
            "j.DepositInvPrintedDtTm, j.DepositInvPrintedUser, j.DepositInvEmailedDtTm, j.DepositInvEmailedUser, " +
            "j.FinalDtTm, j.InvoicedDtTm, j.InvEmailedDtTm, j.PreTaxSubTotAmt, j.SalesTaxTotAmt, j.InvTotAmt, j.InvBalAmt, " +
            "c.Name, c.Email, c.Fax, c.Email, cu.Name as Customer, cu.TaxExemptFlg " +
            "From mcJobs j " +
            "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
            "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
            "Where j.JobRno = " + JobRno;
		DataTable tbl = db.DataTable(Sql);
		if (tbl.Rows.Count > 0)
		{
			DataRow dr = tbl.Rows[0];

			string InvCreatedDt = Fmt.DtTm(DB.DtTm(dr["InvCreatedDtTm"]));
			bool fNew = (InvCreatedDt.Length == 0);

			string CancelledDt = Fmt.DtTm(DB.DtTm(dr["CancelledDtTm"]));
			fJobCancelled = (CancelledDt.Length > 0);

			int InvEventCount = DB.Int32(dr["InvEventCount"]);
			int cMen = DB.Int32(dr["NumMenServing"]);
			int cWomen = DB.Int32(dr["NumWomenServing"]);
			int cChild = DB.Int32(dr["NumChildServing"]);
			EventCount = cMen + cWomen + cChild;
			decimal InvPricePerPerson = DB.Dec(dr["InvPricePerPerson"]);
			PricePerPerson = DB.Dec(dr["PricePerPerson"]);
			DateTime InvDate = DB.DtTm(dr["InvDate"]);

			EventCountInfo =
				"Job Count is " + Fmt.Num(cMen + cWomen + cChild) + " (" +
				Fmt.Num(cMen) + " Men, " +
				Fmt.Num(cWomen) + " Women, " +
				Fmt.Num(cChild) + " Child)";

			decimal ServiceAmt           = DB.Dec(dr["ServiceAmt"]);
			decimal DeliveryAmt          = DB.Dec(dr["DeliveryAmt"]);
			decimal ChinaAmt             = DB.Dec(dr["ChinaAmt"]);
			decimal AddServiceAmt        = DB.Dec(dr["AddServiceAmt"]);
			decimal FuelTravelAmt        = DB.Dec(dr["FuelTravelAmt"]);
			decimal FacilityAmt          = DB.Dec(dr["FacilityAmt"]);
			decimal GratuityAmt          = DB.Dec(dr["GratuityAmt"]);
            decimal VoluntaryGratuityAmt = DB.Dec(dr["VoluntaryGratuityAmt"]);
            decimal RentalsAmt           = DB.Dec(dr["RentalsAmt"]);
			decimal Adj1Amt              = DB.Dec(dr["Adj1Amt"]);
			decimal Adj2Amt              = DB.Dec(dr["Adj2Amt"]);

			decimal SubTotTaxPct        = DB.Dec(dr["SubTotTaxPct"]);
			decimal ServiceTaxPct       = DB.Dec(dr["ServiceTaxPct"]);
			decimal DeliveryTaxPct      = DB.Dec(dr["DeliveryTaxPct"]);
			decimal ChinaTaxPct         = DB.Dec(dr["ChinaTaxPct"]);
			decimal AddServiceTaxPct    = DB.Dec(dr["AddServiceTaxPct"]);
			decimal FuelTravelTaxPct    = DB.Dec(dr["FuelTravelTaxPct"]);
			decimal FacilityTaxPct      = DB.Dec(dr["FacilityTaxPct"]);
			decimal GratuityTaxPct      = DB.Dec(dr["GratuityTaxPct"]);
			decimal RentalsTaxPct       = DB.Dec(dr["RentalsTaxPct"]);
			decimal Adj1TaxPct          = DB.Dec(dr["Adj1TaxPct"]);
			decimal Adj2TaxPct          = DB.Dec(dr["Adj2TaxPct"]);
			decimal CCFeeTaxPct         = DB.Dec(dr["CCFeeTaxPct"]);

			decimal ServiceSubTotPct            = DB.Dec(dr["ServiceSubTotPct"]);
			decimal DeliverySubTotPct           = DB.Dec(dr["DeliverySubTotPct"]);
			decimal ChinaSubTotPct              = DB.Dec(dr["ChinaSubTotPct"]);
			decimal AddServiceSubTotPct         = DB.Dec(dr["AddServiceSubTotPct"]);
			decimal FuelTravelSubTotPct         = DB.Dec(dr["FuelTravelSubTotPct"]);
			decimal FacilitySubTotPct           = DB.Dec(dr["FacilitySubTotPct"]);
			decimal GratuitySubTotPct           = DB.Dec(dr["GratuitySubTotPct"]);
			decimal VoluntaryGratuitySubTotPct  = DB.Dec(dr["VoluntaryGratuitySubTotPct"]);
			decimal RentalsSubTotPct            = DB.Dec(dr["RentalsSubTotPct"]);
			decimal CCFeePct                    = DB.Dec(dr["CCFeePct"]);

			bool ServiceSubTotPctFlg            = DB.Bool(dr["ServiceSubTotPctFlg"]);
			bool DeliverySubTotPctFlg           = DB.Bool(dr["DeliverySubTotPctFlg"]);
			bool ChinaSubTotPctFlg              = DB.Bool(dr["ChinaSubTotPctFlg"]);
			bool AddServiceSubTotPctFlg         = DB.Bool(dr["AddServiceSubTotPctFlg"]);
			bool FuelTravelSubTotPctFlg         = DB.Bool(dr["FuelTravelSubTotPctFlg"]);
			bool FacilitySubTotPctFlg           = DB.Bool(dr["FacilitySubTotPctFlg"]);
			bool GratuitySubTotPctFlg           = DB.Bool(dr["GratuitySubTotPctFlg"]);
			bool VoluntaryGratuitySubTotPctFlg  = DB.Bool(dr["VoluntaryGratuitySubTotPctFlg"]);
			bool RentalsSubTotPctFlg            = DB.Bool(dr["RentalsSubTotPctFlg"]);

			decimal DepositAmt      = DB.Dec(dr["DepositAmt"]);
            string InvEmail         = DB.Str(dr["InvEmail"]);
			string InvFax           = DB.Str(dr["InvFax"]);
			//string InvPmtMethod   = DB.Str(dr["InvPmtMethod"]);
			DateTime dtInvEmailed   = DB.DtTm(dr["InvEmailedDtTm"]);

			if (fNew)
			{
				Sql = "Select * From Settings Where SettingRno = 1";
				try
				{
					InvDate             = DateTime.Today;
					InvEventCount       = EventCount;
					InvPricePerPerson   = PricePerPerson;
					InvEmail            = DB.Str(dr["Email"]);
					InvFax              = DB.Str(dr["Fax"]);
					//InvPmtMethod      = DB.Str(dr["PmtMethod"]);

					DataRow drSettings = db.DataRow(Sql);
					if (drSettings != null)
					{
                        if (SubTotTaxPct        == 0) SubTotTaxPct     = DB.Dec(drSettings["SubTotTaxPct"]);
                        if (GratuityTaxPct      == 0) GratuityTaxPct   = DB.Dec(drSettings["GratuityTaxPct"]);
                        if (ServiceTaxPct       == 0) ServiceTaxPct    = DB.Dec(drSettings["ServiceTaxPct"]);
                        if (DeliveryTaxPct      == 0) DeliveryTaxPct   = DB.Dec(drSettings["DeliveryTaxPct"]);
                        if (ChinaTaxPct         == 0) ChinaTaxPct      = DB.Dec(drSettings["ChinaTaxPct"]);
                        if (AddServiceTaxPct    == 0) AddServiceTaxPct = DB.Dec(drSettings["AddServiceTaxPct"]);
                        if (FuelTravelTaxPct    == 0) FuelTravelTaxPct = DB.Dec(drSettings["FuelTravelTaxPct"]);
                        if (FacilityTaxPct      == 0) FacilityTaxPct   = DB.Dec(drSettings["FacilityTaxPct"]);
                        if (RentalsTaxPct       == 0) RentalsTaxPct    = DB.Dec(drSettings["RentalsTaxPct"]);
                        if (Adj1TaxPct          == 0) Adj1TaxPct       = DB.Dec(drSettings["Adj1TaxPct"]);
                        if (Adj2TaxPct          == 0) Adj2TaxPct       = DB.Dec(drSettings["Adj2TaxPct"]);
                        if (CCFeePct            == 0) CCFeePct         = DB.Dec(drSettings["CCFeePct"]);

                        if (GratuitySubTotPct   == 0) GratuitySubTotPct   = DB.Dec(drSettings["GratuitySubTotPct"]);
                        if (ServiceSubTotPct    == 0) ServiceSubTotPct    = DB.Dec(drSettings["ServiceSubTotPct"]);
                        if (DeliverySubTotPct   == 0) DeliverySubTotPct   = DB.Dec(drSettings["DeliverySubTotPct"]);
                        if (ChinaSubTotPct      == 0) ChinaSubTotPct      = DB.Dec(drSettings["ChinaSubTotPct"]);
                        if (AddServiceSubTotPct == 0) AddServiceSubTotPct = DB.Dec(drSettings["AddServiceSubTotPct"]);
                        if (FuelTravelSubTotPct == 0) FuelTravelSubTotPct = DB.Dec(drSettings["FuelTravelSubTotPct"]);
                        if (FacilitySubTotPct   == 0) FacilitySubTotPct   = DB.Dec(drSettings["FacilitySubTotPct"]);
                        if (RentalsSubTotPct    == 0) RentalsSubTotPct    = DB.Dec(drSettings["RentalsSubTotPct"]);
                        if (CCFeeTaxPct         == 0) CCFeeTaxPct         = DB.Dec(drSettings["CCFeeTaxPct"]);

                        GratuitySubTotPctFlg          = (GratuitySubTotPct          != 0);
						ServiceSubTotPctFlg           = (ServiceSubTotPct           != 0);
						DeliverySubTotPctFlg          = (DeliverySubTotPct          != 0);
						ChinaSubTotPctFlg             = (ChinaSubTotPct             != 0);
						AddServiceSubTotPctFlg        = (AddServiceSubTotPct        != 0);
						FuelTravelSubTotPctFlg        = (FuelTravelSubTotPct        != 0);
						FacilitySubTotPctFlg          = (FacilitySubTotPct          != 0);
						RentalsSubTotPctFlg           = (RentalsSubTotPct           != 0);
					}
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex, Sql);
					Response.Write(Err.Html());
				}
			}

			//decimal InvSubTotal = InvEventCount * InvPricePerPerson + AdditionalPrices(InvEventCount, InvPricePerPerson, SubTotTaxPct);
			AdditionalPrices(InvEventCount, InvPricePerPerson, SubTotTaxPct);
			decimal InvSubTotal = DB.Dec(dr["SubTotAmt"]);

			//CCNum1 = Decrypt(CCNum1);
			//CCNum2 = Decrypt(CCNum2);
			//CCNum3 = Decrypt(CCNum3);

			//CCSecCode1 = Decrypt(CCSecCode1);
			//CCSecCode2 = Decrypt(CCSecCode2);
			//CCSecCode3 = Decrypt(CCSecCode3);

			lblJobRno.Text      = DB.Str(JobRno);
			lblCustomer.Text    = DB.Str(dr["Customer"]);
			lblName.Text        = DB.Str(dr["Name"]);
			txtInvDate.Text     = Fmt.Dt(InvDate);
			txtJobDate.Text     = Fmt.Dt(DB.DtTm(dr["JobDate"]));
			//txtInvEventCount.Text     = Fmt.Num(InvEventCount);
			//txtInvPricePerPerson.Text = Fmt.Dollar(InvPricePerPerson, false);
			//txtInvExtPrice.Text       = Fmt.Dollar(InvEventCount * InvPricePerPerson, false);
			txtInvSubTotal.Text         = Fmt.Dollar(InvSubTotal, false);
			chkSalesTaxExempt.Checked   = DB.Bool(dr["TaxExemptFlg"]);
			txtSubTotTaxPct.Text        = Fmt.Pct(SubTotTaxPct, 3, false);
			txtServiceTaxPct.Text       = Fmt.Pct(ServiceTaxPct, 3, false);
			txtDeliveryTaxPct.Text      = Fmt.Pct(DeliveryTaxPct, 3, false);
			txtChinaTaxPct.Text         = Fmt.Pct(ChinaTaxPct, 3, false);
			txtAddServiceTaxPct.Text    = Fmt.Pct(AddServiceTaxPct, 3, false);
			txtFuelTravelTaxPct.Text    = Fmt.Pct(FuelTravelTaxPct, 3, false);
			txtFacilityTaxPct.Text      = Fmt.Pct(FacilityTaxPct, 3, false);
			txtGratuityTaxPct.Text      = Fmt.Pct(GratuityTaxPct, 3, false);
			txtRentalsTaxPct.Text       = Fmt.Pct(RentalsTaxPct, 3, false);
			txtRentalsDesc.Text         = DB.Str(dr["RentalsDesc"]);
			txtAdj1Desc.Text            = DB.Str(dr["Adj1Desc"]);
			txtAdj1TaxPct.Text          = Fmt.Pct(Adj1TaxPct, 3, false);
			txtAdj2Desc.Text            = DB.Str(dr["Adj2Desc"]);
			txtAdj2TaxPct.Text          = Fmt.Pct(Adj2TaxPct, 3, false);
			chkCCPmtFee.Checked         = (DB.Bool(dr["CCPmtFeeFlg"]) || dr["CCPmtFeeFlg"] == DBNull.Value && DB.Str(dr["PmtMethod"]) == "CC");
			txtCCFeePct.Text            = Fmt.Pct(CCFeePct, 3, false);
			txtCCFeeTaxPct.Text         = Fmt.Pct(CCFeeTaxPct, 3, false);

            txtDepositAmt.Text = 
            hfDepositAmt.Value = Fmt.Dollar(DepositAmt, false);
            decimal CCFeeAmt = DB.Dec(dr["CCFeeAmt"]);
			if (CCFeeAmt != 0)
				fChargeCCFee    = true;
			txtCCFeeAmt.Text    = Fmt.Dollar(CCFeeAmt);
			txtCCFeePct.Text    = Fmt.Pct(CCFeePct, 3, false);
			txtCCFeeTaxPct.Text = Fmt.Pct(CCFeeTaxPct, 3, false);

            Payments();

			txtInvCreatedDt.Text            = Fmt.DtTm(DB.DtTm(dr["InvCreatedDtTm"]));
			txtInvCreatedUser.Text          = DB.Str(dr["InvCreatedUser"]);
			txtInvUpdatedDt.Text            = Fmt.DtTm(DB.DtTm(dr["InvUpdatedDtTm"]));
			txtInvUpdatedUser.Text          = DB.Str(dr["InvUpdatedUser"]);
            txtDepositInvPrintedDtTm.Text   = Fmt.DtTm(DB.DtTm(dr["DepositInvPrintedDtTm"]));
            txtDepositInvPrintedUser.Text   = DB.Str(dr["DepositInvPrintedUser"]);
            txtDepositInvEmailedDtTm.Text   = Fmt.DtTm(DB.DtTm(dr["DepositInvEmailedDtTm"]));
            txtDepositInvEmailedUser.Text   = DB.Str(dr["DepositInvEmailedUser"]);
            lblSaveDt.Text                  = (txtInvUpdatedDt.Text.Length > 0 ? txtInvUpdatedDt.Text : txtInvCreatedDt.Text);
			lblSaveFinalDt.Text             = Fmt.DtTm(DB.DtTm(dr["FinalDtTm"]));
			lblPrintDt.Text                 = Fmt.DtTm(DB.DtTm(dr["InvoicedDtTm"]));
			lblEmailDt.Text                 = Fmt.DtTm(DB.DtTm(dr["InvEmailedDtTm"]));
            lblDepositDt.Text               = Fmt.DtTm(Misc.Max(DB.DtTm(dr["DepositInvPrintedDtTm"]), DB.DtTm(dr["DepositInvEmailedDtTm"])));

            txtEmail.Text   = DB.Str(dr["Email"]);
			txtSubject.Text = "Invoice from Marvellous Catering";
			txtMessage.Text = "Please see the attached invoice from Marvellous Catering.\n\nWe appreciate your business.";

			PctAmt(
				InvSubTotal,
				ServiceSubTotPctFlg,
				ServiceSubTotPct,
				ref ServiceAmt,
				ref txtServiceSubTotPctFlg,
				ref txtServiceSubTotPct,
				ref txtServiceAmt);
			PctAmt(
				InvSubTotal,
				DeliverySubTotPctFlg,
				DeliverySubTotPct,
				ref DeliveryAmt,
				ref txtDeliverySubTotPctFlg,
				ref txtDeliverySubTotPct,
				ref txtDeliveryAmt);
			PctAmt(
				InvSubTotal,
				ChinaSubTotPctFlg,
				ChinaSubTotPct,
				ref ChinaAmt,
				ref txtChinaSubTotPctFlg,
				ref txtChinaSubTotPct,
				ref txtChinaAmt);
			PctAmt(
				InvSubTotal,
				AddServiceSubTotPctFlg,
				AddServiceSubTotPct,
				ref AddServiceAmt,
				ref txtAddServiceSubTotPctFlg,
				ref txtAddServiceSubTotPct,
				ref txtAddServiceAmt);
			PctAmt(
				InvSubTotal,
				FuelTravelSubTotPctFlg,
				FuelTravelSubTotPct,
				ref FuelTravelAmt,
				ref txtFuelTravelSubTotPctFlg,
				ref txtFuelTravelSubTotPct,
				ref txtFuelTravelAmt);
			PctAmt(
				InvSubTotal,
				FacilitySubTotPctFlg,
				FacilitySubTotPct,
				ref FacilityAmt,
				ref txtFacilitySubTotPctFlg,
				ref txtFacilitySubTotPct,
				ref txtFacilityAmt);
            PctAmt(
                InvSubTotal,
                GratuitySubTotPctFlg,
                GratuitySubTotPct,
                ref GratuityAmt,
                ref txtGratuitySubTotPctFlg,
                ref txtGratuitySubTotPct,
                ref txtGratuityAmt);
            PctAmt(
                InvSubTotal,
                VoluntaryGratuitySubTotPctFlg,
                VoluntaryGratuitySubTotPct,
                ref VoluntaryGratuityAmt,
                ref txtVoluntaryGratuitySubTotPctFlg,
                ref txtVoluntaryGratuitySubTotPct,
                ref txtVoluntaryGratuityAmt);
			PctAmt(
				InvSubTotal,
				RentalsSubTotPctFlg,
				RentalsSubTotPct,
				ref RentalsAmt,
				ref txtRentalsSubTotPctFlg,
				ref txtRentalsSubTotPct,
				ref txtRentalsAmt);
			PctAmt(
				InvSubTotal,
				DB.Bool(dr["Adj1SubTotPctFlg"]),
				DB.Dec(dr["Adj1SubTotPct"]),
				ref Adj1Amt,
				ref txtAdj1SubTotPctFlg,
				ref txtAdj1SubTotPct,
				ref txtAdj1Amt);
			PctAmt(
				InvSubTotal,
				DB.Bool(dr["Adj2SubTotPctFlg"]),
				DB.Dec(dr["Adj2SubTotPct"]),
				ref Adj2Amt,
				ref txtAdj2SubTotPctFlg,
				ref txtAdj2SubTotPct,
				ref txtAdj2Amt);

            //decimal PreTaxTotal =
            //	InvSubTotal +
            //	ServiceAmt +
            //	DeliveryAmt +
            //	ChinaAmt +
            //	AddServiceAmt +
            //	FuelTravelAmt +
            //	FacilityAmt +
            //	GratuityAmt +
            //	VoluntaryGratuityAmt +
            //	RentalsAmt +
            //	Adj1Amt +
            //	Adj2Amt +
            //	CCFeeAmt;

            //decimal SalesTaxTotal =
            //	InvSubTotal   * SubTotTaxPct		/ 100 +
            //	ServiceAmt    * ServiceTaxPct		/ 100 +
            //	DeliveryAmt   * DeliveryTaxPct		/ 100 +
            //	ChinaAmt      * ChinaTaxPct			/ 100 +
            //	AddServiceAmt * AddServiceTaxPct	/ 100 +
            //	FuelTravelAmt * FuelTravelTaxPct	/ 100 +
            //	FacilityAmt   * FacilityTaxPct		/ 100 +
            //	GratuityAmt   * GratuityTaxPct		/ 100 +
            //	RentalsAmt    * RentalsTaxPct		/ 100 +
            //	Adj1Amt       * Adj1TaxPct			/ 100 +
            //	Adj2Amt       * Adj2TaxPct			/ 100 +
            //	CCFeeAmt      * CCFeeTaxPct			/ 100;

            //decimal InvTotal   = PreTaxTotal + SalesTaxTotal;
            //decimal InvBalance = InvTotal + PmtAmt1 + PmtAmt2 + PmtAmt3;

            decimal PreTaxTotal     = DB.Dec(dr["PreTaxSubTotAmt"]);
			txtPreTaxTotal.Text     = Fmt.Dollar(PreTaxTotal, false);
			decimal SalesTaxTotal   = DB.Dec(dr["SalesTaxTotAmt"]);
			txtSalesTaxTotal.Text   = Fmt.Dollar(SalesTaxTotal, false);
			decimal InvTotal        = DB.Dec(dr["InvTotAmt"]);
			txtInvTotal.Text        = Fmt.Dollar(InvTotal, false);
			decimal InvBalance      = DB.Dec(dr["InvBalAmt"]);
			txtInvBalance.Text      = Fmt.Dollar(InvBalance, false);

            lblPONumber.Text        = DB.Str(dr["PONumber"]);

			if (txtInvDate.Text.Length > 0)
			{
				calInvDate.Date = Str.DtTm(txtInvDate.Text);
			}

			string txtInvoicedDt = Fmt.Dt(DB.DtTm(dr["InvoicedDtTm"]));
			fPrinted = (txtInvoicedDt.Length > 0);

			if (txtAdj1Desc.Text.Length == 0 && txtInvCreatedDt.Text.Length == 0)
			{
				txtAdj1Desc.Text =
				txtAdj2Desc.Text = "Adjustment";
			}

			if (dtInvEmailed != DateTime.MinValue)
			{
				btnEmail.Value = "Re-Email";
			}

            DefaultServings = DB.Int32(dr["NumMenServing"]) + DB.Int32(dr["NumWomenServing"]) + DB.Int32(dr["NumChildServing"]);
		}
	}

	private decimal AdditionalPrices(int InvEventCount, decimal InvPricePerPerson, decimal SubTotTaxPct)
	{
		decimal Subtotal = 0;
		StringBuilder sb = new StringBuilder();
		int iPrice = 0;

		string Sql = "Select * From JobInvoicePrices Where JobRno = " + JobRno + " Order By Seq, CreatedDtTm";
        try
        {
            DataTable dtPrices = db.DataTable(Sql);

            if (dtPrices.Rows.Count == 0)
            {
                //InsertPrice(Misc.cnPerPerson, InvEventCount, "servings", InvPricePerPerson);
                //dtPrices = db.DataTable(Sql);

                DataRow dr = dtPrices.NewRow();
                dr["PriceType"] = Misc.cnPerPerson;
                dr["InvPerPersonCount"] = InvEventCount;
                dr["InvPerPersonDesc"] = "servings";
                dr["InvPerPersonPrice"] = InvPricePerPerson;
                dr["InvPerPersonExtPrice"] = InvEventCount * InvPricePerPerson;

                dtPrices.Rows.Add(dr);
            }

            foreach (DataRow drPrice in dtPrices.Rows)
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
                        //ExtPrice = Price - (Price * (1 - 1 / (1 + SubTotTaxPct / 100)));
                        ExtPrice = DB.Dec(drPrice["InvAllInclExtPrice"]);
                        Desc = DB.Str(drPrice["InvAllInclDesc"]);
                        ExtDesc = string.Format("All inclusive {0} @ {1}",
                            Desc,
                            Fmt.Dollar(Price));
                        break;

                    default:
                        break;
                }
                int Rno = DB.Int32(drPrice["Rno"]);
                sb.AppendFormat(
                    "<tr>\n" +
                    "<td colspan=\"3\" align=\"right\" class=\"ExtDesc\">{1}</td>\n<td></td>\n" +
                    "<td><input type=\"text\" value=\"{2}\" disabled=\"disabled\" class=\"JobSubTotal DispExtPrice\" /></td>\n<td></td>\n" +
                    "<td>\n" +
                    "<img src=\"Images/Notepage16.png\" class=\"EditPrice\" alt=\"Edit Price\" title=\"Edit Price\" />\n" +
                    "<img src=\"Images/Delete16.png\" class=\"DeletePrice\" alt=\"Delete Price\" title=\"Delete Price\" />\n" +
                    "<input type=\"hidden\" name=\"Seq{0}\" value=\"{0}\" class=\"Seq\" />\n" +
                    "<input type=\"hidden\" name=\"Rno{0}\" value=\"{3}\" class=\"Rno\" />\n" +
                    "<input type=\"hidden\" name=\"PriceType{0}\" value=\"{4}\" class=\"PriceType\" />\n" +
                    "<input type=\"hidden\" name=\"Count{0}\" value=\"{5}\" class=\"Count\" />\n" +
                    "<input type=\"hidden\" name=\"Price{0}\" value=\"{6}\" class=\"Price\" />\n" +
                    "<input type=\"hidden\" name=\"ExtPrice{0}\" value=\"{7}\" class=\"ExtPrice\" />\n" +
                    "<input type=\"hidden\" name=\"Desc{0}\" value=\"{8}\" class=\"Desc\" />\n" +
                    "<input type=\"hidden\" name=\"Delete{0}\" value=\"0\" class=\"Delete\" />\n" +
                    "</td>\n" +
                    "</tr>\n",
                    ++iPrice,
                    ExtDesc,
                    Fmt.Dollar(ExtPrice),
                    (Rno > 0 ? Rno.ToString() : ""),
                    DB.Str(drPrice["PriceType"]),
                    Count,
                    Fmt.Dollar(Price),
                    ExtPrice,
                    Desc);
                Subtotal += ExtPrice;
            }

            sb.AppendFormat(
                "<input type=\"hidden\" id=\"PriceNum\" value=\"{0}\" />\n",
                iPrice);

            ltlPricing.Text = sb.ToString();
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return Subtotal;
	}

    private decimal Payments()
    {
        decimal PaymentAmt = 0;

        string Sql = string.Format("Select * From Payments Where JobRno = {0} Order By Seq", JobRno);

        try
        {
            StringBuilder sb = new StringBuilder();
            int iPmtNum = 0;

            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                int Rno                 = DB.Int32(dr["PaymentRno"]);
                int Seq                 = DB.Int32(dr["Seq"]);
                decimal Amount          = DB.Dec(dr["Amount"]);
                DateTime PaymentDt      = DB.DtTm(dr["PaymentDt"]);
                string Reference        = DB.Str(dr["Reference"]);
                string Method           = DB.Str(dr["Method"]);
                string Type             = DB.Str(dr["Type"]);
                string PurchaseOrder    = DB.Str(dr["PurchaseOrder"]);
                string CCType           = DB.Str(dr["CCType"]);
                string CCStatus         = DB.Str(dr["CCStatus"]);
                string CCResult         = DB.Str(dr["CCResult"]);
                string CCAuth           = DB.Str(dr["CCAuth"]);
                string CCReference      = DB.Str(dr["CCReference"]);
                DateTime CCPmtDtTm      = DB.DtTm(dr["CCPmtDtTm"]);
                string CCPmtUser        = DB.Str(dr["CCPmtUser"]);
                DateTime CCPrintDtTm    = DB.DtTm(dr["CCPrintDtTm"]);
                string CCPrintUser      = DB.Str(dr["CCPrintUser"]);

                sb.AppendFormat(
                    "<tr>\n" +
                        "<td colspan=\"2\"></td>\n" +
                        "<td align=\"right\"><label id=\"lblPmtType{0}\">{1}</label></td>\n" +
                        "<td></td>\n" +
                        "<td><input type=\"textbox\" id=\"txtPmtAmtDisp{0}\" value=\"{5}\" class=\"JobSubTotal aspNetDisabled\" disabled=\"disabled\"></td>\n" +
                        "<td></td>\n" +
                        "<td colspan=\"2\">\n" +
                            "<img src=\"Images/Notepage16.png\" class=\"EditPayment\" alt=\"Edit Payment\" title=\"Edit Payment\" data-pmtnum=\"{0}\" >\n" +
                            "<img src=\"Images/Delete16.png\" class=\"DeletePayment\" alt=\"Delete Payment\" title=\"Delete Payment\" data-pmtnum=\"{0}\" >\n" +
                            "<label id=\"lblPmtRef{0}\">{2}</label>\n" +
                            "<input type=\"hidden\" id=\"txtPaymentRno{0}\"  name=\"txtPaymentRno{0}\"  value=\"{3}\"  >\n" +
                            "<input type=\"hidden\" id=\"txtSeq{0}\"         name=\"txtSeq{0}\"         value=\"{4}\"  >\n" +
                            "<input type=\"hidden\" id=\"txtPmtAmt{0}\"      name=\"txtPmtAmt{0}\"      value=\"{5}\"  >\n" +
                            "<input type=\"hidden\" id=\"txtPmtType{0}\"     name=\"txtPmtType{0}\"     value=\"{1}\"  >\n" +
                            "<input type=\"hidden\" id=\"txtPurchOrder{0}\"  name=\"txtPurchOrder{0}\"  value=\"{6}\"  >\n" +
                            "<input type=\"hidden\" id=\"txtPmtRef{0}\"      name=\"txtPmtRef{0}\"      value=\"{7}\"  >\n" +
                            "<input type=\"hidden\" id=\"txtPmtDt{0}\"       name=\"txtPmtDt{0}\"       value=\"{8}\"  >\n" +
                            "<input type=\"hidden\" id=\"txtPmtMethod{0}\"   name=\"txtPmtMethod{0}\"   value=\"{9}\"  >\n" +
                            "<input type=\"hidden\" id=\"txtCCType{0}\"      name=\"txtCCType{0}\"      value=\"{10}\" >\n" +
                            "<input type=\"hidden\" id=\"txtCCStatus{0}\"    name=\"txtCCStatus{0}\"    value=\"{11}\" >\n" +
                            "<input type=\"hidden\" id=\"txtCCAuth{0}\"      name=\"txtCCAuth{0}\"      value=\"{12}\" >\n" +
                            "<input type=\"hidden\" id=\"txtCCResult{0}\"    name=\"txtCCResult{0}\"    value=\"{13}\" >\n" +
                            "<input type=\"hidden\" id=\"txtCCRef{0}\"       name=\"txtCCRef{0}\"       value=\"{14}\" >\n" +
                            "<input type=\"hidden\" id=\"txtCCPmtDtTm{0}\"   name=\"txtCCPmtDtTm{0}\"   value=\"{15}\" >\n" +
                            "<input type=\"hidden\" id=\"txtCCPmtUser{0}\"   name=\"txtCCPmtUser{0}\"   value=\"{16}\" >\n" +
                            "<input type=\"hidden\" id=\"txtCCPrintDtTm{0}\" name=\"txtCCPrintDtTm{0}\" value=\"{17}\" >\n" +
                            "<input type=\"hidden\" id=\"txtCCPrintUser{0}\" name=\"txtCCPrintUser{0}\" value=\"{18}\" >\n" +
                            "<input type=\"hidden\" id=\"hfPostCC{0}\"       name=\"hfPostCC{0}\"       value=\"{19}\" class=\"hfPostCC\" >\n" +
                            "<input type=\"hidden\" id=\"hfDeletePmt{0}\"    name=\"hfDeletePmt{0}\"    value=\"\"     class=\"DeletePmt\" >\n" +
                        "</td>\n" +
                    "</tr>\n",
                    ++iPmtNum,
                    Type,
                    (Method == "CreditCard" ? "Credit Card" : Method),
                    Rno,
                    Seq,
                    Fmt.Dollar(Amount, false),
                    PurchaseOrder,
                    Reference,
                    Fmt.Dt(PaymentDt),
                    Method,
                    CCType,
                    CCStatus,
                    CCAuth,
                    CCResult,
                    CCReference,
                    Fmt.DtTm(CCPmtDtTm),
                    CCPmtUser,
                    Fmt.DtTm(CCPrintDtTm),
                    CCPrintUser, 
                    (Page.IsPostBack ? Page.Request.Params["hfPostCC" + iPmtNum] : ""));
            }

            hfPmtNum.Value = iPmtNum.ToString();
            ltlPayments.Text = sb.ToString();
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return PaymentAmt;
    }

    private string Decrypt(string Value)
	{
		if (Value.Length > 0)
		{
			try
			{
				Value = Crypt.Decrypt(Value, Misc.Password);
			}
			catch (Exception)
			{
			}
		}

		return Value;
	}

	private void PctAmt(
		decimal SubTotal,
		bool PctFlg,
		decimal Pct,
		ref decimal Amt,
		ref HtmlInputHidden txtPctFlg,
		ref TextBox txtPct,
		ref TextBox txtAmt)
	{
		if (PctFlg)
		{
			Amt = Math.Round(SubTotal * Pct / 100, 2);
		}
		else
		{
			if (SubTotal != 0)
			{
				Pct = Amt / SubTotal * 100;
			}
		}

		txtPctFlg.Value = (PctFlg ? "true" : "false");
		txtPct.Text = Fmt.Pct(Pct, 3, false);
		txtAmt.Text = Fmt.Dollar(Amt, false);
	}

    private decimal DepositAmount()
    {
        decimal DepositAmt = 0;
        int iPmt = 0;

        try
        {
            while (Page.Request.Params["txtPaymentRno" + (++iPmt)] != null)
            {
                bool fDeposit = (Page.Request.Params["txtPmtType" + iPmt] == "Deposit");
                bool fDelete = (Page.Request.Params["hfDeletePmt" + iPmt] == "true");

                if (fDeposit && !fDelete)
                {
                    DepositAmt += Str.Dec(Page.Request.Params["txtPmtAmt" + iPmt]);
                }
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex);
            Response.Write(Err.Html());
        }

        if (DepositAmt == 0)
        {
            DepositAmt = Str.Dec(hfDepositAmt.Value);
        }

        return DepositAmt;
    }

    private void SaveValues()
	{
		string Sql = "";

		try
		{

            UpdatePriceDesc("Service",    "Service",    Str.Dec(txtServiceAmt.Text));
            UpdatePriceDesc("Delivery",   "Delivery",   Str.Dec(txtDeliveryAmt.Text));
            UpdatePriceDesc("China",      "China",      Str.Dec(txtChinaAmt.Text));
            UpdatePriceDesc("AddService", "AddService", Str.Dec(txtAddServiceAmt.Text));
            UpdatePriceDesc("FuelTravel", "FuelTravel", Str.Dec(txtFuelTravelAmt.Text));
            UpdatePriceDesc("Facility",   "Facility",   Str.Dec(txtFacilityAmt.Text));
            UpdatePriceDesc("Rentals",    "Rentals",    Str.Dec(txtRentalsAmt.Text));
            UpdatePriceDesc("Adj1",       "Adj1",       Str.Dec(txtAdj1Amt.Text));
            UpdatePriceDesc("Adj2",       "Adj2",       Str.Dec(txtAdj2Amt.Text));
            UpdatePriceDesc("InvTot",     "EstTot",     Str.Dec(hfInvTotal.Value));
            UpdatePriceDesc("Deposit",    "Deposit",    Str.Dec(txtDepositAmt.Text));

            Sql = "Select InvCreatedDtTm From mcJobs Where JobRno = " + JobRno;
			fNew = (Fmt.DtTm(DB.DtTm(db.SqlDtTm(Sql))) == "");

			string sCreatedUpdated = (fNew ? "Created" : "Updated");
			string sInvDtTm = "Inv" + sCreatedUpdated + "DtTm";
			string sInvUser = "Inv" + sCreatedUpdated + "User";

			DateTime Tm = DateTime.Now;

			Sql =
				"Update mcJobs Set "                +
				//"TaxExemptFlg = "           	    + DB.PutBool(chkSalesTaxExempt.Checked) + ", " +
				"InvDate = "                        + DB.PutDtTm(Str.DtTm(txtInvDate.Text)) + ", " +
				//"InvEventCount = "                + Str.Num(txtInvEventCount.Text) + ", " +
				//"InvPricePerPerson = "            + Str.Dec(txtInvPricePerPerson.Text) + ", " +
				"InvEventCount = 0, "               +
				"InvPricePerPerson = 0, "           + 
				"SubTotAmt = "                      + Str.Dec(hfInvSubTotal.Value) + ", " +
				"SubTotTaxPct = "                   + Str.Dec(txtSubTotTaxPct.Text) + ", " +
				"ServiceSubTotPctFlg = "            + DB.PutBool(txtServiceSubTotPctFlg.Value == "true") + ", " +
				"ServiceSubTotPct = "               + Str.Dec(txtServiceSubTotPct.Text) + ", " +
				"ServiceAmt = "                     + Str.Dec(txtServiceAmt.Text) + ", " +
				"ServiceTaxPct = "                  + Str.Dec(txtServiceTaxPct.Text) + ", " +
				"DeliverySubTotPctFlg = "           + DB.PutBool(txtDeliverySubTotPctFlg.Value == "true") + ", " +
				"DeliverySubTotPct = "              + Str.Dec(txtDeliverySubTotPct.Text) + ", " +
				"DeliveryAmt = "                    + Str.Dec(txtDeliveryAmt.Text) + ", " +
				"DeliveryTaxPct = "                 + Str.Dec(txtDeliveryTaxPct.Text) + ", " +
				"ChinaSubTotPctFlg = "              + DB.PutBool(txtChinaSubTotPctFlg.Value == "true") + ", " +
				"ChinaSubTotPct = "                 + Str.Dec(txtChinaSubTotPct.Text) + ", " +
				"ChinaAmt = "                       + Str.Dec(txtChinaAmt.Text) + ", " +
				"ChinaTaxPct = "                    + Str.Dec(txtChinaTaxPct.Text) + ", " +
				"AddServiceSubTotPctFlg = "         + DB.PutBool(txtAddServiceSubTotPctFlg.Value == "true") + ", " +
				"AddServiceSubTotPct = "            + Str.Dec(txtAddServiceSubTotPct.Text) + ", " +
				"AddServiceAmt = "                  + Str.Dec(txtAddServiceAmt.Text) + ", " +
				"AddServiceTaxPct = "               + Str.Dec(txtAddServiceTaxPct.Text) + ", " +
				"FuelTravelSubTotPctFlg = "         + DB.PutBool(txtFuelTravelSubTotPctFlg.Value == "true") + ", " +
				"FuelTravelSubTotPct = "            + Str.Dec(txtFuelTravelSubTotPct.Text) + ", " +
				"FuelTravelAmt = "                  + Str.Dec(txtFuelTravelAmt.Text) + ", " +
				"FuelTravelTaxPct = "               + Str.Dec(txtFuelTravelTaxPct.Text) + ", " +
				"FacilitySubTotPctFlg = "           + DB.PutBool(txtFacilitySubTotPctFlg.Value == "true") + ", " +
				"FacilitySubTotPct = "              + Str.Dec(txtFacilitySubTotPct.Text) + ", " +
				"FacilityAmt = "                    + Str.Dec(txtFacilityAmt.Text) + ", " +
				"FacilityTaxPct = "                 + Str.Dec(txtFacilityTaxPct.Text) + ", " +
				"GratuitySubTotPctFlg = "           + DB.PutBool(txtGratuitySubTotPctFlg.Value == "true") + ", " +
				"GratuitySubTotPct = "              + Str.Dec(txtGratuitySubTotPct.Text) + ", " +
				"GratuityAmt = "                    + Str.Dec(txtGratuityAmt.Text) + ", " +
				"GratuityTaxPct = "                 + Str.Dec(txtGratuityTaxPct.Text) + ", " +
                "VoluntaryGratuitySubTotPctFlg = "  + DB.PutBool(txtVoluntaryGratuitySubTotPctFlg.Value == "true") + ", " +
		        "VoluntaryGratuitySubTotPct = "     + Str.Dec(txtVoluntaryGratuitySubTotPct.Text) + ", " +
				"VoluntaryGratuityAmt = "           + Str.Dec(txtVoluntaryGratuityAmt.Text) + ", " +
				"RentalsSubTotPctFlg = "            + DB.PutBool(txtRentalsSubTotPctFlg.Value == "true") + ", " +
				"RentalsSubTotPct = "               + Str.Dec(txtRentalsSubTotPct.Text) + ", " +
				"RentalsAmt = "                     + Str.Dec(txtRentalsAmt.Text) + ", " +
				"RentalsTaxPct = "                  + Str.Dec(txtRentalsTaxPct.Text) + ", " +
				"RentalsDesc = "                    + DB.PutStr(txtRentalsDesc.Text) + ", " +
				"Adj1SubTotPctFlg = "               + DB.PutBool(txtAdj1SubTotPctFlg.Value == "true") + ", " +
				"Adj1SubTotPct = "                  + Str.Dec(txtAdj1SubTotPct.Text) + ", " +
				"Adj1Desc = "                       + DB.PutStr(txtAdj1Desc.Text, 50) + ", " +
				"Adj1Amt = "                        + Str.Dec(txtAdj1Amt.Text) + ", " +
				"Adj1TaxPct = "                     + Str.Dec(txtAdj1TaxPct.Text) + ", " +
				"Adj2SubTotPctFlg = "               + DB.PutBool(txtAdj2SubTotPctFlg.Value == "true") + ", " +
				"Adj2SubTotPct = "                  + Str.Dec(txtAdj2SubTotPct.Text) + ", " +
				"Adj2Desc = "                       + DB.PutStr(txtAdj2Desc.Text, 50) + ", " +
				"Adj2Amt = "                        + Str.Dec(txtAdj2Amt.Text) + ", " +
				"Adj2TaxPct = "                     + Str.Dec(txtAdj2TaxPct.Text) + ", " +
		        "DepositAmt = "                     + DepositAmount() + ", " +
				"CCPmtFeeFlg = "                    + DB.PutBool(chkCCPmtFee.Checked) + ", " + 
				"CCFeePct = "                       + Str.Dec(txtCCFeePct.Text) + ", " +
				"CCFeeAmt = "                       + Str.Dec(hfCCFeeAmt.Value) + ", " +
				"CCFeeTaxPct = "                    + Str.Dec(txtCCFeeTaxPct.Text) + ", " +
				"PreTaxSubTotAmt = "                + Str.Dec(hfPreTaxTotal.Value) + ", " +
				"SalesTaxTotAmt = "                 + Str.Dec(hfSalesTaxTotal.Value) + ", " +
				"InvTotAmt = "                      + Str.Dec(hfInvTotal.Value) + ", " +
				"InvBalAmt = "                      + Str.Dec(hfInvBalance.Value) + ", " +
				sInvDtTm + " = "                    + DB.PutDtTm(Tm) + ", " +
				sInvUser + " = "                    + DB.PutStr(g.User) + " " +
				"Where JobRno = "                   + JobRno;
			db.Exec(Sql);

            SaveCustomer();
			SaveAdditionalPrices();
	        SavePayments();

			SaveFinal(Tm);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

    private void UpdatePriceDesc(string Field, string Prop, decimal NewAmount)
    {
        string Sql = string.Format(
            "Select {0}Amt, {1}PropDesc From mcJobs Where JobRno = {2}",
            Field,
            Prop,
            JobRno);
        try
        {
            DataRow dr = db.DataRow(Sql);
            if (dr != null)
            {
                string PrevAmtFmt = Fmt.Dollar(DB.Dec(dr[0]));
                string PrevDesc = DB.Str(dr[1]);
                string NewAmtFmt = Fmt.Dollar(NewAmount);
                string NewDesc = PrevDesc.Replace(PrevAmtFmt, NewAmtFmt);


                if (NewDesc != PrevDesc)
                {
                    Sql = string.Format(
                        "Update mcJobs Set {0}PropDesc = {1} Where JobRno = {2}",
                        Prop,
                        DB.Put(NewDesc, 200),
                        JobRno);
                    db.Exec(Sql);
                }
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    private void SaveCustomer()
    {
        string Sql = string.Format(
            "Select cu.TaxExemptFlg, cu.CustomerRno " +
            "From Customers cu " +
            "Inner Join Contacts c on cu.CustomerRno = c.CustomerRno " +
            "Inner Join mcJobs j on c.ContactRno = j.ContactRno " +
            "Where JobRno = {0}", 
            JobRno);
        try
        {
            DataRow dr = db.DataRow(Sql);
            if (dr != null)
            {
                bool fTaxExempt = DB.Bool(dr["TaxExemptFlg"]);
                int CustomerRno = DB.Int32(dr["CustomerRno"]);

                if (chkSalesTaxExempt.Checked != fTaxExempt)
                {
                    Sql = string.Format("Update Customers Set TaxExemptFlg = {1} Where CustomerRno = {0}", CustomerRno, DB.Put(chkSalesTaxExempt.Checked));
                    db.Exec(Sql);
                }
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    private void SaveAdditionalPrices()
	{
		int iPrice = 0;
		string Sql = "";
		DateTime Tm = DateTime.Now;

		try
		{
			while (Page.Request.Params["Rno" + (++iPrice)] != null)
			{
				string Rno = Page.Request.Params["Rno" + iPrice];

				if (Rno.Length == 0)
				{
					// insert
					if (Page.Request.Params["Delete" + iPrice] == "0")
					{
						InsertPrice(
							Page.Request.Params["PriceType" + iPrice],
							Str.Num(Page.Request.Params["Count" + iPrice]),
							Page.Request.Params["Desc" + iPrice],
							Str.Dec(Page.Request.Params["Price" + iPrice]),
							Str.Dec(Page.Request.Params["ExtPrice" + iPrice]));
						//int nRno = db.NextRno("JobInvoicePrices", "Rno");
						//Sql = "Insert Into JobInvoicePrices (Rno, JobRno, Seq, PriceType, " +
						//    "InvPerPersonCount, InvPerPersonDesc, InvPerPersonPrice, " +
						//    "InvPerItemCount, InvPerItemDesc, InvPerItemPrice, " +
						//    "InvAllInclDesc, InvAllInclPrice, " +
						//    "CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) Values (" +
						//    nRno + ", " +
						//    JobRno + ", " +
						//    db.NextSeq("JobInvoicePrices", "JobRno", JobRno, "Seq") + ", " +
						//    DB.PutStr(Page.Request.Params["PriceType" + iPrice], 20) + ", " +
						//    Str.Num(Page.Request.Params["Count" + iPrice]) + ", " +
						//    DB.PutStr(Page.Request.Params["Desc" + iPrice], 50) + ", " +
						//    Str.Dec(Page.Request.Params["Price" + iPrice]) + ", " +
						//    Str.Num(Page.Request.Params["Count" + iPrice]) + ", " +
						//    DB.PutStr(Page.Request.Params["Desc" + iPrice], 50) + ", " +
						//    Str.Dec(Page.Request.Params["Price" + iPrice]) + ", " +
						//    DB.PutStr(Page.Request.Params["Desc" + iPrice], 50) + ", " +
						//    Str.Dec(Page.Request.Params["Price" + iPrice]) + ", " +
						//    DB.PutDtTm(Tm) + ", " +
						//    DB.PutStr(g.User) + ", " +
						//    DB.PutDtTm(Tm) + ", " +
						//    DB.PutStr(g.User) + ")";
						//db.Exec(Sql);
					}
					else
					{
						// delete - do nothing
					}
				}
				else
				{
					// update
					if (Page.Request.Params["Delete" + iPrice] == "0")
					{
                        Sql = string.Format(
                            "Select InvPerPersonPrice, PropPerPersonDesc, InvPerItemPrice, PropPerItemDesc, InvAllInclPrice, PropAllInclDesc From JobInvoicePrices Where Rno = {0}", Rno);

                        string PropPerPersonDesc = null;
                        string PropPerItemDesc = null;
                        string PropAllInclDesc = null;

                        string NewAmtFmt = Fmt.Dollar(Str.Dec(Page.Request.Params["Price" + iPrice]));

                        DataRow dr = db.DataRow(Sql);
                        if (dr != null)
                        {
                            string PrevPerPersonAmtFmt = Fmt.Dollar(DB.Dec(dr["InvPerPersonPrice"]));
                            if (dr["PropPerPersonDesc"] != DBNull.Value)
                            {
                                PropPerPersonDesc = DB.Str(dr["PropPerPersonDesc"]).Replace(PrevPerPersonAmtFmt, NewAmtFmt);
                            }
                            string PrevPerItemAmtFmt = Fmt.Dollar(DB.Dec(dr["InvPerItemPrice"]));
                            if (dr["PropPerItemDesc"] != DBNull.Value)
                            {
                                PropPerItemDesc = DB.Str(dr["PropPerItemDesc"]).Replace(PrevPerItemAmtFmt, Fmt.Dollar(Str.Num(Page.Request.Params["Count" + iPrice]) * Str.Dec(Page.Request.Params["Price" + iPrice])));
                            }
                            string PrevAllInclAmtFmt = Fmt.Dollar(DB.Dec(dr["InvAllInclPrice"]));
                            if (dr["PropAllInclDesc"] != DBNull.Value)
                            {
                                PropAllInclDesc = DB.Str(dr["PropAllInclDesc"]).Replace(PrevAllInclAmtFmt, NewAmtFmt);
                            }
                        }

                        Sql = "Update JobInvoicePrices Set " +
							"PriceType = "              + DB.PutStr(Page.Request.Params["PriceType" + iPrice], 20) + ", " +
							"InvPerPersonCount = "      + Str.Num(Page.Request.Params["Count" + iPrice]) + ", " +
							"InvPerPersonDesc = "       + DB.PutStr(Page.Request.Params["Desc" + iPrice], 50) + ", " +
							"InvPerPersonPrice = "      + Str.Dec(Page.Request.Params["Price" + iPrice]) + ", " +
							"InvPerPersonExtPrice = "   + Str.Dec(Page.Request.Params["ExtPrice" + iPrice]) + ", " +
							"InvPerItemCount = "        + Str.Num(Page.Request.Params["Count" + iPrice]) + ", " +
							"InvPerItemDesc = "         + DB.PutStr(Page.Request.Params["Desc" + iPrice], 50) + ", " +
							"InvPerItemPrice = "        + Str.Dec(Page.Request.Params["Price" + iPrice]) + ", " +
							"InvPerItemExtPrice = "     + Str.Dec(Page.Request.Params["ExtPrice" + iPrice]) + ", " +
							"InvAllInclDesc = "         + DB.PutStr(Page.Request.Params["Desc" + iPrice], 50) + ", " +
							"InvAllInclPrice = "        + Str.Dec(Page.Request.Params["Price" + iPrice]) + ", " +
							"InvAllInclExtPrice = "     + Str.Dec(Page.Request.Params["ExtPrice" + iPrice]) + ", " +
                            "PropPerPersonDesc = "      + DB.PutStr(PropPerPersonDesc, 1000) + ", " +
                            "PropPerItemDesc = "        + DB.PutStr(PropPerItemDesc, 1000) + ", " +
                            "PropAllInclDesc = "        + DB.PutStr(PropAllInclDesc, 1000) + ", " +
							"UpdatedDtTm = "            + DB.PutDtTm(Tm) + ", " +
							"UpdatedUser = "            + DB.PutStr(g.User) + " " +
							"Where Rno = "              + Rno;
						db.Exec(Sql);
					}
					else
					{
						// delete
						Sql = string.Format("Delete From JobInvoicePrices Where Rno = {0}", Rno);
						db.Exec(Sql);
					}
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	private void InsertPrice(string PriceType, int Count, string Desc, decimal Price, decimal ExtPrice)
	{
		//string Sql = "";
		//DateTime Tm = DateTime.Now;

		//try
		//{
		//	int nRno = db.NextRno("JobInvoicePrices", "Rno");
		//	Sql = 
		//		"Insert Into JobInvoicePrices (Rno, JobRno, Seq, PriceType, " +
		//		"InvPerPersonCount, InvPerPersonDesc, InvPerPersonPrice, " +
		//		"InvPerItemCount, InvPerItemDesc, InvPerItemPrice, " +
		//		"InvAllInclDesc, InvAllInclPrice, " +
		//		"CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) Values (" +
		//		nRno + ", " +
		//		JobRno + ", " +
		//		db.NextSeq("JobInvoicePrices", "JobRno", JobRno, "Seq") + ", " +
		//		DB.PutStr(PriceType, 20) + ", " +
		//		Count + ", " +
		//		DB.PutStr(Desc, 50) + ", " +
		//		Price + ", " +
		//		Count + ", " +
		//		DB.PutStr(Desc, 50) + ", " +
		//		Price + ", " +
		//		DB.PutStr(Desc, 50) + ", " +
		//		Price + ", " +
		//		DB.PutDtTm(Tm) + ", " +
		//		DB.PutStr(g.User) + ", " +
		//		DB.PutDtTm(Tm) + ", " +
		//		DB.PutStr(g.User) + ")";
		//	db.Exec(Sql);
		//}
		//catch (Exception Ex)
		//{
		//	Err Err = new Err(Ex, Sql);
		//	Response.Write(Err.Html());
		//}

		Misc.InsertPrice(db, JobRno, PriceType, Count, Desc, Price, ExtPrice);
	}

    private void SavePayments()
    {
        int iPmt = 0;
        string Sql = "";
        DateTime Tm = DateTime.Now;

        try
        {
            while (Page.Request.Params["txtPaymentRno" + (++iPmt)] != null)
            {
                string Rno = Page.Request.Params["txtPaymentRno" + iPmt];
                bool fDelete = (Page.Request.Params["hfDeletePmt" + iPmt] == "true");

                if (Rno.Length == 0 && !fDelete)
                {
                    // insert a new Payment record
                    Sql = string.Format(
                        "Insert Into Payments (JobRno, Seq, CreatedDtTm, CreatedUser) Values " +
                        "({0}, (Select IsNull(Max(Seq), 0) + 1 From Payments Where JobRno = {0}), {1}, {2});" +
                        "Select Scope_Identity();",
                        JobRno,
                        DB.PutDtTm(Tm),
                        DB.PutStr(g.User, 20));
                    Rno = db.SqlNum(Sql).ToString();
                }

                if (fDelete)
                {
                    if (Rno.Length != 0)
                    {
                        Sql = string.Format("Delete Payments Where PaymentRno = {0}", Rno);
                        db.Exec(Sql);
                    }
                }
                else
                {
                    Sql = string.Format(
                        "Update Payments Set " +
                        "Amount = {1}, " +
                        "PaymentDt = {2}, " +
                        "Reference = {3}, " +
                        "Method = {4}, " +
                        "Type = {5}, " +
                        "PurchaseOrder = {6}, " +
                        "CCType = {7}, " +
                        "UpdatedDtTm = {8}, " +
                        "UpdatedUser = {9} " +
                        "Where PaymentRno = {0}",
                        Rno,
                        Str.Dec(Page.Request.Params["txtPmtAmt" + iPmt]),
                        DB.PutDtTm(Str.DtTm(Page.Request.Params["txtPmtDt" + iPmt])),
                        DB.PutStr(Page.Request.Params["txtPmtRef" + iPmt], 130),
                        DB.PutStr(Page.Request.Params["txtPmtMethod" + iPmt], 12),
                        DB.PutStr(Page.Request.Params["txtPmtType" + iPmt], 12),
                        DB.PutStr(Page.Request.Params["txtPurchOrder" + iPmt], 50),
                        DB.PutStr(Page.Request.Params["txtCCType" + iPmt], 20),
                        DB.PutDtTm(Tm),
                        DB.PutStr(DB.PutStr(g.User), 20));
                    db.Exec(Sql);
                }
            }
        }

        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }


    private void SaveFinal(DateTime Tm)
	{
		if (fSaveFinal)
		{
			String Sql = "Select * From mcJobs Where JobRno = " + JobRno;
			try
			{
				DataTable tbl = db.DataTable(Sql);
				if (tbl.Rows.Count > 0)
				{
					DataRow dr = tbl.Rows[0];
					bool fSavedFinal = (dr["FinalDtTm"] != DBNull.Value);
					bool fProcessed = (dr["ProcessedDtTm"] != DBNull.Value);
					Sql = null;

					if (fSavedFinal)
					{
						if (fProcessed)
						{
							Sql =
								"EditProcessedDtTm = " + DB.PutDtTm(Tm) + ", " +
								"EditProcessedUser = " + DB.PutStr(g.User) + " ";
						}
					}
					else
					{
						Sql =
							"FinalDtTm = " + DB.PutDtTm(Tm) + ", " +
							"FinalUser = " + DB.PutStr(g.User) + " ";

						if (fProcessed)
						{
							Sql +=
								", ProcessedDtTm = Null, " +
								"ProcessedUser = Null ";
						}
					}

					if (Sql != null)
					{ 
						Sql =
							"Update mcJobs Set " + Sql + "Where JobRno = " + JobRno;
						db.Exec(Sql);
					}
				}
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
	}

	private void SaveDefaults()
	{
		String Sql = "Select * From Settings Where SettingRno = 1";
		try
		{
			DataRow dr = db.DataRow(Sql);
			if (dr == null)
			{
				Sql = string.Format("Insert Into Settings (SettingRno, CreatedDtTm, CreatedUser) Values (1, '{0}', '{1}')", DateTime.Now, g.User);
				db.Exec(Sql);
			}

			Sql =
				"Update Settings Set "      +
				"SubTotTaxPct = "           + Str.Dec(txtSubTotTaxPct.Text) + ", " +
				"ServiceSubTotPct = "       + Str.Dec(txtServiceSubTotPct.Text) + ", " +
				"ServiceTaxPct = "          + Str.Dec(txtServiceTaxPct.Text) + ", " +
				"DeliverySubTotPct = "      + Str.Dec(txtDeliverySubTotPct.Text) + ", " +
				"DeliveryTaxPct = "         + Str.Dec(txtDeliveryTaxPct.Text) + ", " +
				"ChinaSubTotPct = "         + Str.Dec(txtChinaSubTotPct.Text) + ", " +
				"ChinaTaxPct = "            + Str.Dec(txtChinaTaxPct.Text) + ", " +
				"AddServiceSubTotPct = "    + Str.Dec(txtAddServiceSubTotPct.Text) + ", " +
				"AddServiceTaxPct = "       + Str.Dec(txtAddServiceTaxPct.Text) + ", " +
				"FuelTravelSubTotPct = "    + Str.Dec(txtFuelTravelSubTotPct.Text) + ", " +
				"FuelTravelTaxPct = "       + Str.Dec(txtFuelTravelTaxPct.Text) + ", " +
				"FacilitySubTotPct = "      + Str.Dec(txtFacilitySubTotPct.Text) + ", " +
				"FacilityTaxPct = "         + Str.Dec(txtFacilityTaxPct.Text) + ", " +
				"GratuitySubTotPct = "      + Str.Dec(txtGratuitySubTotPct.Text) + ", " +
				"GratuityTaxPct = "         + Str.Dec(txtGratuityTaxPct.Text) + ", " +
				"RentalsSubTotPct = "       + Str.Dec(txtRentalsSubTotPct.Text) + ", " +
				"RentalsTaxPct = "          + Str.Dec(txtRentalsTaxPct.Text) + ", " +
				"Adj1TaxPct = "             + Str.Dec(txtAdj1TaxPct.Text) + ", " +
				"Adj2TaxPct = "             + Str.Dec(txtAdj2TaxPct.Text) + ", " +
				"CCFeePct = "               + Str.Dec(txtCCFeePct.Text) + ", " +
				"CCFeeTaxPct = "            + Str.Dec(txtCCFeeTaxPct.Text) + ", " +
				"UpdatedDtTm = "            + DB.PutDtTm(DateTime.Now) + ", " +
				"UpdatedUser = "            + DB.PutStr(g.User) + " " +
				"Where SettingRno = 1";
			db.Exec(Sql);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void btnSaveDefaults_Click(object sender, System.EventArgs e)
	{
		SaveDefaults();
	}

	protected void btnSave_Click(object sender, System.EventArgs e)
	{
		SaveValues();
		ReadValues();

	}

	protected void btnSaveFinal_Click(object sender, System.EventArgs e)
	{
		fSaveFinal = true;
		SaveValues();
		ReadValues();
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

	protected void SendEmail()
	{
		int nTrys = 0;
        bool fDepositInvoice = Str.Bool(hfDepositInvoice.Value);
        decimal DepositAmt = Str.Dec(hfDepositAmt.Value);

	TryAgain:

		nTrys++;

		try
		{
			WebConfig wc = new WebConfig();
            MailMessage Msg = new MailMessage(Misc.CreateFromEmailAddress(g.User), txtEmail.Text);
			Msg.CC.Add(wc.Str("Email CC"));
            Msg.CC.Add(Misc.CreateFromEmailAddress(g.User));
            Msg.Subject = Misc.EnvSubject() + txtSubject.Text;
			Msg.Body = txtMessage.Text;

			MemoryStream ms = Utils.PdfFiles.Invoice(Page, JobRno, fDepositInvoice, DepositAmt);

			bool fTest = false;
			if (fTest)
			{
				FileStream fs = new FileStream(Server.MapPath(@"~/Invoice.pdf"), FileMode.Create, FileAccess.Write);
				ms.WriteTo(fs);
				fs.Close();
			}
			else
			{
				Msg.Attachments.Add(new Attachment(ms, "Invoice.pdf", "application/pdf"));
				SmtpClient Smtp = new SmtpClient();
				Smtp.Send(Msg);
			}

			ms.Close();
		}
		catch (Exception Ex)
		{
			if (nTrys <= 3) goto TryAgain;
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}

		string Sql = "Select InvEmailedDtTm From mcJobs Where JobRno = " + JobRno; 
		try
		{
			DateTime dtInvEmailed = db.SqlDtTm(Sql);
			if (dtInvEmailed == DateTime.MinValue)
			{
				Sql = string.Format("Update mcJobs Set InvEmailedDtTm = GetDate(), InvEmailedUser = '{1}' Where JobRno = {0}", JobRno, g.User);
				db.Exec(Sql);
			}

            if (fDepositInvoice)
            {
                Sql = string.Format("Update mcJobs Set DepositInvEmailedDtTm = GetDate(), DepositInvEmailedUser = '{1}' Where JobRno = {0}", JobRno, g.User);
                db.Exec(Sql);
            }
        }
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

    protected void SaveCreditCardPayment()
    {
        string Sql = string.Empty;

        try
        {
            string InvoiceNum = Request.Params["UMinvoice"];
            string[] Parts = InvoiceNum.Split(new char[] { '-' });
            Int32 JobRno = Str.Num(Parts[0]);
            int PmtNum = Str.Num(Parts[1]);
            string Status = Request.Params["UMstatus"];
            string AuthCode = Request.Params["UMauthCode"];
            string RefNum = Request.Params["UMrefNum"];
            string Error = Request.Params["UMerror"];
            string ErrorCode = Request.Params["UMerrorcode"];

            DateTime Tm = DateTime.Now;

            Sql = string.Format(
                "Update Payments Set " +
                "PaymentDt = {2}, " +
                "CCStatus = {3}, " +
                "CCResult = {4}, " +
                "CCAuth = {5}, " +
                "CCReference = {6}, " +
                "CCPmtDtTm = {7}, " +
                "CCPmtUser = {8}, " +
                "UpdatedDtTm = {7}, " +
                "UpdatedUser = {8} " +
                "Where JobRno = {0} And Seq = {1}",
                JobRno,
                PmtNum,
                DB.PutDtTm(Tm),
                DB.PutStr(Status, 30),
                DB.PutStr(Error + " - " + ErrorCode, 150),
                DB.PutStr(AuthCode, 10),
                DB.PutStr(RefNum, 130),
                DB.PutDtTm(Tm),
                DB.PutStr(g.User, 20));
            db.Exec(Sql);

            ShowPayment = PmtNum;
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }    
    }

    protected int ShowPayment = 0;
}