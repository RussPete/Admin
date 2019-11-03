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

public partial class CleanupShopping : System.Web.UI.Page
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
			Session["Menu"] = WebPage.Shopping.Title;
			//Setup();

			string Sql = string.Empty;
			try
			{
				// ingredient without a receipt price
				Sql =
					"Select IngredRno, Name From Ingredients " +
					"Where IsNull(HideFlg, 0) = 0 " +
					"And IsNull(NonPurchaseFlg, 0) = 0 " +
					"And IngredRno Not In (Select Distinct IngredRno From PurchaseDetails Where IngredRno Is Not Null) " +
					"And IngredRno In (Select IngredRno From RecipeIngredXref) " +
					"Order By Name";
				DataTable dt = db.DataTable(Sql);
				foreach (DataRow dr in dt.Rows)
				{
					int IngredRno = DB.Int32(dr["IngredRno"]);

					HtmlTableRow tr = new HtmlTableRow();
					HtmlTableCell td = new HtmlTableCell();
					td.InnerText = DB.Str(dr["Name"]);
					tr.Controls.Add(td);

					td = new HtmlTableCell();
					HtmlAnchor a = new HtmlAnchor();
					a.HRef = string.Format("Purchases.aspx?IngredRno={0}", DB.Int32(dr["IngredRno"]));
					a.InnerText = "Enter Receipt";
					a.Attributes.Add("title", "Go to the Receipt page to enter a price.");
					a.Target = "Fix";
					td.Controls.Add(a);
					td.Attributes.Add("class", "select");
					tr.Controls.Add(td);

					// non-purchase
					td = new HtmlTableCell();
					CheckBox chk = new CheckBox();
					chk.CssClass = "set-non-purchase";
					chk.ToolTip = "Set Ingredient as Non-Purchased";
					chk.Attributes.Add("data-ingredient-rno", IngredRno.ToString());
					td.Controls.Add(chk);
					// edit ingredient icon
					a = new HtmlAnchor();
					a.HRef = string.Format("Ingredients.aspx?Rno={0}", IngredRno);
					a.Target = "Fix";
					HtmlGenericControl i = new HtmlGenericControl("i");
					i.Attributes.Add("class", "icon-edit");
					i.Attributes.Add("title", "Edit this ingredient");
					a.Controls.Add(i);
					td.Controls.Add(a);
					tr.Controls.Add(td);
					tblIngreds.Rows.Add(tr);
				}
				divIngreds.Visible = (dt.Rows.Count > 0);


				// ingredient purchase units
				Sql =
					"Select i.IngredRno, i.Name, x.UnitRno, d.PurchaseUnitRno, " +
					"(Select UnitSingle From Units Where UnitRno = x.UnitRno) As Unit, " +
					"(Select UnitSingle From Units Where UnitRno = d.PurchaseUnitRno) As PurchaseUnit " +
					"From RecipeIngredXref x " +
					"Inner Join PurchaseDetails d On d.IngredRno = x.IngredRno And d.PurchaseRno = (Select Max(PurchaseRno) From PurchaseDetails Where IngredRno = x.IngredRno) " +
					"Inner Join Recipes r On r.RecipeRno = x.RecipeRno " +
					"Inner Join Ingredients i On i.IngredRno = x.IngredRno " +
					"Where x.UnitRno <> d.PurchaseUnitRno " +
					"And IsNull(r.HideFlg, 0) = 0 " +
					"And IsNull(i.MenuItemAsIsFlg, 0) = 0 " +
					"Group By i.Name, x.UnitRno, i.IngredRno, d.PurchaseUnitRno " +
					"Order By i.Name";
				dt = db.DataTable(Sql);
                Ingred Ingred = new Ingred(0);
                int cProblems = 0;
				foreach (DataRow dr in dt.Rows)
				{
					int IngredRno = DB.Int32(dr["IngredRno"]);
					Ingred = new Ingred(IngredRno);
					decimal ConversionScaler = Ingred.ConversionScaler(DB.Int32(dr["UnitRno"]), DB.Int32(dr["PurchaseUnitRno"]));
					if (ConversionScaler == 0)
					{
						cProblems++;
						HtmlTableRow tr = new HtmlTableRow();
						HtmlTableCell td = new HtmlTableCell();
						HtmlAnchor a = new HtmlAnchor();
						a.HRef = string.Format("Ingredients.aspx?Rno={0}", IngredRno);
						a.InnerText = DB.Str(dr["Name"]);
						a.Target = "Fix";
						td.Controls.Add(a);
						tr.Controls.Add(td);

						td = new HtmlTableCell();
						td.InnerText = DB.Str(dr["Unit"]); ;
						tr.Controls.Add(td);

						td = new HtmlTableCell();
						td.InnerText = DB.Str(dr["PurchaseUnit"]); ;
						tr.Controls.Add(td);

						tblPurchaseUnits.Rows.Add(tr);
					}
				}
				divPurchaseUnits.Visible = (cProblems > 0);
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
	}
}