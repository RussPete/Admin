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

public partial class RecipeScale : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;

	protected int RecipeRno;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.Recipes.Title;
		}

		RecipeRno = Parm.Int("Rno");

		Scale();
	}

	private void Scale()
	{
		string Sql = string.Empty;
		try
		{
			Sql = string.Format(
				"Select r.*, " +
				"(Select UnitSingle From Units Where UnitRno = r.YieldUnitRno) As YieldUnitSingle, " +
				"(Select UnitPlural From Units Where UnitRno = r.YieldUnitRno) As YieldUnitPlural, " +
				"(Select UnitSingle From Units Where UnitRno = r.PortionUnitRno) As PortionUnitSingle, " +
				"(Select UnitPlural From Units Where UnitRno = r.PortionUnitRno) As PortionUnitPlural, " +
				"(Select Top 1 BaseCostPct From Settings) As DefaultBaseCostPct " +
				"From Recipes r Where RecipeRno = {0}", 
				RecipeRno);
			DataRow drRecipe = db.DataRow(Sql);
			if (drRecipe != null)
			{
				decimal RecipeServings		= DB.Dec(drRecipe["NumServings"]);
				decimal ScaleServings		= Str.Dec(txtServings.Text);
				decimal Multiplier			= Str.Dec(txtMultiplier.Text);
				if (ScaleServings == 0)
				{
					ScaleServings = RecipeServings;
				}
				if (Multiplier == 0)
				{
					Multiplier = 1;
				}

				decimal Scaler				= (RecipeServings != 0 ? ScaleServings / RecipeServings : Multiplier);
				decimal Yield				= DB.Dec(drRecipe["YieldQty"]);
				decimal ServingSize			= DB.Dec(drRecipe["PortionQty"]);
				decimal BaseCostPrice		= DB.Dec(drRecipe["BaseCostPrice"]);
				decimal BaseCostPct			= DB.Dec(drRecipe["BaseCostPct"]);
				bool fUseDefaultBaseCostPct	= (drRecipe["BaseCostPct"] == DBNull.Value);
				decimal DefaultBaseCostPct	= DB.Dec(drRecipe["DefaultBaseCostPct"]);
				if (fUseDefaultBaseCostPct)
				{
					BaseCostPct				= DefaultBaseCostPct;
				}

				lblPrmRecipeName.Text	=
				lblRecipeName.Text		= DB.Str(drRecipe["Name"]);
				txtServings.Text		= Fmt.Num(Scaler * RecipeServings, 3);
				txtMultiplier.Text		= Fmt.Num(Scaler, 3);
				hfOrigServings.Value	= RecipeServings.ToString();
				tdServings.Text			= Str.ShowFract(Scaler * RecipeServings);
				tdPrmYield.Text			= 
				tdYield.Text			= Str.ShowFract(Scaler * Yield);
				tdPrmYieldUnit.Text		=
				tdYieldUnit.Text		= (Scaler * Yield <= 1 ? DB.Str(drRecipe["YieldUnitSingle"]) : DB.Str(drRecipe["YieldUnitPlural"]));
				tdPrmSize.Text			=
				tdSize.Text				= Str.ShowFract(ServingSize);
				tdPrmSizeUnit.Text		=
				tdSizeUnit.Text			= (ServingSize <= 1 ? DB.Str(drRecipe["PortionUnitSingle"]) : DB.Str(drRecipe["PortionUnitPlural"]));
				tdPrmServingCost.Text	= 
				tdServingCost.Text		= Fmt.Dollar(RecipeServings != 0 ? BaseCostPrice / RecipeServings : 0);
				tdPrmRecipeCost.Text	=
				tdRecipeCost.Text		= Fmt.Dollar(Scaler * BaseCostPrice);
				tdPrmServingPrice.Text	=
				tdServingPrice.Text		= Fmt.Dollar(RecipeServings != 0 ? BaseCostPrice / RecipeServings : 0);
				tdPrmServingPrice.Text	= 
				tdServingPrice.Text		= Fmt.Dollar(RecipeServings != 0 ? BaseCostPrice / RecipeServings / (BaseCostPct / 100) : 0);
				tdPrmRecipePrice.Text	= 
				tdRecipePrice.Text		= Fmt.Dollar(Scaler * BaseCostPrice / (BaseCostPct / 100));

				ltlDirections.Text		= DB.Str(drRecipe["Instructions"]);	liDirections.Visible	= (ltlDirections.Text.Length > 0);
				ltlNotes.Text			= DB.Str(drRecipe["IntNote"]);		liNotes.Visible			= (ltlNotes.Text.Length > 0);
				ltlSource.Text			= DB.Str(drRecipe["Source"]);		liSource.Visible		= (ltlSource.Text.Length > 0);
					
				Sql = string.Format(
					"Select x.*, IsNull(i.Name, sr.Name) as Name, " +
					"(Select UnitSingle From Units Where UnitRno = x.UnitRno) As UnitSingle, " +
					"(Select UnitPlural From Units Where UnitRno = x.UnitRno) As UnitPlural " +
					"From RecipeIngredXref x Left Join Ingredients i On i.IngredRno = x.IngredRno " + 
					"Left Join Recipes sr on sr.RecipeRno = x.SubrecipeRno " +
					"Where x.RecipeRno = {0} " +
					"Order By RecipeSeq", 
					RecipeRno);
				DataTable dt = db.DataTable(Sql);
				foreach (DataRow dr in dt.Rows)
				{
					string Title = DB.Str(dr["Title"]);
					decimal Qty = Scaler * DB.Dec(dr["UnitQty"]);
					TableRow tr = new TableRow();
					if (Title.Length > 0)
					{
						tr.Cells.Add(new TableCell() { Text = string.Format("<span class='Title'>{0}</span>", Title), ColumnSpan = 4 });
					}
					else
					{
						tr.Cells.Add(new TableCell() { Text = Str.ShowFract(Qty) });
						tr.Cells.Add(new TableCell() { Text = DB.Str(dr["Unit" + (Qty <= 1 ? "Single" : "Plural")]) });
						string note = DB.Str(dr["Note"]);
						tr.Cells.Add(new TableCell() { Text = DB.Str(dr["Name"]) + (note.Length > 0 ? " - " + note : "")});
					}
					tblIngredients.Rows.Add(tr);
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
