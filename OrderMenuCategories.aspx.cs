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

public partial class OrderMenuCategories : System.Web.UI.Page
{
	protected WebPage Pg;
	protected String FocusField = string.Empty;
	protected string NewHtml = string.Empty;

	private void Page_Load(object sender, System.EventArgs e)
	{
		Pg = new WebPage("Images");
        Pg.CheckLogin(Path.GetFileNameWithoutExtension(Page.AppRelativeVirtualPath));

        // Put user code to initialize the page here
        if (!Page.IsPostBack)
		{
			Session["Menu"] = WebPage.MenuSetup.Title;
		}
		else
		{
			SaveUpdates();
		}
	}

	private void Page_PreRender(object sender, System.EventArgs e)
	{
		string Sql = "Select * From mcJobMenuCategories Where (HideFlg = 0 Or HideFlg Is Null) Order By SortOrder";
		try
		{
			DB db = new DB();
			DataTable dt = db.DataTable(Sql);

			hfNumCats.Value = dt.Rows.Count.ToString();
			int i = 0;

			foreach (DataRow dr in dt.Rows)
			{
				string Category = DB.Str(dr["Category"]);
				int SortOrder = DB.Int32(dr["SortOrder"]);
				//bool fMultiSelect = DB.Bool(dr["MultSelFlg"]);
				//bool fHide = DB.Bool(dr["HideFlg"]);

				//ulCats.Controls.Add(EditCategory(i.ToString(), Category, SortOrder, fMultiSelect, fHide));
				ulCats.Controls.Add(EditCategory(i.ToString(), Category, SortOrder));
				i++;
			}
			db.Close();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}

		//NewHtml = Misc.ToString(EditCategory("xxxx", "New Category", 0, false, false));
		NewHtml = Misc.ToString(EditCategory("xxxx", "New Category", 0));
	}

	//private HtmlGenericControl EditCategory(string i, string Category, int SortOrder, bool fMultiSelect, bool fHide)
	private HtmlGenericControl EditCategory(string i, string Category, int SortOrder)
	{
		HtmlGenericControl li = new HtmlGenericControl("li");

		HtmlGenericControl sp = new HtmlGenericControl("span");
		sp.Attributes.Add("class", "ui-icon ui-icon-arrowthick-2-n-s");
		li.Controls.Add(sp);

		HtmlInputHidden hf = new HtmlInputHidden();
		hf.ID = string.Format("hfCategory{0}", i);
		hf.Value = Category;
		li.Controls.Add(hf);

		hf = new HtmlInputHidden();
		hf.ID = string.Format("hfSortOrder{0}", i);
		hf.Value = SortOrder.ToString();
		hf.Attributes.Add("class", "SortOrder");
		li.Controls.Add(hf);

		HtmlGenericControl lbl = new HtmlGenericControl("label");
		lbl.InnerText = Category;
		li.Controls.Add(lbl);

		//HtmlInputText txt = new HtmlInputText();
		//txt.ID = string.Format("txtCategory{0}", i);
		//txt.Value = Category;
		//txt.Style.Add("display", "none");
		//li.Controls.Add(txt);

		//HtmlInputCheckBox chk = new HtmlInputCheckBox();
		//chk.ID = string.Format("chkMultSel{0}", i);
		//chk.Checked = fMultiSelect;
		//li.Controls.Add(chk);

		//chk = new HtmlInputCheckBox();
		//chk.ID = string.Format("chkHide{0}", i);
		//chk.Checked = fHide;
		//li.Controls.Add(chk);

		return li;
	}

	private void SaveUpdates()
	{
		string Sql = string.Empty;
		try
		{
			DB db = new DB();

			int cCats = Str.Num(hfNumCats.Value);
			for (int i = 0; i < cCats; i++)
			{
				string Category = Parm.Str(string.Format("hfCategory{0}", i));
				string NewCategory = Parm.Str(string.Format("txtCategory{0}", i));

				if (Category.Length > 0 || Category.Length == 0 && NewCategory.Length == 0)
				{
					//Sql = string.Format("Update mcJobMenuCategories Set SortOrder = {1}, MultSelFlg = {2}, HideFlg = {3}, UpdatedDtTm = GetDate(), UpdatedUser = {4} Where Category = {0}", 
					//	DB.PutStr(Category),
					//	Parm.Int(string.Format("hfSortOrder{0}", i)),
					//	(Parm.Bool(string.Format("chkMultSel{0}", i)) ? 1 : 0),
					//	(Parm.Bool(string.Format("chkHide{0}", i)) ? 1 : 0),
					//	DB.PutStr(g.User));
					Sql = string.Format("Update mcJobMenuCategories Set SortOrder = {1}, UpdatedDtTm = GetDate(), UpdatedUser = {2} Where Category = {0}",
						DB.PutStr(Category),
						Parm.Int(string.Format("hfSortOrder{0}", i)),
						DB.PutStr(g.User));
					db.Exec(Sql);

					// has the category text itself changed?
					if (NewCategory != Category && NewCategory.Length > 0)
					{
						// has changed, update category values
						Sql = string.Format(
							"Update mcJobMenuCategories Set Category = {1} Where Category = {0}; " +
							"Update mcJobMenuItems Set Category = {1}, UpdatedDtTm = GetDate(), UpdatedUser = {2} Where Category = {0}; ", 
							DB.PutStr(Category),
							DB.PutStr(NewCategory),
							DB.PutStr(g.User));
						db.Exec(Sql);
					}
				}
				else
				{
					//if (NewCategory.Length > 0)
					{
						Sql = string.Format("Insert Into mcJobMenuCategories (Category, SortOrder, MultSelFlg, HideFlg, CreatedDtTm, CreatedUser) Values ({0}, {1}, {2}, {3}, GetDate(), {4})",
							DB.PutStr(NewCategory),
							Parm.Int(string.Format("hfSortOrder{0}", i)),
							(Parm.Bool(string.Format("chkMultSel{0}", i)) ? 1 : 0),
							(Parm.Bool(string.Format("chkHide{0}", i)) ? 1 : 0),
							DB.PutStr(g.User));
						db.Exec(Sql);
					}
				}
			}
			db.Close();
		}
		catch (Exception Ex)
		{
			Err Err = new Err(Ex, Sql);
			Response.Write(Err.Html());
		}
	}
}