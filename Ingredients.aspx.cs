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

public partial class Ingredients : System.Web.UI.Page
{
	public static string Dry = "D";
	public static string Liquid = "L";

	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";
	protected int cConversions;

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
		hfNumConversions.Value = cConversions.ToString();

		string Sql = "Select VendorRno, Name From Vendors Where (HideFlg = 0 Or HideFlg Is Null) And Name <> '' Order By Name";

		try
		{
			ulPrefVendors.Controls.Clear();
			ulRejVendors.Controls.Clear();

			DataTable dt = DB.DBDataTable(Sql);

			foreach (DataRow dr in dt.Rows)
			{
				string ID = DB.Int32(dr["VendorRno"]).ToString();
				CheckBox chk1 = new CheckBox();
				CheckBox chk2 = new CheckBox();
				chk1.Text =
				chk2.Text = Server.HtmlEncode(DB.Str(dr["Name"]));
				chk1.ID = "chkPrefVendor-" + ID;
				chk2.ID = "chkRejVendor-" + ID;
				chk1.Attributes.Add("data-id", ID);
				chk2.Attributes.Add("data-id", ID);

				HtmlGenericControl li = new HtmlGenericControl("li");
				li.Controls.Add(chk1);
				ulPrefVendors.Controls.Add(li);

				li = new HtmlGenericControl("li");
				li.Controls.Add(chk2);
				ulRejVendors.Controls.Add(li);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

    private void Setup()
    {
		string Rno = Parm.Str("Rno");
		if (Rno.Length > 0)
		{
			fStartHere = true;
			string Sql = string.Format("Select Name From Ingredients Where IngredRno = {0}", Rno);
			try
			{
				StartHere = db.SqlStr(Sql);
				//Response.Write("Setup " + StartHere + "<br/>");
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}

		LoadList();
		//LoadConversions();
    }
    
	protected void ClearData()
	{
		lstList.SelectedIndex = -1;
		txtRno.Value =
		txtName.Text =
        txtStockedPurchaseQty.Text = "";
		chkHide.Checked =
		chkStocked.Checked =
		chkNonPurchase.Checked = false;
		//rbDry.Checked =
		//rbLiquid.Checked = false;

		hfNumConversions.Value = string.Empty;

		btnDelete.Enabled = false;
		btnDelete.ToolTip = "New ingredient item record";
	}

    protected void GetData(int Rno)
    {
		string Sql = string.Format(
            "Select i.*, u.UnitSingle, u.UnitPlural From " +
            "(Select i.*, (Select Top 1 PurchaseUnitRno From PurchaseDetails Where IngredRno = i.IngredRno Order By CreatedDtTm Desc) PurchaseUnitRno From Ingredients i Where IngredRno = {0}) " +
            "i Left Join Units u on i.PurchaseUnitRno = u.UnitRno", 
            Rno);

        ClearData();

		try
		{
			DataTable dt = db.DataTable(Sql);
			if (dt.Rows.Count > 0)
			{
				DataRow dr = dt.Rows[0];
				int IngredRno = DB.Int32(dr["IngredRno"]);
				txtRno.Value = IngredRno.ToString();
				txtName.Text = DB.Str(dr["Name"]);
				chkHide.Checked = DB.Bool(dr["HideFlg"]);
				chkStocked.Checked = DB.Bool(dr["StockedFlg"]);
				chkNonPurchase.Checked = DB.Bool(dr["NonPurchaseFlg"]);
				string DryLiquid = DB.Str(dr["DryLiquid"]);
                //rbDry.Checked = DryLiquid == Dry;
                //rbLiquid.Checked = DryLiquid == Liquid;
                decimal Qty = DB.Dec(dr["StockedPurchaseQty"]);
                txtStockedPurchaseQty.Text = Str.ShowFract(Qty);
                hfPrefVendors.Value = DB.Str(dr["PrefVendors"]);
				hfRejVendors.Value = DB.Str(dr["RejVendors"]);
				txtPrefBrands.Text = DB.Str(dr["PrefBrands"]);
				txtRejBrands.Text = DB.Str(dr["RejBrands"]);
				txtCreatedDt.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
				txtCreatedUser.Text = DB.Str(dr["CreatedUser"]);
				txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
				txtUpdatedUser.Text = DB.Str(dr["UpdatedUser"]);

				Sql = string.Format("Select Count(*) From RecipeIngredXref Where IngredRno = {0}", IngredRno);
				int Count = db.SqlNum(Sql);

				if (Count > 0)
				{
					btnDelete.Enabled = false;
					btnDelete.ToolTip = string.Format("Used in {0} Recipes", Count);
				}
				else
				{
					btnDelete.Enabled = true;
					btnDelete.ToolTip = "Not used in any Recipes";
				}

				FindRecipes(IngredRno);
				LoadConversions(IngredRno);
				PriceHistory(IngredRno);

				FocusField = "txtName";
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
    }

	protected void FindRecipes(int IngredRno)
	{
		string Sql = string.Format(
			"Select r.RecipeRno, r.Name, " +
			"(Select UnitSingle From Units Where UnitRno = x.UnitRno) As Unit " +
			"From RecipeIngredXref x Inner Join Recipes r On x.RecipeRno = r.RecipeRno " +
			"Where x.IngredRno = {0} " +
			"Order by r.Name",
			IngredRno);
		try
		{
			DataTable dt = db.DataTable(Sql);

			lblRecipes.Text = string.Format("Used in {0} recipes", dt.Rows.Count);
			lblRecipes.Visible = true;

			string Tip = string.Empty;
			ulRecipes.Controls.Clear();
			foreach (DataRow dr in dt.Rows)
			{
				string Name = DB.Str(dr["Name"]);
				string Unit = DB.Str(dr["Unit"]);

				Tip += string.Format("<li>{0} - {1}</li>\n", Name, Unit);

				HtmlAnchor a = new HtmlAnchor();
				a.HRef = string.Format("Recipes.aspx?Rno={0}", DB.Int32(dr["RecipeRno"]));
				a.InnerText = string.Format("{0} - {1}", Name, Unit);
				HtmlGenericControl li = new HtmlGenericControl("li");
				li.Controls.Add(a);
				ulRecipes.Controls.Add(li);
			}

			lblRecipes.ToolTip = string.Format("<ul>{0}</ul>", Tip);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}
	}

	protected void SaveData()
	{
		int Rno = Str.Num(txtRno.Value);
		cConversions = Str.Num(hfNumConversions.Value);
		string Sql = string.Empty;

		string DryLiquid = null;
		//if (rbDry.Checked) DryLiquid = Dry;
		//if (rbLiquid.Checked) DryLiquid = Liquid;

		if (Rno == 0)
		{
			Sql = string.Format(
				"Insert Into Ingredients (Name, CreatedDtTm, CreatedUser) Values ({0}, GetDate(), {1}); " +
				"Select Scope_Identity()",
				DB.PutStr(txtName.Text, 50),
				DB.PutStr(g.User));
			Rno = db.SqlNum(Sql);
		}
        bool fStockedItem = chkStocked.Checked;
        decimal StockedPurchaseQty = Str.Fract(txtStockedPurchaseQty.Text);
        Sql = string.Format(
			"Update Ingredients Set " +
			"Name = {1}, " + 
			"DryLiquid = {2}, " +
			"StockedFlg = {3}, " +
			"NonPurchaseFlg = {4}, " +
            "StockedPurchaseQty = {5}, " +
            "PrefVendors = {6}, " +
			"RejVendors = {7}, " +
			"PrefBrands = {8}, " +
			"RejBrands = {9}, " +
			"HideFlg = {10}, " +
			"UpdatedDtTm = GetDate(), " +
			"UpdatedUser = " + DB.PutStr(g.User) + " " +
			"Where IngredRno = {0}",
			Rno,
			DB.PutStr(txtName.Text, 50),
			(DryLiquid == null ? "Null" : DB.PutStr(DryLiquid)), 
			DB.PutBool(fStockedItem),
			DB.PutBool(chkNonPurchase.Checked),
            (fStockedItem && StockedPurchaseQty != 0 ? StockedPurchaseQty.ToString() : "Null"),
			DB.PutStr(hfPrefVendors.Value, 100),
			DB.PutStr(hfRejVendors.Value, 100),
			DB.PutStr(txtPrefBrands.Text, 1000),
			DB.PutStr(txtRejBrands.Text, 1000),
			(chkHide.Checked ? 1 : 0));
		try
		{
			db.Exec(Sql);

			fStartHere = true;
			StartHere = txtName.Text;

			RemoveConversions();
			SaveConversions(Rno);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}
	}

	protected void btnUpdate_Click(object sender, EventArgs e)
	{
		//SaveData();
		//ClearData();
		//LoadList();

		Update(false);
	}

	protected void btnUpdateNext_Click(object sender, EventArgs e)
	{
		Update(true);
	}

	protected void btnNew_Click(object sender, EventArgs e)
	{
		ClearData();
		LoadList();
		//LoadConversions();
		btnUpdate.Enabled =
		btnNext.Enabled = true;
	}

	protected void btnDelete_Click(object sender, EventArgs e)
	{
		int Rno = Str.Num(txtRno.Value); 
		string Sql = String.Format(
			"Delete From Ingredients Where IngredRno = {0}",
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
		StartHere = lstList.SelectedItem.Text;
		LoadList();
		btnUpdate.Enabled =
		btnNext.Enabled = true;
	}

	private void LoadList()
	{
		LoadList(false);
	}

	private void LoadList(bool fNext)
	{
		//Response.Write("fNext " + fNext.ToString()+ "<br/>");
		string Sql = "Select IngredRno, Name, HideFlg From Ingredients Where IsNull(MenuItemAsIsFlg, 0) = 0";
		if (!chkShowHidden.Checked) Sql += " And IsNull(HideFlg, 0) = 0";
		Sql += " Order by Name";

        string CurrItem = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Text : string.Empty);
		string NextCurrItem = (lstList.SelectedIndex >= 0 && lstList.SelectedIndex + 1 < lstList.Items.Count ? lstList.Items[lstList.SelectedIndex + 1].Text : string.Empty);
		//Response.Write("NextCurrItem [" + NextCurrItem + "]<br/>");
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

		lstList.Items.Clear();

		LoadListSql(Sql, NextCurrItem);
	}

	private void LoadListSql(string Sql, string NextCurrItem)
	{
		//Response.Write("looking for [" + NextCurrItem + "]<br/>");
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				Int32 Rno = DB.Int32(dr["IngredRno"]);
				String Name = DB.Str(dr["Name"]);
				bool fHidden = DB.Bool(dr["HideFlg"]);
				ListItem Item = new ListItem(Name, Rno.ToString());
				if (fHidden) Item.Attributes.Add("class", "Hidden");

				if (Item.Text == NextCurrItem)
				{
					Item.Selected = true;

					Rno = Str.Num(Item.Value);
					GetData(Rno);
					btnUpdate.Enabled =
					btnNext.Enabled = true;
				}

				lstList.Items.Add(Item);
			}

			lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Items";
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
		//LoadConversions();
	}

	private void AddLines()
	{
		for (int iConversion = 1; iConversion <= cConversions; iConversion++)
		{
			tblConv.Rows.Add(BuildConversionRow(iConversion.ToString()));
		}
	}

	private TableRow BuildConversionRow(string ID)
	{
		int cCells = 6;
		int iCell;
		TableRow rw = new TableRow();

		for (iCell = 0; iCell < cCells; iCell++)
		{
			rw.Cells.Add(new TableCell());
		}

		iCell = 0;
		rw.Cells[iCell++].Text = string.Empty;

		HtmlInputHidden hfIngredConvRno = new HtmlInputHidden();
		hfIngredConvRno.ID = "hfIngredConvRno" + ID;
		rw.Cells[iCell].Controls.Add(hfIngredConvRno);

		HiddenField hfRecipeUnitRno = new HiddenField();
		hfRecipeUnitRno.ID = "hfRecipeUnitRno" + ID;
		rw.Cells[iCell].Controls.Add(hfRecipeUnitRno);

		HiddenField hfPurchaseQty = new HiddenField();
		hfPurchaseQty.ID = "hfPurchaseQty" + ID;
		rw.Cells[iCell].Controls.Add(hfPurchaseQty);

		HiddenField hfPurchaseUnitRno = new HiddenField();
		hfPurchaseUnitRno.ID = "hfPurchaseUnitRno" + ID;
		rw.Cells[iCell].Controls.Add(hfPurchaseUnitRno);

		// Qty
		TextBox txtQty = new TextBox();
		txtQty.ID = "txtQty" + ID;
		txtQty.CssClass = "Qty";
		rw.Cells[iCell++].Controls.Add(txtQty);

		// Unit
		rw.Cells[iCell++].Text = string.Empty;

		// Purchase Qty
		rw.Cells[iCell].CssClass = "Qty";
		rw.Cells[iCell++].Text = string.Empty;

		// Purchase Unit
		rw.Cells[iCell++].Text = string.Empty;

		// Remove
		CheckBox chk = new CheckBox();
		chk.ID = "chkRemove" + ID;
		rw.Cells[iCell].CssClass = "Remove";
		rw.Cells[iCell++].Controls.Add(chk);

		return rw;
	}

	private void LoadConversions(int IngredRno)
	{
		if (IngredRno > 0)
		{
			String Sql = "";

			try
			{
				Sql = string.Format(
					"Select IngredConvRno, c.PurchaseQty, c.RecipeQty, c.MissingFlg, " +
					"pu.UnitSingle As PurchaseUnitSingle, pu.UnitPlural As PurchaseUnitPlural, " +
					"ru.UnitSingle As RecipeUnitSingle, ru.UnitPlural As RecipeUnitPlural " +
					"From IngredConv c " +
					"Left Join Units pu On c.PurchaseUnitRno = pu.UnitRno " +
					"Left Join Units ru On c.RecipeUnitRno = ru.UnitRno " +
					"Where c.IngredRno = {0} " +
					"Order By pu.UnitSingle",
					IngredRno);

				DataTable dt = db.DataTable(Sql);
				cConversions = dt.Rows.Count;

				// prep for finding missing conversions
				Ingred IngredData = new Ingred(IngredRno);
				Ingred.LoadIngredPurchases();
				Ingred.LoadIngredConversions();

				// find any recipe units that doesn't have an easy conversion to the last purchase unit
				string[] RecipeUnitRnos = { };
				Ingred.IngredPurchase Purchase = Ingred.LastPurchase(IngredRno);
				if (Purchase != null)
				{
					Sql = string.Format("Select Distinct UnitRno From RecipeIngredXref Where IngredRno = {0}", IngredRno);
					RecipeUnitRnos = db.StrArray(Sql);
					for (int i = 0; i < RecipeUnitRnos.Length; i++)
					{
						int RecipeUnitRno = Str.Num(RecipeUnitRnos[i]);
						if (IngredData.ConversionScaler(RecipeUnitRno, Purchase.UnitRno) == 0)
						{
							// needs conversion
							cConversions++;
						}
						else
						{
							// doesn't need conversion
							RecipeUnitRnos[i] = string.Empty;
						}
					}
				}

				AddLines();

				// existing conversions
				int iRow = 0;
				foreach (DataRow dr in dt.Rows)
				{
					iRow++;
					TableRow tr = tblConv.Rows[iRow + 1];

					HtmlInputHidden hfIngredConvRno = (HtmlInputHidden)tr.FindControl("hfIngredConvRno" + iRow);
					TextBox txtQty = (TextBox)tr.FindControl("txtQty" + iRow);

					int iCell = 0;
					tr.Cells[iCell++].Text = string.Empty;
					iCell++;

					hfIngredConvRno.Value = DB.Int32(dr["IngredConvRno"]).ToString();
					decimal Qty = DB.Dec(dr["RecipeQty"]);
					txtQty.Text = Str.ShowFract(Qty);
					tr.Cells[iCell++].Text = DB.Str(dr["Recipe" + (Qty <= 1 ? "UnitSingle" : "UnitPlural")]);
					Qty = DB.Dec(dr["PurchaseQty"]);
					tr.Cells[iCell++].Text = Str.ShowFract(Qty);
					tr.Cells[iCell++].Text = DB.Str(dr["Purchase" + (Qty <= 1 ? "UnitSingle" : "UnitPlural")]);
				}

				// needed new conversions
				for (int i = 0; i < RecipeUnitRnos.Length; i++)
				{
					int RecipeUnitRno = Str.Num(RecipeUnitRnos[i]);
					if (RecipeUnitRno != 0)
					{
						iRow++;
						TableRow tr = tblConv.Rows[iRow + 1];

						HtmlInputHidden hfIngredConvRno = (HtmlInputHidden)tr.FindControl("hfIngredConvRno" + iRow);
						TextBox txtQty = (TextBox)tr.FindControl("txtQty" + iRow);
						HiddenField hfRecipeUnitRno = (HiddenField)tr.FindControl("hfRecipeUnitRno" + iRow);
						HiddenField hfPurchaseQty = (HiddenField)tr.FindControl("hfPurchaseQty" + iRow);
						HiddenField hfPurchaseUnitRno = (HiddenField)tr.FindControl("hfPurchaseUnitRno" + iRow);

						int iCell = 0;

						hfIngredConvRno.Value = "0";
						txtQty.Text = "";
						hfRecipeUnitRno.Value = RecipeUnitRno.ToString();
						hfPurchaseQty.Value = (Purchase.Qty * Purchase.UnitQty).ToString();
						hfPurchaseUnitRno.Value = Purchase.UnitRno.ToString();
						tr.Cells[iCell++].Text = "<span class=\"NeedConv\"><i class=\"icon-warning-sign\" /> Enter missing quantity</span>";
						iCell++;
						tr.Cells[iCell++].Text = Ingred.Unit(RecipeUnitRno);
						tr.Cells[iCell++].Text = Str.ShowFract(Purchase.Qty * Purchase.UnitQty);
						tr.Cells[iCell++].Text = Ingred.Unit(Purchase.UnitRno);
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

	private void RemoveConversions()
	{
		for (int iConversion = 1; iConversion <= cConversions; iConversion++)
		{
			bool fRemove = Parm.Bool("chkRemove" + iConversion);
			if (fRemove)
			{
				String Sql = "";
				try
				{
					int IngredConvRno = Parm.Int("hfIngredConvRno" + iConversion);
					Sql = "Delete From IngredConv Where IngredConvRno = " + IngredConvRno;
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

	private void SaveConversions(int IngredRno)
	{
		for (int iConversion = 1; iConversion <= cConversions; iConversion++)
		{
			int IngredConvRno = Parm.Int("hfIngredConvRno" + iConversion);
			decimal Qty = Str.Fract(Parm.Str("txtQty" + iConversion));
			int RecipeUnitRno = Parm.Int("hfRecipeUnitRno" + iConversion);
			decimal PurchaseQty = Parm.Dec("hfPurchaseQty" + iConversion);
			int PurchaseUnitRno = Parm.Int("hfPurchaseUnitRno" + iConversion);
			string Sql = string.Empty;

			bool fRemove = Parm.Bool("chkRemove" + iConversion);
			if (!fRemove)
			{
				try
				{
					if (Qty > 0)
					{
						if (IngredConvRno == 0)
						{
							Sql = string.Format(
								"Insert Into IngredConv (IngredRno, PurchaseQty, PurchaseUnitRno, RecipeUnitRno, CreatedDtTm, CreatedUser) Values (" +
								"{0}, {1}, {2}, {3}, GetDate(), {4}); " +
								"Select Scope_Identity()",
								IngredRno,
								PurchaseQty,
								PurchaseUnitRno,
								RecipeUnitRno,
								DB.PutStr(g.User));
							IngredConvRno = db.SqlNum(Sql);
						}

						Sql = string.Format(
							"Update IngredConv Set " +
							"RecipeQty = {1}, " +
							"UpdatedDtTm = GetDate(), " +
							"UpdatedUser = {2} " +
							"Where IngredConvRno = {0}",
							IngredConvRno,
							Qty,
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

	private void PriceHistory(int IngredRno)
	{
		string Sql = string.Format(
			"Select Top 15 p.PurchaseRno, d.PurchaseDetailRno, p.PurchaseDt, v.Name, d.PurchaseQty, d.PurchaseUnitQty, u.UnitSingle, u.UnitPlural, Price\n " + 
			"From PurchaseDetails d\n " + 
			"Inner Join Purchases p On p.PurchaseRno = d.PurchaseRno\n " + 
			"Inner Join Vendors v On v.VendorRno = p.VendorRno\n " + 
			"Inner Join Units u On u.UnitRno = d.PurchaseUnitRno\n " +
			"Where IngredRno = {0}\n " + 
			"Order By PurchaseDt Desc",
			IngredRno);
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				decimal PurchaseQty = DB.Dec(dr["PurchaseQty"]);
				decimal PurchaseUnitQty = DB.Dec(dr["PurchaseUnitQty"]);
				decimal Price = DB.Dec(dr["Price"]);
				TableRow tr = new TableRow();
				tr.Cells.Add(new TableCell());
				tr.Cells.Add(new TableCell() { Text = Fmt.Dt(DB.DtTm(dr["PurchaseDt"])) });
				tr.Cells.Add(new TableCell() { Text = DB.Str(dr["Name"]) });
				tr.Cells.Add(new TableCell() { Text = Str.ShowFract(PurchaseQty), CssClass="Qty" });
				tr.Cells.Add(new TableCell() { Text = string.Format("{0} {1}", Str.ShowFract(PurchaseUnitQty), DB.Str(dr[PurchaseUnitQty <= 1 ? "UnitSingle" : "UnitPlural"])) });
				tr.Cells.Add(new TableCell() { Text = Fmt.Dollar(Price), CssClass="Price" });
				tr.Cells.Add(new TableCell() { Text = Fmt.Dollar(Math.Round((PurchaseQty != 0 ? Price / PurchaseQty : 0), 2)), CssClass = "Price" });
				tblPrices.Rows.Add(tr);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

    protected string UnitData()
    {
        return Misc.UnitData(db);
    }
}
