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

public partial class SetupMenuCategories : System.Web.UI.Page
{
	protected WebPage Pg;
	private DB db;
	protected String FocusField = "";

	protected int NumMenuItems;
	protected bool fMerging = false;
	protected string MergeCategory;
	protected string MergeIntoCategory;

	private bool fStartHere = false;
	private string StartHere;
	protected string ErrorMsg = string.Empty;
	protected bool fError = false;

	private void Page_Load(object sender, System.EventArgs e)
	{
		db = new DB();

        Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
            Session["Menu"] = WebPage.MenuSetup.Title;
            ClearData();
			Setup();
		}
	}

    private void Setup()
    {
		rbSortMenuOrder.Checked = true;
		btnUpdate.Enabled =
		btnNext.Enabled = true;

        LoadList();
    }
    
	protected void ClearData()
	{
		lstList.SelectedIndex = -1;
		txtCurrCategory.Value =
		txtCategory.Text = string.Empty;
		chkMultiSelect.Checked = 
		chkHide.Checked = false;
		hfMerge.Value = "false";
		btnDelete.Enabled = false;
		btnDelete.ToolTip = "New category record";
	}

    protected void GetData(string Category)
    {
		string Sql = string.Format("Select * From mcJobMenuCategories Where Category = {0}", DB.PutStr(Category));

		ClearData();

        try
        {
            DataRow dr = db.DataRow(Sql);
            if (dr != null)
            {
				txtCurrCategory.Value =
				txtCategory.Text = DB.Str(dr["Category"]);
				chkMultiSelect.Checked = DB.Bool(dr["MultSelFlg"]);
                chkHide.Checked = DB.Bool(dr["HideFlg"]);
				txtCreatedDt.Text = Fmt.DtTm(DB.DtTm(dr["CreatedDtTm"]));
				txtCreatedUser.Text = DB.Str(dr["CreatedUser"]);
				txtUpdatedDt.Text = Fmt.DtTm(DB.DtTm(dr["UpdatedDtTm"]));
				txtUpdatedUser.Text = DB.Str(dr["UpdatedUser"]);
			}

			Sql = string.Format("Select * From mcJobMenuItems Where Category = {0} And (HideFlg = 0 Or HideFlg Is Null) Order By CategorySortOrder, MenuItem", DB.PutStr(txtCategory.Text));
			DataTable dt = db.DataTable(Sql);
			int i = 0;
			foreach (DataRow drMenuItem in dt.Rows)
			{
				HtmlGenericControl li = new HtmlGenericControl("li");

				HtmlGenericControl sp = new HtmlGenericControl("span");
				sp.Attributes.Add("class", "ui-icon ui-icon-arrowthick-2-n-s");
				li.Controls.Add(sp);

				HtmlInputHidden hf = new HtmlInputHidden();
				hf.ID = string.Format("hfMenuItem{0}", i);
				hf.Value = DB.Int32(drMenuItem["MenuItemRno"]).ToString();
				li.Controls.Add(hf);

				hf = new HtmlInputHidden();
				hf.ID = string.Format("hfSortOrder{0}", i);
				hf.Value = DB.Int32(drMenuItem["CategorySortOrder"]).ToString();
				hf.Attributes.Add("class", "SortOrder");
				li.Controls.Add(hf);

				HtmlGenericControl lbl = new HtmlGenericControl("label");
				lbl.InnerText = DB.Str(drMenuItem["MenuItem"]);
				li.Controls.Add(lbl);

				ulItems.Controls.Add(li);

				i++;
			}
			NumMenuItems = i;

			Sql = string.Format("Select Count(*) From mcJobFood Where Category = {0}", DB.PutStr(txtCategory.Text));
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
        }
        catch (Exception Ex)
        {
            Err Err = new Err(Ex, Sql);
            Response.Write(Err.Html());
        }
    }

	protected void SaveData()
	{
		string Category = txtCurrCategory.Value;
		string Sql = string.Empty;

		try
		{
			// setup to check if categories need to be merged
			Sql = string.Format("Select Count(*) From mcJobMenuCategories Where Category = {0}", DB.PutStr(txtCategory.Text));


			// if user has said yes to merging
			if (hfMerge.Value == "true")
			{
				Sql = string.Format(
					"Begin Transaction\n" +
					"Update mcJobFood Set Category = {1} Where Category = {0}\n" +
					"Update mcJobMenuItems Set Category = {1} Where Category = {0}\n" +
					"Delete From mcJobMenuCategories Where Category = {0}\n" +
					"Commit Transaction",
					DB.PutStr(Category), 
					DB.PutStr(txtCategory.Text));
				db.Exec(Sql);

				fStartHere = true;
				StartHere = txtCategory.Text;
				hfMerge.Value = "false";
			}
			else

			// if not a new record and the category name has changed and the new category already exists then ask if they intend to merge
			if (Category.Length > 0 && Category != txtCategory.Text && db.SqlNum(Sql) > 0)
			{
				fMerging = true;
				MergeCategory = Category;
				MergeIntoCategory = txtCategory.Text;
			}
			else // not merging, simple update
			{
				DateTime Tm = DateTime.Now;

				// if a new category (curr category name is blank)
				if (Category.Length == 0)
				{
					// if new category already exists
					if (db.SqlNum(Sql) > 0)
					{
						fError = true;
						ErrorMsg = string.Format("There is already a category named <b>{0}</b>. Please choose another name.", txtCategory.Text);
					}
					else
					{
						Sql = string.Format(
							"Insert Into mcJobMenuCategories (Category, SortOrder, CreatedDtTm, CreatedUser) Values ({0}, (Select Count(*) + 1 From mcJobMenuCategories), {1}, {2})",
							DB.PutStr(txtCategory.Text),
							DB.PutDtTm(Tm),
							DB.PutStr(g.User));
						db.SqlNum(Sql);

						Category = txtCategory.Text;
					}
				}

				// if there wasn't an error
				if (!fError)
				{
					// update the values
					Sql = string.Format(
						"Begin Transaction\n" +
						"Update mcJobMenuCategories Set " +
						"Category = {1}, " +
						"MultSelFlg = {2}, " +
						"HideFlg = {3}, " +
						"UpdatedDtTm = {4}, " +
						"UpdatedUser = {5} " +
						"Where Category = {0}\n",
						DB.PutStr(Category),
						DB.PutStr(txtCategory.Text),
						DB.PutBool(chkMultiSelect.Checked),
						DB.PutBool(chkHide.Checked),
						DB.PutDtTm(Tm),
						DB.PutStr(g.User));

					// if changed, update other areas
					if (Category != txtCategory.Text)
					{
						Sql += string.Format(
							"Update mcJobFood Set Category = {1} Where Category = {0}\n" +
							"Update mcJobMenuItems Set Category = {1} Where Category = {0}\n",
							DB.PutStr(Category),
							DB.PutStr(txtCategory.Text));
					}

					Sql += "Commit Transaction";
					db.Exec(Sql);

					fStartHere = true;
					StartHere = txtCategory.Text;
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
		Response.Write(string.Format("UpdateNext SelectedIndex {0}  Count {1} <br />", lstList.SelectedIndex, lstList.Items.Count));
		Update(true);
	}

	protected void btnNew_Click(object sender, EventArgs e)
	{
		ClearData();
		LoadList();
		btnUpdate.Enabled =
		btnNext.Enabled = true;
		FocusField = "txtCategory";
	}

	protected void btnDelete_Click(object sender, EventArgs e)
	{
		string Sql = String.Format(
			"Delete From mcJobMenuCategories Where Category = {0};\n" +
			"Delete From mcJobMenuItems Where Category = {0};",
			DB.PutStr(txtCategory.Text));
		try
		{
			db.Exec(Sql);
			ClearData();
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
		}
	}

	protected void lstList_SelectedIndexChanged(object sender, System.EventArgs e)
	{
        string Category = lstList.SelectedItem.Value;
        //GetData(Category);
		fStartHere = true;
		StartHere = Category;
		LoadList();
		btnUpdate.Enabled =
		btnNext.Enabled = true;
	}
	
	protected void UpdateList(object sender, System.EventArgs e)
	{
		LoadList();
		Session["CategorySortedBy"] = (rbSortAlpha.Checked ? "Alpha" : "Sort Order");
	}

	private void LoadList()
	{
		LoadList(false);
	}

	private void LoadList(bool fNext)
	{
		string Sql = "Select Category, HideFlg From mcJobMenuCategories ";
		string SqlOrderBy;
        string Hidden = (chkShowHidden.Checked ? string.Empty : " (HideFlg Is Null Or HideFlg = 0) ");
		string Where = "";
		if (Hidden.Length > 0)	Where += (Where.Length > 0 ? " And" : string.Empty) + Hidden;

        string CurrCategory = (lstList.SelectedIndex >= 0 ? lstList.SelectedItem.Value : null);
		string NextCurrCategory = (lstList.SelectedIndex >= 0 && lstList.SelectedIndex + 1 < lstList.Items.Count ? lstList.Items[lstList.SelectedIndex + 1].Value : null);
		if (!fNext && CurrCategory != null && CurrCategory.Length > 0)
		{
			NextCurrCategory = CurrCategory;
		}

		// if merging (or for other reasons), end up on merged into category
		if (!fNext && fStartHere)
		{
			NextCurrCategory = StartHere;
		}

		lstList.Items.Clear();

        if (rbSortAlpha.Checked)
		{
			SqlOrderBy = "Order By Category";
        }
		else
		{
			SqlOrderBy = "Order By SortOrder";
		}

		Sql += (Where.Length > 0 ? " Where" + Where : "") + " " + SqlOrderBy;

		LoadListSql(Sql, NextCurrCategory);
	}

	private void LoadListSql(string Sql, string NextCurrItem)
	{
		try
		{
			DataTable dt = db.DataTable(Sql);
			foreach (DataRow r in dt.Rows)
			{
				string Category = DB.Str(r["Category"]);
				bool fHidden = DB.Bool(r["HideFlg"]);
				ListItem Item = new ListItem(Category);
				if (fHidden) Item.Attributes.Add("class", "Hidden");

				if (Item.Value == NextCurrItem)
				{
					Item.Selected = true;

					Category = Item.Value;
					GetData(Category);
					//btnUpdate.Enabled =
					//btnNext.Enabled = true;
				}


				lstList.Items.Add(Item);
			}

			lblRecCount.Text = Fmt.Num(lstList.Items.Count, true) + " Categories";
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}
}