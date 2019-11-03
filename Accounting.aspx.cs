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

public partial class Accounting : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;


	protected decimal Subtotal = 0;
	protected decimal OtherTotal = 0;
	protected decimal SalesTaxTotal = 0;
	protected decimal Total = 0;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.Accounting.Title;
			//Setup();
		}
		else
		{
			DateTime Tm = DateTime.Now;

			// Recently edited finalized jobs
			int Count = Str.Num(Request.Params["ReCount"]);
			for (int i = 1; i <= Count; i++)
			{
				int JobRno = Str.Num(Request.Params["chkReProc" + i]);
				if (JobRno > 0)
				{
					//string Sql = "Select CCNum1, CCNum2, CCNum3 From mcJobs Where JobRno = " + JobRno;
					//DataRow dr = db.DataRow(Sql);
					//string CCNum1 = DecryptCCNum(dr, "1");
					//string CCNum2 = DecryptCCNum(dr, "2");
					//string CCNum3 = DecryptCCNum(dr, "3");
					//if (CCNum1.Length > 0)
					//{
					//	CCNum1 = "x" + Str.Right(CCNum1, 4);
					//}
					//if (CCNum2.Length > 0)
					//{
					//	CCNum2 = "x" + Str.Right(CCNum2, 4);
					//}
					//if (CCNum3.Length > 0)
					//{
					//	CCNum3 = "x" + Str.Right(CCNum3, 4);
					//}

					string Sql = "Update mcJobs Set " +
						"EditProcessedDtTm = Null, " +
						"EditProcessedUser = Null, " +
						//"CCSecCode1 = Null, CCSecCode2 = Null, CCSecCode3 = Null, " +
						//"CCNum1 = " + DB.PutStr(CCNum1) + ", " +
						//"CCNum2 = " + DB.PutStr(CCNum2) + ", " +
						//"CCNum3 = " + DB.PutStr(CCNum3) + ", " +
						"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
						"UpdatedUser = " + DB.PutStr(g.User) + " " +
						"Where JobRno = " + JobRno;
					db.Exec(Sql);
				}
			}

			// Finalized jobs
			Count = Str.Num(Request.Params["Count"]);
			for (int i = 1; i <= Count; i++)
			{
				int JobRno = Str.Num(Request.Params["chkProc" + i]);
				if (JobRno > 0)
				{
					//string Sql = "Select CCNum1, CCNum2, CCNum3 From mcJobs Where JobRno = " + JobRno;
					//DataRow dr = db.DataRow(Sql);
					//string CCNum1 = DecryptCCNum(dr, "1");
					//string CCNum2 = DecryptCCNum(dr, "2");
					//string CCNum3 = DecryptCCNum(dr, "3");
					//if (CCNum1.Length > 0)
					//{
					//	CCNum1 = "x" + Str.Right(CCNum1, 4);
					//}
					//if (CCNum2.Length > 0)
					//{
					//	CCNum2 = "x" + Str.Right(CCNum2, 4);
					//}
					//if (CCNum3.Length > 0)
					//{
					//	CCNum3 = "x" + Str.Right(CCNum3, 4);
					//}

					string Sql = "Update mcJobs Set " +
						"ProcessedDtTm = " + DB.PutDtTm(Tm) + ", " +
						"ProcessedUser = " + DB.PutStr(g.User) + ", " +
						//"CCSecCode1 = Null, CCSecCode2 = Null, CCSecCode3 = Null, " +
						//"CCNum1 = " + DB.PutStr(CCNum1) + ", " +
						//"CCNum2 = " + DB.PutStr(CCNum2) + ", " +
						//"CCNum3 = " + DB.PutStr(CCNum3) + ", " +
						"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
						"UpdatedUser = " + DB.PutStr(g.User) + " " +
						"Where JobRno = " + JobRno;
					db.Exec(Sql);
				}
			}
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


	private enum StatusType
	{
		RecentlyEdited,
		Finalized,
		NotFinalized
	}

	protected string RecentlyEditedFinalized()
	{
		return FinalizedJobs(StatusType.RecentlyEdited);
	}

	protected string Finalized()
	{
		return FinalizedJobs(StatusType.Finalized);
	}

	protected string NotFinalized()
	{
		return FinalizedJobs(StatusType.NotFinalized);
	}

	private string FinalizedJobs(StatusType Type)
	{
		string Html = string.Empty;
		string Sql = string.Empty;

		try
		{
			decimal RptTotal = 0;

			int iRow = 0;

			string sType = (Type == StatusType.RecentlyEdited ? "Re" : string.Empty);
			string Where = string.Empty;

			switch (Type)
			{
				case StatusType.RecentlyEdited:
					Where = "FinalDtTm Is Not Null And ProcessedDtTm Is Not Null And EditProcessedDtTm Is Not Null";
					break;
				case StatusType.Finalized:
					Where = "FinalDtTm Is Not Null And ProcessedDtTm Is Null";
					break;
				case StatusType.NotFinalized:
					Where = "FinalDtTm Is Null And ProcessedDtTm Is Null And InvCreatedDtTm Is Not Null";
					break;
			}

			Sql =
                "Select Coalesce(cu.Name, c.Name) as Customer, * " +
                "From mcJobs j " +
                "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " + 
                "Where " + Where + " Order By JobRno";

			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				InvoiceAmounts(dr);

				string Customer = DB.Str(dr["Customer"]);
				DateTime? Final = DB.DtTm(dr["FinalDtTm"]);
				DateTime? Processed = DB.DtTm(dr["ProcessedDtTm"]);
				DateTime? EditProcessed = DB.DtTm(dr["EditProcessedDtTm"]);
				DateTime? Updated = DB.DtTm(dr["InvUpdatedDtTm"]);

				if (Updated.HasValue && Updated.Value == DateTime.MinValue)
					Updated = null;

				bool fUpdated = false;
				string Title = string.Empty;
				if (Updated.HasValue)
				{
					DateTime dtUpdated = Updated.Value.AddMinutes(-5);
					switch (Type)
					{
						case StatusType.RecentlyEdited:
							fUpdated = (dtUpdated > EditProcessed);
							if (fUpdated)
							{
								Title = string.Format("Invoice was saved final on {0}\nand then updated again on {1}.", Fmt.DtTm(EditProcessed.Value), Fmt.DtTm(Updated.Value));
							}
							break;
						case StatusType.Finalized:
							fUpdated = (dtUpdated > Final);
							if (fUpdated)
							{
								Title = string.Format("Invoice was saved final on {0}\nand then updated again on {1}.", Fmt.DtTm(Final.Value), Fmt.DtTm(Updated.Value));
							}
							break;
					}
				}

                Int32 JobRno = DB.Int32(dr["JobRno"]);


                Html += string.Format(
					"<tr class=\"{11}\">\n" +
					"<td class=\"Center\"><input type=\"checkbox\" name=\"chk{1}Proc{0}\" value=\"{2}\" /></td>\n" +
					"<td class=\"Right\" title=\"{12}\">{10}<a href=\"Invoice.aspx?JobRno={2}\" target=\"invoice\">{2}</a></td>\n" +
					"<td>{3}</td>\n" +
					"<td class=\"Center\">{4}</td>\n" +
					"<td>{5}</td>\n" +
					"<td class=\"Right\">{6}</td>\n" +
					"<td class=\"Right\">{7}</td>\n" +
					"<td class=\"Right\">{8}</td>\n" +
					"<td class=\"Right\">{9}</td>\n" +
					"</tr>\n",
					++iRow,
					sType,
					JobRno,
					DB.Str(dr["Customer"]),
					Fmt.Dt(DB.DtTm(dr["JobDate"])),
					DB.Str(dr["Location"]),
					Fmt.Dollar(Subtotal),
					Fmt.Dollar(OtherTotal),
					Fmt.Dollar(SalesTaxTotal),
					Fmt.Dollar(Total),
					(fUpdated ? "*" : ""),
					(fUpdated ? "Updated" : ""),
					Title);
				RptTotal += Math.Round(Total, 2);

                string PmtSql = string.Format("Select * from Payments where JobRno = {0} Order by Seq", JobRno);
                try
                {
                    DataTable dtPmt = db.DataTable(PmtSql);
                    foreach (DataRow drPmt in dtPmt.Rows)
                    {
                        Html += CCPayment(drPmt);
                    }
                }
                catch (Exception Ex)
                {
                    Err Err = new Err(Ex, Sql);
                    Response.Write(Err.Html());
                }

    //            Html += CCPayment(dr, "1");
				//Html += CCPayment(dr, "2");
				//Html += CCPayment(dr, "3");
			}

			Html += string.Format(
				"<tr>\n" +
				"<td><input type=\"hidden\" name=\"{0}Count\" value=\"{1}\"/></td>\n" +
				"<td colspan=\"2\"><span style='color: Red;'>*</span><span style='font-size: 80%'>Updated</span></td>\n" +
				"<td colspan=\"2\"></td>\n" +
				"<td align=\"right\"><b>Count</b></td>\n" +
				"<td align=\"right\"><b>{2}</b></td>\n" +
				"<td align=\"right\"><b>Total</b></td>\n" +
				"<td align=\"right\"><b>{3}</b></td>\n" +
				"</tr>\n",
				sType,
				iRow,
				dt.Rows.Count,
				Fmt.Dollar(RptTotal));
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		return Html;
	}

	//protected string CCPayment(DataRow dr, string PmtNum)
    protected string CCPayment(DataRow dr)
    {
        string Html = string.Empty;

		try
		{
            string CCNum = DB.Str(dr["CCNum"]); // + PmtNum]);
            string SecCode = DB.Str(dr["CCSecCode"]);   // + PmtNum]);

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

		//if (fSalesTaxExempt)
		//{
		//	SubtotalTaxPct = 0;
		//}
		//Subtotal = EventCount * Price;
		//Subtotal = AdditionalPrices(JobRno, Subtotal, SubtotalTaxPct);

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

		//decimal CCFee1 = 0;
		//decimal CCFee2 = 0;
		//decimal CCFee3 = 0;

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

		//OtherTotal =
		//	Service +
		//	Delivery +
		//	China +
		//	AddService +
		//	FuelTravel +
		//	Facility +
		//	Gratuity +
		//	Rentals +
		//	Adj1Amt +
		//	Adj2Amt +
		//	CCFee1 +
		//	CCFee2 +
		//	CCFee3;

		//SalesTaxTotal =
		//	Subtotal * SubtotalTaxPct +
		//	Service * ServiceTaxPct +
		//	Delivery * DeliveryTaxPct +
		//	China * ChinaTaxPct +
		//	AddService * AddServiceTaxPct +
		//	FuelTravel * FuelTravelTaxPct +
		//	Facility * FacilityTaxPct +
		//	Gratuity * GratuityTaxPct +
		//	Rentals * RentalsTaxPct +
		//	Adj1Amt * Adj1TaxPct +
		//	Adj2Amt * Adj2TaxPct +
		//	CCFee1 * CCFeeTaxPct +
		//	CCFee2 * CCFeeTaxPct +
		//	CCFee3 * CCFeeTaxPct;
		//SalesTaxTotal /= 100;

		//if (fSalesTaxExempt)
		//{
		//	SalesTaxTotal = 0;
		//	CCFeeTaxPct = 0;
		//}

		//if (fCCFee && Balance > 0)
		//{
		//	//Response.Write("Job #" + JobRno.ToString() + " Balance " + Balance.ToString() + "<br/>");
		//	decimal CCFee = Math.Round(Balance * CCFeePct / 100, 2);
		//	decimal CCFeeTax = CCFee * CCFeeTaxPct / 100;
		//	OtherTotal += CCFee;
		//	SalesTaxTotal += CCFeeTax;
		//	Total += CCFee + CCFeeTax;
		//	Balance += CCFee + CCFeeTax;
		//}

		Subtotal = DB.Dec(dr["SubTotAmt"]);
		OtherTotal = DB.Dec(dr["PreTaxSubTotAmt"]) - Subtotal;
		SalesTaxTotal = DB.Dec(dr["SalesTaxTotAmt"]);
		Total = DB.Dec(dr["InvTotAmt"]);
		decimal Balance = DB.Dec(dr["InvBalAmt"]);
	}

//	private decimal AdditionalPrices(int JobRno, decimal Subtotal, decimal SubTotTaxPct)
//	{
//		DataTable dt = db.DataTable("Select * From JobInvoicePrices Where JobRno = " + JobRno + " Order By Seq");
//		if (dt.Rows.Count > 0)
//		{
//			Subtotal = 0;
//		}

//		foreach (DataRow drPrice in dt.Rows)
//		{
//			string PriceType = DB.Str(drPrice["PriceType"]);
//			int Count = 0;
//			decimal Price = 0;
//			decimal ExtPrice = 0;
//			//string Desc = "";
//			//string ExtDesc = "";

//			switch (PriceType)
//			{
//				case Misc.cnPerPerson:
//					Count = DB.Int32(drPrice["InvPerPersonCount"]);
//					Price = DB.Dec(drPrice["InvPerPersonPrice"]);
//					ExtPrice = Count * Price;
//					//Desc = DB.Str(drPrice["InvPerPersonDesc"]);
//					//ExtDesc = string.Format("{0} {1} @ {2} per person",
//					//    Fmt.Num(Count),
//					//    Desc,
//					//    Fmt.Dollar(Price));
//					break;

//				case Misc.cnPerItem:
//					Count = DB.Int32(drPrice["InvPerItemCount"]);
//					Price = DB.Dec(drPrice["InvPerItemPrice"]);
//					ExtPrice = Count * Price;
//					//Desc = DB.Str(drPrice["InvPerItemDesc"]);
//					//ExtDesc = string.Format("{0} {1} @ {2} per item",
//					//    Fmt.Num(Count),
//					//    Desc,
//					//    Fmt.Dollar(Price));
//					break;

//				case Misc.cnAllIncl:
//					Count = 1;
//					Price = DB.Dec(drPrice["InvAllInclPrice"]);
//					ExtPrice = Price - (Price * (1 - 1 / (1 + SubTotTaxPct / 100)));
//					//Desc = DB.Str(drPrice["InvAllInclDesc"]);
//					//ExtDesc = string.Format("All inclusive {0} @ {1}",
//					//    Desc,
//					//    Fmt.Dollar(Price));
//					break;

//				default:
//					break;
//			}

//			Subtotal += ExtPrice;
//		}
		
//		return Subtotal;
//	}
}
