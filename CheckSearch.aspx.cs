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

public partial class CheckSearch : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;

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

	public string FindCheckNum()
	{
		string Html = string.Empty;
		string Sql = string.Empty;

		try
		{
            if (txtCheckNum.Text.Length > 0)
            {
                decimal RptTotal = 0;

                Sql = string.Format(
                    "Select p.JobRno, p.Seq, p.Reference, Coalesce(cu.Name, c.Name) as Customer, " +
                    "j.JobDate, j.SubTotAmt, j.PreTaxSubTotAmt, j.SalesTaxTotAmt, j.InvTotAmt, j.InvBalAmt " +
                    "From Payments p " +
                    "Inner Join mcJobs j on p.JobRno = j.JobRno " +
                    "Inner Join Contacts c on j.ContactRno = c.ContactRno " +
                    "Left Join Customers cu on c.CustomerRno = cu.CustomerRno " +
                    "Where Method = 'Check' And Reference Like '%{0}%' " +
                    "Order By p.JobRno, p.Seq",
                    txtCheckNum.Text);
                DataTable dt = db.DataTable(Sql);
                foreach (DataRow dr in dt.Rows)
                {
                    string Customer = DB.Str(dr["Customer"]);
                    string Ref = string.Format("Payment #{0} - {1}", DB.Int32(dr["Seq"]), DB.Str(dr["Reference"]));
                    decimal Subtotal = DB.Dec(dr["SubTotAmt"]);
                    decimal OtherTotal = DB.Dec(dr["PreTaxSubTotAmt"]) - Subtotal;
                    decimal SalesTaxTotal = DB.Dec(dr["SalesTaxTotAmt"]);
                    decimal Total = DB.Dec(dr["InvTotAmt"]);
                    decimal Balance = DB.Dec(dr["InvBalAmt"]);

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
                        "</tr>\n",
                        DB.Int32(dr["JobRno"]),
                        DB.Str(dr["Customer"]),
                        Fmt.Dt(DB.DtTm(dr["JobDate"])),
                        Ref,
                        Fmt.Dollar(Subtotal),
                        Fmt.Dollar(OtherTotal),
                        Fmt.Dollar(SalesTaxTotal),
                        Fmt.Dollar(Total));
                    RptTotal += Math.Round(Total, 2);

                    //Html += CCPayment(dr, "1");
                    //Html += CCPayment(dr, "2");
                    //Html += CCPayment(dr, "3");
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

	protected string CCPayment(DataRow dr, string PmtNum)
	{
		string Html = string.Empty;

		try
		{
			string CCNum = DB.Str(dr["CCNum" + PmtNum]);
			string SecCode = DB.Str(dr["CCSecCode" + PmtNum]);

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
						"<dt>Amount</dt><dd class=\"mid\">{7}</dd>\n" +
						"<dt>Card Type</dt><dd>{0}</dd>\n" +
						"<dt>Exp Date</dt><dd class=\"short\">{2}</dd>\n" +
						"<dt>Name</dt><dd class=\"long\">{4}</dd>\n" +
						"<dt></dt><dd class=\"mid\"></dd>\n" +
						"<dt>Card Num</dt><dd>{1}</dd>\n" +
						"<dt>Sec Code</dt><dd class=\"short\">{3}</dd>\n" +
						"<dt>Addr / Zip</dt><dd class=\"long\">{5} / {6}</dd>\n" +
						"</dl>",
						DB.Str(dr["CCType" + PmtNum]),
						CCNum,
						DB.Str(dr["CCExpDt" + PmtNum]),
						SecCode,
						DB.Str(dr["CCName" + PmtNum]),
						DB.Str(dr["CCAddr" + PmtNum]),
						DB.Str(dr["CCZip" + PmtNum]),
						Fmt.Dollar(DB.Dec(dr["PmtAmt" + PmtNum])));
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
}
