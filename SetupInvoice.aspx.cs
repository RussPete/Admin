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

public partial class SetupInvoice : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.MenuSetup.Title;
            ClearData();
			Setup();
		}
	}

    private void Setup()
    {
		string Sql = "Select Top 1 SettingRno From Settings Order By SettingRno";
		try 
		{
			int Rno = db.SqlNum(Sql);
			if (Rno != 0)
			{
				GetData(Rno);
			}
		}
		catch (Exception Ex)
		{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
		}
    }
    
	protected void ClearData()
	{
		txtSubTotTaxPct.Text =
		txtServiceSubTotPct.Text =
		txtServiceTaxPct.Text =
		txtDeliverySubTotPct.Text =
		txtDeliveryTaxPct.Text =
		txtChinaSubTotPct.Text =
		txtChinaTaxPct.Text =
		txtAddServiceSubTotPct.Text =
		txtAddServiceTaxPct.Text =
		txtFuelTravelSubTotPct.Text =
		txtFuelTravelTaxPct.Text =
		txtFacilitySubTotPct.Text =
		txtFacilityTaxPct.Text =
		txtGratuitySubTotPct.Text =
		txtGratuityTaxPct.Text =
		txtRentalsSubTotPct.Text =
		txtRentalsTaxPct.Text =
		txtAdj1TaxPct.Text =
		txtAdj2TaxPct.Text =
		txtCCFeePct.Text =
		txtCCFeeTaxPct.Text = string.Empty;
	}

    protected void GetData(int Rno)
    {
        string Sql = string.Format("Select * From Settings Where SettingRno = {0}", Rno);

		ClearData();

        try
        {
            DataTable dt = db.DataTable(Sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];

				hfRno.Value					= Rno.ToString();
				txtSubTotTaxPct.Text		= Fmt.Num(DB.Dec(dr["SubTotTaxPct"]), 4, false);
				txtServiceSubTotPct.Text	= Fmt.Num(DB.Dec(dr["ServiceSubTotPct"]), 4, false);
				txtServiceTaxPct.Text		= Fmt.Num(DB.Dec(dr["ServiceTaxPct"]), 4, false);
				txtDeliverySubTotPct.Text	= Fmt.Num(DB.Dec(dr["DeliverySubTotPct"]), 4, false);
				txtDeliveryTaxPct.Text		= Fmt.Num(DB.Dec(dr["DeliveryTaxPct"]), 4, false);
				txtChinaSubTotPct.Text		= Fmt.Num(DB.Dec(dr["ChinaSubTotPct"]), 4, false);
				txtChinaTaxPct.Text			= Fmt.Num(DB.Dec(dr["ChinaTaxPct"]), 4, false);
				txtAddServiceSubTotPct.Text = Fmt.Num(DB.Dec(dr["AddServiceSubTotPct"]), 4, false);
				txtAddServiceTaxPct.Text	= Fmt.Num(DB.Dec(dr["AddServiceTaxPct"]), 4, false);
				txtFuelTravelSubTotPct.Text = Fmt.Num(DB.Dec(dr["FuelTravelSubTotPct"]), 4, false);
				txtFuelTravelTaxPct.Text	= Fmt.Num(DB.Dec(dr["FuelTravelTaxPct"]), 4, false);
				txtFacilitySubTotPct.Text	= Fmt.Num(DB.Dec(dr["FacilitySubTotPct"]), 4, false);
				txtFacilityTaxPct.Text		= Fmt.Num(DB.Dec(dr["FacilityTaxPct"]), 4, false);
				txtGratuitySubTotPct.Text	= Fmt.Num(DB.Dec(dr["GratuitySubTotPct"]), 4, false);
				txtGratuityTaxPct.Text		= Fmt.Num(DB.Dec(dr["GratuityTaxPct"]), 4, false);
				txtRentalsSubTotPct.Text	= Fmt.Num(DB.Dec(dr["RentalsSubTotPct"]), 4, false);
				txtRentalsTaxPct.Text		= Fmt.Num(DB.Dec(dr["RentalsTaxPct"]), 4, false);
				txtAdj1TaxPct.Text			= Fmt.Num(DB.Dec(dr["Adj1TaxPct"]), 4, false);
				txtAdj2TaxPct.Text			= Fmt.Num(DB.Dec(dr["Adj2TaxPct"]), 4, false);
				txtCCFeePct.Text			= Fmt.Num(DB.Dec(dr["CCFeePct"]), 4, false);
				txtCCFeeTaxPct.Text			= Fmt.Num(DB.Dec(dr["CCFeeTaxPct"]), 4, false);
			}
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

	protected void SaveData()
	{
		int Rno = Str.Num(hfRno.Value);
		string Sql = string.Empty;

		try
		{
			Sql = string.Format(
				"Update Settings Set " +
				"SubTotTaxPct = {1}, " +
				"ServiceSubTotPct = {2}, " +
				"ServiceTaxPct = {3}, " +
				"DeliverySubTotPct = {4}, " +
				"DeliveryTaxPct = {5}, " +
				"ChinaSubTotPct = {6}, " +
				"ChinaTaxPct = {7}, " +
				"AddServiceSubTotPct = {8}, " +
				"AddServiceTaxPct = {9}, " +
				"FuelTravelSubTotPct = {10}, " +
				"FuelTravelTaxPct = {11}, " +
				"FacilitySubTotPct = {12}, " +
				"FacilityTaxPct = {13}, " +
				"GratuitySubTotPct = {14}, " +
				"GratuityTaxPct = {15}, " +
				"RentalsSubTotPct = {16}, " +
				"RentalsTaxPct = {17}, " +
				"Adj1TaxPct = {18}, " +
				"Adj2TaxPct = {19}, " +
				"CCFeePct = {20}, " +
				"CCFeeTaxPct = {21} " +
				"Where SettingRno = {0}",
				Rno,
				Str.Dec(txtSubTotTaxPct.Text), 
				Str.Dec(txtServiceSubTotPct.Text), 
				Str.Dec(txtServiceTaxPct.Text), 
				Str.Dec(txtDeliverySubTotPct.Text), 
				Str.Dec(txtDeliveryTaxPct.Text), 
				Str.Dec(txtChinaSubTotPct.Text), 
				Str.Dec(txtChinaTaxPct.Text), 
				Str.Dec(txtAddServiceSubTotPct.Text), 
				Str.Dec(txtAddServiceTaxPct.Text), 
				Str.Dec(txtFuelTravelSubTotPct.Text), 
				Str.Dec(txtFuelTravelTaxPct.Text), 
				Str.Dec(txtFacilitySubTotPct.Text), 
				Str.Dec(txtFacilityTaxPct.Text), 
				Str.Dec(txtGratuitySubTotPct.Text), 
				Str.Dec(txtGratuityTaxPct.Text), 
				Str.Dec(txtRentalsSubTotPct.Text),
				Str.Dec(txtRentalsTaxPct.Text),
				Str.Dec(txtAdj1TaxPct.Text),
				Str.Dec(txtAdj2TaxPct.Text),
				Str.Dec(txtCCFeePct.Text), 
				Str.Dec(txtCCFeeTaxPct.Text));
			db.Exec(Sql);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		SaveData();
	}
}