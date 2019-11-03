using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using Utils;
using Globals;

public partial class Food : System.Web.UI.Page
{
	protected WebPage Pg;
	protected DB db;
	protected String FocusField = "";
	protected Int32 JobRno;
	protected int cItems;
	protected string CatInfo;
	protected int NumServings;

    private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));
        JobRno = Pg.JobRno();

		// Put user code to initialize the page here
		if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.Jobs.Title;
            //cItems = Str.Num(hfNumItems.Value);
            //cItems = Str.Num(txtcItems.Text);
            //AddLines();

            // setup the food category/item list 
            Setup();
		}
		else
		{
			//Update();
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
		RemoveLines();		// tear down the rows in the menu table
		LoadFood();			// load up the existing menu table rows plus 1 empty one

		hfNumItems.Value = Convert.ToString(cItems);
		txtcItems.Text = Convert.ToString(cItems);

		string Sql = string.Format("Select Sum(IsNull(NumMenServing, 0) + IsNull(NumWomenServing, 0) + IsNull(NumChildServing, 0)) As NumServings From mcJobs Where JobRno = {0}", JobRno);
		NumServings = db.SqlNum(Sql);
	}

	// setup the food item list control and the sort by radio buttons
	private void Setup()
	{
		switch ((String)Session["FoodSortedBy"])
		{
			case "Category":
			default:
				rbSortCategory.Checked = true;
				break;

			case "MenuItem":
				rbSortMenuItem.Checked = true;
				break;
		}

		// load the list of food category/items 
		LoadList();

		// load drop down list values for new menu items
		string Sql = string.Empty;
		try 
		{
			DataTable dt;

			//Sql = "Select * From mcJobMenuCategories Order By Category";
			//dt = db.SqlDataTable(Sql);
			//ddlCategory.DataSource = dt;
			//ddlCategory.DataTextField = "Category";
			//ddlCategory.DataValueField = "Category";
			//ddlCategory.DataBind();

			Sql = "Select * From KitchenLocations Order By SortOrder";
			dt = db.DataTable(Sql);
			ddlLocation.DataSource = dt;
			ddlLocation.DataTextField = "Name";
			ddlLocation.DataValueField = "KitchenLocRno";
			ddlLocation.DataBind();
		}
		catch (Exception Ex)
		{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
		}

		//lstList.Attributes.Add("onChange", "ClearDirty();");
		btnUpdate.Attributes.Add("onClick", "ClearDirty();");
	}

	// delete Remove checked menu items from the job
	private void RemoveFood()
	{
		cItems = Str.Num(hfNumItems.Value);

		for (int iItem = 1; iItem <= cItems; iItem++)
		{
			//TableRow tr = tblFood.Rows[iItem];

			//CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iItem);
			bool fRemoved = Utils.Parm.Bool("chkRemove" + iItem);

			//if (chkRemove != null && chkRemove.Checked)
			if (fRemoved)
			{
				String Sql = "";
				try
				{
					//Int32 FoodSeq = Str.Num(WebPage.FindTextBox(ref tr, "txtFoodSeq" + iItem));
					Int32 FoodSeq = Utils.Parm.Int("txtFoodSeq" + iItem);

					Sql = "Delete From mcJobFood Where JobRno = " + JobRno + " And FoodSeq = " + FoodSeq;
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

	// save (insert or update) the menu food items for the job
	private void SaveFood()
	{
		cItems = Str.Num(hfNumItems.Value);

		for (int iItem = 1; iItem <= cItems; iItem++)
		{
			//TableRow tr = tblFood.Rows[iItem];

			//CheckBox chkRemove = (CheckBox)tr.FindControl("chkRemove" + iItem);
			bool fRemoved = Utils.Parm.Bool("chkRemove" + iItem);

			//if (chkRemove != null && !chkRemove.Checked)
			if (!fRemoved)
			{
				//Int32 FoodSeq = Str.Num(WebPage.FindTextBox(ref tr, "txtFoodSeq" + iItem));
				Int32 FoodSeq = Utils.Parm.Int("txtFoodSeq" + iItem);
				Int32 JobFoodRno = Utils.Parm.Int("hfJobFoodRno" + iItem);

				//String Category = WebPage.FindTextBox(ref tr, "txtCategory" + iItem);
				string Category = Utils.Parm.Str("txtCategory" + iItem);
				//String OrigCategory = WebPage.FindTextBox(ref tr, "txtOrigCategory" + iItem);
				string OrigCategory = Utils.Parm.Str("txtOrigCategory" + iItem);

				//String MenuItem = WebPage.FindTextBox(ref tr, "txtMenuItem" + iItem);
				string MenuItem = Utils.Parm.Str("txtMenuItem" + iItem);
				//String OrigMenuItem = FindTextBox(ref tr, "txtOrigMenuItem" + iItem);
				string OrigMenuItem = Utils.Parm.Str("hfOrigMenuItem" + iItem);

				string Proposal = Parm.Str("hfProposal" + iItem);
				int KitchenLocRno = Parm.Int("hfLocRno" + iItem);
				bool OneTime = Parm.Bool("hfOneTime" + iItem);
				bool IngredSelFlg = Parm.Bool("hfIngredSelFlg" + iItem);
				bool OrigIngredSelFlg = Parm.Bool("hfOrigIngredSelFlg" + iItem);
				string IngredSel = Parm.Str("hfIngredSel" + iItem);
				string OrigIngredSel = Parm.Str("hfOrigIngredSel" + iItem);



				////String QtyNote = WebPage.FindTextBox(ref tr, "txtQtyNote" + iItem);
				////String OrigQtyNote = WebPage.FindTextBox(ref tr, "txtOrigQtyNote" + iItem);

				//int Qty = Str.Num(WebPage.FindTextBox(ref tr, "txtQty" + iItem));
				int Qty = Utils.Parm.Int("txtQty" + iItem);
				//int OrigQty = Str.Num(WebPage.FindTextBox(ref tr, "txtOrigQty" + iItem));
				int OrigQty = Utils.Parm.Int("txtOrigQty" + iItem);

				//String ServiceNote = WebPage.FindTextBox(ref tr, "txtServiceNote" + iItem);
				string ServiceNote = Utils.Parm.Str("txtServiceNote" + iItem);
				//String OrigServiceNote = WebPage.FindTextBox(ref tr, "txtOrigServiceNote" + iItem);
				string OrigServiceNote = Utils.Parm.Str("txtOrigServiceNote" + iItem);

				if (Category != OrigCategory ||
					MenuItem != OrigMenuItem ||
					//QtyNote != OrigQtyNote ||
					IngredSelFlg != OrigIngredSelFlg ||
					IngredSel != OrigIngredSel ||
					(Qty != OrigQty && (Category != "" || MenuItem != "" || ServiceNote != "")) ||
					ServiceNote != OrigServiceNote)
				{
					DateTime Tm = DateTime.Now;
					String Sql = "";

					try
					{
						//if (FoodSeq == 0)
						if (JobFoodRno == 0)
						{
							FoodSeq = db.NextSeq("mcJobFood", "JobRno", JobRno, "FoodSeq");

							Sql =
								"Insert Into mcJobFood (JobRno, FoodSeq, ProposalSeq, CreatedDtTm, CreatedUser) Values (" +
								JobRno + ", " +
								FoodSeq + ", " +
								FoodSeq + ", " +
								DB.PutDtTm(Tm) + ", " +
								DB.PutStr(g.User) + ") " +
								"Select @@Identity";
							JobFoodRno = db.SqlNum(Sql);
						}

						int MenuItemRno = FindOrAddMenuItem(Category, MenuItem, Proposal, KitchenLocRno, OneTime);

						//Response.Write(string.Format("Category [{0}] OrigMenuItem [{1}] MenuItem [{2}] Proposal [{3}] ", Category, OrigMenuItem, MenuItem, Proposal));

                        string MenuItemProposal = GetMenuItemProposal(MenuItemRno);
                        if (Proposal.Length == 0)
						{
                            Proposal = MenuItemProposal;
							if (Proposal.Length == 0)
							{
								Proposal = MenuItem;
                            }
                        }

                        if (IngredSelFlg && IngredSel.Length > 0 && Proposal.CompareTo(MenuItemProposal) == 0)
                        {
                            Sql = string.Format(
                                "Select Coalesce(i.Name, r.Name) As Name " +
                                "From RecipeIngredXref x " +
                                "Left Join Ingredients i On i.IngredRno = x.IngredRno " +
                                "Left Join Recipes r On r.RecipeRno = x.SubrecipeRno " +
                                "Where x.RecipeIngredRno In ({0}) " +
                                "Order By Name",
                                IngredSel);
                            DataTable dtIngred = db.DataTable(Sql);
                            if (dtIngred.Rows.Count > 0)
                            {
                                Proposal += " - ";
                                foreach (DataRow drIngred in dtIngred.Rows)
                                {
                                    Proposal += DB.Str(drIngred["Name"]) + ", ";
                                }
                                Proposal = Proposal.Substring(0, Proposal.Length - 2);
                            }
						}

						//Response.Write(string.Format("Proposal [{0}]<br />", Proposal));

						Sql =
							"Update mcJobFood Set " +
							"Category = " + DB.PutStr(Category, 50) + ", " +
							"MenuItem = " + DB.PutStr(MenuItem, 50) + ", " +
							"MenuItemRno = " + MenuItemRno + ", " +
							//"QtyNote = " + DB.PutStr(QtyNote, 128) + ", " +
							"Qty = " + Qty + ", " +
							"ServiceNote = " + DB.PutStr(ServiceNote, 128) + ", " +
							"ProposalMenuItem = " + DB.PutStr(Proposal, 100) + ", " +
							"IngredSelFlg = " + DB.PutBool(IngredSelFlg) + ", " +
							"IngredSel = " + DB.PutStr(IngredSel) + ", " +
							"UpdatedDtTm = " + DB.PutDtTm(Tm) + ", " +
							"UpdatedUser = " + DB.PutStr(g.User) + " " +
							//"Where JobRno = " + JobRno + " " +
							//"And FoodSeq = " + FoodSeq;
							"Where JobFoodRno = " + JobFoodRno;
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
	}

	// find the given text box in the given row of the food menu table
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
	//				if (Ctrl.GetType() == txtHidden.GetType())
	//				{
	//					txtHidden = (HtmlInputHidden)Ctrl;
	//					Text = txtHidden.Value.Trim();
	//				}
	//		}
	//	}
	//	catch (Exception Ex)
	//	{
	//		Err Err = new Err(Ex);
	//		Response.Write(Err.Html());
	//	}

	//	return Text;
	//}

	// retrieve the food menu items for the job and present them as rows in the menu table
	private void LoadFood()
	{
		if (JobRno > 0)
		{
			String Sql = "";

			try
			{
				// look for duplicates
				Sql =
					"Select f1.FoodSeq " +
					"From mcJobFood f1 Inner Join mcJobFood f2 " +
					"On f1.JobRno = f2.JobRno And f1.FoodSeq <> f2.FoodSeq " +
					"And f1.Category = f2.Category And f1.MenuItem = f2.MenuItem " +
					"Where f1.JobRno = " + JobRno;
				DupSeq Dups = new DupSeq(db, Sql);

				Sql = 
					"Select f.*, i.ServingQuote, i.ServingPrice, i.InaccuratePriceFlg, i.RecipeRno " +
					"From mcJobFood f Left Join mcJobMenuItems i on f.MenuItemRno = i.MenuItemRno " +
					"Where JobRno = " + JobRno + " And FoodSeq Is Not Null " +
					"Order By FoodSeq";
				DataTable dt = db.DataTable(Sql, 300);
				//cItems = dt.Rows.Count + 1;
				cItems = dt.Rows.Count;
				AddLines();

				int iRow = 0;
				foreach (DataRow dr in dt.Rows)
				{
					iRow++;
					TableRow tr = tblFood.Rows[iRow];

					bool fDup = Dups.In(DB.Int32(dr["FoodSeq"]));

					HtmlInputHidden txtFoodSeq = (HtmlInputHidden)tr.FindControl("txtFoodSeq" + iRow);
					txtFoodSeq.Value = DB.Str(dr["FoodSeq"]);

					HtmlInputHidden hfJobFoodRno = (HtmlInputHidden)tr.FindControl("hfJobFoodRno" + iRow);
					hfJobFoodRno.Value = DB.Str(dr["JobFoodRno"]);

					HtmlInputHidden hfProposal = (HtmlInputHidden)tr.FindControl("hfProposal" + iRow);
					hfProposal.Value = DB.Str(dr["ProposalMenuItem"]);

					HtmlInputHidden hfIngredSelFlg = (HtmlInputHidden)tr.FindControl("hfIngredSelFlg" + iRow);
					HtmlInputHidden hfOrigIngredSelFlg = (HtmlInputHidden)tr.FindControl("hfOrigIngredSelFlg" + iRow);
					bool fIngredSel = DB.Bool(dr["IngredSelFlg"]);
					hfIngredSelFlg.Value = 
					hfOrigIngredSelFlg.Value = fIngredSel.ToString();

					HtmlInputHidden hfIngredSel = (HtmlInputHidden)tr.FindControl("hfIngredSel" + iRow);
					HtmlInputHidden hfOrigIngredSel = (HtmlInputHidden)tr.FindControl("hfOrigIngredSel" + iRow);
					hfIngredSel.Value = 
					hfOrigIngredSel.Value = DB.Str(dr["IngredSel"]);

					HtmlInputHidden hfIngredSelAutoPop = (HtmlInputHidden)tr.FindControl("hfIngredSelAutoPop" + iRow);
					hfIngredSelAutoPop.Value = false.ToString();

					TextBox txtCategory = (TextBox)tr.FindControl("txtCategory" + iRow);
					HtmlInputHidden hfOrigCategory = (HtmlInputHidden)tr.FindControl("hfOrigCategory" + iRow);
					txtCategory.Text =
					hfOrigCategory.Value = DB.Str(dr["Category"]);
					txtCategory.CssClass = (fDup ? "DupFoodCategory " : "") + "FoodCategory";

					TextBox txtMenuItem = (TextBox)tr.FindControl("txtMenuItem" + iRow);
					HtmlInputHidden hfOrigMenuItem = (HtmlInputHidden)tr.FindControl("hfOrigMenuItem" + iRow);
					txtMenuItem.Text =
					hfOrigMenuItem.Value = DB.Str(dr["MenuItem"]);
					txtMenuItem.CssClass = (fDup ? "DupMenuItem " : "") + "MenuItem";

					SelectList slMenuItem = SelectList.Find("MenuItem" + iRow);
					if (slMenuItem != null)
					{
						Sql = "Select Distinct MenuItem From mcJobMenuItems Where Category = " + DB.PutStr(txtCategory.Text) + " And HideFlg != 1 Order By MenuItem";
						slMenuItem.ClearValues();
						slMenuItem.AddDBValues(db, Sql);
					}

					Label lblIngredSel = (Label)tr.FindControl("lblIngredSel" + iRow);
					if (fIngredSel)
					{
						string ToolTip = "<div class='IngredSelQtip'>Selected Ingredients</div>";
						Sql = string.Format(
							"Select Coalesce(i.Name, r.Name) as Name, " +
                            "r.GlutenFreeFlg, r.VeganFlg, r.VegetarianFlg, r.DairyFreeFlg, r.NutsFlg " +
                            "From RecipeIngredXref x " +
							"Left Join Ingredients i On i.IngredRno = x.IngredRno " +
							"Left Join Recipes r on r.RecipeRno = x.SubrecipeRno " +
							"Where x.RecipeRno = {0} " +
							"And RecipeIngredRno In ({1}) " +
							"Order By x.RecipeSeq",
							DB.Int32(dr["RecipeRno"]),
							(hfIngredSel.Value != string.Empty ? hfIngredSel.Value : "null"));
						DataTable dtXref = db.DataTable(Sql);
						foreach (DataRow drXref in dtXref.Rows)
						{
							ToolTip += DB.Str(drXref["Name"]) +
                                (DB.Bool(drXref["GlutenFreeFlg"]) ? " [GF]"  : string.Empty) +
                                (DB.Bool(drXref["VeganFlg"])      ? " [V]"   : string.Empty) +
                                (DB.Bool(drXref["VegetarianFlg"]) ? " [Veg]" : string.Empty) +
                                (DB.Bool(drXref["DairyFreeFlg"])  ? " [DF]"  : string.Empty) +
                                (DB.Bool(drXref["NutsFlg"])       ? " [N]"   : string.Empty) +
                                "<br/>";
						}

						lblIngredSel.ToolTip = ToolTip;
						lblIngredSel.Attributes.Add("style", "display: inline;");
					}
					else
					{
						lblIngredSel.ToolTip = "Select ingredients for the menu item.";
						lblIngredSel.Attributes["style"] = string.Empty;
					}

					//TextBox txtQtyNote = (TextBox)tr.FindControl("txtQtyNote" + iRow);
					//TextBox txtOrigQtyNote = (TextBox)tr.FindControl("txtOrigQtyNote" + iRow);
					//txtQtyNote.Text =
					//txtOrigQtyNote.Text = DB.Str(dr["QtyNote"]);

					TextBox txtQty = (TextBox)tr.FindControl("txtQty" + iRow);
					TextBox txtOrigQty = (TextBox)tr.FindControl("txtOrigQty" + iRow);
					txtQty.Text =
					txtOrigQty.Text = DB.Int32(dr["Qty"]).ToString("##,###");

					TextBox txtServiceNote = (TextBox)tr.FindControl("txtServiceNote" + iRow);
					TextBox txtOrigServiceNote = (TextBox)tr.FindControl("txtOrigServiceNote" + iRow);
					txtServiceNote.Text =
					txtOrigServiceNote.Text = DB.Str(dr["ServiceNote"]);

					decimal ServingQuote = DB.Dec(dr["ServingQuote"]);
					Label lblQuote = (Label)tr.FindControl("lblQuote" + iRow);
					lblQuote.Text = Fmt.Dollar(ServingQuote);

					decimal ServingPrice = DB.Dec(dr["ServingPrice"]);
					Label lblPrice = (Label)tr.FindControl("lblPrice" + iRow);
					lblPrice.Text = Fmt.Dollar(ServingPrice);

					bool fInaccuratePrice = DB.Bool(dr["InaccuratePriceFlg"]);
					Label lblInaccuratePrice = (Label)tr.FindControl("lblInaccuratePrice" + iRow);
					if (fInaccuratePrice)
					{
						lblInaccuratePrice.CssClass = "icon-dollar";
						lblInaccuratePrice.ToolTip = "Cost and Price are inaccurate because there are ingredients with no price from receipts.";
					}
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
			cItems = 1;
			AddLines();
		}

		//if (FocusField.Length == 0) { FocusField = "txtCategory" + cItems; }
	}

	// prepare the SQL script to retrieve the menu category/items and then fill the list control
	private void LoadList()
	{
		string Sql = "";
        string SqlSelect =
            "Select Distinct c.Category, c.MultSelFlg As CatMultSelFlg, c.SortOrder, " +
            "i.MenuItemRno, i.MenuItem, i.MultSelFlg, i.MultItems, i.IngredSelFlg, i.RecipeRno, " +
            "IsNull(i.CategorySortOrder, 0) As ItemSortOrder, i.ServingQuote, i.ServingPrice, i.InaccuratePriceFlg, " +
            "r.GlutenFreeFlg, r.VeganFlg, r.VegetarianFlg, r.DairyFreeFlg, r.NutsFlg " +
            "From mcJobMenuItems i Inner Join mcJobMenuCategories c On i.Category = c.Category " +
            "Left Join Recipes r on i.RecipeRno = r.RecipeRno";
		string SqlOrderBy;
		string[] aCategory = null;
		string List = "";

		if (rbSortCategory.Checked)
		{
			SqlOrderBy = " Order By c.SortOrder, c.Category, ItemSortOrder, i.MenuItem";
		}
		else if (rbSortCategoryAlpha.Checked)
		{
			SqlOrderBy = " Order By c.Category, ItemSortOrder, i.MenuItem";
			//aCategory = new string[] { "Meats", "Sides", "Salads", "Bread", "Desserts", "Drink" };
		}
		else
		{
			SqlOrderBy = " Order By i.MenuItem, c.Category";
		}

		string CurrItem = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : "");
		lstList.Items.Clear();

		if (aCategory != null)
		{
			foreach (string Category in aCategory)
			{
				Sql = SqlSelect + "Where Category = " + DB.PutStr(Category) + " And i.HideFlg != 1 " + SqlOrderBy;
				LoadListSql(Sql, CurrItem);
				List += ", '" + Category + "'";
			}

			List = List.Substring(2);
		}

		Sql = SqlSelect;
		if (List.Length > 0)
		{
			Sql += "Where Category Not In (" + List + ") ";
		}
		Sql += (Sql.IndexOf("Where") > -1 ? " And " : " Where ") + "(i.HideFlg = 0 Or i.HideFlg Is Null) And (c.HideFlg = 0 Or c.HideFlg Is Null)";
		Sql += SqlOrderBy;

		LoadListSql(Sql, CurrItem);

		lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Items";
	}

	// populate the list control from the category/item data returned from the SQL
	private void LoadListSql(string Sql, string CurrItem)
	{
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				int MenuItemRno         = DB.Int32(r["MenuItemRno"]);
				string Category         = DB.Str(r["Category"]);
				bool fCatMultiSelect    = DB.Bool(r["CatMultSelFlg"]);
				int SortOrder           = DB.Int32(r["SortOrder"]);
				string MenuItem         = DB.Str(r["MenuItem"]);
				bool fMultiSelect       = DB.Bool(r["MultSelFlg"]);
				string MultItems        = DB.Str(r["MultItems"]);
				bool fIngredSelect      = DB.Bool(r["IngredSelFlg"]);
				int RecipeRno           = DB.Int32(r["RecipeRno"]);
				int ItemSortOrder       = DB.Int32(r["ItemSortOrder"]);
				decimal Quote           = DB.Dec(r["ServingQuote"]);
				decimal Price           = DB.Dec(r["ServingPrice"]);
				bool fInaccuratePrice   = DB.Bool(r["InaccuratePriceFlg"]);
                bool fGlutenFree        = DB.Bool(r["GlutenFreeFlg"]);
                bool fVegan             = DB.Bool(r["VeganFlg"]);
                bool fVegetarian        = DB.Bool(r["VegetarianFlg"]);
                bool fDairyFree         = DB.Bool(r["DairyFreeFlg"]);
                bool fNuts              = DB.Bool(r["NutsFlg"]);

                ListItem Item = new ListItem(Category + " - " + MenuItem);
				Item.Selected = (Item.Value == CurrItem);
				Item.Attributes.Add("Rno", MenuItemRno.ToString());
				Item.Attributes.Add("Category", Category);
				if (fCatMultiSelect)
				{
					Item.Attributes.Add("CatMultSel", "true");
				}
				Item.Attributes.Add("SortOrder", SortOrder.ToString("000"));
				if (fMultiSelect)
				{
					Item.Attributes.Add("MultItems", MultItems);
				}
				if (fIngredSelect && RecipeRno > 0)
				{
					Sql = string.Format(
                        "Select x.RecipeIngredRno, Coalesce(i.Name, r.Name) as Name, r.GlutenFreeFlg, r.VeganFlg, r.VegetarianFlg, r.DairyFreeFlg, r.NutsFlg " +
						"From RecipeIngredXref x " +
						"Left Join Ingredients i On i.IngredRno = x.IngredRno " +
						"Left Join Recipes r on r.RecipeRno = x.SubrecipeRno " +
						"Where x.RecipeRno = {0} " +
						"Order By Name",
						RecipeRno);
					string Ingred = string.Empty;
					DataTable dtIngred = db.DataTable(Sql);
					foreach (DataRow drIngred in dtIngred.Rows)
					{
						Ingred += string.Format(
                            "{0}|{1}|{2}|{3}|{4}|{5}|{6}~", 
                            DB.Int32(drIngred["RecipeIngredRno"]), 
                            DB.Str(drIngred["Name"]),
                            DB.Int32(drIngred["GlutenFreeFlg"]),
                            DB.Int32(drIngred["VeganFlg"]),
                            DB.Int32(drIngred["VegetarianFlg"]),
                            DB.Int32(drIngred["DairyFreeFlg"]),
                            DB.Int32(drIngred["NutsFlg"]));
                    }
					if (Ingred.Length > 0)
					{
						Ingred = Ingred.Substring(0, Ingred.Length - 1);
						Item.Attributes.Add("Ingred", Ingred);
					}
				}
				Item.Attributes.Add("MenuItem", MenuItem);
				Item.Attributes.Add("ItemSortOrder", ItemSortOrder.ToString("000"));
				Item.Attributes.Add("title", Item.Text);
				Item.Attributes.Add("Quote", Fmt.Dollar(Quote));
				Item.Attributes.Add("Price", Fmt.Dollar(Price));
				Item.Attributes.Add("InaccuratePrice", (fInaccuratePrice ? "true" : "false"));
                if (fGlutenFree) { Item.Attributes.Add("GlutenFree", "1"); }
                if (fVegan)      { Item.Attributes.Add("Vegan", "1"); }
                if (fVegetarian) { Item.Attributes.Add("Vegetarian", "1"); }
                if (fDairyFree)  { Item.Attributes.Add("DairyFree", "1"); }
                if (fNuts)       { Item.Attributes.Add("Nuts", "1"); }
                lstList.Items.Add(Item);
			}

			Sql =
				"Select Distinct c.Category, i.MenuItem " +
				"From mcJobMenuItems i Inner Join mcJobMenuCategories c On i.Category = c.Category " +
				"Order By c.Category, i.MenuItem";
			dt = db.DataTable(Sql);

			StringBuilder sbData = new StringBuilder();
			string PrevCategory = null;

			foreach (DataRow dr in dt.Rows)
			{
				string Category = DB.Str(dr["Category"]);
				string MenuItem = DB.Str(dr["MenuItem"]);

				if (Category != PrevCategory)
				{
					if (sbData.Length > 0)
					{
						sbData.Append("]},\n\t");
					}
					sbData.AppendFormat("{{cat:\"{0}\",items:[", Category.Replace("\"", "\\\""));
					PrevCategory = Category;
				}
				sbData.AppendFormat("\"{0}\",", MenuItem.Replace("\"", "\\\""));
			}
			sbData.Append("]}");

			CatInfo = sbData.ToString();
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
	}

	private void AddLines()
	{
		//String Sql;
		SelectList.Clear();

		//int NumItems = (cItems > 0 ? cItems : 1);
		int NumItems = cItems;

		for (int iItem = 1; iItem <= NumItems; iItem++)
		{
			TableRow tr = BuildFoodTableRow(iItem.ToString());
			tblFood.Rows.Add(tr);

			// because the previous page values are still floating around and get matched up when the row is added to the table
			// we need to blank the out here
			Label lbl = (Label)tr.FindControl("lblIngredSel" + iItem);
			lbl.ToolTip = string.Empty;
			lbl.Attributes["style"] = string.Empty;
			TextBox txt = (TextBox)tr.FindControl("txtCategory" + iItem);
			txt.CssClass = "FoodCategory";
			txt = (TextBox)tr.FindControl("txtMenuItem" + iItem);
			txt.CssClass = "MenuItem";
			lbl = (Label)tr.FindControl("lblQuote" + iItem);
			lbl.Text = "$0.00";
			lbl = (Label)tr.FindControl("lblPrice" + iItem);
			lbl.Text = "$0.00";
			lbl = (Label)tr.FindControl("lblInaccuratePrice" + iItem);
			lbl.Text = "";
			lbl = (Label)tr.FindControl("lblExtQuote" + iItem);
			lbl.Text = "$0.00";
			lbl = (Label)tr.FindControl("lblExtPrice" + iItem);
			lbl.Text = "$0.00";
			lbl = (Label)tr.FindControl("lblOverUnder" + iItem);
			lbl.Text = "0% over";
		}
	}

	protected string AddRowHtml()
	{
		TableRow rw = BuildFoodTableRow("~ID~");

		StringBuilder sb = new StringBuilder();
		rw.RenderControl(new HtmlTextWriter(new StringWriter(sb)));

		return sb.ToString().Replace("\\", "\\\\").Replace("\r", "\\r").Replace("\n", "\\n").Replace("\"", "\\\""); 
	}

	private TableRow BuildFoodTableRow(string ID)
	{
		TableRow rw = new TableRow();

		for (int iCell = 0; iCell < 22; iCell++)
		{
			rw.Cells.Add(new TableCell());
		}

		rw.Cells[0].Text = "<b>" + ID + "</b>";

		HtmlInputHidden txtFoodSeq = new HtmlInputHidden()
        {
            ID = "txtFoodSeq" + ID
        };
		rw.Cells[1].Controls.Add(txtFoodSeq);

        HtmlInputHidden hfJobFoodRno = new HtmlInputHidden
        {
            ID = "hfJobFoodRno" + ID
        };
        rw.Cells[1].Controls.Add(hfJobFoodRno);

        HtmlInputHidden hfProposal = new HtmlInputHidden
        {
            ID = "hfProposal" + ID
        };
        hfProposal.Attributes.Add("class", "Proposal");
		rw.Cells[1].Controls.Add(hfProposal);

        HtmlInputHidden hfLocRno = new HtmlInputHidden
        {
            ID = "hfLocRno" + ID
        };
        hfLocRno.Attributes.Add("class", "LocRno");
		rw.Cells[1].Controls.Add(hfLocRno);

		HtmlInputHidden hfOneTime = new HtmlInputHidden();
		hfLocRno.ID = "hfOneTime" + ID;
		hfLocRno.Attributes.Add("class", "OneTime");
		rw.Cells[1].Controls.Add(hfOneTime);

        HtmlInputHidden hfIngredSelFlg = new HtmlInputHidden
        {
            ID = "hfIngredSelFlg" + ID
        };
        hfIngredSelFlg.Attributes.Add("class", "IngredSelFlg");
		rw.Cells[1].Controls.Add(hfIngredSelFlg);

        HtmlInputHidden hfOrigIngredSelFlg = new HtmlInputHidden
        {
            ID = "hfOrigIngredSelFlg" + ID
        };
        rw.Cells[1].Controls.Add(hfOrigIngredSelFlg);

        HtmlInputHidden hfIngredSel = new HtmlInputHidden
        {
            ID = "hfIngredSel" + ID
        };
        hfIngredSel.Attributes.Add("class", "IngredSel");
		rw.Cells[1].Controls.Add(hfIngredSel);

        HtmlInputHidden hfOrigIngredSel = new HtmlInputHidden
        {
            ID = "hfOrigIngredSel" + ID
        };
        rw.Cells[1].Controls.Add(hfOrigIngredSel);

        HtmlInputHidden hfIngredSelAutoPop = new HtmlInputHidden
        {
            ID = "hfIngredSelAutoPop" + ID
        };
        hfIngredSelAutoPop.Attributes.Add("class", "IngredSelAutoPop");
		hfIngredSelAutoPop.Value = true.ToString();
		rw.Cells[1].Controls.Add(hfIngredSelAutoPop);

        // Remove
        CheckBox chkRemove = new CheckBox
        {
            ID = "chkRemove" + ID
        };
        chkRemove.Attributes.Add("onClick", "SetDirty();");
		rw.Cells[2].Controls.Add(chkRemove);
		rw.Cells[2].HorizontalAlign = HorizontalAlign.Center;

        // Category
        TextBox txtCategory = new TextBox
        {
            ID = "txtCategory" + ID,
            MaxLength = 50,
            CssClass = "FoodCategory"
        };
        //txtCategory.AutoPostBack = true;
        //			txtCategory.Attributes.Add("onChange", "ClearDirty();");
        //			txtCategory.TextChanged += new System.EventHandler(this.btnUpdate_Click);
        rw.Cells[4].Controls.Add(txtCategory);

        //System.Web.UI.WebControls.Image imgCategory = new System.Web.UI.WebControls.Image();
        //imgCategory.ID = "ddCategory" + ID;
        //imgCategory.ImageUrl = "Images/DropDown.gif";
        //imgCategory.BorderWidth = 0;
        //imgCategory.ImageAlign = ImageAlign.AbsMiddle;
        //imgCategory.CssClass = "ddFoodCategory";
        //r.Cells[4].Controls.Add(imgCategory);

        //Sql = "Select Distinct Category From mcJobMenuItems Order By Category";
        //SelectList slCategory = new SelectList("Category" + ID, ref txtCategory);
        //slCategory.ImageButton(imgCategory);
        //slCategory.AutoPostBack = true;
        //slCategory.ClearValues();
        //slCategory.AddDBValues(db, Sql);

        HtmlInputHidden hfOrigCategory = new HtmlInputHidden
        {
            ID = "hfOrigCategory" + ID
        };
        rw.Cells[4].Controls.Add(hfOrigCategory);

        // MenuItem
        TextBox txtMenuItem = new TextBox
        {
            ID = "txtMenuItem" + ID,
            MaxLength = 50,
            CssClass = "MenuItem"
        };
        //			txtMenuItem.Attributes.Add("onChange", "SetDirty();");
        rw.Cells[6].Controls.Add(txtMenuItem);

        //System.Web.UI.WebControls.Image imgMenuItem = new System.Web.UI.WebControls.Image();
        //imgMenuItem.ID = "ddMenuItem" + ID;
        //imgMenuItem.ImageUrl = "Images/DropDown.gif";
        //imgMenuItem.BorderWidth = 0;
        //imgMenuItem.ImageAlign = ImageAlign.AbsMiddle;
        //imgMenuItem.CssClass = "ddMenuItem";
        //r.Cells[6].Controls.Add(imgMenuItem);

        //SelectList slMenuItem = new SelectList("MenuItem" + ID, ref txtMenuItem);
        //slMenuItem.ImageButton(imgMenuItem);
        //slMenuItem.NextField = "txtCategory" + cItems;
        //slMenuItem.ClearValues();

        HtmlInputHidden hfOrigMenuItem = new HtmlInputHidden
        {
            ID = "hfOrigMenuItem" + ID
        };
        rw.Cells[6].Controls.Add(hfOrigMenuItem);

        Label lblIngredSel = new Label
        {
            ID = "lblIngredSel" + ID,
            CssClass = "lblIngredSel icon-check",
            ToolTip = "Select ingredients for the menu item."
        };
        rw.Cells[7].Controls.Add(lblIngredSel);

        //// Qty Note
        //TextBox txtQtyNote = new TextBox();
        //txtQtyNote.ID = "txtQtyNote" + ID;
        //txtQtyNote.MaxLength = 128;
        //txtQtyNote.Style.Add("width", "100px");
        //txtQtyNote.Attributes.Add("onChange", "SetDirty();");
        //r.Cells[8].Controls.Add(txtQtyNote);

        //txtQtyNote = new TextBox();
        //txtQtyNote.ID = "txtOrigQtyNote" + ID;
        //txtQtyNote.Visible = false;
        //r.Cells[8].Controls.Add(txtQtyNote);

        // Qty 
        TextBox txtQty = new TextBox
        {
            ID = "txtQty" + ID,
            //txtQty.Text = Fmt.Num(NumServings);
            CssClass = "Quantity"
        };
        //txtQty.Style.Add("width", "100px");
        //txtQty.Attributes.Add("onChange", "ValidateNum(this);SetDirty();");
        rw.Cells[8].Controls.Add(txtQty);

        txtQty = new TextBox
        {
            ID = "txtOrigQty" + ID,
            Visible = false
        };
        rw.Cells[8].Controls.Add(txtQty);

        // Service Note
        TextBox txtServiceNote = new TextBox
        {
            ID = "txtServiceNote" + ID,
            MaxLength = 128
        };
        txtServiceNote.Style.Add("width", "150px");
		txtServiceNote.Attributes.Add("onChange", "SetDirty();");
		rw.Cells[10].Controls.Add(txtServiceNote);

        txtServiceNote = new TextBox
        {
            ID = "txtOrigServiceNote" + ID,
            Visible = false
        };
        rw.Cells[10].Controls.Add(txtServiceNote);

        // Quote
        Label lblQuote = new Label
        {
            ID = "lblQuote" + ID,
            Text = "$0.00"
        };
        rw.Cells[12].Controls.Add(lblQuote);
		rw.Cells[12].CssClass = "Right";

        // Price		
        Label lblPrice = new Label
        {
            ID = "lblPrice" + ID,
            Text = "$0.00"
        };
        rw.Cells[14].Controls.Add(lblPrice);
		rw.Cells[14].CssClass = "Right";

        // inaccurate price
        Label lblInaccuratePrice = new Label
        {
            ID = "lblInaccuratePrice" + ID,
            Text = ""
        };
        rw.Cells[15].Controls.Add(lblInaccuratePrice);
		rw.Cells[15].CssClass = "InaccuratePrice";

        // Ext Quote
        Label lblExtQuote = new Label
        {
            ID = "lblExtQuote" + ID,
            Text = "$0.00"
        };
        rw.Cells[17].Controls.Add(lblExtQuote);
		rw.Cells[17].CssClass = "Right";

        // Ext Price		
        Label lblExtPrice = new Label
        {
            ID = "lblExtPrice" + ID,
            Text = "$0.00"
        };
        rw.Cells[19].Controls.Add(lblExtPrice);
		rw.Cells[19].CssClass = "Right";

        // Price Note
        Label lblOverUnder = new Label
        {
            ID = "lblOverUnder" + ID,
            Text = "0% over",
            CssClass = "OverUnder"
        };
        rw.Cells[21].Controls.Add(lblOverUnder);

		return rw;
	}

	// tear down rows in the menu food items table
	private void RemoveLines()
	{
		while (tblFood.Rows.Count > 1)
		{
			tblFood.Rows.Remove(tblFood.Rows[1]);
		}
	}

	// a new food item has been selected in the list control
	// add it to the job
	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
		RemoveFood();	// delete Removed menu items from the job
		SaveFood();		// save the menu food items for the job

		String Sql = "";

		try
		{
			String Category = lstList.SelectedItem.Value;
			String MenuItem = "";
			int i = Category.LastIndexOf(" - ");
			if (i >= 0)
			{
				MenuItem = Category.Substring(i + 3);
				Category = Category.Substring(0, i);
			}

			Int32 FoodSeq = db.NextSeq("mcJobFood", "JobRno", JobRno, "FoodSeq");
			int MenuItemRno = FindOrAddMenuItem(Category, MenuItem, MenuItem, 0, false);

			Sql =
				"Insert Into mcJobFood (JobRno, FoodSeq, Category, MenuItem, MenuItemRno, ProposalMenuItem, CreatedDtTm, CreatedUser) Values (" +
				JobRno + ", " +
				FoodSeq + ", " +
				DB.PutStr(Category, 50) + ", " +
				DB.PutStr(MenuItem, 50) + ", " +
				MenuItemRno + ", " +
				DB.PutStr(MenuItem, 50) + ", " +
				DB.PutDtTm(DateTime.Now) + ", " +
				DB.PutStr(g.User) + ")";
			db.Exec(Sql);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		LoadList();
	}

	// find or add a newly entered (first time) food item in to the table of food items
	protected int FindOrAddMenuItem(string Category, string MenuItem, string Proposal, int KitchenLocRno, bool OneTime)
	{
		int MenuItemRno = 0;
		string Sql = "";

		if (MenuItem.Length > 0)
		{
			try
			{
				//put in mcJobMenuItems also if not already in there
				Sql = string.Format(
					"Select Top 1 MenuItemRno From mcJobMenuItems Where Category = {0} And MenuItem = {1}",
					DB.PutStr(Category, 50),
					DB.PutStr(MenuItem, 50));
				MenuItemRno = db.SqlNum(Sql);

				if (MenuItemRno == 0)
				{
					//Sql = "Select Top 1 KitchenLocRno From KitchenLocations Where DefaultFlg = 1";
					//int KitchenLocRno = db.SqlNum(Sql);
					Sql = string.Format(
						"Insert Into mcJobMenuItems (Category, MenuItem, ProposalMenuItem, KitchenLocRno, HideFlg, CreatedDtTm, CreatedUser) Values " +
						"({0}, {1}, {2}, {3}, {4}, GetDate(), {5}); Select @@Identity",
						DB.PutStr(Category, 50),
						DB.PutStr(MenuItem, 50),
						DB.PutStr(Proposal, 100),
						KitchenLocRno,
						(OneTime ? "1" : "null"),
						DB.PutStr(g.User));
					MenuItemRno = db.SqlNum(Sql);

					//put in mcJobMenuCategories also if not already in there
					Sql = string.Format(
						"Select Count(*) From mcJobMenuCategories Where Category = {0}",
						DB.PutStr(Category, 50));
					if (db.SqlNum(Sql) == 0)
					{
						Sql = string.Format("Select Max(SortOrder) From mcJobMenuCategories");
						int SortOrder = db.SqlNum(Sql) + 1;

						Sql = string.Format(
							"Insert Into mcJobMenuCategories (Category, SortOrder, CreatedDtTm, CreatedUser) Values " +
							"({0}, {1}, GetDate(), {2})",
							DB.PutStr(Category, 50),
							SortOrder,
							DB.PutStr(g.User));
						db.Exec(Sql);
					}
				}
			}
			catch (Exception Ex)
			{
				Err Err = new Err(Ex, Sql);
				Response.Write(Err.Html());
			}
		}

		return MenuItemRno;
	}

	protected string GetMenuItemProposal(int MenuItemRno)
	{
		string Sql = string.Format("Select ProposalMenuItem From mcJobMenuItems Where MenuItemRno = {0}", MenuItemRno);
		string Proposal = string.Empty;

		try
		{
			Proposal = db.SqlStr(Sql);
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		return Proposal;
	}

	// update button was clicked or a category item was seleted
	protected void btnUpdate_Click(object sender, System.EventArgs e)
	{
		Update();	// save the job food items

		// see if this update is from a Category
		TextBox txtBox = new TextBox();
		if (sender.GetType() == txtBox.GetType())
		{
			txtBox = (TextBox)sender;
			String Category = "txtCategory";
			if (txtBox.ID.Substring(0, Category.Length) == Category)
			{
				// set the focus on the MenuItem
				FocusField = "txtMenuItem" + txtBox.ID.Substring(Category.Length);
			}
		}
	}

	private void Update()
	{
		RemoveFood();	// delete Removed menu items from the job
		SaveFood();		// save the menu food items for the job
		LoadList();		// populate the list control
	}
}