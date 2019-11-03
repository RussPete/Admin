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

public partial class PaymentSearch : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;


	protected decimal Subtotal = 0;
	protected decimal OtherTotal = 0;
	protected decimal SalesTaxTotal = 0;
	protected decimal Total = 0;
    protected decimal Balance = 0;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.Accounting.Title;
		}
	}

	protected string DecryptCCNum(DataRow dr, string Num)
	{
		string CCNum = DB.Str(dr["CCNum" + Num]);

		try
		{
			CCNum = Crypt.Decrypt(CCNum, Misc.Password);
		}
		catch (Exception)
		{
		}

		return CCNum;
	}

	public string FindPayment()
	{
		string Html = string.Empty;
		string Sql = string.Empty;

		try
		{
            if (txtPaymentAmt.Text.Length > 0)
            {
                decimal RptTotal = 0;

                Sql = string.Format(
                    "Select Coalesce(cu.Name, c.Name) as Customer, *, 1 as Section " +
                    "From mcJobs j " +
                    "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                    "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
                    "Where {0} = j.InvBalAmt And j.InvCreatedDtTm Is Not Null And j.JobDate >= DateAdd(day, -90, GetDate())\n" +
                    "union\n" +
                    "Select Coalesce(cu.Name, c.Name) as Customer, *, 2 as Section " +
                    "From mcJobs j " +
                    "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                    "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
                    "Where {0} Between j.InvBalAmt And j.InvBalAmt + 0.25 * j.SubTotAmt And j.InvBalAmt > 0 And j.InvCreatedDtTm Is Not Null And j.JobDate >= DateAdd(day, -90, GetDate())\n" +
                    "Order by Section, JobDate",
                    Str.Dec(txtPaymentAmt.Text));
                DataTable dt = db.DataTable(Sql);
                foreach (DataRow dr in dt.Rows)
                {
                    InvoiceAmounts(dr);

                    string Customer = DB.Str(dr["Customer"]);

                    Html += string.Format(
                        "<tr>\n" +
                        "<td class=\"Right\"><a href=\"Invoice.aspx?JobRno={0}\" target=\"invoice\">{0}</a></td>\n" +
                        "<td>{1}</td>\n" +
                        "<td class=\"Center\">{2}</td>\n" +
                        "<td>{3}</td>\n" +
                        "<td class=\"Right\">{4}</td>\n" +
                        "<td class=\"Right\">{5}</td>\n" +
                        "<td class=\"Right\">{6}</td>\n" +
                        "<td class=\"Right\">{7}</td>\n" +
                        "<td class=\"Right\">{8}</td>\n" +
                        "</tr>\n",
                        DB.Int32(dr["JobRno"]),
                        DB.Str(dr["Customer"]),
                        Fmt.Dt(DB.DtTm(dr["JobDate"])),
                        DB.Str(dr["Location"]),
                        Fmt.Dollar(Subtotal),
                        Fmt.Dollar(OtherTotal),
                        Fmt.Dollar(SalesTaxTotal),
                        Fmt.Dollar(Total),
                        Fmt.Dollar(Balance));
                    RptTotal += Math.Round(Total, 2);

                    Html += CCPayments(DB.Int32(dr["JobRno"]));
                }
            }
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		return Html;
	}

	protected string CCPayments(Int32 JobRno)
	{
		string Html = string.Empty;
        string Sql = string.Format("Select * From Payments Where JobRno = {0} Order By Seq", JobRno);

		try
		{
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {

			string CCNum = DB.Str(dr["CCNum"]);
			string SecCode = DB.Str(dr["CCSecCode"]);

			try
			{
				CCNum = Crypt.Decrypt(CCNum, Misc.Password);
			}
			catch (Exception)
			{
			}

			try
			{
				SecCode = Crypt.Decrypt(SecCode, Misc.Password);
			}
			catch (Exception)
			{
			}

			if (CCNum.Length > 0)
			{
				string CardInfo = string.Format(
						"<dl>\n" +
						"<dt>Amount</dt><dd class=\"mid\">{1}</dd>\n" +
						"<dt>Card Type</dt><dd>{0}</dd>\n" +
						"</dl>",
						DB.Str(dr["CCType"]),
						Fmt.Dollar(DB.Dec(dr["Amount"])));
				Html += string.Format(
					"<tr><td colspan=\"11\" class=\"nil\" /></tr>\n" +
					"<tr>\n" +
					"<td colspan=\"2\" />\n" +
					"<td colspan=\"7\">{0}</td>\n" +
					"</tr>\n",
					CardInfo);
                }
            }

        }
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}

		return Html;
	}

	protected void InvoiceAmounts(DataRow dr)
	{
		Int32 JobRno = DB.Int32(dr["JobRno"]);
		Int32 EventCount = DB.Int32(dr["InvEventCount"]);
		decimal Price = DB.Dec(dr["InvPricePerPerson"]);
		decimal SubtotalTaxPct = DB.Dec(dr["SubTotTaxPct"]);
		bool fSalesTaxExempt = DB.Bool(dr["TaxExemptFlg"]);

		decimal Service = DB.Dec(dr["ServiceAmt"]);
		decimal Delivery = DB.Dec(dr["DeliveryAmt"]);
		decimal China = DB.Dec(dr["ChinaAmt"]);
		decimal AddService = DB.Dec(dr["AddServiceAmt"]);
		decimal FuelTravel = DB.Dec(dr["FuelTravelAmt"]);
		decimal Facility = DB.Dec(dr["FacilityAmt"]);
		decimal Gratuity = DB.Dec(dr["GratuityAmt"]);
		decimal Rentals = DB.Dec(dr["RentalsAmt"]);
		string RentalsDesc = DB.Str(dr["RentalsDesc"]);
		string Adj1Desc = DB.Str(dr["Adj1Desc"]);
		string Adj2Desc = DB.Str(dr["Adj2Desc"]);
		decimal Adj1Amt = DB.Dec(dr["Adj1Amt"]);
		decimal Adj2Amt = DB.Dec(dr["Adj2Amt"]);

		decimal ServiceTaxPct = DB.Dec(dr["ServiceTaxPct"]);
		decimal DeliveryTaxPct = DB.Dec(dr["DeliveryTaxPct"]);
		decimal ChinaTaxPct = DB.Dec(dr["ChinaTaxPct"]);
		decimal AddServiceTaxPct = DB.Dec(dr["AddServiceTaxPct"]);
		decimal FuelTravelTaxPct = DB.Dec(dr["FuelTravelTaxPct"]);
		decimal FacilityTaxPct = DB.Dec(dr["FacilityTaxPct"]);
		decimal GratuityTaxPct = DB.Dec(dr["GratuityTaxPct"]);
		decimal RentalsTaxPct = DB.Dec(dr["RentalsTaxPct"]);
		decimal Adj1TaxPct = DB.Dec(dr["Adj1TaxPct"]);
		decimal Adj2TaxPct = DB.Dec(dr["Adj2TaxPct"]);
		decimal CCFeeTaxPct = DB.Dec(dr["CCFeeTaxPct"]);
		decimal CCFeePct = DB.Dec(dr["CCFeePct"]);
		bool fCCFee = DB.Bool(dr["CCPmtFeeFlg"]);

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

		Subtotal = DB.Dec(dr["SubTotAmt"]);
		OtherTotal = DB.Dec(dr["PreTaxSubTotAmt"]) - Subtotal;
		SalesTaxTotal = DB.Dec(dr["SalesTaxTotAmt"]);
		Total = DB.Dec(dr["InvTotAmt"]);
		Balance = DB.Dec(dr["InvBalAmt"]);
	}
}
