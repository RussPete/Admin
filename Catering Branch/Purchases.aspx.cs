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

public partial class Purchases : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";
	protected int cDetails;
	protected int NewIngredRno = 0;

	private int StartShoppingHere;
	private int StartHere;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.Shopping.Title;

			// if an ingred rno passed in then well setup for a new receipt for the ingredient on today with No Vendor
			NewIngredRno = Parm.Int("IngredRno");
			if (NewIngredRno != 0)
			{
				IngredReceipt(NewIngredRno);
				LoadList();
                hfTab.Value = "1";
			}
			else
			{
				Setup();
			}
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
	}

	private void Setup()
	{
		ClearData();
        LoadShoppingList();
		LoadList();
        hfTab.Value = "0";
		cDetails = 1;
		AddLines();
	}

	protected void ClearData()
	{
        hfShoppingListRno.Value =
		hfRno.Value =
		txtPurchaseDate.Text = 
		txtVendor.Text = 
		txtOrderNum.Text =
		txtVendorInvoice.Text =
		txtCreatedDt.Text =
		txtCreatedUser.Text =
		txtUpdatedDt.Text =
		txtUpdatedUser.Text = string.Empty;

		btnDelete.Enabled = false;
		btnDelete.ToolTip = "New purchase record";

		txtPurchaseDate.Text = Fmt.Dt(DateTime.Today);
	}

    protected void GetShoppingData(int Rno)
    {
        string Sql = string.Format(
            "Select * " +
            "From ShoppingLists s Inner Join Vendors v On s.VendorRno = v.VendorRno " +
            "Where ShoppingListRno = {0}",
            Rno);

        ClearData();

        try
        {
            DataRow dr = db.DataRow(Sql);
            if (dr != null)
            {
                hfShoppingListRno.Value = Rno.ToString();
                txtPurchaseDate.Text = Fmt.Dt(DateTime.Today);
                hfVendorRno.Value = DB.Int32(dr["VendorRno"]).ToString();
                txtVendor.Text = DB.Str(dr["Name"]);

                btnDelete.Enabled = false;

                FocusField = "txtPurchaseDate";

                LoadShoppingDetails(Rno);
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    protected void GetData(int Rno)
	{
		string Sql = string.Format(
			"Select * " +
			"From Purchases p Inner Join Vendors v On p.VendorRno = v.VendorRno " +
			"Where PurchaseRno = {0}",
			Rno);

		ClearData();

		try
		{
			DataRow dr = db.DataRow(Sql);
			if (dr != null)
			{
				hfRno.Value = Rno.ToString();
				txtPurchaseDate.Text = Fmt.Dt(DB.DtTm(dr["PurchaseDt"]));
				hfVendorRno.Value = DB.Int32(dr["VendorRno"]).ToString();
				txtVendor.Text = DB.Str(dr["Name"]);
				txtOrderNum.Text = DB.Str(dr["PurchaseOrdNum"]);
				txtVendorInvoice.Text = DB.Str(dr["VendorInvNum"]);
				txtCreatedDt.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
				txtCreatedUser.Text = DB.Str(dr["CreatedUser"]);
				txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
				txtUpdatedUser.Text = DB.Str(dr["UpdatedUser"]);

				btnDelete.Enabled = true;

				FocusField = "txtPurchaseDate";

				LoadDetails(Rno);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected int SaveData()
	{
        int ShoppingListRno = Str.Num(hfShoppingListRno.Value);
        int Rno = Str.Num(hfRno.Value);
		cDetails = Str.Num(hfNumDetails.Value);
		string Sql = string.Empty;
		try
		{
			int VendorRno = Parm.Int("hfVendorRno");
			if (Rno == 0)
			{
				Sql = string.Format(
					"Insert Into Purchases (VendorRno, CreatedDtTm, CreatedUser) Values ({0}, GetDate(), {1}); " +
					"Select Scope_Identity()",
					VendorRno,
					DB.PutStr(g.User));
				Rno = db.SqlNum(Sql);
			}

			Sql = string.Format(
				"Update Purchases Set " +
				"VendorRno = {1}, " +
				"PurchaseDt = {2}, " +
				"PurchaseOrdNum = {3}, " +
				"VendorInvNum = {4}, " +
				"UpdatedDtTm = GetDate(), " +
				"UpdatedUser = {5} " +
				"Where PurchaseRno = {0}",
				Rno,
				VendorRno,
				DB.PutDtTm(Parm.DtTm("txtPurchaseDate")),
				DB.PutStr(txtOrderNum.Text, 20),
				DB.PutStr(txtVendorInvoice.Text, 20),
				DB.PutStr(g.User));
			db.Exec(Sql);

			StartHere = Rno;

			RemoveDetails();
			SaveDetails(Rno);

            // cleanup shopping list
            if (ShoppingListRno != 0)
            {
                Sql = string.Format(
                    "Delete From ShoppingListDetails Where ShoppingListRno = {0};\n" +
                    "Delete From ShoppingLists Where ShoppingListRno = {0};",
                    ShoppingListRno);
                db.Exec(Sql);
                LoadShoppingList();
            }
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}

		return Rno;
	}

    private void LoadShoppingList()
    {
        string Sql =
            "Select s.ShoppingListRno, s.BegDt, v.Name As Vendor  " +
            "From ShoppingLists s Inner Join Vendors v On s.VendorRno = v.VendorRno";
        Sql += " Order by " + (rbShoppingDate.Checked ? "s.BegDt, v.Name" : "v.Name, s.BegDt");

        string CurrItem = (lstShoppingList.SelectedIndex >= 0 ? lstShoppingList.SelectedItem.Value : "");

        lstShoppingList.Items.Clear();

        LoadShoppingListSql(Sql, CurrItem);

        lblShoppingRecCount.Text = Fmt.Num(lstShoppingList.Items.Count, true) + " Shopping Lists";

        //btnSave.Enabled = (CurrItem.Length > 0);
    }

    private void LoadShoppingListSql(string Sql, string NextCurrItem)
    {
        //Response.Write("NextCurr [" + NextCurrItem + "]<br/>");
        try
        {
            DataTable dt = db.DataTable(Sql);
            //Response.Write(string.Format("Num Rows {0}<br/>", dt.Rows.Count));
            foreach (DataRow dr in dt.Rows)
            {
                Int32 Rno = DB.Int32(dr["ShoppingListRno"]);
                DateTime dtBeg = DB.DtTm(dr["BegDt"]);
                String Vendor = DB.Str(dr["Vendor"]);
                ListItem Item = new ListItem(string.Format("{0} - {1}", Fmt.Dt(dtBeg), Vendor), Rno.ToString());
                Item.Selected = (Item.Value == NextCurrItem);
                lstShoppingList.Items.Add(Item);
            }
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

    private void LoadList()
	{
		string Sql =
			"Select p.PurchaseRno, p.PurchaseDt, v.Name As Vendor  " +
			"From Purchases p Inner Join Vendors v On p.VendorRno = v.VendorRno";
		Sql += " Order by " + (rbDate.Checked ? "p.PurchaseDt Desc, v.Name" : "v.Name, p.PurchaseDt Desc");

		string CurrItem = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : "");

		lstList.Items.Clear();

		LoadListSql(Sql, CurrItem);

		lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Purchases";

		//btnSave.Enabled = (CurrItem.Length > 0);
	}

	private void LoadListSql(string Sql, string NextCurrItem)
	{
		//Response.Write("NextCurr [" + NextCurrItem + "]<br/>");
		try
		{
			DataTable dt = db.DataTable(Sql);
			//Response.Write(string.Format("Num Rows {0}<br/>", dt.Rows.Count));
			foreach (DataRow dr in dt.Rows)
			{
				Int32 Rno = DB.Int32(dr["PurchaseRno"]);
				DateTime dtPurchase = DB.DtTm(dr["PurchaseDt"]);
				String Vendor = DB.Str(dr["Vendor"]);
				ListItem Item = new ListItem(string.Format("{0} - {1}", Fmt.Dt(dtPurchase), Vendor), Rno.ToString());
				Item.Selected = (Item.Value == NextCurrItem);
				lstList.Items.Add(Item);
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

    protected void UpdateShoppingList(object sender, System.EventArgs e)
    {
        LoadShoppingList();
    }

    protected void UpdateList(object sender, System.EventArgs e)
	{
		LoadList();
	}

	protected void Update()
	{
		int Rno = SaveData();
		LoadList();
		LoadDetails(Rno);
	}

	private void AddLines()
	{
		//Response.Write(string.Format("AddLines {0}<br/>", cDetails));
		for (int iDetail = 1; iDetail <= cDetails; iDetail++)
		{
			TableRow tr = BuildDetailRow(iDetail.ToString());
			tblDetails.Rows.Add(tr);

			// clean up some leftover properties
			TextBox txt = (TextBox)tr.FindControl("txtUnit" + iDetail);
			txt.Attributes.Clear();
		}
	}

	protected string AddRowHtml()
	{
		TableRow rw = BuildDetailRow("~ID~");

		StringBuilder sb = new StringBuilder();
		rw.RenderControl(new HtmlTextWriter(new StringWriter(sb)));

		return sb.ToString().Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "\\\"");
	}

	private TableRow BuildDetailRow(string ID)
	{
		int cCells = 6;
		int iCell;
		TableRow rw = new TableRow();

		for (iCell = 0; iCell < cCells; iCell++)
		{
			rw.Cells.Add(new TableCell());
		}

		iCell = 0;

		HtmlInputHidden hfPurchaseDetailRno = new HtmlInputHidden();
		hfPurchaseDetailRno.ID = "hfPurchaseDetailRno" + ID;
		rw.Cells[iCell].Controls.Add(hfPurchaseDetailRno);

		// Original Ingredient Rno
		HtmlInputHidden hfOrigIngredRno = new HtmlInputHidden();
		hfOrigIngredRno.ID = "hfOrigIngredRno" + ID;
		hfOrigIngredRno.Attributes.Add("class", "OrigIngredRno");
		rw.Cells[iCell].Controls.Add(hfOrigIngredRno);

		// Ingredient Rno
		HtmlInputHidden hfIngredRno = new HtmlInputHidden();
		hfIngredRno.ID = "hfIngredRno" + ID;
		hfIngredRno.Attributes.Add("class", "IngredRno");
		rw.Cells[iCell].Controls.Add(hfIngredRno);

		// Stocked
		HtmlInputHidden hfStocked = new HtmlInputHidden();
		hfStocked.ID = "hfStocked" + ID;
		hfStocked.Attributes.Add("class", "Stocked");
		rw.Cells[iCell].Controls.Add(hfStocked);

		// Ingredient
		TextBox txtIngredient = new TextBox();
		txtIngredient.ID = "txtIngredient" + ID;
		txtIngredient.CssClass = "Ingredient";
		rw.Cells[iCell++].Controls.Add(txtIngredient);

		// Purchase Qty
		TextBox txtPurchaseQty = new TextBox();
		txtPurchaseQty.ID = "txtPurchaseQty" + ID;
		txtPurchaseQty.CssClass = "Qty";
		rw.Cells[iCell++].Controls.Add(txtPurchaseQty);

		// Unit Qty
		TextBox txtUnitQty = new TextBox();
		txtUnitQty.ID = "txtUnitQty" + ID;
		txtUnitQty.CssClass = "Qty UnitQty";
		rw.Cells[iCell++].Controls.Add(txtUnitQty);

		// Unit
		HtmlInputHidden hfUnitRno = new HtmlInputHidden();
		hfUnitRno.ID = "hfUnitRno" + ID;
		hfUnitRno.Attributes.Add("class", "UnitRno");
		rw.Cells[iCell].Controls.Add(hfUnitRno);

		TextBox txtUnit = new TextBox();
		txtUnit.ID = "txtUnit" + ID;
		txtUnit.CssClass = "Unit";
		rw.Cells[iCell++].Controls.Add(txtUnit);

		// Price
		TextBox txtPrice = new TextBox();
		txtPrice.ID = "txtPrice" + ID;
		txtPrice.CssClass = "Price";
		rw.Cells[iCell++].Controls.Add(txtPrice);

		// Remove
		CheckBox chkRemove = new CheckBox();
		chkRemove.ID = "chkRemove" + ID;
		chkRemove.CssClass = "Remove";
		chkRemove.TabIndex = -1;
		rw.Cells[iCell++].Controls.Add(chkRemove);

		return rw;
	}

    protected void lstShoppingList_SelectedIndexChanged(object sender, System.EventArgs e)
    {
        int Rno = Str.Num(lstShoppingList.SelectedItem.Value);
        GetShoppingData(Rno);
        StartShoppingHere = Rno;
        LoadShoppingList();
        btnSave.Enabled = true;
        hfTab.Value = "0";
    }

    protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		int Rno = Str.Num(lstList.SelectedItem.Value);
		GetData(Rno);
		StartHere = Rno;
		LoadList();
		btnSave.Enabled = true;
        hfTab.Value = "1";
    }

    protected void btnSave_Click(object sender, EventArgs e)
	{
		Update();
	}

	protected void btnNew_Click(object sender, System.EventArgs e)
	{
		ClearData();
		LoadDetails(0);
	}

	protected void btnDelete_Click(object sender, EventArgs e)
	{
		int Rno = Str.Num(hfRno.Value);
		string Sql = String.Format(
			"Select IngredRno From PurchaseDetails Where PurchaseRno = {0}",
			Rno);

		try
		{
			// prep for recalculating the latest price for all the ingredients on this receipt that is being deleted
			DataTable dt = db.DataTable(Sql);

			// delete the receipt
			Sql = String.Format(
				"Delete From PurchaseDetails Where PurchaseRno = {0}; " +
				"Delete From Purchases Where PurchaseRno = {0}; ",
				Rno);	
			db.Exec(Sql);

			// now recalc the latest prices
			foreach (DataRow dr in dt.Rows)
			{
				int IngredRno = DB.Int32(dr["IngredRno"]);

				Ingred.UpdateWithLastPrice(IngredRno);
			}

			LoadList();

			ClearData();
			LoadDetails(0);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	private void LoadShoppingDetails(int Rno)
	{
		//Response.Write(string.Format("LoadDetails Rno {0}<br/>", Rno));
		if (Rno > 0)
		{
			String Sql = "";

			try
			{
				Sql = string.Format(
					"Select d1.ShoppingListDetailRno " +
                    "From ShoppingListDetails d1 Inner Join ShoppingListDetails d2 " +
                    "On d1.ShoppingListRno = d2.ShoppingListRno And d1.ShoppingListDetailRno <> d2.ShoppingListDetailRno " +
                    "Where d1.ShoppingListRno = {0} " +
                    "And d1.ShoppingListDetailRno = d2.ShoppingListDetailRno",
					Rno);
				DupSeq Dups = new DupSeq(db, Sql);

				Sql = string.Format(
					"Select d.*, i.Name, i.MenuItemAsIsFlg, u.UnitSingle, u.UnitPlural " +
                    "From ShoppingListDetails d " +
					"Inner Join Ingredients i On d.IngredRno = i.IngredRno " +
					"Inner Join Units u On d.PurchaseUnitRno = u.UnitRno " +
                    "Where ShoppingListRno = {0} Order By ShoppingListDetailRno", Rno);
				DataTable dt = db.DataTable(Sql);
				cDetails = dt.Rows.Count + 1;
				//Response.Write(string.Format("dt rows {0} cDetails {1}<br/>", dt.Rows.Count, cDetails));
				//Response.Write(Sql + "<br/>");
				AddLines();

				int iRow = 0;
				foreach (DataRow dr in dt.Rows)
				{
					iRow++;
					TableRow tr = tblDetails.Rows[iRow];

					int ShoppingListDetailRno = DB.Int32(dr["ShoppingListDetailRno"]);
					bool fDup = Dups.In(ShoppingListDetailRno);

					HtmlInputHidden hfPurchaseDetailRno = (HtmlInputHidden)tr.FindControl("hfPurchaseDetailRno" + iRow);
					HtmlInputHidden hfOrigIngredRno = (HtmlInputHidden)tr.FindControl("hfOrigIngredRno" + iRow);
					HtmlInputHidden hfIngredRno = (HtmlInputHidden)tr.FindControl("hfIngredRno" + iRow);
					TextBox txtIngredient = (TextBox)tr.FindControl("txtIngredient" + iRow);
					TextBox txtPurchaseQty = (TextBox)tr.FindControl("txtPurchaseQty" + iRow);
					TextBox txtUnitQty = (TextBox)tr.FindControl("txtUnitQty" + iRow);
					HtmlInputHidden hfUnitRno = (HtmlInputHidden)tr.FindControl("hfUnitRno" + iRow);
					TextBox txtUnit = (TextBox)tr.FindControl("txtUnit" + iRow);
					TextBox txtPrice = (TextBox)tr.FindControl("txtPrice" + iRow);
					CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iRow);

					hfPurchaseDetailRno.Value = string.Empty;
					hfOrigIngredRno.Value = 
					hfIngredRno.Value = DB.Int32(dr["IngredRno"]).ToString();
					txtIngredient.Text = DB.Str(dr["Name"]);
					decimal Qty = DB.Dec(dr["PurchaseQty"]);
                    Qty = Math.Ceiling(Qty);
					txtPurchaseQty.Text = Str.ShowFract(Qty);
					txtUnitQty.Text = Str.ShowFract(DB.Dec(dr["PurchaseUnitQty"]));
					hfUnitRno.Value = DB.Int32(dr["PurchaseUnitRno"]).ToString();
					txtUnit.Text = DB.Str(dr[Qty <= 1 ? "UnitSingle" : "UnitPlural"]);
					if (DB.Bool(dr["MenuItemAsIsFlg"]))
					{
						txtUnit.Attributes.Add("disabled", "true");
					}
					//txtPrice.Text = Fmt.Dollar(DB.Dec(dr["Price"]));
					chkRemove.Checked = false;
				}

				hfNumDetails.Value = cDetails.ToString();
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}
		else
		{
			cDetails = 1;
			AddLines();
		}
	}

    private void LoadDetails(int Rno)
    {
        //Response.Write(string.Format("LoadDetails Rno {0}<br/>", Rno));
        if (Rno > 0)
        {
            String Sql = "";

            try
            {
                Sql = string.Format(
                    "Select d1.PurchaseDetailRno " +
                    "From PurchaseDetails d1 Inner Join PurchaseDetails d2 " +
                    "On d1.PurchaseRno = d2.PurchaseRno And d1.PurchaseDetailRno <> d2.PurchaseDetailRno " +
                    "Where d1.PurchaseRno = {0} " +
                    "And d1.PurchaseDetailRno = d2.PurchaseDetailRno",
                    Rno);
                DupSeq Dups = new DupSeq(db, Sql);

                Sql = string.Format(
                    "Select d.*, i.Name, i.MenuItemAsIsFlg, u.UnitSingle, u.UnitPlural " +
                    "From PurchaseDetails d " +
                    "Inner Join Ingredients i On d.IngredRno = i.IngredRno " +
                    "Inner Join Units u On d.PurchaseUnitRno = u.UnitRno " +
                    "Where PurchaseRno = {0} Order By PurchaseDetailRno", Rno);
                DataTable dt = db.DataTable(Sql);
                cDetails = dt.Rows.Count + 1;
                //Response.Write(string.Format("dt rows {0} cDetails {1}<br/>", dt.Rows.Count, cDetails));
                //Response.Write(Sql + "<br/>");
                AddLines();

                int iRow = 0;
                foreach (DataRow dr in dt.Rows)
                {
                    iRow++;
                    TableRow tr = tblDetails.Rows[iRow];

                    int PurchaseDetailRno = DB.Int32(dr["PurchaseDetailRno"]);
                    bool fDup = Dups.In(PurchaseDetailRno);

                    HtmlInputHidden hfPurchaseDetailRno = (HtmlInputHidden)tr.FindControl("hfPurchaseDetailRno" + iRow);
                    HtmlInputHidden hfOrigIngredRno = (HtmlInputHidden)tr.FindControl("hfOrigIngredRno" + iRow);
                    HtmlInputHidden hfIngredRno = (HtmlInputHidden)tr.FindControl("hfIngredRno" + iRow);
                    TextBox txtIngredient = (TextBox)tr.FindControl("txtIngredient" + iRow);
                    TextBox txtPurchaseQty = (TextBox)tr.FindControl("txtPurchaseQty" + iRow);
                    TextBox txtUnitQty = (TextBox)tr.FindControl("txtUnitQty" + iRow);
                    HtmlInputHidden hfUnitRno = (HtmlInputHidden)tr.FindControl("hfUnitRno" + iRow);
                    TextBox txtUnit = (TextBox)tr.FindControl("txtUnit" + iRow);
                    TextBox txtPrice = (TextBox)tr.FindControl("txtPrice" + iRow);
                    CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iRow);

                    hfPurchaseDetailRno.Value = PurchaseDetailRno.ToString();
                    hfOrigIngredRno.Value =
                    hfIngredRno.Value = DB.Int32(dr["IngredRno"]).ToString();
                    txtIngredient.Text = DB.Str(dr["Name"]);
                    decimal Qty = DB.Dec(dr["PurchaseQty"]);
                    txtPurchaseQty.Text = Str.ShowFract(Qty);
                    txtUnitQty.Text = Str.ShowFract(DB.Dec(dr["PurchaseUnitQty"]));
                    hfUnitRno.Value = DB.Int32(dr["PurchaseUnitRno"]).ToString();
                    txtUnit.Text = DB.Str(dr[Qty <= 1 ? "UnitSingle" : "UnitPlural"]);
                    if (DB.Bool(dr["MenuItemAsIsFlg"]))
                    {
                        txtUnit.Attributes.Add("disabled", "true");
                    }
                    txtPrice.Text = Fmt.Dollar(DB.Dec(dr["Price"]));
                    chkRemove.Checked = false;
                }

                hfNumDetails.Value = cDetails.ToString();
            }
            catch (Exception Ex)
            {
                Err Err = new Err(Ex, Sql);
                Response.Write(Err.Html());
            }
        }
        else
        {
            cDetails = 1;
            AddLines();
        }
    }

    private void RemoveDetails()
	{
		for (int iDetail = 1; iDetail <= cDetails; iDetail++)
		{
			bool fRemove = Parm.Bool("chkRemove" + iDetail);
			if (fRemove)
			{
				String Sql = "";
				try
				{
					int PurchaseDetailRno = Parm.Int("hfPurchaseDetailRno" + iDetail);
					Sql = "Delete From PurchaseDetails Where PurchaseDetailRno = " + PurchaseDetailRno;
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

	private void SaveDetails(int Rno)
	{
		// force a refresh of last purchase prices
		Ingred.LoadIngredPurchases();

		for (int iDetail = 1; iDetail <= cDetails; iDetail++)
		{
			bool fRemove = Parm.Bool("chkRemove" + iDetail);
			int OrigIngredRno = Parm.Int("hfOrigIngredRno" + iDetail);
			int IngredRno = Parm.Int("hfIngredRno" + iDetail);

			if (!fRemove)
			{
				int PurchaseDetailRno = Parm.Int("hfPurchaseDetailRno" + iDetail);
				bool fNewRec = (PurchaseDetailRno == 0);
				bool fStockedFlg = Parm.Bool("hfStocked" + iDetail);
				string Ingredient = Parm.Str("txtIngredient" + iDetail);
				decimal PurchaseQty = Str.Fract(Parm.Str("txtPurchaseQty" + iDetail));
				decimal UnitQty = Str.Fract(Parm.Str("txtUnitQty" + iDetail));
				int UnitRno = Parm.Int("hfUnitRno" + iDetail);
				decimal Price = (Parm.Dec("txtPrice" + iDetail));

				DateTime Tm = DateTime.Now;
				String Sql = "";

				try
				{
					// if a new ingredient, create it
					if (Ingredient.Length > 0)
					{
						if (IngredRno == 0)
						{
							// first off, lets see if there is an ingredient with this name, just in case there already is one, probably hidden
							Sql = string.Format("Select IngredRno, HideFlg From Ingredients Where Name = {0}", DB.PutStr(Ingredient));
							DataRow dr = db.DataRow(Sql);
							if (dr != null)
							{
								// the ingredient does indeed exist
								IngredRno = DB.Int32(dr["IngredRno"]);
								bool fHide = DB.Bool(dr["HideFlg"]);
								if (fHide)
								{
									// it is hidden, so now unhide it
									Sql = string.Format("Update Ingredients Set HideFlg = 0 Where IngredRno = {0}", IngredRno);
									db.Exec(Sql);
								}
							}
							else
							{
								// the indgredient does indeed not exist, so create it
								Sql = string.Format(
									"Insert Into Ingredients (Name, StockedFlg, CreatedDtTm, CreatedUser) " +
									"Values ({0}, {1}, GetDate(), {2});" +
									"Select Scope_Identity()",
									DB.PutStr(Ingredient),
									DB.PutBool(fStockedFlg),
									DB.PutStr(g.User));
								IngredRno = db.SqlNum(Sql);
							}
						}

						if (PurchaseDetailRno == 0)
						{
							Sql = string.Format(
								"Insert Into PurchaseDetails (PurchaseRno, IngredRno, CreatedDtTm, CreatedUser) " +
								"Values ({0}, {1}, GetDate(), {2});" +
								"Select Scope_Identity()",
								Rno,
								IngredRno,
								DB.PutStr(g.User));
							PurchaseDetailRno = db.SqlNum(Sql);
						}

						Sql = string.Format(
							"Update PurchaseDetails Set " +
							"IngredRno = {1}, " +
							"PurchaseQty = {2}, " +
							"PurchaseUnitQty = {3}, " +
							"PurchaseUnitRno = {4}, " +
							"Price = {5}, " +
							"UpdatedDtTm = GetDate(), " +
							"UpdatedUser = {6} " +
							"Where PurchaseDetailRno = {0}",
							PurchaseDetailRno,
							IngredRno,
							PurchaseQty,
							UnitQty,
							UnitRno,
							Price,
							DB.PutStr(g.User));
						db.Exec(Sql);

						// update the ingredient prices
						Ingred.UpdateWithLastPrice(IngredRno);
					}
				}
				catch (Exception Ex)
				{
					Err Err = new Err(Ex, Sql);
					Response.Write(Err.Html());
				}
			}

			// if the deleted or changed, update the original ingredient prices
			if (fRemove || IngredRno != OrigIngredRno)
			{
				Ingred.UpdateWithLastPrice(OrigIngredRno);
			}
		}
	}

	protected string VendorData()
	{
		string Sql = "Select VendorRno, Name From Vendors Where IsNull(HideFlg, 0) = 0 Order By Name";
		StringBuilder sb = new StringBuilder();

		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow dr in dt.Rows)
			{
				sb.AppendFormat("{{label:'{0}',id:{1}}},", DB.Str(dr["Name"]).Replace("'", "\\'"), DB.Int32(dr["VendorRno"]));
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}

		return string.Format("[{0}]", sb.ToString());
	}

	protected string IngredientData()
	{
		string Sql = 
			"With Details As\n" +
			"(\n" +
			"	Select\n" +
			"	i.IngredRno, i.Name, IsNull(i.MenuItemAsIsFlg, 0) AsIsFlg, p.VendorRno, pd.PurchaseQty, pd.PurchaseUnitQty, PurchaseUnitRno, pd.Price, p.PurchaseDt,\n" +
			"	Row_Number() Over(Partition By i.IngredRno, p.VendorRno Order By p.PurchaseDt Desc) As Seq\n" +
			"	From Ingredients i\n" +
			"	Left Join PurchaseDetails pd On i.IngredRno = pd.IngredRno\n" +
			"	Left Join Purchases p On pd.PurchaseRno = p.PurchaseRno\n" +
			"	Where (IsNull(i.HideFlg, 0) = 0 Or IsNull(MenuItemAsIsFlg, 0) = 1) And IsNull(NonPurchaseFlg, 0) = 0\n" +
			")\n" +
			"Select *\n" +
			"From Details Where Seq = 1\n" +
			"Order By Name, IngredRno, VendorRno\n";
		StringBuilder sb = new StringBuilder();

		try
		{
			DataTable dt = db.DataTable(Sql);
			int PrevIngredRno = 0;
			string PrevName = string.Empty;
			bool PrevAsIs = false;

			StringBuilder sbVendors = new StringBuilder();
			foreach (DataRow dr in dt.Rows)
			{
				int IngredRno = DB.Int32(dr["IngredRno"]);
				if (IngredRno != PrevIngredRno)
				{
					if (PrevIngredRno != 0)
					{
						sb.AppendFormat("{{label:'{0}',id:{1},prev:[{2}],asis:{3}}},", 
							PrevName.Replace(@"\", @"\\").Replace("'", @"\'"), 
							PrevIngredRno, 
							sbVendors.ToString().Remove(sbVendors.Length - 1, 1), 
							PrevAsIs.ToString().ToLower());
					}

					sbVendors.Length = 0;
					PrevIngredRno = IngredRno;
					PrevName = DB.Str(dr["Name"]);
					PrevAsIs = DB.Bool(dr["AsIsFlg"]);
				}

				sbVendors.AppendFormat(
					"{{vendor:{0},qty:{1},unitQty:{2},unitRno:{3},price:{4}}},", 
					DB.Int32(dr["VendorRno"]), 
					DB.Dec(dr["PurchaseQty"]), 
					DB.Dec(dr["PurchaseUnitQty"]), 
					DB.Int32(dr["PurchaseUnitRno"]), 
					DB.Dec(dr["Price"]));
			}
			if (PrevIngredRno != 0)
			{
				sb.AppendFormat("{{label:'{0}',id:{1},prev:[{2}],asis:{3}}}", 
					PrevName.Replace(@"\", @"\\").Replace("'", @"\'"), 
					PrevIngredRno, 
					sbVendors.ToString().Remove(sbVendors.Length - 1, 1), 
					PrevAsIs.ToString().ToLower());
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex);
			Response.Write(Err.Html());
		}

		return string.Format("[{0}]", sb.ToString());
	}

	private void IngredReceipt(int IngredRno)
	{
		string Sql = string.Empty;
		try
		{
			ClearData();

			// find the No Vendor 
			Sql = "Select Top 1 VendorRno From Vendors Where Name Like '%No Vendor%'";
			int VendorRno = db.SqlNum(Sql);
			if (VendorRno != 0)
			{
				// find a purchase for the No Vendor today
				Sql = string.Format("Select Top 1 PurchaseRno From Purchases Where VendorRno = {0} And PurchaseDt = {1}", VendorRno, DB.PutDtTm(DateTime.Today));
				int PurchaseRno = db.SqlNum(Sql);
				if (PurchaseRno == 0)
				{
					// a purchase wasn't found so create one
					Sql = string.Format(
						"Insert Into Purchases (VendorRno, PurchaseDt, CreatedDtTm, CreatedUser) Values ({0}, {1}, GetDate(), {2}); Select Scope_Identity();", 
						VendorRno,
						DB.PutDtTm(DateTime.Today),
						DB.PutStr(g.User));
					PurchaseRno = db.SqlNum(Sql);
				}

				GetData(PurchaseRno);
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

