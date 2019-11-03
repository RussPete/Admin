using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Web;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Utils;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class MenuItemQuotes : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	
	protected void Page_Load(object sender, EventArgs e)
    {
		db = new DB();

        Session["Menu"] = WebPage.MenuSetup.Title;
        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			tblRpt.Visible = false;

			string Sql = "Select QuoteDiffPct From Settings";
			try
			{
				decimal DiffPct = db.SqlDec(Sql);
				if (DiffPct == 0)
				{
					DiffPct = 15;
				}

				txtDiffPct.Text = Fmt.Num(DiffPct, 0);
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
		else
		{
			tblRpt.Visible = true;
			fields.Visible = false;
			ProcessReport();
		}
	}

	private void ProcessReport()
	{
		decimal DiffPct = Str.Dec(txtDiffPct.Text);

		string Sql = "Update Settings Set QuoteDiffPct = " + DiffPct.ToString();
		try 
		{
			db.Exec(Sql);

			Sql =
				"Select m.MenuItemRno, m.Category, m.MenuItem, m.ServingQuote, m.ServingPrice, m.InaccuratePriceFlg " +
				"From mcJobMenuItems m " +
				"Inner Join mcJobMenuCategories c On m.Category = c.Category " +
				"Where IsNull(m.HideFlg, 0) = 0 " +
				"And IsNull(c.HideFlg, 0) = 0 " +
				"And (IsNull(m.ServingQuote, 0) = 0 Or IsNull(m.ServingPrice, 0) <> 0) " +
				"And MenuItem <> '' " +
				"Order By m.Category, m.MenuItem ";

			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				decimal Quote = Math.Round(DB.Dec(dr["ServingQuote"]), 2);
				decimal Price = Math.Round(DB.Dec(dr["ServingPrice"]), 2);
				decimal Diff = (Price != 0 ? (Quote - Price) / Price * 100M : 0);

				if (Quote == 0 || Diff > DiffPct || -Diff > DiffPct)
				{
					HtmlTableRow tr = new HtmlTableRow();

					HtmlTableCell td = new HtmlTableCell();
					td.InnerText = DB.Str(dr["Category"]);
					tr.Controls.Add(td);

					td = new HtmlTableCell();
					HtmlAnchor a = new HtmlAnchor();
					a.InnerText = DB.Str(dr["MenuItem"]);
					a.HRef = string.Format("SetupMenuItems.aspx?Rno={0}", DB.Int32(dr["MenuItemRno"]));
					a.Target = "Fix";
					td.Controls.Add(a);
					tr.Controls.Add(td);

					td = new HtmlTableCell();
					td.InnerText = Fmt.Dollar(DB.Dec(dr["ServingQuote"]));
					td.Attributes.Add("class", "amt quote");
					tr.Controls.Add(td);

					td = new HtmlTableCell();
					td.InnerText = Fmt.Dollar(DB.Dec(dr["ServingPrice"]));
					td.Attributes.Add("class", "amt price");
					tr.Controls.Add(td);

					td = new HtmlTableCell();
					if (DB.Bool(dr["InaccuratePriceFlg"]))
					{
						HtmlGenericControl i = new HtmlGenericControl("i");
						i.Attributes.Add("class", "icon-dollar");
						i.Attributes.Add("title", "Cost and Price are inaccurate because there are ingredients with no price from receipts.");
						td.Controls.Add(i);
					}
					tr.Controls.Add(td);

					td = new HtmlTableCell();
					td.InnerText = string.Format("{0:0}% {1}", Math.Abs(Diff), (Diff < 0 ? "under" : "over"));
					tr.Controls.Add(td);

					tblRpt.Rows.Add(tr);
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
