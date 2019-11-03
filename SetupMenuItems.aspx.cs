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

public partial class SetupMenuItems : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";

	protected bool fMerging = false;
	protected string MergeMenuItem;
	protected string MergeIntoMenuItem;
	protected string MergeCategory;
	protected string MergeIntoCategory;
	protected string AffectedJobs;
	protected bool fSimilarMenuItemsWithRecipe = false;

	private bool fStartHere = false;
	private string StartHere;
	protected string ErrorMsg = string.Empty;
	protected bool fError = false;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Session["Menu"] = WebPage.MenuSetup.Title;
        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			ClearData();
			Setup();

			// handle a particular menu item being edited
			int MenuItemRno = Parm.Int("Rno");
			if (MenuItemRno != 0)
			{
				GetData(MenuItemRno);
			}
		}
	}

    private void Setup()
    {
        rbSortCategory.Checked = true;
		btnUpdate.Enabled =
		btnNext.Enabled = true;

        LoadList();

		string Sql = "Select * From mcJobMenuCategories where (HideFlg Is Null Or HideFlg = 0) Order By Category"; // SortOrder";
		try 
		{
			DataTable dt = db.DataTable(Sql);
			ddlCategory.DataSource = dt;
			ddlCategory.DataTextField = "Category";
			ddlCategory.DataValueField = "Category";
			ddlCategory.DataBind();

			Sql = "Select * From KitchenLocations Order By SortOrder";
			dt = db.DataTable(Sql);
			ddlLocation.DataSource = dt;
			ddlLocation.DataTextField = "Name";
			ddlLocation.DataValueField = "KitchenLocRno";
			ddlLocation.DataBind();

			int Rno = Parm.Int("Rno");
			if (Rno != 0)
			{
				GetData(Rno);
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
		//Response.Write("ClearData()<br/>");
		lstList.SelectedIndex = -1;
		txtRno.Value =
		txtCurrCategory.Value = 
		txtCurrMenuItem.Value =
		txtMenuItem.Text =
		txtProposal.Text =
		txtServingQuote.Text =
		txtServingPrice.Text =
		lblInaccuratePrice.Text =
 		lnkRecipe.Text =
		txtMultItems.Value = string.Empty;
		ltlRecipe.Text = string.Empty;
		ddlCategory.SelectedIndex = 
		ddlLocation.SelectedIndex = -1;
		chkAsIs.Checked =
		chkMultiSelect.Checked = 
		chkIngredSelect.Checked =
		chkHide.Checked = false;
		btnDelete.Enabled = false;
		btnDelete.ToolTip = "New menu item record";
	}

    protected void GetData(int Rno)
    {
        string Sql = string.Format("Select *, (Select Name From Recipes Where RecipeRno = i.RecipeRno) As Name From mcJobMenuItems i Where MenuItemRno = {0}", Rno);

		ClearData();

        try
        {
            DataTable dt = db.DataTable(Sql);
            if (dt.Rows.Count > 0)
            {
                DataRow dr = dt.Rows[0];
				int KitchenLocRno = DB.Int32(dr["KitchenLocRno"]);
				if (KitchenLocRno == 0)
				{
					Sql = "Select KitchenLocRno From KitchenLocations Where DefaultFlg = 1";
					KitchenLocRno = db.SqlNum(Sql);
				}

				txtRno.Value = DB.Int32(dr["MenuItemRno"]).ToString();
				txtCurrCategory.Value = 
                ddlCategory.Text = DB.Str(dr["Category"]);
				txtCurrMenuItem.Value =
                txtMenuItem.Text = DB.Str(dr["MenuItem"]);
				txtProposal.Text = DB.Str(dr["ProposalMenuItem"]);
				txtServingQuote.Text = Fmt.Dollar(DB.Dec(dr["ServingQuote"]));
				txtServingPrice.Text = Fmt.Dollar(DB.Dec(dr["ServingPrice"]));
				if (DB.Bool(dr["InaccuratePriceFlg"]))
				{
					HtmlGenericControl i = new HtmlGenericControl("i");
					i.Attributes.Add("class", "icon-dollar");
					i.Attributes.Add("title", "Cost and Price are inaccurate because there are ingredients with no price from receipts.");
					lblInaccuratePrice.Controls.Add(i);
				}
				bool fAsIs = DB.Bool(dr["AsIsFlg"]);
				int RecipeRno = 0;

				if (fAsIs)
				{
					ltlRecipe.Text = "none";
				}
				else
				{
					RecipeRno = DB.Int32(dr["RecipeRno"]);
					if (RecipeRno == 0)
					{
						ltlRecipe.Text = "none";
						chkAsIs.Enabled = true;
						chkAsIs.ToolTip = "";
					}
					else
					{
						lnkRecipe.Text = DB.Str(dr["Name"]);
						lnkRecipe.NavigateUrl = string.Format("Recipes.aspx?Rno={0}", RecipeRno);
						chkAsIs.Enabled = false;
						chkAsIs.ToolTip = "As Is items cannot have a recipe. Remove this item from its recipe first.";
					}
				}
				ddlLocation.SelectedValue = KitchenLocRno.ToString();
				chkAsIs.Checked = fAsIs;
				chkMultiSelect.Checked = DB.Bool(dr["MultSelFlg"]);
				chkIngredSelect.Checked = DB.Bool(dr["IngredSelFlg"]);
				chkIngredSelect.Enabled = (RecipeRno != 0);
                chkHide.Checked = DB.Bool(dr["HideFlg"]);
				txtMultItems.Value = DB.Str(dr["MultItems"]);
				string[] MultiItems = txtMultItems.Value.Split(',');
				chkMultiSelect.Text = string.Format((txtMultItems.Value.Length > 0 && MultiItems.Length > 0 ? " {0} ({1} items)" : " {0}"), "Multi-Select", MultiItems.Length); 
				txtCreatedDt.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
				txtCreatedUser.Text = DB.Str(dr["CreatedUser"]);
				txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
				txtUpdatedUser.Text = DB.Str(dr["UpdatedUser"]);

				Sql = string.Format("Select Count(*) From mcJobFood Where Category = {0} And MenuItem = {1}", DB.PutStr(ddlCategory.Text), DB.PutStr(txtMenuItem.Text));
				int NumJobs = db.SqlNum(Sql);

				if (NumJobs > 0)
				{
					btnDelete.Enabled = false;
					btnDelete.ToolTip = string.Format("Used in {0} Jobs", NumJobs);
				}
				else
				{
					btnDelete.Enabled = true;
					btnDelete.ToolTip = "Not used in any Jobs";
				}

				Sql = string.Format(
					"Select MenuItemRno, Category, RecipeRno " +
					"From mcJobMenuItems Where MenuItem In " +
					"(Select MenuItem From mcJobMenuItems Where MenuItemRno = {0}) And MenuItemRno <> {0} " +
					"And IsNull(HideFlg, 0) = 0 " +
					"And Category In (Select Category From mcJobMenuCategories Where IsNull(HideFlg, 0) = 0) " + 
					"Order By Category", 
					Rno);
				DataTable dtSimlar = db.DataTable(Sql);
				if (dtSimlar.Rows.Count == 0)
				{
					lblSimilar.Visible = false;
				}
				else
				{
					lblSimilar.Visible = true;
					lblSimilar.Text = string.Format("{0} more categories", dtSimlar.Rows.Count);

					string Tip = string.Empty;
					ulSimilar.Controls.Clear();
					foreach (DataRow drSimilar in dtSimlar.Rows)
					{
						string Category = DB.Str(drSimilar["Category"]);
						Tip += string.Format("<li>{0}</li>\n", Category);

						HtmlAnchor a = new HtmlAnchor();
						a.HRef = string.Format("SetupMenuItems.aspx?Rno={0}", DB.Int32(drSimilar["MenuItemRno"]));
						a.InnerText = Category;
						HtmlGenericControl li = new HtmlGenericControl("li");
						li.Controls.Add(a);
						ulSimilar.Controls.Add(li);

						if (DB.Int32(drSimilar["RecipeRno"]) > 0)
						{
							fSimilarMenuItemsWithRecipe = true;
						}
					}
					lblSimilar.ToolTip = string.Format("<ul>{0}</ul>", Tip);
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
		int Rno = Str.Num(txtRno.Value);
		string Category = txtCurrCategory.Value;
		string MenuItem = txtCurrMenuItem.Value;
		string Sql = string.Empty;

		try
		{
			// setup to check if menu items need to be merged
			Sql = string.Format("Select Count(*) From mcJobMenuItems Where Category = {0} And MenuItem = {1}", DB.PutStr(ddlCategory.Text), DB.PutStr(txtMenuItem.Text));

			// if user has said yes to merging
			if (hfMerge.Value == "true")
			{
				Sql = string.Format(
					"Begin Transaction\n" +
					"Update mcJobFood Set Category = {2}, MenuItem = {3} Where Category = {0} And MenuItem = {1}\n" +
					"Delete From mcJobMenuItems Where Category = {0} And MenuItem = {1}\n" +
					"Commit Transaction",
					DB.PutStr(Category),
					DB.PutStr(MenuItem),
					DB.PutStr(ddlCategory.Text),
					DB.PutStr(txtMenuItem.Text));
				//Response.Write("Sql " + Sql + "<br />");
				db.Exec(Sql);

				fStartHere = true;
				StartHere = ddlCategory.Text + " - " + txtMenuItem.Text;
				hfMerge.Value = "false";
			}
			else

			// if not a new record and the category or menu item name has changed and the new value already exists then ask if they intend to merge
			if ((Category.Length > 0 && Category != ddlCategory.Text ||
				 MenuItem.Length > 0 && MenuItem != txtMenuItem.Text) && db.SqlNum(Sql) > 0)
			{
				fMerging = true;
				MergeMenuItem = MenuItem;
				MergeIntoMenuItem = txtMenuItem.Text;
				MergeCategory = Category;
				MergeIntoCategory = ddlCategory.Text;

				Sql = string.Format(
					"Select Count(*) From mcJobFood Where Category = {0} And MenuItem = {1}",
					DB.PutStr(Category),
					DB.PutStr(MenuItem));
				AffectedJobs = Fmt.Num(db.SqlNum(Sql));
			}
			else // not merging, simple update
			{
				DateTime Tm = DateTime.Now;

				// if a new menu item
				if (Rno == 0)
				{
					// if the new menu item already exists
					if (db.SqlNum(Sql) > 0)
					{
						fError = true;
						ErrorMsg = string.Format("There is already a menu item in this category named <b>{0}</b>. Please choose another name.", txtMenuItem.Text);
					}
					else
					{
						Sql = string.Format(
							"Insert Into mcJobMenuItems (MenuItem, CategorySortOrder, CreatedDtTm, CreatedUser) Values " +
							"('temp', (Select Count(*) + 1 From mcJobMenuItems Where Category = {2} And (HideFlg = 0 Or HideFlg Is Null)), {0}, {1}); Select @@Identity",
							DB.PutDtTm(Tm),
							DB.PutStr(g.User),
							DB.PutStr(ddlCategory.Text));
						Rno = db.SqlNum(Sql);

						Category = ddlCategory.Text;
						MenuItem = txtMenuItem.Text;
					}
				}
				// if there wasn't an error
				if (!fError)
				{
					Misc.SetAsIsData(Rno, chkAsIs.Checked);

					// update the values
					Sql = string.Format(
						"Begin Transaction\n" +
						"Update mcJobMenuItems Set " +
						"Category = {1}, " +
						"MenuItem = {2}, " +
						"ProposalMenuItem = {3}, " +
						"ServingQuote = {4}, " +
						"KitchenLocRno = {5}, " +
						"AsIsFlg = {6}, " + 
						"MultSelFlg = {7}, " +
						"MultItems = {8}, " +
						"IngredSelFlg = {9}, " +
						"HideFlg = {10}, " +
						"UpdatedDtTm = {11}, " +
						"UpdatedUser = {12} " +
						"Where MenuItemRno = {0}",
						Rno,
						DB.PutStr(ddlCategory.Text),
						DB.PutStr(txtMenuItem.Text),
						DB.PutStr(txtProposal.Text),
						Str.Dec(txtServingQuote.Text),
						ddlLocation.SelectedValue,
						DB.PutBool(chkAsIs.Checked),
						DB.PutBool(chkMultiSelect.Checked),
						DB.PutStr(txtMultItems.Value),
						DB.PutBool(chkIngredSelect.Checked),
						DB.PutBool(chkHide.Checked),
						DB.PutDtTm(Tm),
						DB.PutStr(g.User));

					// if changed, update other areas
					if (Category != ddlCategory.Text || MenuItem != txtMenuItem.Text)
					{
						Sql += string.Format(
							"Update mcJobFood Set Category = {2}, MenuItem = {3} Where Category = {0} And MenuItem = {1}\n",
							DB.PutStr(Category),
							DB.PutStr(MenuItem),
							DB.PutStr(ddlCategory.Text),
							DB.PutStr(txtMenuItem.Text));
					}

					Sql += "Commit Transaction";
					//Response.Write("Sql " + Sql + "<br />");
					db.Exec(Sql);

					fStartHere = true;
					StartHere = ddlCategory.Text + " - " + txtMenuItem.Text;
				}
			}
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}

	protected void btnUpdate_Click(object sender, EventArgs e)
	{
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
		btnUpdate.Enabled =
		btnNext.Enabled = true;
	}

	protected void btnDelete_Click(object sender, EventArgs e)
	{
		string Sql = String.Format(
			"Delete From mcJobMenuItems Where Category = {0} And MenuItem = {1}",
			DB.PutStr(ddlCategory.Text), 
			DB.PutStr(txtMenuItem.Text));

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
		if (!fMerging && !fError)
		{
			//ClearData();
			LoadList(fNext);
			if (chkShowNew.Checked && lstList.Items.Count > 0)
			{
				int Rno = Str.Num(lstList.Items[0].Value);
				GetData(Rno);
			}
		}
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
	
	protected void UpdateList(object sender, System.EventArgs e)
	{
		LoadList();
		Session["FoodSortedBy"] = (rbSortCategory.Checked ? "Category" : "MenuItem");
	}

	private void LoadList()
	{
		LoadList(false);
	}

	private void LoadList(bool fNext)
	{
		string Sql = "";
		string SqlSelect = "Select MenuItemRno, Category, MenuItem, HideFlg From mcJobMenuItems ";
		string SqlOrderBy;
		string List = "";
		string Hidden = (chkShowHidden.Checked ? string.Empty : " IsNull(HideFlg, 0) = 0 ");
		string New = (chkShowNew.Checked ? " UpdatedDtTm Is Null" : "");
		string Where = " Category In (Select Category From mcJobMenuCategories Where IsNull(HideFlg, 0) = 0)";
		Where = DB.And(Where, Hidden);
		Where = DB.And(Where, New);

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

        if (rbSortCategory.Checked)
		{
			SqlOrderBy = "Order By Category, MenuItem";

			//FoodCategories FoodCat = new FoodCategories();
			//for (
			//    string Category = FoodCat.FirstCategory(FoodCategories.Type.Hot);
			//    Category != null;
			//    Category = FoodCat.NextCategory())
			//{
			//    Sql = SqlSelect + "Where Category = " + DB.PutStr(Category) + (Where.Length > 0 ? " And" + Where : "") + " " + SqlOrderBy;
			//    LoadListSql(Sql, CurrItem);
			//}
			//for (
			//    string Category = FoodCat.FirstCategory(FoodCategories.Type.Cold);
			//    Category != null;
			//    Category = FoodCat.NextCategory())
			//{
			//    Sql = SqlSelect + "Where Category = " + DB.PutStr(Category) + (Where.Length > 0 ? " And" + Where : "") + " " + SqlOrderBy;
			//    LoadListSql(Sql, CurrItem);
			//}

			//string Hot = FoodCat.SqlList(FoodCategories.Type.Hot);
			//string Cold = FoodCat.SqlList(FoodCategories.Type.Cold);
			//if (Hot.Length > 0)		List += (List.Length > 0 ? ", " : "") + Hot;
			//if (Cold.Length > 0)	List += (List.Length > 0 ? ", " : "") + Cold;
        }
		else
		{
			SqlOrderBy = "Order By MenuItem, Category";
		}

		Sql = SqlSelect;
        if (List.Length > 0)
        {
			Sql += "Where Category Not In (" + List + ")" + (Where.Length > 0 ? " And" + Where : "") + " ";
        }
        else
        {
			Sql += (Where.Length > 0 ? " Where" + Where : "");
        }
		Sql += " " + SqlOrderBy;

		//Response.Write(Sql + "<br />");

		//LoadListSql(Sql, CurrItem, NextCurrItem);
		LoadListSql(Sql, NextCurrItem);
	}

	//private void LoadListSql(string Sql, string CurrItem, string NextCurrItem)
	private void LoadListSql(string Sql, string NextCurrItem)
	{
		//Response.Write("looking for [" + NextCurrItem + "]<br/>");
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				string Category = DB.Str(r["Category"]);
				string MenuItem = DB.Str(r["MenuItem"]);
				int Rno = DB.Int32(r["MenuItemRno"]);
				bool fHidden = DB.Bool(r["HideFlg"]);
				ListItem Item = new ListItem(Category + " - " + MenuItem, Rno.ToString());
				Item.Attributes.Add("title", Category + " - " + MenuItem);
				if (fHidden) Item.Attributes.Add("class", "Hidden");
				/*
				Item.Selected = (Item.Value == CurrItem);
				if (Item.Selected)
				{
					Rno = Str.Num(Item.Value);
					GetData(Rno);
					btnUpdate.Enabled = true;

					NextCurrItem = string.Empty;
				}
				else
				{
				*/
				//Response.Write("Item [" + Item.Text + "]<br/>");
					if (Item.Text == NextCurrItem)
					{
						//Response.Write("Found it [" + Item.Text + "]<br/>");
						Item.Selected = true;

						Rno = Str.Num(Item.Value);
						GetData(Rno);
						btnUpdate.Enabled =
						btnNext.Enabled = true;
					}
				//}

				lstList.Items.Add(Item);
			}

			lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Menu Items";
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}
}