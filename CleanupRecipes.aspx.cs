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

public partial class CleanupRecipes : System.Web.UI.Page
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
			Session["Menu"] = WebPage.Recipes.Title;
			//Setup();

			string Sql = string.Empty;
			try
			{
				// get list of recipes for menu items
				Sql = "Select Name, RecipeRno From Recipes Where IsNull(HideFlg, 0) = 0 Order By Name";
				DataTable dtRecipes = db.DataTable(Sql);

				// menu item with a recipe or marked As Is
				Sql =
					"Select MenuItemRno, Category, MenuItem " +
					"From mcJobMenuItems " +
					"Where RecipeRno Is Null " +
					"And IsNull(HideFlg, 0) = 0 " +
					"And IsNull(AsIsFlg, 0) = 0 " +
					"And MenuItem <> '' " +
					"And Category <> '' " +
					"And Category In (Select Category From mcJobMenuCategories Where IsNull(HideFlg, 0) = 0) " +
					"Order By MenuItem, Category";
				DataTable dt = db.DataTable(Sql);
				foreach (DataRow dr in dt.Rows)
				{
					int MenuItemRno = DB.Int32(dr["MenuItemRno"]);

					// category
					HtmlTableRow tr = new HtmlTableRow();
					HtmlTableCell td = new HtmlTableCell();
					td.InnerText = DB.Str(dr["Category"]);
					td.Attributes.Add("class", "category");
					tr.Controls.Add(td);

					// menu item
					td = new HtmlTableCell();
					td.Attributes.Add("class", "menu-item");
					td.Attributes.Add("data-name", DB.Str(dr["MenuItem"]));
					td.InnerText = DB.Str(dr["MenuItem"]);
					tr.Controls.Add(td);

					// Hide
					td = new HtmlTableCell();
					CheckBox chk = new CheckBox();
					chk.CssClass = "set-as-hidden";
					chk.ToolTip = "Hide this menu item";
					chk.Attributes.Add("data-item-menu-rno", MenuItemRno.ToString());
					td.Controls.Add(chk);
					tr.Controls.Add(td);

					// as is
					td = new HtmlTableCell();
					chk = new CheckBox();
					chk.CssClass = "mark-as-is";
					chk.ToolTip = "Set menu item As Is";
					chk.Attributes.Add("data-item-menu-rno", MenuItemRno.ToString());
					td.Controls.Add(chk);
					// edit menu item icon
					HtmlAnchor a = new HtmlAnchor();
					a.HRef = string.Format("SetupMenuItems.aspx?Rno={0}", MenuItemRno);
					a.Target = "Fix";
					HtmlGenericControl i = new HtmlGenericControl("i");
					i.Attributes.Add("class", "icon-edit");
					i.Attributes.Add("title", "Edit the menu item");
					a.Controls.Add(i);
					td.Controls.Add(a);
					tr.Controls.Add(td);

					// recipe
					td = new HtmlTableCell();
					HtmlGenericControl s = new  HtmlGenericControl("select");
					s.Attributes.Add("class", "select-recipe");
					s.Attributes.Add("data-item-menu-rno", MenuItemRno.ToString());
					HtmlGenericControl o = new HtmlGenericControl("option");
					o.InnerText = "Select Recipe";
					o.Attributes.Add("value", "0");
					o.Attributes.Add("style", "color: #888;");
					s.Controls.Add(o);
					foreach (DataRow drRecipe in dtRecipes.Rows)
					{
						o = new HtmlGenericControl("option");
						o.InnerText = DB.Str(drRecipe["Name"]);
						o.Attributes.Add("value", DB.Int32(drRecipe["RecipeRno"]).ToString());
						s.Controls.Add(o);
					}
					td.Controls.Add(s);
					a = new HtmlAnchor();
					a.HRef = "Recipes.aspx";
					a.Target = "Fix";
					i = new HtmlGenericControl("i");
					i.Attributes.Add("class", "icon-edit");
					i.Attributes.Add("title", "Create a recipe for this menu item.");
					a.Controls.Add(i);
					td.Controls.Add(a);
					td.Attributes.Add("class", "select");
					tr.Controls.Add(td);


					tblMenuItems.Rows.Add(tr);
				}
				divMenuItems.Visible = (dt.Rows.Count > 0);


				// subrecipe with mismatched units
				Sql =
					"Select r.RecipeRno, r.Name As RecipeName, sr.Name As SubrecipeName, x.SubrecipeRno, x.UnitRno, sr.YieldUnitRno, " +
					"(Select UnitSingle From Units Where UnitRno = x.UnitRno) As Unit, " +
					"(Select UnitSingle From Units Where UnitRno = sr.YieldUnitRno) As YieldUnit " +
					"From RecipeIngredXref x " +
					"Inner Join Recipes sr On sr.RecipeRno = x.SubrecipeRno " +
					"Inner Join Recipes r On r.RecipeRno = x.RecipeRno " +
					"Where SubrecipeRno Is Not Null " +
					"And x.UnitRno <> sr.YieldUnitRno " +
					"And IsNull(r.HideFlg, 0) = 0 " +
					"Order By r.Name, sr.Name";
				dt = db.DataTable(Sql);
				Ingred Ingred = new Ingred(0);
				int cProblems = 0;
				foreach (DataRow dr in dt.Rows)
				{
					decimal ConversionScaler = Ingred.ConversionScaler(DB.Int32(dr["UnitRno"]), DB.Int32(dr["YieldUnitRno"]));
					if (ConversionScaler == 0)
					{
						cProblems++;
						HtmlTableRow tr = new HtmlTableRow();
						HtmlTableCell td = new HtmlTableCell();
						HtmlAnchor a = new HtmlAnchor();
						a.HRef = string.Format("Recipes.aspx?Rno={0}", DB.Int32(dr["RecipeRno"]));
						a.InnerText = DB.Str(dr["RecipeName"]);
						a.Target = "Fix";
						td.Controls.Add(a);
						tr.Controls.Add(td);

						td = new HtmlTableCell();
						td.InnerText = DB.Str(dr["Unit"]); ;
						tr.Controls.Add(td);

						td = new HtmlTableCell();
						td.InnerText = DB.Str(dr["YieldUnit"]); ;
						tr.Controls.Add(td);

						td = new HtmlTableCell();
						td.InnerText = DB.Str(dr["SubrecipeName"]); ;
						tr.Controls.Add(td);

						tblSubrecipe.Rows.Add(tr);
					}
				}
				divSubrecipe.Visible = (cProblems > 0);


				// zero servings recipes
				Sql =
					"Select RecipeRno, Name From Recipes Where IsNull(NumServings, 0) = 0 " +
					"And IsNull(SubrecipeFlg, 0) = 0 " +
					"And IsNull(HideFlg, 0) = 0 " +
					"Order By Name";
				dt = db.DataTable(Sql);
				foreach (DataRow dr in dt.Rows)
				{
					HtmlTableRow tr = new HtmlTableRow();
					HtmlTableCell td = new HtmlTableCell();
					HtmlAnchor a = new HtmlAnchor();
					a.HRef = string.Format("Recipes.aspx?Rno={0}", DB.Int32(dr["RecipeRno"]));
					a.InnerText = DB.Str(dr["Name"]);
					a.Target = "Fix";
					td.Controls.Add(a);
					tr.Controls.Add(td);

					tblRecipes.Rows.Add(tr);
				}
				divRecipes.Visible = (dt.Rows.Count > 0);
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
	}
}