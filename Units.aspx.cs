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
using System.Text;
using System.IO;

// Copyright(c) 2005-2019 PeteSoft, LLC. All Rights Reserved 

public partial class Units : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";

	private bool fStartHere = false;
	private string StartHere;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.Recipes.Title;
			Setup();
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
	}

	private void Setup()
	{
		LoadList();
	}

	protected void ClearData()
	{
		hfRno.Value =
		txtSingle.Text =
		txtPlural.Text =
		txtCreatedDt.Text =
		txtCreatedUser.Text =
		txtUpdatedDt.Text =
		txtUpdatedUser.Text = string.Empty;
        chkHide.Checked = false;
        btnDelete.Enabled = false;
		btnDelete.ToolTip = "New unit record";
	}

	protected void GetData(int Rno)
	{
		string Sql = string.Format(
			"Select * " +
			"From Units " +
			"Where UnitRno = {0}",
			Rno);

		ClearData();

		try
		{
			DataRow dr = db.DataRow(Sql);
			if (dr != null)
			{
				hfRno.Value = Rno.ToString();
				txtSingle.Text = DB.Str(dr["UnitSingle"]);
				txtPlural.Text = DB.Str(dr["UnitPlural"]);
                chkHide.Checked = DB.Bool(dr["HideFlg"]);
                txtCreatedDt.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
				txtCreatedUser.Text = DB.Str(dr["CreatedUser"]);
				txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
				txtUpdatedUser.Text = DB.Str(dr["UpdatedUser"]);

				Sql = string.Format("Select Count(*) From RecipeIngredXref Where UnitRno = {0}", Rno);
                int NumRecipes = db.SqlNum(Sql);
                int Count = NumRecipes;
                if (NumRecipes > 0)
                {
                    Sql = string.Format("Select Name, RecipeRno From Recipes Where RecipeRno In (Select RecipeRno From RecipeIngredXref Where UnitRno = {0}) Order By Name", Rno);
                    DataTable dtRecipes = db.DataTable(Sql);
                    foreach (DataRow drRecipe in dtRecipes.Rows)
                    {
                        HtmlGenericControl li = new HtmlGenericControl("li");
                        HtmlAnchor a = new HtmlAnchor();
                        a.InnerText = DB.Str(drRecipe["Name"]);
                        a.HRef = string.Format("Recipes.aspx?Rno={0}", DB.Int32(drRecipe["RecipeRno"]));
                        a.Target = "UnitUsed";
                        li.Controls.Add(a);
                        ulRecipes.Controls.Add(li);
                    }
                }

				//Sql = string.Format("Select Count(*) From VendorIngredXref Where PurchaseUnitRno = {0}", Rno);
    //            int NumIngreds = db.SqlNum(Sql);
    //            Count += NumIngreds;
                //if (NumIngreds > 0)
                //{
                //    Sql = string.Format("Select Name From Ingredients Where IngredRno In (Select IngredRno From VendorIngredXref Where PurchaseUnitRno = {0}) Order By Name", Rno);
                //    DataTable dtIngreds = db.DataTable(Sql);
                //    foreach (DataRow drIngred in dtIngreds.Rows)
                //    {
                //        HtmlGenericControl li = new HtmlGenericControl("li");
                //        HtmlAnchor a = new HtmlAnchor();
                //        a.InnerText = DB.Str(drIngred["Name"]);
                //        a.HRef = string.Format("Recipes.aspx?Rno={0}", DB.Int32(drRecipe["RecipeRno"]));
                //        a.Target = "UnitUsed";
                //        li.Controls.Add(a);
                //        ulVendorIngred.Controls.Add(li);

                //        HtmlGenericControl li = new HtmlGenericControl("li");
                //        HtmlAnchor a = new HtmlAnchor();
                //        a.InnerText = DB.Str(drRecipe["Name"]);
                //        a.HRef = string.Format("Recipes.aspx?Rno={0}", DB.Int32(drRecipe["RecipeRno"]));
                //        a.Target = "UnitUsed";
                //        li.Controls.Add(a);
                //        ulRecipes.Controls.Add(li);

                //    }
                //}

                Sql = string.Format("Select Count(*) From IngredConv Where PurchaseUnitRno = {0} Or RecipeUnitRno = {0}", Rno);
                int NumIngreds = db.SqlNum(Sql);
                Count += NumIngreds;
                if (NumIngreds > 0)
                {
                    Sql = string.Format("Select Name, IngredRno From Ingredients Where IngredRno In (Select IngredRno From IngredConv Where PurchaseUnitRno = {0} Or RecipeUnitRno = {0}) Order By Name", Rno);
                    DataTable dtIngreds = db.DataTable(Sql);
                    foreach (DataRow drIngred in dtIngreds.Rows)
                    {
                        HtmlGenericControl li = new HtmlGenericControl("li");
                        HtmlAnchor a = new HtmlAnchor();
                        a.InnerText = DB.Str(drIngred["Name"]);
                        a.HRef = string.Format("Ingredients.aspx?Rno={0}", DB.Int32(drIngred["IngredRno"]));
                        a.Target = "UnitUsed";
                        li.Controls.Add(a);
                        ulIngredConv.Controls.Add(li);
                    }
                }

				if (Count > 0)
				{
					btnDelete.Enabled = false;
					btnDelete.ToolTip = string.Format("Units for {0} recipe ingredients & ingredient conversions", Count);
				}
				else
				{
					btnDelete.Enabled = true;
					btnDelete.ToolTip = "Not used by any recipe ingredients & ingredient conversions";
				}

				FocusField = "txtSingle";

				Sql = string.Format(
					"Select c.*, up.UnitSingle As PurchaseUnitSingle, up.UnitPlural As PurchaseUnitPlural, " +
					"ur.UnitSingle As RecipeUnitSingle, ur.UnitPlural As RecipeUnitPlural " +
					"From IngredConv c " +  
					"Inner Join Units up On c.PurchaseUnitRno = up.UnitRno " + 
					"Inner Join Units ur On c.RecipeUnitRno = ur.UnitRno " + 
					"Where IngredRno = 0 And (PurchaseUnitRno = {0} Or RecipeUnitRno = {0}) Order By PurchaseQty Desc, RecipeQty", Rno);
				DataTable dt = db.DataTable(Sql);
				foreach (DataRow drConv in dt.Rows)
				{
					decimal PurchaseQty = DB.Dec(drConv["PurchaseQty"]);
					decimal RecipeQty = DB.Dec(drConv["RecipeQty"]);
					TableRow tr = new TableRow();
					TableCell td;

					td = new TableCell();
					td.Text = Str.ShowFract(RecipeQty);
					td.CssClass = "Qty";
					tr.Controls.Add(td);

					td = new TableCell();
					td.Text = DB.Str(drConv["Recipe" + (RecipeQty <= 1 ? "UnitSingle" : "UnitPlural")]);
					if (DB.Int32(drConv["RecipeUnitRno"]) == Rno)
					{
						td.CssClass = "Unit";
					}
					tr.Controls.Add(td);

					td = new TableCell();
					td.Text = Str.ShowFract(PurchaseQty);
					td.CssClass = "Qty";
					tr.Controls.Add(td);

					td = new TableCell();
					td.Text = DB.Str(drConv["Purchase" + (PurchaseQty <= 1 ? "UnitSingle" : "UnitPlural")]);
					if (DB.Int32(drConv["PurchaseUnitRno"]) == Rno)
					{
						td.CssClass = "Unit";
					}
					tr.Controls.Add(td);

					tblConv.Controls.Add(tr);
				}
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
			if (Rno == 0)
			{
				//Response.Write("New<br/>");
				Sql = string.Format(
					"Insert Into Units (UnitSingle, CreatedDtTm, CreatedUser) Values ({0}, GetDate(), {1}); " +
					"Select Scope_Identity()",
					DB.PutStr(txtSingle.Text, 20),
					DB.PutStr(g.User));
				Rno = db.SqlNum(Sql);
			}

			Sql = string.Format(
				"Update Units Set " +
				"UnitSingle = {1}, " +
				"UnitPlural = {2}, " +
                "HideFlg = {3}, " +
                "UpdatedDtTm = GetDate(), " +
				"UpdatedUser = {3} " +
				"Where UnitRno = {0}",
				Rno,
				DB.PutStr(txtSingle.Text, 20),
				DB.PutStr(txtPlural.Text, 20),
                DB.PutBool(chkHide.Checked),
                DB.PutStr(g.User));
			db.Exec(Sql);

			fStartHere = true;
			StartHere = Rno.ToString();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}
	}

	private void LoadList()
	{
		LoadList(false);
	}

	private void LoadList(bool fNext)
	{
		string Sql =
			"Select UnitRno, UnitSingle, UnitPlural, HideFlg " +
			"From Units ";
        string SqlWhere = (chkShowHidden.Checked ? string.Empty : "IsNull(HideFlg, 0) = 0");
        Sql += DB.Where(SqlWhere);
        Sql += " Order by UnitSingle";

		string CurrItem = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : "");
		string NextCurrItem = (lstList.SelectedIndex >= 0 && lstList.SelectedIndex + 1 < lstList.Items.Count ? lstList.Items[lstList.SelectedIndex + 1].Value : string.Empty);
		if (!fNext && CurrItem.Length > 0)
		{
			NextCurrItem = CurrItem;
		}

		//Response.Write("fStartHere " + fStartHere + "<br/>");
		// if a place to start
		if (!fNext && fStartHere)
		{
			NextCurrItem = StartHere;
			//Response.Write("Start Here [" + StartHere + "]<br/>");
		}

		//if (lstList.SelectedIndex >= 0) Response.Write("Curr " + lstList.SelectedItem.Value + "<br/>");
		lstList.Items.Clear();

		//Response.Write(Sql + "<br/>");
		LoadListSql(Sql, NextCurrItem);

		lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Units";

		//btnUpdate.Enabled = (CurrItem.Length > 0);
	}

	private void LoadListSql(string Sql, string NextCurrItem)
	{
		//Response.Write("NextCurr [" + NextCurrItem + "]<br/>");
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				Int32 Rno = DB.Int32(dr["UnitRno"]);
				String Unit = DB.Str(dr["UnitSingle"]);
				ListItem Item = new ListItem(Unit, Rno.ToString());

				if (Item.Value == NextCurrItem)
				{
					Item.Selected = true;

					Rno = Str.Num(Item.Value);
					GetData(Rno);
					btnUpdate.Enabled =
					btnNext.Enabled = true;
				}

				lstList.Items.Add(Item);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void UpdateList(object sender, System.EventArgs e)
	{
		LoadList();
	}

	protected void Update()
	{
		Update(false);
	}

	protected void Update(bool fNext)
	{
		SaveData();

		//ClearData();
		LoadList(fNext);
	}

	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		int Rno = Str.Num(lstList.SelectedItem.Value);
		//GetData(Rno);
		fStartHere = true;
		StartHere = lstList.SelectedItem.Value;
		LoadList();
		btnUpdate.Enabled =
		btnNext.Enabled = true;
	}

	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		SaveData();
		ClearData();
		LoadList();

		//Update(false);
	}

	protected void btnUpdateNext_Click(object sender, EventArgs e)
	{
		SaveData();
		ClearData();
		LoadList(true);
	}

	protected void btnNew_Click(object sender, System.EventArgs e)
	{
		ClearData();
	}

	protected void btnDelete_Click(object sender, EventArgs e)
	{
		int Rno = Str.Num(hfRno.Value);
		string Sql = String.Format(
			"Delete From Units Where UnitRno = {0}",
			Rno);

		try
		{
			db.Exec(Sql);
			LoadList(true);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}
}

