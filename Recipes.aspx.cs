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

public partial class Recipes : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = string.Empty;
	protected Int32 RecipeRno;
	protected int cMenuItems;
	protected int cIngredients;

	private bool fStartHere = false;
	private int StartHere;

	private bool fRecalcRecipePrice;

	protected string ErrMsg = string.Empty;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        RecipeRno = Parm.Int("Rno");
		int Rno = Str.Num(hfRecipeRno.Value);
		if (Rno != 0)
		{
			RecipeRno = Rno;
		}
		//cItems = Str.Num(hf txtcItems.Text);
		//AddLines();

		//pnlNewIngred.Visible = false;

		// Put user code to initialize the page here
		if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.Recipes.Title;
            Setup();
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
		hfNumMenuItems.Value =
		hfOrigNumMenuItems.Value = cMenuItems.ToString();
		hfNumIngredients.Value =
		hfOrigNumIngredients.Value = cIngredients.ToString();
	}

	private void Setup()
    {
		string Sql = "Select Top 1 BaseCostPct From Settings As DefaultBaseCostPct";
		try
		{
			string DefaultBaseCostPct = Fmt.Num(db.SqlDec(Sql), 4);
			lblBaseCostPct.Text = DefaultBaseCostPct;
			chkUseDefaultBaseCostPct.Text = string.Format("Use {0}% default", DefaultBaseCostPct);
			chkUseDefaultBaseCostPct.Checked = true;			

			LoadList();

			if (RecipeRno > 0)
			{
				GetData(RecipeRno);
			}
			else
			{
				LoadMenuItems();
				LoadIngredients();
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
		RecipeRno = 0;
		ddlType.SelectedIndex = -1;
		txtName.Text =
		txtInstructions.Text =
		txtNumServings.Text =
		//txtMenRatio.Text =
		//txtWomenRatio.Text =
		//txtChildRatio.Text =
		txtYield.Text =
		hfYieldUnitRno.Value =
		txtYieldUnit.Text =
		txtPortion.Text =
		hfPortionUnitRno.Value =
		txtPortionUnit.Text =
		txtPortionPrice.Text =
		txtRecipePrice.Text = 
		txtBaseCostPct.Text =
		txtBaseCostPrice.Text =
		txtRecipeBaseCostPrice.Text = 
        txtCategory.Text = 
		txtSource.Text =
		txtNote.Text =
		txtCreatedDt.Text =
		txtCreatedUser.Text =
		txtUpdatedDt.Text =
		txtUpdatedUser.Text = string.Empty;
		chkHide.Checked =
		chkGlutenFree.Checked = 
        chkVegan.Checked =
        chkVegetarian.Checked =
        chkDairyFree.Checked =
        chkNuts.Checked =
        chkIncludeInBook.Checked = false;
        hfRecipeRno.Value =
		hfNumIngredients.Value =
		hfOrigNumIngredients.Value = string.Empty;
	}

	protected void GetData(int Rno)
	{
		string Sql = string.Format(
			"Select *, " +
			"(Select UnitSingle From Units Where UnitRno = r.YieldUnitRno) As YieldUnitSingle, " +
			"(Select UnitPlural From Units Where UnitRno = r.YieldUnitRno) As YieldUnitPlural, " +
			"(Select UnitSingle From Units Where UnitRno = r.PortionUnitRno) As PortionUnitSingle, " +
			"(Select UnitPlural From Units Where UnitRno = r.PortionUnitRno) As PortionUnitPlural, " +
			"(Select Top 1 BaseCostPct From Settings) As DefaultBaseCostPct " +
			"From Recipes r " +
			"Where r.RecipeRno = {0}", 
			Rno);

		ClearData();

		try
		{
			DataRow dr = db.DataRow(Sql);
			if (dr != null)
			{
				bool fSubrecipe                     = DB.Bool(dr["SubrecipeFlg"]);
				RecipeRno                           = DB.Int32(dr["RecipeRno"]);
				lblRno.Text                         =
				hfRecipeRno.Value                   = RecipeRno.ToString();
				txtName.Text                        = DB.Str(dr["Name"]);
				chkHide.Checked                     = DB.Bool(dr["HideFlg"]);
				ddlType.Text                        = (fSubrecipe ? "Subrecipe" : "Menu Item");
				txtInstructions.Text                = DB.Str(dr["Instructions"]);
				int NumServings                     = DB.Int32(dr["NumServings"]);
				txtNumServings.Text                 = 
				hfOrigNumServings.Value             = Fmt.Num(NumServings);
				//txtMenRatio.Text                  = Fmt.Num(DB.Dec(dr["MenServingRatio"]), 4);
				//txtWomenRatio.Text                = Fmt.Num(DB.Dec(dr["WomenServingRatio"]), 4);
				//txtChildRatio.Text                = Fmt.Num(DB.Dec(dr["ChildServingRatio"]), 4);
				lnkScale.NavigateUrl                = Page.ResolveUrl(string.Format("~/RecipeScale.aspx?Rno={0}", RecipeRno));
				decimal YieldQty                    = DB.Dec(dr["YieldQty"]);
				txtYield.Text                       = Str.ShowFract(YieldQty, false);
				hfYieldUnitRno.Value                = DB.Int32(dr["YieldUnitRno"]).ToString();
				txtYieldUnit.Text                   = DB.Str(dr["YieldUnit" + (YieldQty <= 1 ? "Single" : "Plural")]);
				decimal PortionQty                  = DB.Dec(dr["PortionQty"]);
				txtPortion.Text                     = Str.ShowFract(PortionQty, false);
				hfPortionUnitRno.Value              = DB.Int32(dr["PortionUnitRno"]).ToString();
				txtPortionUnit.Text                 = DB.Str(dr["PortionUnit" + (PortionQty <= 1 ? "Single" : "Plural")]);
				txtRecipeBaseCostPrice.Text         = Fmt.Dollar(DB.Dec(dr["BaseCostPrice"]));
				txtBaseCostPrice.Text               = Fmt.Dollar(NumServings != 0 ? DB.Dec(dr["BaseCostPrice"]) / NumServings : 0);
				bool fUseDefaultBaseCostPct         = (dr["BaseCostPct"] == DBNull.Value);
				txtBaseCostPct.Text                 = Fmt.Num(DB.Dec(dr["BaseCostPct"]), 4);
				string DefaultBaseCostPct           = Fmt.Num(DB.Dec(dr["DefaultBaseCostPct"]), 4);
				lblBaseCostPct.Text                 = DefaultBaseCostPct;
				if (fUseDefaultBaseCostPct)
				{
					txtBaseCostPct.Style.Add("display", "none");
					lblBaseCostPct.Style.Clear();
				}
				else
				{
					txtBaseCostPct.Style.Clear();
					lblBaseCostPct.Style.Add("display", "none");
				}
				chkUseDefaultBaseCostPct.Text       = string.Format("Use {0}% default", DefaultBaseCostPct);
				chkUseDefaultBaseCostPct.Checked    = fUseDefaultBaseCostPct;
				chkGlutenFree.Checked               = DB.Bool(dr["GlutenFreeFlg"]);
                chkVegan.Checked                    = DB.Bool(dr["VeganFlg"]);
                chkVegetarian.Checked               = DB.Bool(dr["VegetarianFlg"]);
                chkDairyFree.Checked                = DB.Bool(dr["DairyFreeFlg"]);
                chkNuts.Checked                     = DB.Bool(dr["NutsFlg"]);
                chkIncludeInBook.Checked            = DB.Bool(dr["IncludeInBookFlg"]);
                txtCategory.Text                    = DB.Str(dr["Category"]);
                txtSource.Text                      = DB.Str(dr["Source"]);
				txtNote.Text                        = DB.Str(dr["IntNote"]);
				txtCreatedDt.Text                   = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
				txtCreatedUser.Text                 = DB.Str(dr["CreatedUser"]);
				txtUpdatedDt.Text                   = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
				txtUpdatedUser.Text                 = DB.Str(dr["UpdatedUser"]);

				GetSubrecipes(Rno);
				LoadMenuItems();
				LoadIngredients();
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	private void GetSubrecipes(int RecipeRno)
	{
		string Sql = string.Format("Select r.RecipeRno, r.Name From RecipeIngredXref x Inner Join Recipes r On x.RecipeRno = r.RecipeRno Where x.SubrecipeRno = {0} Order By r.Name", RecipeRno);

		try
		{
			DataTable dt = db.DataTable(Sql);
			lblRecipes.Visible = (dt.Rows.Count > 0);
			if (lblRecipes.Visible)
			{
				string Tip = string.Empty;
				lblRecipes.Text = string.Format("Used in {0} recipes", dt.Rows.Count);
				foreach (DataRow dr in dt.Rows)
				{
					Tip += string.Format("<li>{0}</li>\n", DB.Str(dr["Name"]));

					HtmlAnchor a = new HtmlAnchor();
					a.HRef = string.Format("Recipes.aspx?Rno={0}", DB.Int32(dr["RecipeRno"]));
					a.InnerText = DB.Str(dr["Name"]);
					HtmlGenericControl li = new HtmlGenericControl("li");
					li.Controls.Add(a);
					ulRecipes.Controls.Add(li);					
				}

				lblRecipes.ToolTip = string.Format("<ul>{0}</ul>", Tip);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	private void LoadMenuItems()
	{
		if (RecipeRno > 0)
		{
			string Sql = string.Format(
				"Select MenuItemRno, Category, MenuItem " +
				"From mcJobMenuItems Where RecipeRno = {0} " +
				"And IsNull(HideFlg, 0) = 0 " +
				"And Category In (Select Category From mcJobMenuCategories Where IsNull(HideFlg, 0) = 0) " +
				"Order By MenuItem, Category", 
				RecipeRno);
			LoadMenuItemsFromSql(Sql, false);
		}
		else
		{
			AddMenuItemLines(1);
		}
	}

	protected void LoadMenuItemsFromSql(string Sql, bool fNewRecipe)
	{
		try
		{
			DataTable dt = db.DataTable(Sql);
			cMenuItems = dt.Rows.Count + 1;
			AddMenuItemLines(cMenuItems);

			int iRow = 0;
			foreach (DataRow dr in dt.Rows)
			{
				iRow++;
				TableRow tr = tblMenuItems.Rows[iRow];

				HtmlInputHidden hfOrigMenuItemRno = (HtmlInputHidden)tr.FindControl("hfOrigMenuItemRno" + iRow);
				HtmlInputHidden hfMenuItemRno = (HtmlInputHidden)tr.FindControl("hfMenuItemRno" + iRow);
				TextBox txtMenuItem = (TextBox)tr.FindControl("txtMenuItem" + iRow);

				hfMenuItemRno.Value = DB.Int32(dr["MenuItemRno"]).ToString();
				hfOrigMenuItemRno.Value = (fNewRecipe ? string.Empty : hfMenuItemRno.Value);
				txtMenuItem.Text = string.Format("{0} - {1}", DB.Str(dr["MenuItem"]), DB.Str(dr["Category"]));
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
		RecipeRno = Str.Num(hfRecipeRno.Value);
		cMenuItems = Str.Num(hfNumMenuItems.Value);
		cIngredients = Str.Num(hfNumIngredients.Value);
		string Sql = string.Empty;
		fRecalcRecipePrice = false;

		try
		{
			int NumServings = Str.Num(txtNumServings.Text);
			int OrigNumServings = Str.Num(hfOrigNumServings.Value);

			if (RecipeRno == 0)
			{
				Sql = string.Format(
					"Insert Into Recipes (Name, CreatedDtTm, CreatedUser) Values ({0}, GetDate(), {1}); " +
					"Select Scope_Identity()",
					DB.PutStr(txtName.Text, 50),
					DB.PutStr(g.User));
				RecipeRno = db.SqlNum(Sql);
			}

			Sql = string.Format(
				"Update Recipes Set " +
				"Name = {1}, " +
				"SubrecipeFlg = {2}, " +
				"NumServings = {3}, " +
				//"MenServingRatio = {4}, " +
				//"WomenServingRatio = {5}, " +
				//"ChildServingRatio = {6}, " +
				"YieldQty = {4}, " +
				"YieldUnitRno = {5}, " +
				"PortionQty = {6}, " +
				"PortionUnitRno = {7}, " +
				"PortionPrice = {8}, " +
				"BaseCostPct = {9}, " +
				"Instructions = {10}, " +
                "Category = {11}, " +
				"Source = {12}, " +
				"IntNote = {13}, " +
				"HideFlg = {14}, " +
				"GlutenFreeFlg = {15}, " +
                "VeganFlg = {16}, " +
                "VegetarianFlg = {17}, " +
                "DairyFreeFlg = {18}, " +
                "NutsFlg = {19}, " +
                "IncludeInBookFlg = {20}, " + 
				"UpdatedDtTm = GetDate(), " +
				"UpdatedUser = {21} " + 
				"Where RecipeRno = {0}",
				RecipeRno,
				DB.PutStr(txtName.Text, 50),
				DB.PutBool(ddlType.Text == "Subrecipe"),
				NumServings,
				//Str.Dec(txtMenRatio.Text),
				//Str.Dec(txtWomenRatio.Text),
				//Str.Dec(txtChildRatio.Text),
				Str.Fract(txtYield.Text),
				(txtYieldUnit.Text.Length > 0 ? Str.Num(hfYieldUnitRno.Value) : 0),
				Str.Fract(txtPortion.Text),
				(txtPortionUnit.Text.Length > 0 ? Str.Num(hfPortionUnitRno.Value) : 0),
				Str.Dec(txtPortionPrice.Text),
				(chkUseDefaultBaseCostPct.Checked ? "Null" : (Str.Dec(txtBaseCostPct.Text)).ToString()), 
				DB.PutStr(txtInstructions.Text, 8000),
                DB.PutStr(txtCategory.Text, 50),
				DB.PutStr(txtSource.Text, 1000),
				DB.PutStr(txtNote.Text, 2000),
				DB.PutBool(chkHide.Checked),
				DB.PutBool(chkGlutenFree.Checked),
                DB.PutBool(chkVegan.Checked),
                DB.PutBool(chkVegetarian.Checked),
                DB.PutBool(chkDairyFree.Checked),
                DB.PutBool(chkNuts.Checked),
                DB.PutBool(chkIncludeInBook.Checked),
                DB.PutStr(g.User));
			db.Exec(Sql);

			fStartHere = true;
			StartHere = RecipeRno;

			if (NumServings != OrigNumServings)
			{
				fRecalcRecipePrice = true;
			}

			RemoveMenuItems();
			SaveMenuItems(RecipeRno);

			RemoveIngredients();
			SaveIngredients(RecipeRno);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	private void RemoveMenuItems()
	{
		for (int iMenuItem = 1; iMenuItem <= cMenuItems; iMenuItem++)
		{
			bool fRemove = Parm.Bool("chkRemoveMenuItem" + iMenuItem);
			if (fRemove)
			{
				String Sql = "";
				try
				{
					int MenuItemRno = Parm.Int("hfMenuItemRno" + iMenuItem);
					Sql = "Update mcJobMenuItems Set RecipeRno = Null, ServingPrice = Null, InaccuratePriceFlg = Null Where MenuItemRno = " + MenuItemRno;
					db.Exec(Sql);
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex, Sql);
					Response.Write(Err.Html());
				}
			}
		}
	}

	private void SaveMenuItems(int RecipeRno)
	{
		string Sql = string.Empty;

		for (int iMenuItem = 1; iMenuItem <= cMenuItems; iMenuItem++)
		{
			try
			{
				bool fRemove = Parm.Bool("chkRemoveMenuItem" + iMenuItem);
				if (!fRemove)
				{
					int OrigMenuItemRno = Parm.Int("hfOrigMenuItemRno" + iMenuItem);
					int MenuItemRno = Parm.Int("hfMenuItemRno" + iMenuItem);

					if (MenuItemRno != OrigMenuItemRno && MenuItemRno > 0)
					{
						if (OrigMenuItemRno > 0)
						{
							Sql = string.Format("Update mcJobMenuItems Set RecipeRno = Null, ServingPrice = Null, InaccuratePriceFlg = Null Where MenuItemRno = {0}", OrigMenuItemRno);
							db.Exec(Sql);
						}
						if (MenuItemRno > 0)
						{
							Sql = string.Format("Update mcJobMenuItems Set RecipeRno = {1} Where MenuItemRno = {0}", MenuItemRno, RecipeRno);
							db.Exec(Sql);
							fRecalcRecipePrice = true;
						}
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

	private void LoadList()
	{
		string Sql = "";
		// default for category or menu item
		string SqlSelect = 
			"Select MenuItemRno, Category, MenuItem, RecipeRno " +
			"From mcJobMenuItems i ";
		string SqlWhere = "IsNull(HideFlg, 0) = 0 And Category In (Select Category From mcJobMenuCategories Where IsNull(HideFlg, 0) = 0) And IsNull(AsIsFlg, 0) = 0";
		SqlWhere = DB.And(SqlWhere, (chkWithRecipe.Checked ? "RecipeRno Is Not Null" : string.Empty));
		string SqlOrderBy = string.Empty;

		if (rbSortCategory.Checked)
		{
			SqlOrderBy = " Order By Category, MenuItem";
		}
		else if (rbSortMenuItem.Checked)
		{
			SqlOrderBy = " Order By MenuItem, Category";
		}
		else if (rbSortRecipe.Checked)
		{
			string NumMenuItems = "(Select Count(*) From mcJobMenuItems Where RecipeRno = r.RecipeRno And IsNull(HideFlg, 0) = 0 And Category In (Select Category From mcJobMenuCategories Where IsNull(HideFlg, 0) = 0))";
			SqlSelect = "Select RecipeRno, Name, HideFlg, SubrecipeFlg, " + NumMenuItems + " As NumMenuItems From Recipes r ";
			SqlWhere = string.Empty;
			if (rbRecipes.Checked)
			{
				SqlWhere = DB.And(SqlWhere, "IsNull(SubrecipeFlg, 0) = 0");
			}
			if (rbSubrecipes.Checked)
			{
				SqlWhere = DB.And(SqlWhere, "IsNull(SubrecipeFlg, 0) = 1");
			}
			SqlWhere = DB.And(SqlWhere, (chkWithMenuItem.Checked ? NumMenuItems + " > 0" : string.Empty));
			SqlWhere = DB.And(SqlWhere, "IsNull(MenuItemAsIsFlg, 0) = 0");
			SqlWhere = DB.And(SqlWhere, (chkShowHidden.Checked ? string.Empty : "IsNull(HideFlg, 0) = 0"));
			SqlOrderBy = " Order By Name";
		}

		//string CurrItem = (lstList.SelectedIndex >= 0 ? (fStartHere && lstList.SelectedItem.Value.StartsWith("0|") ? StartHere.ToString() + lstList.SelectedItem.Value.Substring(1) : lstList.SelectedItem.Value) : string.Empty);
		string CurrItem = (lstList.SelectedIndex >= 0 ? (fStartHere ? StartHere.ToString() : lstList.SelectedItem.Value) : string.Empty);

		//Response.Write("Curr " + lstList.SelectedItem.Value);
		lstList.Items.Clear();

		Sql = SqlSelect;
		Sql += DB.Where(SqlWhere);
		Sql += SqlOrderBy;

		//Response.Write(Sql + "<br/>");

		LoadListSql(Sql, CurrItem);

		lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Items";

		//btnUpdate.Enabled = (CurrItem.Length > 0);
	}

	private void LoadListSql(string Sql, string NextCurrItem)
	{
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				string Rno;
				string sItem;
				string Class = string.Empty;

				if (rbSortRecipe.Checked)
				{
					Rno = DB.Int32(dr["RecipeRno"]).ToString();
					bool fSubrecipe = DB.Bool(dr["SubrecipeFlg"]);
					bool fHas = (DB.Int32(dr["NumMenuItems"]) > 0);
					sItem = (fSubrecipe ? "↓ " : string.Empty) + (fHas ? "♦ " : string.Empty) + DB.Str(dr["Name"]);
					Class = (DB.Bool(dr["HideFlg"]) ? "Hidden" : (fHas ? "Has" : string.Empty));
				}
				else
				{
					Rno = string.Format("{0}|{1}", DB.Int32(dr["RecipeRno"]), DB.Int32(dr["MenuItemRno"]));
					string Category = DB.Str(dr["Category"]);
					string MenuItem = DB.Str(dr["MenuItem"]);
					bool fHas = (DB.Int32(dr["RecipeRno"]) > 0);
					string Seperator = (fHas ? " ♦ " : " - ");
					Class = (fHas ? "Has" : string.Empty);
					sItem = (rbSortCategory.Checked ? Category + Seperator + MenuItem : MenuItem + Seperator + Category);
				}
				ListItem Item = new ListItem(sItem, Rno);
				//Response.Write(string.Format("[{0}] [{1}] [{2}] [{3}] <br/>", sItem, Rno, Item.Text, Item.Value));
				Item.Attributes.Add("title", sItem);
				Item.Selected = (Item.Value == NextCurrItem);
				Item.Attributes.Add("class", Class);
				lstList.Items.Add(Item);
			}

			lstList.CssClass = "SelectJob" + (rbSortRecipe.Checked ? string.Empty : " MenuList");
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
		Session["FoodSortedBy"] = (rbSortCategory.Checked ? "Category" : "MenuItem");

		ClearData();
		LoadMenuItems();
		LoadIngredients();
	}

	private void AddMenuItemLines(int cMenuItems)
	{
		for (int iMenuItem = 1; iMenuItem <= cMenuItems; iMenuItem++)
		{
			tblMenuItems.Rows.Add(BuildMenuItemRow(iMenuItem.ToString()));
		}
	}

	protected string AddMenuItemHtml()
	{	
		TableRow rw = BuildMenuItemRow("~ID~");

		StringBuilder sb = new StringBuilder();
		rw.RenderControl(new HtmlTextWriter(new StringWriter(sb)));

		return sb.ToString().Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "\\\"");
	}

	private TableRow BuildMenuItemRow(string ID)
	{
		int cCells = 2;
		int iCell;
		TableRow rw = new TableRow();
		rw.CssClass = "MenuItem";

		for (iCell = 0; iCell < cCells; iCell++)
		{
			rw.Cells.Add(new TableCell());
		}

		iCell = 0;

		HtmlInputHidden hfOrigMenuItemRno = new HtmlInputHidden();
		hfOrigMenuItemRno.ID = "hfOrigMenuItemRno" + ID;
		rw.Cells[iCell].Controls.Add(hfOrigMenuItemRno);

		HtmlInputHidden hfMenuItemRno = new HtmlInputHidden();
		hfMenuItemRno.ID = "hfMenuItemRno" + ID;
		rw.Cells[iCell].Controls.Add(hfMenuItemRno);

		TextBox txt = new TextBox();
		txt.ID = "txtMenuItem" + ID;
		txt.CssClass = "Name";
		rw.Cells[iCell++].Controls.Add(txt);

		// Remove
		CheckBox chkRemove = new CheckBox();
		chkRemove.ID = "chkRemoveMenuItem" + ID;
		chkRemove.CssClass = "Remove";
		chkRemove.TabIndex = -1;
		rw.Cells[iCell++].Controls.Add(chkRemove);

		return rw;
	}

	private void AddLines()
	{
		for (int iIngredient = 1; iIngredient <= cIngredients; iIngredient++)
		{
			TableRow tr = BuildIngredientRow(iIngredient.ToString());
			tblIngredients.Rows.Add(tr);
		}
	}

	protected string AddRowHtml()
	{
		TableRow rw = BuildIngredientRow("~ID~");

		StringBuilder sb = new StringBuilder();
		rw.RenderControl(new HtmlTextWriter(new StringWriter(sb)));

		return sb.ToString().Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "\\\"");
	}

	private TableRow BuildIngredientRow(string ID)
	{
		int cCells = 8;
		int iCell;
		TableRow rw = new TableRow();

		for (iCell = 0; iCell < cCells; iCell++)
		{
			rw.Cells.Add(new TableCell());
		}

		iCell = 0;

		// Seq
		HtmlInputHidden hfRecipeSeq = new HtmlInputHidden();
		hfRecipeSeq.ID = "hfRecipeSeq" + ID;
		hfRecipeSeq.Attributes.Add("class", "Seq");
		hfRecipeSeq.Value = ID;
		rw.Cells[iCell].Controls.Add(hfRecipeSeq);

		// sortable
		HtmlGenericControl div = new HtmlGenericControl("div");
		div.Attributes.Add("class", "arrow ui-icon ui-icon-arrowthick-2-n-s");
		div.InnerText = "&nbsp;";
		rw.Cells[iCell++].Controls.Add(div);

		HtmlInputHidden hfRecipeIngredRno = new HtmlInputHidden();
		hfRecipeIngredRno.ID = "hfRecipeIngredRno" + ID;
		rw.Cells[iCell].Controls.Add(hfRecipeIngredRno);

		// Qty
		TextBox txtQty = new TextBox();
		txtQty.ID = "txtQty" + ID;
		txtQty.CssClass = "Qty";
		rw.Cells[iCell++].Controls.Add(txtQty);

		// Unit
		HtmlInputHidden hfUnitRno = new HtmlInputHidden();
		hfUnitRno.ID = "hfUnitRno" + ID;
		hfUnitRno.Attributes.Add("class", "UnitRno");
		rw.Cells[iCell].Controls.Add(hfUnitRno);

		TextBox txtUnit = new TextBox();
		txtUnit.ID = "txtUnit" + ID;
		txtUnit.CssClass = "Unit";
		rw.Cells[iCell++].Controls.Add(txtUnit);

		// Subrecipe
		HtmlGenericControl spnSubrecipe = new HtmlGenericControl("span");
		spnSubrecipe.ID = "spnSubrecipe" + ID;
		rw.Cells[iCell].Controls.Add(spnSubrecipe);

		// Missing price
		HtmlGenericControl spnMissingPrice = new HtmlGenericControl("span");
		spnMissingPrice.ID = "spnMissingPrice" + ID;
		rw.Cells[iCell++].Controls.Add(spnMissingPrice);

		// Ingredient
		HtmlInputHidden hfIngredRno = new HtmlInputHidden();
		hfIngredRno.Attributes.Clear();
		hfIngredRno.ID = "hfIngredRno" + ID;
		hfIngredRno.Attributes.Add("class", "IngredRno");
		rw.Cells[iCell].Controls.Add(hfIngredRno);

		HtmlInputHidden hfSubrecipeRno = new HtmlInputHidden();
		hfSubrecipeRno.ID = "hfSubrecipeRno" + ID;
		hfSubrecipeRno.Attributes.Add("class", "SubrecipeRno");
		rw.Cells[iCell].Controls.Add(hfSubrecipeRno);

		TextBox txtIngredient = new TextBox();
		txtIngredient.ID = "txtIngredient" + ID;
		txtIngredient.CssClass = "Ingredient";
		rw.Cells[iCell++].Controls.Add(txtIngredient);

		// Notes
		TextBox txtNote = new TextBox();
		txtNote.ID = "txtNote" + ID;
		txtNote.CssClass = "Note";
		rw.Cells[iCell++].Controls.Add(txtNote);

		// Remove
		CheckBox chkRemove = new CheckBox();
		chkRemove.ID = "chkRemove" + ID;
		chkRemove.CssClass = "Remove";
		chkRemove.TabIndex = -1;
		rw.Cells[iCell++].Controls.Add(chkRemove);

		// Price
		Label lblPrice = new Label();
		lblPrice.ID = "lblPrice" + ID;
		rw.Cells[iCell].CssClass = "Price";
		rw.Cells[iCell++].Controls.Add(lblPrice);

		return rw;
	}

	protected string AddTitleRowHtml()
	{
		TableRow rw = BuildTitleRow("~ID~");

		StringBuilder sb = new StringBuilder();
		rw.RenderControl(new HtmlTextWriter(new StringWriter(sb)));

		return sb.ToString().Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "\\\"");
	}

	private TableRow BuildTitleRow(string ID)
	{
		int cCells = 3;
		int iCell;
		TableRow rw = new TableRow();

		for (iCell = 0; iCell < cCells; iCell++)
		{
			rw.Cells.Add(new TableCell());
		}

		iCell = 0;

		// Seq
		HtmlInputHidden hfRecipeSeq = new HtmlInputHidden();
		hfRecipeSeq.ID = "hfRecipeSeq" + ID;
		hfRecipeSeq.Attributes.Add("class", "Seq");
		hfRecipeSeq.Value = ID;
		rw.Cells[iCell].Controls.Add(hfRecipeSeq);

		// sortable
		HtmlGenericControl div = new HtmlGenericControl("div");
		div.Attributes.Add("class", "arrow ui-icon ui-icon-arrowthick-2-n-s");
		div.InnerText = "&nbsp;";
		rw.Cells[iCell++].Controls.Add(div);

		HtmlInputHidden hfRecipeIngredRno = new HtmlInputHidden();
		hfRecipeIngredRno.ID = "hfRecipeIngredRno" + ID;
		rw.Cells[iCell].Controls.Add(hfRecipeIngredRno);

        // Title
        TextBox txtTitle = new TextBox();
        //HtmlTextArea txtTitle = new HtmlTextArea();
        txtTitle.ID = "txtTitle" + ID;
		txtTitle.CssClass = "Title";
        //txtTitle.Attributes.Add("Class", "Title");
        txtTitle.TextMode = TextBoxMode.MultiLine;
        rw.Cells[iCell].ColumnSpan = 5;
		rw.Cells[iCell++].Controls.Add(txtTitle);

		// Remove
		CheckBox chkRemove = new CheckBox();
		chkRemove.ID = "chkRemove" + ID;
		chkRemove.CssClass = "Remove";
		chkRemove.TabIndex = -1;
		rw.Cells[iCell++].Controls.Add(chkRemove);

		return rw;
	}

	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		//RemoveIngredients();
		//SaveIngredients();

		String Sql = "";

		try
		{
			string Value = lstList.SelectedItem.Value;
			string[] aValue = Value.Split('|');
			RecipeRno = Str.Num(aValue[0]);

			if (RecipeRno > 0)
			{
				GetData(RecipeRno);
				fStartHere = true;
				StartHere = RecipeRno;
				LoadList();
			}
			else
			{
				ClearData();
				int MenuItemRno = Str.Num(aValue[1]);

				Sql = string.Format("Select MenuItem From mcJobMenuItems Where MenuItemRno = {0}", MenuItemRno);
				string MenuItem = db.SqlStr(Sql);
				txtName.Text = MenuItem;

				Sql = string.Format("Select MenuItemRno, Category, MenuItem from mcJobMenuItems Where MenuItem = {0} Order By MenuItem, Category", DB.PutStr(MenuItem));
				LoadMenuItemsFromSql(Sql, true);

				cIngredients = 1;
				AddLines();

				LoadList();

				FocusField = "txtQty1";
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void btnNew_Click(object sender, System.EventArgs e)
	{
		ClearData();
		LoadMenuItems();
		LoadIngredients();
	}

	protected void btnSave_Click(object sender, System.EventArgs e)
	{
		Update();

		//// see if this update is from an Ingredient
		//TextBox txtBox = new TextBox();
		//if (sender.GetType() == txtBox.GetType())
		//{
		//	txtBox = (TextBox)sender;
		//	String Ingredient = "txtIngredient";
		//	if (txtBox.ID.Substring(0, Ingredient.Length) == Ingredient)
		//	{
		//		// set the focus on the Qty
		//		FocusField = "txtQty" + txtBox.ID.Substring(Ingredient.Length);
		//	}
		//}
	}

	protected void btnCopy_Click(object sender, System.EventArgs e)
	{
		Copy();
		GetData(RecipeRno);
		LoadList();
	}

	private void Update()
	{
		SaveData();
		GetData(RecipeRno);
		LoadList();
		//LoadMenuItems();
		//LoadIngredients();
	}

	private void LoadIngredients()
	{
		bool fMissingCost = false;

		if (RecipeRno > 0)
		{
			String Sql = "";

			try
			{
				Sql = string.Format(
					"Select x1.RecipeSeq " +
					"From RecipeIngredXref x1 Inner Join RecipeIngredXref x2 " +
					"On x1.RecipeRno = x2.RecipeRno And x1.RecipeSeq <> x2.RecipeSeq " + 
					"Where x1.RecipeRno = {0} " +
					"And (x1.IngredRno = x2.IngredRno And x1.IngredRno <> 0 Or x1.SubrecipeRno = x2.SubrecipeRno And x1.SubrecipeRno <> 0)",
					RecipeRno);
				DupSeq Dups = new DupSeq(db, Sql);

				Sql = string.Format(
					"Select x.*, Coalesce(i.Name, r.Name, '') As Ingredient, u.UnitSingle, u.UnitPlural, " +
					"r.NumServings, r.YieldQty, r.YieldUnitRno, (Select UnitPlural From Units Where UnitRno = r.YieldUnitRno) As YieldUnit, " +
					"r.PortionQty, (Select UnitPlural From Units Where UnitRno = r.PortionUnitRno) As PortionUnit, " +
					"i.NonPurchaseFlg " +
					"From RecipeIngredXref x " +
					"Left Join Ingredients i On x.IngredRno = i.IngredRno " +
					"Left Join Recipes r On x.SubrecipeRno = r.RecipeRno " +
					"Left Join Units u On x.UnitRno = u.UnitRno " + 
					"Where x.RecipeRno = {0} " +
					"Order By RecipeSeq",
					RecipeRno);

				DataTable dt = db.DataTable(Sql);

				int iRow = 0;
				foreach (DataRow dr in dt.Rows)
				{
					int IngredRno = DB.Int32(dr["IngredRno"]);
					int SubrecipeRno = DB.Int32(dr["SubrecipeRno"]);
					TableRow tr;

					iRow++;

					if (IngredRno != 0 || SubrecipeRno != 0)
					{
						tr = BuildIngredientRow(iRow.ToString());
					}
					else
					{
						tr = BuildTitleRow(iRow.ToString());
					}

					tblIngredients.Rows.Add(tr);

					bool fDup = Dups.In(DB.Int32(dr["RecipeSeq"]));

					HtmlInputHidden hfRecipeSeq = (HtmlInputHidden)tr.FindControl("hfRecipeSeq" + iRow);
					HtmlInputHidden hfRecipeIngredRno = (HtmlInputHidden)tr.FindControl("hfRecipeIngredRno" + iRow);
					CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iRow);

					hfRecipeSeq.Value = DB.Int32(dr["RecipeSeq"]).ToString();
					hfRecipeIngredRno.Value = DB.Int32(dr["RecipeIngredRno"]).ToString();
					chkRemove.Checked = false;
					

					if (IngredRno != 0 || SubrecipeRno != 0)
					{
						TextBox txtQty = (TextBox)tr.FindControl("txtQty" + iRow);
						HtmlInputHidden hfUnitRno = (HtmlInputHidden)tr.FindControl("hfUnitRno" + iRow);
						TextBox txtUnit = (TextBox)tr.FindControl("txtUnit" + iRow);
						HtmlInputHidden hfIngredRno = (HtmlInputHidden)tr.FindControl("hfIngredRno" + iRow);
						HtmlInputHidden hfSubrecipeRno = (HtmlInputHidden)tr.FindControl("hfSubrecipeRno" + iRow);
						HtmlGenericControl spnSubrecipe = (HtmlGenericControl)tr.FindControl("spnSubrecipe" + iRow);
						HtmlGenericControl spnMissingPrice = (HtmlGenericControl)tr.FindControl("spnMissingPrice" + iRow);
						TextBox txtIngredient = (TextBox)tr.FindControl("txtIngredient" + iRow);
						TextBox txtNote = (TextBox)tr.FindControl("txtNote" + iRow);
						Label lblPrice = (Label)tr.FindControl("lblPrice" + iRow);

						decimal Qty = DB.Dec(dr["UnitQty"]);
						txtQty.Text = Str.ShowFract(Qty);
						int UnitRno = DB.Int32(dr["UnitRno"]);
						hfUnitRno.Value = UnitRno.ToString();
						txtUnit.Text = DB.Str(dr[Qty <= 1 ? "UnitSingle" : "UnitPlural"]);
						hfIngredRno.Value = IngredRno.ToString();
						hfSubrecipeRno.Value = SubrecipeRno.ToString();
						txtIngredient.Text = DB.Str(dr["Ingredient"]);
						txtIngredient.CssClass = "Ingredient";
						txtNote.Text = DB.Str(dr["Note"]);
						lblPrice.Text = Fmt.Dollar(DB.Dec(dr["BaseCostPrice"]));
						bool fConvScalerProblem = false;

						// subrecipe info
						if (SubrecipeRno != 0)
						{
							HtmlGenericControl i = new HtmlGenericControl("i");
							i.Attributes.Add("class", "icon-folder-close");
							HtmlAnchor a = new HtmlAnchor();
							a.Controls.Add(i);
							a.HRef = string.Format("Recipes.aspx?Rno={0}", SubrecipeRno);
							a.Title = string.Format(
								"<ul>" +
								"<li><label>Subrecipe</label><span>{0}</span></li>" +
								"<li><label>Servings</label><span>{1}</span></li>" +
								"<li><label>Yield</label><span>{2} {3}</span></li>" +
								"<li><label>Serving Size</label><span>{4} {5}</span></li>" +
								"</ull>",
								DB.Str(dr["Ingredient"]),
								Fmt.Num(DB.Dec(dr["NumServings"]), 4, false),
								Str.ShowFract(DB.Dec(dr["YieldQty"])),
								DB.Str(dr["YieldUnit"]),
								Fmt.Num(DB.Dec(dr["PortionQty"]), 4, false),
								DB.Str(dr["PortionUnit"]));
							a.Attributes.Add("class", "qtp");
							spnSubrecipe.Controls.Add(a);

							// test for convertable units
							Ingred Ingred = new Ingred(0);
							int YieldUnitRno = DB.Int32(dr["YieldUnitRno"]);
							decimal ConversionScaler = Ingred.ConversionScaler(UnitRno, YieldUnitRno);

							if (ConversionScaler == 0)
							{
								i = new HtmlGenericControl("i");
								i.Attributes.Add("class", "icon-warning-sign");
								i.Attributes.Add("title", string.Format("Recipe's unit ({0}) doesn't convert to this subrecipe's yield units ({1}).", DB.Str(dr["UnitPlural"]), DB.Str(dr["YieldUnit"])));
								a = new HtmlAnchor();
								a.Controls.Add(i);
								a.HRef = string.Format("Recipes.aspx?Rno={0}", SubrecipeRno);
								spnSubrecipe.Controls.Add(a);
								fConvScalerProblem = true;
							}
						}

						if (dr["BaseCostPrice"] == DBNull.Value && !DB.Bool(dr["NonPurchaseFlg"]))
						{
							fMissingCost = true;
							HtmlGenericControl i = new HtmlGenericControl("i");
							i.Attributes.Add("class", "icon-dollar");
							i.Attributes.Add("title", (!fConvScalerProblem ? "There are no receipts with this ingredient." : "The subrecipe conversion problem prevents the price from being calculated."));
							HtmlAnchor a = new HtmlAnchor();
							a.Controls.Add(i);
							a.HRef = "Purchases.aspx?IngredRno=" + IngredRno.ToString();
							a.Target = "Fix";
							spnMissingPrice.Controls.Add(a);
						}

						if (fDup)
						{
							txtIngredient.CssClass = "DupIngredient";
						}
					}
					else
					{
                        TextBox txtTitle = (TextBox)tr.FindControl("txtTitle" + iRow);
                        txtTitle.Text = DB.Str(dr["Title"]);
                        //HtmlTextArea txtTitle = (HtmlTextArea)tr.FindControl("txtTitle" + iRow);
                        //txtTitle.Value = DB.Str(dr["Title"]);
                    }
                }

				iRow++;
				// add a blank row at the bottom to leave a place to add a new ingredient/subrecipe
				TableRow tr1 = BuildIngredientRow(iRow.ToString());
				tblIngredients.Rows.Add(tr1);

				// clear a few of the values left behind from a deleted row
				HtmlGenericControl spnMissingPrice1 = (HtmlGenericControl)tr1.FindControl("spnMissingPrice" + iRow);
				spnMissingPrice1.Attributes.Clear();
				Label lblPrice1 = (Label)tr1.FindControl("lblPrice" + iRow);
				lblPrice1.Text = string.Empty;

				cIngredients = iRow;

				if (fMissingCost)
				{
					HtmlGenericControl i = new HtmlGenericControl("i");
					i.Attributes.Add("class", "icon-dollar");
					i.Attributes.Add("title", "Cost and Price are inaccurate because there are ingredients with no price from receipts.");
					lblCost.Controls.Add(i);
				}
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
		else
		{
			cIngredients = 1;
			AddLines();
		}
	}

	private void Copy()
	{
		string Sql = string.Empty;
		string RecipeFields = "SubrecipeFlg, NumServings, MenServingRatio, WomenServingRatio, ChildServingRatio, YieldQty, YieldUnitRno, PortionQty, PortionUnitRno, PortionPrice, BaseCostPrice, BaseCostPct, InaccuratePriceFlg, Instructions, Source, Images, IntNote, CT_RecipeID, MenuItemAsIsFlg, HideFlg";
		string XrefFields = "RecipeSeq, IngredRno, SubrecipeRno, UnitQty, UnitRno, BaseCostPrice, Title, Note";

		try
		{
			Sql = string.Format("Select Count(*) From Recipes Where Name = (Select Name + ' - Copy' From Recipes Where RecipeRno = {0})", RecipeRno);
			if (db.SqlNum(Sql) == 0)
			{
				Sql = string.Format(
					"Insert Into Recipes (Name, {1}, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) " +
					"Select Name + ' - Copy', {1}, GetDate(), {2}, GetDate(), {2} From Recipes Where RecipeRno = {0};" +
					"Select @@Identity;",
					RecipeRno,
					RecipeFields,
					DB.PutStr(g.User));

				int NewRecipeRno = db.SqlNum(Sql);

				Sql = string.Format(
					"Insert Into RecipeIngredXref (RecipeRno, {2}, CreatedDtTm, CreatedUser, UpdatedDtTm, UpdatedUser) " +
					"Select {1}, {2}, GetDate(), {3}, GetDate(), {3} From RecipeIngredXref Where RecipeRno = {0}",
					RecipeRno,
					NewRecipeRno,
					XrefFields,
					DB.PutStr(g.User));

				db.Exec(Sql);

				RecipeRno = NewRecipeRno;
				fStartHere = true;
				StartHere = RecipeRno;
			}
			else
			{
				Sql = string.Format("Select Name From Recipes Where RecipeRno = {0}", RecipeRno);
				ErrMsg = string.Format("<b>Warning!</b> There is already a recipe named <b style='font-size: 125%;'>{0} - Copy</b>", db.SqlStr(Sql));
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	private void RemoveIngredients()
	{
		for (int iIngredient = 1; iIngredient <= cIngredients; iIngredient++)
		{
			//TableRow tr = tblIngredients.Rows[iIngredient];

			//CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iIngredient);
			bool fRemove = Parm.Bool("chkRemove" + iIngredient);
			//if (chkRemove != null && chkRemove.Checked)
			if (fRemove)
			{
				String Sql = "";
				try
				{
					//Int32 RecipeIngredRno = Str.Num(WebPage.FindTextBox(ref tr, "hfRecipeIngredRno" + iIngredient));
					int RecipeIngredRno = Parm.Int("hfRecipeIngredRno" + iIngredient);
					Sql = "Delete From RecipeIngredXref Where RecipeIngredRno = " + RecipeIngredRno;
					db.Exec(Sql);
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex, Sql);
					Response.Write(Err.Html());
				}
			}
		}
	}

	private void SaveIngredients(int RecipeRno)
	{
		for (int iIngredient = 1; iIngredient <= cIngredients; iIngredient++)
		{
			bool fRemove = Parm.Bool("chkRemove" + iIngredient);
			if (fRemove)
			{
				fRecalcRecipePrice = true;
			}
			else
			{
				//Int32 RecipeSeq = 1;
				int RecipeSeq = Parm.Int("hfRecipeSeq" + iIngredient);
				//Int32 RecipeIngredRno = Str.Num(WebPage.FindTextBox(ref tr, "hfRecipeIngredRno" + iIngredient));
				int RecipeIngredRno = Parm.Int("hfRecipeIngredRno" + iIngredient);
				bool fNewRec = (RecipeIngredRno == 0);
				//decimal Qty = Str.Fract(WebPage.FindTextBox(ref tr, "txtQty" + iIngredient));
				decimal Qty = Str.Fract(Parm.Str("txtQty" + iIngredient));
				//int UnitRno = Str.Num(WebPage.FindTextBox(ref tr, "hfUnitRno" + iIngredient));
				int UnitRno = Parm.Int("hfUnitRno" + iIngredient);
				//int IngredRno = Str.Num(WebPage.FindTextBox(ref tr, "hfIngredRno" + iIngredient));
				int IngredRno = Parm.Int("hfIngredRno" + iIngredient);
				int SubrecipeRno = Parm.Int("hfSubrecipeRno" + iIngredient);
				//string Note = WebPage.FindTextBox(ref tr, "txtNote" + iIngredient);
				string Note = Parm.Str("txtNote" + iIngredient);
				string Title = Parm.Str("txtTitle" + iIngredient);

				if (IngredRno > 0 || SubrecipeRno > 0 || Title.Length > 0)
				{
					DateTime Tm = DateTime.Now;
					String Sql = "";

					try
					{
						if (RecipeIngredRno == 0)
						{
							Sql = string.Format(
								"Insert Into RecipeIngredXref (RecipeRno, RecipeSeq, CreatedDtTm, CreatedUser) " +
								"Values ({0}, {1}, GetDate(), {2});" +
								"Select Scope_Identity()",
								RecipeRno,
								RecipeSeq,
								DB.PutStr(g.User));
							RecipeIngredRno = db.SqlNum(Sql);
						}

						if (IngredRno != 0 || SubrecipeRno != 0)
						{
							Sql = string.Format(
								"Update RecipeIngredXref Set " +
								"RecipeSeq = {1}, " +
								"IngredRno = {2}, " +
								"SubrecipeRno = {3}, " +
								"UnitQty = {4}, " +
								"UnitRno = {5}, " +
								"Note = {6}, " +
								"UpdatedDtTm = GetDate(), " +
								"UpdatedUser = {7} " +
								"Where RecipeIngredRno = {0}",
								RecipeIngredRno,
								RecipeSeq,
								(IngredRno != 0 ? IngredRno.ToString() : "Null"),
								(SubrecipeRno != 0 ? SubrecipeRno.ToString() : "Null"),
								Qty,
								UnitRno,
								(Note.Length > 0 ? DB.PutStr(Note, 2000) : "Null"),
								DB.PutStr(g.User));
							db.Exec(Sql);

							if (IngredRno != 0)
							{
								Ingred.UpdateWithLastPrice(IngredRno);
							}
							else if (SubrecipeRno != 0)
							{
								Ingred.UpdateSubrecipePrice(SubrecipeRno);
							}
						}
						else
						{
							Sql = string.Format(
								"Update RecipeIngredXref Set " +
								"RecipeSeq = {1}, " +
								"Title = {2}, " +
								"UpdatedDtTm = GetDate(), " +
								"UpdatedUser = {3} " +
								"Where RecipeIngredRno = {0}",
								RecipeIngredRno,
								RecipeSeq,
								(Title.Length > 0 ? DB.PutStr(Title, 500) : "Null"),
								DB.PutStr(g.User));
							db.Exec(Sql);
						}
					}
					catch (Exception Ex)
					{
						Err Err = new Err(Ex, Sql);
						Response.Write(Err.Html());
					}
				}
			}
		}

		// if an ingredient was removed 
		if (fRecalcRecipePrice)
		{
			// calculate the new recipe cost and it's affect in upline recipes
			Ingred.RecalcRecipePrice(db, RecipeRno);
		}
	}

	//private String FindTextBox(ref TableRow tr, String ID)
	//{
	//	String Text = "";

	//	try
	//	{
	//		Control Ctrl = tr.FindControl(ID);
	//		if (Ctrl != null)
	//		{
	//			TextBox txtBox = new TextBox();
	//			HtmlInputHidden txtHidden = new HtmlInputHidden();

	//			if (Ctrl.GetType() == txtBox.GetType())
	//			{
	//				txtBox = (TextBox)Ctrl;
	//				Text = txtBox.Text.Trim();
	//			}
	//			else
	//			if (Ctrl.GetType() == txtHidden.GetType())
	//			{
	//				txtHidden = (HtmlInputHidden)Ctrl;
	//				Text = txtHidden.Value.Trim();
	//			}
	//		}
	//	}
	//	catch (Exception Ex)
	//	{
	//		Err Err = new Err(Ex);
	//		Response.Write(Err.Html());
	//	}

	//	return Text;
	//}

	protected string MenuItemData()
	{
		string Sql = string.Format(
			"Select MenuItemRno, Category, MenuItem, RecipeRno " +
			"From mcJobMenuItems " +
			"Where IsNull(HideFlg, 0) = 0 " + 
			"And Category In (Select Category From mcJobMenuCategories Where IsNull(HideFlg, 0) = 0) " +
			"And IsNull(AsIsFlg, 0) = 0 " +
			"Order By MenuItem, Category", 
			(RecipeRno != 0 ? RecipeRno  : -1));
		//And IsNull(RecipeRno, 0) <> {0} " +
		StringBuilder sb = new StringBuilder();

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				//if (++c > 64) { break; }
				sb.AppendFormat("{{label:'{0} - {1}',id:{2},used:{3}}},",
					DB.Str(dr["MenuItem"]).Replace(@"\", @"\\").Replace("'", @"\'"),
					DB.Str(dr["Category"]).Replace(@"\", @"\\").Replace("'", @"\'"), 
					DB.Int32(dr["MenuItemRno"]), 
					(DB.Int32(dr["RecipeRno"]) != 0).ToString().ToLower());
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}

		return string.Format("[{0}]", (sb.Length > 0 ? sb.ToString().Remove(sb.Length - 1, 1) : string.Empty));
	}

	protected string IngredientData()
	{
		string Sql = string.Format(
			"If Object_ID('tempdb..#Rno') Is Not Null Drop Table #Rno\n" +
			"Select {0} As RecipeRno Into #Rno\n" +
			"Declare @PrevCnt integer\n" +
			"Declare @Cnt integer\n" +
			"Set @PrevCnt = 0\n" +
			"Select @Cnt = Count(*) From #Rno\n" +
			"While (@Cnt <> @PrevCnt)\n" +
			"Begin\n" +
			"	Insert Into #Rno (RecipeRno)\n" +
			"	Select RecipeRno From RecipeIngredXref Where SubrecipeRno In (Select RecipeRno From #Rno)\n" +
			"	And RecipeRno Not In (Select RecipeRno From #Rno)\n" +
			"	Set @PrevCnt = @Cnt\n" +
			"	Select @Cnt = Count(*) From #Rno\n" +
			"End\n" +
			"\n" +
			"Select IngredRno, Null As SubrecipeRno, Name From Ingredients Where IsNull(HideFlg, 0) = 0\n" +
			"Union\n" +
			"Select Null As IngredRno, RecipeRno As SubrecipeRno, Name From Recipes Where SubrecipeFlg = 1 And IsNull(HideFlg, 0) = 0 And IsNull(MenuItemAsIsFlg, 0) = 0 And RecipeRno Not In (Select RecipeRno From #Rno)\n" +
			"Order By Name\n" +
			"Drop Table #Rno",
			RecipeRno);

		StringBuilder sb = new StringBuilder();

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				sb.AppendFormat("{{label:'{0}',ingredId:{1}, subrecipeId:{2}}},",
					DB.Str(dr["Name"]).Replace(@"\", @"\\").Replace("'", @"\'"), 
					DB.Int32(dr["IngredRno"]), 
					DB.Int32(dr["SubrecipeRno"]));
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}

		return string.Format("[{0}]", sb.ToString().Remove(sb.Length - 1, 1));
	}

	protected string UnitData()
	{
        return Misc.UnitData(db);
	}

    protected string CategoryData()
    {
        string Sql = 
            "Select Distinct Category " +
            "From Recipes " +
            "Where Category Is Not Null " +
            "Order By Category";
        StringBuilder sb = new StringBuilder();

        try
        {
            DataTable dt = db.DataTable(Sql);
            foreach (DataRow dr in dt.Rows)
            {
                sb.AppendFormat("\"{0}\",", DB.Str(dr["Category"]));
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex);
            Response.Write(Err.Html());
        }

        return string.Format("[{0}]", (sb.Length > 0 ? sb.ToString().Remove(sb.Length - 1, 1) : string.Empty));
    }
}