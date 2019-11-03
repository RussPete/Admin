using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using Utils;
using Globals;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class ProposalPricing : System.Web.UI.Page
{
	public const string PerPerson	= "PerPerson";
	public const string Service		= "Service";
	public const string Delivery	= "Delivery";
	public const string China		= "China";
	public const string AddService	= "AddService";
	public const string FuelTravel	= "FuelTravel";
	public const string Facility	= "Facility";
	public const string Rentals		= "Rentals";

	public const string DefaultService		= "On-site Service";
	public const string DefaultDelivery		= "Delivery";
	public const string DefaultChina		= "China Rental";
	public const string DefaultAddService	= "Additional On-site Service";
	public const string DefaultFuelTravel	= "Fuel & Travel";
	public const string DefaultFacility		= "Venue";
	public const string DefaultRentals		= "Rentals";
	public const string DefaultDeposit		= "Deposit Paid";


	public class OptionType
	{
		public const string Hidden = "Hidden";
		public const string Checkbox = "Checkbox";
		public const string Number = "Number";
	}

	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";
	protected Int32 JobRno;
	protected string EmailType;
	protected string EmailAttachment;
	protected string ErrMsg = string.Empty;

    private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        JobRno = Pg.JobRno();

        pnlPreview.CssClass = "PdfPreview";

        //pnlPreview.Visible = false;

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.Jobs.Title;
        }
        else
		{
			if (hfEmail.Value == "true")
			{
				SaveAttachment();
				SendEmail();
			}
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
        hfJobRno.Value = JobRno.ToString();

        Misc.DefaultInvoicePrice(db, JobRno);
		ReadValues();
	}

	protected void btnSave_Click(object sender, System.EventArgs e)
	{
        SaveValues();
	}

	protected void btnSavePreview_Click(object sender, System.EventArgs e)
	{
        SaveValues();
		Misc.DefaultProposalMenu(JobRno);
        //pnlPreview.Visible = true;
        pnlPreview.CssClass = "PdfPreview ShowPreview";
        frmPreview.Attributes.Add("src", Page.ResolveUrl(string.Format("{0}?Rno={1}", @"~\ProposalPreview.aspx", JobRno)));
	}

	protected void ReadValues()
	{
		ClearValues();

		String Sql =
            "Select j.SubTotTaxPct, j.Adj1Desc, j.Adj2Desc, j.ServiceAmt, j.DeliveryAmt, j.ChinaAmt, j.AddServiceAmt, j.FuelTravelAmt, " +
            "j.FacilityAmt, j.RentalsAmt, j.Adj1Amt, j.Adj2Amt, j.DepositAmt, j.ServiceTaxPct, j.DeliveryTaxPct, j.ChinaTaxPct, " +
            "j.AddServiceTaxPct, j.FuelTravelTaxPct, j.FacilityTaxPct, j.RentalsTaxPct, j.Adj1TaxPct, j.Adj2TaxPct, j.ServicePropDesc, " +
            "j.DeliveryPropDesc, j.ChinaPropDesc, j.AddServicePropDesc, j.FuelTravelPropDesc, j.FacilityPropDesc, j.RentalsPropDesc, " +
            "j.Adj1PropDesc, j.Adj2PropDesc, j.EstTotPropDesc, j.DepositPropDesc, j.SalesTaxTotAmt, j.InvTotAmt, j.InvBalAmt, j.ServicePropDescCustFlg, " +
            "j.DeliveryPropDescCustFlg, j.ChinaPropDescCustFlg, j.AddServicePropDescCustFlg, j.FuelTravelPropDescCustFlg, " +
            "j.FacilityPropDescCustFlg, j.RentalsPropDescCustFlg, j.EstTotPropDescCustFlg, j.DepositPropDescCustFlg, j.ServicePropOptions, " +
            "j.DeliveryPropOptions, j.ChinaPropOptions, j.AddServicePropOptions, j.FuelTravelPropOptions, j.FacilityPropOptions, " +
            "j.RentalsPropOptions, j.EstTotPropOptions, j.DepositPropOptions, j.NumMenServing, j.NumWomenServing, j.NumChildServing, " +
            "j.ProposalFlg, j.PropCreatedDtTm, j.PropEmailedDtTm, j.ConfirmEmailedDtTm, j.JobType, c.Email, j.PropCreatedUser, " +
            "j.PropUpdatedDtTm, j.PropUpdatedUser, j.PropGeneratedDtTm, j.PropGeneratedUser, j.PropEmailedUser, j.ConfirmEmailedUser " +
            "From mcJobs j " +
            "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
            "Where JobRno = " + JobRno;
        try
        {
		    DataRow dr = db.DataRow(Sql);
            if (dr != null)
            {
				//decimal SalesTax = 0;
				//decimal Total = 0;
				string Default = string.Empty;
				string Zero = Fmt.Dollar(0);

				decimal SubTotTaxPct = DB.Dec(dr["SubTotTaxPct"]);

				// prices
				Sql = string.Format("Select * From JobInvoicePrices Where JobRno = {0} Order By Seq", JobRno);
				DataTable dtPrices = db.DataTable(Sql);
				foreach (DataRow drPrice in dtPrices.Rows)
				{
					int Rno = DB.Int32(drPrice["Rno"]);
					string PriceType = DB.Str(drPrice["PriceType"]);
					//string PriceDesc = string.Empty;
					int Count = 0;
					decimal Price = 0;
					decimal ExtPrice = 0;
					string FmtPrice = string.Empty;
					//string Desc = string.Empty;
					string PropDesc = string.Empty;
					bool fCustomized = false;
					Default = string.Empty;
					string Options = string.Empty;

					switch (PriceType)
					{
						case Misc.cnPerPerson:
							Count = DB.Int32(drPrice["InvPerPersonCount"]);
							Price = DB.Dec(drPrice["InvPerPersonPrice"]);
							//ExtPrice = Count * Price;
							ExtPrice = DB.Dec(drPrice["InvPerPersonExtPrice"]);
							FmtPrice = Fmt.Dollar(Price);
							//Desc = DB.Str(drPrice["InvPerPersonDesc"]);
							//PriceDesc = string.Format("Price per person: {0}", FmtPrice);
							PropDesc = DB.Str(drPrice["PropPerPersonDesc"]);
							fCustomized = DB.Bool(drPrice["PropPerPersonDescCustFlg"]);
							Options = DB.Str(drPrice["PropPerPersonOptions"]);
							Default = string.Format("Price per person: {0}", FmtPrice);
							if (!fCustomized && PropDesc.Length > 0 && FmtPrice != Zero)
							{
								PropDesc = Default;
							}
							break;

						case Misc.cnPerItem:
							Count = DB.Int32(drPrice["InvPerItemCount"]);
							Price = DB.Dec(drPrice["InvPerItemPrice"]);
							//ExtPrice = Count * Price;
							ExtPrice = DB.Dec(drPrice["InvPerItemExtPrice"]);
							FmtPrice = Fmt.Dollar(Count * Price);
							//Desc = DB.Str(drPrice["InvPerItemDesc"]);
							//PriceDesc = string.Format("{0} {1} @ {2} per item",
							//	Fmt.Num(Count),
							//	Desc,
							//	Fmt.Dollar(Price));
							PropDesc = DB.Str(drPrice["PropPerItemDesc"]);
							fCustomized = DB.Bool(drPrice["PropPerItemDescCustFlg"]);
							PriceType = string.Format("{0} {1}", Count, DB.Str(drPrice["InvPerItemDesc"]));
							Default = string.Format("{0}: {1}", PriceType,  FmtPrice);
							if (!fCustomized && PropDesc.Length > 0 && FmtPrice != Zero)
							{
								PropDesc = Default;
							}
							break;

						case Misc.cnAllIncl:
							Count = 1;
							Price = DB.Dec(drPrice["InvAllInclPrice"]);
							//ExtPrice = Price - (Price * (1 - 1 / (1 + SubTotTaxPct / 100)));
							ExtPrice = DB.Dec(drPrice["InvAllInclExtPrice"]);
							FmtPrice = Fmt.Dollar(Price);
							//Desc = DB.Str(drPrice["InvAllInclDesc"]);
							//PriceDesc = string.Format("All inclusive {0} @ {1}",
							//	Desc,
							//	Fmt.Dollar(Price));
							PropDesc = DB.Str(drPrice["PropAllInclDesc"]);
							fCustomized = DB.Bool(drPrice["PropAllInclDescCustFlg"]);
							PriceType = DB.Str(drPrice["InvAllInclDesc"]);
							Default = string.Format("{0}: {1}", PriceType, FmtPrice);
							if (!fCustomized && PropDesc.Length > 0 && FmtPrice != Zero)
							{
								PropDesc = Default;
							}
							break;
					}

					//if (PropDesc.Length > 0)
					{
						HtmlGenericControl tr = new HtmlGenericControl("tr");

						HtmlGenericControl td = new HtmlGenericControl("td");
						td.Attributes.Add("align", "right");
                        td.InnerHtml = string.Format("{0} (<span class=\"Amt\">{1}</span>)", PriceType, FmtPrice);
						tr.Controls.Add(td);

						td = new HtmlGenericControl("td");

						HtmlTextArea txt = new HtmlTextArea();
						txt.ID = string.Format("txtPrice_{0}", Rno);
						txt.Attributes.Add("class", "FeeDesc");
						txt.Attributes.Add("data-default", Default);
						txt.Value = PropDesc;
						td.Controls.Add(txt);

						HtmlInputHidden hf = new HtmlInputHidden();
						hf.ID = string.Format("hfPriceCustomized_{0}", Rno);
						hf.Value = Customized(fCustomized);
						td.Controls.Add(hf);

						HtmlGenericControl ul = new HtmlGenericControl("ul");
						ul.ID = string.Format("ulPrice_{0}", Rno);
						ul.Attributes.Add("class", "Options");
						td.Controls.Add(ul);

						RenderOptions(ul, PerPerson, Options);

						tr.Controls.Add(td);
						td = new HtmlGenericControl("td");

						HtmlGenericControl lbl = new HtmlGenericControl("label");
						lbl.Attributes.Add("class", "Reset");
						lbl.Attributes.Add("title", "Click to reset the description and show all the options.");
						lbl.InnerText = "Undo Custom";
						td.Controls.Add(lbl);

						lbl = new HtmlGenericControl("label");
						lbl.Attributes.Add("class", "SeeOptions");
						lbl.InnerText = "click Undo Custom to see available options";
						td.Controls.Add(lbl);

						tr.Controls.Add(td);
						phPrices.Controls.Add(tr);
					}

					//Total += ExtPrice;
				}

				//SalesTax += Math.Round(Total * SubTotTaxPct / 100, 2);

				lblAdj1Desc.Text = DB.Str(dr["Adj1Desc"]);
				lblAdj2Desc.Text = DB.Str(dr["Adj2Desc"]);

				decimal ServiceAmt		= DB.Dec(dr["ServiceAmt"]);
				decimal DeliveryAmt		= DB.Dec(dr["DeliveryAmt"]);
				decimal ChinaAmt		= DB.Dec(dr["ChinaAmt"]);
				decimal AddServiceAmt	= DB.Dec(dr["AddServiceAmt"]);
				decimal FuelTravelAmt	= DB.Dec(dr["FuelTravelAmt"]);
				decimal FacilityAmt		= DB.Dec(dr["FacilityAmt"]);
				decimal RentalsAmt		= DB.Dec(dr["RentalsAmt"]);
				decimal Adj1Amt			= DB.Dec(dr["Adj1Amt"]);
				decimal Adj2Amt			= DB.Dec(dr["Adj2Amt"]);
				decimal DepositAmt		= DB.Dec(dr["DepositAmt"]);

				decimal ServicePct		= DB.Dec(dr["ServiceTaxPct"]);
				decimal DeliveryPct		= DB.Dec(dr["DeliveryTaxPct"]);
				decimal ChinaPct		= DB.Dec(dr["ChinaTaxPct"]);
				decimal AddServicePct	= DB.Dec(dr["AddServiceTaxPct"]);
				decimal FuelTravelPct	= DB.Dec(dr["FuelTravelTaxPct"]);
				decimal FacilityPct		= DB.Dec(dr["FacilityTaxPct"]);
				decimal RentalsPct		= DB.Dec(dr["RentalsTaxPct"]);
				decimal Adj1Pct			= DB.Dec(dr["Adj1TaxPct"]);
				decimal Adj2Pct			= DB.Dec(dr["Adj2TaxPct"]);

				lblServiceAmt.Text		= Fmt.Dollar(ServiceAmt);
				lblDeliveryAmt.Text		= Fmt.Dollar(DeliveryAmt);
				lblChinaAmt.Text		= Fmt.Dollar(ChinaAmt);
				lblAddServiceAmt.Text	= Fmt.Dollar(AddServiceAmt);
				lblFuelTravelAmt.Text	= Fmt.Dollar(FuelTravelAmt);
				lblFacilityAmt.Text		= Fmt.Dollar(FacilityAmt);
				lblRentalsAmt.Text		= Fmt.Dollar(RentalsAmt);
				lblAdj1Amt.Text			= Fmt.Dollar(Adj1Amt);
				lblAdj2Amt.Text			= Fmt.Dollar(Adj2Amt);
				lblDepositAmt.Text		= Fmt.Dollar(DepositAmt);

				txtServiceDesc.Text		= DB.Str(dr["ServicePropDesc"]);
                txtDeliveryDesc.Text	= DB.Str(dr["DeliveryPropDesc"]);
                txtChinaDesc.Text		= DB.Str(dr["ChinaPropDesc"]);
                txtAddServiceDesc.Text	= DB.Str(dr["AddServicePropDesc"]);
                txtFuelTravelDesc.Text	= DB.Str(dr["FuelTravelPropDesc"]);
                txtFacilityDesc.Text	= DB.Str(dr["FacilityPropDesc"]);
                txtRentalsDesc.Text		= DB.Str(dr["RentalsPropDesc"]);
                txtAdj1Desc.Text		= DB.Str(dr["Adj1PropDesc"]);
                txtAdj2Desc.Text		= DB.Str(dr["Adj2PropDesc"]);
				txtTotalDesc.Text		= DB.Str(dr["EstTotPropDesc"]);
				txtDepositDesc.Text		= DB.Str(dr["DepositPropDesc"]);

				//SalesTax += 
				//	Math.Round(ServiceAmt		* ServicePct	/ 100, 2) +
				//	Math.Round(DeliveryAmt		* DeliveryPct	/ 100, 2) +
				//	Math.Round(ChinaAmt			* ChinaPct		/ 100, 2) +
				//	Math.Round(AddServiceAmt	* AddServicePct / 100, 2) +
				//	Math.Round(FuelTravelAmt	* FuelTravelPct / 100, 2) +
				//	Math.Round(FacilityAmt		* FacilityPct	/ 100, 2) +
				//	Math.Round(RentalsAmt		* RentalsPct	/ 100, 2) +
				//	Math.Round(Adj1Amt			* Adj1Pct		/ 100, 2) +
				//	Math.Round(Adj2Amt			* Adj2Pct		/ 100, 2);

				//Total +=
				//	ServiceAmt +
				//	DeliveryAmt +
				//	ChinaAmt +
				//	AddServiceAmt +
				//	FuelTravelAmt +
				//	FacilityAmt +
				//	RentalsAmt +
				//	Adj1Amt +
				//	Adj2Amt;

				//bool fTaxExempt = DB.Bool(dr["TaxExemptFlg"]);
				//if (!fTaxExempt)
				//{
				//	Total += SalesTax;
				//}

				//bool fCreditCardPaymentFee = DB.Bool(dr["CCPmtFeeFlg"]);
				//if (fCreditCardPaymentFee)
				//{
				//	decimal CreditCardFeePct = DB.Dec(dr["CCFeePct"]);
				//	decimal CreditCardFeeAmt = Math.Round(Total * CreditCardFeePct / 100, 2);
				//	decimal CreditCardFeeTaxPct = DB.Dec(dr["CCFeeTaxPct"]);
				//	decimal CreditCardFeeTaxAmt = 0;
				//	if (!fTaxExempt)
				//	{
				//		CreditCardFeeTaxAmt = Math.Round(CreditCardFeeAmt * CreditCardFeeTaxPct / 100, 2);
				//	}

				//	SalesTax += CreditCardFeeTaxAmt;
				//	Total += CreditCardFeeAmt + CreditCardFeeTaxAmt;
				//}

				decimal SalesTax = DB.Dec(dr["SalesTaxTotAmt"]);
				decimal Total = DB.Dec(dr["InvTotAmt"]);
                decimal Balance = Total - DepositAmt; // DB.Dec(dr["InvBalAmt"]);

                bool fServiceCustomized		= DB.Bool(dr["ServicePropDescCustFlg"]);
                bool fDeliveryCustomized	= DB.Bool(dr["DeliveryPropDescCustFlg"]);
                bool fChinaCustomized		= DB.Bool(dr["ChinaPropDescCustFlg"]);
                bool fAddServiceCustomized	= DB.Bool(dr["AddServicePropDescCustFlg"]);
                bool fFuelTravelCustomized	= DB.Bool(dr["FuelTravelPropDescCustFlg"]);
                bool fFacilityCustomized	= DB.Bool(dr["FacilityPropDescCustFlg"]);
                bool fRentalsCustomized		= DB.Bool(dr["RentalsPropDescCustFlg"]);
				bool fTotalCustomized		= DB.Bool(dr["EstTotPropDescCustFlg"]);
				bool fDepositCustomized		= DB.Bool(dr["DepositPropDescCustFlg"]);

                string ServiceOptions		= DB.Str(dr["ServicePropOptions"]);
                string DeliveryOptions		= DB.Str(dr["DeliveryPropOptions"]);
                string ChinaOptions			= DB.Str(dr["ChinaPropOptions"]);
                string AddServiceOptions	= DB.Str(dr["AddServicePropOptions"]);
                string FuelTravelOptions	= DB.Str(dr["FuelTravelPropOptions"]);
                string FacilityOptions		= DB.Str(dr["FacilityPropOptions"]);
                string RentalsOptions		= DB.Str(dr["RentalsPropOptions"]);
				string TotalOptions			= DB.Str(dr["EstTotPropOptions"]);
				string DepositOptions		= DB.Str(dr["DepositPropOptions"]);

                hfServiceCustomized.Value		= Customized(fServiceCustomized);
                hfDeliveryCustomized.Value		= Customized(fDeliveryCustomized);
                hfChinaCustomized.Value			= Customized(fChinaCustomized);
                hfAddServiceCustomized.Value	= Customized(fAddServiceCustomized);
                hfFuelTravelCustomized.Value	= Customized(fFuelTravelCustomized);
                hfFacilityCustomized.Value		= Customized(fFacilityCustomized);
                hfRentalsCustomized.Value		= Customized(fRentalsCustomized);
				hfTotalCustomized.Value			= Customized(fTotalCustomized);
				hfDepositCustomized.Value		= Customized(fDepositCustomized);
				
                //trAdj1.Visible = (lblAdj1Desc.Text.Length > 0);
                //trAdj2.Visible = (lblAdj2Desc.Text.Length > 0);

                RenderOptions(ulService, Service, ServiceOptions);
				RenderOptions(ulDelivery, Delivery, DeliveryOptions);
				RenderOptions(ulChina, China, ChinaOptions);
				RenderOptions(ulAddService, AddService, AddServiceOptions);
				RenderOptions(ulFuelTravel, FuelTravel, FuelTravelOptions);
				RenderOptions(ulFacility, Facility, FacilityOptions);
				RenderOptions(ulRentals, Rentals, RentalsOptions);

				Default = (lblServiceAmt.Text != Zero ? string.Format("{0}: {1}", DefaultService + " (gratuity not included)", lblServiceAmt.Text) : string.Empty);
				txtServiceDesc.Attributes.Add("data-default", Default);
				if (!fServiceCustomized && lblServiceAmt.Text != Zero) 
				{ 
					txtServiceDesc.Text = Default; 
				}

				Default = (lblDeliveryAmt.Text != Zero ? string.Format("{0}: {1}", DefaultDelivery, lblDeliveryAmt.Text) : string.Empty);
				txtDeliveryDesc.Attributes.Add("data-default", Default);
				if (!fDeliveryCustomized && lblDeliveryAmt.Text != Zero) 
				{ 
					txtDeliveryDesc.Text = Default; 
				}
				
				Default = (lblChinaAmt.Text != Zero ? string.Format("{0}: {1}", DefaultChina, lblChinaAmt.Text) : string.Empty);
				txtChinaDesc.Attributes.Add("data-default", Default);
				if (!fChinaCustomized && lblChinaAmt.Text != Zero) 
				{ 
					txtChinaDesc.Text = Default; 
				}

				Default = (lblAddServiceAmt.Text != Zero ? string.Format("{0}: {1}", DefaultAddService, lblAddServiceAmt.Text) : string.Empty);
				txtAddServiceDesc.Attributes.Add("data-default", Default);
				if (!fAddServiceCustomized && lblAddServiceAmt.Text != Zero) 
				{ 
					txtAddServiceDesc.Text = Default; 
				}

				Default = (lblFuelTravelAmt.Text != Zero ? string.Format("{0}: {1}", DefaultFuelTravel, lblFuelTravelAmt.Text) : string.Empty);
				txtFuelTravelDesc.Attributes.Add("data-default", Default);
				if (!fFuelTravelCustomized && lblFuelTravelAmt.Text != Zero) 
				{ 
					txtFuelTravelDesc.Text = Default; 
				}

				Default = (lblFacilityAmt.Text != Zero ? string.Format("{0}: {1}", DefaultFacility, lblFacilityAmt.Text) : string.Empty);
				txtFacilityDesc.Attributes.Add("data-default", Default);
				if (!fFacilityCustomized && lblFacilityAmt.Text != Zero) 
				{ 
					txtFacilityDesc.Text = Default; 
				}

				Default = (lblRentalsAmt.Text != Zero ? string.Format("{0}: {1}", DefaultRentals, lblRentalsAmt.Text) : string.Empty);
				txtRentalsDesc.Attributes.Add("data-default", Default);
				if (!fRentalsCustomized && lblRentalsAmt.Text != Zero) 
				{ 
					txtRentalsDesc.Text = Default; 
				}

				Default = (lblAdj1Amt.Text != Zero ? string.Format("{0}: {1}", lblAdj1Desc.Text, lblAdj1Amt.Text) : string.Empty);
				txtAdj1Desc.Attributes.Add("data-default", Default);
				if (txtAdj1Desc.Text.Length == 0 && lblAdj1Amt.Text != Zero) 
				{ 
					txtAdj1Desc.Text = Default; 
				}

				Default = (lblAdj2Amt.Text != Zero ? string.Format("{0}: {1}", lblAdj2Desc.Text, lblAdj2Amt.Text) : string.Empty);
				txtAdj2Desc.Attributes.Add("data-default", Default);
				if (txtAdj2Desc.Text.Length == 0 && lblAdj2Amt.Text != Zero) 
				{ 
					txtAdj2Desc.Text = Default; 
				}

				int NumServings = DB.Int32(dr["NumMenServing"]) + DB.Int32(dr["NumWomenServing"]) + DB.Int32(dr["NumChildServing"]);
				lblTotalAmt.Text = Fmt.Dollar(Total);
				Default = string.Format("Estimated Total: {0} - Based on {1} guests. Price per person and on-site service are subject to change as guest count changes.", Fmt.Dollar(Total), Fmt.Num(NumServings));
				txtTotalDesc.Attributes.Add("data-default", Default);
				if (!fTotalCustomized) 
				{ 
					txtTotalDesc.Text = Default; 
				}

				Default = (lblDepositAmt.Text != Zero ? string.Format("{0}: {1}, remaining Invoice Balance: {2}", DefaultDeposit, lblDepositAmt.Text, Fmt.Dollar(Balance)) : string.Empty);
				txtDepositDesc.Attributes.Add("data-default", Default);
				if (txtDepositDesc.Text.Length == 0 && lblDepositAmt.Text != Zero)
				{
					txtDepositDesc.Text = Default;
				}

				bool fProposal =  DB.Bool(dr["ProposalFlg"]);
				DateTime dtPropCreated = DB.DtTm(dr["PropCreatedDtTm"]);
				DateTime dtPropEmailed = DB.DtTm(dr["PropEmailedDtTm"]);
				DateTime dtConfirmEmailed = DB.DtTm(dr["ConfirmEmailedDtTm"]);
                string JobType = DB.Str(dr["JobType"]);

                EmailType = (fProposal ? "Proposal" : (JobType == "Private" ? "Private Confirmation" : "Corporate Confirmation"));
				txtEmail.Text = DB.Str(dr["Email"]);
				txtSubject.Text = (fProposal ? (dtPropEmailed == DateTime.MinValue ? string.Empty : "Revised ") + "Proposal" : (dtConfirmEmailed == DateTime.MinValue ? string.Empty : "Revised ") + "Confirmation") + " from " + Globals.g.Company.Name;
				txtMessage.Text = (fProposal ? "Attached is your " + (dtPropEmailed == DateTime.MinValue ? string.Empty : "revised ") + "proposal from " + Globals.g.Company.Name + ".\n\nWe appreciate your business." : (dtConfirmEmailed == DateTime.MinValue ? string.Empty : "Attached is your revised confirmation from " + Globals.g.Company.Name + ".\n\nWe appreciate your business."));
                chkTermsOfService.Checked = (JobType == "Private");

                btnEmail.Visible = fProposal;
				btnEmailConfirm.Visible = !fProposal;

				if (dtPropCreated == DateTime.MinValue)
				{
					btnEmail.Attributes.Add("disabled", "disabled");
					btnEmail.Attributes.Add("title", "Save Client Pricing and then email.");
					btnEmailConfirm.Attributes.Add("disabled", "disabled");
					btnEmailConfirm.Attributes.Add("title", "Save Client Pricing and then email.");
				}

				if (dtPropEmailed != DateTime.MinValue)
				{
					btnEmail.Value = "Re-Email Proposal";
				}
				if (dtConfirmEmailed != DateTime.MinValue)
				{
					btnEmailConfirm.Value = "Re-Email Confirm";
				}

				txtCreatedDt.Text = Fmt.DtTm(dtPropCreated);
				txtCreatedUser.Text = DB.Str(dr["PropCreatedUser"]);
				txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["PropUpdatedDtTm"]));
				txtUpdatedUser.Text = DB.Str(dr["PropUpdatedUser"]);
				txtGeneratedDt.Text = Fmt.DtTm(DB.DtTm(dr["PropGeneratedDtTm"]));
				txtGeneratedUser.Text = DB.Str(dr["PropGeneratedUser"]);
				txtPropEmailedDt.Text = Fmt.DtTm(dtPropEmailed);
				txtPropEmailedUser.Text = DB.Str(dr["PropEmailedUser"]);
				txtConfirmEmailedDt.Text = Fmt.DtTm(dtConfirmEmailed);
				txtConfirmEmailedUser.Text = DB.Str(dr["ConfirmEmailedUser"]);
				pnlPreview.ToolTip = string.Format("{0} Preview", (fProposal ? "Proposal" : "Confirmation"));
			}
        }
        catch (Exception Ex)
        {
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
        }
	}

    protected string Customized(bool fCustomized) 
    { 
        return (fCustomized ? "1" : "0"); 
    }

    protected void RenderOptions(HtmlGenericControl ul, string PricingCode, string Options)
    {
        string[] aOptions = Options.Split(',');
		bool fUseDefaults = (aOptions.Length == 1 && aOptions[0] == string.Empty);
        string Sql = string.Format(
            "Select * From ProposalPricingOptions " +
            "Where PricingCode = '{0}' " +
            "And Seq Is Not Null And " +
            "DeletedDtTm Is Null " +
            "Order by Seq", 
            PricingCode);
        try
        {
            DataTable dt = db.DataTable(Sql);
            {
                foreach (DataRow dr in dt.Rows)
	            {
		            int PropPricingRno = DB.Int32(dr["PropPricingRno"]);
                    string Type = DB.Str(dr["OptionType"]);
                    string Desc = DB.Str(dr["OptionCheckboxDesc"]);
                    string PropDesc = DB.Str(dr["OptionPropDesc"]).Replace("<Initials>", g.Company.Initials);

                    string Default = DB.Str(dr["OPtionDefault"]);

                    HtmlGenericControl li = new HtmlGenericControl("li");
                    HtmlGenericControl lbl;
                    switch (Type)
                    {
						case OptionType.Hidden:
                        case OptionType.Checkbox:
                            HtmlInputCheckBox chk = new HtmlInputCheckBox();
                            chk.ID = string.Format("opt_{0}", PropPricingRno);
                            chk.Value = "1";
                            chk.Attributes.Add("data-desc", PropDesc);

							if (fUseDefaults)
							{
								chk.Checked = (Default != string.Empty);
							}
							else
							{
								foreach (string Option in aOptions)
								{
									if (PropPricingRno.ToString() == Option)
									{
										chk.Checked = true;
										break;
									}
								}
							}
                            li.Controls.Add(chk);

                            lbl = new HtmlGenericControl("label");
                            lbl.Attributes.Add("for", chk.ID);
                            lbl.InnerText = Desc;
                            li.Controls.Add(lbl);

							if (Type == OptionType.Hidden)
							{
								chk.Checked = true;
								li.Style.Add("display", "none");
							}
                            break;

                        case OptionType.Number:
                            lbl = new HtmlGenericControl("label");
                            lbl.InnerText = string.Format("{0} ", Desc);
                            li.Controls.Add(lbl);

                            HtmlInputText txt = new HtmlInputText();
                            txt.ID = string.Format("opt_{0}", PropPricingRno);
                            txt.Attributes.Add("class", "NumCrew");
                            txt.Attributes.Add("data-desc", PropDesc);

							if (fUseDefaults)
							{
								txt.Value = Default;
							}
							else
							{
								foreach (string Option in aOptions)
								{
									string[] aValues = Option.Split(' ');
									if (PropPricingRno.ToString() == aValues[0])
									{
										txt.Value = (aValues[1] == "0" ? string.Empty : aValues[1]);
										break;
									}
								}
							}
                            li.Controls.Add(txt);
                            break;
                    }

                    ul.Controls.Add(li);
	            }
            } 
        }
        catch (Exception Ex)
        {
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
        }
    }

	protected void SaveValues()
	{
		string Sql = string.Format("Select PropCreatedDtTm From mcJobs Where JobRno = {0}", JobRno);
        try
        {
			DateTime dtCreated = db.SqlDtTm(Sql);

			Sql = string.Format(
				"Update mcJobs Set " +
				"ServicePropDesc = {1}, " +
				"DeliveryPropDesc = {2}, " +
				"ChinaPropDesc = {3}, " +
				"AddServicePropDesc = {4}, " +
				"FuelTravelPropDesc = {5}, " +
				"FacilityPropDesc = {6}, " +
				"RentalsPropDesc = {7}, " +
				"Adj1PropDesc = {8}, " +
				"Adj2PropDesc = {9}, " +
				"ServicePropDescCustFlg = {10}, " +
				"DeliveryPropDescCustFlg = {11}, " +
				"ChinaPropDescCustFlg = {12}, " +
				"AddServicePropDescCustFlg = {13}, " +
				"FuelTravelPropDescCustFlg = {14}, " +
				"FacilityPropDescCustFlg = {15}, " +
				"RentalsPropDescCustFlg = {16}, " +
				"ServicePropOptions = '{17}', " +
				"DeliveryPropOptions = '{18}', " +
				"ChinaPropOptions = '{19}', " +
				"AddServicePropOptions = '{20}', " +
				"FuelTravelPropOptions = '{21}', " +
				"FacilityPropOptions = '{22}', " +
				"RentalsPropOptions = '{23}', " +
				"EstTotPropDesc = {24}, " +
				"EstTotPropDescCustFlg = {25}, " +
				"DepositPropDesc = {26}, " +
				"DepositPropDescCustFlg = {27}, " +
				"Prop{28}DtTm = '{29}', " + 
				"Prop{28}User = '{30}' " + 
				"Where JobRno = {0}",
				JobRno,
				DB.PutStr(txtServiceDesc.Text, 1000),
				DB.PutStr(txtDeliveryDesc.Text, 1000),
				DB.PutStr(txtChinaDesc.Text, 1000),
				DB.PutStr(txtAddServiceDesc.Text, 1000),
				DB.PutStr(txtFuelTravelDesc.Text, 1000),
				DB.PutStr(txtFacilityDesc.Text, 1000),
				DB.PutStr(txtRentalsDesc.Text, 1000),
				DB.PutStr(txtAdj1Desc.Text, 1000),
				DB.PutStr(txtAdj2Desc.Text, 1000),
				hfServiceCustomized.Value,
				hfDeliveryCustomized.Value,
				hfChinaCustomized.Value,
				hfAddServiceCustomized.Value,
				hfFuelTravelCustomized.Value,
				hfFacilityCustomized.Value,
				hfRentalsCustomized.Value,
				ResponseOptions(Service),
				ResponseOptions(Delivery),
				ResponseOptions(China),
				ResponseOptions(AddService),
				ResponseOptions(FuelTravel),
				ResponseOptions(Facility),
				ResponseOptions(Rentals),
				DB.PutStr(txtTotalDesc.Text, 1000),
				hfTotalCustomized.Value,
				DB.PutStr(txtDepositDesc.Text, 1000),
				hfDepositCustomized.Value,
				(dtCreated == DateTime.MinValue ? "Created" : "Updated"), 
				DateTime.Now,
				g.User);

			// job info
            db.Exec(Sql);

			// prices
			Sql = string.Format("Select * From JobInvoicePrices Where JobRno = {0} Order By Seq", JobRno);
			DataTable dtPrices = db.DataTable(Sql);
			foreach (DataRow drPrice in dtPrices.Rows)
			{
				int Rno = DB.Int32(drPrice["Rno"]);
				string PriceType = DB.Str(drPrice["PriceType"]);

				switch (PriceType)
				{
					case Misc.cnPerPerson:
						Sql = string.Format(
							"Update JobInvoicePrices Set PropPerPersonDesc = {1}, PropPerPersonDescCustFlg = {2}, PropPerPersonOptions = {3} Where Rno = {0}", 
							Rno,
							DB.Put(Parm.Str(string.Format("txtPrice_{0}", Rno))),
                            DB.Put(Parm.Str(string.Format("hfPriceCustomized_{0}", Rno))),
							DB.Put(ResponseOptions(PerPerson)));
						db.Exec(Sql);
						break;

					case Misc.cnPerItem:
						Sql = string.Format(
							"Update JobInvoicePrices Set PropPerItemDesc = {1}, PropPerItemDescCustFlg = {2} Where Rno = {0}", 
							Rno,
							DB.Put(Parm.Str(string.Format("txtPrice_{0}", Rno))),
							Parm.Str(string.Format("hfPriceCustomized_{0}", Rno)));
						db.Exec(Sql);
						break;

					case Misc.cnAllIncl:
						Sql = string.Format(
							"Update JobInvoicePrices Set PropAllInclDesc = {1}, PropAllInclDescCustFlg = {2} Where Rno = {0}", 
							Rno,
                            DB.Put(Parm.Str(string.Format("txtPrice_{0}", Rno))),
							Parm.Str(string.Format("hfPriceCustomized_{0}", Rno)));
						db.Exec(Sql);
						break;
				}
			}
		}
        catch (Exception Ex)
        {
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
        }
	}

    // build a comma delimited list of PropPricingRno values that are checked
    protected string ResponseOptions(string PricingCode)
    {
        string Options = string.Empty;
        string Sql = string.Format(
            "Select PropPricingRno, OptionType From ProposalPricingOptions " +
            "Where PricingCode = '{0}' " +
            "And Seq Is Not Null And " +
            "DeletedDtTm Is Null " +
            "Order by Seq",
            PricingCode);
        try
        {
            DataTable dt = db.DataTable(Sql);
            {
                foreach (DataRow dr in dt.Rows)
                {
                    int PropPricingRno = DB.Int32(dr["PropPricingRno"]);
                    string Type = DB.Str(dr["OptionType"]);
                    string ID = string.Format("opt_{0}", PropPricingRno);

                    switch (Type)
                    {
						case OptionType.Hidden:
							break;

						case OptionType.Checkbox:
                            if (Parm.Bool(ID))
                            {
                                Options += string.Format(",{0}", PropPricingRno);
                            }
                            break;

						case OptionType.Number:
                            int Num = Parm.Int(ID);
                            Options += string.Format(",{0} {1}", PropPricingRno, Num);
                            break;
                    }
                }
            }

            if (Options.Length > 0)
            {
                Options = Options.Substring(1);
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }

        return Options;
    }

	protected void ClearValues()
	{
		txtServiceDesc.Text =
		txtDeliveryDesc.Text =
		txtChinaDesc.Text =
		txtAddServiceDesc.Text =
		txtFuelTravelDesc.Text = 
		txtFacilityDesc.Text =
		txtRentalsDesc.Text =
		lblAdj1Desc.Text = 
		txtAdj1Desc.Text =
		lblAdj2Desc.Text =
		txtAdj2Desc.Text = 
		txtEmail.Text =
		txtSubject.Text =
		txtMessage.Text =
		txtCreatedDt.Text =
		txtCreatedUser.Text =
		txtUpdatedDt.Text =
		txtUpdatedUser.Text =
		txtGeneratedDt.Text =
		txtGeneratedUser.Text =
		txtPropEmailedDt.Text =
		txtPropEmailedUser.Text =
		txtConfirmEmailedDt.Text =
		txtConfirmEmailedUser.Text = string.Empty;

		btnEmail.Visible = true;
		btnEmail.Disabled = false;
		btnEmail.Attributes.Add("text", "Email Proposal");
		btnEmail.Attributes.Add("title", "Email the proposal to the customer.");
		btnEmailConfirm.Visible = true;
		btnEmailConfirm.Disabled = false;
		btnEmailConfirm.Attributes.Add("text", "Email Confirm");
		btnEmailConfirm.Attributes.Add("title", "Email the confirmation to the customer.");
	}

	protected void SendEmail()
	{
		Misc.SendProposalEmail(Page, db, JobRno, txtEmail.Text, txtSubject.Text, txtMessage.Text, chkTermsOfService.Checked, EmailAttachment);
	}

	protected void SaveAttachment()
	{
		EmailAttachment = string.Empty;

		if (fuAttachment.HasFile)
		{
			WebConfig Cfg = new WebConfig();
			string AttachmentDir = Cfg.Str("Attachment Dir");

			if (AttachmentDir.Length == 0)
			{
				ErrMsg += "The 'Attachment Dir' value '{0}' is missing in the Web.config file.<br />";
			}
			else if (!Directory.Exists(AttachmentDir))
			{
				ErrMsg += string.Format("The 'Attachment Dir' value '{0}' in the Web.config file points to an invalid directory.<br />", AttachmentDir);
			}
			else
			{
				try
				{
					string Filename = string.Format(@"{0}\{1} - {2}", AttachmentDir, JobRno, fuAttachment.FileName);
					if (File.Exists(Filename))
					{
						File.Delete(Filename);
					}
					fuAttachment.SaveAs(Filename);

					EmailAttachment = Filename;
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex);
					Response.Write(Err.Html());
				}
			}
		}
	}
}
